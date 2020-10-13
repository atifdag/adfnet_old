using System.Collections.Generic;
using Adfnet.Core.ValueObjects;

namespace Adfnet.Core.GenericCrudModels
{
    public class ListModel<T> where T : class, IServiceModel, new()
    {

        public Paging Paging { get; set; }

        public List<T> Items { get; set; }

        public string Message { get; set; }

        public bool HasError { get; set; }

    }
}
