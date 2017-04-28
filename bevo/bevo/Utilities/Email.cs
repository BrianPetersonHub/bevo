using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Net.Mail;
using System.Net;

namespace bevo.Messaging
{
    // username: patricksmis375@gmail.com
    // password: ABC123def

    public class EmailMessaging
    {
        public static void SendEmail(String toEmailAddress, String emailSubject, String emailBody)
        {

            //Create an email client to send the emails
            var client = new SmtpClient("smtp.gmail.com", 587)
            {
                Credentials = new NetworkCredential("patricksmis375@gmail.com", "ABC123def"),
                EnableSsl = true
            };

            //Add anything that you need to the body of the message
            // /n is a new line – this will add some white space after the main body of the message
            String finalMessage = emailBody + "\n\n This is a disclaimer that will be on all messages. ";

            //Create an email address object for the sender address
            MailAddress senderEmail = new MailAddress("patrickmis375@gmail.com", "Team 1");


            MailMessage mm = new MailMessage();
            mm.Subject = "Team 1 - " + emailSubject;
            mm.Sender = senderEmail;
            mm.From = senderEmail;
            mm.To.Add(new MailAddress(toEmailAddress));
            mm.Body = finalMessage;
            client.Send(mm);
            //NOTE: Works but receiving gmail account needs to have security settings switched to "Less Secure"
        }

    }
}

