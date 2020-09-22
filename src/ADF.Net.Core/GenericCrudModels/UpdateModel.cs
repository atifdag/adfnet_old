namespace ADF.Net.Core.GenericCrudModels
{

    public class UpdateModel<T> where T : class, IServiceModel, new()
    {
        public T Item { get; set; }
        public string Message { get; set; }
    }
}