using System;
using System.Collections.Generic;
using Kjell;
using PM;
using UnityEngine;

public class CaseCorrection : MonoBehaviour, IPMCaseSwitched, IPMCompilerStopped
{
	private static int inputIndex;
	private static int outputIndex;

	private static List<string> inputs;
	private static List<string> outputs;

	public static void NextInput(GameObject inputValueObject)
	{
		if (Main.Instance.LevelData.cases[PMWrapper.currentCase].caseDefinition.test != null)
		{
			if (inputs == null || inputs.Count == 0)
				PMWrapper.RaiseTaskError("För många input. Jag förväntade mig ingen input.");

			else if (inputIndex > inputs.Count - 1)
				PMWrapper.RaiseTaskError("För många input. Jag förväntade mig bara " + inputs.Count + " input.");

			else
			{
				var nextInput = inputs[inputIndex];
				inputValueObject.GetComponent<InputValue>().SubmitInput(nextInput);
				inputIndex++;
			}
		}
	}

	public static void NextOutput(string output)
	{
		if (Main.Instance.LevelData.cases[PMWrapper.currentCase].caseDefinition.test != null)
		{
			if (outputIndex > outputs.Count - 1)
				PMWrapper.RaiseTaskError("För många print. Jag förväntade mig bara " + outputs.Count + " print.");

			else if (output != outputs[outputIndex])
				PMWrapper.RaiseTaskError("Fel print.");

			outputIndex++;
		}
	}

	public void OnPMCaseSwitched(int caseNumber)
	{
		if (Main.Instance.LevelData.cases[caseNumber].caseDefinition.test != null)
		{
			inputIndex = 0;
			inputs = Main.Instance.LevelData.cases[caseNumber].caseDefinition.test.input;

			outputIndex = 0;
			outputs = Main.Instance.LevelData.cases[caseNumber].caseDefinition.test.output;

			if (outputs == null || outputs.Count == 0)
				throw new Exception("There is a test defined but no output defined.");
		}
	}

	public void OnPMCompilerStopped(HelloCompiler.StopStatus status)
	{
		if (status == HelloCompiler.StopStatus.Finished)
		{
			if (inputs != null && inputIndex < inputs.Count)
				PMWrapper.RaiseTaskError("För få input. Jag förväntade mig " + inputs.Count + " input.");
			else if (outputs != null && outputIndex < outputs.Count)
				PMWrapper.RaiseTaskError("För få print. Jag förväntade mig " + outputs.Count + " print.");
		}
	}
}
