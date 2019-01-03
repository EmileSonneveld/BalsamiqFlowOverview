using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BalsamiqFlowOverview
{
	class FlowLink
	{
		public string linkName;
		public FlowScreen screen;
	}

	class FlowScreen
	{
		public FlowScreen(string name) {
			this.name = name;
		}
		public readonly string name;
		public Point pos;
		public List<FlowLink> linstToScreens;
	}
}
