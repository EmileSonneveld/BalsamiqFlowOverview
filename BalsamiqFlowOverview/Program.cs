using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SQLite;
using Newtonsoft.Json;
using System.Diagnostics;

namespace BalsamiqFlowOverview
{
	static class Program
	{
		static void Main(string[] args)
		{
			var conn =
new SQLiteConnection("Data Source=C:/Users/emill/Dropbox (Persoonlijk)/slimmerWorden/2018-2019-Semester1/CMDM/PROJECT/balsamiq_mockups.bmpr;Version=3;");
			conn.Open();

			var bmmls = new Dictionary<string, BalsamiqBmml>();
			var command = new SQLiteCommand("SELECT * FROM RESOURCES;", conn);
			var reader = command.ExecuteReader();
			while (reader.Read())
			{
				var attributes = JsonConvert.DeserializeObject<BalsamiqAttributes>((string)reader["ATTRIBUTES"]);
				if (attributes.mimeType != "text/vnd.balsamiq.bmml") continue;

				var data = JsonConvert.DeserializeObject<BalsamiqBmml>((string)reader["DATA"]);
				Console.WriteLine("ID: " + reader["ID"] + "\t  DATA: " + reader["DATA"]);
				bmmls.Add((string)reader["ID"], data);

				var cts = GetControls(data);
				foreach (var c in cts)
					foreach (var href in GetHrefs(c))
						Debug.Assert(bmmls.Keys.Contains(href));
			}

			Console.ReadLine();
		}

		static List<Control> GetControls(BalsamiqBmml bmml)
		{
			return bmml.mockup.controls.control;
		}

		static List<string> GetHrefs(Control c)
		{
			var ret = new List<string>();
			if (c.properties?.hrefs?.href == null) return ret;

			foreach (var href in c.properties.hrefs.href)
			{
				if (!string.IsNullOrEmpty(href.ID))
					ret.Add(href.ID);
			}
			return ret;
		}


	}
}
