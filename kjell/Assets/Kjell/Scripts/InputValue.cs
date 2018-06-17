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

        public void SubmitInput()
        {
            SubmittedText.text = InputText.text;
            InputField.SetActive(false);
            SendButton.SetActive(false);
            SubmittedText.gameObject.SetActive(true);
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.KeypadEnter) || Input.GetKeyDown(KeyCode.Return))  
            {
                SubmitInput();
            }
        }
    }

    
}
