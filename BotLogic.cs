using BoxyBot.Seafight;
using BoxyBot.Seafight.Constants;
using BoxyBot.Seafight.Messages;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading;

namespace BoxyBot
{
    public static class BotLogic
    {
        public static int attacking = -1;
        public static bool OnAttackRunning { get; set; } = false;
        public static Random random = new Random();
        private static Thread onAttackThread;
        public static bool LogicAlive
        {
            get
            {
                return (Bot._botThread != null && Bot._botThread.IsAlive);
            }
        }

        #region onAttacked

        public static void StartRepairThread()
        {
            if (BotLogic.onAttackThread != null)
            {
                BotLogic.OnAttackRunning = false;
                BotLogic.onAttackThread.Abort();
                BotLogic.onAttackThread.Join();
            }
            BotLogic.OnAttackRunning = true;
            BotLogic.onAttackThread = new Thread(new ThreadStart(RepairThread));
            BotLogic.onAttackThread.Start();
        }

        public static void StartAvoidEnemyThread()
        {
            if (BotLogic.onAttackThread != null)
            {
                BotLogic.OnAttackRunning = false;
                BotLogic.onAttackThread.Abort();
                BotLogic.onAttackThread.Join();
            }
            BotLogic.OnAttackRunning = true;
            BotLogic.onAttackThread = new Thread(new ThreadStart(FleeIfEnemyNearbyThread));
            BotLogic.onAttackThread.Start();
        }

        public static void StartonAttackThread()
        {
            if (!Client.Entitys.ContainsKey(Bot.underAttackBy.entityId) || (!(Client.Entitys[Bot.underAttackBy.entityId] is TowerInitMessage) && !(Client.Entitys[Bot.underAttackBy.entityId] is PlayerInitMessage) && ((Client.Entitys[Bot.underAttackBy.entityId] is ShipInitMessage && !Account.OnBM))))
            {
                return;
            }
            if (BotLogic.onAttackThread != null)
            {
                BotLogic.OnAttackRunning = false;
                BotLogic.onAttackThread.Abort();
                BotLogic.onAttackThread.Join();
            }
            if ((Account.OnBM || (Account.MapID >= 19 && Account.MapID != 301)) && Client.Entitys[Bot.underAttackBy.entityId] is ShipInitMessage && Account.HP < Account.MaxHP)
            {
                BotLogic.OnAttackRunning = true;
                BotLogic.onAttackThread = new Thread(new ThreadStart(RepairThread));
                BotLogic.onAttackThread.Start();
                return;
            }
            bool flag = (Client.Entitys[Bot.underAttackBy.entityId] is TowerInitMessage);
            if (flag)
            {
                BotLogic.onAttackThread = new Thread(new ThreadStart(FleeFromIslandThread));
            }
            else
            {
                if (Client.Entitys[Bot.underAttackBy.entityId] is PlayerInitMessage)
                {
                    if (BotSettings.ShootBackType == 1)
                    {
                        BotLogic.onAttackThread = new Thread(new ThreadStart(FleeThread));
                    }
                    else if (BotSettings.ShootBackType == 2)
                    {
                        BotLogic.onAttackThread = new Thread(new ThreadStart(ShootBackThread));
                    }
                    else return;
                }
                else return;
            }
            BotLogic.OnAttackRunning = true;
            BotLogic.onAttackThread.Start();
        }

        public static void StoponAttackThrad()
        {
            BotLogic.OnAttackRunning = false;
            if (BotLogic.onAttackThread != null)
            {
                BotLogic.onAttackThread.Abort();
            }
        }

        public static void RepairThread()
        {
            BotLogic.StopBotThread();
            Bot.Running = false;
            BotLogic.OnAttackRunning = true;
            try
            {
                BotMethods.WriteLine("Going to repair.");
                var point = default(Point);
                if (Account.OnBM)
                {
                    point = Bot.MoveToFarestCorner();
                }
                else
                {
                    var _point = new Point(Account.Position.X, Account.Position.Y);
                    if (BotCalculator.CalculateDistance(_point.X, _point.Y, Bot.corner_1.X, Bot.corner_1.Y) < 10 || BotCalculator.CalculateDistance(_point.X, _point.Y, Bot.corner_2.X, Bot.corner_2.Y) < 10 || BotCalculator.CalculateDistance(_point.X, _point.Y, Bot.corner_3.X, Bot.corner_3.Y) < 10 || BotCalculator.CalculateDistance(_point.X, _point.Y, Bot.corner_4.X, Bot.corner_4.Y) < 10)
                    {
                        point = Bot.GetNextCorner();
                        BotMethods.MoveTo(point.X, point.Y);
                    }
                    else
                    {
                        if (BotSettings.repatcorner)
                        {
                            point = Bot.MoveToCorner();
                        }
                        if (BotSettings.repatborder)
                        {
                            point = Bot.MoveToClosestBorder();
                        }
                        if (BotSettings.repAtIsland && Bot.userGuildTower != null && Bot.userGuildTower.entityInfo != null)
                        {
                            var p = BotCalculator.CalculateIslandMiddle(Bot.userGuildTower);
                            point = new Point(p.X, p.Y);
                        }
                        if (Account.MapID == 302 && BotLogic.RaidBossExists())
                        {
                            point = Bot.MoveToCorner();
                        }
                    }
                }
                BotMethods.MoveTo(point.X, point.Y);
                Thread.Sleep(random.Next(100, 330));
                if (BotLogic.attacking > 2 && random.Next(0, 2) == 1)
                {
                    BotMethods.StopAttack();
                }
                int num = 3;
                int x = 0;
                int y = 0;
                Thread.Sleep(2000);
                try
                {
                    while (true)
                    {
                        if (Account.Position.X == point.X && Account.Position.Y == point.Y)
                        {
                            Thread.Sleep(850);
                            if (BotSettings.useDesignChanger && Account.DesignId != BotSettings.repDesign)
                            {
                                BotMethods.ChangeDesign(BotSettings.repDesign);
                            }
                            break;
                        }
                        if (!Account.IsSunk && !Account.Repairing)
                        {
                            Thread.Sleep(1000);
                            if (x != Account.Position.X || y != Account.Position.Y)
                            {
                                x = Account.Position.X;
                                y = Account.Position.Y;
                                num = 3;
                            }
                            num--;
                            if (num <= 0)
                            {
                                point = Bot.GetNextCorner();
                                BotMethods.MoveTo(point.X, point.Y);
                                Thread.Sleep(500);
                            }
                            attacking--;
                            if (Account.Poisioned)
                            {
                                Thread.Sleep(7500);
                            }
                            if (!Account.IsRepMaatLevel5 || Account.Repairing || BotLogic.attacking >= 13)
                            {
                                continue;
                            }
                            if (Account.HP < Account.MaxHP)
                            {
                                BotMethods.Repair();
                                Thread.Sleep(500);
                                continue;
                            }
                            break;
                        }
                        Thread.Sleep(1000);
                    }
                    if (!Account.Repairing)
                    {
                        BotMethods.Repair();
                    }
                    var oldHp = Account.HP;
                    var _counter = 0;
                    while (true)
                    {
                        Thread.Sleep(1000);
                        if (!Account.Repairing)
                        {
                            break;
                        }
                        if (_counter >= 4)
                        {
                            Thread.Sleep(500);
                            if (Account.HP == oldHp)
                            {
                                break;
                            }
                            oldHp = Account.HP;
                            _counter = 0;
                        }
                        _counter++;
                    }
                }
                catch (Exception)
                {
                }
                if (Account.OnBM || Account.OnRaid && Account.HP < 99)
                {
                    RepairFunction(100);
                }
            }
            catch (Exception ex)
            {
                BotMethods.WriteLine("There was an error while running repair function.\n\n" + ex.Message);
            }
            BotLogic.OnAttackRunning = false;
            Bot.underAttackBy = new EntityInfo(0.0, 0);
            Thread.Sleep(2000);
            if (Account.OnBM && Account.GetCurrentHpPercent <= BotSettings.repathpbm)
            {
                BotLogic.RepairThread();
            }
            if (BotSettings.useDesignChanger && Account.DesignId != BotSettings.botDesign && Account.Route.Count <= 1)
            {
                Thread.Sleep(random.Next(150, 450));
                BotMethods.ChangeDesign(BotSettings.botDesign);
            }
            BotLogic.StartBotThread();
        }

