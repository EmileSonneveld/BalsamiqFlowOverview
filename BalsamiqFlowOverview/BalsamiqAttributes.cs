using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BalsamiqFlowOverview
{
	/// <summary>
	/// RootObject
	/// </summary>
	public class BalsamiqAttributes
	{
		public string importedFrom { get; set; }
		public object notes { get; set; }
		public string mimeType { get; set; }
		public string thumbnailID { get; set; }
		public int creationDate { get; set; }
		public string kind { get; set; }
		public bool trashed { get; set; }
		public string name { get; set; }
		public double order { get; set; }
	}
}
