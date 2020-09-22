using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using ADF.Net.Service;
using ADF.Net.Service.GenericCrudModels;

namespace ADF.Net.Desktop.WindowsFormApp
{
    public partial class ProductForm : Form
    {

        public ProductForm(IProductService serviceProduct)
        {
            var items = serviceProduct.List(new FilterModel()).Items;
            InitializeComponent(items);
        }
    }
}