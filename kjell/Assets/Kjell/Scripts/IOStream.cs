using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Compiler;
using PM;
using UnityEngine;


namespace Kjell
{
	public class IOStream : MonoBehaviour, IPMCompilerStarted, IPMLevelChanged, IPMCompilerStopped
	{
		public string LatestReadInput;

		public GameObject PrintPrefab;
		public GameObject LabelPrefab;
		public GameObject ValuePrefab;
        public Sprite InputLabelPop;
        public Sprite InputValuePop;
        public Sprite InputLabelPlain;
        public Sprite InputValuePlain;
        public GameObject labelObject;
        public GameObject valueObject;

		public Dictionary<int, string> LinesWithInput;

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
            message = message.Replace("\\n","\n");
            output.Text.text = message;
            
        }

		// Parse code and find what lines makes a function call
		private void FindInputInCode(string code)
		{
			LinesWithInput = new Dictionary<int, string>();

			var lines = code.Split('\n');
			var lineIndex = 0;
			foreach (var line in lines)
			{
                if(!line.Trim().StartsWith("#") && line.Trim() != "")
                {
                    var inputArgument = ParseInputArgument(line);
                    if (inputArgument != null)
                        LinesWithInput[lineIndex] = inputArgument;
                    lineIndex++;
                }
				
			}
		}

		// Parse a line of code, find input call and extract and return its parameter. Return null if no input in line.
		private string ParseInputArgument(string line)
		{
			var charIndex = 6;
			var inputArgument = "";
			var inputInLine = false;
			var parenthesisCount = -1;

			while (charIndex < line.Length)
			{
                if (line[charIndex - 6] == '#') // oklart (för att bortse från kommentarer)
                    break;


                if (line[charIndex - 6] == 'i' && line[charIndex - 5] == 'n' && line[charIndex - 4] == 'p' &&
				    line[charIndex - 3] == 'u' && line[charIndex - 2] == 't' && line[charIndex - 1] == '(')
				{
					inputInLine = true;
					parenthesisCount = 0;

					while (parenthesisCount >= 0 && charIndex < line.Length)
					{
						if (line[charIndex] == '(')
							parenthesisCount++;
						else if (line[charIndex] == ')')
							parenthesisCount--;

						if (parenthesisCount >= 0)
							inputArgument += line[charIndex];

						charIndex++;
					}
				}
				else
				{
					charIndex++;
				}

				if (inputInLine && charIndex < line.Length && !char.IsWhiteSpace(line[charIndex]))
				{
                    print(line[charIndex]);
					if (line[charIndex] == ')')
						PMWrapper.RaiseError(CodeWalker.CurrentLineNumber, "Fel vid input. Det är för många slutparenteser.");
                    else if (line[charIndex] == '#') //makes you able to have comments after a line (but the compiler (PM) says its wrong)
                        break;
                    else
                        PMWrapper.RaiseError(CodeWalker.CurrentLineNumber, "Fel vid input. Det får inte vara några tecken efter input().");
				}
			}
			if (parenthesisCount >= 0)
				PMWrapper.RaiseError(CodeWalker.CurrentLineNumber, "Fel antal parenteser. Det är fler startparenteser än slutparenteser");

			if (inputInLine)
				return inputArgument;

			return null;
		}

		public string InterpretArgument(string argument)
		{
			if (string.IsNullOrEmpty(argument))
				return "";

			var lines = Compiler.SyntaxCheck.parseLines(argument);
			var words = lines.First().words;
			var logic = WordsToLogicParser.determineLogicFromWords(words, 1, Compiler.SyntaxCheck.MainScopeCopy);
			var variable = SumParser.parseIntoSum(logic, 1, Compiler.SyntaxCheck.MainScopeCopy);

			var label = "";
			if (variable.variableType == VariableTypes.textString)
				label = variable.getString();
			else if (variable.variableType == VariableTypes.number)
				label = variable.getNumber().ToString();
			else if (variable.variableType == VariableTypes.boolean)
				label = variable.getBool().ToString();

			return label;
		}

        public IEnumerator CallInput(int lineNumber)
		{

            yield return new WaitForSeconds(PMWrapper.walkerStepTime * PMWrapper.speedMultiplier);

            IDELineMarker.SetWalkerPosition(lineNumber+1);
            if (!LinesWithInput.ContainsKey(lineNumber))
				throw new Exception("There is no input on line " + lineNumber);

			var argument = InterpretArgument(LinesWithInput[lineNumber]);
			TriggerInput(argument);
		}

		public void TriggerInput(string message)
		{
			labelObject = Instantiate(LabelPrefab);
			valueObject = Instantiate(ValuePrefab);

			labelObject.transform.SetParent(gameObject.transform, false);
			valueObject.transform.SetParent(gameObject.transform, false);

			labelObject.GetComponent<InputLabel>().Text.text = message;

            labelObject.GetComponent<InputLabel>().BubbleImage.sprite = InputLabelPop;
            valueObject.GetComponent<InputValue>().BubbleImage.sprite = InputValuePop;
                

        }
        public void InputSubmitted(string submitedText)
        {
            LatestReadInput = submitedText;
            labelObject.GetComponent<InputLabel>().BubbleImage.sprite = InputLabelPlain;
            valueObject.GetComponent<InputValue>().BubbleImage.sprite = InputValuePlain;
        }

		public void OnPMCompilerStarted()
		{
			Clear();
			FindInputInCode(PMWrapper.fullCode);
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

        public void OnPMCompilerStopped(HelloCompiler.StopStatus status)
        {
	        if (gameObject.transform.childCount > 0)
	        {
		        var inputValue = gameObject.transform.GetChild(gameObject.transform.childCount - 1).gameObject.GetComponent<InputValue>();
		        if (inputValue != null)
			        inputValue.SubmitInput();
			}
                
        }
    }
}
