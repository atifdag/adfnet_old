using ADF.Net.Service;
using ADF.Net.Service.Models;
using ADF.Net.Web.Common;

namespace ADF.Net.Web.Api.Controllers
{

    public class CategoriesController : BaseCrudApiController<CategoryModel>
    {
        public CategoriesController(IMainService serviceMain, ICategoryService serviceCategory) : base(serviceMain, serviceCategory)
        {
        }
    }
}
