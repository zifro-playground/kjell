using Compiler;
using Kjell;
using UnityEngine;

public class Print : Function
{
	public Print()
	{
		name = "print";
		buttonText = "print()";
		pauseWalker = false;
		hasReturnVariable = false;

		for (int i = 0; i < 20; i++)
			inputParameterAmount.Add(i);
	}

	public override Variable runFunction(Scope currentScope, Variable[] inputParas, int lineNumber)
	{
		IOStream.Instance.Print(inputParas[0].getString());

		return new Variable();
	}
}
