<Query Kind="Program">
  <Namespace>System.Net.Mail</Namespace>
</Query>

void Main()
{
	Util.GetPassword("sendgrid-home").Dump();
	
	var client = new SmtpClient("smtp.sendgrid.net", 587);
	client.EnableSsl = true;
	client.Credentials = new System.Net.NetworkCredential("apikey", Util.GetPassword("sendgrid-home"));
	var from = Util.GetPassword("mail-mail");
	var to = Util.GetPassword("mail-mail");
	var subject = "Sendgrid test";
	var body = "hello from sendgrid!";
	var msg = new System.Net.Mail.MailMessage(from, to, subject, body);
	client.Send(msg);
}

// Define other methods, classes and namespaces here