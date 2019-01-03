﻿using System;
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
				screen.pos.X = 250 + (int)Math.Round(Math.Cos(angle) * radius);
				screen.pos.Y = 250 + (int)Math.Round(Math.Sin(angle) * radius);

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
				rect.Width = Math.Max(rect.Width, s.pos.X + 50);
				rect.Height = Math.Max(rect.Height, s.pos.Y + 50);
			}
			return rect;
		}

		public string GetSvg()
		{
			var sb = new StringBuilder();

			sb.AppendLine("<?xml version='1.0' encoding='UTF-8' ?>");

			var bounds = GetBounds();
			sb.AppendLine("<svg width='" + (bounds.Width + 32) + "' height='" + bounds.Height + "' xmlns='http://www.w3.org/2000/svg' xmlns:xlink='http://www.w3.org/1999/xlink' version='1.1'>");
			sb.AppendLine(@"<defs>
  <marker id='head' orient='auto'
    markerWidth='2' markerHeight='4'
    refX='0.1' refY='2'>
    <!-- triangle pointing right (+x) -->
    <path d='M0,0 V4 L2,2 Z' fill='red'/>
  </marker>
</defs>");
			foreach (var n in this.screens)
			{
				foreach (var link in n.linstToScreens)
				{
					var p = link.screen.pos;
					sb.AppendLine($@"<path
  id='arrow-line'
  marker-end='url(#head)'
  stroke-width='3'
  fill='none' stroke='black'  
  d='M{n.pos.X},{n.pos.Y} C{n.pos.X},{n.pos.Y} {p.X},{p.Y} {p.X},{p.Y}'
  />");
				}
			}

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
