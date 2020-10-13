namespace Adfnet.Core.ValueObjects
{
    public struct CustomSmtpClient
    {
        public bool UseDefaultNetworkCredentials { get; set; }
        public bool UseDefaultCredentials { get; set; }
        public string Host { get; set; }
        public int Port { get; set; }
        public bool EnableSsl { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
    }
}
