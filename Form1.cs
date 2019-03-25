using BoxyBot.Licensing;
using BoxyBot.Properties;
using BoxyBot.Proxy;
using BoxyBot.Seafight;
using BoxyBot.Seafight.Constants;
using BoxyBot.Seafight.Messages;
using BoxyBot.Util;
using Fiddler;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Threading;
using System.Windows.Forms;

namespace BoxyBot
{
    public partial class Form1 : Form
    {
        public static Form1 form1;
        public delegate void writerDelegate(string new_item);
        public delegate void bonusmapDelegate();
        public writerDelegate writer;
        public bonusmapDelegate bonusmap;
        public bool updateFormText;
        private HelpTools help;
        private Thread localThread;
        private DateTime loginTime;

        public Form1()
        {
            InitializeComponent();
            HelpTools.CoInternetSetFeatureEnabled();
            HelpTools.UrlMkSetSessionOption("Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/56.0.2924.87 Safari/537.36");
            this.Init();
            this.OnStartUp();
        }

        private void OnStartUp()
        {
            CertMaker.createRootCert();
            CertMaker.trustRootCert();
            this.Text = "BountyBot - V." + Program.BountyBotVersion;
            this.WriteLine(this.Text);  
            this.CheckLicense();
            this.StartLocalProxy();
        }

