﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NConfiguration.Json.Parsing
{
	public class JBoolean: JValue
	{
		public bool Value { get; private set; }

		public JBoolean(bool val)
		{
			Value = val;
		}

		public override TokenType Type
		{
			get
			{
				return TokenType.Boolean;
			}
		}

		public override string ToString()
		{
			return Value?"true":"false";
		}
	}
}
