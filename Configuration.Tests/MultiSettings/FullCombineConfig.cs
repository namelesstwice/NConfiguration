using System;
using System.Xml.Serialization;
using System.Collections.Generic;

namespace Configuration
{
	public class FullCombineConfig : ICombinable<FullCombineConfig>
	{
		[XmlAttribute]
		public string F = null;

		public FullCombineConfig Combine(FullCombineConfig prev, FullCombineConfig next)
		{
			if(prev == null)
				return next;

			if(next == null)
				return prev;
			
			prev.F = next.F ?? prev.F;

			return prev;
		}
	}
}
