using Compiler;
using Kjell;

public class InputFunction : Function
{

	public InputFunction()
	{
		name = "input";
		buttonText = "input()";
		pauseWalker = false;
		hasReturnVariable = false;
		inputParameterAmount.Add(0);
		inputParameterAmount.Add(1);
	}

	public override Variable runFunction(Scope currentScope, Variable[] inputParas, int lineNumber)
	{
		var userInput = IOStream.Instance.LatestReadInput;
		return new Variable("userInput", userInput);
	}
}