        private static void RepairFunction(int hpPercentage)
        {
            if (Account.GetCurrentHpPercent <= hpPercentage && !Account.IsSunk)
            {
                Thread.Sleep(random.Next(250, 550));
                if (Account.Repairing)
                {
                    BotMethods.StopRepair();
                    Thread.Sleep(random.Next(1000, 1850));
                }
                var point = default(Point);
                var _point = new Point(Account.Position.X, Account.Position.Y);
                if (BotCalculator.CalculateDistance(_point.X, _point.Y, Bot.corner_1.X, Bot.corner_1.Y) < 10 || BotCalculator.CalculateDistance(_point.X, _point.Y, Bot.corner_2.X, Bot.corner_2.Y) < 10 || BotCalculator.CalculateDistance(_point.X, _point.Y, Bot.corner_3.X, Bot.corner_3.Y) < 10 || BotCalculator.CalculateDistance(_point.X, _point.Y, Bot.corner_4.X, Bot.corner_4.Y) < 10)
                {
                    point = Bot.GetNextCorner();
                }
                else
                {
                    if (BotSettings.repatcorner)
                    {
                        point = Bot.MoveToCorner();
                    }
                    if (BotSettings.repatborder)
                    {
                        if (BotCalculator.CalculateDistance(_point.X, _point.Y, Bot.mapJumpUp.X, Bot.mapJumpUp.Y) < 10 || BotCalculator.CalculateDistance(_point.X, _point.Y, Bot.mapJumpRight.X, Bot.mapJumpRight.Y) < 10 || BotCalculator.CalculateDistance(_point.X, _point.Y, Bot.mapJumpLeft.X, Bot.mapJumpLeft.Y) < 10 || BotCalculator.CalculateDistance(_point.X, _point.Y, Bot.mapJumpDown.X, Bot.mapJumpDown.Y) < 10)
                        {
                            var _border = Bot.GetClostesBorder();
                            if (_border < 4)
                            {
                                _border++;
                            }
                            else
                            {
                                _border--;
                            }
                            point = Bot.MoveToClosestBorder(_border);
                        }
                        else
                        {
                            point = Bot.MoveToClosestBorder(Bot.GetClostesBorder());
                        }
                    }
                    if (!BotSettings.repatborder && !BotSettings.repatcorner && Account.MapID >= 19)
                    {
                        if (Account.MapID < 45)
                        {
                            if (BotCalculator.CalculateDistance(Bot.GetClosestShip().position) < 24)
                            {
                                point = Bot.MoveToCorner();
                            }
                        }
                        else if (Account.MapID <= 52)
                        {
                            if (BotCalculator.CalculateDistance(Bot.GetClosestShip().position) < 42)
                            {
                                point = Bot.MoveToCorner();
                            }
                        }
                        else if (Account.MapID == 300)
                        {
                            if (BotCalculator.CalculateDistance(Bot.GetClosestShipRaid().position) < 45)
                            {
                                point = Bot.MoveToCorner();
                            }
                        }
                    }
                    if (Account.OnBM)
                    {
                        point = Bot.MoveToFarestCorner();
                    }
                    if (Account.MapID == 302 && RaidBossExists())
                    {
                        point = Bot.MoveToCorner();
                    }
                }
                BotMethods.MoveTo(point.X, point.Y);
                Thread.Sleep(random.Next(100, 300));
                if (BotLogic.attacking > 2 && random.Next(0, 2) == 0)
                {
                    BotMethods.StopAttack();
                }
                Thread.Sleep(2000);
                var _counter = 61;
                var _count = Account.Route.Count / 1.4;
                while (Account.Route.Count >= 4 && !Account.IsSunk && _counter > 0)
                {
                    if (Account.Route.Count < _count && Account.IsRepMaatLevel5 && !Account.Poisioned && Account.MapID != 302)
                    {
                        Thread.Sleep(random.Next(1000, 2750));
                        break;
                    }
                    if (Account.Position.X == point.X && Account.Position.Y == point.Y)
                    {
                        Thread.Sleep(250);
                        break;
                    }
                    if (Account.Route.Count <= 2 || _count <= 5)
                    {
                        Thread.Sleep(600);
                        break;
                    }
                    if (Account.MapID == 302 && BotCalculator.CalculateDistance(point) < 25)
                    {
                        Thread.Sleep(random.Next(2250, 4250));
                        break;
                    }
                    _counter--;
                    Thread.Sleep(2000);
                }
                if (Account.Poisioned)
                {
                    Thread.Sleep(7500);
                }
                if (!Account.Repairing)
                {
                    BotMethods.Repair();
                }
                var oldHp = Account.HP;
                var _repCounter = 0;
                Thread.Sleep(1250);
                while (Account.GetCurrentHpPercent < 100 && !Account.IsSunk && _repCounter < 10 && Account.Repairing)
                {
                    if (BotSettings.useDesignChanger && Account.DesignId != BotSettings.repDesign && Account.Route.Count <= 1)
                    {
                        Thread.Sleep(random.Next(250, 750));
                        BotMethods.ChangeDesign(BotSettings.repDesign);
                    }
                    if (!Account.Repairing)
                    {
                        if (Account.Poisioned)
                        {
                            Thread.Sleep(random.Next(1250, 1750));
                            BotMethods.Repair();
                            continue;
                        }
                        else
                        {
                            Thread.Sleep(250);
                            break;
                        }
                    }
                    if (Account.OnBM)
                    {
                        var ship = Bot.GetClosestShip();
                        if (ship.position.X != 0 && BotCalculator.CalculateDistance(ship.position) < 20)
                        {
                            break;
                        }
                    }
                    if (Account.OnRaid && Account.MapID == 300)
                    {
                        var ship = Bot.GetClosestShipRaid();
                        if (ship.position.X != 0 && BotCalculator.CalculateDistance(ship.position) < 45 && Account.GetCurrentHpPercent > 51)
                        {
                            break;
                        }
                    }
                    if (Account.GetCurrentHpPercent >= 100)
                    {
                        break;
                    }
                    if (_repCounter >= 7)
                    {
                        if (Account.HP == oldHp)
                        {
                            Account.Repairing = false;
                            break;
                        }
                        oldHp = Account.HP;
                        _repCounter = 0;
                    }
                    Thread.Sleep(1000);
                    _repCounter++;
                }
                var _hp = Account.OnBM ? BotSettings.repathpbm : Account.OnRaid ? BotSettings.repathpraid : BotSettings.repathp;
                if ((Account.MapID == 302 && Account.GetCurrentHpPercent < 99) || Account.GetCurrentHpPercent < _hp++)
                {
                    Thread.Sleep(1000);
                    RepairFunction(hpPercentage);
                    return;
                }
                BotLogic.OnAttackRunning = false;
                Thread.Sleep(2000);
                if (Account.Repairing)
                {
                    BotMethods.StopRepair();
                    return;
                }
                if (BotSettings.useDesignChanger && Account.DesignId != BotSettings.botDesign && !Account.IsSunk)
                {
                    while (true)
                    {
                        if (Account.Route.Count <= 1)
                        {
                            Thread.Sleep(random.Next(150, 450));
                            BotMethods.ChangeDesign(BotSettings.botDesign);
                            break;
                        }
                        Thread.Sleep(1000);
                    }
                }
                Thread.Sleep(1000);
            }
        }

        public static void FleeFromIslandThread()
        {
            BotLogic.StopBotThread();
            Bot.Running = false;
            if (!Client.Entitys.ContainsKey(Bot.underAttackBy.entityId) || !(Client.Entitys[Bot.underAttackBy.entityId] is TowerInitMessage))
            {
                goto IL_END;
            }
            var tower = (TowerInitMessage)Client.Entitys[Bot.underAttackBy.entityId];
            BotMethods.WriteLine("FLEEING FROM " + tower.guild + " ISLAND!");
            if (Client.Entitys.ContainsKey(Bot.targetentityId.entityId) && Client.Entitys[Bot.targetentityId.entityId] is BoxInitMessage)
            {
                Bot.invalidEntitys.Add(Bot.targetentityId.entityId, Client.Entitys[Bot.targetentityId.entityId]);
            }
            BotLogic.attacking = 15;
            Thread.Sleep(100);
            while (BotLogic.OnAttackRunning && !Account.IsSunk)
            {
                Point point = Bot.MoveToCorner(BotCalculator.CalculateIslandFleePoint(tower));
                BotMethods.MoveTo(point.X, point.Y);
                int counter = 80;
                int repetitions = 3;
                int x = 0;
                int y = 0;
                if (Client.Entitys.ContainsKey(tower.entityInfo.entityId))
                {
                    try
                    {
                        while (true)
                        {
                            if (counter > 0 && !Account.IsSunk && repetitions > 0 && BotLogic.attacking > 0 && BotLogic.OnAttackRunning)
                            {
                                Thread.Sleep(1000);
                                counter--;
                                BotLogic.attacking--;
                                if (x != Account.Position.X || y != Account.Position.Y)
                                {
                                    x = Account.Position.X;
                                    y = Account.Position.Y;
                                    repetitions = 3;
                                }
                                repetitions--;
                                if (!Client.Entitys.ContainsKey(tower.entityInfo.entityId))
                                {
                                    Thread.Sleep(random.Next(2520, 4975));
                                    goto IL_OUT;
                                }
                                if (x == point.X && y == point.Y)
                                {
                                    goto IL_END;
                                }
                                if (BotCalculator.CalculateDistance(point) <= 3 || repetitions <= 0 || (Account.Position.X == point.X && Account.Position.Y == point.Y))
                                {
                                    goto IL_OUT;
                                }
                                if (Account.Route.Count < 3)
                                {
                                    goto IL_OUT;
                                }
                                if (!BotLogic.OnAttackRunning)
                                {
                                    break;
                                }
                            }
                        }
                        return;
                    IL_OUT:;
                    }
                    catch (Exception)
                    {
                        break;
                    }
                    Dictionary<double, Seafight.Message> copy = Client.Entitys;
                    lock (copy)
                    {
                        if (!copy.ContainsKey(tower.entityInfo.entityId) || BotCalculator.CalculateDistance(tower.position) >= 90.0)
                        {
                            break;
                        }
                    }
                    continue;
                }
                break;
            }
        IL_END:
            BotLogic.OnAttackRunning = false;
            BotSettings.escapingIsland = false;
            BotSettings.underAttack = false;
            Bot.underAttackBy = new EntityInfo(0.0, 0);
            Bot.targetentityId = new EntityInfo(0.0, 0);
            if (!Account.IsSunk)
            {
                BotMethods.WriteLine("Succesfully escaped Island. Going on with Botting.");
            }
            Thread.Sleep(2000);
            StartBotThread();
        }

