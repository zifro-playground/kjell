﻿using System.Collections.Generic;

public class Test
{
	public List<string> input { get; set; }
	public List<string> output { get; set; }
}

public class KjellCaseDefinition : CaseDefinition
{
	public Test test { get; set; }
}
