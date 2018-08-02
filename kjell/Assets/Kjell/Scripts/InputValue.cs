using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace Kjell
{
	public class InputValue : MonoBehaviour
	{
		public Text SubmittedText;
		public Text InputText;

		public GameObject InputField;
		public GameObject SendButton;

		public Image BubbleImage;

		private void Update()
		{
			if (Input.GetKeyDown(KeyCode.KeypadEnter) || Input.GetKeyDown(KeyCode.Return))
			{
				SubmitInput();
			}
		}

		public void SubmitInput()
        {
			if (!string.IsNullOrEmpty(InputText.text))
				SubmittedText.text = InputText.text;

            InputField.SetActive(false);
            SendButton.SetActive(false);
            SubmittedText.gameObject.SetActive(true);

            IOStream.Instance.InputSubmitted(SubmittedText.text);
        }

		public IEnumerator StartInputAnimation(string message)
		{
			foreach (var character in message)
			{
				InputField.GetComponent<InputField>().text += character;
				InputField.GetComponent<InputField>().caretPosition = message.Length;

				yield return new WaitForSeconds((1 - PMWrapper.speedMultiplier) * 0.4f);
			}

			yield return new WaitForSeconds((1 - PMWrapper.speedMultiplier) * 2);

			SubmitInput();
		}

		public void DeactivateInputValue()
		{
			if (!string.IsNullOrEmpty(InputText.text))
				SubmittedText.text = InputText.text;

			InputField.SetActive(false);
			SendButton.SetActive(false);
			SubmittedText.gameObject.SetActive(true);
		}
    }

    
}
