using System.Collections.Generic;

#pragma warning disable IDE1006 // Naming Styles

/// <summary>
/// http://json2csharp.com/
/// </summary>
namespace BalsamiqFlowOverview
{
	public class Href
	{
		/// <summary>
		/// GUID
		/// </summary>
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
		public Href href { get; set; }
		public string text { get; set; }
		public Src src { get; set; }
	}

	public class Control
	{
		public string ID { get; set; }
		public double measuredH { get; set; }
		public double measuredW { get; set; }
		public Properties properties { get; set; }
		/// <summary>
		/// SiteMap, BlockOfText, BreadCrumbs, ComboBox, DataGrid,
		/// IconLabel, NumericStepper, PieChart, Map,
		/// BrowserWindow, Image, LineOfText, Button
		/// </summary>
		public string typeID { get; set; }
		public double x { get; set; }
		public double y { get; set; }
		public double zOrder { get; set; }
	}

	public class Controls
	{
		public List<Control> control { get; set; }
	}

	public class Mockup
	{
		public Controls controls { get; set; }
		public double measuredH { get; set; }
		public double measuredW { get; set; }
		public double mockupH { get; set; }
		public double mockupW { get; set; }
		public double version { get; set; }
	}

	/// <summary>
	/// RootObject
	/// </summary>
	public class BalsamiqBmml
	{
		public Mockup mockup { get; set; }
	}
}
#pragma warning restore IDE1006 // Naming Styles
