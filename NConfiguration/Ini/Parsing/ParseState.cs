﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NConfiguration.Ini.Parsing
{
	internal enum ParseState
	{
		BeginLine,
		EmptyLine,
		Comment,
		SectionName,
		SectionEnd,
		KeyName,
		EndKeyName,
		BeginValue,
		SimpleValue,
		QuotedValue,
		EscValue,
		EndQuotedValue
	}
}
