using System;
using System.IO;
using System.Net;
using System.Net.Mail;
using System.Net.Mime;
using System.Threading.Tasks;
using System.Web.Configuration;

namespace TMS.Controllers
{
    internal class SendEmail
    {
        public async System.Threading.Tasks.Task sendEmailAsync(string subject, string body, string email, string attachmentFilename)
        {
            var message = new MailMessage();
            message.To.Add(new MailAddress(email));  // replace with valid value 
            message.From = new MailAddress(WebConfigurationManager.AppSettings["emailAccount"]);  // replace with valid value
            message.Subject = subject;
            message.Body = string.Format(body, message.Subject, WebConfigurationManager.AppSettings["emailPassword"], message);
            //message.Body = string.Format(body, subject, cOMPANY.CREDENTIAL_PASSWORD, message);

            if (attachmentFilename != null)
            {
                Attachment attachment = new Attachment(attachmentFilename, MediaTypeNames.Application.Octet);
                ContentDisposition disposition = attachment.ContentDisposition;
                disposition.CreationDate = File.GetCreationTime(attachmentFilename);
                disposition.ModificationDate = File.GetLastWriteTime(attachmentFilename);
                disposition.ReadDate = File.GetLastAccessTime(attachmentFilename);
                disposition.FileName = Path.GetFileName(attachmentFilename);
                disposition.Size = new FileInfo(attachmentFilename).Length;
                disposition.DispositionType = DispositionTypeNames.Attachment;
                message.Attachments.Add(attachment);
            }


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