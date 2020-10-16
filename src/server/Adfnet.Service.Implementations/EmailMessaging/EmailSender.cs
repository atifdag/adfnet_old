using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Adfnet.Core;
using Adfnet.Core.Constants;
using Adfnet.Core.Enums;
using Adfnet.Core.Globalization;
using Adfnet.Core.Helpers;
using Adfnet.Core.ValueObjects;


namespace Adfnet.Service.Implementations.EmailMessaging
{
    public class EmailSender
    {
        private readonly ApplicationSettings _applicationSettings;
        private readonly ISmtp _smtp;
       

        public EmailSender(IMainService serviceMain, ISmtp smtp)
        {
            _applicationSettings = serviceMain.ApplicationSettings;
            _smtp = smtp;
        }

        private static string ConvertTemplateToString(string emailTemplate, EmailRow emailRow)
        {
            return emailRow.EmailKeys.Aggregate(emailTemplate, (current, key) => current.TemplateParser(EmailConstants.EmailKeyRegEx.Replace(EmailConstants.EmailTokenName, key.Key.ToString()), key.Value));
        }
        public void SendEmailToUser(EmailUser user, EmailTypeOption emailTypes)
        {

            var projectRootPath = AppContext.BaseDirectory.Substring(0, AppContext.BaseDirectory.IndexOf("bin", StringComparison.Ordinal));


            var templateFolderPath = Path.Combine(projectRootPath, _applicationSettings.EmailTemplatePath);
            var templateFilePath = Path.Combine(templateFolderPath, emailTypes + ".html");
            var templateText = FileHelper.ReadAllLines(templateFilePath);

            var from = new EmailAddress
            {
                DisplayName = _applicationSettings.SmtpSenderName,
                Address = _applicationSettings.SmtpSenderMail
            };

            string eMailSubject;

            var emailRow = new EmailRow();

            var emailKeys = new List<EmailKey>
            {
                new EmailKey
                {
                    Key = EmailKeyOption.ApplicationName,
                    Value = _applicationSettings.ApplicationName
                },
                new EmailKey
                {
                    Key = EmailKeyOption.ApplicationUrl,
                    Value = _applicationSettings.ApplicationUrl
                },
                new EmailKey
                {
                    Key = EmailKeyOption.FirstName,
                    Value = user.FirstName
                }
                ,
                new EmailKey
                {
                    Key = EmailKeyOption.LastName,
                    Value = user.LastName
                }
            };

            switch (emailTypes)
            {
                case EmailTypeOption.Add:
                    {
                        eMailSubject = Dictionary.UserInformation;
                        emailKeys.Add(new EmailKey
                        {
                            Key = EmailKeyOption.Username,
                            Value = user.Username
                        });
                        emailKeys.Add(new EmailKey
                        {
                            Key = EmailKeyOption.Password,
                            Value = user.Password
                        });
                        break;
                    }
                case EmailTypeOption.Register:
                    {
                        eMailSubject = Dictionary.UserInformation;
                        emailKeys.Add(new EmailKey
                        {
                            Key = EmailKeyOption.Username,
                            Value = user.Username
                        });
                        emailKeys.Add(new EmailKey
                        {
                            Key = EmailKeyOption.Password,
                            Value = user.Password
                        });
                        emailKeys.Add(new EmailKey
                        {
                            Key = EmailKeyOption.ActivationCode,
                            Value = (user.Id + "@" + user.CreationTime).Encrypt()
                        });
                        break;
                    }
                case EmailTypeOption.ForgotPassword:
                    {
                        eMailSubject = Dictionary.NewPassword;

                        emailKeys.Add(new EmailKey
                        {
                            Key = EmailKeyOption.Username,
                            Value = user.Username
                        });
                        emailKeys.Add(new EmailKey
                        {
                            Key = EmailKeyOption.Password,
                            Value = user.Password
                        });
                        break;
                    }
                case EmailTypeOption.Update:
                    {
                        eMailSubject = Dictionary.UserInformation;
                        break;
                    }

                case EmailTypeOption.UpdateMyPassword:
                    {
                        eMailSubject = Dictionary.NewPassword;
                        emailKeys.Add(new EmailKey
                        {
                            Key = EmailKeyOption.Username,
                            Value = user.Username
                        });
                        emailKeys.Add(new EmailKey
                        {
                            Key = EmailKeyOption.Password,
                            Value = user.Password
                        });
                        break;
                    }
                case EmailTypeOption.UpdateMyInformation:
                    {
                        eMailSubject = Dictionary.UserInformation;
                        break;
                    }
                default:
                    {
                        throw new ArgumentOutOfRangeException(nameof(emailTypes), emailTypes, null);
                    }
            }

            emailKeys.Add(new EmailKey
            {
                Key = EmailKeyOption.Subject,
                Value = eMailSubject
            });

            emailRow.EmailKeys = emailKeys;

            var eMailMessage = new EmailMessage
            {
                To = new List<EmailAddress>
                {
                    new EmailAddress
                    {
                        Address = user.Email
                    }
                },
                Subject = eMailSubject,
                From = from,
                IsBodyHtml = true,
                Priority = EmailPriorityOption.Normal,
                Body = ConvertTemplateToString(templateText, emailRow)
            };

            _smtp.Send(eMailMessage);
        }
    }
}