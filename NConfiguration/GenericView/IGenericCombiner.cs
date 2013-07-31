﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NConfiguration.GenericView
{
	public interface IGenericCombiner
	{
		T Combine<T>(T x, T y);
	}
}
