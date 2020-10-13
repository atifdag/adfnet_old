using Adfnet.Core.ValueObjects;
using System.Collections.Generic;

namespace Adfnet.Core
{
    public interface ISmtp
    {
        void Send(EmailMessage eMailMessage);
        void SendWithTemplate(EmailMessage emailMessage, string emailTemplate, List<EmailRow> emailRows);
    }
}