        public static void ShootBackThread()
        {
            BotLogic.StopBotThread();
            Bot.Running = false;
            if (!Client.Entitys.ContainsKey(Bot.underAttackBy.entityId) || !(Client.Entitys[Bot.underAttackBy.entityId] is PlayerInitMessage))
            {
                goto IL_END;
            }
            var player = (PlayerInitMessage)Client.Entitys[Bot.underAttackBy.entityId];
            var usedHpItem = false;
            var usedVpItem = false;
            var usedSkyfire = false;
            var usedElmosfire = false;
            BotMethods.WriteLine("UNDER ATTACK! SHOOTING BACK!");
            BotSession.Sessionshotback++;
            BotLogic.attacking = 15;
            Bot.ActivatePowderAndPlates(BotSettings.usePowderPlayer, BotSettings.useArmorPlayer);
            Thread.Sleep(1000);
            while (BotLogic.OnAttackRunning && !Account.IsSunk && Client.Entitys.ContainsKey(player.entityInfo.entityId) && (Client.Entitys[player.entityInfo.entityId] is PlayerInitMessage) && attacking > 0)
            {
                if (BotCalculator.CalculateDistance(player.position) <= Account.CanonRange - 6)
                {
                    BotMethods.Attack(player.entityInfo, BotSettings.AmmoIDShootBack);
                    BotLogic.attacking = 15;
                    BotMethods.WriteLine("Attacking " + player.username + " - [" + player.entityInfo.projectId + "/" + player.entityInfo.entityId + "]");
                    Thread.Sleep(random.Next(150, 325));
                    if (Account.Route.Count >= 3)
                    {
                        BotMethods.MoveTo(Account.Position);
                        Thread.Sleep(random.Next(110, 185));
                    }
                    while (BotLogic.attacking > 0 && Client.Entitys.ContainsKey(player.entityInfo.entityId) && BotCalculator.CalculateDistance(player.position) <= Account.CanonRange - 6 && BotLogic.OnAttackRunning && !Account.IsSunk && attacking > 0 && (Client.Entitys[player.entityInfo.entityId] is PlayerInitMessage))
                    {
                        if (BotSettings.useElmosfire && !usedElmosfire && BotCalculator.CalculateDistance(player.position) <= 15)
                        {
                            BotMethods.ActivateRocket(Items.ELMOS_FIRE, player.entityInfo);
                            usedElmosfire = true;
                            BotMethods.WriteLine("Using Elmosfire.");
                            Thread.Sleep(random.Next(608, 758));
                        }
                        if (BotSettings.useSkyfire && !usedSkyfire && BotCalculator.CalculateDistance(player.position) <= 32)
                        {
                            BotMethods.ActivateRocket(Items.SKY_FIRE, player.entityInfo);
                            usedSkyfire = true;
                            BotMethods.WriteLine("Using Skyfire");
                            Thread.Sleep(random.Next(608, 758));
                        }
                        if (BotSettings.useShootbackHPItem && !usedHpItem && Account.GetCurrentHpPercent < 75)
                        {
                            BotMethods.ActivateItem(BotSettings.shootBackHPItemID);
                            usedHpItem = true;
                            BotMethods.WriteLine("Using HP item.");
                            Thread.Sleep(random.Next(708, 888));
                        }
                        if (BotSettings.useShootbackVPItem && !usedVpItem && Account.GetCurrentVpPercent < 15)
                        {
                            BotMethods.ActivateItem(BotSettings.shootBackVPItemID);
                            usedVpItem = true;
                            BotMethods.WriteLine("Using VP Item.");
                            Thread.Sleep(random.Next(708, 888));
                        }
                        Thread.Sleep(1000);
                        attacking--;
                    }
                }
                while (Client.Entitys.ContainsKey(player.entityInfo.entityId) && BotLogic.OnAttackRunning && !Account.IsSunk && attacking > 0 && BotCalculator.CalculateDistance(player.position) > Account.CanonRange - 6 && (Client.Entitys[player.entityInfo.entityId] is PlayerInitMessage))
                {
                    if (BotCalculator.CalculateDistance(player.position) <= Account.CanonRange - 6)
                    {
                        break;
                    }
                    if (Account.Route.Count <= 1 || BotCalculator.CalculateDistance(Account.TargetX, Account.TargetY, player.position.X, player.position.Y) >= Account.CanonRange - 6)
                    {
                        BotMethods.MoveTo(player.position);
                        BotMethods.WriteLine("Moving closer to target.");
                    }
                    Thread.Sleep(1000);
                }
                Thread.Sleep(1000);
                attacking--;
            }
        IL_END:
            BotLogic.OnAttackRunning = false;
            BotSettings.escapingIsland = false;
            BotSettings.underAttack = false;
            Bot.underAttackBy = new EntityInfo(0.0, 0);
            Bot.targetentityId = new EntityInfo(0.0, 0);
            if (!Account.IsSunk)
            {
                BotMethods.WriteLine("Going on with Botting.");
            }
            Thread.Sleep(2000);
            StartBotThread();
        }

        public static void FleeThread()
        {
            BotLogic.StopBotThread();
            Bot.Running = false;
            if (!Client.Entitys.ContainsKey(Bot.underAttackBy.entityId) || !(Client.Entitys[Bot.underAttackBy.entityId] is PlayerInitMessage))
            {
                goto IL_END;
            }
            var player = (PlayerInitMessage)Client.Entitys[Bot.underAttackBy.entityId];
            var usedSpeedstone = false;
            var usedSmokebomb = false;
            BotMethods.WriteLine("UNDER ATTACK! FLEEING!");
            BotLogic.attacking = 15;
            Thread.Sleep(1000);
            while (BotLogic.OnAttackRunning && !Account.IsSunk)
            {
                Point point = Bot.GetQuarter(true);
                BotMethods.MoveTo(point.X, point.Y);
                int counter = 80;
                int repetitions = 3;
                int x = 0;
                int y = 0;
                if (Client.Entitys.ContainsKey(player.entityInfo.entityId))
                {
                    try
                    {
                        while (true)
                        {
                            if (counter > 0 && !Account.IsSunk && repetitions > 0 && BotLogic.attacking > 0 && BotLogic.OnAttackRunning)
                            {
                                Thread.Sleep(1000);
                                counter--;
                                BotLogic.attacking--;
                                if (x != Account.Position.X || y != Account.Position.Y)
                                {
                                    x = Account.Position.X;
                                    y = Account.Position.Y;
                                    repetitions = 3;
                                }
                                repetitions--;
                                if (!usedSpeedstone && BotSettings.useFleeSpeedStone)
                                {
                                    BotMethods.ActivateItem(Items.SPEEDSTONE);
                                    usedSpeedstone = true;
                                    Thread.Sleep(608);
                                }
                                if (!usedSmokebomb && BotSettings.useFleeSmokebomb && BotCalculator.CalculateDistance(player.position) <= 15)
                                {
                                    BotMethods.ActivateItem(Items.SMOKE_BOMB);
                                    usedSmokebomb = true;
                                    Thread.Sleep(703);
                                }
                                if (!Client.Entitys.ContainsKey(player.entityInfo.entityId))
                                {
                                    goto IL_OUT;
                                }
                                if (BotCalculator.CalculateDistance(point) <= 20 || repetitions <= 0)
                                {
                                    counter = 80;
                                    point = Bot.corner_1;
                                    switch (Bot.GetQuarter())
                                    {
                                        case 1:
                                            point = Bot.corner_2;
                                            break;
                                        case 2:
                                            point = Bot.corner_4;
                                            break;
                                        case 3:
                                            point = Bot.corner_3;
                                            break;
                                        case 4:
                                            point = Bot.corner_1;
                                            break;
                                    }
                                    BotMethods.MoveTo(point.X, point.Y);
                                }
                                if (!BotLogic.OnAttackRunning)
                                {
                                    break;
                                }
                            }
                        }
                        return;
                    IL_OUT:;
                    }
                    catch (Exception)
                    {
                        break;
                    }
                    Dictionary<double, Seafight.Message> copy = Client.Entitys;
                    lock (copy)
                    {
                        if (!copy.ContainsKey(player.entityInfo.entityId))
                        {
                            break;
                        }
                    }
                    continue;
                }
                break;
            }
        IL_END:
            BotLogic.OnAttackRunning = false;
            BotSettings.escapingIsland = false;
            BotSettings.underAttack = false;
            Bot.underAttackBy = new EntityInfo(0.0, 0);
            Bot.targetentityId = new EntityInfo(0.0, 0);
            if (!Account.IsSunk)
            {
                BotSession.Sessionescapedplayer++;
                BotMethods.WriteLine("Succesfully escaped. Going on with Botting.");
            }
            Thread.Sleep(2000);
            StartBotThread();
        }

        public static void FleeIfEnemyNearbyThread()
        {
            BotLogic.StopBotThread();
            Bot.Running = false;
            if (!Client.Entitys.ContainsKey(Bot.underAttackBy.entityId) || !(Client.Entitys[Bot.underAttackBy.entityId] is PlayerInitMessage))
            {
                goto IL_END;
            }
            var player = (PlayerInitMessage)Client.Entitys[Bot.underAttackBy.entityId];
            BotLogic.attacking = 15;
            Thread.Sleep(100);
            while (BotLogic.OnAttackRunning && !Account.IsSunk)
            {
                Point point = Bot.MoveToCorner();
                BotMethods.MoveTo(point.X, point.Y);
                int counter = 80;
                int repetitions = 3;
                int x = 0;
                int y = 0;
                if (Client.Entitys.ContainsKey(player.entityInfo.entityId))
                {
                    try
                    {
                        while (true)
                        {
                            if (counter > 0 && !Account.IsSunk && repetitions > 0 && BotLogic.attacking > 0 && BotLogic.OnAttackRunning)
                            {
                                Thread.Sleep(1000);
                                counter--;
                                BotLogic.attacking--;
                                if (x != Account.Position.X || y != Account.Position.Y)
                                {
                                    x = Account.Position.X;
                                    y = Account.Position.Y;
                                    repetitions = 3;
                                }
                                repetitions--;
                                if (!Client.Entitys.ContainsKey(player.entityInfo.entityId))
                                {
                                    Thread.Sleep(random.Next(1250, 2250));
                                    var _waitCounter = 10;
                                    BotMethods.WriteLine("Lost Enemy, returning to bot in ~10Seconds.");
                                    while (_waitCounter > 0 && !Account.IsSunk)
                                    {
                                        Thread.Sleep(random.Next(900, 1200));
                                        _waitCounter--;
                                    }
                                    goto IL_OUT;
                                }
                                if (BotCalculator.CalculateDistance(point) <= 10 || repetitions <= 0)
                                {
                                    var _waitCounter = 20;
                                    BotMethods.WriteLine("Will wait in corner, before returning to bot.");
                                    while (_waitCounter > 0 && !Account.IsSunk)
                                    {
                                        Thread.Sleep(1000);
                                        _waitCounter--;
                                    }
                                    goto IL_OUT;
                                }
                                if (!BotLogic.OnAttackRunning)
                                {
                                    break;
                                }
                            }
                        }
                        return;
                    IL_OUT:;
                    }
                    catch (Exception)
                    {
                        break;
                    }
                    Dictionary<double, Seafight.Message> copy = Client.Entitys;
                    lock (copy)
                    {
                        if (!copy.ContainsKey(player.entityInfo.entityId))
                        {
                            break;
                        }
                    }
                    continue;
                }
                break;
            }
        IL_END:
            BotLogic.OnAttackRunning = false;
            BotSettings.escapingIsland = false;
            BotSettings.underAttack = false;
            Bot.underAttackBy = new EntityInfo(0.0, 0);
            Bot.targetentityId = new EntityInfo(0.0, 0);
            if (!Account.IsSunk)
            {
                BotMethods.WriteLine("Succesfully avoided Enemy. Going on with Botting.");
            }
            Thread.Sleep(2000);
            StartBotThread();
        }
        #endregion onAttack

