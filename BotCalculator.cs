using BoxyBot.Seafight;
using BoxyBot.Seafight.Messages;
using BoxyBot.Util;
using System;
using System.Drawing;
using System.Linq;
using System.Threading;

namespace BoxyBot
{
    public static class BotCalculator
    {
        public static HelpTools help = new HelpTools();
        private static Thread _shipThread;
        private static Thread _connectionThread;

        private static void ConnectionThread()
        {
            try
            {
                DateTime dateTime_ = DateTime.Now;
                while (true)
                {
                    TimeSpan ts = (DateTime.Now - Client.lastPacket);
                    if (!Server.IsConnected() || ts.TotalSeconds > 70.0)
                    {
                        BotSession.Sessionreconnects++;
                        break;
                    }
                    TimeSpan ts2 = (DateTime.Now - dateTime_);
                    if (ts2.TotalMinutes >= 1.0)
                    {
                        GC.Collect();
                        GC.WaitForPendingFinalizers();
                        HelpTools.ClearMemory(HelpTools.GetCurrentProcess());
                        dateTime_ = DateTime.Now;
                    }
                    TimeSpan ts3 = (DateTime.Now - BotSession.dailyQuestFinishTime);
                    if (ts3.TotalHours >= 24 && ts3.TotalMinutes >= 1 && ts3.TotalSeconds >= 1 && !BotSession.Sessioncandodailyquest)
                    {
                        BotSession.Sessioncandodailyquest = true;
                    }
                    Thread.Sleep(1000);
                }
                Thread.Sleep(2000);
                Bot.Running = false;
                if (Server._targetSocket != null && Server._targetSocket.Connected)
                {
                    Server._targetSocket.Close();
                }
                if (Bot.Running)
                {
                    BotLogic.StopBotThread();
                }
                if (BotLogic.OnAttackRunning)
                {
                    BotLogic.StoponAttackThrad();
                }
                Server.Connected = false;
                if (BotSettings.tasksDoneLogout && TaskSystem.TasksSystem.LogginOut)
                {
                    return;
                }
                BotSession.lostConnection = true;
            }
            catch (Exception ex)
            {
                BotMethods.WriteLine("There was an error in the connection-Thread!\n\n" + ex.Message);
                ConnectionThread();
                return;
            }
        }

        private static void ShipThread()
        {
            while (true)
            {
                try
                {
                    if (Account.gClass.entityInfo != null)
                    {
                        CalculateCoordinates();
                    }
                    if (Client.Entitys.Count > 0)
                    {
                        var shipList = Client.Entitys.Values.OfType<ShipInitMessage>().ToList();
                        foreach (var ship in shipList)
                        {
                            if (ship != null)
                            {
                                CalculateCoordinatesNPC(ship);
                            }
                        }
                        var playerList = Client.Entitys.Values.OfType<PlayerInitMessage>().ToList();
                        foreach (var player in playerList)
                        {
                            if (player != null)
                            {
                                CalculateCoordinatesPlayer(player);
                            }
                        }
                        if (BotSettings.avoidIslands)
                        {
                            var towerList = Client.Entitys.Values.OfType<TowerInitMessage>().ToList();
                            foreach (var tower in towerList)
                            {
                                if (tower != null)
                                {
                                    CheckForIsland(tower);
                                }
                            }
                        }
                    }
                    Thread.Sleep(100);
                }
                catch (Exception)
                {
                }
            }
        }

        public static void StartThreads()
        {
            if (_shipThread != null)
            {
                _shipThread.Abort();
                int num = 0;
                while (true)
                {
                    if (!_shipThread.IsAlive)
                    {
                        if (_shipThread.ThreadState != System.Threading.ThreadState.Running)
                        {
                            break;
                        }
                    }
                    if (num >= 5)
                    {
                        break;
                    }
                    num++;
                    Thread.Sleep(1000);
                }
            }
            if (_connectionThread != null)
            {
                _connectionThread.Abort();
                int num = 0;
                while (true)
                {
                    if (!_connectionThread.IsAlive)
                    {
                        if (_connectionThread.ThreadState != System.Threading.ThreadState.Running)
                        {
                            break;
                        }
                    }
                    if (num >= 5)
                    {
                        break;
                    }
                    num++;
                    Thread.Sleep(1000);
                }
            }
            if ((Bot._botThread == null || !Bot.Running) && BotLogic.attacking >= 0)
            {
                BotLogic.StartBotThread();
            }
            _shipThread = new Thread((ShipThread));
            _shipThread.Start();
            _connectionThread = new Thread((ConnectionThread));
            _connectionThread.Start();
        }

