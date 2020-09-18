using ADF.Net.Service;
using ADF.Net.Service.Models;
using ADF.Net.Web.Common;

namespace ADF.Net.Web.Api.Controllers
{

    public class ProductsController : BaseCrudApiController<ProductModel>
    {
        public ProductsController(IProductService service) : base(service)
        {
        }

       
    }
}
