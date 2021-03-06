using System;
using System.Xml;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using System.Xml.Serialization;
using System.Collections.Generic;
using NConfiguration.Joining;
using NConfiguration.GenericView;

namespace NConfiguration.Json
{
	public class JsonFileSettingsLoader : FileSearcher
	{
		private readonly IStringConverter _converter;

		public JsonFileSettingsLoader(IGenericDeserializer deserializer, IStringConverter converter)
			: base(deserializer)
		{
			_converter = converter;
		}

		public IIdentifiedSource LoadFile(string path)
		{
			return new JsonFileSettings(path, _converter, Deserializer);
		}

		/// <summary>
		/// name of including configuration
		/// </summary>
		public override string Tag
		{
			get
			{
				return "JsonFile";
			}
		}

		public override IIdentifiedSource CreateFileSetting(string path)
		{
			return new JsonFileSettings(path, _converter, Deserializer);
		}
	}
}