        #region Bonusmap

        private static bool BmShipThread()
        {
            var distance = 0.0;
            var range = ((Account.CanonRange >= 10) ? (Account.CanonRange - 5) : Account.CanonRange);
            var targetPoint = default(Point);
            var movecounter = 0;
            var ammo = BotSettings.AmmoIDBM;
            var ship = Bot.GetClosestShip();
            if (ship.position.X == 0)
            {
                return false;
            }
            if (!Client.Entitys.ContainsKey(Bot.targetentityId.entityId))
            {
                if (ship.entityInfo.entityId.Equals(0.0) && attacking < 3)
                {
                    ship.entityInfo = Client.Entitys.ToList().OfType<ShipInitMessage>().FirstOrDefault().entityInfo;
                    attacking++;
                }
                else if (attacking >= 3)
                {
                    BotSession.lostConnection = true;
                }
                if (!Client.Entitys.ContainsKey(ship.entityInfo.entityId))
                {
                    return false;
                }
            }
            Bot.targetentityId = ship.entityInfo;
            Bot.ActivatePowderAndPlates(BotSettings.usePowderBM, BotSettings.useArmorBM);
            distance = BotCalculator.CalculateDistance(ship.position);
            range = ((Account.CanonRange >= 10) ? (Account.CanonRange - 5) : Account.CanonRange);
            if (distance > range)
            {
                BotMethods.MoveTo(ship.position);
            }
            if (BotSettings.useBMAmmoChanger && Account.BonusMaps[Account.MapID].currentWave >= BotSettings.changeAmmoBMWave && BotSettings.AmmoIDBMChanged != ammo)
            {
                ammo = BotSettings.AmmoIDBMChanged;
            }
            BotMethods.WriteLine("Going to next " + ship.name);
            attacking = 0;
            while (attacking < 150 && distance > range && Client.Entitys.ContainsKey(ship.entityInfo.entityId) && !Bot.targetentityId.entityId.Equals(0.0))
            {
                if (Account.Route.Count <= 1)
                {
                    BotMethods.MoveTo(ship.position);
                }
                distance = BotCalculator.CalculateDistance(ship.position);
                Thread.Sleep(500);
                attacking++;
            }
            if (attacking >= 150)
            {
                if (Client.Entitys.ContainsKey(ship.entityInfo.entityId))
                {
                    Client.Entitys.Remove(ship.entityInfo.entityId);
                }
                return false;
            }
            attacking = 0;
            if (Account.Route.Count > 0)
            {
                BotMethods.MoveTo(Account.Position.X + random.Next(-1, 2), Account.Position.Y + random.Next(-2, 1));
                Thread.Sleep(random.Next(450, 650));
            }
            BotMethods.Attack(ship.entityInfo, ammo);
            BotMethods.WriteLine("Attacking " + ship.name);
            Thread.Sleep(random.Next(100, 275));
            double entityId = Bot.targetentityId.entityId;
            BotLogic.attacking = 15;
            while (!Bot.targetentityId.entityId.Equals(0.0) && Client.Entitys.ContainsKey(ship.entityInfo.entityId) && Account.GetCurrentHpPercent > BotSettings.repathpbm && attacking > 0)
            {
                ship = Bot.GetClosestShip();
                if (ship.position.X == 0)
                {
                    break;
                }
                distance = BotCalculator.CalculateDistance(ship.position);
                if (distance >= Account.CanonRange && (double)attacking < 14.0 - ((Account.ReloadTime > 0.0 ? Account.ReloadTime / 1000.0 : 5.0)))
                {
                    Bot.targetentityId = new EntityInfo(0.0, 0);
                    break;
                }
                if (!Bot.targetentityId.entityId.Equals(entityId) && !Bot.targetentityId.entityId.Equals(0.0) && BotSettings.rangeBMNpcs)
                {
                    entityId = ship.entityInfo.entityId;
                    Thread.Sleep(random.Next(350, 650));
                    BotMethods.Attack(ship.entityInfo, ammo);
                    Thread.Sleep(random.Next(249, 349));
                }
                if (BotSettings.rangeBMNpcs)
                {
                    if (distance < range)
                    {
                        var num5 = 1850;
                        var num6 = 1950;
                        if (range >= 13)
                        {
                            if (distance > range - 5)
                            {
                                num5 = 200;
                                num6 = 350;
                            }
                            if (distance > range - 10)
                            {
                                num5 = 400;
                                num6 = 650;
                            }
                        }
                        Point point = Bot.GetQuarter(true);
                        var num7 = point.X;
                        distance = BotCalculator.CalculateDistance(ship.position);
                        var num8 = (distance > range ? 0.0 : (range - distance)) - 1;
                        if (!num8.Equals(0.0))
                        {
                            if (num7 <= 298)
                            {
                                if (num7 != 4)
                                {
                                    if (num7 == 298)
                                    {
                                        if ((double)Account.Position.X > 288.0 && (double)Account.Position.Y > -283.0)
                                        {
                                            targetPoint = new Point(Account.Position.X - (int)num8 - random.Next(-3, 4), Account.Position.Y - (int)num8);
                                            BotMethods.MoveTo(targetPoint.X, targetPoint.Y);
                                            Thread.Sleep(random.Next(num5, num6));
                                        }
                                        else
                                        {
                                            targetPoint = new Point(Account.Position.X - (int)num8, Account.Position.Y + (int)num8 + random.Next(-3, 4));
                                            BotMethods.MoveTo(targetPoint.X, targetPoint.Y);
                                            Thread.Sleep(random.Next(num5, num6));
                                        }
                                    }
                                }
                                else if ((double)Account.Position.X > 14.0 && (double)Account.Position.Y < 9.0)
                                {
                                    targetPoint = new Point(Account.Position.X - (int)num8, Account.Position.Y + (int)num8 + random.Next(-3, 4));
                                    BotMethods.MoveTo(targetPoint.X, targetPoint.Y);
                                    Thread.Sleep(random.Next(num5, num6));
                                }
                                else
                                {
                                    targetPoint = new Point(Account.Position.X + (int)num8 + random.Next(-3, 4), Account.Position.Y + (int)num8);
                                    BotMethods.MoveTo(targetPoint.X, targetPoint.Y);
                                    Thread.Sleep(random.Next(num5, num6));
                                }
                            }
                            else if (num7 != 300)
                            {
                                if (num7 == 594)
                                {
                                    if ((double)Account.Position.X < 584.0 && (double)Account.Position.Y > -8.0)
                                    {
                                        targetPoint = new Point(Account.Position.X + (int)num8, Account.Position.Y - (int)num8 - random.Next(-3, 4));
                                        BotMethods.MoveTo(targetPoint.X, targetPoint.Y);
                                        Thread.Sleep(random.Next(num5, num6));
                                    }
                                    else
                                    {
                                        targetPoint = new Point(Account.Position.X - (int)num8 - random.Next(-3, 4), Account.Position.Y - (int)num8);
                                        BotMethods.MoveTo(targetPoint.X, targetPoint.Y);
                                        Thread.Sleep(random.Next(num5, num6));
                                    }
                                }
                            }
                            else if ((double)Account.Position.X < 310.0 && (double)Account.Position.Y < 285.0)
                            {
                                targetPoint = new Point(Account.Position.X + (int)num8 + random.Next(-3, 4), Account.Position.Y + (int)num8);
                                BotMethods.MoveTo(targetPoint.X, targetPoint.Y);
                                Thread.Sleep(random.Next(num5, num6));
                            }
                            else
                            {
                                targetPoint = new Point(Account.Position.X + (int)num8, Account.Position.Y - (int)num8 - random.Next(-3, 4));
                                BotMethods.MoveTo(targetPoint.X, targetPoint.Y);
                                Thread.Sleep(random.Next(num5, num6));
                            }
                            movecounter++;
                        }
                    }
                    if (Account.Route.Count > 0)
                    {
                        movecounter = 0;
                    }
                    else if (movecounter > 3 && Account.TargetX != targetPoint.X && Account.TargetY != targetPoint.Y)
                    {
                        var num7 = Bot.GetQuarter(true).X;
                        if (num7 <= 298)
                        {
                            if (num7 != 4)
                            {
                                if (num7 == 298)
                                {
                                    if ((double)Account.Position.X > 288.0 && (double)Account.Position.Y > -283.0)
                                    {
                                        BotMethods.MoveTo(Bot.corner_3.X, Bot.corner_3.Y);
                                    }
                                    else
                                    {
                                        BotMethods.MoveTo(Bot.corner_1.X, Bot.corner_1.Y);
                                    }
                                }
                            }
                            else if ((double)Account.Position.X > 14.0 && (double)Account.Position.Y < 9.0)
                            {
                                BotMethods.MoveTo(Bot.corner_1.X, Bot.corner_1.Y);
                            }
                            else
                            {
                                BotMethods.MoveTo(Bot.corner_2.X, Bot.corner_2.Y);
                            }
                        }
                        else if (num7 != 300)
                        {
                            if (num7 == 594)
                            {
                                if ((double)Account.Position.X < 584.0 && (double)Account.Position.Y > -8.0)
                                {
                                    BotMethods.MoveTo(Bot.corner_4.X, Bot.corner_4.Y);
                                }
                                else
                                {
                                    BotMethods.MoveTo(Bot.corner_3.X, Bot.corner_3.Y);
                                }
                            }
                        }
                        else if ((double)Account.Position.X < 310.0 && (double)Account.Position.Y < 285.0)
                        {
                            BotMethods.MoveTo(Bot.corner_2.X, Bot.corner_2.Y);
                        }
                        else
                        {
                            BotMethods.MoveTo(Bot.corner_4.X, Bot.corner_4.Y);
                        }
                        Thread.Sleep(random.Next(1750, 2550));
                        BotMethods.MoveTo(Account.Position.X - random.Next(-2, 2), Account.Position.Y + random.Next(-2, 2));
                        movecounter = 0;
                    }
                }
                attacking--;
                Thread.Sleep(1000);
            }
            attacking = 0;
            return true;
        }

