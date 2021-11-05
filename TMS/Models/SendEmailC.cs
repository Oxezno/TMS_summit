using Microsoft.Ajax.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Web;
using System.Web.Configuration;

namespace SummitWebsite.Models
{
    public class SendEmailC
    {
        public async System.Threading.Tasks.Task sendEmailAsync(string subject, string body, string email)
        {
            var message = new MailMessage();
            message.To.Add(new MailAddress(email));  // replace with valid value 
            message.From = new MailAddress(WebConfigurationManager.AppSettings["emailAccount"]);  // replace with valid value
            message.Subject = subject;
            message.Body = string.Format(body, message.Subject, WebConfigurationManager.AppSettings["emailPassword"], message);
            //message.Body = string.Format(body, subject, cOMPANY.CREDENTIAL_PASSWORD, message);

            message.IsBodyHtml = true;

            using (var smtp = new SmtpClient())
            {
                var credential = new NetworkCredential
                {
                    UserName = WebConfigurationManager.AppSettings["emailAccount"],  // replace with valid value
                    Password = WebConfigurationManager.AppSettings["emailPassword"]  // replace with valid value
                };
                smtp.Credentials = credential;
                smtp.Host = WebConfigurationManager.AppSettings["emailHost"];
                smtp.Port = Convert.ToInt32(WebConfigurationManager.AppSettings["emailPort"]);
                smtp.EnableSsl = true;
                await smtp.SendMailAsync(message);

            }
        }
    }
}