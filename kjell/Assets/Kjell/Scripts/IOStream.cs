using System;
using System.Collections.Generic;
using System.Linq;
using Compiler;
using UnityEngine;

namespace Kjell
{
	public class IOStream : MonoBehaviour, IPMCompilerStarted, IPMLevelChanged
	{
		public GameObject PrintPrefab;
		public GameObject LabelPrefab;
		public GameObject ValuePrefab;

		public Dictionary<int, string> LinesWithInput;

		public static IOStream Instance;

		private void Start()
		{
			if (Instance == null)
				Instance = this;
		}

		private void FindInputInCode(string code)
		{
			LinesWithInput = new Dictionary<int, string>();

			var lines = code.Split('\n');
			var lineIndex = 0;
			foreach (var line in lines)
			{
				var inputArgument = ParseInputArgument(line);
				if (inputArgument != null)
					LinesWithInput.Add(lineIndex, inputArgument);
				lineIndex++;
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
					if (line[charIndex] == ')')
						PMWrapper.RaiseError("Fel vid input. Det är för många slutparenteser.");
					else
						PMWrapper.RaiseError("Fel vid input. Det får inte vara några tecken efter input().");
				}
			}

			if (parenthesisCount >= 0)
				PMWrapper.RaiseError("Fel antal parenteser. Det är fler startparenteser än slutparenteser");

			if (inputInLine)
				return inputArgument;

			return null;
		}

		public void TestCompiler(string input)
		{
			var lines = Compiler.SyntaxCheck.parseLines(input);
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

			print("Input: " + input + " -> " + label);
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
	}
}
