using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Security;
using System.Text;
using System.Threading.Tasks;

namespace BalsamiqFlowOverview
{
	class FlowOverview
	{
		private readonly List<FlowScreen> screens;
		public FlowOverview(List<FlowScreen> screens)
		{
			this.screens = screens;
		}

		public void CalculateLayout()
		{
			var deltaAngle = 2 * Math.PI / screens.Count;

			var angle = 0.0;
			foreach (var screen in this.screens)
			{
				//var screen = pair.Value;
				var radius = 200;
				screen.pos.X = 200 + (int)Math.Round(Math.Cos(angle) * radius);
				screen.pos.Y = 200 + (int)Math.Round(Math.Sin(angle) * radius);

				angle += deltaAngle;
			}

		}

		public Rectangle GetBounds()
		{
			var rect = new Rectangle();

			foreach (var s in screens)
			{
				// TODO: Left top bounds needed?
				//rect.X = Math.Min(rect.X, s.pos.X);
				//rect.Y = Math.Min(rect.Y, s.pos.Y);
				rect.Width = Math.Max(rect.Width, s.pos.X);
				rect.Height = Math.Max(rect.Height, s.pos.Y);
			}
			return rect;
		}

		public string GetSvg()
		{
			var sb = new StringBuilder();

			sb.AppendLine("<?xml version='1.0' encoding='UTF-8' ?>");

			var bounds = GetBounds();
			sb.AppendLine("<svg width='" + (bounds.Width + 32) + "' height='" + bounds.Height + "' xmlns='http://www.w3.org/2000/svg' xmlns:xlink='http://www.w3.org/1999/xlink' version='1.1'>");

			foreach (var n in this.screens)
			{

				var nam = SecurityElement.Escape(n.name);
				var nam_len = 30;
				var w = (nam_len * 7.2);

				sb.AppendLine("<rect x='" + (n.pos.X - nam_len * 3) + "' y='" + (n.pos.Y - 11) + "' width='" + w + "' height='15' style='fill:#FFFFFF; fill-opacity:0.7;'></rect>\n");

				sb.AppendLine("<text x='" + (n.pos.X - nam_len * 3) + "' y='" + n.pos.Y + "' width='" + w + "' height='15' style='font-family: monospace;'>" + nam + "</text>\n");
			}
			sb.AppendLine("</svg>");

			return sb.ToString();
		}
	}
}
