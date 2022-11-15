using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using System.Linq;

namespace BalsamiqFlowOverview
{
	internal class FlowOverview
	{
		private readonly List<FlowScreen> _screens;
		public FlowOverview(List<FlowScreen> screens)
		{
			this._screens = screens;
		}

		[System.Diagnostics.CodeAnalysis.SuppressMessage("Minor Code Smell", "S3267:Loops should be simplified with \"LINQ\" expressions", Justification = "<Pending>")]
		public void CalculateLayout()
		{
			var deltaAngle = 2 * Math.PI / _screens.Count;

			var angle = 0.0;
			foreach (var screen in this._screens)
			{
				var radius = 200;
				screen.pos.X = 250 + (int)Math.Round(Math.Cos(angle) * radius);
				screen.pos.Y = 250 + (int)Math.Round(Math.Sin(angle) * radius);

				angle += deltaAngle;
			}
		}

		public Rectangle GetBounds()
		{
			var rect = new Rectangle
			{
				Width = _screens.Max(s => s.pos.X) + 50,
				Height = _screens.Max(s => s.pos.Y) + 50
			};
			return rect;
		}

		/// <summary>
		/// http://www.webgraphviz.com/
		/// </summary>
		public string GetGraphViz()
		{
			var sb = new StringBuilder();
			sb.AppendLine("# You can visualise this file here: http://webgraphviz.com");
			sb.AppendLine("digraph BalsamiqFlowOverview {");
			sb.AppendLine("    rankdir=LR;");
			sb.AppendLine("#    concentrate = true;"); // User can uncomment
			sb.AppendLine("    node [shape = rectangle, style=filled, color=\"0.650 0.200 1.000\"];");

			foreach (var s in _screens)
			{
				foreach (var link in s.linksToScreens)
				{
					var s2 = link.screen;

					// Check if link goes in both directions:
					if (s2.linksToScreens.Find(l2 => l2.linkName == link.linkName && l2.screen == s) != null)
					{
						if (s.GetHashCode() < s2.GetHashCode())
							sb.AppendLine($"    \"{s.name.Replace("\"", "\\\"")}\"->\"{s2.name}\" [label = \"{link.linkName}\", dir=both, penwidth=2]");
					}
					else
					{
						sb.AppendLine(
							$"    \"{s.name.Replace("\"", "\\\"")}\"->\"{s2.name}\" [label = \"{link.linkName}\"]");
					}
				}
				if (s.linksToScreens.Count == 0)
				{
					sb.AppendLine($"    \"{s.name.Replace("\"", "\\\"")}\"");
				}
			}
			sb.AppendLine("}");

			return sb.ToString();
		}
	}
}
