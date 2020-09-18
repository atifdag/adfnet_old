namespace ADF.Net.Core.ValueObjects
{
    public struct ApplicationSettings
    {
        public string ApplicationName { get; set; }
        public string ApplicationUrl { get; set; }
        public int DefaultPageSize { get; set; }
        public string PageSizeList { get; set; }

    }
}
