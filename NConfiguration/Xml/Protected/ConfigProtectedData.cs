using System.Runtime.Serialization;
using NConfiguration.GenericView;

namespace NConfiguration.Xml.Protected
{
	[DataContract(Name = "configProtectedData")]
	public class ConfigProtectedData
	{
		[DataMember(Name = "providers")]
		public ICfgNode Providers { get; set; }
	}
}

