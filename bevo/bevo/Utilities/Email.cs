using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Net.Mail;
using System.Net;
using System.IO;
using System.Net.Mime;

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
            String finalMessage = emailBody + "\n\n Comments, questions, concerns? Contact us at test@example.com.";

                // <img src= ”” alt=”UT_Campus” title=”UT_Campus” height=”220” width=”490” />";


            // Code for adding a picture to the email.
            //AlternateView av = AlternateView.CreateAlternateViewFromString(body, null, isHTML ? System.Net.Mime.MediaTypeNames.Text.Html : System.Net.Mime.MediaTypeNames.Text.Plain);

            //LinkedResource logo = new LinkedResource("SomeRandomValue", System.Net.Mime.MediaTypeNames.Image.Jpeg);
            //logo.ContentId = currentLinkedResource.Key;
            //logo.ContentType = new System.Net.Mime.ContentType("image/jpg");

            //av.LinkedResources.Add(logo);



            //Create an email address object for the sender address
            MailAddress senderEmail = new MailAddress("patrickmis375@gmail.com", "Team 1");


            MailMessage mm = new MailMessage();
            mm.Subject = "Team 1 - " + emailSubject;
            mm.Sender = senderEmail;
            mm.From = senderEmail;
            mm.To.Add(new MailAddress(toEmailAddress));
            mm.Body = finalMessage;
            mm.IsBodyHtml = true;
            //for adding an image in:
            //mm.AlternateViews.Add(av);
            client.Send(mm);
            //NOTE: Works but receiving gmail account needs to have security settings switched to "Less Secure"
        }

    }
}

