using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SQLite;

namespace BalsamiqFlowOverview
{
	class Program
	{
		static void Main(string[] args)
		{
			var m_dbConnection =
new SQLiteConnection("Data Source=C:/Users/emill/Dropbox (Persoonlijk)/slimmerWorden/2018-2019-Semester1/CMDM/PROJECT/balsamiq_mockups.bmpr;Version=3;");
			m_dbConnection.Open();
		}
	}
}
