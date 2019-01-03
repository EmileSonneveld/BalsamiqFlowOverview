using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BalsamiqFlowOverview
{
	public class Href
	{
		public string ID { get; set; }
	}

	public class Hrefs
	{
		public List<Href> href { get; set; }
	}

	public class Src
	{
		public int Anchor { get; set; }
		public string ID { get; set; }
	}

	public class Properties
	{
		public Hrefs hrefs { get; set; }
		public string text { get; set; }
		public Src src { get; set; }
	}

	public class Control
	{
		public string ID { get; set; }
		public string measuredH { get; set; }
		public string measuredW { get; set; }
		public Properties properties { get; set; }
		public string typeID { get; set; }
		public string x { get; set; }
		public string y { get; set; }
		public string zOrder { get; set; }
	}

	public class Controls
	{
		public List<Control> control { get; set; }
	}

	public class Mockup
	{
		public Controls controls { get; set; }
		public string measuredH { get; set; }
		public string measuredW { get; set; }
		public string mockupH { get; set; }
		public string mockupW { get; set; }
		public string version { get; set; }
	}

	public class RootObject
	{
		public Mockup mockup { get; set; }
	}
}