        public static bool BmThread()
        {
            BotMethods.WriteLine("Starting Bonus Map Bot...");
            while (Bot.Running)
            {
                try
                {
                    Bot.BuyItems(false);
                    if (Account.OnBM)
                    {
                        if (BotSettings.leaveBM && BotSettings.canLeaveBM)
                        {
                            Thread.Sleep(250);
                            Bot.JoinMapThread("Leave Kraken");
                            BotSettings.leaveBM = false;
                            BotSettings.canLeaveBM = false;
                            return true;
                        }
                        RepairFunction(BotSettings.repathpbm);
                        BmShipThread();
                    }
                    else
                    {
                        Thread.Sleep(1500);
                    }
                    if (!Account.OnBM && BotSettings.autoJoinBM && BotSettings.lastBM.Length > 1 && Account.BonusMaps.Count > 0 && !Account.OnRaid)
                    {
                        var mapid = Account.BonusMaps.FirstOrDefault().Key;
                        var map = Account.BonusMaps.Select(item => item.Value.mapName == BotSettings.lastBM) as BonusMapStub;
                        if (BotSettings.joinSameBM && BotSettings.lastBM.Length > 1 && map != null && map.amount > 0)
                        {
                            mapid = map.mapId;
                        }
                        Thread.Sleep(250);
                        Bot.JoinMapThread("Join BM", mapid);
                        Thread.Sleep(500);
                        if (Account.OnBM)
                        {
                            BotSettings.lastBM = BotHandlers.MapHandler(Account.MapID);
                        }
                    }
                    Thread.Sleep(300);
                }
                catch (Exception)
                {
                }
            }
            return true;
        }

        #endregion BonusMap

        #region Raid

        public static bool IsValidRaidNPC(ShipInitMessage ship)
        {
            if (IsBoss(ship))
            {
                if (BotSettings.shootRaidBoss)
                {
                    return true;
                }
            }
            else
            {
                if (BotSettings.onlyFullHPRaidNpc)
                {
                    if (ship.pointsCurrent.hitpoints != ship.pointsMax.hitpoints)
                    {
                        return false;
                    }
                    return true;
                }
                return true;
            }
            return false;
        }

        private static bool RaidBossExists()
        {
            var shipCopy = Client.Entitys.Values.ToList().OfType<ShipInitMessage>();
            foreach (var ship in shipCopy)
            {
                if (IsBoss(ship))
                {
                    return true;
                }
            }
            return false;
        }

        private static bool IsValidRaidBoard(ShipInitMessage ship)
        {
            if (IsBoss(ship))
            {
                if (!BotSettings.boardVCMA)
                {
                    return false;
                }
            }
            return true;
        }

        private static ShipInitMessage GetRaidBoss()
        {
            var shipCopy = Client.Entitys.Values.ToList().OfType<ShipInitMessage>();
            var _ship = new ShipInitMessage(0.0, 0, 0, 0, 0, new PositionStub(0, 0), new List<PositionStub>());
            foreach (var ship in shipCopy)
            {
                if (IsBoss(ship))
                {
                    return ship;
                }
            }
            return _ship;
        }

        public static bool IsBoss(ShipInitMessage ship)
        {
            return (ship.nameId == 1150 || ship.nameId == 1265 || ship.nameId == 1753 || ship.nameId == 1754);
        }

        private static bool RaidShipsThread()
        {
            bool usedCandle = false;
            bool usedSnowman = false;
            bool usedSpeedstone = false;
            bool usedBloodlust = false;
            DateTime usedBloodlustTime = DateTime.Now;
            var ship = Bot.GetClosestShipRaid();
            if (RaidBossExists() && !IsBoss(ship) && BotSettings.shootRaidBoss)
            {
                ship = GetRaidBoss();
            }
            if (ship.position.X == 0 || ship.entityInfo.entityId.Equals(0.0))
            {
                Bot.lastEntity = ship.entityInfo;
                return false;
            }
            if (!Client.Entitys.ContainsKey(ship.entityInfo.entityId))
            {
                Bot.lastEntity = ship.entityInfo;
                return false;
            }
            if (!IsBoss(ship))
            {
                Bot.ActivatePowderAndPlates(BotSettings.usePowderRaid, BotSettings.useArmorRaid);
            }
            if (IsBoss(ship))
            {
                Bot.ActivatePowderAndPlates(BotSettings.usePowderRaidBoss, BotSettings.useArmorRaidBoss);
            }
            Bot.targetentityId = ship.entityInfo;
            double entityId = ship.entityInfo.entityId;
            attacking = 0;
            var bCounter = 0;
            var point = default(Point);
            BotMethods.WriteLine("Going to next " + ship.name + " [" + ship.entityInfo.entityId + "]");
            if (BotCalculator.CalculateDistance(ship.position) > (Account.CanonRange - 5))
            {
                point = Bot.MoveToNPC(ship);
                Thread.Sleep(random.Next(350, 675));
            }
            while (attacking < 75 && BotCalculator.CalculateDistance(ship.position) >= Account.CanonRange - 5)
            {
                if (Account.Route.Count <= 1)
                {
                    point = new Point(ship.position.X, ship.position.Y);
                    BotMethods.MoveTo(ship.position);
                }
                if (Account.TargetX != point.X && Account.TargetY != point.Y)
                {
                    BotMethods.MoveTo(point.X, point.Y);
                }
                if (!Client.Entitys.ContainsKey(ship.entityInfo.entityId))
                {
                    break;
                }
                if (IsBoss(ship) && BotCalculator.CalculateDistance(ship.position) > (Account.CanonRange * 1.5) && BotSettings.useRaidBossSpeedstone && !usedSpeedstone && Account.Items.ContainsKey(Items.SPEEDSTONE))
                {
                    BotMethods.ActivateItem(Items.SPEEDSTONE);
                    BotMethods.WriteLine("Using Speedstone.");
                    Thread.Sleep(600);
                }
                attacking++;
                Thread.Sleep(1000);
            }
            if (!Client.Entitys.ContainsKey(ship.entityInfo.entityId))
            {
                Bot.lastEntity = ship.entityInfo;
                return false;
            }
            if (!IsBoss(ship))
            {
                BotMethods.Attack(Bot.targetentityId, BotSettings.AmmoIDRaid);
            }
            else if (IsBoss(ship))
            {
                BotMethods.Attack(Bot.targetentityId, BotSettings.AmmoIDRaidBoss);
            }
            BotMethods.WriteLine("Attacking " + ship.name);
            Thread.Sleep(random.Next(150, 550));
            if (Account.Route.Count > 3)
            {
                BotMethods.MoveTo(Account.Position);
            }
            attacking = 15;
            int hp = BotSettings.finishNPCs ? BotSettings.finishNPCHpLimit : BotSettings.repathpraid;
            if (Account.MapID == 302)
            {
                if (IsBoss(ship))
                {
                    hp = BotSettings.repathpraid;
                }
            }
            while (Client.Entitys.ContainsKey(ship.entityInfo.entityId) && !Bot.targetentityId.entityId.Equals(0.0) && Bot.Running && Account.GetCurrentHpPercent >= hp && attacking > 0 && !Account.IsSunk)
            {
                if (IsBoss(ship))
                {
                    if (BotSettings.useRaidBossSnowman && !usedSnowman && (ship.pointsCurrent.hitpoints < (ship.pointsMax.hitpoints * 20 / 100)) && Account.Items.ContainsKey(Items.SNOWMAN))
                    {
                        BotMethods.ActivateItem(Items.SNOWMAN);
                        Thread.Sleep(random.Next(600, 1000));
                        usedSnowman = true;
                    }
                    if (BotSettings.useRaidBossCandle && !usedCandle && (ship.pointsCurrent.hitpoints < (ship.pointsMax.hitpoints * 20 / 100)) && Account.Items.ContainsKey(Items.CANDLE))
                    {
                        BotMethods.ActivateItem(Items.CANDLE);
                        Thread.Sleep(random.Next(600, 1000));
                        usedCandle = true;
                    }
                    if (BotSettings.useRaidBossBloodlust && !usedBloodlust && (ship.pointsCurrent.hitpoints > (ship.pointsMax.hitpoints * 7 / 100)) && Account.Items.ContainsKey(Items.BLOODLUST))
                    {
                        BotMethods.ActivateItem(Items.BLOODLUST);
                        Thread.Sleep(random.Next(600, 1000));
                        usedBloodlust = true;
                        usedBloodlustTime = DateTime.Now;
                    }
                    if (BotSettings.useRaidBossBloodlust && (DateTime.Now - usedBloodlustTime).TotalSeconds > random.Next(61, 65) && usedBloodlust)
                    {
                        usedBloodlust = false;
                    }
                    if (BotSettings.rebuyAmmoID == 51)
                    {
                        Bot.BuyItems(true);
                    }
                }
                if (BotSettings.shootRaidBoss && BotLogic.RaidBossExists() && !IsBoss(ship))
                {
                    BotMethods.WriteLine("Raid Boss spotted.");
                    Thread.Sleep(random.Next(300, 750));
                    break;
                }
                if (Account.MapID == 302)
                {
                    if (ship.pointsCurrent.hitpoints <= (ship.pointsMax.hitpoints * Account.BoardHPLimit / 100) && !ship.boarded)
                    {
                        if (IsValidRaidBoard(ship) && bCounter < 3)
                        {
                            if (BotCalculator.CalculateDistance(ship.position) >= Account.BoardingRange)
                            {
                                BotMethods.MoveTo(ship.position);
                                bCounter++;
                                Thread.Sleep(random.Next(1000, 1750));
                            }
                            if (BotCalculator.CalculateDistance(ship.position) < Account.BoardingRange)
                            {
                                BotMethods.BoardShip(Bot.targetentityId);
                                BotMethods.WriteLine("Boarding " + ship.name);
                                Thread.Sleep(random.Next(1000, 1750));
                                if (ship.boarded)
                                {
                                    bCounter += 4;
                                    Thread.Sleep(random.Next(250, 750));
                                }
                            }
                        }
                    }
                }
                if (BotCalculator.CalculateDistance(ship.position) >= Account.CanonRange)
                {
                    BotMethods.MoveTo(ship.position);
                    Thread.Sleep(random.Next(1000, 1850));
                    BotMethods.MoveTo(Account.Position.X + random.Next(-2, 2), Account.Position.Y + random.Next(-2, 2));
                }
                if (BotCalculator.CalculateDistance(ship.position) <= 3)
                {
                    BotMethods.MoveTo(ship.position.X + random.Next(6, 18), ship.position.Y + random.Next(4, 10));
                }
                Thread.Sleep(1000);
                attacking--;
            }
            if (!Client.Entitys.ContainsKey(ship.entityInfo.entityId) || Bot.targetentityId.entityId.Equals(0.0))
            {
                Bot.targetentityId = new EntityInfo(0, 0);
                BotSession.Sessionnpcs++;
            }
            return true;
        }

