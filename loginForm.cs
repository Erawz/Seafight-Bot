using BoxyBot.HttpWeb;
using BoxyBot.Licensing;
using BoxyBot.Properties;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Windows.Forms;

namespace BoxyBot
{
    public partial class LoginForm : Form
    {
        public LoginForm()
        {
            InitializeComponent();
            this.loginNameTxtbox.Text = Settings.Default.login_username;
            this.loginPassTxtbox.Text = Settings.Default.login_password;
        }

        private void RegisterLinkLabel_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Process.Start("");
        }

        private void LoginButton_Click(object sender, EventArgs e)
        {
            var httpClient = new HttpClient
            {
                UserAgent = $"BoxyBot Login"
            };
            var request = new RequestBuilder();
            var iv = "";
            var rnd = new Random();
            for (var i = 0; i < 32; i++)
            {
                iv += rnd.Next(0, 9).ToString();
            }
            request.Add("AuthKey", iv);
            request.Add("username", Crypt.Encrypt(this.loginNameTxtbox.Text, "32252821674067076967647077869621", iv));
            request.Add("password", Crypt.Encrypt(this.loginPassTxtbox.Text, "32252821674067076967647077869621", iv));
            request.Add("indentifier", Crypt.Encrypt(Program.fingerprint, "32252821674067076967647077869621", iv));
            var response = httpClient.Request(request, "http://sinlyu.me/boxybot/auth/login.php");
            response = Crypt.Decrypt(response, "32252821674067076967647077869621", iv);
            JArray jsonData = null;
            string status = "0";
            if (response.Contains("["))
            {
                jsonData = JArray.Parse(response);
                status = jsonData.Last.SelectToken("status").ToString();
            }
            else
            {
                status = JObject.Parse(response).SelectToken("status").ToString();
            }
            if (status == "0")
            {
                MessageBox.Show("There was an error while connecting to the Server!", "Connection Error!", MessageBoxButtons.OK, MessageBoxIcon.Error);
            } else if (status == "-4")
            {
                MessageBox.Show("Your Account got banned!", "Banned Account!", MessageBoxButtons.OK, MessageBoxIcon.Error);
            } else if (status == "-3")
            {
                MessageBox.Show("Invalid Account!", "Invalid Account!", MessageBoxButtons.OK, MessageBoxIcon.Error);
            } else if (status == "-2")
            {
                MessageBox.Show("You have entered an invalid Password!", "Invalid Password!", MessageBoxButtons.OK, MessageBoxIcon.Error);
            } else if (status == "-1")
            {
                MessageBox.Show("Your license is expired!", "License Expired!", MessageBoxButtons.OK, MessageBoxIcon.Information);
            } else if (status == "1")
            {
                DialogResult = DialogResult.OK;
            }
        }       
    }
}
