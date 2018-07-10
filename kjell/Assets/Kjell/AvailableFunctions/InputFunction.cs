using Compiler;
using Kjell;

public class InputFunction : Function
{

	public InputFunction()
	{
		name = "input";
		buttonText = "input()";
		pauseWalker = true;
		hasReturnVariable = false;
		inputParameterAmount.Add(0);
		inputParameterAmount.Add(1);
	}

	public override Variable runFunction(Scope currentScope, Variable[] inputParas, int lineNumber)
	{
		var inputLabel = "";

		if (inputParas.Length > 0)
		{
			if (inputParas[0].variableType == VariableTypes.number)
				PMWrapper.RaiseError("Fel datatyp. Input kan bara ta in en sträng men fick in ett tal.");
			else if (inputParas[0].variableType == VariableTypes.boolean)
				PMWrapper.RaiseError("Fel datatyp. Input kan bara ta in en sträng men fick in ett boolskt värde.");
			else
				inputLabel = inputParas[0].getString();
		}

		IOStream.Instance.StartCoroutine(IOStream.Instance.TriggerInput(inputLabel));

		return new Variable("inputValue", "");
	}
}
