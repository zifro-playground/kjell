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
        public Sprite PlainValueSprite;
        public Image InputLabelImage;
        public Sprite PlainLabelSprite;

        public void SubmitInput()
        {
            SubmittedText.text = InputText.text;
            InputField.SetActive(false);
            SendButton.SetActive(false);
            SubmittedText.gameObject.SetActive(true);

            //BubbleImage.sprite = PlainValueSprite;
            //InputLabelImage.sprite = PlainLabelSprite;

            IOStream.Instance.InputSubmitted(SubmittedText.text);
	        //LatestReadInput = ;
	        CodeWalker.IsWaitingForInput = false;
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
