using BoxyBot.Seafight;
using BoxyBot.Seafight.Messages;
using BoxyBot.Seafight.Constants;
using BoxyBot.Util;
using System;
using System.Diagnostics;
using System.Linq;
using System.Net;

namespace BoxyBot
{
    public static class BotMethods
    {
        private static HelpTools help = new HelpTools();
        private static Random random = new Random();

        public static void MoveTo(int X, int Y)
        {
            Server.Send(new MoveMessage(X, Y));
        }

        public static void MoveTo(PositionStub position)
        {
            MoveTo(position.X, position.Y);
        }

        public static void Repair()
        {
            Server.Send(new RepairMessage(0));
        }

        public static void StopRepair()
        {
            Server.Send(new RepairMessage(1));
        }

        public static void Revive()
        {
            Server.Send(new MapReturnToOldMapMessage());
        }

        public static void ReviveRaid()
        {
            Server.Send(new MapReturnToMapMessage());
        }

        public static void JumpMap(int MapID)
        {
            Server.Send(new MapChangeRequestMessage(MapID));
        }

        public static void JoinBonusMap(int MapID)
        {
            Server.Send(new MapChangeDelayedMessage(MapID));
        }

        public static void JoinSafeHeaven(int MapID)
        {
            Server.Send(new MapChangeDelayedAskMessage(MapID));
        }

        public static void ActivateItem(int itemID)
        {
            Server.Send(new ActionItemUseMessage(itemID));
        }

        public static void ActivateRocket(int RocketID, EntityInfo hash)
        {
            Server.Send(new ActionItemAttackMessage(RocketID, hash.entityId, hash.projectId));
        }

        public static void Powder()
        {
            ActivateItem(Items.POWDER);
        }

        public static void Plates()
        {
            ActivateItem(Items.PLATES);
        }

        public static void Attack(EntityInfo ship, int ammoId)
        {
            if (Account.Repairing)
            {
                StopRepair();
            }
            Server.Send(new CannonAttackMessage(-ship.entityId, ship.projectId, ammoId));
        }

        public static void AttackMonster(EntityInfo monster, int harpoonId)
        {
            if (Account.Repairing)
            {
                StopRepair();
            }
            Server.Send(new HarpoonAttackMessage(monster.entityId, monster.projectId, harpoonId));
        }

        public static void StopAttack()
        {
            Server.Send(new AbortAttackMessage());
        }

        public static void BoardShip(EntityInfo hash)
        {
            if (Client.Entitys.ContainsKey(hash.entityId) && Client.Entitys[hash.entityId] is ShipInitMessage)
            {
                Server.Send(new BoardUserMessage(hash.entityId, hash.projectId));
            }
        }

        public static void Logout(int duration)
        {
            Server.Send(new LogoutMessage(duration));
        }

        public static void AcceptLoginBonus()
        {
            Server.Send(new AcceptLoginBonusMessage());
        }

        public static void AcceptQuest(int questId)
        {
            Server.Send(new QuestMessage(questId, 1));
        }

        public static void AbortQuest(int questId)
        {
            Server.Send(new QuestMessage(questId, 2));
        }

        public static void Teleport(int mapId)
        {
            Server.Send(new UseTeleportMessage(mapId));
        }

        public static void ChangeDesign(int designId)
        {
            if (Account.DesignId != designId)
            {
                Server.Send(new ChangeDesignMessage(designId, Account.MedallionId));
                var design = Bot.Designs.FirstOrDefault(key => key.Value == designId);
                var designType = designId == BotSettings.repDesign ? " [Repair Design]" : designId == BotSettings.botDesign ? " [Bot Design]" : " [Unkown Design]\n\n<WARNING> REPORT THIS INSTANTLY!\nINFO: " + designId + ";" + Account.MedallionId + ";";
                if (design.Key != null)
                    WriteLine("Changing Design to " + design.Key + designType);
            }
        }

        public static bool BuyItem(int itemType, int itemID, int amount)
        {
            string Type = (itemType == 0) ? "harpoons" : itemType == 1 ? "ammunition" : (itemType == 2 ? "inventory" : "fireworks");
            try
            {
                string error = "https://" + BotSession.Server + ".seafight.bigpoint.com/api/client/handleQuickBuy.php";
                error = help.CreateWebRequest(BotSession.currentCookies, error);
                string rtvt = help.Between(help.Between(error, "<RTVT>", "</RTVT>"), "![CDATA[", "]]");
                string response = help.CreateWebRequest(BotSession.currentCookies, $"https://{BotSession.Server}.seafight.bigpoint.com/api/client/handleQuickBuy.php?RTVT={rtvt}&itemID={itemID}&itemType={Type}&itemAmount={amount}");
                if (!response.Contains("error"))
                {
                    string result = help.Between(response, "<result>", "</result>");
                    WriteLine(help.Between(result, "![CDATA[", "]]"));
                    return true;
                }
                return false;
            }
            catch (Exception ex)
            {
                WriteLine("Error on Buy Item Request!\n\n" + ex.Message);
            }
            return false;
        }

        public static void ClearCache()
        {
            try
            {
                Process.Start("rundll32.exe", "InetCpl.cpl,ClearMyTracksByProcess 10");
            }
            catch (Exception ex)
            {
                WriteLine("Could not clear cache.\n" + ex.Message);
            }
        }

        public static bool GetCompileTime(string server = "int1")
        {
            for (int i = 0; i < 4; i++)
            {
                try
                {
                    using (WebClient wc = new WebClient())
                    {
                        if (wc.DownloadString($"https://{server}.seafight.bigpoint.com/api/client/getCompileTime.php") == Program.compileTime)
                            return true;
                    }
                }
                catch (Exception ex)
                {
                    WriteLine("There was an error while getting compile Time!\n" + ex.Message + "\nRetrying in 1 Minute");
                }
                System.Threading.Thread.Sleep(60000);
            }
            return false;
        }

        public static void WriteLine(string message)
        {
            try
            {
                Form1.form1.Invoke(Form1.form1.writer, new string[]
                {
                    message
                });
            }
            catch (Exception)
            {
            }
        }
    }
}
