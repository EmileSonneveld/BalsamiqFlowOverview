using Newtonsoft.Json;
using System.Data.SQLite;
using System.Diagnostics;

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
				Console.WriteLine("    BalsamiqFlowOverview.exe /path/to/balsamiq.bmpr [-graphviz]");
				Console.WriteLine();
				Console.WriteLine("This will output a 'flow_graph.svg' file in the current directory.");
				Console.WriteLine("Use -graphviz to also output the graphviz notation: 'flow_graphviz.txt'");
				return -1;
			}

			string bmprPath = args[0];
			Console.WriteLine("Path: " + bmprPath);
			if (!File.Exists(bmprPath))
			{
				// This check is needed, otherwise SQLite will create an empty file here.
				Console.WriteLine("Path not found!");
				return -1;
			}
			var outputGraphvizFile = args.Contains("-graphviz");

			// Parse the Balsamiq file...
			var resources = ParseBmprToResources(bmprPath);

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
			// A .bmpr file is actually a SQLite database
			var conn = new SQLiteConnection($"Data Source={bmprPath};Version=3;");
			conn.Open();

			var resources = new Dictionary<string, Resource>();

			// Parse the the relevant info from the .bmpr file...
			var command = new SQLiteCommand("SELECT * FROM RESOURCES;", conn);
			var reader = command.ExecuteReader();
			while (reader.Read())
			{
				var attributesJson = (string)reader["ATTRIBUTES"];
				var attributes = JsonConvert.DeserializeObject<BalsamiqAttributes>(attributesJson).ThrowIfNull();

				if (attributes.mimeType != null && attributes.mimeType != "text/vnd.balsamiq.bmml") continue;
				if (attributes.kind == "symbolLibrary") continue;

				var branchId = (string)reader["BRANCHID"];
				if (branchId != "Master")
				{
					Console.WriteLine("Ignoring an alternate version of a frame");
					continue;
				}

				var id = (string)reader["ID"];
				var dataJson = (string)reader["DATA"];
				var bmml = JsonConvert.DeserializeObject<BalsamiqBmml>(dataJson).ThrowIfNull();
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
				flowScreens.Add(pair.Key, new FlowScreen(pair.Value.Attributes.name.ThrowIfNull()));
			}

			foreach (var pair in resources)
			{
				if (!pair.Value.Attributes.trashed)
				{
					var controls = GetControls(pair.Value.Bmml);
					if (controls != null)
					{
						foreach (var control in controls)
						{
							var controlText = control.properties?.text;
							foreach (var href in GetHrefs(control))
							{
								var fl = new FlowLink(
									controlText,
									flowScreens.ContainsKey(href) ?
										flowScreens[href]
										: new FlowScreen("Broken ref: " + href)
								);
								flowScreens[pair.Key].linksToScreens.Add(fl);
							}
						}
					}
				}
			}

			return flowScreens;
		}


		private static IEnumerable<Control> GetControls(BalsamiqBmml bmml)
		{
			return bmml.mockup.controls.control;
		}

		/// <summary>
		/// Get all hrefs from the control.
		/// </summary>
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


		/// <summary>
		/// Used to find the graphviz program.
		/// </summary>
		public static string? SearchProgramWhileBubblingUpPath(string relPathFromParentList)
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


		/// <summary>
		/// Will run graphviz in a separate procces to generate the SVG content.
		/// </summary>
		public static string GraphVizToSvg(string graphCode)
		{
			string fileName = Path.GetTempPath() + Guid.NewGuid() + ".svg";
			fileName = Path.Combine(fileName);
			string tmpInputPath = Path.GetTempPath() + Guid.NewGuid() + ".txt";
			tmpInputPath = Path.Combine(tmpInputPath);
			File.WriteAllText(tmpInputPath, graphCode);

			string dotPath = SearchProgramWhileBubblingUpPath("graphviz-2.38-minimal/dot").ThrowIfNull();
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
