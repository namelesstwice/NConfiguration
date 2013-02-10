﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Configuration.GenericView;

namespace Configuration.Ini
{
	public abstract class IniSettings : IAppSettings
	{
		private readonly IPlainConverter _converter;
		private readonly IGenericDeserializer _deserializer;

		public IniSettings(IPlainConverter converter, IGenericDeserializer deserializer)
		{
			_converter = converter;
			_deserializer = deserializer;
		}

		protected abstract IEnumerable<Section> Sections { get; }

		public IEnumerable<T> LoadCollection<T>(string sectionName)
		{
			foreach (var section in Sections)
			{
				if (section.Name == sectionName)
					yield return _deserializer.Deserialize<T>(new ViewSection(_converter, section));

				if (section.Name == string.Empty)
					foreach(var pair in section.Pairs.Where(p => p.Key == sectionName))
						yield return _deserializer.Deserialize<T>(new ViewPlainField<string>(_converter, pair.Value));
			}
		}
	}
}
