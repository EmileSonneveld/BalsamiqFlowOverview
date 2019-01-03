using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
//using System.Data.SQLite;
using Newtonsoft.Json;

namespace BalsamiqFlowOverview.UnitTestProject
{
	[TestClass]
	public class UnitTest1
	{
		const string exampleJson = @"{
    ""mockup"": {
        ""controls"": {
            ""control"": [
                {
                    ""ID"": ""0"",
                    ""measuredH"": ""310"",
                    ""measuredW"": ""1524"",
                    ""properties"": {
                        ""hrefs"": {
                            ""href"": [
                                {},
                                {
                                    ""ID"": ""33915D60-C70E-A976-E11B-791060D6A925""
                                },
                                {
                                    ""ID"": ""C7039852-E190-13F0-0A52-5F781832C7E5""
                                },
                                {
                                    ""ID"": ""CA981B6F-B94F-7A4C-B9BA-5F6D3214A5D0""
                                },
                                {
                                    ""ID"": ""CD37F0A1-1BE0-17B2-86DF-5F7A1B186C23""
                                },
                                {
                                    ""ID"": ""1231B7D8-1D39-4C73-32CC-506861015E17""
                                },
                                {
                                    ""ID"": ""02983192-A6FE-FFC1-BE04-5F771AA9DEBC""
                                },
                                {
                                    ""ID"": ""8DDF8F28-D674-72BB-8CF2-5AD14E04EA64""
                                },
                                {
                                    ""ID"": ""9F71214B-9AB5-A360-C5E9-5ADE5E6260CF""
                                },
                                {
                                    ""ID"": ""9BCA4C53-DA10-5DD9-322B-5AE390612B83""
                                },
                                {
                                    ""ID"": ""E33E46FF-1F7B-57BA-5948-5AFD063C8758""
                                },
                                {
                                    ""ID"": ""793ACEF7-1804-9001-BE75-5F7176879288""
                                },
                                {
                                    ""ID"": ""F36631E6-122A-7DFE-C35E-5F74839BCA6B""
                                }
                            ]
                        },
                        ""text"": ""modify_doctors_appointment\n- index\n- portal_patient\n-- patient_make_appointment\n- portal_secretary\n- modify_appointment\n-- modify_appointment_step2\n- acces_schedule\n-- select_doctor_from_department\n- show_schedule\n- select_patient_task\n- Make appointment online\n- information_submitted""
                    },
                    ""typeID"": ""SiteMap"",
                    ""x"": ""0"",
                    ""y"": ""99"",
                    ""zOrder"": ""0""
                }
            ]
        },
        ""measuredH"": ""409"",
        ""measuredW"": ""1524"",
        ""mockupH"": ""310"",
        ""mockupW"": ""1524"",
        ""version"": ""1.0""
    }
}";
		[TestMethod]
		public void TestMethod1()
		{
			var parsed = JsonConvert.DeserializeObject<Mockup>(exampleJson);
		}
	}
}
