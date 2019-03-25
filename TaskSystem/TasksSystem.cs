using BoxyBot.Seafight;
using BoxyBot.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace BoxyBot.TaskSystem
{
    public static class TasksSystem
    {
        public static Dictionary<string, Task> Tasks { get; set; } = new Dictionary<string, Task>();
        public static bool LogginOut { get; private set; } = false;
        private static SettingsCopy settingsCopy;
        private static string currentID;
        private static System.Timers.Timer taskTimer;
        public static bool UseTaskSystem
        {
            get
            {
                return Tasks.Count > 0;
            }
        }

        public static void StartTaskSystem()
        {
            settingsCopy = new SettingsCopy();
            var startTask = Tasks.Values.ToArray()[0];
            currentID = startTask.id;
            ChangeTask(startTask.type);
            taskTimer = new System.Timers.Timer(60 * (startTask.duration * 1000));
            taskTimer.Elapsed += TaskTimer_Elapsed;
            taskTimer.Start();
            BotMethods.WriteLine("Starting Tasks.");
        }

        public static void StopTaskSystem()
        {
            if (taskTimer != null)
            {
                ChangeTask("null");
                taskTimer.Elapsed -= TaskTimer_Elapsed;
                taskTimer.Stop();
            }
            Tasks.Clear();
            BotMethods.WriteLine("Stopping Tasks.");
        }

        private static void startNewTask(string taskID)
        {
            taskTimer.Elapsed -= TaskTimer_Elapsed;
            taskTimer = new System.Timers.Timer(60 * (Tasks[taskID].duration * 1000));
            taskTimer.Elapsed += TaskTimer_Elapsed;
            taskTimer.Start();
        }

        public static void CheckTasks()
        {
            if (UseTaskSystem && Tasks.Count > 0 && Tasks.ContainsKey(currentID) && (Tasks[currentID].startTime - DateTime.Now).TotalSeconds > 0)
            {
                try
                {

                }
                catch (Exception)
                {

                }
            }
            else if ((Tasks[currentID].startTime - DateTime.Now).TotalSeconds <= 0 || !Tasks.ContainsKey(currentID) && Tasks.Count > 0)
            {
                var _newTask = Tasks.Values.First();
                currentID = Tasks.Keys.First();
                _newTask.startTime = DateTime.Now;
                ChangeTask(_newTask.type);
                if (_newTask.type == "bm" && Tasks.Count > 0 && Account.OnRaid)
                {
                    BotSettings.leaveBM = true;
                    BotSettings.autoJoinBM = false;
                }
                if (_newTask.type == "raid" && Tasks.Count > 0 && Account.OnBM)
                {
                    BotSettings.leaveBM = true;
                    BotSettings.autoJoinRaid = false;
                }
                BotMethods.WriteLine("Starting next Task.");
            }
        }

        private static void TaskTimer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            if (Tasks.Count >= 1)
            {
                var _task = Tasks[currentID];
                if (RemoveTask(currentID))
                {
                    var nextTask = Tasks.Values.ToArray()[0];
                    currentID = nextTask.id;
                    ChangeTask(nextTask.type);
                    if (_task.type == "bm" && Tasks.Count > 0)
                    {
                        BotSettings.leaveBM = true;
                        BotSettings.autoJoinBM = false;
                    }
                    if (_task.type == "raid" && Tasks.Count > 0)
                    {
                        BotSettings.leaveBM = true;
                        BotSettings.autoJoinRaid = false;
                    }
                    BotMethods.WriteLine("Finished Task. Starting " + Tasks[currentID].type + " Task.");
                    Thread.Sleep(100);
                }
            }
            else
            {
                StopTaskSystem();
                if (BotSettings.tasksDoneLogout)
                {
                    BotMethods.WriteLine("Finished all Tasks. Logging out.");
                    BotLogic.StopBotThread();
                    var point = Bot.MoveToCorner();
                    BotMethods.MoveTo(point.X, point.Y);
                    var counter = 60;
                    Thread.Sleep(555);
                    while (Account.Route.Count >= 3 && !Account.IsSunk && counter > 0)
                    {
                        if (point.X == Account.Position.X && point.Y == Account.Position.Y)
                        {
                            break;
                        }
                        Thread.Sleep(1000);
                        counter--;
                    }
                    Thread.Sleep(new Random().Next(175, 385));
                    BotMethods.Logout(0);
                    LogginOut = true;
                    while (Server.Connected)
                    {
                        Thread.Sleep(100);
                    }
                    BotCalculator.StopThreads();
                    BotMethods.WriteLine("Succesfully logged out.");
                    return;
                }
                BotMethods.WriteLine("Finished all Tasks. Returning to default Settings.");
            }
        }

        private static void ChangeTask(string taskType)
        {
            switch (taskType)
            {
                case TaskTypes.BOXES:
                    BotSettings.autoJoinBM = false;
                    BotSettings.autoJoinRaid = false;
                    BotSettings.collectchests = false;
                    BotSettings.collecteventchests = false;
                    BotSettings.collectEventGlitter = settingsCopy.collectEventGlitter;
                    BotSettings.collectGlitters = settingsCopy.collectGlitters;
                    BotSettings.collectMeat = false;
                    BotSettings.shootNPCs = false;
                    BotSettings.shootMonsters = false;
                    break;
                case TaskTypes.CHESTS:
                    BotSettings.autoJoinBM = false;
                    BotSettings.autoJoinRaid = false;
                    BotSettings.collectchests = settingsCopy.collectchests;
                    BotSettings.collecteventchests = settingsCopy.collecteventchests;
                    BotSettings.collectEventGlitter = false;
                    BotSettings.collectGlitters = false;
                    BotSettings.collectMeat = settingsCopy.collectMeat;
                    BotSettings.shootNPCs = false;
                    BotSettings.shootMonsters = false;
                    break;
                case TaskTypes.NPCS:
                    BotSettings.autoJoinBM = false;
                    BotSettings.autoJoinRaid = false;
                    BotSettings.collectchests = false;
                    BotSettings.collecteventchests = false;
                    BotSettings.collectEventGlitter = false;
                    BotSettings.collectGlitters = false;
                    BotSettings.collectMeat = false;
                    BotSettings.shootNPCs = true;
                    BotSettings.shootMonsters = false;
                    break;
                case TaskTypes.RAID:
                    BotSettings.autoJoinBM = false;
                    BotSettings.autoJoinRaid = true;
                    BotSettings.RaidType = (Account.Level > 16 ? Account.Level > 25 ? 3 : 2 : 1);
                    BotSettings.collectchests = false;
                    BotSettings.collecteventchests = false;
                    BotSettings.collectEventGlitter = false;
                    BotSettings.collectGlitters = false;
                    BotSettings.collectMeat = false;
                    BotSettings.shootNPCs = false;
                    BotSettings.shootMonsters = false;
                    break;
                case TaskTypes.BONUSMAP:
                    Account.LastMapID = 106;
                    BotSettings.autoJoinBM = true;
                    BotSettings.autoJoinRaid = false;
                    BotSettings.collectchests = false;
                    BotSettings.collecteventchests = false;
                    BotSettings.collectEventGlitter = false;
                    BotSettings.collectGlitters = false;
                    BotSettings.collectMeat = false;
                    BotSettings.shootNPCs = false;
                    BotSettings.shootMonsters = false;
                    break;
                case TaskTypes.MONSTERS:
                    BotSettings.autoJoinBM = false;
                    BotSettings.autoJoinRaid = false;
                    BotSettings.collectchests = false;
                    BotSettings.collecteventchests = false;
                    BotSettings.collectEventGlitter = false;
                    BotSettings.collectGlitters = false;
                    BotSettings.collectMeat = false;
                    BotSettings.shootNPCs = false;
                    BotSettings.shootMonsters = true;
                    break;
                case TaskTypes.NULL:
                    settingsCopy.ResetBotSettings();
                    break;
            }
        }

        public static void AddNewTask(string Type, int Duration)
        {
        IL_RESET:
            try
            {
                var hash = ((new Random().Next(9999) + new Random(Duration).NextDouble()).ToString() + new HelpTools().RandomString(5)).Replace(new HelpTools().RandomString(1), "");
                if (Tasks.ContainsKey(hash))
                {
                    goto IL_RESET;
                }
                Tasks.Add(hash, new Task(Type, hash, Duration));
            }
            catch (Exception ex)
            {
                BotMethods.WriteLine(ex.ToString());
            }
        }

        public static bool RemoveTask(string ID)
        {
            try
            {
                Tasks.Remove(ID);
            }
            catch (Exception)
            {
                return false;
            }
            return true;
        }
    }

    class SettingsCopy
    {
        public SettingsCopy()
        {
            autoJoinBM = BotSettings.autoJoinBM;
            autoJoinRaid = BotSettings.autoJoinRaid;
            collectchests = BotSettings.collectchests;
            collecteventchests = BotSettings.collecteventchests;
            collectEventGlitter = BotSettings.collectEventGlitter;
            collectGlitters = BotSettings.collectGlitters;
            collectMeat = BotSettings.collectMeat;
            shootNPCs = BotSettings.shootNPCs;
            shootMonsters = BotSettings.shootMonsters;
        }

        public void ResetBotSettings()
        {
            BotSettings.autoJoinBM = autoJoinBM;
            BotSettings.autoJoinRaid = autoJoinRaid;
            BotSettings.collectchests = collectchests;
            BotSettings.collecteventchests = collecteventchests;
            BotSettings.collectEventGlitter = collectEventGlitter;
            BotSettings.collectGlitters = collectGlitters;
            BotSettings.collectMeat = collectMeat;
            BotSettings.shootNPCs = shootNPCs;
            BotSettings.shootMonsters = shootMonsters;
        }

        public bool autoJoinBM = BotSettings.autoJoinBM;
        public bool autoJoinRaid = BotSettings.autoJoinRaid;
        public bool collectchests = BotSettings.collectchests;
        public bool collecteventchests = BotSettings.collecteventchests;
        public bool collectEventGlitter = BotSettings.collectEventGlitter;
        public bool collectGlitters = BotSettings.collectGlitters;
        public bool collectMeat = BotSettings.collectMeat;
        public bool shootNPCs = BotSettings.shootNPCs;
        public bool shootMonsters = BotSettings.shootMonsters;
    }
}
