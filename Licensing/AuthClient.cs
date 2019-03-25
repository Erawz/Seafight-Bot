using System;
using Newtonsoft.Json.Linq;
using System.Windows.Forms;
using BoxyBot.HttpWeb;
using BoxyBot.Seafight;
using System.Diagnostics;
using System.Threading;
using BoxyBot.Util;
using BoxyBot.Seafight.Messages;

namespace BoxyBot.Licensing
{
    public class AuthClient
    {
        private const string Password = "32252821674067076967647077869621";
        public DateTime validUntil;
        public string timeleft;
        public string status = "-1";
        public string versionStatus = "0";
        public string message = "Thank you for using BoxyBot!";
        public bool isUnlimited;

        public void LicenseThread()
        {
            var _restartCounter = 0;
            IL_BEGIN:
            try
            {
                while (true)
                {
                    try
                    {
                        this.Authenticate();
                        var versionflag = CheckVersion();
                        if (!versionflag)
                        {
                            if (!Bot.Running && !BotLogic.OnAttackRunning && !BotSession.logginIn && !BotSession.loggedIn)
                            {
                                if (MessageBox.Show("A new version is available!\nDo you wish to visit the download-area now?", "New Version!", MessageBoxButtons.YesNo, MessageBoxIcon.Information) == DialogResult.Yes)
                                {
                                    Process.Start("https://www.elitepvpers.com/forum/seafight/4222822-release-boxybot.html");
                                }
                            }
                            BotMethods.WriteLine("A newer Version is available!");
                        }
                        if (status == "-4")
                        {
                            BotMethods.WriteLine("License Suspended!");
                            MessageBox.Show("Your License has been suspended!", "License suspended!");
                            Account.gClass = null;
                        }
                        else if (status == "-3")
                        {
                            BotMethods.WriteLine(string.Join("", new string[]
                            {
                            HelpTools.FromByteToString(32),
                            HelpTools.FromByteToString(45),
                            HelpTools.FromByteToString(32),
                            HelpTools.FromByteToString(76),
                            HelpTools.FromByteToString(105),
                            HelpTools.FromByteToString(99),
                            HelpTools.FromByteToString(101),
                            HelpTools.FromByteToString(110),
                            HelpTools.FromByteToString(115),
                            HelpTools.FromByteToString(01),
                            HelpTools.FromByteToString(32),
                            HelpTools.FromByteToString(69),
                            HelpTools.FromByteToString(120),
                            HelpTools.FromByteToString(112),
                            HelpTools.FromByteToString(105),
                            HelpTools.FromByteToString(114),
                            HelpTools.FromByteToString(101),
                            HelpTools.FromByteToString(100),
                            HelpTools.FromByteToString(33),
                            }));
                            try
                            {
                                if (Bot.Running)
                                {
                                    BotLogic.StopBotThread();
                                }
                                if (BotLogic.OnAttackRunning)
                                {
                                    BotLogic.StoponAttackThrad();
                                }
                            }
                            catch (Exception ex)
                            {
                                BotMethods.WriteLine("Could not shutdown Bot-Threads!\n" + ex);
                            }
                                Bot.Running = false;
                                BotLogic.OnAttackRunning = false;
                                Account.gClass = null;
                            if (MessageBox.Show("Your License has expired!\nDo you want to buy a new one?", "License Expired!", MessageBoxButtons.YesNo, MessageBoxIcon.Information) == DialogResult.Yes)
                            {
                                Form1.form1.ShowPaymentForm();
                            }
                            Account.gClass = null;
                        }
                        else if (status == "-1")
                        {
                            BotMethods.WriteLine("There was an internal Server Error!\nPlease check your internet connection!\nRetrying in 3min!");
                            Account.gClass = null;
                        }
                        else if (status == "1")
                        {
                            if (Account.gClass == null)
                            {
                                Account.gClass = new UserInitMessage();
                            }
                        }
                        var ts = (validUntil - DateTime.Now);
                        if (ts.TotalHours < 25.0)
                        {
                            if (HelpTools.IsVirtualMachine())
                            {
                                try
                                {
                                    BotCalculator.StopThreads();
                                    if (Bot.Running)
                                    {
                                        BotLogic.StopBotThread();
                                    }
                                    if (BotLogic.OnAttackRunning)
                                    {
                                        BotLogic.StoponAttackThrad();
                                    }  
                                }
                                catch (Exception)
                                {
                                }
                                Bot.Running = false;
                                BotLogic.OnAttackRunning = false;
                                Account.gClass = null;
                                if (MessageBox.Show("This program does not work on a Virtual Machine unless you own a Premium License!", "VM Detected!", MessageBoxButtons.OK, MessageBoxIcon.Error) != DialogResult.None)
                                {
                                    Application.Exit();
                                    Process.GetCurrentProcess().Kill();
                                }
                            }
                        }
                        Thread.Sleep(180000);
                    }
                    catch (Exception ex)
                    {
                        BotMethods.WriteLine("There was an error while checking License!\nRetrying in a few seconds!\n" + ex.Message);
                        Thread.Sleep(50000);
                    }
                }
            } catch (Exception ex)
            {
                BotMethods.WriteLine("Restarting License-Thread, reason: " + ex.Message);
                _restartCounter++;
                if (_restartCounter > 14)
                {
                    MessageBox.Show("There seems to be an error with the License-Thread!\nReason: " + ex + "\nPlease report this!\nThe Bot will now stop!", "Error!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    Application.Exit();
                    Process.GetCurrentProcess().Kill();
                }
                goto IL_BEGIN;
            }
        }

        public bool Authenticate()
        {
            var _version = Environment.OSVersion;
            var username = Environment.UserName;
            var processorCount = Environment.ProcessorCount;
            var httpClient = new HttpClient
            {
                UserAgent = $"BoxyBot Windows:{_version}, User:{username}, Count:{processorCount}"
            };
            var request = new RequestBuilder();
            var iv = "";
            var rnd = new Random();
            for (var i = 0; i < 32; i++)
            {
                iv += rnd.Next(0, 9).ToString();
            }
            request.Add("AuthKey", iv); 
            request.Add("Identifier", Crypt.Encrypt(Program.fingerprint, Password,iv));
            var ts = (long) (DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0)).TotalSeconds;
            request.Add("Timestamp", Crypt.Encrypt((ts.ToString()),Password,iv));
            var response = httpClient.Request(request, "http://sinlyu.me/boxybot/auth/api.php");
            return this.HandleRequest(Crypt.Decrypt(response, Password, iv));
        }

