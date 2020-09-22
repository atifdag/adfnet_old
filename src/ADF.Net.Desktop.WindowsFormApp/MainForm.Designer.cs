namespace ADF.Net.Desktop.WindowsFormApp
{
    partial class MainForm
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
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
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.BtnCategory = new System.Windows.Forms.Button();
            this.BtnProduct = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // BtnCategory
            // 
            this.BtnCategory.Location = new System.Drawing.Point(29, 43);
            this.BtnCategory.Name = "BtnCategory";
            this.BtnCategory.Size = new System.Drawing.Size(89, 23);
            this.BtnCategory.TabIndex = 0;
            this.BtnCategory.Text = "Kategoriler";
            this.BtnCategory.UseVisualStyleBackColor = true;
            this.BtnCategory.Click += new System.EventHandler(this.BtnCategory_Click);
            // 
            // BtnProduct
            // 
            this.BtnProduct.Location = new System.Drawing.Point(158, 43);
            this.BtnProduct.Name = "BtnProduct";
            this.BtnProduct.Size = new System.Drawing.Size(75, 23);
            this.BtnProduct.TabIndex = 1;
            this.BtnProduct.Text = "Ürünler";
            this.BtnProduct.UseVisualStyleBackColor = true;
            this.BtnProduct.Click += new System.EventHandler(this.BtnProduct_Click);
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1280, 450);
            this.Controls.Add(this.BtnProduct);
            this.Controls.Add(this.BtnCategory);
            this.Name = "MainForm";
            this.Text = "Form1";
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button BtnCategory;
        private System.Windows.Forms.Button BtnProduct;
    }
}