        public static void StopThreads()
        {
            if (_connectionThread != null)
            {
                _connectionThread.Abort();
            }
            if (Server._targetSocket != null && Server._targetSocket.Connected)
            {
                Server._targetSocket.Close();
            }
            if (_shipThread != null)
            {
                _shipThread.Abort();
            }
        }

        private static void CheckForIsland(TowerInitMessage tower)
        {
            try
            {
                if (!tower.guild.Equals(Account.Guild))
                {
                    foreach (var pos in Account.Route)
                    {
                        if ((CalculateDistance(pos.X, pos.Y, tower.position.X, tower.position.Y) <= 48))
                        {
                            if (CalculateDistance(Account.Position.X, Account.Position.Y, pos.X, pos.Y) < 4.5)
                            {
                                Bot.underAttackBy = tower.entityInfo;
                                BotSettings.escapingIsland = true;
                                if (!BotLogic.OnAttackRunning && Bot.Running)
                                {
                                    BotLogic.StartonAttackThread();
                                }
                                return;
                            }
                        }
                    }
                }
            }
            catch (Exception)
            {
            }
        }

        public static int CalculateIslandFleePoint(TowerInitMessage tower)
        {
            if (tower == null)
            {
                return 1;
            }
            switch (tower.towerId)
            {
                case 1:
                    return 4;
                case 2:
                    return 1;
                case 3:
                    return 1;
                case 4:
                    return 1;
                case 5:
                    return 2;
                case 6:
                    return 2;
                case 7:
                    return 2;
                case 8:
                    return 2;
                case 9:
                    return 3;
                case 10:
                    return 3;
                case 11:
                    return 3;
                case 12:
                    return 4;
            }
            return 3;
        }

        private static void CalculateCoordinates()
        {
            if (Account.Position != null)
            {
                double speed = Account.Speed / help.SimplySpeed(Account.Speed);
                var user = new UserInitMessage()
                {
                    route = Account.Route
                };
                if (user.route != null && user.route.Count > 0)
                {
                    try
                    {
                        if (Account.Position.dX.Equals(0.0) && Account.Position.dY.Equals(0.0))
                        {
                            Account.Position.dX = Account.Position.X;
                            Account.Position.dY = Account.Position.Y;
                        }
                        foreach (var current in user.route)
                        {
                            if ((user.route != null && user.route.Count == 0) || user.route == null)
                            {
                                break;
                            }
                            int newX = current.X - Account.Position.X;
                            int newY = current.Y - Account.Position.Y;
                            double distance = Math.Sqrt(Math.Pow((double)newX, 2.0) + Math.Pow((double)newY, 2.0));
                            if (distance > speed || (distance.Equals(1.0) && speed > 1.0))
                            {
                                double duration = speed / distance;
                                Account.Position.dX += duration * (double)newX;
                                Account.Position.dY += duration * (double)newY;
                                Account.Position.X = (int)Account.Position.dX;
                                Account.Position.Y = (int)Account.Position.dY;
                                if ((Account.Position.X == current.X && Account.Position.Y == current.Y) || duration > 1.0)
                                {
                                    user.route.Remove(current);
                                }
                                break;
                            }
                            Account.Position.dX = (double)current.X;
                            Account.Position.dY = (double)current.Y;
                            Account.Position = current;
                            user.route.Remove(current);
                        }
                    }
                    catch (Exception)
                    {
                    }
                }
            }
        }

        private static void CalculateCoordinatesNPC(ShipInitMessage ship)
        {
            if (ship != null)
            {
                double speed = ship.speed / help.SimplySpeed(ship.speed);
                if (ship.route != null && ship.route.Count > 0)
                {
                    try
                    {
                        if (ship.position.dX.Equals(0.0) && ship.position.dY.Equals(0.0))
                        {
                            ship.position.dX = (double)(ship.position.X);
                            ship.position.dY = (double)(ship.position.Y);
                        }
                        foreach (var position in ship.route)
                        {
                            if ((ship.route != null && ship.route.Count == 0) || ship.route == null)
                            {
                                break;
                            }
                            int newX = position.X - ship.position.X;
                            int newY = position.Y - ship.position.Y;
                            double distance = Math.Sqrt(Math.Pow((double)newX, 2.0) + Math.Pow((double)newY, 2.0));
                            if (distance > speed)
                            {
                                double duration = speed / distance;
                                ship.position.dX += duration * (double)newX;
                                ship.position.dY += duration * (double)newY;
                                ship.position.X = (int)ship.position.dX;
                                ship.position.Y = (int)ship.position.dY;
                                if (ship.position.X == position.X && ship.position.Y == position.Y && (ship.route.Contains(position)))
                                {
                                    ship.route.Remove(position);
                                }
                                break;
                            }
                            ship.position.dX = (double)position.X;
                            ship.position.dY = (double)position.Y;
                            ship.position = position;
                            if ((ship.route.Contains(position)))
                            {
                                ship.route.Remove(position);
                            }
                        }
                    }
                    catch (Exception)
                    {
                    }
                }
            }
        }

