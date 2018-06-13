using UnityEngine;

namespace Kjell
{
	public class IOStream : MonoBehaviour, IPMCompilerStarted, IPMLevelChanged
	{
		public GameObject PrintPrefab;
		public GameObject LabelPrefab;
		public GameObject ValuePrefab;

		public static IOStream Instance;

		private void Start()
		{
			if (Instance == null)
				Instance = this;
		}

		public void Print(string message)
		{
			var outputObject = Instantiate(PrintPrefab);
			outputObject.transform.SetParent(gameObject.transform, false);

			var output = outputObject.GetComponent<Output>();
			output.Text.text = message;
		}

		public void Input(string message)
		{

		}

		public void OnPMCompilerStarted()
		{
			Clear();
		}

		public void OnPMLevelChanged()
		{
			Clear();
		}

		private void Clear()
		{
			foreach (Transform child in transform)
			{
				Destroy(child.gameObject);
			}
		}
	}
}
