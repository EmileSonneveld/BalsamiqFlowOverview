﻿using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using System.Diagnostics;
using System.IO;

namespace BalsamiqFlowOverview.UnitTestProject
{
	[TestClass]
	public class UnitTest1
	{
		[TestMethod]
		public void TestMethod1()
		{
			var json = File.ReadAllText("../../example_bmml.json");
			var parsed = JsonConvert.DeserializeObject<BalsamiqBmml>(json);
			Debug.Assert(parsed.mockup != null);
			Debug.Assert(parsed.mockup.controls.control[0].measuredH == 310);
			var href = parsed.mockup.controls.control[0].properties.hrefs.href[1].ID;
			Guid.Parse(href); // Should not crash
			Debug.Assert(href == "33915D60-C70E-A976-E11B-791060D6A925");
		}

		/*[TestMethod]
		public void TestMethod2()
		{
			object ret = Program.EvalJScript("1+2");
			Debug.Assert(ret.Equals(3));
		}*/


		[TestMethod]
		public void TestMethod2()
		{
			var svg = Program.GraphVizToSvg(@"digraph BalsamiqFlowOverview {
	rankdir=LR;
	node [shape = rectangle];
	""patient_make_appointment""->""patient_make_appointment_online""[label = ""Make appointment on this date""]
	""patient_make_appointment_online""->""information_submitted""[label = ""Submit""]
	""patient_make_appointment_online""->""select_doctor""[label = ""Select doctor""]
	""patient_make_appointment_online""->""patient_portal""[label = ""cancel""]
	""admin_modify_appointment""->""admin_search_patient""[label = ""Set patient""]
	""admin_modify_appointment""->""information_submitted""[label = ""Done""]
	""admin_modify_appointment""->""select_doctor""[label = ""Set doctor""]
	""patient_register""->""account_registered""[label = ""Register""]
	""account_registered""->""after_confirmation_mail""[label = ""click confirmation link""]
	""after_confirmation_mail""->""patient_login""[label = ""Go to login""]
	""patient_login""->""patient_portal""[label = ""Log in""]
	""patient_portal""->""patient_make_appointment""[label = ""Make appointment""]
	""patient_portal""->""patient_view_medical_file""[label = ""View my medical file""]
	""patient_portal""->""patient_make_appointment""[label = ""View schedule""]
	""patient_portal""->""vaccinations_needed_for_country""[label = ""View vaccinations needed for country""]
	""patient_view_medical_file""->""patient_portal""[label = ""<- Back""]
	""patient_view_schedule""->""patient_make_appointment_online""[label = ""Edit this appointment""]
	""doctor_portal""->""admin_search_patient""[label = ""Search patients""]
	""doctor_portal""->""doctor_view_doctor_schedule""[label = ""See Schedule""]
	""admin_search_patient""->""doctor_patient_view_medical_file""[label = ""View patient information""]
	""admin_search_patient""->""select_doctor""[label = ""Linked to doctor""]
	""doctor_patient_view_medical_file""->""doctor_portal""[label = ""<- Back""]
	""doctor_view_patient_schedule""->""admin_modify_appointment""[label = ""Edit this appointment""]
	""doctor_view_patient_schedule""->""doctor_portal""[label = ""<- Back""]
	""doctor_view_doctor_schedule""->""admin_modify_appointment""[label = ""Edit this appointment""]
}
");
			Debug.Assert(svg.ToLower().Contains("<svg"));
			Debug.Write(svg);
		}
	}
}
