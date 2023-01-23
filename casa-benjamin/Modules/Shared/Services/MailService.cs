using System;
using System.Diagnostics;
using System.Net;
using System.Net.Mail;

namespace casa_benjamin.Modules.Shared.Services
{
    public class MailService
    {        
        public static void SendMail(string smtpUser, string smtpPassword, string title, string body, string from, string to)
        {
            try
            {             
                var client = new SmtpClient("smtp.gmail.com", 587)
                {                   
                    EnableSsl = true,
                    UseDefaultCredentials = true
                };
                client.Credentials = new NetworkCredential(smtpUser, smtpPassword);

                System.Net.ServicePointManager.ServerCertificateValidationCallback = delegate (object s,
                       System.Security.Cryptography.X509Certificates.X509Certificate certificate,
                       System.Security.Cryptography.X509Certificates.X509Chain chain,
                       System.Net.Security.SslPolicyErrors sslPolicyErrors)
                {
                    return true;
                };

                MailMessage msg = new MailMessage(from, to, title, body);
                msg.IsBodyHtml = true;
                client.Send(msg);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }

        }

    }
}