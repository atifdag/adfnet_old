namespace Adfnet.Core.ValueObjects
{
    public struct ApplicationSettings
    {
        public string Address { get; set; }
        public string ApplicationName { get; set; }
        public string ApplicationUrl { get; set; }
        public string CorporationName { get; set; }
        public string DefaultLanguage { get; set; }
        public int DefaultPageSize { get; set; }
        public string EmailTemplatePath { get; set; }
        public string Fax { get; set; }
        public string MemoryCacheMainKey { get; set; }
        public string PageSizeList { get; set; }
        public string Phone { get; set; }
        public bool SendMailAfterAddUser { get; set; }
        public bool SendMailAfterUpdateUserInformation { get; set; }
        public bool SendMailAfterUpdateUserPassword { get; set; }
        public int SessionTimeOut { get; set; }
        public string SmtpPassword { get; set; }
        public int SmtpPort { get; set; }
        public string SmtpSenderMail { get; set; }
        public string SmtpSenderName { get; set; }
        public string SmtpServer { get; set; }
        public bool SmtpSsl { get; set; }
        public string SmtpUser { get; set; }
        public string TaxAdministration { get; set; }
        public string TaxNumber { get; set; }
        public bool UseDefaultCredentials { get; set; }
        public bool UseDefaultNetworkCredentials { get; set; }

    }
}
