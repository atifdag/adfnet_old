using ADF.Net.Core.ValueObjects;

namespace ADF.Net.Service.Implementations
{
    public class MainService : IMainService
    {
       
        public ApplicationSettings ApplicationSettings => new ApplicationSettings
        {
            ApplicationName = "ADF.Net",
            ApplicationUrl = "http://example.com",
            DefaultPageSize = 10,
            PageSizeList = "10,25,50,100",
        };
    }
}
