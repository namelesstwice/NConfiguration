using NConfiguration.Xml;
using NUnit.Framework;
using NConfiguration.GenericView;
using NConfiguration.Tests;

namespace NConfiguration
{
	[TestFixture]
	public class XmlSystemSettingsTests
	{
		[Test]
		public void ReadForDefaultName()
		{
			var cfg = new XmlSystemSettings("ExtConfigure", Global.PlainConverter, Global.GenericDeserializer).First<MyXmlConfig>();
			
			Assert.AreEqual("attr field text", cfg.AttrField);
			Assert.AreEqual("elem field text", cfg.ElemField);
		}
	}
}

