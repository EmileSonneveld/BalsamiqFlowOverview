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
	public static class Program
	{
		static string imagerUlPrefix = "data:image/png;base64,";

		static void Main(string[] args)
		{
			string path = null;
			if (args.Length >= 1)
				path = args[0];
			else
			{
				Console.WriteLine("No Argument specified. Using default path.");
				path = "C:/Users/emill/Dropbox (Persoonlijk)/slimmerWorden/2018-2019-Semester1/CMDM/PROJECT/DELIVERABLE_Emile_Maurin_Yassine/balsamiq_mockups.bmpr";
			}
			Console.WriteLine("Path: " + path);

			var conn = new SQLiteConnection($"Data Source={path};Version=3;");
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
				if (attributes.kind == "symbolLibrary") continue;

				var data = JsonConvert.DeserializeObject<BalsamiqBmml>((string)reader["DATA"]);
				bmmls.Add(ID, data);

				flowScreens.Add(ID, new FlowScreen(attributes.name));
			}


			foreach (var pair in bmmls)
			{
				var attr = attrs[pair.Key];

				var lst = new List<FlowLink>();
				if (!attr.trashed)
				{
					var cts = GetControls(pair.Value);
					foreach (var c in cts)
					{
						var controlText = c.properties?.text;
						foreach (var href in GetHrefs(c))
						{
							var fl = new FlowLink();
							fl.linkName = controlText;
							if (flowScreens.ContainsKey(href))
								fl.screen = flowScreens[href];
							else
								fl.screen = new FlowScreen("Broken ref: " + href);
							lst.Add(fl);
						}
					}
				}
				flowScreens[pair.Key].linksToScreens = lst;

			}

			var flowOverview = new FlowOverview(flowScreens.Values.ToList());
			flowOverview.CalculateLayout();
			//var svg = flowOverview.GetSvg();
			//Console.WriteLine("Outputting flow.svg");
			//File.WriteAllText("flow.svg", svg);

			var graphViz = flowOverview.GetGraphViz();
			Console.WriteLine("Outputting flow.txt");
			File.WriteAllText("flow.txt", graphViz);

			var svg2 = GraphVizToSvg(graphViz);
			Console.WriteLine("Outputting flow_graph.svg");
			File.WriteAllText("flow_graph.svg", svg2);
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

			if (c.properties?.hrefs?.href != null)
			{
				foreach (var href in c.properties.hrefs.href)
				{
					if (!string.IsNullOrEmpty(href.ID))
						ret.Add(href.ID);
				}
			}
			return ret;
		}

		public static string SearchProgramWhileBubelingUpPath(string relPathFromParentList)
		{
			if (relPathFromParentList.StartsWith("/") || relPathFromParentList.StartsWith("\\"))
				relPathFromParentList = relPathFromParentList.Substring(1);

			DirectoryInfo di = new DirectoryInfo(Directory.GetCurrentDirectory());

			while (di != null)
			{
				var absPath = di.FullName;
				if (!absPath.EndsWith("/") && !absPath.EndsWith("\\"))
					absPath += "\\";
				absPath += relPathFromParentList;

				if (File.Exists(absPath)) return absPath;
				if (File.Exists(absPath + ".exe")) return absPath; // + ".exe";
				//if (File.Exists(absPath + ".bat")) return absPath + ".bat";

				di = di.Parent;
			}
			return null;
			//throw new FileNotFoundException(relPathFromParentList + " not found");
		}

		public static string OverloadWindowsPath(string postfix)
		{
			if (postfix.StartsWith("/") || postfix.StartsWith("\\"))
				postfix = postfix.Substring(1);

			var ret = "";
			DirectoryInfo di = new DirectoryInfo(Directory.GetCurrentDirectory());

			while (di != null)
			{
				var absPath = di.FullName;
				if (!absPath.EndsWith("/") && !absPath.EndsWith("\\"))
					absPath += "\\";
				absPath += postfix;

				//Debug.WriteLine(absPath);
				ret += ";" + absPath;
				//AddToWindowsPath(absPath);

				di = di.Parent;
			}
			return ret;
		}


		public static string GraphVizToSvg(string graphCode)
		{
			try
			{
				string fileName = System.IO.Path.GetTempPath() + Guid.NewGuid().ToString() + ".svg";
				fileName = Path.Combine(fileName);
				string tmpInputPath = System.IO.Path.GetTempPath() + Guid.NewGuid().ToString() + ".txt";
				tmpInputPath = Path.Combine(tmpInputPath);
				File.WriteAllText(tmpInputPath, graphCode);

				var dot_path = SearchProgramWhileBubelingUpPath("graphviz-2.38-minimal/dot");
				// Add -v parameter to debug dot.exe
				var arguments = tmpInputPath + " -Tsvg -o " + fileName;

				// Start the child process.
				Process p = new Process();
				// Redirect the output stream of the child process.
				p.StartInfo.UseShellExecute = false;
				p.StartInfo.RedirectStandardOutput = true;
				p.StartInfo.FileName = dot_path;
				p.StartInfo.Arguments = arguments;
				p.Start();
				// Do not wait for the child process to exit before
				// reading to the end of its redirected stream.
				// p.WaitForExit();
				// Read the output stream first and then wait.
				string output = p.StandardOutput.ReadToEnd();
				p.WaitForExit();
				//return output;
				return File.ReadAllText(fileName);
			}
			catch (Exception e)
			{
				Console.WriteLine(e.Message + "\n\n");
				return e.Message;
			}
		}
		public static string GraphVizToSvgUsingNode(string graphCode)
		{
			try
			{
				var viz_path = SearchProgramWhileBubelingUpPath("/viz.js");
				var vizCode = File.ReadAllText(viz_path);
				vizCode += @"
var data = `" + graphCode + @"`;
var svg = this.Viz(data, 'svg');
console.log(svg)
";
				string fileName = System.IO.Path.GetTempPath() + Guid.NewGuid().ToString() + ".js";
				File.WriteAllText(fileName, vizCode);

				string nodePath = SearchProgramWhileBubelingUpPath("/nodejs/node");
				if (string.IsNullOrEmpty(nodePath))
				{
					Console.WriteLine("Embedded ./nodejs/node not found. Resorting to node that has been added to system path.");
					nodePath = "node";
				}
				else
				{
					Console.WriteLine("Found node here: " + nodePath);
				}

				// Start the child process.
				Process p = new Process();
				// Redirect the output stream of the child process.
				p.StartInfo.UseShellExecute = false;
				p.StartInfo.RedirectStandardOutput = true;
				p.StartInfo.FileName = nodePath;
				p.StartInfo.Arguments = fileName;
				p.Start();
				// Do not wait for the child process to exit before
				// reading to the end of its redirected stream.
				// p.WaitForExit();
				// Read the output stream first and then wait.
				string output = p.StandardOutput.ReadToEnd();
				p.WaitForExit();
				return output;
			}
			catch (Exception e)
			{
				Console.WriteLine(e.Message + "\n\n");
				var str = "!Could not run node to generate the graph! Be sure that it is installed, and added to path.\n";
				str += "The graph code is still exported and can be copy pasted into http://webgraphviz.com";
				Console.WriteLine(str);
				return "<svg>" + str + "</svg>";
			}
		}

	}
}
