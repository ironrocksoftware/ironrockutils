
using System;
using System.Net;
using System.IO;
using Microsoft.Win32;
using System.Net.Mail;
using System.Collections.Specialized;

namespace IronRockUtils
{
	public class MailUtils
	{
		// Sends an email using the specified SMTP details and the email settings.
		public static void SendMail (string host, int port, string user, string pass, string from, string fromName, string to, string subject, string msg, string[] files)
		{
			string[] recipients = to.Split(',');
			if (recipients.Length > 1)
			{
				for (int i = 0; i < recipients.Length; i++)
				{
					try {
						SendMail(host, port, user, pass, from, fromName, recipients[i].Trim(), subject, msg, files);
					} catch (Exception e) {
						Log.write("Failed sending to: " + recipients[i] + ", Error: " + e.Message);
					}
				}

				return;
			}

			MailMessage mail = new MailMessage (new MailAddress (from, fromName), new MailAddress (to));
        	SmtpClient client = new SmtpClient();

        	client.Host = host;
        	client.Port = port;
        	client.EnableSsl = port == 465 ? true : false;
	        client.DeliveryMethod = SmtpDeliveryMethod.Network;
	        client.UseDefaultCredentials = false;
	        client.Credentials = new NetworkCredential (user, pass);

	        mail.IsBodyHtml = true;
	        mail.Subject = subject;
	        mail.Body = msg;

	        if (files != null && files.Length != 0)
	        {
	        	for (int i = 0; i < files.Length; i++)
	        		mail.Attachments.Add (new Attachment (files[i]));
	        }

	        client.Send(mail);
		}

		// Similar to SendMail/9 but the SMTP details are taken from the config object.
		public static void SendMail (Config config, string from, string fromName, string to, string subject, string message)
		{
			SendMail (config.get("smtpHost"), config.getInt("smtpPort"), config.get("smtpUser"), config.get("smtpPass"), from, fromName, to, subject, message, null);
		}

		// Similar to SendMail/6 but the "from" field are taken from the config object.
		public static void SendMail (Config config, string to, string subject, string message)
		{
			SendMail (config.get("smtpHost"), config.getInt("smtpPort"), config.get("smtpUser"), config.get("smtpPass"), config.get("smtpFrom"), config.get("smtpFromName"), to, subject, message, null);
		}

		// Similar to SendMail/4 but the "to" field is taken from the config object.
		public static void SendMail (Config config, string subject, string message)
		{
			SendMail (config.get("smtpHost"), config.getInt("smtpPort"), config.get("smtpUser"), config.get("smtpPass"), config.get("smtpFrom"), config.get("smtpFromName"), config.get("smtpTo"), subject, message, null);
		}

		// Similar to SendMail/9 but the SMTP details are taken from the config object. [Supports attachments].
		public static void SendMail (Config config, string from, string fromName, string to, string subject, string message, string[] files)
		{
			SendMail (config.get("smtpHost"), config.getInt("smtpPort"), config.get("smtpUser"), config.get("smtpPass"), from, fromName, to, subject, message, files);
		}

		// Similar to SendMail/6 but the "from" field are taken from the config object. [Supports attachments].
		public static void SendMail (Config config, string to, string subject, string message, string[] files)
		{
			SendMail (config.get("smtpHost"), config.getInt("smtpPort"), config.get("smtpUser"), config.get("smtpPass"), config.get("smtpFrom"), config.get("smtpFromName"), to, subject, message, files);
		}

		// Similar to SendMail/4 but the "to" field is taken from the config object. [Supports attachments].
		public static void SendMail (Config config, string subject, string message, string[] files)
		{
			SendMail (config.get("smtpHost"), config.getInt("smtpPort"), config.get("smtpUser"), config.get("smtpPass"), config.get("smtpFrom"), config.get("smtpFromName"), config.get("smtpTo"), subject, message, files);
		}
	}
}
