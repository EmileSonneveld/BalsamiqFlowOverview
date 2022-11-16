using System.Collections.Generic;

#pragma warning disable IDE1006 // Naming Styles

// Classes used to parse JSON to plain old c# objects.
// Code generated with: http://json2csharp.com/
// Select 'Generate Immutable Classes'
// Select 'Use Nullable Types'
// Change the 'Root' object name
namespace BalsamiqFlowOverview
{
	public class Control
	{
		public Control(
			string ID,
			string measuredH,
			string measuredW,
			Properties properties,
			string typeID,
			string x,
			string y,
			string zOrder
		)
		{
			this.ID = ID;
			this.measuredH = measuredH;
			this.measuredW = measuredW;
			this.properties = properties;
			this.typeID = typeID;
			this.x = x;
			this.y = y;
			this.zOrder = zOrder;
		}

		public string ID { get; }
		public string measuredH { get; }
		public string measuredW { get; }
		public Properties properties { get; }
		public string typeID { get; }
		public string x { get; }
		public string y { get; }
		public string zOrder { get; }
	}

	public class Controls
	{
		public Controls(
			List<Control> control
		)
		{
			this.control = control;
		}

		public IReadOnlyList<Control> control { get; }
	}

	public class Href
	{
		public Href(
			string ID,
			string URL
		)
		{
			this.ID = ID;
			this.URL = URL;
		}

		public string ID { get; }
		public string URL { get; }
	}

	public class Hrefs
	{
		public Hrefs(
			List<Href> href
		)
		{
			this.href = href;
		}

		public IReadOnlyList<Href> href { get; }
	}

	public class Mockup
	{
		public Mockup(
			Controls controls,
			string measuredH,
			string measuredW,
			string mockupH,
			string mockupW,
			string version
		)
		{
			this.controls = controls;
			this.measuredH = measuredH;
			this.measuredW = measuredW;
			this.mockupH = mockupH;
			this.mockupW = mockupW;
			this.version = version;
		}

		public Controls controls { get; }
		public string measuredH { get; }
		public string measuredW { get; }
		public string mockupH { get; }
		public string mockupW { get; }
		public string version { get; }
	}

	public class Properties
	{
		public Properties(
			Hrefs hrefs,
			string text,
			Src src,
			Href href
		)
		{
			this.hrefs = hrefs;
			this.text = text;
			this.src = src;
			this.href = href;
		}

		public Hrefs hrefs { get; }
		public string text { get; }
		public Src src { get; }
		public Href href { get; }
	}

	public class BalsamiqBmml
	{
		public BalsamiqBmml(
			Mockup mockup
		)
		{
			this.mockup = mockup;
		}

		public Mockup mockup { get; }
	}

	public class Src
	{
		public Src(
			int? Anchor,
			string ID
		)
		{
			this.Anchor = Anchor;
			this.ID = ID;
		}

		public int? Anchor { get; }
		public string ID { get; }
	}


}
#pragma warning restore IDE1006 // Naming Styles
