using BoxyBot.Seafight;
using BoxyBot.Seafight.Messages;
using BoxyBot.Util;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading;
using BoxyBot.Licensing;
using System.Text.RegularExpressions;

namespace BoxyBot
{
    public static class Bot
    {
        private static HelpTools help = new HelpTools();
        public static bool Running;
        public static bool addedHandlers;
        public static bool usedActionItem;
        public static bool addedItemUser;
        public static Thread _botThread;
        public static DateTime nextMapJump = DateTime.Now;
        public static DateTime nextActionItem = DateTime.Now;
        public static Dictionary<string, bool> entitys = new Dictionary<string, bool>();
        public static Dictionary<string, bool> quests = new Dictionary<string, bool>();
        public static Dictionary<string, int> entitysAmount = new Dictionary<string, int>();
        public static Dictionary<double, Message> invalidEntitys = new Dictionary<double, Message>();
        public static Dictionary<int, string> NPCs { get; private set; }
        public static Dictionary<int, string> Monsters { get; private set; }
        public static Dictionary<string, int> Designs { get; private set; }
        public static Dictionary<string, int> Items { get; private set; }
        public static Dictionary<string, int> Quests { get; private set; }
        private static List<Point> sideCollisionList = new List<Point>();
        public static EntityInfo targetentityId = new EntityInfo(0.0, 0);
        public static EntityInfo lastEntity = new EntityInfo(0.0, 0);
        public static EntityInfo underAttackBy = new EntityInfo(0.0, 0);
        public static QuestConditionStub currentQuest;
        public static TowerInitMessage userGuildTower;
        public static AuthClient auth;
        public static int quarter;
        public static Point corner_1 = new Point(4, 0);
        public static Point corner_2 = new Point(300, 295);
        public static Point corner_3 = new Point(298, -293);
        public static Point corner_4 = new Point(594, 2);
        public static Point moveTo_1 = new Point(60, 55);
        public static Point moveTo_2 = new Point(338, 230);
        public static Point moveTo_3 = new Point(310, -204);
        public static Point moveTo_4 = new Point(506, -54);
        public static Point moveTo_5 = new Point(300, 0);
        public static Point mapJumpUp = new Point(150, 150);
        public static Point mapJumpDown = new Point(445, -150);
        public static Point mapJumpLeft = new Point(150, -150);
        public static Point mapJumpRight = new Point(450, 150);

        private static void CreateSideCollisionList()
        {
            sideCollisionList = new List<Point>
            {
                new Point(20,-20),
                new Point(340,255),
                new Point(110,-110),
                new Point(440,155),
                new Point(180,180),
                new Point(515,80),
                new Point(250,-250),
                new Point(565,30),
                new Point(295,-295),
                new Point(320, 275)
            };
        }

        public static void CheckTimers()
        {
            try
            {
                TimeSpan _ts = (DateTime.Now - Bot.nextMapJump);
                if (BotSettings.jumpMaps && _ts.TotalMinutes >= BotSettings.switchMapTime)
                {
                    BotSettings.needMapSwitch = true;
                    Bot.nextMapJump = DateTime.Now.AddMinutes(BotSettings.switchMapTime);
                }
            }
            catch
            {
                Bot.nextMapJump = DateTime.Now.AddMinutes(BotSettings.switchMapTime);
                Bot.nextActionItem = DateTime.Now.AddSeconds(60.0);
            }
        }

        public static void AddItemUser()
        {
            if (!addedItemUser)
            {
                Account.HpChanged += (s, e) =>
                {
                    TimeSpan __ts = (DateTime.Now - Bot.nextActionItem);
                    if (BotSettings.useActionItemUser && __ts.TotalSeconds >= 60.0 && (Bot.Running || BotLogic.OnAttackRunning))
                    {
                        if (Account.GetCurrentHpPercent < (BotSettings.useActionItemType == 2 ? 50 : 100))
                        {
                            int ID = BotSettings.useActionItemID;
                            if (ID != 60 && ID != 59 && ID != 58 && ID != 57 && ID != 56 && ID != 53 && ID != 47 && ID != 35 && ID != 11 && ID != 10)
                            {
                                BotMethods.ActivateItem(ID);
                                BotMethods.WriteLine("Using " + Items.FirstOrDefault(item => item.Value == ID).Key);
                            }
                            else
                            {
                                if (!targetentityId.entityId.Equals(0.0) && (Client.Entitys[targetentityId.entityId] is ShipInitMessage))
                                {
                                    BotMethods.ActivateRocket(ID, targetentityId);
                                    BotMethods.WriteLine("Using " + Items.FirstOrDefault(item => item.Value == ID).Key + " on current Target.");
                                }
                            }
                        }
                        Bot.nextActionItem = DateTime.Now.AddSeconds(60.0);
                    }
                };
                addedItemUser = true;
            }
        }

