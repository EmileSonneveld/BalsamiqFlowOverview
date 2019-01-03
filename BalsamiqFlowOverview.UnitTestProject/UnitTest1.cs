using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using System.Diagnostics;
using System.IO;

namespace BalsamiqFlowOverview.UnitTestProject
{
	[TestClass]
	public class UnitTest1
	{
		[TestMethod]
		public void TestMethod1()
		{
			var json = File.ReadAllText("../../example_bmml.json");
			var parsed = JsonConvert.DeserializeObject<BalsamiqBmml>(json);
			Debug.Assert(parsed.mockup != null);
			Debug.Assert(parsed.mockup.controls.control[0].measuredH == 310);
			var href = parsed.mockup.controls.control[0].properties.hrefs.href[1].ID;
			Guid.Parse(href); // Should not crash
			Debug.Assert(href == "33915D60-C70E-A976-E11B-791060D6A925");

		}
	}
}