        public void CheckLicense()
        {
            try
            {
                if (Bot.auth == null)
                {
                    Bot.auth = new AuthClient();
                }               
                if (Bot.auth.CheckForStartUpMessage())
                {
                    MessageBox.Show(this, Bot.auth.message, "Special Message.", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                if (!Bot.auth.CheckVersion() && !Bot.Running && !BotLogic.OnAttackRunning)
                {
                    this.WriteLine("A new Version is available!");
                    if (MessageBox.Show("A new version is available!\nDo you wish to visit the download-area now?", "New Version!", MessageBoxButtons.YesNo, MessageBoxIcon.Information) == DialogResult.Yes)
                    {
                        Process.Start("https://www.elitepvpers.com/forum/seafight/4222822-release-boxybot.html");
                        Application.Exit();
                        Process.GetCurrentProcess().Kill();
                    }
                }
                var flag = Bot.auth.Authenticate();
                if (flag)
                {
                    if (Bot.auth.status == "-2")
                    {
                        this.WriteLine("License status: Trial");
                        this.WriteLine("License is Valid, until: " + Bot.auth.timeleft);
                    } else if (Bot.auth.status == "1")
                    {
                        this.WriteLine("License status: Premium");
                        this.WriteLine("License is Valid, until: " + Bot.auth.timeleft);
                    }
                    Account.gClass = new UserInitMessage();
                } else
                {
                    if (Bot.auth.status != "-4")
                    {
                        this.WriteLine("License status: Expired");   
                    } else
                    {
                        this.WriteLine("License status: Suspended");
                    }
                    Account.gClass = null;
                }
                if (HelpTools.IsVirtualMachine() && !Bot.auth.isUnlimited)
                {
                    Account.gClass = null;
                    if (MessageBox.Show("This program does not work on a Virtual Machine!", "VM detected!", MessageBoxButtons.OK, MessageBoxIcon.Error) != DialogResult.None)
                    {
                        Application.Exit();
                        Process.GetCurrentProcess().Kill();
                    }
                }
                this.WriteLine("Buy License: " + "http://sinlyu.me/boxybot/buy.php?key=" + Program.fingerprint);
            } catch (Exception ex)
            {
                this.WriteLine("There was an error while checking License: " + ex.Message);
            }
            new Thread(new ThreadStart(Bot.auth.LicenseThread)) { IsBackground = true }.Start();           
        }

        private void Init()
        {
            this.writer = new Form1.writerDelegate(this.WriteLine);
            this.bonusmap = new Form1.bonusmapDelegate(this.UpdateBMList);
            form1 = this;
            this.updateFormText = true;
            this.help = new HelpTools();
            BotHandlers.loadMapNames();
            this.LoadSettings();
            this.CreateSettings();  
        }

        private void UpdateBMList()
        {

            if (InvokeRequired)
            {
                Invoke(new MethodInvoker(UpdateBMList));
                return;
            }
            if (Account.BonusMaps.Count >= 1)
            {
                this.bmComboBox.Items.Clear();
                foreach (var item in Account.BonusMaps.Values)
                {
                    if (!this.bmComboBox.Items.Contains(BotHandlers.MapHandler(item.mapId)))
                    {
                        this.bmComboBox.Items.Add(BotHandlers.MapHandler(item.mapId) + " (" + item.amount + ")");
                    }
                }
            }
            BotMethods.WriteLine("Bonusmaps Loaded.");
        }

        private void UpdateAttackOnSightTextBox(string enemy)
        {
            if (InvokeRequired)
            {
                Invoke(new MethodInvoker(() => UpdateAttackOnSightTextBox(enemy)));
                return;
            }
            if (enemy.Length > 1)
            {
                attackSightTextbox.Text += (enemy.Replace(" ", "") + ";");
            }
        }

        public void UpdateQuestList(string itemName)
        {
            if (InvokeRequired)
            {
                Invoke(new MethodInvoker(() => UpdateQuestList(itemName)));
                return;
            }
            if (itemName.Length > 2)
            {
                for (int i = 0; i < selectedQuestsListbox.CheckedItems.Count; i++)
                {
                    if (selectedQuestsListbox.CheckedItems[i].ToString() == itemName)
                    {
                        selectedQuestsListbox.SetItemCheckState(i, CheckState.Unchecked);
                        break;
                    }
                }
                if (selectedQuestsListbox.Items.Contains(itemName))
                {
                    selectedQuestsListbox.Items.Remove(itemName);
                }
            }
        }

        private void LoadDefaultGridItems(bool npcs = true, bool monsters = true)
        {
            if (npcs)
            {
                npcGridView.Rows.Clear();
                npcGridView.Rows.Add(new object[] { false, "Admiral Jack", "Hollowballs (20)", false, false, false, "0" });
                npcGridView.Rows.Add(new object[] { false, "Admiral Sinclare", "Hollowballs (20)", false, false, false, "0" });
                npcGridView.Rows.Add(new object[] { false, "Admiral Calico", "Hollowballs (20)", false, false, false, "0" });
                npcGridView.Rows.Add(new object[] { false, "Admiral Kiribati", "Hollowballs (20)", false, false, false, "0" });
                npcGridView.Rows.Add(new object[] { false, "Admiral Lucia", "Hollowballs (20)", false, false, false, "0" });
                npcGridView.Rows.Add(new object[] { false, "Admiral Quintor", "Hollowballs (20)", false, false, false, "0" });
                npcGridView.Rows.Add(new object[] { false, "Blackbeard's Pirates", "Hollowballs (20)", false, false, false, "0" });
                npcGridView.Rows.Add(new object[] { false, "Calico's Jack", "Hollowballs (20)", false, false, false, "0" });
                npcGridView.Rows.Add(new object[] { false, "Rackham", "Hollowballs (20)", false, false, false, "0" });
                npcGridView.Rows.Add(new object[] { false, "Red Korsar", "Hollowballs (20)", false, false, false, "0" });
                npcGridView.Rows.Add(new object[] { false, "Los Renegados", "Hollowballs (20)", false, false, false, "0" });
                npcGridView.Rows.Add(new object[] { false, "Wild 13", "Hollowballs (20)", false, false, false, "0" });
                npcGridView.Rows.Add(new object[] { false, "Tortuga Gang", "Hollowballs (20)", false, false, false, "0" });
                npcGridView.Rows.Add(new object[] { false, "Sinclare's Men", "Hollowballs (20)", false, false, false, "0" });
                npcGridView.Rows.Add(new object[] { false, "Rat Pack", "Hollowballs (20)", false, false, false, "0" });
                npcGridView.Rows.Add(new object[] { false, "Calico's Men", "Hollowballs (20)", false, false, false, "0" });
                npcGridView.Rows.Add(new object[] { false, "Los Amistadores", "Hollowballs (20)", false, false, false, "0" });
                npcGridView.Rows.Add(new object[] { false, "Morgan's Buccaneers", "Hollowballs (20)", false, false, false, "0" });
                npcGridView.Rows.Add(new object[] { false, "Flying Dutchman", "Hollowballs (20)", false, false, false, "0" });
                npcGridView.Rows.Add(new object[] { false, "Tiamat", "Hollowballs (20)", false, false, false, "0" });
                npcGridView.Rows.Add(new object[] { false, "Kilimatu", "Hollowballs (20)", false, false, false, "0" });
                npcGridView.Rows.Add(new object[] { false, "Kiribati", "Hollowballs (20)", false, false, false, "0" });
                npcGridView.Rows.Add(new object[] { false, "Kiliwallis", "Hollowballs (20)", false, false, false, "0" });
                npcGridView.Rows.Add(new object[] { false, "Kokelau", "Hollowballs (20)", false, false, false, "0" });
                npcGridView.Rows.Add(new object[] { false, "Trinidad", "Hollowballs (20)", false, false, false, "0" });
                npcGridView.Rows.Add(new object[] { false, "Santa Lucia", "Hollowballs (20)", false, false, false, "0" });
                npcGridView.Rows.Add(new object[] { false, "Esmeralda", "Hollowballs (20)", false, false, false, "0" });
                npcGridView.Rows.Add(new object[] { false, "Almirante", "Hollowballs (20)", false, false, false, "0" });
                npcGridView.Rows.Add(new object[] { false, "Almiranto", "Hollowballs (20)", false, false, false, "0" });
                npcGridView.Rows.Add(new object[] { false, "Marant", "Hollowballs (20)", false, false, false, "0" });
                npcGridView.Rows.Add(new object[] { false, "Picasso", "Hollowballs (20)", false, false, false, "0" });
                npcGridView.Rows.Add(new object[] { false, "Syanite", "Hollowballs (20)", false, false, false, "0" });
                npcGridView.Rows.Add(new object[] { false, "Boreas", "Hollowballs (20)", false, false, false, "0" });
                npcGridView.Rows.Add(new object[] { false, "Magmor", "Hollowballs (20)", false, false, false, "0" });
                npcGridView.Rows.Add(new object[] { false, "Marduk", "Hollowballs (20)", false, false, false, "0" });
                npcGridView.Rows.Add(new object[] { false, "Notos", "Hollowballs (20)", false, false, false, "0" });
                npcGridView.Rows.Add(new object[] { false, "Quintor", "Hollowballs (20)", false, false, false, "0" });
                npcGridView.Rows.Add(new object[] { false, "Vuur", "Hollowballs (20)", false, false, false, "0" });
                npcGridView.Rows.Add(new object[] { false, "Swashbuckler", "Hollowballs (20)", false, false, false, "0" });
                npcGridView.Rows.Add(new object[] { false, "Drake's Marauder", "Hollowballs (20)", false, false, false, "0" });
            }
            if (monsters)
            {
                monsterGridView.Rows.Clear();
                monsterGridView.Rows.Add(new object[] { false, "Moby Dick", "Bronze Harpoons (50)", "0" });
                monsterGridView.Rows.Add(new object[] { false, "Trankus", "Bronze Harpoons (50)", "0" });
                monsterGridView.Rows.Add(new object[] { false, "Serena", "Bronze Harpoons (50)", "0" });
                monsterGridView.Rows.Add(new object[] { false, "Orca", "Bronze Harpoons (50)", "0" });
                monsterGridView.Rows.Add(new object[] { false, "Azzlan", "Bronze Harpoons (50)", "0" });
                monsterGridView.Rows.Add(new object[] { false, "Azurenos", "Bronze Harpoons (50)", "0" });
                monsterGridView.Rows.Add(new object[] { false, "Gigantur", "Bronze Harpoons (50)", "0" });
                monsterGridView.Rows.Add(new object[] { false, "Octalus", "Bronze Harpoons (50)", "0" });
                monsterGridView.Rows.Add(new object[] { false, "Glasciadon", "Bronze Harpoons (50)", "0" });
                monsterGridView.Rows.Add(new object[] { false, "Raskvarik", "Bronze Harpoons (50)", "0" });
                monsterGridView.Rows.Add(new object[] { false, "Valocto", "Bronze Harpoons (50)", "0" });
                monsterGridView.Rows.Add(new object[] { false, "Zirkzeaa", "Bronze Harpoons (50)", "0" });
                monsterGridView.Rows.Add(new object[] { false, "Aligant", "Bronze Harpoons (50)", "0" });
                monsterGridView.Rows.Add(new object[] { false, "Elmato", "Bronze Harpoons (50)", "0" });
                monsterGridView.Rows.Add(new object[] { false, "Limfirii", "Bronze Harpoons (50)", "0" });
                monsterGridView.Rows.Add(new object[] { false, "Skiliadon", "Bronze Harpoons (50)", "0" });
                monsterGridView.Rows.Add(new object[] { false, "Ghirdora", "Bronze Harpoons (50)", "0" });
                monsterGridView.Rows.Add(new object[] { false, "Bicuvila", "Bronze Harpoons (50)", "0" });
                monsterGridView.Rows.Add(new object[] { false, "Brapiapoda", "Bronze Harpoons (50)", "0" });
                monsterGridView.Rows.Add(new object[] { false, "Vilundara", "Bronze Harpoons (50)", "0" });
                monsterGridView.Rows.Add(new object[] { false, "Taipan", "Bronze Harpoons (50)", "0" });
                monsterGridView.Rows.Add(new object[] { false, "Big Taipan", "Bronze Harpoons (50)", "0" });
            }
        }

        public void LoadDesignList(Dictionary<string, int> Designs)
        {
            try
            {
                if (InvokeRequired)
                {
                    Invoke(new MethodInvoker(() => LoadDesignList(Designs)));
                    return;
                }
                repDesignComboBox.Items.Clear();
                botDesignComboBox.Items.Clear();
                Designs = help.SortDictionary(Designs);
                foreach (var item in Designs)
                {
                    if (!repDesignComboBox.Items.Contains(item.Key))
                    {
                        repDesignComboBox.Items.Add(item.Key);
                    }
                    if (!botDesignComboBox.Items.Contains(item.Key))
                    {
                        botDesignComboBox.Items.Add(item.Key);
                    }
                }
            }
            catch (Exception ex)
            {
                BotMethods.WriteLine("Error while loading Designs!\n\n" + ex.Message);
            }
        }

        public void LoadItemList(Dictionary<string, int> Items)
        {
            try
            {
                if (InvokeRequired)
                {
                    Invoke(new MethodInvoker(() => LoadItemList(Items)));
                    return;
                }
                useItemComboBox.Items.Clear();
                Items = help.SortDictionary(Items);
                foreach (var item in Items)
                {
                    if (!useItemComboBox.Items.Contains(item.Key))
                    {
                        useItemComboBox.Items.Add(item.Key);
                    }
                }
            }
            catch (Exception ex)
            {
                BotMethods.WriteLine("Error while loading Items!\n\n" + ex.Message);
            }
        }

        public void LoadQuestList(Dictionary<string, int> Quests)
        {
            try
            {
                if (InvokeRequired)
                {
                    Invoke(new MethodInvoker(() => LoadQuestList(Quests)));
                    return;
                }
                Quests = help.SortDictionary(Quests);
                foreach (var quest in Quests)
                {
                    if (!selectedQuestsListbox.Items.Contains(quest.Key))
                    {
                        selectedQuestsListbox.Items.Add(quest.Key);
                    }
                }
            } catch (Exception ex)
            {
                BotMethods.WriteLine("Error while loading Quests!\n\n" + ex.Message);
            }
        }

        private void UpdateStats()
        {
            try
            {
                userlicensevalidlabel.Text = Bot.auth.timeleft;
                userlatestversionlabel.Text = Bot.auth.versionStatus == "1" ? "True" : "False";
                if (Server.Connected && BotSession.loggedIn)
                {
                    sessiongoldlabel.Text = BotSession.Sessiongold.ToString();
                    sessionpearlslabel.Text = BotSession.Sessionpearls.ToString();
                    sessionmojoslabel.Text = BotSession.Sessionmojos.ToString();
                    sessioncrownslabel.Text = BotSession.Sessioncrowns.ToString();
                    sessionxplabel.Text = BotSession.Sessionxp.ToString();
                    sessionhplabel.Text = BotSession.Sessionhp.ToString();
                    sessionglitterlabel.Text = BotSession.Sessionglitters.ToString();
                    sessionchestslabel.Text = BotSession.Sessionchests.ToString();
                    sessionnpcslabel.Text = BotSession.Sessionnpcs.ToString();
                    sessionmonsterlabel.Text = BotSession.Sessionmonsters.ToString();
                    sessiondeathslabel.Text = BotSession.Sessiondeaths.ToString();
                    sessionBMNpcslabel.Text = BotSession.Sessionbmnpcs.ToString();
                    sessionBMsLabel.Text = BotSession.Sessionbms.ToString();
                    sessionBMWavesLabel.Text = BotSession.Sessionwaves.ToString();
                    sessionELPLabel.Text = BotSession.Sessionelps.ToString();
                    sessionPowderLabel.Text = BotSession.Sessionpowder.ToString();
                    sessionPlatesLabel.Text = BotSession.Sessionplates.ToString();
                    sessioncrystalslabel.Text = BotSession.Sessioncrystals.ToString();
                    sessionreconnectslabel.Text = BotSession.Sessionreconnects.ToString();
                    sessionattackedbyplayerslabel.Text = BotSession.Sessionattackedbyplayer.ToString();
                    sessionescapedislandslabel.Text = BotSession.Sessionescapedisland.ToString();
                    sessioncursedsoulslabel.Text = BotSession.Sessioncursedsouls.ToString();
                    sessionradiantsoulslabel.Text = BotSession.Sessionradiantsouls.ToString();
                    if (Bot.Running)
                    {
                        int num = Convert.ToInt32(new DateTime(DateTime.Now.Subtract(BotSession.sessionStartTime).Ticks).ToString("d "));
                        num--;
                        this.sessionruntimelabel.Text = num + " days " + new DateTime(DateTime.Now.Subtract(BotSession.sessionStartTime).Ticks).ToString("HH:mm:ss");
                        this.sessionstarttimelabel.Text = BotSession.sessionStartTime.ToShortDateString() + " " + BotSession.sessionStartTime.ToLongTimeString();
                    }
                    userNameLabel.Text = Account.Username;
                    userGuildLabel.Text = Account.Guild.Length > 0 ? Account.Guild : "-";
                    userUIDLabel.Text = Account.ProjectID + "/" + Account.UserID.ToString();
                    userSessionLabel.Text = BotSession.Sessionid;
                    userHPBar.Maximum = Account.MaxHP;
                    if (Account.HP > Account.MaxHP || Account.HP < 0)
                    {
                        userHPBar.Value = Account.MaxHP;
                    }
                    else
                    {
                        userHPBar.Value = Account.HP;
                    }
                    userHPLabel.Text = $"{Account.HP}/{Account.MaxHP}";
                    userVPBar.Maximum = Account.MaxVP;
                    if (Account.VP > Account.MaxVP || Account.VP < 0)
                    {
                        userVPBar.Value = Account.MaxVP;
                    }
                    else
                    {
                        userVPBar.Value = Account.VP;
                    }
                    userVPLabel.Text = $"{Account.VP}/{Account.MaxVP}";
                    userXPBar.Maximum = Convert.ToInt32(Account.MaxXP);
                    if (Account.XP > Account.MaxXP || Account.XP < 0)
                    {
                        userXPBar.Value = Convert.ToInt32(Account.MaxXP);
                    }
                    else
                    {
                        userXPBar.Value = Convert.ToInt32(Account.XP);
                    }
                    userXPLabel.Text = $"{Account.XP}/{Account.MaxXP}";
                    userBPBar.Maximum = Account.MaxBP;
                    if (Account.BP > Account.MaxBP || Account.BP < 0)
                    {
                        userBPBar.Value = Account.MaxBP;
                    }
                    else
                    {
                        userBPBar.Value = Account.BP;
                    }
                    userBPLabel.Text = $"{Account.BP}/{Account.MaxBP}";
                    userGoldLabel.Text = Account.Gold.ToString();
                    userPearlsLabel.Text = Account.Pearls.ToString();
                    userCrystalsLabel.Text = Account.Crystals.ToString();
                    userMojosLabel.Text = Account.Mojos.ToString();
                    userCursedSoulsLabel.Text = Account.CursedSouls.ToString();
                    userRadiantSoulsLabel.Text = Account.RadianSouls.ToString();
                    userMapLabel.Text = BotHandlers.MapHandler(Account.MapID) == "Unknown Map" ? Account.Map : BotHandlers.MapHandler(Account.MapID);
                    userPositionLabel.Text = $"X: {Account.Position.X}|Y: {Account.Position.Y}";
                    userRepairmateLabel.Text = Account.IsRepMaatLevel5 ? "Level 5" : "Normal";
                    if (Account.Items.ContainsKey(Account.RaidMedallion))
                    {
                        raidMedallionsLabel.Text = (Account.Items[Account.RaidMedallion] as ActionItemStub).amount.ToString();
                    }
                    sessionaveragedamagelabel.Text = BotSession.Canondamage.ToString();
                    if (BotSession.Sessionharpoondamage != -1)
                    {
                        sessionharpoondamagelabel.Text = BotSession.Sessionharpoondamage.ToString();
                    }
                    sessionspeedlabel.Text = (Account.Speed * 100).ToString();
                    sessionpremiumlabel.Text = (Account.Premium == true ? "Yes." : "Expired.");
                    sessiondesignlabel.Text = Bot.Designs.FirstOrDefault(design => design.Value == Account.DesignId).Key;
                    sessionviewdistancelabel.Text = (Account.ViewDistance > 0 ? Account.ViewDistance : Account.ViewDistance = 55).ToString();
                    sessioncanonrangelabel.Text = (Account.CanonRange > 0 ? Account.CanonRange : Account.CanonRange = 30).ToString();
                    sessionreloadtimelabel.Text = (Account.ReloadTime / 1000).ToString() + "Seconds.";
                    sessiontreasurehunterlabel.Text = Account.TreasureHunter ? "Yes." : "No.";
                    sessionrepairinglabel.Text = Account.Repairing ? "Yes." : "No.";
                    sessionboardincommanderlabel.Text = Account.BoardHPLimit == 50 ? "Level 2" : Account.BoardHPLimit > 0 ? "Level 1" : "None";
                    sessiondestroyedplayerslabel.Text = BotSession.Sessiondestroyedplayer.ToString();
                    sessionescapedplayerslabel.Text = BotSession.Sessionescapedplayer.ToString();
                    sessionshotbacklabel.Text = BotSession.Sessionshotback.ToString();
                    if (Account.OnBM)
                    {
                        Text = $"BoxyBot - {BotHandlers.MapHandler(Account.MapID)} Wave: " + Account.BonusMaps[Account.MapID].currentWave + "/" + BonusMapConstants.GetBonusmapMaxWaves()[Account.MapID];
                    }
                    else if (Account.OnRaid)
                    {
                        Text = "BoxyBot - Raid NPC Left: " + Account.NPCLeft;
                    }
                    else if (this.updateFormText)
                    {
                        this.updateFormText = false;
                        Text = "BoxyBot - " + (Account.Guild.Length >= 1 ? $"[{Account.Guild}]" : "") + Account.Username + (Account.SpawnQueue != "null" ? $" - Queue: {Account.SpawnQueue}" : "");
                    }
                } else
                {
                    if (BotSession.logginIn)
                    {
                        var ts = DateTime.Now.Subtract(this.loginTime);
                        if (ts.TotalSeconds >= 300.0)
                        {
                            BotSession.lostConnection = true;
                        }
                    }
                }
                if (BotSession.lostConnection)
                {
                    BotSession.lostConnection = false;
                    BotSession.Sessionacceptedloginbonus = false;
                    BotMethods.WriteLine("Restarting Bot...");
                    this.LoginMethod();
                    this.ResetUserStatistics();
                    if (Account.gClass != null)
                    {
                        Account.gClass = new UserInitMessage();
                    }
                    Thread.Sleep(5000);
                    BotLogic.StartBotThread();
                }
            }
            catch (Exception)
            {
            }
        }

        public void CreateSettings()
        {
            try
            {
                BotSession.Username = userNameTextbox.Text;
                BotSession.Password = userPassTextbox.Text;
                var text = useItemActionCombobox.Text;
                if (text.Contains("< 100"))
                {
                    BotSettings.useActionItemType = 1;
                }
                else
                if (text.Contains("< 50"))
                {
                    BotSettings.useActionItemType = 2;
                }
                if (useItemComboBox.Text.Length > 1 && Bot.Items != null && Bot.Items.ContainsKey(useItemComboBox.Text))
                {
                    BotSettings.useActionItemID = Bot.Items[useItemComboBox.Text];
                }
                BotSettings.collectchests = collectChestsCheckbox.Checked;
                BotSettings.collecteventchests = collectEventChestsCheckbox.Checked;
                BotSettings.repathp = (int)repathpbar.Value;
                BotSettings.repatcorner = reponcornercheckbox.Checked;
                BotSettings.autoJoinRaid = autoJoinCheckBox.Checked;
                BotSettings.collectGlitters = CollectGlittersCheckbox.Checked;
                BotSettings.shootMonsters = shootMonstersCheckbox.Checked;
                BotSettings.shootNPCs = shootNPCsCheckbox.Checked;
                BotSettings.onlyFullHPRaidNpc = onlyFullHPRaidsCheckBox.Checked;
                BotSettings.onlyFullHPNpc = onlyFullHPNPCsCheckBox.Checked;
                BotSettings.onlyFullHPMonsters = onlyFullHPMonstersCheckBox.Checked;
                BotSettings.collectMeat = collectKrakenMeatCheckbox.Checked;
                BotSettings.usePowderRaid = usePowderRaidCheckbox.Checked;
                BotSettings.useArmorRaid = usePlatesRaidCheckbox.Checked;
                BotSettings.usePowderRaidBoss = usePowderRaidBossCheckbox.Checked;
                BotSettings.useArmorRaidBoss = usePlatesRaidBossCheckbox.Checked;
                BotSettings.usePowderBM = usePowderBMCheckbox.Checked;
                BotSettings.useArmorBM = usePlatesBMCheckbox.Checked;
                BotSettings.rangeBMNpcs = bmRangeModuscheckbox.Checked;
                BotSettings.usePowderPlayer = shootbackPowderCheckBox.Checked;
                BotSettings.useArmorPlayer = shootbackPlatesCheckBox.Checked;
                BotSettings.maxRaidRejoins = (int)maxRaidRejoinsNumeric.Value;
                BotSettings.useFleeSpeedStone = useFleeSpeedCheckbox.Checked;
                BotSettings.useFleeSmokebomb = useFleeSmokeCheckbox.Checked;
                BotSettings.useRaidBossCandle = useRaidBossCandleCheckbox.Checked;
                BotSettings.useRaidBossSnowman = useRaidBossSnowmanCheckbox.Checked;
                BotSettings.useElmosfire = useElmosFireCheckbox.Checked;
                BotSettings.useSkyfire = useSkyfireCheckbox.Checked;
                BotSettings.useHumanMovement = humanMovementCheckbox.Checked;
                BotSettings.collectEventGlitter = collectCargoCheckbox.Checked;
                BotSettings.joinSameBM = joinSameBMCheckbox.Checked;
                BotSettings.repatborder = reponbordercheckbox.Checked;
                BotSettings.jumpMapRandom = jumpMapRandomRadioButton.Checked;
                if (BotSettings.jumpMapRandom)
                {
                    BotSettings.jumpMapLeftRight = false;
                }
                BotSettings.jumpMapLeftRight = jumpMapCircleRadioButton.Checked;
                if (BotSettings.jumpMapLeftRight)
                {
                    BotSettings.jumpMapRandom = false;
                }
                BotSettings.repathpraid = (int)repatraidnumericupdown.Value;
                BotSettings.repathpbm = (int)repairatbmnumericupdown.Value;
                BotSettings.rangeBMNpcs = bmRangeModuscheckbox.Checked;
                BotSettings.jumpMapIfAvailable = jumpMapIfAvailableCheckbox.Checked;
                BotSettings.repAtIsland = repAtIslandCheckbox.Checked;
                BotSettings.ignoreBoxesPlayerNearby = ignoreBoxesPlayerNearbyCheckbox.Checked;
                BotSettings.finishNPCHpLimit = (int)finishNPCHPLimitUpDown.Value;
                BotSettings.rebuyRaidMedallions = rebuyRaidMedallionsCheckbox.Checked;
                BotSettings.boardVCMA = boardVCMACheckBox.Checked;
                if (movementCombobox.Text.Contains("Fixed-"))
                {
                    BotSettings.movementType = 0;
                }
                if (movementCombobox.Text.Contains("Border"))
                {
                    BotSettings.movementType = 1;
                }
                if (movementCombobox.Text.Contains("Corner"))
                {
                    BotSettings.movementType = 2;
                }
                if (movementCombobox.Text.Contains("Side"))
                {
                    BotSettings.movementType = 3;
                }
                BotSettings.prioNPCs = prioNPCsCheckbox.Checked;
                BotSettings.betterGFX = useBetterGFX.Checked;
                BotSettings.avoidIslands = avoidIslandsCheckbox.Checked;
                BotSettings.switchMapTime = help.ToInt(jumpMapTimerBox.Text);
                Bot.nextMapJump = DateTime.Now.AddMinutes(BotSettings.switchMapTime);
                if (Items.GetItems().ContainsKey(shootbackHPItemCombobox.Text))
                {
                    BotSettings.shootBackHPItemID = Items.GetItems()[shootbackHPItemCombobox.Text];
                }
                if (Items.GetItems().ContainsKey(shootbackVPItemCombobox.Text))
                {
                    BotSettings.shootBackVPItemID = Items.GetItems()[shootbackVPItemCombobox.Text];
                }
                if (Bot.Designs != null && Bot.Designs.ContainsKey(botDesignComboBox.Text))
                {
                    BotSettings.botDesign = Bot.Designs[botDesignComboBox.Text];
                }
                if (Bot.Designs != null && Bot.Designs.ContainsKey(repDesignComboBox.Text))
                {
                    BotSettings.repDesign = Bot.Designs[repDesignComboBox.Text];
                }
                BotSettings.tasksDoneLogout &= !tasksDoneContinueCheckbox.Checked;
                BotSettings.tasksDoneLogout |= tasksDoneLogoutCheckbox.Checked;
                BotSettings.rebuyAmmoSmaller = (int)rebuyAmmoSmallerNumeric.Value;
                BotSettings.rebuyHarpoonsSmaller = (int)rebuyHarpoonsSmallerNumeric.Value;
                BotSettings.prioMonsters = prioMonstersCheckbox.Checked;
                BotSettings.joinBeheLvl26 = joinBeheLvl26Checkbox.Checked;
                BotSettings.acceptLoginBonus = acceptLoginBonusCheckbox.Checked;
                BotSettings.collectWhileAttack = collectWhileAttackCheckbox.Checked;
                BotSettings.avoidBeheNPCs = avoidBeheNpcsCheckbox.Checked;
                BotSettings.rebuyKeys = rebuyChestkeysCheckbox.Checked;
                BotSettings.fleeIfEnemyNearby = fleeIfEnemyNearbyCheckbox.Checked;
                BotSettings.AmmoIDBMChanged = BotHandlers.AmmoHandler(ammoBMWaveBox.Text);
                BotSettings.useBMAmmoChanger = ammoBmChangeCheckbox.Checked;
                BotSettings.changeAmmoBMWave = (int)ammoBMWaveNumeric.Value;
                BotSettings.useQuestSystem = useQuestSystemCheckbox.Checked;
                BotSettings.AmmoIDBM = BotHandlers.AmmoHandler(ammoBMBox.Text);
                BotSettings.AmmoIDRaid = BotHandlers.AmmoHandler(raidNPCAmmoBox.Text);
                BotSettings.AmmoIDRaidBoss = BotHandlers.AmmoHandler(raidBossAmmoBox.Text);
                BotSettings.useQuestSystem = useQuestSystemCheckbox.Checked;
                BotSettings.doDailyQuest = doDailyQuestCheckbox.Checked;
                BotSettings.useRaidBossSpeedstone = useRaidBossSpeedstoneCheckbox.Checked;
                BotSettings.useRaidBossBloodlust = useRaidBossBloodlustCheckbox.Checked;
            }
            catch (Exception ex)
            {
                BotMethods.WriteLine("Save Settings Error 1.\n           " + ex.Message);
            }
            try
            {
                Bot.entitys = new Dictionary<string, bool>();
                Bot.quests = new Dictionary<string, bool>();
                for (int i = 0; i < npcGridView.Rows.Count; i++)
                {
                    foreach (var cell in npcGridView.Rows[i].Cells)
                    {
                        if (cell is DataGridViewCheckBoxCell)
                        {
                            if ((cell as DataGridViewCheckBoxCell).Value == null)
                            {
                                (cell as DataGridViewCheckBoxCell).Value = "0";
                            }
                            if (((cell as DataGridViewCheckBoxCell).Value.ToString() != "0" && (cell as DataGridViewCheckBoxCell).Value.ToString() != "1"))
                            {
                                (cell as DataGridViewCheckBoxCell).Value = "0";
                            }
                        }
                    }
                    if (!Bot.entitys.ContainsKey(npcGridView.Rows[i].Cells[1].Value.ToString() + "|" + npcGridView.Rows[i].Cells[2].Value.ToString() + "|" + npcGridView.Rows[i].Cells[3].Value.ToString() + "|" + npcGridView.Rows[i].Cells[4].Value.ToString() + "|" + npcGridView.Rows[i].Cells[5].Value.ToString()))
                    {
                        Bot.entitys.Add(npcGridView.Rows[i].Cells[1].Value.ToString() + "|" + npcGridView.Rows[i].Cells[2].Value.ToString() + "|" + npcGridView.Rows[i].Cells[3].Value.ToString() + "|" + npcGridView.Rows[i].Cells[4].Value.ToString() + "|" + npcGridView.Rows[i].Cells[5].Value.ToString(), (bool)(npcGridView.Rows[i].Cells[0].Value.ToString() == "1"));
                    }
                    if (npcGridView.Rows[i].Cells[6].Value.ToString().Length <= 0)
                    {
                        npcGridView.Rows[i].Cells[6].Value = "0";
                    }
                    if (!Bot.entitysAmount.ContainsKey(npcGridView.Rows[i].Cells[1].Value.ToString()) && int.Parse(npcGridView.Rows[i].Cells[6].Value.ToString()) > 0)
                    {
                        Bot.entitysAmount.Add(npcGridView.Rows[i].Cells[1].Value.ToString(), int.Parse(npcGridView.Rows[i].Cells[6].Value.ToString()));
                    }
                    else if (Bot.entitysAmount.ContainsKey(npcGridView.Rows[i].Cells[1].Value.ToString()))
                    {
                        Bot.entitysAmount[npcGridView.Rows[i].Cells[1].Value.ToString()] = int.Parse(npcGridView.Rows[i].Cells[6].Value.ToString());
                    }
                }
                for (int i = 0; i < monsterGridView.Rows.Count; i++)
                {
                    foreach (var cell in monsterGridView.Rows[i].Cells)
                    {
                        if (cell is DataGridViewCheckBoxCell)
                        {
                            if ((cell as DataGridViewCheckBoxCell).Value == null)
                            {
                                (cell as DataGridViewCheckBoxCell).Value = "0";
                            }
                            if (((cell as DataGridViewCheckBoxCell).Value.ToString() != "0" && (cell as DataGridViewCheckBoxCell).Value.ToString() != "1"))
                            {
                                (cell as DataGridViewCheckBoxCell).Value = "0";
                            }
                        }
                    }
                    if (!Bot.entitys.ContainsKey(monsterGridView.Rows[i].Cells[1].Value.ToString() + "|" + monsterGridView.Rows[i].Cells[2].Value.ToString()))
                    {
                        Bot.entitys.Add(monsterGridView.Rows[i].Cells[1].Value.ToString() + "|" + monsterGridView.Rows[i].Cells[2].Value.ToString(), (bool)(monsterGridView.Rows[i].Cells[0].Value.ToString() == "1"));
                    }
                    if (monsterGridView.Rows[i].Cells[3].Value.ToString().Length <= 0)
                    {
                        monsterGridView.Rows[i].Cells[3].Value = "0";
                    }
                    if (!Bot.entitysAmount.ContainsKey(monsterGridView.Rows[i].Cells[1].Value.ToString()) && int.Parse(monsterGridView.Rows[i].Cells[3].Value.ToString()) > 0)
                    {
                        Bot.entitysAmount.Add(monsterGridView.Rows[i].Cells[1].Value.ToString(), int.Parse(monsterGridView.Rows[i].Cells[3].Value.ToString()));
                    } else if (Bot.entitysAmount.ContainsKey(monsterGridView.Rows[i].Cells[1].Value.ToString()))
                    {
                        Bot.entitysAmount[monsterGridView.Rows[i].Cells[1].Value.ToString()] = int.Parse(monsterGridView.Rows[i].Cells[3].Value.ToString());
                    }
                }
                for (int i = 0; i < selectedQuestsListbox.Items.Count; i++)
                {
                    Bot.quests.Add(selectedQuestsListbox.GetItemText(selectedQuestsListbox.Items[i]), selectedQuestsListbox.GetItemChecked(i));
                }
            }
            catch (Exception ex)
            {
                BotMethods.WriteLine("Save Settings Error 2.\n           " + ex.Message);
            }
            try
            {
                string[] array = attackSightTextbox.Text.Split(';');
                var _out = 0;
                if (array.Length > 0)
                {
                    foreach (var enemy in array)
                    {
                        if (enemy.Length > 0)
                        {
                            if (int.TryParse(enemy, out _out))
                            {
                                var _enemy = enemy.Replace(" ", "");
                                var uid = help.ToDouble(_enemy.Split('/')[1]);
                                var projectID = help.ToInt(_enemy.Split('/')[0]);
                                if (!BotSettings.attackOnSightShips.ContainsKey(uid))
                                {
                                    BotSettings.attackOnSightShips.Add(uid, projectID);
                                }
                            }
                            else
                            {
                                var _guild = enemy.Replace("\"", "");
                                var index = BotSettings.attackOnSightGuilds.Count;
                                if (!BotSettings.attackOnSightGuilds.ContainsKey(_guild))
                                {
                                    BotSettings.attackOnSightGuilds.Add(_guild, index++);
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                BotMethods.WriteLine("Save Settings Error 3.\n           " + ex.Message);
            }
        }

        public string GetLines()
        {
            var text = "";
            try
            {
                if (InvokeRequired)
                {
                    Invoke(new MethodInvoker(() =>
                    {
                        text = LogBox.Text;
                    }));
                }
                else
                {
                    text = LogBox.Text;
                }
            } catch (Exception ex)
            {
                text = "Log-Exception: " + ex;
            }
            return text;
        }

        public void WriteLine(string message)
        {
            int num = 0;
            int num2 = 0;
            string[] lines = this.LogBox.Lines;
            string[] array = lines;
            for (int i = 0; i < array.Length; i++)
            {
                string text = array[i];
                if (num >= 2)
                {
                    lines[num - 1] = text;
                }
                num++;
            }
            if (lines.Length > 1)
            {
                num2 = lines.Length - 2;
            }
            message = "[" + DateTime.Now.ToString("T") + "] - " + message;
            lines[num2] = message;
            this.LogBox.Lines = lines;
            this.LogBox.SelectionStart = this.LogBox.Text.Length;
            this.LogBox.ScrollToCaret();
        }

        private void LoadSettings()
        {
            try
            {
                if (Settings.Default != null)
                {
                    if (Settings.Default.firstStart)
                    {
                        this.LoadDefaultGridItems(true, true);
                        Settings.Default.firstStart = false;
                    }
                    this.userNameTextbox.Text = Settings.Default.Username;
                    this.userPassTextbox.Text = Settings.Default.Password;
                    this.clearCacheCheckbox.Checked = Settings.Default.ClearCache;
                    this.Width = Settings.Default.Width;
                    this.Height = Settings.Default.Height;
                    this.collectChestsCheckbox.Checked = Settings.Default.collectchests;
                    this.collectEventChestsCheckbox.Checked = Settings.Default.collecteventchests;
                    this.reponcornercheckbox.Checked = Settings.Default.repatcorner;
                    this.reponbordercheckbox.Checked = Settings.Default.repatborder;
                    this.repathpbar.Value = Settings.Default.repathp;
                    this.raidNPCAmmoBox.Text = Settings.Default.ammoRaid;
                    this.autoJoinCheckBox.Checked = Settings.Default.autoJoinRaid;
                    this.CollectGlittersCheckbox.Checked = Settings.Default.CollectGlitters;
                    this.shootMonstersCheckbox.Checked = Settings.Default.ShootMonsters;
                    this.shootNPCsCheckbox.Checked = Settings.Default.shootNPCs;
                    this.shootRaidBossCheckBox.Checked = Settings.Default.shootRaidBoss;
                    this.raidBossAmmoBox.Text = Settings.Default.ammoRaidBoss;
                    this.ammoBMBox.Text = Settings.Default.ammoBM;
                    this.autoJoinBMCheckBox.Checked = Settings.Default.autoJoinBM;
                    this.onlyFullHPMonstersCheckBox.Checked = Settings.Default.onlyFullHPMonsters;
                    this.onlyFullHPNPCsCheckBox.Checked = Settings.Default.onlyFullHPNpcs;
                    this.onlyFullHPRaidsCheckBox.Checked = Settings.Default.onlyFullHPRaidNpcs;
                    this.rebuyAmmoCheckBox.Checked = Settings.Default.rebuyAmmo;
                    this.rebuyAmountBox.Text = Settings.Default.rebuyAmmoAmount;
                    this.rebuyAmmoComboBox.Text = Settings.Default.rebuyAmmoType;
                    this.shootbackAmmoComboBox.Text = Settings.Default.shootBackAmmo;
                    this.rebuyHarpoonsCheckBox.Checked = Settings.Default.rebuyHarpoons;
                    this.rebuyHarpoonsAmountComboBox.Text = Settings.Default.rebuyHarpoonsAmount;
                    this.rebuyHarpoonsComboBox.Text = Settings.Default.rebuyHarpoonsType;
                    this.collectKrakenMeatCheckbox.Checked = Settings.Default.collectMeat;
                    this.jumpMapsCheckbox.Checked = Settings.Default.jumpMaps;
                    this.usePowderBMCheckbox.Checked = Settings.Default.usePowderBM;
                    this.usePlatesBMCheckbox.Checked = Settings.Default.usePlatesBM;
                    this.shootbackPowderCheckBox.Checked = Settings.Default.usePowderPlayer;
                    this.shootbackPlatesCheckBox.Checked = Settings.Default.usePlatesPlayer;
                    this.usePowderRaidCheckbox.Checked = Settings.Default.usePowderRaid;
                    this.usePlatesRaidCheckbox.Checked = Settings.Default.usePlatesRaid;
                    this.usePowderRaidBossCheckbox.Checked = Settings.Default.usePowderRaidBoss;
                    this.usePlatesRaidBossCheckbox.Checked = Settings.Default.usePlatesRaidBoss;
                    this.repDesignComboBox.Text = Settings.Default.repDesign;
                    this.botDesignComboBox.Text = Settings.Default.botDesign;
                    this.useDesignChangerCheckbox.Checked = Settings.Default.useDesignChanger;
                    this.maxRaidRejoinsNumeric.Value = Settings.Default.maxRaidRejoins;
                    this.useFleeSmokeCheckbox.Checked = Settings.Default.useFleeSmoke;
                    this.useFleeSpeedCheckbox.Checked = Settings.Default.useFleeSpeed;
                    this.useRaidBossCandleCheckbox.Checked = Settings.Default.useRaidBossCandle;
                    this.useRaidBossSnowmanCheckbox.Checked = Settings.Default.useRaidBossSnowman;
                    this.useShootBackHPItemCheckBox.Checked = Settings.Default.useShootbackHPItem;
                    this.useShootBackVPItemCheckbox.Checked = Settings.Default.useShootbackVPItem;
                    this.shootbackHPItemCombobox.Text = Settings.Default.shootbackHPItem;
                    this.shootbackVPItemCombobox.Text = Settings.Default.shootbackVPItem;
                    this.useSkyfireCheckbox.Checked = Settings.Default.useSkyfire;
                    this.useElmosFireCheckbox.Checked = Settings.Default.useElmosfire;
                    this.humanMovementCheckbox.Checked = Settings.Default.useHumanMovement;
                    this.collectCargoCheckbox.Checked = Settings.Default.collectCargo;
                    this.finishNPCsCheckbox.Checked = Settings.Default.finishNPCs;
                    this.joinSameBMCheckbox.Checked = Settings.Default.joinSameBonusMap;
                    this.attackSightCheckbox.Checked = Settings.Default.attackOnSight;
                    this.jumpMapCircleRadioButton.Checked = Settings.Default.jumpMapCircle;
                    this.jumpMapRandomRadioButton.Checked = Settings.Default.jumpMapRandom;
                    this.jumpMapTimerBox.Text = Settings.Default.jumpMapTimer.ToString() + " Min.";
                    this.repairatbmnumericupdown.Value = Settings.Default.repAtHPBM;
                    this.repatraidnumericupdown.Value = Settings.Default.repAtHPRaid;
                    this.bmRangeModuscheckbox.Checked = Settings.Default.rangeBMNpcs;
                    this.jumpMapIfAvailableCheckbox.Checked = Settings.Default.jumpMapIfAvailable;
                    this.repAtIslandCheckbox.Checked = Settings.Default.repairAtIsland;
                    this.ignoreBoxesPlayerNearbyCheckbox.Checked = Settings.Default.ignoreBoxesPlayerNearby;
                    this.finishNPCHPLimitUpDown.Value = Settings.Default.finishNPCHpLimit;
                    this.rebuyRaidMedallionsCheckbox.Checked = Settings.Default.rebuyRaidMedallions;
                    this.useItemCheckbox.Checked = Settings.Default.useActionItemUser;
                    this.useItemComboBox.Text = Settings.Default.useActionItemID;
                    this.useItemActionCombobox.Text = Settings.Default.useActionItemType;
                    this.boardVCMACheckBox.Checked = Settings.Default.boardVCMA;
                    this.movementCombobox.Text = Settings.Default.movementType;
                    this.prioNPCsCheckbox.Checked = Settings.Default.prioNPCs;
                    this.useBetterGFX.Checked = Settings.Default.betterGFX;
                    this.avoidIslandsCheckbox.Checked = Settings.Default.avoidIslands;
                    this.tasksDoneLogoutCheckbox.Checked = Settings.Default.tasksDoneLogout;
                    this.tasksDoneContinueCheckbox.Checked = !Settings.Default.tasksDoneLogout;
                    this.rebuyAmmoSmallerNumeric.Value = Settings.Default.rebuyAmooSmaller;
                    this.rebuyHarpoonsSmallerNumeric.Value = Settings.Default.rebuyHarpoonsSmaller;
                    this.prioMonstersCheckbox.Checked = Settings.Default.prioMonsters;
                    this.joinBeheLvl26Checkbox.Checked = Settings.Default.joinBeheLvl26;
                    this.acceptLoginBonusCheckbox.Checked = Settings.Default.acceptLoginBonus;
                    this.collectWhileAttackCheckbox.Checked = Settings.Default.collectWhileAttack;
                    this.avoidBeheNpcsCheckbox.Checked = Settings.Default.avoidBeheNPCs;
                    this.rebuyChestkeysCheckbox.Checked = Settings.Default.rebuyKeys;
                    this.fleeIfEnemyNearbyCheckbox.Checked = Settings.Default.fleeIfEnemyNearby;
                    this.ammoBMWaveBox.Text = Settings.Default.bmAmmoChanged;
                    this.ammoBmChangeCheckbox.Checked = Settings.Default.useBMAmmoChanger;
                    this.ammoBMWaveNumeric.Value = Settings.Default.bmAmmoChangerWave;
                    this.useQuestSystemCheckbox.Checked = Settings.Default.useQuestSystem;
                    this.doDailyQuestCheckbox.Checked = Settings.Default.doDaily21Quest;
                    this.useRaidBossSpeedstoneCheckbox.Checked = Settings.Default.useRaidBossSpeedstone;
                    this.useRaidBossBloodlustCheckbox.Checked = Settings.Default.useRaidBossBloodlust;
                    switch (Settings.Default.shootBackType)
                    {
                        case 0:
                            this.IgnoreRadioButton.Checked = true;
                            break;
                        case 1:
                            this.fleeRadioButton.Checked = true;
                            break;
                        case 2:
                            this.shootbackRadioButton.Checked = true;
                            break;
                        case 3:
                            this.automaticOnAttackedModeCheckbox.Checked = true;
                            break;
                    }
                    if (Settings.Default.usernamesList != null && Settings.Default.passwordsList != null)
                    {
                        List<string> usernames = new List<string>();
                        List<string> passwords = new List<string>();
                        for (int i = 0; i < Settings.Default.usernamesList.Count; i++)
                        {
                            if (!usernames.Contains(Settings.Default.usernamesList[i]))
                            {
                                usernames.Add(Settings.Default.usernamesList[i]);
                            }
                            if (!userNameTextbox.Items.Contains(Settings.Default.usernamesList[i]))
                            {
                                userNameTextbox.Items.Add(Settings.Default.usernamesList[i]);
                            }
                        }
                        for (int i = 0; i < Settings.Default.passwordsList.Count; i++)
                        {
                            passwords.Add(Settings.Default.passwordsList[i]);
                        }
                        BotSession.userLoginData = help.ListsToDictionary(usernames, passwords);
                    }
                    if (Settings.Default.Monsters != null)
                    {
                        try
                        {
                            for (int i = 0; i < Settings.Default.Monsters.Count; i++)
                            {
                                string[] args = Settings.Default.Monsters[i].Split('|');
                                monsterGridView.Rows.Add(new object[] { false, args[1], args[2], args[3] });
                                foreach (DataGridViewRow row in monsterGridView.Rows)
                                {
                                    if ((string)row.Cells[1].Value == (args[1]))
                                    {
                                        if ((args[0]) == "1")
                                        {
                                            row.Cells[0].Value = (row.Cells[0] as DataGridViewCheckBoxCell).TrueValue;
                                        }
                                        row.Cells[2].Value = args[2];
                                        row.Cells[3].Value = args[3];
                                    }
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            if (MessageBox.Show("There was an Error while loading Monsters.\n" + ex.Message + "\nDo you want to load the Default Monster List?", "Load Settings Error.", MessageBoxButtons.YesNo, MessageBoxIcon.Error) == DialogResult.Yes)
                            {
                                LoadDefaultGridItems(false, true);
                            }
                        }
                    }
                    if (Settings.Default.NPCs != null)
                    {
                        try
                        {
                            for (int i = 0; i < Settings.Default.NPCs.Count; i++)
                            {
                                string[] args = Settings.Default.NPCs[i].Split('|');
                                npcGridView.Rows.Add(new object[] { false, args[1], args[2], false, false, false });
                                foreach (DataGridViewRow row in npcGridView.Rows)
                                {
                                    if ((string)row.Cells[1].Value == (args[1]))
                                    {
                                        if ((args[0]) == "1")
                                        {
                                            row.Cells[0].Value = (row.Cells[0] as DataGridViewCheckBoxCell).TrueValue;
                                        }
                                        if ((args[3]) == "1")
                                        {
                                            row.Cells[3].Value = (row.Cells[3] as DataGridViewCheckBoxCell).TrueValue;
                                        }
                                        if ((args[4]) == "1")
                                        {
                                            row.Cells[4].Value = (row.Cells[4] as DataGridViewCheckBoxCell).TrueValue;
                                        }
                                        if ((args[5]) == "1")
                                        {
                                            row.Cells[5].Value = (row.Cells[5] as DataGridViewCheckBoxCell).TrueValue;
                                        }
                                        row.Cells[6].Value = args[6];
                                        row.Cells[2].Value = args[2];
                                    }
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            if (MessageBox.Show("There was an Error while loading NPCs.\n" + ex.Message + "\nDo you want to load the Default NPC List?", "Load Settings Error.", MessageBoxButtons.YesNo, MessageBoxIcon.Error) == DialogResult.Yes)
                            {
                                LoadDefaultGridItems(true, false);
                            }
                        }
                    }
                    if (Settings.Default.attackOnSightList != null)
                    {
                        for (int i = 0; i < Settings.Default.attackOnSightList.Count; i++)
                        {
                            UpdateAttackOnSightTextBox(Settings.Default.attackOnSightList[i]);
                        }
                    }
                    if (Settings.Default.selectedQuests != null)
                    {
                        for (int i = 0; i < Settings.Default.selectedQuests.Count; i++)
                        {
                            selectedQuestsListbox.Items.Add(Settings.Default.selectedQuests[i], true);
                        }
                    }
                }
                else Settings.Default.Save();
            }
            catch (Exception ex)
            {
                BotMethods.WriteLine("Error in Load Settings!\n\n" + ex);
            }
        }

        private void SaveSettings()
        {
            try
            {
                Settings.Default.Username = this.userNameTextbox.Text;
                Settings.Default.Password = this.userPassTextbox.Text;
                Settings.Default.ClearCache = this.clearCacheCheckbox.Checked;
                Settings.Default.Width = this.Width;
                Settings.Default.Height = this.Height;
                Settings.Default.collectchests = this.collectChestsCheckbox.Checked;
                Settings.Default.collecteventchests = this.collectEventChestsCheckbox.Checked;
                Settings.Default.repatcorner = this.reponcornercheckbox.Checked;
                Settings.Default.repatborder = this.reponbordercheckbox.Checked;
                Settings.Default.repathp = (int)this.repathpbar.Value;
                Settings.Default.ammoRaid = this.raidNPCAmmoBox.Text;
                Settings.Default.autoJoinRaid = this.autoJoinCheckBox.Checked;
                Settings.Default.CollectGlitters = this.CollectGlittersCheckbox.Checked;
                Settings.Default.ShootMonsters = this.shootMonstersCheckbox.Checked;
                Settings.Default.shootNPCs = this.shootNPCsCheckbox.Checked;
                Settings.Default.shootRaidBoss = this.shootRaidBossCheckBox.Checked;
                Settings.Default.ammoRaidBoss = this.raidBossAmmoBox.Text;
                Settings.Default.ammoBM = this.ammoBMBox.Text;
                Settings.Default.autoJoinBM = this.autoJoinBMCheckBox.Checked;
                Settings.Default.onlyFullHPMonsters = this.onlyFullHPMonstersCheckBox.Checked;
                Settings.Default.onlyFullHPNpcs = this.onlyFullHPNPCsCheckBox.Checked;
                Settings.Default.onlyFullHPRaidNpcs = this.onlyFullHPRaidsCheckBox.Checked;
                Settings.Default.rebuyAmmo = this.rebuyAmmoCheckBox.Checked;
                Settings.Default.rebuyAmmoAmount = this.rebuyAmountBox.Text;
                Settings.Default.rebuyAmmoType = this.rebuyAmmoComboBox.Text;
                Settings.Default.shootBackAmmo = this.shootbackAmmoComboBox.Text;
                if (fleeRadioButton.Checked)
                    Settings.Default.shootBackType = 1;
                if (shootbackRadioButton.Checked)
                    Settings.Default.shootBackType = 2;
                if (IgnoreRadioButton.Checked)
                    Settings.Default.shootBackType = 0;
                if (automaticOnAttackedModeCheckbox.Checked)
                    Settings.Default.shootBackType = 3;
                Settings.Default.rebuyHarpoons = this.rebuyHarpoonsCheckBox.Checked;
                Settings.Default.rebuyHarpoonsAmount = this.rebuyHarpoonsAmountComboBox.Text;
                Settings.Default.rebuyHarpoonsType = this.rebuyHarpoonsComboBox.Text;
                Settings.Default.collectMeat = this.collectKrakenMeatCheckbox.Checked;
                Settings.Default.jumpMaps = this.jumpMapsCheckbox.Checked;
                Settings.Default.usePowderBM = this.usePowderBMCheckbox.Checked;
                Settings.Default.usePlatesBM = this.usePlatesBMCheckbox.Checked;
                Settings.Default.usePowderPlayer = this.shootbackPowderCheckBox.Checked;
                Settings.Default.usePlatesPlayer = this.shootbackPlatesCheckBox.Checked;
                Settings.Default.usePowderRaid = this.usePowderRaidCheckbox.Checked;
                Settings.Default.usePlatesRaid = this.usePlatesRaidCheckbox.Checked;
                Settings.Default.usePowderRaidBoss = this.usePowderRaidBossCheckbox.Checked;
                Settings.Default.usePlatesRaidBoss = this.usePlatesRaidBossCheckbox.Checked;
                Settings.Default.repDesign = this.repDesignComboBox.Text;
                Settings.Default.botDesign = this.botDesignComboBox.Text;
                Settings.Default.useDesignChanger = this.useDesignChangerCheckbox.Checked;
                Settings.Default.maxRaidRejoins = (int)this.maxRaidRejoinsNumeric.Value;
                Settings.Default.useFleeSmoke = this.useFleeSmokeCheckbox.Checked;
                Settings.Default.useFleeSpeed = this.useFleeSpeedCheckbox.Checked;
                Settings.Default.useRaidBossCandle = this.useRaidBossCandleCheckbox.Checked;
                Settings.Default.useRaidBossSnowman = this.useRaidBossSnowmanCheckbox.Checked;
                Settings.Default.useShootbackHPItem = this.useShootBackHPItemCheckBox.Checked;
                Settings.Default.useShootbackVPItem = this.useShootBackVPItemCheckbox.Checked;
                Settings.Default.shootbackHPItem = this.shootbackHPItemCombobox.Text;
                Settings.Default.shootbackVPItem = this.shootbackVPItemCombobox.Text;
                Settings.Default.useSkyfire = this.useSkyfireCheckbox.Checked;
                Settings.Default.useElmosfire = this.useElmosFireCheckbox.Checked;
                Settings.Default.useHumanMovement = this.humanMovementCheckbox.Checked;
                Settings.Default.collectCargo = this.collectCargoCheckbox.Checked;
                Settings.Default.finishNPCs = this.finishNPCsCheckbox.Checked;
                Settings.Default.joinSameBonusMap = this.joinSameBMCheckbox.Checked;
                Settings.Default.attackOnSight = this.attackSightCheckbox.Checked;
                Settings.Default.jumpMapCircle = this.jumpMapCircleRadioButton.Checked;
                Settings.Default.jumpMapRandom = this.jumpMapRandomRadioButton.Checked;
                Settings.Default.jumpMapTimer = help.ToInt(this.jumpMapTimerBox.Text);
                Settings.Default.repAtHPBM = (int)this.repairatbmnumericupdown.Value;
                Settings.Default.repAtHPRaid = (int)this.repatraidnumericupdown.Value;
                Settings.Default.rangeBMNpcs = this.bmRangeModuscheckbox.Checked;
                Settings.Default.jumpMapIfAvailable = this.jumpMapIfAvailableCheckbox.Checked;
                Settings.Default.repairAtIsland = this.repAtIslandCheckbox.Checked;
                Settings.Default.ignoreBoxesPlayerNearby = this.ignoreBoxesPlayerNearbyCheckbox.Checked;
                Settings.Default.finishNPCHpLimit = (int)this.finishNPCHPLimitUpDown.Value;
                Settings.Default.rebuyRaidMedallions = this.rebuyRaidMedallionsCheckbox.Checked;
                Settings.Default.useActionItemUser = this.useItemCheckbox.Checked;
                Settings.Default.useActionItemID = this.useItemComboBox.Text;
                Settings.Default.useActionItemType = this.useItemActionCombobox.Text;
                Settings.Default.boardVCMA = this.boardVCMACheckBox.Checked;
                Settings.Default.movementType = this.movementCombobox.Text;
                Settings.Default.prioNPCs = this.prioNPCsCheckbox.Checked;
                Settings.Default.betterGFX = this.useBetterGFX.Checked;
                Settings.Default.avoidIslands = this.avoidIslandsCheckbox.Checked;
                Settings.Default.tasksDoneLogout = this.tasksDoneLogoutCheckbox.Checked;
                Settings.Default.rebuyAmooSmaller = (int)this.rebuyAmmoSmallerNumeric.Value;
                Settings.Default.rebuyHarpoonsSmaller = (int)this.rebuyHarpoonsSmallerNumeric.Value;
                Settings.Default.prioMonsters = this.prioMonstersCheckbox.Checked;
                Settings.Default.joinBeheLvl26 = this.joinBeheLvl26Checkbox.Checked;
                Settings.Default.acceptLoginBonus = this.acceptLoginBonusCheckbox.Checked;
                Settings.Default.collectWhileAttack = this.collectWhileAttackCheckbox.Checked;
                Settings.Default.avoidBeheNPCs = this.avoidBeheNpcsCheckbox.Checked;
                Settings.Default.rebuyKeys = this.rebuyChestkeysCheckbox.Checked;
                Settings.Default.fleeIfEnemyNearby = this.fleeIfEnemyNearbyCheckbox.Checked;
                Settings.Default.bmAmmoChanged = this.ammoBMWaveBox.Text;
                Settings.Default.useBMAmmoChanger = this.ammoBmChangeCheckbox.Checked;
                Settings.Default.bmAmmoChangerWave = (int)this.ammoBMWaveNumeric.Value;
                Settings.Default.useQuestSystem = this.useQuestSystemCheckbox.Checked;
                Settings.Default.doDaily21Quest = this.doDailyQuestCheckbox.Checked;
                Settings.Default.useRaidBossSpeedstone = this.useRaidBossSpeedstoneCheckbox.Checked;
                Settings.Default.useRaidBossBloodlust = this.useRaidBossBloodlustCheckbox.Checked;
                Settings.Default.usernamesList = new System.Collections.Specialized.StringCollection();
                Settings.Default.passwordsList = new System.Collections.Specialized.StringCollection();
                var i = 0;
                var max = BotSession.userLoginData.Count;
                while (i < max)
                {
                    if (!Settings.Default.usernamesList.Contains(BotSession.userLoginData.ElementAt(i).Key))
                    {
                        Settings.Default.usernamesList.Add(BotSession.userLoginData.ElementAt(i).Key);
                        Settings.Default.passwordsList.Add(BotSession.userLoginData.ElementAt(i).Value);
                    }
                    i++;
                }
                Settings.Default.Monsters = new System.Collections.Specialized.StringCollection();
                foreach (DataGridViewRow row in monsterGridView.Rows)
                {
                    Settings.Default.Monsters.Add($"{row.Cells[0].Value}|{row.Cells[1].Value}|{row.Cells[2].Value}|{row.Cells[3].Value}");
                }
                Settings.Default.NPCs = new System.Collections.Specialized.StringCollection();
                foreach (DataGridViewRow row in npcGridView.Rows)
                {
                    Settings.Default.NPCs.Add($"{row.Cells[0].Value}|{row.Cells[1].Value}|{row.Cells[2].Value}|{row.Cells[3].Value}|{row.Cells[4].Value}|{row.Cells[5].Value}|{row.Cells[6].Value}");
                }
                Settings.Default.attackOnSightList = new System.Collections.Specialized.StringCollection();
                foreach (string enemy in attackSightTextbox.Text.Split(';'))
                {
                    Settings.Default.attackOnSightList.Add(enemy);
                }
                Settings.Default.selectedQuests = new System.Collections.Specialized.StringCollection();
                foreach (var quest in selectedQuestsListbox.CheckedItems)
                {
                    Settings.Default.selectedQuests.Add(quest.ToString());
                }
                Settings.Default.Save(); 
            }
            catch (Exception ex)
            {
                BotMethods.WriteLine("Error in Save Settings!\n\n" + ex.Message);
            }
            this.CreateSettings();
        }

        public void LoginMethod()
        {
            if (clearCacheCheckbox.Checked)
            {
                try
                {
                    BotMethods.ClearCache();
                    BotMethods.WriteLine("Cache cleaned.");
                }
                catch (Exception)
                {
                    BotMethods.WriteLine("There was an error while cleaning cache!");
                }
            }
            try
            {
                if (Account.gClass != null)
                {
                    Account.gClass.entityInfo = null;
                }
                var _compileTime = help.GetCompileTime();
                var _retrys = 0;
                while (_compileTime.Length < 16 && _retrys < 10)
                {
                    _compileTime = help.GetCompileTime();
                    _retrys++;
                    Thread.Sleep(1000);
                }
                if (_retrys >= 10)
                {
                    BotMethods.WriteLine("Failed to get a valid compileTime! Retrying in a few seconds.");
                    Thread.Sleep(60000);
                    LoginMethod();
                }
                if (_compileTime == Program.compileTime)
                {
                    this.loginTime = DateTime.Now;
                    Server.Connected = false;
                    BotSession.loggedIn = false;
                    BotSession.logginIn = true;
                    BotMethods.WriteLine("Loading Seafight page...");
                    this.webBrowser1.Navigate("http://www.seafight.com/");
                    return;
                }
                BotMethods.WriteLine("------------------------->SEAFIGHT UPDATE<-------------------------");
                BotMethods.WriteLine("Seafight has been updated, please wait for the next bot update!");
                webBrowser1.Navigate("http://www.elitepvpers.com/forum/seafight/4222822-release-boxybot.html");
                this.browserHiderPicBox.Visible = false;
            } catch (Exception ex)
            {
                BotMethods.WriteLine("Could not login!\n" + ex.Message);
            }
        }

        private void WebBrowser1_DocumentCompleted(object sender, WebBrowserDocumentCompletedEventArgs e)
        {
            if (webBrowser1.DocumentText.Contains("bgcdw_login_form_username"))
            {
                HtmlElement lname = webBrowser1.Document.GetElementById("bgcdw_login_form_username");
                HtmlElement lpass = webBrowser1.Document.GetElementById("bgcdw_login_form_password");
                if (!lname.GetAttribute("value").Equals(userNameTextbox.Text))
                {
                    lname.SetAttribute("value", userNameTextbox.Text);
                }
                if (!lpass.GetAttribute("value").Equals(userPassTextbox.Text))
                {
                    lpass.SetAttribute("value", userPassTextbox.Text);
                }
                try
                {
                    webBrowser1.Document.GetElementById("bgcdw_login_form_login").InvokeMember("click");
                } catch (Exception)
                {
                    foreach (HtmlElement element in webBrowser1.Document.All)
                    {
                        if (element.GetAttribute("className") == "bgcdw_button bgcdw_login_form_login")
                        {
                            element.InvokeMember("click");
                            break;
                        }
                    }
                }
                BotMethods.WriteLine("Logging in user: " + BotSession.Username);
                logInButton.Enabled = true;
            }
            if (webBrowser1.DocumentText.Contains("href=\"index.es?action=internalMap\""))
            {
                BotSession.loggedIn = true;
                webBrowser1.Document.GetElementById("game_start_button").InvokeMember("click");
                BotMethods.WriteLine("Succesfully logged in. Session ID: " + BotSession.Sessionid);
                BotMethods.WriteLine("Establishing Seafight connection...");
            }
        }

        private void StartToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (BotSession.loggedIn && Server.Connected)
            {
                if (!Account.OnBM && !Account.OnRaid && !TaskSystem.TasksSystem.UseTaskSystem)
                {
                    BotMethods.WriteLine("Starting Bot...");
                }
                this.CreateSettings();
                BotLogic.StartBotThread();
                BotSession.sessionStartTime = DateTime.Now;
            }
            else BotMethods.WriteLine("LogIn first!");
        }

        private void StopToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (BotSession.loggedIn && Server.Connected)
            {
                WriteLine("Stopping Bot...");
                BotLogic.StopBotThread();
                BotLogic.StoponAttackThrad();
                TaskSystem.TasksSystem.StopTaskSystem();
            }
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            this.SaveSettings();
            BotCalculator.StopThreads();
        }

        private void LogBox_LinkClicked(object sender, LinkClickedEventArgs e)
        {
            if (e.LinkText.Contains("buy.php"))
            {
                ShowPaymentForm();
                return;
            }
            Process.Start(e.LinkText);
        }

        private void Form1_Resize(object sender, EventArgs e)
        {
            bool hidden = false;
            if (base.WindowState == FormWindowState.Minimized)
            {
                this.notifyIcon1.Visible = true;
                this.notifyIcon1.Text = base.Text;
                base.WindowState = FormWindowState.Normal;
                if (!hidden)
                {
                    string tipTitle = $"BoxyBot - V.{Program.BountyBotVersion}";
                    string tipText = "BoxyBot will continue running in the background,\nDouble Click the Icon to reopen the Bot.";
                    this.notifyIcon1.ShowBalloonTip(5000, tipTitle, tipText, ToolTipIcon.Info);
                    hidden = true;
                }
                base.Hide();
            }
        }

        private void notifyIcon1_DoubleClick(object sender, EventArgs e)
        {
            base.WindowState = FormWindowState.Normal;
            base.Show();
            base.Activate();
            this.notifyIcon1.Visible = false;
        }

        private void joinBehemothMapButton_Click(object sender, EventArgs e)
        {
            if (BotSession.loggedIn && Server.Connected)
            {
                if (Bot.Running)
                {
                    BotMethods.WriteLine("Stop the Bot first!");
                    return;
                }
                BotSettings.RaidType = Account.Level > 16 ? Account.Level > 25 ? 3 : 2 : 1;
                BotMethods.ActivateItem(Account.RaidMedallion);

            }
            else BotMethods.WriteLine("LogIn first!");
        }

        private void leaveRaidMapButton_Click(object sender, EventArgs e)
        {
            if (BotSession.loggedIn && Server.Connected)
            {
                if (Bot.Running)
                {
                    BotMethods.WriteLine("Stop the Bot first!");
                    return;
                }
                if (!Account.OnRaid)
                    return;
                BotMethods.Revive();
                BotMethods.WriteLine("Leaving Raid!");
            } 
            else BotMethods.WriteLine("LogIn first!");
        }

        private void resetClientToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (base.Size != new Size(1037, 803))
            {
                base.Size = new Size(1037, 803);
                BotMethods.WriteLine("Client Reset.");
            }
            base.WindowState = FormWindowState.Normal;
        }

        private void shootRaidBossCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            BotSettings.shootRaidBoss = shootRaidBossCheckBox.Checked;
            if (BotSettings.shootRaidBoss)
            {
                raidBossAmmoBox.Enabled = true;
                usePlatesRaidBossCheckbox.Enabled = true;
                usePowderRaidBossCheckbox.Enabled = true;
                useRaidBossSnowmanCheckbox.Enabled = true;
                useRaidBossCandleCheckbox.Enabled = true;
                boardVCMACheckBox.Enabled = true;
            }
            else
            {
                raidBossAmmoBox.Enabled = false;
                usePlatesRaidBossCheckbox.Enabled = false;
                usePowderRaidBossCheckbox.Enabled = false;
                useRaidBossSnowmanCheckbox.Enabled = false;
                useRaidBossCandleCheckbox.Enabled = false;
                boardVCMACheckBox.Enabled = false;
            }
        }

        private void checkAllNPCsCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            if (resetNPCsCheckbox.Checked)
            {
                LoadDefaultGridItems(true, false);
                resetNPCsCheckbox.Checked = false;
                BotMethods.WriteLine("NPC List reset.");
            }
        }

        private void CheckAllMonstersCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            if (resetMonstersCheckBox.Checked)
            {
                LoadDefaultGridItems(false, true);
                resetMonstersCheckBox.Checked = false;
                BotMethods.WriteLine("Monster List reset.");
            }
        }

        private void joinBMButton_Click(object sender, EventArgs e)
        {
            try
            {
                if (BotSession.loggedIn && Server.Connected)
                {
                    if (Bot.Running)
                    {
                        BotMethods.WriteLine("Stop the Bot first!");
                        return;
                    }
                    if (bmComboBox.Text.Length <= 2)
                    {
                        BotMethods.WriteLine("Select a Bonusmap first!");
                        return;
                    } 
                    BotSettings.lastBM = bmComboBox.Text.Replace(help.Between(bmComboBox.Text, " (", ")"), "").Replace(" (", "").Replace(")", "");
                    var mapId = Account.BonusMaps.Values.FirstOrDefault(bm => bm.mapName == BotSettings.lastBM).mapId;
                    BotMethods.JoinBonusMap(mapId);
                }
                else BotMethods.WriteLine("LogIn first!");
            }
            catch (Exception ex)
            {
                BotMethods.WriteLine("There was an error while joining Bonusmap!\n\n" + ex.Message);
            }
        }

        private void autoJoinBMCheckBox_CheckStateChanged(object sender, EventArgs e)
        {
            BotSettings.autoJoinBM = autoJoinBMCheckBox.Checked;
            if (BotSettings.autoJoinBM)
            {
                joinSameBMCheckbox.Enabled = true;
            }
            else
            {
                joinSameBMCheckbox.Enabled = false;
            }
        }

        private void rebuyAmmoCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            BotSettings.rebuyAmmo = rebuyAmmoCheckBox.Checked;
            if (BotSettings.rebuyAmmo)
            {
                rebuyAmountBox.Enabled = true;
                rebuyAmmoComboBox.Enabled = true;
                label12.Enabled = true;
            }
            else
            {
                rebuyAmountBox.Enabled = false;
                rebuyAmmoComboBox.Enabled = false;
                label12.Enabled = false;
            }
        }

        private void rebuyAmountBox_TextChanged(object sender, EventArgs e)
        {
            BotSettings.ReBuyAmmoAmount = help.ToInt(rebuyAmountBox.Text.Replace(".", ""));
        }

        private void webBrowser1_Navigated(object sender, WebBrowserNavigatedEventArgs e)
        {
            BotSession.currentCookies = webBrowser1.Document.Cookie;
            BotSession.currentUrl = e.Url.AbsoluteUri;
        }

        private void rebuyAmmoComboBox_TextChanged(object sender, EventArgs e)
        {
            if (rebuyAmmoComboBox.Text.Length > 1)
            {
                BotSettings.rebuyAmmoID = BotHandlers.AmmoHandler(rebuyAmmoComboBox.Text);
            }
        }

        private void fleeRadioButton_CheckedChanged(object sender, EventArgs e)
        {
            BotSettings.ShootBackType = 1;
            shootbackAmmoComboBox.Enabled = false;
            shootbackPlatesCheckBox.Enabled = false;
            shootbackPowderCheckBox.Enabled = false;
            useShootBackHPItemCheckBox.Enabled = false;
            useShootBackVPItemCheckbox.Enabled = false;
            shootbackHPItemCombobox.Enabled = false;
            shootbackVPItemCombobox.Enabled = false;
            useSkyfireCheckbox.Enabled = false;
            useElmosFireCheckbox.Enabled = false;
            panel1.Enabled = true;
            attackSightTextbox.Enabled = false;
            attackSightCheckbox.Enabled = false;
        }

        private void IgnoreRadioButton_CheckedChanged(object sender, EventArgs e)
        {
            BotSettings.ShootBackType = 0;
            shootbackAmmoComboBox.Enabled = false;
            shootbackPlatesCheckBox.Enabled = false;
            shootbackPowderCheckBox.Enabled = false;
            useShootBackHPItemCheckBox.Enabled = false;
            useShootBackVPItemCheckbox.Enabled = false;
            shootbackHPItemCombobox.Enabled = false;
            shootbackVPItemCombobox.Enabled = false;
            useSkyfireCheckbox.Enabled = false;
            useElmosFireCheckbox.Enabled = false;
            panel1.Enabled = false;
            attackSightTextbox.Enabled = false;
            attackSightCheckbox.Enabled = false;
        }

        private void shootbackRadioButton_CheckedChanged(object sender, EventArgs e)
        {
            BotSettings.ShootBackType = 2;
            shootbackAmmoComboBox.Enabled = true;
            shootbackPlatesCheckBox.Enabled = true;
            shootbackPowderCheckBox.Enabled = true;
            useShootBackHPItemCheckBox.Enabled = true;
            useShootBackVPItemCheckbox.Enabled = true;
            useSkyfireCheckbox.Enabled = true;
            useElmosFireCheckbox.Enabled = true;
            panel1.Enabled = false;
            attackSightCheckbox.Enabled = true;
            if (attackSightCheckbox.Checked)
            {
                attackSightTextbox.Enabled = true;
            }
        }

        private void automaticOnAttackedModeCheckbox_CheckedChanged(object sender, EventArgs e)
        {
            BotSettings.ShootBackType = 3;
            shootbackAmmoComboBox.Enabled = true;
            shootbackPlatesCheckBox.Enabled = true;
            shootbackPowderCheckBox.Enabled = true;
            useShootBackHPItemCheckBox.Enabled = true;
            useShootBackVPItemCheckbox.Enabled = true;
            useSkyfireCheckbox.Enabled = true;
            useElmosFireCheckbox.Enabled = true;
            panel1.Enabled = true;
            attackSightCheckbox.Enabled = true;
            if (attackSightCheckbox.Checked)
            {
                attackSightTextbox.Enabled = true;
            }
        }

        private void shootbackAmmoComboBox_TextChanged(object sender, EventArgs e)
        {
            BotSettings.AmmoIDShootBack = BotHandlers.AmmoHandler(shootbackAmmoComboBox.Text);
        }

        private void rebuyHarpoonsCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            BotSettings.rebuyHarpoons = rebuyHarpoonsCheckBox.Checked;
            if (BotSettings.rebuyHarpoons)
            {
                rebuyHarpoonsComboBox.Enabled = true;
                label22.Enabled = true;
                rebuyHarpoonsAmountComboBox.Enabled = true;
            }
            else
            {
                rebuyHarpoonsComboBox.Enabled = false;
                label22.Enabled = false;
                rebuyHarpoonsAmountComboBox.Enabled = false;
            }
        }

        private void rebuyHarpoonsComboBox_TextChanged(object sender, EventArgs e)
        {
            if (rebuyHarpoonsComboBox.Text.Length > 1)
                BotSettings.rebuyHarpoonsID = BotHandlers.HarpoonHandler(rebuyHarpoonsComboBox.Text);
        }

        private void rebuyHarpoonsAmountComboBox_TextChanged(object sender, EventArgs e)
        {
            BotSettings.ReBuyHarpoonsAmount = help.ToInt(rebuyHarpoonsAmountComboBox.Text.Replace(".", ""));
        }

        private void jumpMapsCheckbox_CheckedChanged(object sender, EventArgs e)
        {
            BotSettings.jumpMaps = jumpMapsCheckbox.Checked;
            if (BotSettings.jumpMaps)
            {
                jumpMapTimerBox.Enabled = true;
                jumpMapRandomRadioButton.Enabled = true;
                jumpMapCircleRadioButton.Enabled = true;
                jumpMapIfAvailableCheckbox.Enabled = true;
            }
            else
            {
                jumpMapTimerBox.Enabled = false;
                jumpMapRandomRadioButton.Enabled = false;
                jumpMapCircleRadioButton.Enabled = false;
                jumpMapIfAvailableCheckbox.Enabled = false;
            }
        }

        private void useDesignChangerCheckbox_CheckedChanged(object sender, EventArgs e)
        {
            BotSettings.useDesignChanger = useDesignChangerCheckbox.Checked;
            if (BotSettings.useDesignChanger)
            {
                botDesignComboBox.Enabled = true;
                repDesignComboBox.Enabled = true;
                label37.Enabled = true;
                label38.Enabled = true;
            }
            else
            {
                botDesignComboBox.Enabled = false;
                repDesignComboBox.Enabled = false;
                label37.Enabled = false;
                label38.Enabled = false;
            }
        }

        private void leaveBMButton_Click(object sender, EventArgs e)
        {
            if (BotSession.loggedIn && Server.Connected)
            {
                if (Bot.Running)
                {
                    BotMethods.WriteLine("Stop the Bot first!");
                    return;
                }
                if (!Account.OnBM)
                    return;
                BotMethods.Revive();
                BotMethods.WriteLine("Leaving Bonus Map!");
            }
            else BotMethods.WriteLine("LogIn first!");
        }

        private void useShootBackHPItemCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            BotSettings.useShootbackHPItem = useShootBackHPItemCheckBox.Checked;
            if (BotSettings.useShootbackHPItem)
            {
                shootbackHPItemCombobox.Enabled = true;
            }
            else
            {
                shootbackHPItemCombobox.Enabled = false;
            }
        }

        private void useShootBackVPItemCheckbox_CheckedChanged(object sender, EventArgs e)
        {
            BotSettings.useShootbackVPItem = useShootBackVPItemCheckbox.Checked;
            if (BotSettings.useShootbackVPItem)
            {
                shootbackVPItemCombobox.Enabled = true;
            }
            else
            {
                shootbackVPItemCombobox.Enabled = false;
            }
        }

        private void finishNPCsCheckbox_CheckedChanged(object sender, EventArgs e)
        {
            BotSettings.finishNPCs = finishNPCsCheckbox.Checked;
            if (BotSettings.finishNPCs)
            {
                finishNPCHPLimitUpDown.Enabled = true;
                label63.Enabled = true;
            }
            else
            {
                finishNPCHPLimitUpDown.Enabled = true;
                label63.Enabled = false;
            }
        }

        private void attackSightCheckbox_CheckedChanged(object sender, EventArgs e)
        {
            BotSettings.attackOnSight = attackSightCheckbox.Checked;
            if (BotSettings.attackOnSight)
            {
                attackSightTextbox.Enabled = true;
            }
            else
            {
                attackSightTextbox.Enabled = false;
            }
        }

        private void repairOnPlaceCheckbox_CheckedChanged(object sender, EventArgs e)
        {
            if (repairOnPlaceCheckbox.Checked)
            {
                BotSettings.repatborder = false;
                BotSettings.repatcorner = false;
            }
        }

        private void ResetStatistics()
        {
            BotSession.Sessiongold = 0;
            BotSession.Sessionpearls = 0;
            BotSession.Sessionmojos = 0;
            BotSession.Sessioncrowns = 0;
            BotSession.Sessionxp = 0;
            BotSession.Sessionhp = 0;
            BotSession.Sessionglitters = 0;
            BotSession.Sessionchests = 0;
            BotSession.Sessionnpcs = 0;
            BotSession.Sessionmonsters = 0;
            BotSession.Sessiondeaths = 0;
            BotSession.Sessionbmnpcs = 0;
            BotSession.Sessionbms = 0;
            BotSession.Sessionwaves = 0;
            BotSession.Sessionelps = 0;
            BotSession.Sessionpowder = 0;
            BotSession.Sessionplates = 0;
            BotSession.Sessioncrystals = 0;
            BotSession.Sessionreconnects = 0;
            BotSession.sessionStartTime = DateTime.Now;
        }

        private void ResetUserStatistics()
        {
            try
            {
                userBPBar.Value = 0;
                userBPBar.Maximum = 0;
                userBPLabel.Text = "/";
                userCrystalsLabel.Text = "/";
                userCursedSoulsLabel.Text = "/";
                userGoldLabel.Text = "/";
                userGuildLabel.Text = "/";
                userHPBar.Value = 0;
                userHPBar.Maximum = 0;
                userHPLabel.Text = "/";
                userMapLabel.Text = "/";
                userMojosLabel.Text = "/";
                userNameLabel.Text = "/";
                userPearlsLabel.Text = "/";
                userPositionLabel.Text = "/";
                userRadiantSoulsLabel.Text = "/";
                userRepairmateLabel.Text = "/";
                userSessionLabel.Text = "/";
                userUIDLabel.Text = "/";
                userVPBar.Value = 0;
                userVPBar.Maximum = 0;
                userVPLabel.Text = "/";
                userXPBar.Value = 0;
                userXPBar.Maximum = 0;
                userXPLabel.Text = "/";
            } catch (Exception)
            {
            }
        }

        private void resetStatisticsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.ResetStatistics();
            BotMethods.WriteLine("Statistics Reset.");
        }

        private void userNameTextbox_TextChanged(object sender, EventArgs e)
        {
            BotSession.Username = userNameTextbox.Text;
            if (BotSession.userLoginData.ContainsKey(BotSession.Username))
            {
                userPassTextbox.Text = BotSession.userLoginData[BotSession.Username];
            }
        }

        private void userPassTextbox_TextChanged(object sender, EventArgs e)
        {
            BotSession.Password = userPassTextbox.Text;
        }

        private void logInButton_Click(object sender, EventArgs e)
        {
            if (userNameTextbox.Text.Length > 1 && userPassTextbox.Text.Length > 1)
            {
                if (Account.gClass == null)
                {
                    BotMethods.WriteLine(string.Join("", new string[] {
                        HelpTools.FromByteToString(76),
                        HelpTools.FromByteToString(105),
                        HelpTools.FromByteToString(99),
                        HelpTools.FromByteToString(101),
                        HelpTools.FromByteToString(110),
                        HelpTools.FromByteToString(115),
                        HelpTools.FromByteToString(101),
                        HelpTools.FromByteToString(32),
                        HelpTools.FromByteToString(69),
                        HelpTools.FromByteToString(120),
                        HelpTools.FromByteToString(112),
                        HelpTools.FromByteToString(105),
                        HelpTools.FromByteToString(114),
                        HelpTools.FromByteToString(101),
                        HelpTools.FromByteToString(100),
                        HelpTools.FromByteToString(33)
                    }));
                    return;
                }
                if (BotSession.loggedIn && MessageBox.Show("Do you really want to create a new Session?", "Create new Session.", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.No)
                {
                    return;
                }
                Client.EntityMovementList.Clear();
                Client.Entitys.Clear();
                logInButton.Enabled = false;
                if (!BotSession.userLoginData.ContainsKey(userNameTextbox.Text))
                {
                    BotSession.userLoginData.Add(userNameTextbox.Text, userPassTextbox.Text);
                }
                if (!userNameTextbox.Items.Contains(userNameTextbox.Text))
                {
                    userNameTextbox.Items.Add(userNameTextbox.Text);
                } 
                try
                {
                    if (Bot.Running)
                    {
                        BotLogic.StopBotThread();
                    }
                    if (BotLogic.OnAttackRunning)
                    {
                        BotLogic.StartonAttackThread();
                    }
                    BotCalculator.StopThreads();
                }
                catch (Exception ex)
                {
                    BotMethods.WriteLine("There was an Exception while closing current Threads!\n" + ex.Message);
                }
                BotSession.logginIn = false;
                BotSession.loggedIn = false;
                BotSession.Username = userNameTextbox.Text;
                BotSession.Password = userPassTextbox.Text;
                this.Text = "BoxyBot - V." + Program.BountyBotVersion + " " + BotSession.Username;
                this.browserHiderPicBox.Visible = false;
                this.LoginMethod();
            }
            else
            {
                MessageBox.Show("Please enter valid login information!", "Invalid Information!", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }  
        }

        private void reInstallCertCheckbox_CheckedChanged(object sender, EventArgs e)
        {
            if (reInstallCertCheckbox.Checked)
            {
                if (CertMaker.removeFiddlerGeneratedCerts(true))
                {
                    BotMethods.WriteLine("Unistalled BoxyBot Certificates.");
                }
                MessageBox.Show("All Certificates have been removed!\nPlease restart the Bot, for the changes to take affect!", "Unistalled Certificate.", MessageBoxButtons.OK, MessageBoxIcon.Information);
                reInstallCertCheckbox.Checked = false;
            }
        }

        private void clearCacheCheckbox_CheckedChanged(object sender, EventArgs e)
        {
            BotSettings.clearCache = clearCacheCheckbox.Checked;
        }

        private void addTaskButton_Click(object sender, EventArgs e)
        {
            if (help.IsNumeric(timeTaskBox.Text))
            {
                var task = typeTaskBox.Text.Replace("X", timeTaskBox.Text);
                tasksListBox.Items.Add(task);
                var taskType = help.GetTaskType(typeTaskBox.Text);
                if (taskType != "null")
                {
                    TaskSystem.TasksSystem.AddNewTask(taskType, help.ToInt(timeTaskBox.Text));
                }
                else
                {
                    MessageBox.Show("Unkown Task Type!\nPlease Retry, if this Error continues contact the developer.", "Unkown Type.", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            else
            {
                MessageBox.Show("Invalid Task Duration!\nPlease check the entered Duration Value!", "Invalid Value.", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void removeTaskButton_Click(object sender, EventArgs e)
        {
            try
            {
                tasksListBox.Items.Remove(BotSession.selectedTaskIndex);
                TaskSystem.TasksSystem.Tasks.Remove(TaskSystem.TasksSystem.Tasks.ElementAt(BotSession.selectedTaskIndex).Key);
            }
            catch (Exception ex)
            {
                BotMethods.WriteLine("Could not remove Task.\n" + ex.Message);
            }
        }

        private void updateFormTimer_Tick(object sender, EventArgs e)
        {
            UpdateStats();
        }


        private void showNPCsButton_Click(object sender, EventArgs e)
        {
            if (BotSession.loggedIn && Bot.NPCs.Count > 0)
            {
                var _npcs = help.SortDictionary_2(Bot.NPCs).Values.ToList<string>();
                AllNPCsForm npcsForm = new AllNPCsForm(_npcs);
                if (npcsForm.ShowDialog(this) == DialogResult.OK)
                {
                    Dictionary<int, string> npcs = new Dictionary<int, string>();
                    foreach (var npc in npcsForm.selectedNPCs)
                    {
                        npcs.Add(Bot.NPCs.FirstOrDefault(item => item.Value == npc).Key, npc);
                    }
                    Dictionary<int, string> NPCs = help.SortDictionary_2(npcs);
                    foreach (var item in NPCs)
                    {
                        if (!help.GridViewContains(npcGridView, item.Value))
                        {
                            npcGridView.Rows.Add(new object[] { false, item.Value, "Hollowballs (20)", false, false, false, "0" });
                        }
                    }
                }
                npcsForm.Close();
                npcsForm.Dispose();
            }
            else BotMethods.WriteLine("Login first!");
        }

        private void npcGridView_DataError(object sender, DataGridViewDataErrorEventArgs e)
        {
            if (npcGridView.Rows[e.RowIndex].Cells[e.ColumnIndex] is DataGridViewCheckBoxCell)
            {
                npcGridView.Rows[e.RowIndex].Cells[e.ColumnIndex].Value = "0";
            }
            if (npcGridView.Rows[e.RowIndex].Cells[e.ColumnIndex] is DataGridViewTextBoxCell)
            {
                npcGridView.Rows[e.RowIndex].Cells[e.ColumnIndex].Value = "0";
            }
            e.Cancel = true;
        }

        private void monsterGridView_DataError(object sender, DataGridViewDataErrorEventArgs e)
        {
            if (monsterGridView.Rows[e.RowIndex].Cells[e.ColumnIndex] is DataGridViewCheckBoxCell)
            {
                monsterGridView.Rows[e.RowIndex].Cells[e.ColumnIndex].Value = "0";
            }
            if (monsterGridView.Rows[e.RowIndex].Cells[e.ColumnIndex] is DataGridViewTextBoxCell)
            {
                monsterGridView.Rows[e.RowIndex].Cells[e.ColumnIndex].Value = "0";
            }
            e.Cancel = true;
        }

        private void showAllMonstersButton_Click(object sender, EventArgs e)
        {
            if (BotSession.loggedIn && Bot.Monsters.Count > 0)
            {
                var _monsters = help.SortDictionary_2(Bot.Monsters).Values.ToList<string>();
                var monstersForm = new AllMonstersForm(_monsters);
                if (monstersForm.ShowDialog(this) == DialogResult.OK)
                {
                    Dictionary<int, string> monsters = new Dictionary<int, string>();
                    foreach (var monster in monstersForm.selectedMonsters)
                    {
                        monsters.Add(Bot.Monsters.FirstOrDefault(item => item.Value == monster).Key, monster);
                    }
                    Dictionary<int, string> Monsters = help.SortDictionary_2(monsters);
                    foreach (var item in Monsters)
                    {
                        if (!help.GridViewContains(monsterGridView, item.Value))
                        {
                            monsterGridView.Rows.Add(new object[] { false, item.Value, "Bronze Harpoons (50)", "0" });
                        }
                    }
                }
                monstersForm.Close();
                monstersForm.Dispose();
            }
            else BotMethods.WriteLine("Login first!");
        }

        private void webBrowser1_Navigating(object sender, WebBrowserNavigatingEventArgs e)
        {
            if (e.Url.AbsoluteUri.Contains("elitepvpers") || e.Url.AbsoluteUri.Contains("seafight"))
            {
                browserHiderPicBox.Visible = false;
            }
        }

        private void useItemCheckbox_CheckedChanged(object sender, EventArgs e)
        {
            BotSettings.useActionItemUser = useItemCheckbox.Checked;
            if (BotSettings.useActionItemUser)
            {
                useItemActionCombobox.Enabled = true;
                useItemComboBox.Enabled = true;
                useItemLabel.Enabled = true;
            }
            else
            {
                useItemActionCombobox.Enabled = false;
                useItemComboBox.Enabled = false;
                useItemLabel.Enabled = false;
            }
        }

        private void saveSettingsButton_Click(object sender, EventArgs e)
        {
            this.SaveSettings();
            BotMethods.WriteLine("Settings saved.");
        }

        private void tasksListBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            BotSession.selectedTaskIndex = tasksListBox.SelectedIndex;
        }

        private void hideLogButton_Click(object sender, EventArgs e)
        {
            if (LogBox.Visible)
            {
                LogBox.Visible = false;
                hideLogButton.Text = "Show Log";
                tabControl1.Height += LogBox.Size.Height;
            }
            else
            {
                LogBox.Visible = true;
                hideLogButton.Text = "Hide Log";
                tabControl1.Height -= LogBox.Size.Height;
            }
        }

        public void StartLocalProxy()
        {
            Server server = new Server();
            this.localThread = new Thread(server.Start);
            this.localThread.IsBackground = false;
            this.localThread.Start();
            this.StartBrowserProxy();
        }

        public void StartBrowserProxy()
        {
            BrowserProxy.Start();
            WinInetInterop.SetConnectionProxy(string.Concat(new object[]
                {
                    "127.0.0.1",
                    ":",
                    Server.FiddlerPort
                })); 
        }

        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
            this.notifyIcon1.Visible = false;
            Process.GetCurrentProcess().Kill();
        }

        private void webBrowser1_NewWindow(object sender, System.ComponentModel.CancelEventArgs e)
        {
            webBrowser1.Navigate(this.webBrowser1.Url.AbsoluteUri.Replace("action=internalStart&loginDone=true", "action=internalMap"));
            e.Cancel = true;
        }

        public void ShowPaymentForm()
        {
            var paymentForm = new PaymentForm();
            paymentForm.Show(this);
            BotMethods.WriteLine("Loading...");
        }

        private void buylicensebutton_Click(object sender, EventArgs e)
        {
            ShowPaymentForm();
        }

        private void addNPCNameButton_Click(object sender, EventArgs e)
        {
            npcGridView.Rows.Add(new object[] { false, addNPCNameTextbox.Text, "Hollowballs (20)", false, false, false, "0" });
        }

        private void addMonsterNameButton_Click(object sender, EventArgs e)
        {
            monsterGridView.Rows.Add(new object[] { false, addMonsterNameTextbox.Text, "Bronze Harpoons (50)", "0" });
        }

        private void ammoBmChangeCheckbox_CheckedChanged(object sender, EventArgs e)
        {
            if (ammoBmChangeCheckbox.Checked)
            {
                label86.Enabled = true;
                ammoBMWaveBox.Enabled = true;
                ammoBMWaveNumeric.Enabled = true;
            } else
            {
                label86.Enabled = false;
                ammoBMWaveBox.Enabled = false;
                ammoBMWaveNumeric.Enabled = false;
            }
        }

        private void useQuestSystemCheckbox_CheckedChanged(object sender, EventArgs e)
        {
            if (useQuestSystemCheckbox.Checked)
            {
                label87.Enabled = true;
                selectedQuestsListbox.Enabled = true;
            } else
            {
                label87.Enabled = false;
                selectedQuestsListbox.Enabled = false;
            }
        }
    }
}