        public static bool RaidThread()
        {
            BotMethods.WriteLine("Starting Raid Bot...");
            while (Bot.Running)
            {
                try
                {
                    bool flag = false;
                    if (Account.OnRaid && BotSettings.leaveBM)
                    {
                        BotSettings.leaveBM = false;
                        BotSettings.canLeaveBM = false;
                        Thread.Sleep(450);
                        Bot.JoinMapThread("Leave Kraken");
                        Thread.Sleep(1000);
                        return false;
                    }
                    if (Account.OnRaid)
                    {
                        RepairFunction(BotSettings.repathpraid);
                        if (!flag)
                        {
                            flag = RaidShipsThread();
                        }
                        if (!flag)
                        {
                            var point = Bot.MoveThread();
                            BotMethods.MoveTo(point.X++, point.Y--);
                            BotMethods.WriteLine("Moving to next Position.");
                            int num5 = 60;
                            int num6 = 10;
                            Thread.Sleep(new Random().Next(200, 600));
                            int numX = 0;
                            int numY = 0;
                            while (Account.Position.X != point.X && Account.Position.Y != point.Y && Bot.Running && num5 > 0 && num6 > 0)
                            {
                                lock (Client.Entitys)
                                {
                                    var ship = Bot.GetClosestShipRaid();
                                    if (ship.position.X != 0 && ship.position.Y != 0 && ship.entityInfo != Bot.lastEntity)
                                    {
                                        break;
                                    }
                                }
                                if (!Account.OnRaid)
                                {
                                    break;
                                }
                                Thread.Sleep(1000);
                                num5--;
                                if (numX != Account.Position.X || numY != Account.Position.Y)
                                {
                                    numX = Account.Position.X;
                                    numY = Account.Position.Y;
                                    num6 = 10;
                                }
                                num6--;
                                if (Account.Route.Count <= 6)
                                {
                                    point = Bot.MoveThread();
                                    BotMethods.MoveTo(point.X, point.Y);
                                    num5 = 60;
                                }
                                if (Account.IsSunk)
                                {
                                    break;
                                }
                                if (Account.IsRepMaatLevel5 && !Account.Repairing && Account.HP < Account.MaxHP && !Account.Poisioned)
                                {
                                    BotMethods.Repair();
                                    Thread.Sleep(500);
                                    continue;
                                }
                            }
                        }
                        Thread.Sleep(100);
                        if (!Account.OnRaid)
                        {
                            Thread.Sleep(500);
                            if (!Account.OnRaid && BotSettings.autoJoinRaid)
                            {
                                Thread.Sleep(500);
                                Bot.JoinMapThread("Join Raid");
                            }
                            if (!Account.OnRaid)
                            {
                                return true;
                            }
                        }
                    }
                }
                catch (Exception)
                {
                }
            }
            return true;
        }
        #endregion Raid

