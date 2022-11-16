using System.Drawing;

namespace BalsamiqFlowOverview
{
	class FlowLink
	{
		public readonly string? linkName;
		public readonly FlowScreen screen;

		public FlowLink(string? linkName, FlowScreen screen)
		{
			this.linkName = linkName;
			this.screen = screen;
		}
	}

	class FlowScreen
	{
		public FlowScreen(string name)
		{
			this.name = name;
		}
		public readonly string name;
		public Point pos;
		public readonly List<FlowLink> linksToScreens = new List<FlowLink>();
	}
}
