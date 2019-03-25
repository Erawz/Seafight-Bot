namespace BoxyBot
{
    partial class PaymentForm
    {
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(PaymentForm));
            this.keyTextbox = new System.Windows.Forms.TextBox();
            this.paymentBrowser = new System.Windows.Forms.WebBrowser();
            this.SuspendLayout();
            // 
            // keyTextbox
            // 
            this.keyTextbox.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.keyTextbox.Enabled = false;
            this.keyTextbox.Location = new System.Drawing.Point(0, 481);
            this.keyTextbox.Name = "keyTextbox";
            this.keyTextbox.Size = new System.Drawing.Size(584, 20);
            this.keyTextbox.TabIndex = 0;
            // 
            // paymentBrowser
            // 
            this.paymentBrowser.Dock = System.Windows.Forms.DockStyle.Fill;
            this.paymentBrowser.Location = new System.Drawing.Point(0, 0);
            this.paymentBrowser.MinimumSize = new System.Drawing.Size(20, 20);
            this.paymentBrowser.Name = "paymentBrowser";
            this.paymentBrowser.ScriptErrorsSuppressed = true;
            this.paymentBrowser.Size = new System.Drawing.Size(584, 481);
            this.paymentBrowser.TabIndex = 1;
            // 
            // buyForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(584, 501);
            this.Controls.Add(this.paymentBrowser);
            this.Controls.Add(this.keyTextbox);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "buyForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "BoxyBot - Buy License: ";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox keyTextbox;
        private System.Windows.Forms.WebBrowser paymentBrowser;
    }
}