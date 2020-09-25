using System;
using System.Windows.Forms;
using ADF.Net.Service;

namespace ADF.Net.Desktop.WindowsFormApp
{
    public partial class MainForm : Form
    {
        private readonly ICategoryService _serviceCategory;
        private CategoryForm _formCategory;

        private readonly IProductService _serviceProduct;
        private ProductForm _formProduct;
        public MainForm(ICategoryService serviceCategory, IProductService serviceProduct)
        {
            _serviceCategory = serviceCategory;
            _serviceProduct = serviceProduct;
            InitializeComponent();
        }

        private void BtnCategory_Click(object sender, EventArgs e)
        {
            _formCategory = new CategoryForm(_serviceCategory);
            _formCategory.Show();
            Hide();
        }

        private void BtnProduct_Click(object sender, EventArgs e)
        {
            _formProduct = new ProductForm(_serviceProduct);
            _formProduct.Show();
        }
    }
}