        #region Npc & Monster
        private static bool IsValidBoard(ShipInitMessage ship)
        {
            try
            {
                if (ship != null)
                {
                    if (ship.useBoard == true)
                    {
                        if (ship.pointsCurrent.hitpoints <= (ship.pointsMax.hitpoints * Account.BoardHPLimit / 100))
                        {
                            if (ship.boardinAttackValue < Account.BoardingAttackValue)
                                return true;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                BotMethods.WriteLine(ex.ToString());
            }
            return false;
        }

        public static bool NpcThread()
        {
            Bot.targetentityId.entityId = 0.0;
            var ship = Bot.GetClosestNPC();
            var amount = (Bot.entitysAmount.ContainsKey(ship.name) ? Bot.entitysAmount[ship.name] : -10);
            if (amount <= 0 && amount != -10)
            {
                if (Bot.entitysAmount.ContainsKey(ship.name))
                {
                    Bot.entitysAmount.Remove(ship.name);
                }
                if (Bot.entitys.ContainsKey(ship.name))
                {
                    Bot.entitys[ship.name] = false;
                    Bot.entitys.Remove(ship.name);
                }
                Bot.lastEntity = ship.entityInfo;
                return false;
            }
            if (ship.position.X == 0 || ship.entityInfo.entityId.Equals(0.0))
            {
                Bot.lastEntity = ship.entityInfo;
                return false;
            }
            if (!Client.Entitys.ContainsKey(ship.entityInfo.entityId))
            {
                Bot.lastEntity = ship.entityInfo;
                return false;
            }
            Bot.BuyItems();
            BotMethods.WriteLine("Going to next " + ship.name + " [" + ship.entityInfo.entityId + "]" + (amount > 0 ? " - Amount left: " + amount-- : ""));
            Bot.targetentityId = ship.entityInfo;
            attacking = 0;
            Point point = new Point(0, 0);
            DateTime dateTime_ = DateTime.Now;
            TimeSpan timeSpan = new TimeSpan(0, 0, 60);
            int distance = (int)BotCalculator.CalculateDistance(ship.position);
            int range = (int)((Account.CanonRange >= 10) ? (Account.CanonRange - 5) : Account.CanonRange);
            if (distance > range)
            {
                if (ship.nameId != 1403)
                {
                    point = Bot.MoveToNPC(ship);
                }
                else
                {
                    point = new Point(ship.position.X + random.Next(-4, 4), ship.position.Y - range + random.Next(-4, 4));
                    BotMethods.MoveTo(point.X, point.Y);
                }
                Thread.Sleep(random.Next(300, 500));
            }
            Bot.ActivatePowderAndPlates(ship.usePowder, ship.usePlates);
            while (Bot.Running && distance > range && Client.Entitys.ContainsKey(ship.entityInfo.entityId))
            {
                Thread.Sleep(100);
                distance = (int)BotCalculator.CalculateDistance(ship.position);
                if (Account.TargetX != point.X && Account.TargetY != point.Y)
                {
                    BotMethods.MoveTo(point.X, point.Y);
                    Thread.Sleep(random.Next(200, 400));
                }
                if (Account.Route.Count < 1 && distance > range)
                {
                    point = Bot.MoveToNPC(ship);
                    Thread.Sleep(random.Next(200, 400));
                }
                timeSpan = DateTime.Now - dateTime_;
                attacking++;
                if (attacking > 600)
                {
                    return false;
                }
            }
            if (!Client.Entitys.ContainsKey(ship.entityInfo.entityId))
            {
                Bot.lastEntity = ship.entityInfo;
                return false;
            }
            if (Account.Route.Count > 2 && BotCalculator.CalculateDistance(Account.TargetX, Account.TargetY, ship.position.X, ship.position.Y) < 3)
            {
                BotMethods.MoveTo(Account.Position);
                Thread.Sleep(random.Next(125, 375));
            }
            if (BotLogic.OnAttackRunning)
            {
                return false;
            }
            BotMethods.WriteLine("Attacking " + ship.name);
            BotMethods.Attack(Bot.targetentityId, ship.ammoId);
            var bCounter = 0.0;
            var entityId = Bot.targetentityId;
            attacking = 15;
            int hplimit = BotSettings.finishNPCs ? BotSettings.finishNPCHpLimit : BotSettings.repathp;
            while (Bot.Running && !Bot.targetentityId.entityId.Equals(0.0) && Account.GetCurrentHpPercent >= hplimit && !Account.IsSunk && attacking > 0)
            {
                distance = (int)BotCalculator.CalculateDistance(ship.position);
                if (distance > Account.CanonRange - ((Account.CanonRange >= 30) ? 6 : 4) && ship.route.Count > 0)
                {
                    BotMethods.MoveTo(ship.position);
                    Thread.Sleep(random.Next(1250, 1750));
                    BotMethods.MoveTo(Account.Position);
                }
                if (distance <= 3)
                {
                    BotMethods.MoveTo(ship.position.X + random.Next(6, 18), ship.position.Y + random.Next(4, 10));
                    Thread.Sleep(random.Next(250, 375));
                }
                if (BotSettings.useHumanMovement && ship.pointsCurrent.hitpoints <= BotSession.Canondamage && Account.ReloadTime <= 4.9 && distance <= range - 6)
                {
                    Thread.Sleep(random.Next((int)(Account.ReloadTime * 0.2), (int)(Account.ReloadTime * 0.7)));
                    Bot.targetentityId.entityId = 0.0;
                    break;
                }
                if (IsValidBoard(ship))
                {
                    if (!ship.boarded && bCounter < 3)
                    {
                        var _position = new PositionStub(Account.Position.X + random.Next(-4, 4), Account.Position.Y + random.Next(-4, 4));
                        if (distance >= Account.BoardingRange)
                        {
                            BotMethods.MoveTo(ship.position);
                            Thread.Sleep(random.Next(500, 1050));
                        }
                        if (distance < Account.BoardingRange)
                        {
                            BotMethods.BoardShip(Bot.targetentityId);
                            bCounter++;
                            BotMethods.WriteLine("Boarding " + ship.name);
                            Thread.Sleep(random.Next(1120, 1820));
                            if (ship.boarded)
                            {
                                Thread.Sleep(random.Next(1250, 2225));
                                bCounter += 3.5;
                                BotMethods.MoveTo(_position);
                            }
                        }
                    }
                }
                if (BotSettings.collectWhileAttack && ((double)ship.pointsCurrent.hitpoints / (double)ship.pointsMax.hitpoints * 100.0) >= 5)
                {
                    var _point = Bot.GetClosestBoxInRange(new Point(ship.position.X, ship.position.Y), (int)(Account.CanonRange - 2));
                    if (_point.position.X != 0 && _point.position.Y != 0)
                    {
                    IL_SEARCH:
                        var num = 0;
                        var value = _point.entityId;
                        while (!Bot.targetentityId.entityId.Equals(0.0) && _point.position.X != 0 && Bot.Running && Client.Entitys.ContainsKey(value))
                        {
                            if ((Account.TargetX != _point.position.X || Account.TargetY != _point.position.Y))
                            {
                                BotMethods.MoveTo(_point.position);
                                Thread.Sleep(random.Next(250, 450));
                            }
                            if (Account.Position.X == _point.position.X && Account.Position.Y == _point.position.Y || Account.Route.Count <= 2)
                            {
                                num++;
                                Thread.Sleep(100);
                            }
                            if (num > 40)
                            {
                                break;
                            }
                            timeSpan = (DateTime.Now - dateTime_);
                            Thread.Sleep(10);
                        }
                        if (Account.Position.X == _point.position.X && Account.Position.Y == _point.position.Y || !Client.Entitys.ContainsKey(value))
                        {
                            BotSession.Sessionglitters++;
                        }
                        if ((_point = Bot.GetClosestBoxInRange(new Point(ship.position.X, ship.position.Y), (int)(Account.CanonRange - 2))).position.X != 0 && !Bot.targetentityId.entityId.Equals(0.0) && Bot.Running && Client.Entitys.ContainsKey(ship.entityInfo.entityId))
                        {
                            goto IL_SEARCH;
                        }
                    }
                }
                attacking--;
                Thread.Sleep(1000);
            }
            if (!Client.Entitys.ContainsKey(entityId.entityId) || Bot.targetentityId.entityId.Equals(0.0))
            {
                BotSession.Sessionnpcs++;
                if (Bot.entitysAmount.ContainsKey(ship.name))
                {
                    Bot.entitysAmount[ship.name] -= 1;
                }
                if (Bot.entitysAmount.ContainsKey(ship.name) && Bot.entitysAmount[ship.name] <= 0 && Bot.entitys.ContainsKey(ship.name))
                {
                    Bot.entitys[ship.name] = false;
                    Bot.entitysAmount.Remove(ship.name);
                    BotMethods.WriteLine("Finished shooting " + ship.name + " Amount.");
                }
            }
            return true;
        }

        private static bool monsterThread()
        {
            var monster = Bot.GetClosestMonster();
            if (!monster.entityInfo.entityId.Equals(0.0) && monster.position.X != 0)
            {
                var amount = (Bot.entitysAmount.ContainsKey(monster.name) ? Bot.entitysAmount[monster.name] : -10);
                if (amount <= 0 && amount != -10)
                {
                    if (Bot.entitysAmount.ContainsKey(monster.name))
                    {
                        Bot.entitysAmount.Remove(monster.name);
                    }
                    if (Bot.entitys.ContainsKey(monster.name))
                    {
                        Bot.entitys[monster.name] = false;
                        Bot.entitys.Remove(monster.name);
                    }
                    Bot.lastEntity = monster.entityInfo;
                    return false;
                }
                Bot.targetentityId = monster.entityInfo;
                Bot.BuyItems();
                BotMethods.WriteLine($"Going to next {monster.name} [" + monster.entityInfo.entityId + "]" + (amount > 0 ? " - Amount left: " + amount-- : ""));
                int distance = (int)BotCalculator.CalculateDistance(monster.position);
                int _counter = 120;
                int currentHp = monster.hitpoints;
                var point = new Point(0, 0);
                if (distance > 10)
                {
                    point = Bot.MoveToMonster(monster);
                }
                while (Bot.Running && _counter > 0 && distance > 10)
                {
                    if (Account.TargetX != point.X && Account.TargetY != point.Y)
                    {
                        BotMethods.MoveTo(point.X, point.Y);
                        Thread.Sleep(random.Next(200, 400));
                    }
                    if (Account.Route.Count < 2 && distance >= 12)
                    {
                        point = Bot.MoveToMonster(monster);
                        Thread.Sleep(random.Next(200, 400));
                    }
                    if (monster.hitpoints < currentHp && BotSettings.onlyFullHPMonsters)
                    {
                        if (Client.Entitys.ContainsKey(Bot.targetentityId.entityId))
                        {
                            Client.Entitys.Remove(Bot.targetentityId.entityId);
                        }
                        Bot.targetentityId = new EntityInfo(0, 0);
                        Bot.lastEntity = monster.entityInfo;
                        goto IL_END;
                    }
                    distance = (int)BotCalculator.CalculateDistance(monster.position);
                    _counter--;
                    Thread.Sleep(500);
                }
                if (_counter <= 0)
                {
                    if (Client.Entitys.ContainsKey(Bot.targetentityId.entityId))
                    {
                        Client.Entitys.Remove(Bot.targetentityId.entityId);
                    }
                    Bot.targetentityId = new EntityInfo(0, 0);
                    Bot.lastEntity = monster.entityInfo;
                    goto IL_END;
                }
                if (Account.Route.Count > 3 && BotCalculator.CalculateDistance(Account.TargetX, Account.TargetY, monster.position.X, monster.position.Y) < 3)
                {
                    BotMethods.MoveTo(Account.Position.X, Account.Position.Y);
                    Thread.Sleep(random.Next(150, 250));
                }
                if (BotLogic.OnAttackRunning)
                {
                    return false;
                }
                BotMethods.WriteLine("Attacking " + monster.name);
                BotMethods.AttackMonster(Bot.targetentityId, monster.harpoonId);
                BotLogic.attacking = 15;
                while (Bot.Running && !Bot.targetentityId.entityId.Equals(0.0) && BotLogic.attacking > 0 && Account.GetCurrentHpPercent >= (BotSettings.finishNPCs ? BotSettings.finishNPCHpLimit : BotSettings.repathp) && !Account.IsSunk)
                {
                    if (BotCalculator.CalculateDistance(monster.position) <= 3)
                    {
                        BotMethods.MoveTo(monster.position.X + random.Next(2, 5), monster.position.Y + random.Next(1, 3));
                    }
                    BotLogic.attacking--;
                    Thread.Sleep(1000);
                }
                if (!Client.Entitys.ContainsKey(monster.entityInfo.entityId) || Bot.targetentityId.entityId.Equals(0.0))
                {
                    BotSession.Sessionmonsters++;
                    if (Bot.entitysAmount.ContainsKey(monster.name))
                    {
                        Bot.entitysAmount[monster.name] -= 1;
                    }
                    if (Bot.entitysAmount.ContainsKey(monster.name) && Bot.entitysAmount[monster.name] <= 0 && Bot.entitys.ContainsKey(monster.name))
                    {
                        Bot.entitys[monster.name] = false;
                        Bot.entitysAmount.Remove(monster.name);
                        BotMethods.WriteLine("Finished shooting " + monster.name + " Amount.");
                    }
                }
            IL_END:
                return true;
            }
            return false;
        }

        #endregion Npc & Monster

        #region Logic
        private static void BotThread()
        {
            while (Account.gClass == null)
            {
                Thread.Sleep(1000);
            }
            while (Account.gClass.entityInfo == null)
            {
                Thread.Sleep(2000);
            }
            int counter = 0;
            if (TaskSystem.TasksSystem.Tasks.Count > 0 || TaskSystem.TasksSystem.UseTaskSystem)
            {
                TaskSystem.TasksSystem.StartTaskSystem();
                Thread.Sleep(1000);
            }
            while (Bot.Running)
            {
                try
                {
                    bool flag = false;
                    counter = 0;
                    if (BotSettings.useQuestSystem && !Account.OnBM && !Account.OnRaid && !Account.IsSunk)
                    {
                        Bot.NextQuest();
                        if (Bot.currentQuest != null)
                        {
                            flag = Bot.DoQuest();
                        }
                    }
                    if (!Account.OnRaid && BotSettings.autoJoinRaid && BotSettings.RaidType > 0 && !Account.OnBM && !Account.IsSunk)
                    {
                        Bot.JoinMapThread("Join Raid");
                        Thread.Sleep(100);
                    }
                    if (!Account.OnBM && BotSettings.autoJoinBM && BotSettings.lastBM.Length > 1 && Account.BonusMaps.Count > 0 && !Account.OnRaid && !Account.IsSunk)
                    {
                        var mapid = Account.BonusMaps.FirstOrDefault().Key;
                        var map = Account.BonusMaps.Select(item => item.Value.mapName == BotSettings.lastBM) as BonusMapStub;
                        if (BotSettings.joinSameBM && BotSettings.lastBM.Length > 1 && map != null && map.amount > 0)
                        {
                            mapid = map.mapId;
                        }
                        Bot.JoinMapThread("Join BM", mapid);
                        Thread.Sleep(100);
                    }
                    if (Account.OnBM)
                    {
                        BotSettings.lastBM = BotHandlers.MapHandler(Account.MapID);
                        BmThread();
                        flag = true;
                        Thread.Sleep(2000);
                    }
                    if (Account.OnRaid)
                    {
                        RaidThread();
                        flag = true;
                        Thread.Sleep(1500);
                    }
                    CheckMap();
                    RepairFunction(BotSettings.repathp);
                    if (!flag && BotSettings.shootNPCs && BotSettings.prioNPCs)
                    {
                        flag = NpcThread();
                    }
                    if (!flag && BotSettings.shootMonsters && BotSettings.prioMonsters)
                    {
                        flag = monsterThread();
                    }
                    if (!flag && (BotSettings.collectGlitters || BotSettings.collecteventchests || BotSettings.collectchests || BotSettings.collectMeat || BotSettings.collectEventGlitter))
                    {
                        var point = Bot.GetClosestBox();
                        var mispoint = new Point(point.X + random.Next(-2, 3), point.Y + random.Next(-3, 2));
                        if (point.X != 0 || point.Y != 0)
                        {
                            int modus = random.Next(0, 3);
                            if (BotSettings.useHumanMovement && BotCalculator.CalculateChance((int)BotCalculator.CalculateDistance(point)))
                            {
                                BotMethods.MoveTo(mispoint.X, mispoint.Y);
                                Thread.Sleep(random.Next(295, 525));
                            }
                            else
                            {
                                try
                                {
                                    if (Client.Entitys.ToList().OfType<MonsterInitMessage>().ToList().Exists(monster => BotCalculator.CalculateDistance(monster.position.X, monster.position.Y, point.X, point.Y) <= 2.88))
                                    {
                                        BotMethods.MoveTo(point.X + 2, point.Y + random.Next(1, 3));
                                        modus = 5;
                                    }
                                    else
                                    {
                                        BotMethods.MoveTo(point);
                                    }
                                }
                                catch (Exception)
                                {
                                    BotMethods.MoveTo(point);
                                }
                            }
                            BotMethods.WriteLine($"Going to next Item[{Bot.targetentityId.entityId}] - X: {point.X}|Y: " + point.Y);
                            DateTime dateTime_ = DateTime.Now;
                            TimeSpan timeSpan = (DateTime.Now - dateTime_);
                            double value = Bot.targetentityId.entityId;
                            int num = 0;
                            while (!Bot.targetentityId.entityId.Equals(0.0) && timeSpan.TotalSeconds < 60.0 && Bot.Running && Account.GetCurrentHpPercent >= Account.GetRepAtHp)
                            {
                                if ((Account.TargetX != point.X || Account.TargetY != point.Y) && modus < 2)
                                {
                                    BotMethods.MoveTo(modus == 1 ? point : new PositionStub(mispoint.X, point.Y));
                                    modus = random.Next(0, 3);
                                    Thread.Sleep(100);
                                }
                                if (((Account.TargetX != point.X || Account.TargetY != point.Y) && modus >= 2) && BotCalculator.CalculateDistance(point) < 12)
                                {
                                    BotMethods.MoveTo(modus == 2 ? point : new PositionStub(point.X, mispoint.Y));
                                    modus = random.Next(0, 3);
                                    Thread.Sleep(100);
                                }
                                if (Account.Route.Count < 1 && BotCalculator.CalculateDistance(point) > 0.51 && modus >= 2)
                                {
                                    BotMethods.MoveTo(point);
                                }
                                if (Account.Position.X == point.X && Account.Position.Y == point.Y)
                                {
                                    num++;
                                    Thread.Sleep(100);
                                }
                                if (!Client.Entitys.ContainsKey(value) || num > 40)
                                {
                                    break;
                                }
                                timeSpan = (DateTime.Now - dateTime_);
                                Thread.Sleep(10);
                            }
                            if (Client.Entitys.ContainsKey(value) && timeSpan.TotalSeconds >= 59 || num > 40)
                            {
                                Client.Entitys.Remove(value);
                            }
                            flag = true;
                        }
                    }
                    if (!flag && BotSettings.shootNPCs && !BotSettings.prioNPCs)
                    {
                        flag = NpcThread();
                    }
                    if (!flag && BotSettings.shootMonsters && !BotSettings.prioMonsters)
                    {
                        flag = monsterThread();
                    }
                    if (!flag)
                    {
                        var point = Bot.MoveThread();
                        BotMethods.MoveTo(point.X++, point.Y--);
                        BotMethods.WriteLine("Moving to next Position.");
                        int num5 = 60;
                        int num6 = 10;
                        int num7 = 11;
                        Thread.Sleep(new Random().Next(200, 600));
                        int numX = 0;
                        int numY = 0;
                        while (Account.Position.X != point.X && Account.Position.Y != point.Y && Bot.Running && num5 > 0 && num6 > 0)
                        {
                            Dictionary<double, Message> copyEntitys = Client.Entitys;
                            lock (copyEntitys)
                            {
                                if (BotSettings.collectGlitters || BotSettings.collecteventchests || BotSettings.collectchests || BotSettings.collectMeat || BotSettings.collectEventGlitter)
                                {
                                    var pos = Bot.GetClosestBox();
                                    if (pos.X != 0 && pos.Y != 0)
                                    {
                                        break;
                                    }
                                }
                                if (BotSettings.shootNPCs && Bot.Running)
                                {
                                    var ship = Bot.GetClosestNPC();
                                    if (ship.position.X != 0 && ship.position.Y != 0 && ship.entityInfo != Bot.lastEntity)
                                    {
                                        break;
                                    }
                                }
                                if (BotSettings.shootMonsters && Bot.Running)
                                {
                                    var monster = Bot.GetClosestMonster();
                                    if (!monster.entityInfo.entityId.Equals(0.0) && monster.entityInfo != Bot.lastEntity)
                                    {
                                        break;
                                    }
                                }
                            }
                            if (BotSettings.jumpMaps && BotSettings.needMapSwitch && !BotSettings.jumpMapIfAvailable && Bot.Running)
                            {
                                Thread.Sleep(1200);
                                if (Bot.JumpMapCheck())
                                {
                                    break;
                                }
                                Thread.Sleep(500);
                            }
                            if (Account.OnBM || Account.OnRaid || (!Account.OnBM && Account.BonusMaps.Count > 0 && BotSettings.autoJoinBM && BotSettings.lastBM.Length > 1) || (!Account.OnRaid && BotSettings.autoJoinRaid && Account.Items.ContainsKey(Account.RaidMedallion) && Account.Items[Account.RaidMedallion].amount > 0 && BotSettings.RaidType > 0))
                            {
                                break;
                            }
                            Bot.CheckTimers();
                            Thread.Sleep(1000);
                            num5--;
                            if (numX != Account.Position.X || numY != Account.Position.Y)
                            {
                                numX = Account.Position.X;
                                numY = Account.Position.Y;
                                num6 = 10;
                            }
                            num6--;
                            if (Account.Route.Count <= num7)
                            {
                                point = Bot.MoveThread();
                                BotMethods.MoveTo(point.X, point.Y);
                                num5 = 60;
                                num7 = random.Next(-5, 9);
                                num7 = num7 < 0 ? 0 : num7;
                            }
                            if (Account.IsSunk || (Account.GetCurrentHpPercent < BotSettings.repathp && Account.GetCurrentHpPercent > 10 && !Account.IsRepMaatLevel5))
                            {
                                break;
                            }
                            if (Account.IsRepMaatLevel5 && !Account.Repairing && Account.HP < Account.MaxHP && !Account.Poisioned && counter <= 5)
                            {
                                BotMethods.Repair();
                                Thread.Sleep(500);
                                counter++;
                                continue;
                            }
                        }
                    }
                }
                catch (Exception)
                {
                }
            }
        }

        #endregion Logic

        #region Functions

        public static void CheckMap()
        {
            try
            {
                if (Account.MapID == 99)
                {
                    if (((Bot.entitys.ContainsKey(BotSession.krakenTentacleName) && !Bot.entitys[BotSession.krakenTentacleName]) && (Bot.entitys.ContainsKey(BotSession.krakenName) && !Bot.entitys[BotSession.krakenName])) || !BotSettings.shootNPCs)
                    {
                        Bot.JoinMapThread("Leave Kraken");
                        Thread.Sleep(1250);
                    }
                }
                if (Account.IsSunk)
                {
                    Thread.Sleep(2000);
                    if (Account.IsSunk)
                    {
                        if (Account.MapID == 500)
                        {
                            Thread.Sleep(random.Next(250, 1025));
                            if ((Account.LastMapID == 301 || Account.LastMapID == 302 || Account.LastMapID == 300) && Account.RejoinCurrentMap && BotSettings.autoJoinRaid)
                            {
                                BotMethods.ReviveRaid();
                            }
                            else
                            {
                                BotMethods.Revive();
                            }
                            BotMethods.WriteLine("Ship repaired.");
                            Thread.Sleep(random.Next(120, 725));
                            return;
                        }
                        BotCalculator.StopThreads();
                        BotLogic.StopBotThread();
                        BotMethods.WriteLine("There was an error while reading packets!");
                        BotSession.lostConnection = true;
                    }
                }
            }
            catch (Exception)
            {
            }
        }

        #endregion Functions

        #region Logic Starter
        public static void StartBotThread()
        {
            try
            {
                Bot.Running = false;
                if (Bot._botThread != null)
                {
                    Bot._botThread.Abort();
                    Bot._botThread.Join();
                }
                Bot.Running = true;
                Bot._botThread = new Thread(new ThreadStart(BotThread));
                Bot._botThread.Start();
            }
            catch (Exception)
            {
                Thread.Sleep(2000);
                StartBotThread();
            }
        }

        public static void StopBotThread()
        {
            try
            {
                Bot.Running = false;
                if (Bot._botThread != null)
                {
                    Bot._botThread.Abort();
                    Bot._botThread.Join();
                }
            }
            catch
            {
                Bot.Running = false;
            }
        }

        #endregion Logic Starter
    }
}
