using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Compiler;
using UnityEngine;

namespace Kjell
{
	public static class InputParser
	{
		// Parse code and find what lines makes a function call
		public static Dictionary<int, string> FindInputInCode(string code)
		{
			var LinesWithInput = new Dictionary<int, string>();

			var lines = code.Split('\n');
			var lineIndex = 0;
			var lineNumberForError = 1;

			foreach (var line in lines)
			{
				if (!line.Trim().StartsWith("#") && line.Trim() != "")
				{
					var inputArgument = ParseInputArgument(line, lineNumberForError);
					if (inputArgument != null)
						LinesWithInput[lineIndex] = inputArgument;
					lineIndex++;
				}

				lineNumberForError++;
			}

			return LinesWithInput;
		}

		public static string InterpretArgument(string argument)
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

		// Parse a line of code, find input call and extract and return its parameter. Return null if no input in line.
		private static string ParseInputArgument(string line, int lineNumber)
		{
			var charIndex = 6;
			var inputArgument = "";
			var inputInLine = false;

			while (charIndex < line.Length)
			{
				if (line[charIndex - 6] == '#') // skip rest of line if it is a comment
					break;

				if (line[charIndex - 6] == 'i' && line[charIndex - 5] == 'n' && line[charIndex - 4] == 'p' &&
					line[charIndex - 3] == 'u' && line[charIndex - 2] == 't' && line[charIndex - 1] == '(')
				{
					inputInLine = true;
					var parenthesisCount = 0;

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
			}
			
			if (inputInLine)
			{
				CheckParenthesisCount(line, lineNumber);
				return inputArgument;
			}

			return null;
		}

		private static void CheckParenthesisCount(string line, int lineNumber)
		{
			var startParenthesisCount = line.Count(x => x == '(');
			var endParenthesisCount = line.Count(x => x == ')');

			if (startParenthesisCount > endParenthesisCount)
				PMWrapper.RaiseError(lineNumber, "Fel antal parenteser. Det är fler startparenteser än slutparenteser men borde vara lika många.");
			if (startParenthesisCount < endParenthesisCount)
				PMWrapper.RaiseError(lineNumber, "Fel antal parenteser. Det är fler slutparenteser än startparenteser men borde vara lika många.");
		}
	}
}

