
using System;
using Adfnet.Core.Globalization;

namespace Adfnet.Desktop.WindowsForms.Forms.AuthenticationForms
{
    partial class LoginForm
    {
        private readonly IServiceProvider _serviceProvider;
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.BtnLogin = new System.Windows.Forms.Button();
            this.LblPassword = new System.Windows.Forms.Label();
            this.TxbPassword = new System.Windows.Forms.TextBox();
            this.LblUsername = new System.Windows.Forms.Label();
            this.TxbUsername = new System.Windows.Forms.TextBox();
            this.LnkRegister = new System.Windows.Forms.LinkLabel();
            this.LnkForgotPassword = new System.Windows.Forms.LinkLabel();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.BtnLogin);
            this.groupBox1.Controls.Add(this.LblPassword);
            this.groupBox1.Controls.Add(this.TxbPassword);
            this.groupBox1.Controls.Add(this.LblUsername);
            this.groupBox1.Controls.Add(this.TxbUsername);
            this.groupBox1.Location = new System.Drawing.Point(13, 13);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(381, 158);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = Dictionary.Login;
            // 
            // BtnLogin
            // 
            this.BtnLogin.Location = new System.Drawing.Point(290, 118);
            this.BtnLogin.Name = "BtnLogin";
            this.BtnLogin.Size = new System.Drawing.Size(75, 23);
            this.BtnLogin.TabIndex = 4;
            this.BtnLogin.Text = Dictionary.Login;
            this.BtnLogin.UseVisualStyleBackColor = true;
            this.BtnLogin.Click += new System.EventHandler(this.BtnLogin_Click);
            // 
            // LblPassword
            // 
            this.LblPassword.AutoSize = true;
            this.LblPassword.Location = new System.Drawing.Point(17, 82);
            this.LblPassword.Name = "LblPassword";
            this.LblPassword.Size = new System.Drawing.Size(40, 15);
            this.LblPassword.TabIndex = 3;
            this.LblPassword.Text = Dictionary.Password;
            // 
            // TxbPassword
            // 
            this.TxbPassword.Location = new System.Drawing.Point(115, 79);
            this.TxbPassword.Name = "TxbPassword";
            this.TxbPassword.Size = new System.Drawing.Size(250, 23);
            this.TxbPassword.TabIndex = 2;
            // 
            // LblUsername
            // 
            this.LblUsername.AutoSize = true;
            this.LblUsername.Location = new System.Drawing.Point(17, 41);
            this.LblUsername.Name = "LblUsername";
            this.LblUsername.Size = new System.Drawing.Size(73, 15);
            this.LblUsername.TabIndex = 1;
            this.LblUsername.Text = Dictionary.Username;
            // 
            // TxbUsername
            // 
            this.TxbUsername.Location = new System.Drawing.Point(115, 38);
            this.TxbUsername.Name = "TxbUsername";
            this.TxbUsername.Size = new System.Drawing.Size(250, 23);
            this.TxbUsername.TabIndex = 0;
            // 
            // LnkRegister
            // 
            this.LnkRegister.AutoSize = true;
            this.LnkRegister.Location = new System.Drawing.Point(113, 188);
            this.LnkRegister.Name = "LnkRegister";
            this.LnkRegister.Size = new System.Drawing.Size(48, 15);
            this.LnkRegister.TabIndex = 1;
            this.LnkRegister.TabStop = true;
            this.LnkRegister.Text = Dictionary.Register;
            this.LnkRegister.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.LnkRegister_LinkClicked);
            // 
            // LnkForgotPassword
            // 
            this.LnkForgotPassword.AutoSize = true;
            this.LnkForgotPassword.Location = new System.Drawing.Point(171, 188);
            this.LnkForgotPassword.Name = "LnkForgotPassword";
            this.LnkForgotPassword.Size = new System.Drawing.Size(95, 15);
            this.LnkForgotPassword.TabIndex = 2;
            this.LnkForgotPassword.TabStop = true;
            this.LnkForgotPassword.Text = Dictionary.IForgotMyPassword;
            // 
            // Login
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(406, 226);
            this.Controls.Add(this.LnkForgotPassword);
            this.Controls.Add(this.LnkRegister);
            this.Controls.Add(this.groupBox1);
            this.Name = "Login";
            this.Text = Dictionary.Login;
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Button BtnLogin;
        private System.Windows.Forms.Label LblPassword;
        private System.Windows.Forms.TextBox TxbPassword;
        private System.Windows.Forms.Label LblUsername;
        private System.Windows.Forms.TextBox TxbUsername;
        private System.Windows.Forms.LinkLabel LnkRegister;
        private System.Windows.Forms.LinkLabel LnkForgotPassword;
    }
}