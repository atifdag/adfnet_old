using System.Collections.Generic;
using Adfnet.Core;
using Adfnet.Core.Constants;
using Adfnet.Core.Enums;
using Adfnet.Core.Helpers;
using Adfnet.Core.ValueObjects;
using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;

namespace Adfnet.Service.Implementations.EmailMessaging.MailKit
{
    public class MailKitSmtp : ISmtp
    {
        private readonly CustomSmtpClient _smtpClient;

        public MailKitSmtp(IMainService serviceMain)
        {
            var applicationSettings = serviceMain.ApplicationSettings;
            _smtpClient = new CustomSmtpClient
            {
                EnableSsl = applicationSettings.SmtpSsl,
                Host = applicationSettings.SmtpServer,
                Port = applicationSettings.SmtpPort,
                Username = applicationSettings.SmtpUser,
                Password = applicationSettings.SmtpPassword
            };
        }

        public void Send(EmailMessage eMailMessage)
        {
            var message = new MimeMessage { Subject = eMailMessage.Subject };
            message.From.Add(new MailboxAddress(eMailMessage.From.DisplayName, eMailMessage.From.Address));
            foreach (var mailAddress in eMailMessage.To)
            {
                message.To.Add(new MailboxAddress("", mailAddress.Address));
            }
            if (eMailMessage.Cc != null)
            {
                foreach (var mailAddress in eMailMessage.Cc)
                {
                    message.Cc.Add(new MailboxAddress("", mailAddress.Address));

                }
            }

            if (eMailMessage.Bcc != null)
            {
                foreach (var mailAddress in eMailMessage.Bcc)
                {
                    message.Bcc.Add(new MailboxAddress("", mailAddress.Address));
                }
            }


            var bodyBuilder = new BodyBuilder();
            if (eMailMessage.IsBodyHtml)
            {
                bodyBuilder.HtmlBody = eMailMessage.Body;
                message.Body = bodyBuilder.ToMessageBody();
            }
            else
            {
                message.Body = new TextPart("plain")
                {
                    Text = eMailMessage.Body
                };
            }

            using var smtpClient = new SmtpClient();
            smtpClient.Connect(_smtpClient.Host, _smtpClient.Port, _smtpClient.EnableSsl ? SecureSocketOptions.SslOnConnect : SecureSocketOptions.None);
            if (!_smtpClient.UseDefaultCredentials)
            {
                if (!_smtpClient.UseDefaultNetworkCredentials)
                {
                    smtpClient.Authenticate(_smtpClient.Username, _smtpClient.Password);
                }
            }
            smtpClient.Send(message);
            smtpClient.Disconnect(true);
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