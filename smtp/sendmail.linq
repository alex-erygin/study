<Query Kind="Program">
  <Namespace>System.Net.Mail</Namespace>
</Query>

void Main()
{
	var client = new SmtpClient("smtp.sendgrid.net", 587);
	client.EnableSsl = true;
	client.Credentials = new System.Net.NetworkCredential("apikey", Util.GetPassword("sendgrid-home"));
	var msg = new MailMessage(Util.GetPassword("mail-mail"), Util.GetPassword("mail-mail"), "Sendgrid test", "hello from sendgrid!");
	client.Send(msg);
}

// Define other methods, classes and namespaces here
