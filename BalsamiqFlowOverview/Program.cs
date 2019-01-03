using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SQLite;
using Newtonsoft.Json;
using System.Diagnostics;
using System.IO;

namespace BalsamiqFlowOverview
{
	static class Program
	{
		static string imagerUlPrefix = "data:image/png;base64,";

		static void Main(string[] args)
		{
			var conn =
new SQLiteConnection("Data Source=C:/Users/emill/Dropbox (Persoonlijk)/slimmerWorden/2018-2019-Semester1/CMDM/PROJECT/balsamiq_mockups.bmpr;Version=3;");
			conn.Open();

			var bmmls = new Dictionary<string, BalsamiqBmml>();
			var flowScreens = new Dictionary<string, FlowScreen>();
			var attrs = new Dictionary<string, BalsamiqAttributes>();

			var command = new SQLiteCommand("SELECT * FROM RESOURCES;", conn);
			var reader = command.ExecuteReader();
			while (reader.Read())
			{
				var ID = (string)reader["ID"];
				var attributes = JsonConvert.DeserializeObject<BalsamiqAttributes>((string)reader["ATTRIBUTES"]);
				attrs.Add(ID, attributes);
				if (attributes.mimeType != "text/vnd.balsamiq.bmml") continue;

				var data = JsonConvert.DeserializeObject<BalsamiqBmml>((string)reader["DATA"]);
				bmmls.Add(ID, data);

				flowScreens.Add(ID, new FlowScreen(attributes.name));
			}


			foreach (var pair in bmmls)
			{
				var attr = attrs[pair.Key];

				var lst = new List<FlowLink>();
				var cts = GetControls(pair.Value);
				foreach (var c in cts)
				{
					var controlText = c.properties?.text;
					foreach (var href in GetHrefs(c))
					{
						var fl = new FlowLink();
						fl.linkName = controlText;
						fl.screen = flowScreens[href];
						lst.Add(fl);
					}
				}

				flowScreens[pair.Key].linstToScreens = lst;

			}

			var flowOverview = new FlowOverview(flowScreens.Values.ToList());
			flowOverview.CalculateLayout();
			var svg = flowOverview.GetSvg();
			File.WriteAllText("flow.svg", svg);
			var graphViz = flowOverview.GetGraphViz();
			File.WriteAllText("flow.txt", graphViz);
			//Console.ReadLine();
		}

		static List<Control> GetControls(BalsamiqBmml bmml)
		{
			return bmml.mockup.controls.control;
		}

		static List<string> GetHrefs(Control c)
		{
			var ret = new List<string>();

			if (!string.IsNullOrEmpty(c.properties?.href?.ID))
				ret.Add(c.properties.href.ID);

			/* Only used for index tree?
			if (c.properties?.hrefs?.href != null)
			{
				foreach (var href in c.properties.hrefs.href)
				{
					if (!string.IsNullOrEmpty(href.ID))
						ret.Add(href.ID);
				}
			}*/
			return ret;
		}


	}
}
