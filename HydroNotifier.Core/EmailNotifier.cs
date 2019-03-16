using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Mail;
using System.Text;

namespace HydroNotifier.Core
{
    public class EmailNotifier
    {
        public void SendEmailNotification(ILogger log)
        {
            using (SmtpClient client = new SmtpClient("smtp.gmail.com"))
            {
                client.UseDefaultCredentials = false;
                client.Credentials = new NetworkCredential("", "");
                client.Port = 587;
                client.EnableSsl = true;
                client.DeliveryMethod = SmtpDeliveryMethod.Network;

                using (MailMessage mailMessage = new MailMessage())
                {
                    mailMessage.From = new MailAddress("hydronotifier@noreply.com");
                    mailMessage.To.Add("");
                    mailMessage.Body = "body";
                    mailMessage.Subject = "HydroNotifier";
                    client.Send(mailMessage);
                }
            }
        }
    }
}
