using System;
using System.Threading;
using System.Windows.Forms;
using Adfnet.Core.Globalization;
using Adfnet.Core.Helpers;
using Adfnet.Core.Security;
using Adfnet.Service;
using Adfnet.Service.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Index = Adfnet.Desktop.Forms.UserForms.Index;

namespace Adfnet.Desktop.Forms.AuthenticationForms
{
    public partial class Login : Form
    {
        private readonly IConfiguration _configuration;
        private readonly IAuthenticationService _serviceAuthentication;
        private readonly IIdentityService _serviceIdentity;
        public Login(IServiceProvider serviceProvider, IAuthenticationService serviceAuthentication, IIdentityService serviceIdentity, IConfiguration configuration)
        {
            _serviceAuthentication = serviceAuthentication;
            _serviceIdentity = serviceIdentity;
            _configuration = configuration;
            _serviceProvider = serviceProvider;
            InitializeComponent();
        }

        private void LnkRegister_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Hide();
            _serviceProvider.GetRequiredService<Register>().Show();
        }

        private void BtnLogin_Click(object sender, EventArgs e)
        {
            
            try
            {
                var model = new LoginModel
                {
                    Username = TxbUsername.Text,
                    Password = TxbPassword.Text,
                    Key = _configuration.GetSection("JwtSecurityKey").Value
                };
                _serviceAuthentication.Login(model);
                var identity = (CustomIdentity)Thread.CurrentPrincipal.Identity;
                _serviceIdentity.Set(identity, DateTime.Now.AddMinutes(20), false);
                Hide();
                _serviceProvider.GetRequiredService<MainForm>().Show();
            }
            catch (Exception exception)
            {
                MessageBox.Show(exception.Message);
            }
        }
    }
}
