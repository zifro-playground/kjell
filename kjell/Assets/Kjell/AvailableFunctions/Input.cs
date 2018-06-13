using Compiler;
using Kjell;

public class Input : Function
{

	public Input()
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
		return new Variable("name", "hej");
	}
}
