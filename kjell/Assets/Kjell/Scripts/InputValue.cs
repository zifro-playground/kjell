using PM;
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

		public void SubmitInput(string message)
		{
			SubmittedText.text = message;
			InputText.text = message;
			InputField.SetActive(false);
			SendButton.SetActive(false);
			SubmittedText.gameObject.SetActive(true);

			IOStream.Instance.InputSubmitted(SubmittedText.text);
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
