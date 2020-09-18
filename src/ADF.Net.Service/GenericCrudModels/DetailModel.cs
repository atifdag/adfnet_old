namespace ADF.Net.Service.GenericCrudModels
{

    public class DetailModel<T> where T : class, IServiceModel, new()
    {
        public T Item { get; set; }
        public string Message { get; set; }
    }
}