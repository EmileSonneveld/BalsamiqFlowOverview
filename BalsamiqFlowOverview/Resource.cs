namespace BalsamiqFlowOverview
{
	public class Resource
	{
		public Resource(BalsamiqBmml bmml, BalsamiqAttributes attributes)
		{
			this.Bmml = bmml;
			this.Attributes = attributes;
		}

		public BalsamiqBmml Bmml { get; private set; }
		public BalsamiqAttributes Attributes { get; private set; }
	}
}
