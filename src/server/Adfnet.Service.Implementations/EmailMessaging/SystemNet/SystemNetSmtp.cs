using System.Collections.Generic;
using System.Net;
using System.Net.Mail;
using System.Text;
using Adfnet.Core;
using Adfnet.Core.Constants;
using Adfnet.Core.Enums;
using Adfnet.Core.Helpers;
using Adfnet.Core.ValueObjects;

namespace Adfnet.Service.Implementations.EmailMessaging.SystemNet
{
    public class SystemNetSmtp : ISmtp
    {
        private readonly CustomSmtpClient _smtpClient;
        private readonly ApplicationSettings _applicationSettings;
        public SystemNetSmtp()
        {
            _smtpClient = new CustomSmtpClient
            {
                EnableSsl = _applicationSettings.SmtpSsl,
                Host = _applicationSettings.SmtpServer,
                Port = _applicationSettings.SmtpPort,
                Username = _applicationSettings.SmtpUser,
                Password = _applicationSettings.SmtpPassword,
                UseDefaultCredentials = _applicationSettings.UseDefaultCredentials,
                UseDefaultNetworkCredentials = _applicationSettings.UseDefaultNetworkCredentials
            };
        }

        public void Send(EmailMessage eMailMessage)
        {
            var mailMessage = new MailMessage
            {
                From = new MailAddress(eMailMessage.From.Address, eMailMessage.From.DisplayName),
                HeadersEncoding = Encoding.UTF8,
                BodyEncoding = Encoding.UTF8,
                Subject = eMailMessage.Subject,
                Body = eMailMessage.Body,
                IsBodyHtml = eMailMessage.IsBodyHtml,
                Priority = eMailMessage.Priority.ToString().ToEnum<MailPriority>()
            };
            foreach (var mailAddress in eMailMessage.To)
            {
                mailMessage.To.Add(mailAddress.Address);
            }
            if (eMailMessage.Cc != null)
            {
                foreach (var mailAddress in eMailMessage.Cc)
                {
                    mailMessage.CC.Add(mailAddress.Address);
                }
            }

            if (eMailMessage.Bcc != null)
            {
                foreach (var mailAddress in eMailMessage.Bcc)
                {
                    mailMessage.Bcc.Add(mailAddress.Address);
                }
            }

            if (eMailMessage.Attachments != null)
            {
                foreach (var eMailAttachment in eMailMessage.Attachments)
                {
                    mailMessage.Attachments.Add(new Attachment(eMailAttachment.ContentStream, eMailAttachment.Name, eMailAttachment.MediaType));
                }
            }

            var smtpClient = new SmtpClient
            {
                Host = _smtpClient.Host,
                Port = _smtpClient.Port,
                EnableSsl = _smtpClient.EnableSsl
            };

            if (!_smtpClient.UseDefaultCredentials)
            {
                if (!_smtpClient.UseDefaultNetworkCredentials)
                {
                    smtpClient.Credentials = new NetworkCredential
                    {
                        UserName = _smtpClient.Username,
                        Password = _smtpClient.Password
                    };

                    smtpClient.DeliveryMethod = SmtpDeliveryMethod.Network;
                }
                else
                {
                    smtpClient.UseDefaultCredentials = true;
                }
            }
            else
            {
                smtpClient.UseDefaultCredentials = true;
            }
            smtpClient.Send(mailMessage);
        }

        public void SendWithTemplate(EmailMessage emailMessage, string emailTemplate, List<EmailRow> emailRows)
        {
            foreach (var emailRow in emailRows)
            {
                emailMessage.Body = emailTemplate;
                var toEmail = new EmailAddress();
                foreach (var emailKey in emailRow.EmailKeys)
                {
                    if (emailKey.Key != EmailKeyOption.Name)
                    {
                        if (emailKey.Key == EmailKeyOption.ToEmail)
                        {
                            toEmail.Address = emailKey.Value;
                            continue;
                        }
                        emailMessage.Body = emailMessage.Body.TemplateParser(string.Format(EmailConstants.EmailKeyRegEx, emailKey.Key), emailKey.Value);
                    }
                    else
                    {
                        toEmail.DisplayName = emailKey.Value;
                    }
                }
                Send(emailMessage);
            }
        }
    }
}