        public bool CheckVersion()
        {
            var httpClient = new HttpClient
            {
                UserAgent = $"BoxyBot Version Check"
            };
            var request = new RequestBuilder();
            var iv = "";
            var rnd = new Random();
            for (var i = 0; i < 32; i++)
            {
                iv += rnd.Next(0, 9).ToString();
            }
            request.Add("AuthKey", iv);
            request.Add("Identifier", Crypt.Encrypt(Program.fingerprint, Password, iv));
            request.Add("Version", Program.BountyBotVersion);
            var response = httpClient.Request(request, "http://sinlyu.me/boxybot/auth/version.php");
            var recieved = Crypt.Decrypt(response, Password, iv);
            JArray jsonData = null;
            if (recieved.Contains("["))
            {
                jsonData = JArray.Parse(recieved);
                versionStatus = jsonData.Last.SelectToken("status").ToString();
            }
            else
            {
                versionStatus = JObject.Parse(recieved).SelectToken("status").ToString();
            }
            if (versionStatus != "1")
            {
                return false;
            } 
            return true;
        }
        
        public bool CheckForStartUpMessage()
        {
            var httpClient = new HttpClient
            {
                UserAgent = $"BoxyBot Message Check"
            };
            var request = new RequestBuilder();
            var help = new HelpTools();
            request.Add("Message", "1.0");
            var response = httpClient.Request(request, "http://sinlyu.me/boxybot/auth/messages.php");
            this.message = string.Empty;
            if (response.Length > ("{\"message\":\"\"}").Length + 2)
            {
                this.message = help.Between(response, "message\":\"", "\"}").Replace("\\", "");
                return true;
            }
            return false;
        }

       // public bool SendStatistics()
       // {
        //    var httpClient = new HttpClient
        //    {
        //        UserAgent = $"BoxyBot Statistics",
        //   };
        //    var request = new RequestBuilder();
		//	var iv = "";
		//	var rnd = new Random();
		//	for (var i = 0; i < 32; i++)
		//	{
		//		iv += rnd.Next(0, 9).ToString();
		//	}
		//	request.Add("AuthKey", iv);
		//	request.Add("Identifier", Crypt.Encrypt(Program.fingerprint, Password, iv));
        //    request.Add("Server", Crypt.Encrypt(Account.ProjectID.ToString(), Password, iv));
        //    request.Add("StartTime", BotSession.sessionStartTime.ToString());
        //    request.Add("Reconnects", BotSession.Sessionreconnects.ToString());
        //    request.Add("Deaths", BotSession.Sessiondeaths.ToString());
        //    return httpClient.Request(request, "http://sinlyu.me/boxybot/auth/stats.php").Length > 0;
       // }

        private bool HandleRequest(string response)
        {
            JArray jsonData = null;
            if (response.Contains("["))
            {
                jsonData = JArray.Parse(response);
                status = jsonData.Last.SelectToken("status").ToString();
            }
            else
            {
                status = JObject.Parse(response).SelectToken("status").ToString();
            }

            switch (status)
            {
                case "-4":
                    return false;
                case "-3":
                    return false;
                case "-2":
                    timeleft = jsonData.First.SelectToken("timeleft").ToString();
                    validUntil = DateTime.Parse(timeleft);
                    return true;
                case "-1":
                    return false;
                case "1":
                    timeleft = jsonData.First.SelectToken("timeleft").ToString();
                    validUntil = DateTime.Parse(timeleft);
                    if (timeleft.Contains("2099"))
                    {
                        timeleft = "unlimited";
                        this.isUnlimited = true;
                    }
                    return true;
                default:
                    return false;
            }
        }
    }
}
