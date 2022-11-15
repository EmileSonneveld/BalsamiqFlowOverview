using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace BalsamiqFlowOverview
{
	public static class Program
	{
		public static int Main(string[] args)
		{
			// Parse user input...
			if (args.Length < 1)
			{
				Console.WriteLine("Run the tool like this:");
				Console.WriteLine();
				Console.WriteLine("BalsamiqFlowOverview.exe /path/to/balsamiq.bmpr [-graphviz]");
				Console.WriteLine("    -graphviz to also output the graphviz notation");
				Console.WriteLine();
				Console.WriteLine("Files will be outputted in the current directory.");
				return -666;
			}

			string path = args[0];
			Console.WriteLine("Path: " + path);
			var outputGraphvizFile = args.Contains("-graphviz");

			// A .bmpr file is actually a SQLite database
			var resources = ParseBmprToResources(path);

			// Link everything...
			var flowScreens = MakeLinkedFlowScreens(resources);

			// Generate and write output...
			var flowOverview = new FlowOverview(flowScreens.Values.ToList());
			flowOverview.CalculateLayout();

			var graphViz = flowOverview.GetGraphViz();
			if (outputGraphvizFile)
			{
				Console.WriteLine("Outputting flow_graphviz.txt");
				File.WriteAllText("flow_graphviz.txt", graphViz);
			}

			var svg = GraphVizToSvg(graphViz);
			Console.WriteLine("Outputting flow_graph.svg");
			File.WriteAllText("flow_graph.svg", svg);
			return 0;
		}


		private static Dictionary<string, Resource> ParseBmprToResources(string bmprPath)
		{
			var conn = new SQLiteConnection($"Data Source={bmprPath};Version=3;");
			conn.Open();

			var resources = new Dictionary<string, Resource>();

			// Parse the the relevant info from the .bmpr file...
			var command = new SQLiteCommand("SELECT * FROM RESOURCES;", conn);
			var reader = command.ExecuteReader();
			while (reader.Read())
			{
				var id = (string)reader["ID"];
				var attributesJson = (string)reader["ATTRIBUTES"];
				var attributes = JsonConvert.DeserializeObject<BalsamiqAttributes>(attributesJson);

				Debug.Assert(attributes != null, nameof(attributes) + " != null");
				if (attributes.mimeType != "text/vnd.balsamiq.bmml") continue;
				if (attributes.kind == "symbolLibrary") continue;

				var dataJson = (string)reader["DATA"];
				var bmml = JsonConvert.DeserializeObject<BalsamiqBmml>(dataJson);
				resources.Add(id, new Resource(bmml, attributes));
			}

			return resources;
		}


		/// <summary>
		/// Flow screens that will point to each other depending
		/// if there is a control that allows the user to go from one screen to another.
		/// </summary>
		private static Dictionary<string, FlowScreen> MakeLinkedFlowScreens(
			Dictionary<string, Resource> resources
			)
		{
			var flowScreens = new Dictionary<string, FlowScreen>();
			foreach (var pair in resources)
			{
				flowScreens.Add(pair.Key, new FlowScreen(pair.Value.Attributes.name));
			}

			foreach (var pair in resources)
			{

				var lst = new List<FlowLink>();
				if (!pair.Value.Attributes.trashed)
				{
					var controls = GetControls(pair.Value.Bmml);
					foreach (var control in controls)
					{
						var controlText = control.properties?.text;
						foreach (var href in GetHrefs(control))
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

			return flowScreens;
		}


		private static List<Control> GetControls(BalsamiqBmml bmml)
		{
			return bmml.mockup.controls.control;
		}


		private static List<string> GetHrefs(Control c)
		{
			var ret = new List<string>();

			if (!string.IsNullOrEmpty(c.properties?.href?.ID))
				ret.Add(c.properties.href.ID);

			if (c.properties?.hrefs?.href != null)
			{
				ret.AddRange(from href in c.properties.hrefs.href
							 where !string.IsNullOrEmpty(href.ID)
							 select href.ID);
			}
			return ret;
		}


		public static string SearchProgramWhileBubblingUpPath(string relPathFromParentList)
		{
			if (relPathFromParentList.StartsWith("/") || relPathFromParentList.StartsWith("\\"))
				relPathFromParentList = relPathFromParentList.Substring(1);

			var di = new DirectoryInfo(
				AppDomain.CurrentDomain.BaseDirectory
				?? Directory.GetCurrentDirectory() // Fallback for when running as test.
				);

			while (di != null)
			{
				var absPath = di.FullName;
				if (!absPath.EndsWith("/") && !absPath.EndsWith("\\"))
					absPath += "\\";
				absPath += relPathFromParentList;

				if (File.Exists(absPath)) return absPath;
				if (File.Exists(absPath + ".exe")) return absPath; // could als .exe extension to absPath.

				di = di.Parent;
			}
			return null;
		}


		public static string GraphVizToSvg(string graphCode)
		{
			string fileName = Path.GetTempPath() + Guid.NewGuid() + ".svg";
			fileName = Path.Combine(fileName);
			string tmpInputPath = Path.GetTempPath() + Guid.NewGuid() + ".txt";
			tmpInputPath = Path.Combine(tmpInputPath);
			File.WriteAllText(tmpInputPath, graphCode);

			string dotPath = SearchProgramWhileBubblingUpPath("graphviz-2.38-minimal/dot");
			// Add -v parameter to debug dot.exe
			string arguments = tmpInputPath + " -Tsvg -o " + fileName;

			Console.WriteLine("Ignore the following .dll warning...");
			// Warning: Could not load "[...]\graphviz-2.38-minimal\gvplugin_pango.dll" - can't open the module
			// Start the child process.
			var p = new Process();
			// Redirect the output stream of the child process.
			p.StartInfo.UseShellExecute = false;
			p.StartInfo.RedirectStandardOutput = true;
			p.StartInfo.FileName = dotPath;
			p.StartInfo.Arguments = arguments;
			p.Start();
			// Do not wait for the child process to exit before
			// reading to the end of its redirected stream.
			// Read the output stream first and then wait.
			p.StandardOutput.ReadToEnd();
			p.WaitForExit();
			return File.ReadAllText(fileName);
		}
	}
}
