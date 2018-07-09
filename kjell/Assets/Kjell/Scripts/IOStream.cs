using System;
using System.Collections;
using System.Collections.Generic;
using PM;
using UnityEngine;


namespace Kjell
{
	public class IOStream : MonoBehaviour, IPMCompilerStarted, IPMLevelChanged, IPMCompilerStopped, IPMLineParsed, IPMActivateWalker
	{
		public string LatestReadInput;

		public GameObject PrintPrefab;
		public GameObject LabelPrefab;
		public GameObject ValuePrefab;

		public Sprite InputLabelPop;
		public Sprite InputValuePop;
		public Sprite InputLabelPlain;
		public Sprite InputValuePlain;

		private GameObject labelObject;
		private GameObject valueObject;

		public Dictionary<int, string> LinesWithInput;

		public static IOStream Instance;

		private void Start()
		{
			if (Instance == null)
				Instance = this;
		}

		public void Print(string message)
		{
			CaseCorrection.NextOutput(message);

			var outputObject = Instantiate(PrintPrefab);
			outputObject.transform.SetParent(gameObject.transform, false);
			var output = outputObject.GetComponent<Output>();
			message = message.Replace("\\n", "\n");
			output.Text.text = message;
		}

		public IEnumerator CallInput(int lineNumber)
		{
			yield return new WaitForSeconds(PMWrapper.walkerStepTime * (1 - PMWrapper.speedMultiplier));

			if (!PMWrapper.IsCompilerRunning)
				yield break;

			IDELineMarker.SetWalkerPosition(lineNumber + 1);
			if (!LinesWithInput.ContainsKey(lineNumber))
				throw new Exception("There is no input on line " + lineNumber);

			var argument = InputParser.InterpretArgument(LinesWithInput[lineNumber]);
			StartCoroutine(TriggerInput(argument));
		}

		public IEnumerator TriggerInput(string message)
		{
			labelObject = Instantiate(LabelPrefab);
			labelObject.transform.SetParent(gameObject.transform, false);
			labelObject.GetComponent<InputLabel>().Text.text = message;
			labelObject.GetComponent<InputLabel>().BubbleImage.sprite = InputLabelPop;

			yield return new WaitForSeconds(2 * (1 - PMWrapper.speedMultiplier));

			valueObject = Instantiate(ValuePrefab);
			valueObject.transform.SetParent(gameObject.transform, false);
			valueObject.GetComponent<InputValue>().BubbleImage.sprite = InputValuePop;

			CaseCorrection.NextInput(valueObject);
		}

		public void InputSubmitted(string submitedText)
		{
			LatestReadInput = submitedText;
			labelObject.GetComponent<InputLabel>().BubbleImage.sprite = InputLabelPlain;
			valueObject.GetComponent<InputValue>().BubbleImage.sprite = InputValuePlain;
		}

		private void Clear()
		{
			foreach (Transform child in transform)
			{
				Destroy(child.gameObject);
			}
		}

		private void SubmitLastInput()
		{
			if (gameObject.transform.childCount > 0)
			{
				var inputValue = gameObject.transform.GetChild(gameObject.transform.childCount - 1).gameObject.GetComponent<InputValue>();
				if (inputValue != null)
					inputValue.SubmitInput();
			}
		}

		public void OnPMCompilerStarted()
		{
			Clear();
			LinesWithInput = InputParser.FindInputInCode(PMWrapper.fullCode);
		}

		public void OnPMLevelChanged()
		{
			SubmitLastInput();
			Clear();
		}

		public void OnPMCompilerStopped(HelloCompiler.StopStatus status)
		{
			SubmitLastInput();
			StopAllCoroutines();
		}

		public void OnPMLineParsed()
		{
			if (LinesWithInput.ContainsKey(PMWrapper.CurrentLineNumber + 1))
			{
				StartCoroutine(CallInput(PMWrapper.CurrentLineNumber + 1));
				PMWrapper.IsWaitingForUserInput = true;
			}
		}

		public void OnPMActivateWalker()
		{
			if (LinesWithInput.ContainsKey(0))
			{
				StartCoroutine(CallInput(0));
				PMWrapper.IsWaitingForUserInput = true;
			}
		}
	}
}