        private static void CalculateCoordinatesPlayer(PlayerInitMessage player)
        {
            if (player != null)
            {
                double speed = player.speed / help.SimplySpeed(player.speed);
                if (player.route != null && player.route.Count > 0)
                {
                    try
                    {
                        if (player.position.dX.Equals(0.0) && player.position.dY.Equals(0.0))
                        {
                            player.position.dX = (double)(player.position.X);
                            player.position.dY = (double)(player.position.Y);
                        }
                        foreach (var position in player.route)
                        {
                            if ((player.route != null && player.route.Count == 0) || player.route == null)
                            {
                                break;
                            }
                            int newX = position.X - player.position.X;
                            int newY = position.Y - player.position.Y;
                            double distance = Math.Sqrt(Math.Pow((double)newX, 2.0) + Math.Pow((double)newY, 2.0));
                            if (distance > speed)
                            {
                                double duration = speed / distance;
                                player.position.dX += duration * (double)newX;
                                player.position.dY += duration * (double)newY;
                                player.position.X = (int)player.position.dX;
                                player.position.Y = (int)player.position.dY;
                                if (player.position.X == position.X && player.position.Y == position.Y && (player.route.Contains(position)))
                                {
                                    player.route.Remove(position);
                                }
                                break;
                            }
                            player.position.dX = (double)position.X;
                            player.position.dY = (double)position.Y;
                            player.position = position;
                            if (player.route.Contains(position))
                            {
                                player.route.Remove(position);
                            }
                        }
                    }
                    catch (Exception)
                    {
                    }
                }
            }
        }

        public static double CalculateDistance(int x1, int y1, int x2, int y2)
        {
            return Math.Sqrt(Math.Pow(x2 - x1, 2.0) + Math.Pow(y2 - y1, 2.0));
        }

        public static double CalculateDistance(int x, int y)
        {
            return CalculateDistance(Account.Position.X, Account.Position.Y, x, y);
        }

        public static double CalculateDistance(Point point)
        {
            return CalculateDistance(point.X, point.Y);
        }

        public static double CalculateDistance(PositionStub position)
        {
            return CalculateDistance(position.X, position.Y);
        }

        public static PositionStub CalculateIslandMiddle(TowerInitMessage t)
        {
            var Position = t.position;
            switch (t.towerId)
            {
                case 1:
                    Position.X = Position.X + 5;
                    Position.Y = Position.Y + 10;
                    break;
                case 2:
                    Position.X = Position.X + 10;
                    Position.Y = Position.Y + 7;
                    break;
                case 3:
                    Position.X = Position.X + 15;
                    Position.Y = Position.Y + 10;
                    break;
                case 4:
                    Position.X = Position.X + 15;
                    Position.Y = Position.Y - 5;
                    break;
                case 5:
                    Position.X = Position.X + 10;
                    Position.Y = Position.Y - 10;
                    break;
                case 6:
                    Position.X = Position.X + 5;
                    Position.Y = Position.Y - 5;
                    break;
                case 7:
                    Position.X = Position.X - 10;
                    Position.Y = Position.Y - 10;
                    break;
                case 8:
                    Position.X = Position.X - 10;
                    Position.Y = Position.Y - 10;
                    break;
                case 9:
                    Position.X = Position.X - 15;
                    break;
                case 10:
                    Position.X = Position.X + 5;
                    Position.Y = Position.Y - 15;
                    break;
                case 11:
                    Position.X = Position.X + 10;
                    Position.Y = Position.Y - 15;
                    break;
                case 12:
                    Position.X = Position.X - 5;
                    Position.Y = Position.Y + 10;
                    break;
            }
            return Position;
        }

        public static bool CalculateChance(int percentage)
        {
            return new Random().Next(100) < percentage;
        }
    }
}