        public static void BuyItems(bool bought = false)
        {
            try
            {
                if (!BotSettings.rebuyAmmo && !BotSettings.rebuyHarpoons && !BotSettings.rebuyKeys && !BotSettings.rebuyRaidMedallions)
                {
                    return;
                }
                if (BotSettings.rebuyAmmoID > 0)
                {
                    if (BotSettings.rebuyAmmoSmaller > 0)
                    {
                        if (Account.Ammo.ContainsKey(BotSettings.rebuyAmmoID) && Account.Ammo[BotSettings.rebuyAmmoID].value > BotSettings.rebuyAmmoSmaller)
                        {
                            return;
                        }
                    }
                    if (BotSettings.rebuyAmmo && Account.Gold >= (30 * BotSettings.ReBuyAmmoAmount) && BotSettings.rebuyAmmoID != 51)
                    {
                        bought = BotMethods.BuyItem(1, BotSettings.rebuyAmmoID, BotSettings.ReBuyAmmoAmount);
                    }
                    if (BotSettings.rebuyAmmo && BotSettings.rebuyAmmoID == 51)
                    {
                        if ((Account.Pearls >= (0.48 * BotSettings.ReBuyAmmoAmount)) && (!Account.Ammo.ContainsKey(51) || Account.Ammo[51].value <= 1000))
                        {
                            bought = BotMethods.BuyItem(1, 51, BotSettings.ReBuyAmmoAmount);
                        }
                    }
                }
                if (BotSettings.rebuyHarpoonsID > 0)
                {
                    if (BotSettings.rebuyHarpoonsSmaller > 0)
                    {
                        if (Account.Ammo.ContainsKey(BotSettings.rebuyHarpoonsID) && Account.Ammo[BotSettings.rebuyHarpoonsID].value > BotSettings.rebuyHarpoonsSmaller)
                        {
                            return;
                        }
                    }
                    if (BotSettings.rebuyHarpoons && Account.Gold >= 120 * BotSettings.ReBuyHarpoonsAmount && BotSettings.rebuyHarpoonsID != 75)
                    {
                        bought = BotMethods.BuyItem(0, BotSettings.rebuyHarpoonsID, BotSettings.ReBuyHarpoonsAmount);
                    }
                    if (BotSettings.rebuyHarpoons && BotSettings.rebuyHarpoonsID == 75)
                    {
                        if ((Account.Pearls >= (95 * BotSettings.ReBuyHarpoonsAmount)) && (!Account.Ammo.ContainsKey(75) || Account.Ammo[75].value <= 10))
                        {
                            bought = BotMethods.BuyItem(0, 75, BotSettings.ReBuyHarpoonsAmount);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                BotMethods.WriteLine(ex.ToString());
            }
        }

        public static bool RebuyRaidMedallions(bool bought = false)
        {
            if (BotSettings.rebuyRaidMedallions)
            {
                if (!Account.Items.ContainsKey(Account.RaidMedallion) || (Account.Items[Account.RaidMedallion] as ActionItemStub).amount <= 1)
                {
                    if (Account.Pearls >= 900 && BotSettings.RaidType < 3)
                    {
                        bought = BotMethods.BuyItem(3, Account.RaidMedallion, 1);
                    }
                }
            }
            return bought;
        }

        public static bool RebuyKeys(bool bought = false)
        {
            if (BotSettings.rebuyKeys)
            {
                if (Account.Keys <= 1)
                {
                    if (Account.Pearls >= 1600)
                    {
                        bought = BotMethods.BuyItem(2, 77, 1);
                    }
                }
            }
            return bought;
        }

        public static Point MoveToNPC(ShipInitMessage ship)
        {
            if (BotSettings.useHumanMovement)
            {
                var random = new Random();
                var range = (int)(Account.CanonRange > 12 ? Account.CanonRange - 6 : Account.CanonRange) + new Random().Next(-2, 2);
                var point = new Point(ship.position.X, ship.position.Y);
                var box = GetClosestBoxInRange(point, range);
                if (box.position.X == 0 || box.entityId.Equals(0.0) || box.position.Y == 0)
                {
                    BotMethods.MoveTo(ship.position);
                    Thread.Sleep(random.Next(100, 300));
                    for (int i = 0; i < Account.Route.Count; i++)
                    {
                        var pos = Account.Route.ElementAt(i);
                        if (BotCalculator.CalculateDistance(ship.position.X, ship.position.Y, pos.X, pos.Y) <= range && BotCalculator.CalculateDistance(ship.position.X, ship.position.Y, pos.X, pos.Y) >= range - 5)
                        {
                            point = new Point(pos.X, pos.Y);
                            break;
                        }
                    }
                }
                else
                {
                    point = new Point(box.position.X, box.position.Y);
                }
                BotMethods.MoveTo(point.X, point.Y);
                Thread.Sleep(random.Next(100, 250));
                return point;
            }
            else
            {
                BotMethods.MoveTo(ship.position);
                Thread.Sleep(100);
                return new Point(ship.position.X, ship.position.Y);
            }
        }

        public static Point MoveToMonster(MonsterInitMessage monster)
        {
            if (BotSettings.useHumanMovement)
            {
                var random = new Random();
                var range = 9 + random.Next(-3, 1);
                var point = new Point(monster.position.X, monster.position.Y);
                var box = GetClosestBoxInRange(point, 11);
                if (box.position.X == 0 || box.entityId.Equals(0.0) || box.position.Y == 0)
                {
                    BotMethods.MoveTo(monster.position);
                    Thread.Sleep(random.Next(100, 300));
                    for (int i = 0; i < Account.Route.Count; i++)
                    {
                        var pos = Account.Route.ElementAt(i);
                        if (BotCalculator.CalculateDistance(monster.position.X, monster.position.Y, pos.X, pos.Y) <= range && BotCalculator.CalculateDistance(monster.position.X, monster.position.Y, pos.X, pos.Y) >= 4)
                        {
                            point = new Point(pos.X, pos.Y);
                            break;
                        }
                    }
                }
                else
                {
                    point = new Point(box.position.X, box.position.Y);
                }
                BotMethods.MoveTo(point.X, point.Y);
                Thread.Sleep(100);
                return point;
            }
            else
            {
                BotMethods.MoveTo(monster.position);
                Thread.Sleep(100);
                return new Point(monster.position.X, monster.position.Y);
            }
        }

        public static Point MoveToFarestCorner()
        {
            Random random = new Random();
            int X = corner_1.X;
            int Y = corner_1.Y;
            int adder = random.Next(0, 6);
            switch (GetQuarter())
            {
                case 1:
                    X = corner_3.X - adder;
                    Y = corner_3.Y + adder;
                    break;
                case 2:
                    X = corner_4.X - adder;
                    Y = corner_4.Y - adder;
                    break;
                case 3:
                    X = corner_1.X + adder;
                    Y = corner_1.Y + adder;
                    break;
                case 4:
                    X = corner_2.X - adder;
                    Y = corner_2.Y - adder;
                    break;
                default:
                    X = corner_1.X + adder;
                    Y = corner_1.Y + adder;
                    break;
            }
            return new Point(X, Y);
        }

        public static Point MoveToCorner()
        {
            Random random = new Random();
            int X = 1;
            int Y = 1;
            int adder = random.Next(0, 6);
            if (Account.OnBM)
            {
                var point = MoveToFarestCorner();
                BotMethods.WriteLine("Moving to next Corner.");
                return point;
            }
            if (BotSettings.repAtIsland && userGuildTower != null && userGuildTower.guild == Account.Guild)
            {
                MoveToIsland(userGuildTower);
                BotCalculator.CalculateIslandMiddle(userGuildTower);
                BotMethods.WriteLine("Moving to Island.");
                return new Point(X, Y);
            }
            if (BotSettings.repatcorner)
            {
                switch (GetQuarter())
                {
                    case 1:
                        X = corner_1.X + adder;
                        Y = corner_1.Y + adder;
                        break;
                    case 2:
                        X = corner_2.X - adder;
                        Y = corner_2.Y - adder;
                        break;
                    case 3:
                        X = corner_4.X - adder;
                        Y = corner_4.Y - adder;
                        break;
                    case 4:
                        X = corner_3.X - adder;
                        Y = corner_3.Y + adder;
                        break;
                    default:
                        X = corner_1.X;
                        Y = corner_1.Y;
                        break;
                }
                BotMethods.WriteLine("Moving to Corner.");
                return new Point(X, Y);
            }
            if (BotSettings.repatborder)
            {
                BotMethods.WriteLine("Moving to Border.");
                return MoveToClosestBorder(GetClostesBorder());
            }
            return new Point(X, Y);
        }

        public static Point MoveToCorner(int quarter)
        {
            Random random = new Random();
            int X = 1;
            int Y = 1;
            int adder = random.Next(0, 6);
            switch (quarter)
            {
                case 1:
                    X = corner_1.X + adder;
                    Y = corner_1.Y + adder;
                    break;
                case 2:
                    X = corner_2.X - adder;
                    Y = corner_2.Y - adder;
                    break;
                case 3:
                    X = corner_4.X - adder;
                    Y = corner_4.Y - adder;
                    break;
                case 4:
                    X = corner_3.X - adder;
                    Y = corner_3.Y + adder;
                    break;
            }
            return new Point(X, Y);
        }

        public static Point MoveToClosestBorder(int quarter = 1)
        {
            var adder = GetClosestBorderCoordinates(quarter);
            var X = mapJumpUp.X;
            var Y = mapJumpUp.Y;
            switch (quarter)
            {
                case 1:
                    X = Bot.mapJumpUp.X + adder;
                    Y = Bot.mapJumpUp.Y + adder;
                    break;
                case 2:
                    X = Bot.mapJumpLeft.X - adder;
                    Y = Bot.mapJumpLeft.Y + adder;
                    break;
                case 3:
                    X = Bot.mapJumpRight.X + adder;
                    Y = Bot.mapJumpRight.Y - adder;
                    break;
                case 4:
                    X = Bot.mapJumpDown.X - adder;
                    Y = Bot.mapJumpDown.Y - adder;
                    break;
                default:
                    BotMethods.WriteLine("Unknown Map Quarter! Using Default Border.");
                    X += adder;
                    Y += adder;
                    break;
            }
            return new Point(X, Y);
        }

        public static void MoveToIsland(TowerInitMessage tower)
        {
            var Position = BotCalculator.CalculateIslandMiddle(tower);
            var random = new Random();
            BotMethods.MoveTo(Position.X + random.Next(-2, 2), Position.Y + random.Next(-2, 2));
        }

        public static int GetClosestBorderCoordinates(int quarter)
        {
            int _coords = new Random().Next(-66, 66);
            List<int> _coordsList = new List<int>();
            for (int i = -139; i < 141; i++)
            {
                _coordsList.Add(i);
            }
            switch (quarter)
            {
                case 1:
                    _coords = _coordsList.OrderBy(coords => BotCalculator.CalculateDistance(Bot.mapJumpUp.X + coords, Bot.mapJumpUp.Y + coords)).FirstOrDefault();
                    break;
                case 2:
                    _coords = _coordsList.OrderBy(coords => BotCalculator.CalculateDistance(Bot.mapJumpLeft.X - coords, Bot.mapJumpLeft.Y + coords)).FirstOrDefault();
                    break;
                case 3:
                    _coords = _coordsList.OrderBy(coords => BotCalculator.CalculateDistance(Bot.mapJumpRight.X + coords, Bot.mapJumpRight.Y - coords)).FirstOrDefault();
                    break;
                case 4:
                    _coords = _coordsList.OrderBy(coords => BotCalculator.CalculateDistance(Bot.mapJumpDown.X - coords, Bot.mapJumpDown.Y - coords)).FirstOrDefault();
                    break;
            }
            return _coords + new Random().Next(-1, 1);
        }

        public static int GetQuarter()
        {
            if (BotCalculator.CalculateDistance(corner_1.X, corner_1.Y) < 282.7)
                return 1;
            if (BotCalculator.CalculateDistance(corner_2.X, corner_2.Y) < 280.5)
                return 2;
            if (BotCalculator.CalculateDistance(corner_4.X, corner_4.Y) < BotCalculator.CalculateDistance(corner_3.X, corner_3.Y))
                return 3;

            return 4;
        }

        public static int GetQuarter(int X, int Y)
        {
            if (BotCalculator.CalculateDistance(X, Y, corner_1.X, corner_1.Y) < 282.7)
                return 1;
            if (BotCalculator.CalculateDistance(X, Y, corner_2.X, corner_2.Y) < 280.5)
                return 2;
            if (BotCalculator.CalculateDistance(X, Y, corner_4.X, corner_4.Y) < BotCalculator.CalculateDistance(corner_3.X, corner_3.Y))
                return 3;

            return 4;
        }

        public static Point GetQuarter(Point point)
        {
            if (BotCalculator.CalculateDistance(corner_1.X, corner_1.Y) < 282.7)
                return corner_1;
            if (BotCalculator.CalculateDistance(corner_2.X, corner_2.Y) < 280.5)
                return corner_2;
            if (BotCalculator.CalculateDistance(corner_4.X, corner_4.Y) < BotCalculator.CalculateDistance(corner_3.X, corner_3.Y))
                return corner_4;

            return point;
        }

        public static Point GetQuarter(bool use = true)
        {
            double distance = BotCalculator.CalculateDistance(corner_1.X, corner_1.Y);
            double closerDistance = distance;
            Point result = corner_4;
            distance = BotCalculator.CalculateDistance(corner_3.X, corner_3.Y);
            if (closerDistance < distance)
            {
                closerDistance = distance;
                result = corner_2;
            }
            distance = BotCalculator.CalculateDistance(corner_2.X, corner_2.Y);
            if (closerDistance < distance)
            {
                closerDistance = distance;
                result = corner_3;
            }
            distance = BotCalculator.CalculateDistance(corner_4.X, corner_4.Y);
            if (closerDistance < distance)
            {
                result = corner_1;
            }
            return result;
        }

        public static int GetSelectedQuarter(int mapId)
        {
            if (mapId == (Account.MapID + 4))
            {
                return 1;
            }
            if (mapId == (Account.MapID - 1))
            {
                return 2;
            }
            if (mapId == (Account.MapID + 1))
            {
                return 3;
            }
            if (mapId == (Account.MapID - 4))
            {
                return 4;
            }
            return new Random().Next(1, 4);
        }

        public static int GetClostesBorder()
        {
            var quarter = GetQuarter();
            switch (quarter)
            {
                case 1:
                    if (BotCalculator.CalculateDistance(Bot.mapJumpUp) < BotCalculator.CalculateDistance(Bot.mapJumpLeft))
                    {
                        quarter = 1;
                    }
                    else
                    {
                        quarter = 2;
                    }
                    break;
                case 2:
                    if (BotCalculator.CalculateDistance(Bot.mapJumpUp) < BotCalculator.CalculateDistance(Bot.mapJumpRight))
                    {
                        quarter = 1;
                    }
                    else
                    {
                        quarter = 3;
                    }
                    break;
                case 3:
                    if (BotCalculator.CalculateDistance(Bot.mapJumpRight) < BotCalculator.CalculateDistance(Bot.mapJumpDown))
                    {
                        quarter = 3;
                    }
                    else
                    {
                        quarter = 4;
                    }
                    break;
                case 4:
                    if (BotCalculator.CalculateDistance(Bot.mapJumpLeft) < BotCalculator.CalculateDistance(Bot.mapJumpDown))
                    {
                        quarter = 2;
                    }
                    else
                    {
                        quarter = 4;
                    }
                    break;
            }
            return quarter;
        }

        public static Point GetNextCorner()
        {
            Point result = Bot.corner_2;
            Point point = GetQuarter(corner_3);
            if (BotCalculator.CalculateDistance(corner_2) < 16)
            {
                result = Bot.corner_3;
            }
            else if (BotCalculator.CalculateDistance(corner_4) < 16)
            {
                result = Bot.corner_1;
            }
            else if (BotCalculator.CalculateDistance(corner_3) < 16)
            {
                result = Bot.corner_4;
            }
            return result;
        }

        private static Point GetSelectedBorder(int quarter)
        {
            switch (quarter)
            {
                case 1:
                    return Bot.mapJumpUp;
                case 2:
                    return Bot.mapJumpLeft;
                case 3:
                    return Bot.mapJumpRight;
                case 4:
                    return Bot.mapJumpDown;
            }
            return Bot.mapJumpUp;
        }

        public static int GetAngle(int X, int Y)
        {
            var _out = new Random().Next(1, 4);
            if (X > Account.Position.X)
            {
                _out = 1;
            }
            if (Y > Account.Position.Y)
            {
                _out = 2;
            }
            if (X < Account.Position.X)
            {
                _out = 3;
            }
            if (Y < Account.Position.Y)
            {
                _out = 4;
            }
            return _out;
        }

        private static Point BorderMovement()
        {
            var random = new Random();
            var X = mapJumpUp.X;
            var Y = mapJumpUp.Y;
            var num = random.Next(-30, 30);
            switch (quarter)
            {
                case 0:
                    X = mapJumpUp.X + num;
                    Y = mapJumpUp.Y + num - 5;
                    quarter++;
                    break;
                case 1:
                    X = mapJumpLeft.X - num + 6;
                    Y = mapJumpLeft.Y + num;
                    quarter++;
                    break;
                case 2:
                    X = mapJumpDown.X + num;
                    Y = mapJumpDown.Y - num + 4;
                    quarter++;
                    break;
                case 3:
                    X = mapJumpRight.X - num - 5;
                    Y = mapJumpRight.Y - num;
                    quarter++;
                    break;
                case 4:
                    X = moveTo_5.X + num - 2;
                    Y = moveTo_5.Y - num + 2;
                    quarter = 0;
                    break;
                default:
                    quarter = 0;
                    break;
            }
            return new Point(X, Y);
        }

        private static Point CircleMovement()
        {
            var random = new Random();
            var X = moveTo_1.X;
            var Y = moveTo_1.Y;
            var num = random.Next(2, 32);
            switch (quarter)
            {
                case 0:
                    X = moveTo_1.X + num;
                    Y = moveTo_1.Y - num;
                    quarter++;
                    break;
                case 1:
                    X = moveTo_2.X + num;
                    Y = moveTo_2.Y - num;
                    quarter++;
                    break;
                case 2:
                    X = moveTo_4.X - num;
                    Y = moveTo_4.Y + num;
                    quarter++;
                    break;
                case 3:
                    X = moveTo_3.X - num;
                    Y = moveTo_3.Y + num;
                    quarter++;
                    break;
                case 4:
                    X = moveTo_5.X + (num > 25 ? -num : num);
                    Y = moveTo_5.Y + (num > 25 ? num : -num);
                    quarter++;
                    break;
                case 5:
                    X = moveTo_1.X + num;
                    Y = moveTo_1.Y + num;
                    quarter++;
                    break;
                case 6:
                    X = moveTo_3.X - num;
                    Y = moveTo_3.Y + num;
                    quarter++;
                    break;
                case 7:
                    X = moveTo_4.X - num;
                    Y = moveTo_4.Y + num;
                    quarter++;
                    break;
                case 8:
                    X = moveTo_2.X + num;
                    Y = moveTo_2.Y - num;
                    quarter = 0;
                    break;
                default:
                    quarter = 0;
                    break;
            }
            return new Point(X, Y);
        }

        public static Point CornerMovement()
        {
            var random = new Random();
            var X = corner_1.X;
            var Y = corner_1.Y;
            var num = random.Next(2, 57);
            switch (quarter)
            {
                case 0:
                    X = corner_1.X + num;
                    Y = corner_1.Y + num;
                    quarter++;
                    break;
                case 1:
                    X = corner_4.X - num;
                    Y = corner_4.Y - num;
                    quarter++;
                    break;
                case 2:
                    X = corner_2.X - num;
                    Y = corner_2.Y - num;
                    quarter++;
                    break;
                case 3:
                    X = corner_3.X - num;
                    Y = corner_3.Y + num;
                    quarter++;
                    break;
                case 4:
                    X = moveTo_5.X + num;
                    Y = moveTo_5.Y - num;
                    quarter = 0;
                    break;
                default:
                    quarter = 0;
                    break;
            }
            return new Point(X, Y);
        }

        public static Point SideCollisionMovement()
        {
            var random = new Random();
            if (sideCollisionList.Count <= 4)
            {
                CreateSideCollisionList();
            }
            if (quarter >= sideCollisionList.Count)
            {
                quarter = 0;
            }
            var point = sideCollisionList[quarter];
            quarter++;
            return new Point(point.X++, point.Y--);
        }

        public static PositionStub MoveThread(int Seed = 0)
        {
            var point = moveTo_5;
            if (BotSettings.movementType == 0)
            {
                point = CircleMovement();
            }
            if (BotSettings.movementType == 1)
            {
                point = BorderMovement();
            }
            if (BotSettings.movementType == 2)
            {
                point = CornerMovement();
            }
            if (BotSettings.movementType == 3)
            {
                point = SideCollisionMovement();
            }
            return new PositionStub(point.X, point.Y);
        }

        public static void ActivatePowderAndPlates(bool powder, bool plates)
        {
            if (Account.Items.ContainsKey(Seafight.Constants.Items.POWDER))
            {
                var powderItem = Account.Items[Seafight.Constants.Items.POWDER];
                if (powder && !powderItem.active)
                {
                    BotMethods.Powder();
                    Thread.Sleep(new Random().Next(608, 758));
                }
                if (!powder && powderItem.active)
                {
                    BotMethods.Powder();
                    Thread.Sleep(new Random().Next(608, 758));
                }
            }
            if (Account.Items.ContainsKey(Seafight.Constants.Items.PLATES))
            {
                var platesItem = Account.Items[Seafight.Constants.Items.PLATES];
                if (plates && !platesItem.active)
                {
                    BotMethods.Plates();
                    Thread.Sleep(new Random().Next(608, 758));
                }
                if (!plates && platesItem.active)
                {
                    BotMethods.Plates();
                    Thread.Sleep(new Random().Next(608, 758));
                }
            }
        }

        public static bool IsValidMonster(MonsterInitMessage monster)
        {
            foreach (var monsterName in entitys.Keys)
            {
                string[] args = monsterName.Split('|');
                if (args[0] == monster.name && entitys[monsterName] && Client.Entitys.ContainsKey(monster.entityInfo.entityId))
                {
                    (Client.Entitys[monster.entityInfo.entityId] as MonsterInitMessage).harpoonId = BotHandlers.HarpoonHandler(args[1]);
                    {
                        return !BotSettings.onlyFullHPMonsters || monster.hitpoints == monster.maxHitpoints || monster.taggingEntity.entityId.Equals(Account.UserID) || monster.taggingEntity.entityId.Equals(0.0) ? true : false;
                    }
                }
            }
            return false;
        }

        public static bool IsValidNPC(ShipInitMessage ship)
        {
            if (ship == null)
            {
                return false;
            }
            try
            {
                foreach (var npcName in entitys.Keys)
                {
                    string[] args = npcName.Split('|');
                    if (args[0] == (ship.name) && entitys[npcName] && Client.Entitys.ContainsKey(ship.entityInfo.entityId))
                    {
                        (Client.Entitys[ship.entityInfo.entityId] as ShipInitMessage).ammoId = BotHandlers.AmmoHandler(args[1]);
                        if (args[2] == "true" || args[2] == "1")
                        {
                            (Client.Entitys[ship.entityInfo.entityId] as ShipInitMessage).usePowder = true;
                        }
                        else
                        {
                            (Client.Entitys[ship.entityInfo.entityId] as ShipInitMessage).usePowder = false;
                        }
                        if (args[3] == "true" || args[3] == "1")
                        {
                            (Client.Entitys[ship.entityInfo.entityId] as ShipInitMessage).usePlates = true;
                        }
                        else
                        {
                            (Client.Entitys[ship.entityInfo.entityId] as ShipInitMessage).usePlates = false;
                        }
                        if (args[4] == "true" || args[4] == "1")
                        {
                            (Client.Entitys[ship.entityInfo.entityId] as ShipInitMessage).useBoard = true;
                        }
                        else
                        {
                            (Client.Entitys[ship.entityInfo.entityId] as ShipInitMessage).useBoard = false;
                        }
                        return !BotSettings.onlyFullHPNpc || ship.pointsCurrent.hitpoints == ship.pointsMax.hitpoints ? true : false;
                    }
                }
            }
            catch (Exception)
            {
            }
            return false;
        }

        public static bool PlayerNearby()
        {
            bool result = false;
            var playerCopy = Client.Entitys.ToList().OfType<PlayerInitMessage>();
            int num = 0;
            for (int i = 0; i < playerCopy.Count(); i++)
            {
                var player = playerCopy.ElementAt(i);
                if (player.guild != Account.Guild)
                {
                    num++;
                }
            }
            result |= num > 0;
            return result;
        }

        public static bool PlayerWithCoordsExists(PositionStub position)
        {
            var playerCopy = Client.Entitys.ToList().OfType<PlayerInitMessage>();
            foreach (var player in playerCopy)
            {
                if (player.position.X == position.X)
                {
                    if (player.position.Y == position.Y)
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        public static PlayerInitMessage GetPlayerWithTarget(PositionStub position)
        {
            var player = new PlayerInitMessage(0.0, 0, "", "", new PositionStub(0, 0), new List<PositionStub>());
            var playerCopy = Client.Entitys.ToList().OfType<PlayerInitMessage>();
            foreach (var p in playerCopy)
            {
                if (p.route.Count > 1)
                {
                    if (p.route.Last().X == position.X)
                    {
                        if (p.route.Last().Y == position.Y)
                        {
                            player = p;
                            break;
                        }
                    }
                }
            }
            return player;
        }

        public static bool PlayerRouteContainsPosition(PositionStub position)
        {
            var playerCopy = Client.Entitys.ToList().OfType<PlayerInitMessage>().ToList();
            foreach (var player in playerCopy)
            {
                if (player.route.Exists(pos => pos.X == position.X))
                {
                    if (player.route.Exists(pos => pos.Y == position.Y))
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        public static PositionStub GetClosestBox()
        {
            var point = new PositionStub(0, 0);
            try
            {
                if (BotSettings.ignoreBoxesPlayerNearby && PlayerNearby())
                {
                    return point;
                }
                int maxDistance = 6000;
                int distance = maxDistance;
                var boxCopy = Client.Entitys.Values.ToList().OfType<BoxInitMessage>();
                foreach (var box in boxCopy)
                {
                    if (!PlayerRouteContainsPosition(box.position) && !invalidEntitys.ContainsKey(box.entityId))
                    {
                        int closerDistance = (int)BotCalculator.CalculateDistance(box.position);
                        if (closerDistance < distance && (((box.type == 17 || box.type == 19) && BotSettings.collectMeat) || (box.type == 15 && BotSettings.collecteventchests && Account.EventKeys > 0) || (box.type == 5 && BotSettings.collectchests && Account.Keys > 0) || (box.type == 20 && BotSettings.collectEventGlitter)))
                        {
                            distance = closerDistance;
                            point = box.position;
                            targetentityId = new EntityInfo(box.entityId, box.projectId);
                        }
                        else if (closerDistance < distance && (box.type == 0 || box.type == 1 || box.type == 2) && BotSettings.collectGlitters && (box.position != Account.Position))
                        {
                            distance = closerDistance;
                            point = box.position;
                            targetentityId = new EntityInfo(box.entityId, box.projectId);
                        }
                    }
                }
            }
            catch (Exception)
            {
            }
            return point;
        }

        public static BoxInitMessage GetClosestBoxInRange(Point _point, int _range)
        {
            var point = new BoxInitMessage(0.0, 0, 0, new PositionStub(0, 0), false);
            try
            {
                if (BotSettings.ignoreBoxesPlayerNearby && PlayerNearby())
                {
                    return point;
                }
                int maxDistance = 6000;
                int distance = maxDistance;
                var boxCopy = Client.Entitys.Values.ToList().OfType<BoxInitMessage>();
                foreach (var box in boxCopy)
                {
                    if (!invalidEntitys.ContainsKey(box.entityId))
                    {
                        int boxDistance = (int)BotCalculator.CalculateDistance(box.position);
                        int range = (int)BotCalculator.CalculateDistance(_point.X, _point.Y, box.position.X, box.position.Y);
                        if (boxDistance < distance && range < _range)
                        {
                            if ((box.type == 0 || box.type == 1 || box.type == 2) && BotSettings.collectGlitters && (box.position != Account.Position))
                            {
                                distance = boxDistance;
                                point = box;
                            }
                            else if ((((box.type == 17 || box.type == 19) && BotSettings.collectMeat) || (box.type == 15 && BotSettings.collecteventchests && Account.EventKeys > 0) || (box.type == 5 && BotSettings.collectchests && Account.Keys > 0) || (box.type == 20 && BotSettings.collectEventGlitter)))
                            {
                                distance = boxDistance;
                                point = box;
                            }
                        }
                    }
                }
            }
            catch (Exception)
            {
            }
            return point;
        }

        public static MonsterInitMessage GetClosestMonster()
        {
            double distance = 0;
            double closerDistance = 6000.0;
            var closestMonster = new MonsterInitMessage(0.0, 0, -1, 1, 1, new PositionStub(0, 0));
            var monsterCopy = Client.Entitys.Values.ToList().OfType<MonsterInitMessage>();
            try
            {
                foreach (var monster in monsterCopy)
                {
                    if (IsValidMonster(monster))
                    {
                        distance = BotCalculator.CalculateDistance(monster.position);
                        if (entitysAmount.ContainsKey(monster.name))
                        {
                            if (entitysAmount[monster.name] > 0)
                            {
                                if (distance < closerDistance)
                                {
                                    closerDistance = distance;
                                    closestMonster = monster;
                                }
                            }
                        }
                        else
                        {
                            if (distance < closerDistance)
                            {
                                closerDistance = distance;
                                closestMonster = monster;
                            }
                        }
                    }
                }
            }
            catch (Exception)
            {
            }
            return closestMonster;
        }

        public static ShipInitMessage GetClosestNPC()
        {
            double distance = 0;
            double closerDistance = 6000.0;
            var closestShip = new ShipInitMessage(0.0, 0, -1, 1, 1, new PositionStub(0, 0), new List<PositionStub>());
            var shipCopy = Client.Entitys.Values.ToList().OfType<ShipInitMessage>();
            try
            {
                foreach (var ship in shipCopy)
                {
                    if (IsValidNPC(ship))
                    {
                        distance = BotCalculator.CalculateDistance(ship.position);
                        if (entitysAmount.ContainsKey(ship.name))
                        {
                            if (entitysAmount[ship.name] > 0)
                            {
                                if (distance < closerDistance)
                                {
                                    closerDistance = distance;
                                    closestShip = ship;
                                }
                            }
                        }
                        else
                        {
                            if (distance < closerDistance)
                            {
                                closerDistance = distance;
                                closestShip = ship;
                            }
                        }
                    }
                }
            }
            catch (Exception)
            {
            }
            return closestShip;
        }

        public static ShipInitMessage GetClosestShip()
        {
            var distance = 0.0;
            var closerDistance = 6000.0;
            var closestShip = new ShipInitMessage(0.0, 0, -1, 1, 1, new PositionStub(0, 0), new List<PositionStub>());
            var shipCopy = Client.Entitys.Values.ToList().OfType<ShipInitMessage>();
            try
            {
                foreach (var ship in shipCopy)
                {
                    distance = BotCalculator.CalculateDistance(ship.position);
                    if (distance < closerDistance)
                    {
                        closerDistance = distance;
                        closestShip = ship;
                        targetentityId = new EntityInfo(ship.entityInfo.entityId, ship.entityInfo.projectId);
                    }
                }
            }
            catch (Exception)
            {
            }
            return closestShip;
        }

        public static ShipInitMessage GetClosestShipRaid()
        {
            double distance = 0;
            double closerDistance = 6000.0;
            var closestShip = new ShipInitMessage(0.0, 0, -1, 1, 1, new PositionStub(0, 0), new List<PositionStub>());
            var shipCopy = Client.Entitys.Values.ToList().OfType<ShipInitMessage>();
            try
            {
                foreach (var ship in shipCopy)
                {
                    if (BotLogic.IsValidRaidNPC(ship))
                    {
                        distance = BotCalculator.CalculateDistance(ship.position);
                        if (distance < closerDistance)
                        {
                            closerDistance = distance;
                            closestShip = ship;
                        }
                    }
                }
            }
            catch (Exception)
            {
            }
            return closestShip;
        }

        public static void NextQuest()
        {
            try
            {
                if (BotSession.currentQuest.Length < 2 && BotSettings.useQuestSystem)
                {
                    foreach (var quest in quests.Keys)
                    {
                        if (quests[quest])
                        {
                            var questId = Bot.Quests[quest];
                            BotMethods.AcceptQuest(questId);
                            BotMethods.WriteLine("Starting next Quest [" + quest + "]");
                            BotSession.currentQuest = quest;
                            break;
                        }
                    }
                }
            }
            catch (Exception)
            {
            }
        }

        public static void UpdateQuest()
        {
            try
            {
                if (currentQuest.type == 10)
                {
                    currentQuest.position = new PositionStub(int.Parse(Bot.currentQuest.values[0]), int.Parse(Bot.currentQuest.values[1]));
                }
                if (currentQuest.type == 11)
                {
                    var currentAmount = int.Parse(currentQuest.values[0]);
                    var maxAmount = int.Parse(currentQuest.values[1]);
                    var nameIds = currentQuest.values[2].Split(',');
                    foreach (var nameId in nameIds)
                    {
                        if (Bot.NPCs.ContainsKey(int.Parse(nameId)))
                        {
                            var npc = Bot.NPCs[int.Parse(nameId)];
                            var flag = false;
                            foreach (var npcKey in Bot.entitys.Keys)
                            {
                                if (npcKey.Contains(npc))
                                {
                                    flag = true;
                                    break;
                                }
                            }
                            if (!flag)
                            {
                                Bot.entitys.Add(npc + "|Hollowballs (20)|0|0|0", true);
                            }
                            else
                            {
                                var key = Bot.entitys.Keys.FirstOrDefault(n => n.Contains(npc));
                                Bot.entitys[key] = true;
                            }
                            if (!Bot.entitysAmount.ContainsKey(npc))
                            {
                                Bot.entitysAmount.Add(npc, maxAmount);
                            }
                        }
                    }
                }
                if (currentQuest.type == 12)
                {
                    var currentAmount = int.Parse(currentQuest.values[0]);
                    var maxAmount = int.Parse(currentQuest.values[1]);
                    var nameId = int.Parse(currentQuest.values[2]);
                    if (Bot.Monsters.ContainsKey(nameId))
                    {
                        var monster = Bot.Monsters[nameId];
                        var flag = false;
                        foreach (var monsterKey in Bot.entitys.Keys)
                        {
                            if (monsterKey.Contains(monster))
                            {
                                flag = true;
                                break;
                            }
                        }
                        if (!flag)
                        {
                            Bot.entitys.Add(monster + "|Bronze Harpoons (50)", true);
                        }
                        else
                        {
                            var key = Bot.entitys.Keys.FirstOrDefault(m => m.Contains(monster));
                            Bot.entitys[key] = true;
                        }
                        if (!Bot.entitysAmount.ContainsKey(monster))
                        {
                            Bot.entitysAmount.Add(monster, maxAmount);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                BotMethods.WriteLine("There was an error while updating quests!\n\n" + ex);
            }
        }

        public static bool DoQuest()
        {
            try
            {
                if (currentQuest.type == 10)
                {
                    var position = new PositionStub(0, 0);
                    var x = 0;
                    var y = 0;
                    var counter = 60;
                    var retrys = 0;
                    while (!Account.IsSunk && Bot.Running && currentQuest != null)
                    {
                        position = currentQuest.position;
                        if ((position == null || position.X == 0) && retrys < 7)
                        {
                            UpdateQuest();
                            retrys++;
                        }
                        if (retrys >= 7)
                        {
                            break;
                        }
                        if (BotCalculator.CalculateDistance(position) > 0 && (Account.Route.Count <= 0 || !Account.Route.Exists(pos => pos.X == position.X && pos.Y == position.Y)))
                        {
                            BotMethods.WriteLine($"Moving to next Quest-position [{currentQuest.id}].");
                            BotMethods.MoveTo(position);
                            Thread.Sleep(300);
                        }
                        while (Account.Route.Count > 0 && Account.Position.X != position.X && Account.Position.Y != position.Y && !Account.IsSunk && Bot.Running && counter > 0)
                        {
                            if (x != Account.Position.X || y != Account.Position.Y)
                            {
                                x = Account.Position.X;
                                y = Account.Position.Y;
                            }
                            if (Account.TargetX != position.X || Account.TargetY != position.Y)
                            {
                                BotMethods.MoveTo(position);
                                Thread.Sleep(600);
                            }
                            if (Account.IsRepMaatLevel5 && Account.GetCurrentHpPercent < 100 && !Account.Repairing)
                            {
                                BotMethods.Repair();
                                Thread.Sleep(500);
                            }
                            Thread.Sleep(1000);
                            counter--;
                        }
                        Thread.Sleep(1000);
                    }
                    return true;
                }
                if (currentQuest.type == 11)
                {
                    if (!BotSettings.shootMonsters)
                    {
                        BotSettings.shootMonsters = true;
                    }
                    return false;
                }
                if (currentQuest.type == 12)
                {
                    if (!BotSettings.shootNPCs)
                    {
                        BotSettings.shootNPCs = true;
                    }
                    return false;
                }
                if (BotSettings.doDailyQuest && BotSession.currentQuest.Length < 2 && BotSession.Sessioncandodailyquest)
                {
                    if (MoveToSafeHeaven())
                    {
                        Thread.Sleep(500);
                        BotMethods.MoveTo(183 + new Random().Next(-2, 1), 71);
                        Thread.Sleep(2000);
                        while (Account.Route.Count > 1)
                        {
                            Thread.Sleep(1000);
                        }
                        Thread.Sleep(100);
                        BotMethods.AcceptQuest(392);
                        BotSession.currentQuest = "Daily Quest";
                        BotMethods.WriteLine("Starting " + BotSession.currentQuest + ".");
                        BotSession.Sessioncandodailyquest = false;
                    }
                    else
                    {
                        Bot.MoveToSafeHeaven();
                        Thread.Sleep(100);
                        Bot.NextQuest();
                    }
                    Thread.Sleep(1000);
                    BotMethods.Revive();
                    Thread.Sleep(18000);
                }
                if (BotSettings.doDailyQuest && BotSession.currentQuest.Contains("Daily") && Account.MapID < 53)
                {
                    Thread.Sleep(100);
                    Bot.JumpMapThread(53);
                    Thread.Sleep(500);
                    if (Bot.NPCs.ContainsKey(1572) && !Bot.entitys.Keys.Contains(Bot.NPCs[1572]))
                    {
                        Bot.entitys.Add(Bot.NPCs[1572] + "|Explosiveballs (75)|1|1|1", true);
                    }
                    if (Bot.NPCs.ContainsKey(1575) && !Bot.entitys.Keys.Contains(Bot.NPCs[1575]))
                    {
                        Bot.entitys.Add(Bot.NPCs[1575] + "|Explosiveballs (75)|1|1|1", true);
                    }
                    if (Bot.NPCs.ContainsKey(1572) && !Bot.entitysAmount.ContainsKey(Bot.NPCs[1572]))
                    {
                        Bot.entitysAmount.Add(Bot.NPCs[1572], 10);
                    }
                    if (Bot.NPCs.ContainsKey(1575) && !Bot.entitysAmount.ContainsKey(Bot.NPCs[1575]))
                    {
                        Bot.entitysAmount.Add(Bot.NPCs[1575], 10);
                    }
                    Thread.Sleep(100);
                    return true;
                }
            }
            catch (Exception ex)
            {
                BotMethods.WriteLine("There was an error while completing the current Quest!\n\n" + ex);
            }
            return false;
        }

        public static bool MoveToSafeHeaven()
        {
            if (Account.MapID == 505)
            {
                return true;
            }
            if (Account.MapID != 49)
            {
                JoinMapThread("Teleport", 49);
                Thread.Sleep(1500);
            }
            if (Account.MapID == 49)
            {
                var point = Bot.MoveToClosestBorder(2);
                BotMethods.MoveTo(point.X, point.Y);
                Thread.Sleep(2500);
                while (!Account.IsSunk && Account.Route.Count >= 2 && Bot.Running)
                {
                    if (Account.TargetX != point.X || Account.TargetY != point.Y)
                    {
                        point = Bot.MoveToClosestBorder(2);
                        BotMethods.MoveTo(point.X, point.Y);
                        Thread.Sleep(1000);
                    }
                    if (Account.Position.X == point.X && Account.Position.Y == point.Y)
                    {
                        Thread.Sleep(1250);
                        break;
                    }
                    if (BotCalculator.CalculateDistance(point) <= 15 && Account.Repairing)
                    {
                        BotMethods.StopRepair();
                        Thread.Sleep(500);
                    }
                    Thread.Sleep(1000);
                }
                if (Account.IsSunk)
                {
                    BotSettings.needMapSwitch = false;
                    return false;
                }
                Thread.Sleep(2000);
                BotMethods.JoinSafeHeaven(505);
                var _counter = 16;
                while (_counter > 0 && !Account.IsSunk && Bot.Running)
                {
                    Thread.Sleep(1000);
                    _counter--;
                }
                if (Account.MapID == 505)
                {
                    return true;
                }
            }
            return false;
        }

        public static bool JumpMapThread(int mapId)
        {
            var _counter = 2;
            try
            {
                while (_counter > 0 && Bot.Running)
                {
                    var random = new Random();
                    var mapQuarter = GetSelectedQuarter(mapId);
                    if (!help.LegitJump(Account.Level, mapId))
                    {
                        mapId += 4;
                    }
                    var point = Bot.MoveToClosestBorder(mapQuarter);
                    BotMethods.MoveTo(point.X, point.Y);
                    Thread.Sleep(3000);
                    while (!Account.IsSunk && Account.Route.Count >= 2 && Bot.Running)
                    {
                        if (Account.TargetX != point.X || Account.TargetY != point.Y)
                        {
                            point = Bot.MoveToClosestBorder(mapQuarter);
                            BotMethods.MoveTo(point.X, point.Y);
                            Thread.Sleep(1000);
                        }
                        if (Account.Position.X == point.X && Account.Position.Y == point.Y)
                        {
                            Thread.Sleep(1250);
                            break;
                        }
                        if (BotCalculator.CalculateDistance(point) <= 11 && Account.Repairing)
                        {
                            BotMethods.StopRepair();
                            Thread.Sleep(random.Next(100, 500));
                        }
                        Thread.Sleep(1000);
                    }
                    if (Account.IsSunk)
                    {
                        BotSettings.needMapSwitch = false;
                        return false;
                    }
                    Thread.Sleep(random.Next(3000, 4000));
                    if (Account.MapID != mapId && BotCalculator.CalculateDistance(point.X, point.Y) < 4)
                    {
                        Thread.Sleep(1000);
                        if (Account.MapID != mapId && BotCalculator.CalculateDistance(point.X, point.Y) < 4)
                        {
                            BotMethods.WriteLine("Jumping to next Map [" + BotHandlers.maps[mapId] + "].");
                            BotMethods.JumpMap(mapId);
                            BotSettings.needMapSwitch = false;
                            return false;
                        }
                        BotMethods.WriteLine("There was an error while reading MapChangeRequest packets!");
                        return false;
                    }
                    if (!BotSettings.needMapSwitch)
                    {
                        return true;
                    }
                    _counter--;
                    Thread.Sleep(1000);
                }
            }
            catch (Exception)
            {
                BotSettings.needMapSwitch = false;
            }
            Thread.Sleep(2000);
            return false;
        }

        public static bool JumpMapCheck()
        {
            if (BotSettings.jumpMaps && BotSettings.needMapSwitch && !BotSettings.jumpMapIfAvailable)
            {
                if (BotSettings.jumpMapRandom)
                {
                    BotMethods.WriteLine("Going to random Map.");
                    return JumpMapThread(new Random().Next(-4, 4));
                }
                else
                {
                    if (BotSettings.jumpMapLeftRight)
                    {
                        BotMethods.WriteLine("Going to next Map.");
                        var nextMapID = Account.MapID;
                        if (help.IsOdd(nextMapID))
                        {
                            nextMapID += 1;
                        }
                        else
                        {
                            nextMapID -= 1;
                        }
                        return JumpMapThread(nextMapID);
                    }
                    else
                    {
                        BotMethods.WriteLine("No jump Map Action selected! Skipping.");
                        BotSettings.jumpMaps = false;
                    }
                }
                BotSettings.needMapSwitch = false;
            }
            return false;
        }

        public static void JoinMapThread(string join = "Join Raid", int MapID = 0)
        {
            try
            {
                Thread.Sleep(new Random().Next(500, 1500));
                BotMethods.WriteLine("Moving to corner before " + (join != "Join Raid" ? MapID == 0 ? "leaving Map." : join != "Teleport" ? "joining Bonusmap." : "Teleporting." : "joining Raid."));
                Point point = MoveToCorner(GetQuarter());
                BotMethods.MoveTo(point.X, point.Y);
                Thread.Sleep(3500);
                int counter = 0;
                int x = 0;
                int y = 0;
                while (Account.Route.Count >= 2 && counter > 0 && Running && !Account.IsSunk)
                {
                    if (x != Account.Position.X || y != Account.Position.Y)
                    {
                        x = Account.Position.X;
                        y = Account.Position.Y;
                        counter = 3;
                    }
                    if (x == point.X && y == point.Y)
                    {
                        Thread.Sleep(200);
                        break;
                    }
                    Thread.Sleep(1000);
                    counter--;
                }
                if (Account.IsSunk)
                {
                    return;
                }
                counter = 0;
                switch (join)
                {
                    case "Join Raid":
                        if (BotSettings.maxRaidRejoins > 0)
                        {
                            if (BotSession.Sessionraidrejoins > BotSettings.maxRaidRejoins)
                            {
                                BotMethods.WriteLine("Max Raid Rejoin Limit Reached! Going back to normal Tasks!");
                                Account.Joining = false;
                                BotSettings.RaidType = 0;
                                BotSettings.autoJoinRaid = false;
                                return;
                            }
                        }
                        if (BotSettings.rebuyRaidMedallions && (!Account.Items.ContainsKey(Account.RaidMedallion) || Account.Items[Account.RaidMedallion].amount <= 1))
                        {
                            RebuyRaidMedallions();
                            Thread.Sleep(1250);
                        }
                        BotSession.Sessionraidrejoins++;
                        BotMethods.ActivateItem(Account.RaidMedallion);
                        break;
                    case "Join BM":
                        BotMethods.JoinBonusMap(MapID);
                        break;
                    case "Leave Kraken":
                        BotMethods.Revive();
                        BotMethods.WriteLine("Leaving Map.");
                        Account.Joining = true;
                        break;
                    case "Teleport":
                        BotMethods.Teleport(MapID);
                        break;
                    default:
                        BotMethods.WriteLine("Error in Join Map Thread!\n\nUnknown Map Type! #" + join + "\n" + MapID);
                        return;
                }
                Thread.Sleep(500);
                while (counter < 16)
                {
                    counter++;
                    Thread.Sleep(1000);
                    if (!Account.Joining)
                    {
                        break;
                    }
                }
            }
            catch (Exception)
            {
            }
            Account.Joining = false;
        }

        public static Dictionary<int, string> LoadNPCNames()
        {
            var _out = new Dictionary<int, string>();
            try
            {
                string resource = help.CreateWebClientRequest("http://www.seafight.com/client/lang/npcnames.php?lang=" + BotSession.language);
                string[] splitter = { "<item name=\"" };
                string[] args = resource.Split(splitter, StringSplitOptions.None);
                for (int i = 0; i < args.Length; i++)
                {
                    if (args[i].Contains("<![CDATA["))
                    {
                        string npc = help.Between(args[i], "<![CDATA[", "]]>");
                        string id = args[i].ElementAt(0) + help.Between(args[i], args[i].ElementAt(0).ToString(), "\">");
                        id = Regex.Replace(id, "[^0-9.]", "");
                        if (int.Parse(id) == 1147 || int.Parse(id) == 1148)
                        {
                            if (int.Parse(id) == 1147)
                            {
                                BotSession.krakenTentacleName = npc;
                            }
                            else
                            {
                                BotSession.krakenName = npc;
                            }
                        }
                        if (!_out.ContainsKey(int.Parse(id)))
                            _out.Add(int.Parse(id), npc);
                    }
                }
            }
            catch (Exception)
            {
                BotMethods.WriteLine("There was an Error while loading NPC-names.");
            }
            return _out;
        }

        public static Dictionary<int, string> LoadMonsterNames()
        {
            var _out = new Dictionary<int, string>();
            try
            {
                string resource = help.CreateWebClientRequest("https://www.seafight.com/client/lang/labels.lang.php?lang=" + BotSession.language);
                resource = help.Between(resource, "<category name=\"seafight.labels.Monsters\">", "</category>");
                string[] splitter = { "<item name=\"" };
                string[] args = resource.Split(splitter, StringSplitOptions.None);
                for (int i = 0; i < args.Length; i++)
                {
                    if (args[i].Contains("<![CDATA[") && !args[i].Contains("groupname"))
                    {
                        string npc = help.Between(args[i], "<![CDATA[", "]]>");
                        string id = args[i].ElementAt(0) + help.Between(args[i], args[i].ElementAt(0).ToString(), "\">");
                        id = Regex.Replace(id, "[^0-9.]", "");
                        if (!_out.ContainsKey(int.Parse(id)))
                            _out.Add(int.Parse(id), npc);
                    }
                }
            }
            catch (Exception)
            {
                BotMethods.WriteLine("There was an Error while loading Monster-names.");
            }
            return _out;
        }

        public static Dictionary<string, int> LoadDesignNames()
        {
            Dictionary<string, int> designs = new Dictionary<string, int>();
            try
            {
                string resource = help.CreateWebClientRequest("https://www.seafight.com/client/lang/labels.lang.php?lang=" + BotSession.language);
                resource = help.Between(resource, "<category name=\"seafight.labels.ShipDesigns\">", "</category>");
                string[] splitter = { "<item name=\"" };
                string[] args = resource.Split(splitter, StringSplitOptions.None);
                for (int i = 0; i < args.Length; i++)
                {
                    if (args[i].Contains("<![CDATA["))
                    {
                        string npc = help.Between(args[i], "<![CDATA[", "]]>");
                        string id = args[i].ElementAt(0) + help.Between(args[i], args[i].ElementAt(0).ToString(), "\">");
                        id = Regex.Replace(id, "[^0-9.]", "");
                        if (!designs.ContainsKey(npc))
                            designs.Add(npc, help.ToInt(id));
                    }
                }
            }
            catch (Exception)
            {
                BotMethods.WriteLine("There was an Error while loading Design-names.");
            }
            return designs;
        }

        public static Dictionary<string, int> LoadItemNames()
        {
            var _out = new Dictionary<string, int>();
            try
            {
                string resource = help.CreateWebClientRequest("https://www.seafight.com/client/lang/labels.lang.php?lang=" + BotSession.language);
                resource = help.Between(resource, "<category name=\"seafight.labels.ActionItems\">", "</category>");
                string[] splitter = { "<item name=\"" };
                string[] args = resource.Split(splitter, StringSplitOptions.None);
                for (int i = 0; i < args.Length; i++)
                {
                    if (args[i].Contains("<![CDATA[") && !args[i].Contains("groupname"))
                    {
                        string npc = help.Between(args[i], "<![CDATA[", "]]>");
                        string id = args[i].ElementAt(0) + help.Between(args[i], args[i].ElementAt(0).ToString(), "\">");
                        id = Regex.Replace(id, "[^0-9.]", "");
                        int ID = int.Parse(id);
                        if (!_out.ContainsKey(npc))
                            _out.Add(npc, ID);
                    }
                }
            }
            catch (Exception)
            {
                BotMethods.WriteLine("There was an Error while loading Item-names.");
            }
            return _out;
        }

        public static Dictionary<string, int> LoadQuests()
        {
            var _out = new Dictionary<string, int>();
            try
            {
                string resource = help.CreateWebClientRequest("https://" + BotSession.Server + ".seafight.bigpoint.com/client/lang/seafight.quests.php?lang=" + BotSession.language);
                resource = help.Between(resource, "<quest>", "</quest>");
                string[] splitter = { "<category name=\"" };
                string[] args = resource.Split(splitter, StringSplitOptions.None);
                for (int i = 0; i < args.Length; i++)
                {
                    if (args[i].Contains("Quest"))
                    {
                        string name = help.Between(args[i], "<item name=\"Headline\">", "</item>");
                        name = help.Between(name, "<![CDATA[", "]]>");
                        string id = help.Between(args[i], "Quest", "\">");
                        id = Regex.Replace(id, "[^0-9.]", "");
                        int ID = int.Parse(id);
                        if (!_out.ContainsKey(name) && name.Length > 2)
                        {
                            _out.Add(name, ID);
                        }
                    }
                }
            }
            catch (Exception)
            {
                BotMethods.WriteLine("There was an Error while loading Quests.");
            }
            return _out;
        }

        public static void LoadNames()
        {
            NPCs = Bot.LoadNPCNames();
            Monsters = Bot.LoadMonsterNames();
            Designs = Bot.LoadDesignNames();
            Items = Bot.LoadItemNames();
            Quests = Bot.LoadQuests();
            Form1.form1.LoadItemList(Bot.Items);
            Form1.form1.LoadDesignList(Bot.Designs);
            Form1.form1.LoadQuestList(Bot.Quests);
            BotMethods.WriteLine("Names loaded!");
        }
    }
}
