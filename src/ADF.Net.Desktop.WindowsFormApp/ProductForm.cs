using System.Windows.Forms;
using ADF.Net.Service;

namespace ADF.Net.Desktop.WindowsFormApp
{
    public partial class ProductForm : Form
    {

        public ProductForm(IProductService serviceProduct)
        {
            InitializeComponent(serviceProduct);
        }
    }
}