BalsamiqFlowOverview
====================

Generate an overview the control flow from a Balsamiq document.
It reads the Balsamiq database and generates an SVG, or a Grapviz file that in its turn can be converted to an image.

Check out the releases tab on GitHub to download the command line version!
Example on how to run the application:
BalsamiqFlowOverview.exe "C:\path\to\balsamiq_mockups.bmpr"

This should output 2 files: flow.svg and flow.txt
The content from flow.txt can be converted here: http://www.webgraphviz.com/

Example of the flow.txt content:
```
digraph BalsamiqFlowOverview {
	rankdir=LR;
	size="8, 5"
	node [shape = rectangle];
	modify_appointment->modify_appointment_step2[label = "Next >"]
	acces_schedule->select_doctor_from_department[label = "Submit department"]
	patient_make_appointment->make_appointment_online[label = "Make appointment on this date"]
	make_appointment_online->information_submitted[label = "Submit"]
	make_appointment_online->select_doctor_from_department[label = "Select doctor"]
	make_appointment_online->patient_portal[label = "cancel"]
	information_submitted->patient_portal[label = "Back to patient portal"]
	modify_appointment_step2->information_submitted[label = "Done"]
	portal_secretary->modify_appointment[label = "Modify appointment"]
	patient_register->account_registered[label = "Register"]
	account_registered->after_confirmation_mail[label = "click confirmation link"]
	after_confirmation_mail->patient_login[label = "Go to login"]
	patient_login->patient_portal[label = "Log in"]
	patient_portal->patient_make_appointment[label = "Make appointment"]
	patient_portal->patient_view_medical_file[label = "View my medical file"]
	patient_portal->patient_make_appointment[label = "View schedule"]
	patient_view_medical_file->patient_portal[label = "<- Back"]
	patient_view_schedule->make_appointment_online[label = "Edit this appointment"]
}
```
Which webgraphviz transforms into:
![cover_photo](https://github.com/EmileSonneveld/BalsamiqFlowOverview/blob/master/example_grapviz.svg)
