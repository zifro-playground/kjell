using UnityEngine;

namespace Kjell
{
	public class IOStream : MonoBehaviour
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
			var printObject = GameObject.Instantiate(PrintPrefab);
			
		}
	}
}
