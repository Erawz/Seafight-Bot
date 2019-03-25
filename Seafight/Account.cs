using BoxyBot.Seafight.Messages;
using System;
using System.Collections.Generic;

namespace BoxyBot.Seafight
{
    public static class Account
    {
        public static void SetAccount(double UserID)
        {
            Account.UserID = UserID;
        }

        public static void Reset()
        {
            Username = "";
            Guild = "";
            Map = "";
            _hp = 0;
            MaxHP = 0;
            VP = 0;
            MaxVP = 0;
            XP = 0;
            MaxXP = 0;
            MaxXP = 0;
            BP = 0;
            MaxBP = 0;
            MedallionId = 0;
            DesignId = 0;
            RepMaatType = 0;
            Speed = 0.0;
            ViewDistance = 0.0;
            CanonRange = 0.0;
            HarpoonRange = 10;
            ReloadTime = 0.0;
            BoardingRange = 10;
            BoardHPLimit = 0.0;
            BoardingAttackValue = 0.0;
            Premium = false;
            TreasureHunter = false;
            Repairing = false;
            Poisioned = false;
            NPCLeft = "null";
            SpawnQueue = "null";
            MapID = -1;
            Bot.AddItemUser();
        }

        public static UserInitMessage gClass = new UserInitMessage();

        public static double UserID { get; private set; }
        public static int ProjectID { get; set; }
        public static string Username { get; set; } = "";
        public static string Guild { get; set; } = "";
        public static string Map { get; set; } = "";
        public static int HP { get { return _hp; } set { Account._hp = value; HpChanged.Invoke(new object(), Account._hp); } }
        private static int _hp;
        public static int MaxHP { get; set; }
        public static int VP { get; set; }
        public static int MaxVP { get; set; }
        public static int Level { get; set; }
        public static int BP { get; set; }
        public static int MaxBP { get; set; }
        public static int MedallionId { get; set; }
        public static int DesignId { get; set; }
        public static int TargetX { get; set; }
        public static int TargetY { get; set; }
        public static int RepMaatType { get; set; }
        public static int CurrentAmmoID { get; set; }
        public static int CurrentHarpoonsID { get; set; }
        public static int NextMapID { get; set; } = -1;
        public static PositionStub Position { get; set; } = new PositionStub(0, 0);
        public static List<PositionStub> Route { get; set; } = new List<PositionStub>();
        public static double XP { get; set; }
        public static double MaxXP { get; set; }
        public static double Speed { get; set; }
        public static double ViewDistance { get; set; }
        public static double CanonRange { get; set; }
        public static double HarpoonRange { get; set; } = 10;
        public static double ReloadTime { get; set; } = -1;
        public static double BoardingRange { get; set; } = 10;
        public static double BoardHPLimit { get; set; } = 0;
        public static double BoardingAttackValue { get; set; }
        public static double ELP { get; set; } = 1;
        public static bool Repairing { get; set; }
        public static bool Joining { get; set; }
        public static bool JumpAvailable { get; set; }
        public static bool Premium { get; set; }
        public static bool TreasureHunter { get; set; }
        public static bool Poisioned { get; set; }
        public static bool RejoinCurrentMap { get; set; }
        public static bool IsRepMaatLevel5
        {
            get
            {
                return (RepMaatType == 2);
            }
        }
        public static bool OnRaid
        {
            get
            {
                return (MapID == 300 || MapID == 301 || MapID == 302);
            }
        }
        public static bool OnBM
        {
            get
            {
                return Account.BonusMaps.ContainsKey(Account.MapID);
            }
        }
        public static bool IsSunk
        {
            get
            {
                return (MapID == 500 || Account.HP <= 0);
            }
        }
        public static int RaidMedallion
        {
            get
            {
                return (Account.Level > 15 ? Account.Level > 25 && !BotSettings.joinBeheLvl26 ? 68 : 30 : 38);
            }
        }
        public static int GetCurrentHpPercent
        {
            get
            {
                return (int)((double)Account.HP / (double)Account.MaxHP * 100.0);
            }
        }
        public static int GetCurrentVpPercent
        {
            get
            {
                return (int)((double)Account.VP / (double)Account.MaxVP * 100.0);
            }
        }
        public static int GetRepAtHp
        {
            get
            {
                var result = BotSettings.repathp;
                if (Account.OnBM)
                {
                    result = BotSettings.repathpbm;
                }
                else if (Account.OnRaid)
                {
                    result = BotSettings.repathpraid;
                }
                return result;
            }
        }
        public static string NPCLeft { get; set; } = "null";
        public static string SpawnQueue { get; set; } = "null";
        public static int MapID { get; set; } = -1;
        public static int LastMapID { get; set; }
        public static Dictionary<int, BonusMapStub> BonusMaps { get; set; } = new Dictionary<int, BonusMapStub>();
        public static Dictionary<int, ActionItemStub> Items { get; set; } = new Dictionary<int, ActionItemStub>();

        public static int Gold { get; set; }
        public static int Pearls { get; set; }
        public static int Crystals { get; set; }
        public static int Mojos { get; set; }
        public static int CursedSouls { get; set; }
        public static int RadianSouls { get; set; }
        public static int Crowns { get; set; }
        public static int Keys { get; set; }
        public static int EventKeys { get; set; }
        public static Dictionary<int, InventoryItemStub> Ammo { get; set; } = new Dictionary<int, InventoryItemStub>();

        public static event EventHandler<int> HpChanged;
    }
}
