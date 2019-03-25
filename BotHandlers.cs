using BoxyBot.Seafight;
using BoxyBot.Seafight.Messages;
using BoxyBot.Util;
using System;
using System.Collections.Generic;
using System.Linq;

namespace BoxyBot
{
    public static class BotHandlers
    {
        private static HelpTools help = new HelpTools();
        public static Dictionary<int, string> maps = new Dictionary<int, string>();

        public static string MessageHandler(string Message, List<string> Args)
        {
            int value = 0;
            try
            {
                switch (Message)
                {
                    case "warmap:boxelp":
                        value = help.ToInt(Args.First());
                        BotSession.Sessionelps += value;
                        return ($"Received {value}x ELP.");

                    case "warmap:boxplate":
                        value = help.ToInt(Args.First());
                        BotSession.Sessionplates += value;
                        return ($"Received {value}x Armor Plate(s).");

                    case "warmap:boxpwoder":
                        value = help.ToInt(Args.First());
                        BotSession.Sessionpowder += value;
                        return ($"Received {value}x Gun Powder.");

                    case "warmap:boxpearls":
                        value = help.ToInt(Args.First());
                        BotSession.Sessionpearls += value;
                        return ($"Received {value}x Pearl(s).");

                    case "warmap:boxadmiralpoints":
                        value = help.ToInt(Args.First());
                        return ($"Received {value}x Admiral Point(s).");

                    case "warmap:reward":
                        value = help.ToInt(Args.First());
                        if (Args.Contains("labels!ActionItems!1"))
                        {
                            BotSession.Sessionpowder += value;
                            return ($"Received {value}x Gun Powder.");
                        }
                        else
                        if (Args.Contains("labels!ActionItems!2"))
                        {
                            BotSession.Sessionplates += value;
                            return ($"Received {value}x Armor Plate(s).");
                        }
                        else
                        if (Args.Contains("labels!Currency!1"))
                        {
                            BotSession.Sessiongold += value;
                            return ($"Received {value}x Gold.");
                        }
                        else
                            if (Args.Contains("labels!Currency!2"))
                        {
                            BotSession.Sessionpearls += value;
                            return ($"Received {value}x Pearls.");
                        }
                        else
                            if (Args.Contains("labels!Currency!3"))
                        {
                            BotSession.Sessionmojos += value;
                            return ($"Received {value}x Mojo(s).");
                        }
                        else
                            if (Args.Contains("labels!Currency!4"))
                        {
                            BotSession.Sessioncrystals += value;
                            return ($"Received {value}x Crystal(s).");
                        }
                        else
                            if (Args.Contains("labels!Currency!8"))
                        {
                            BotSession.Sessioncursedsouls += value;
                            return ($"Received {value}x Cursed Soul(s).");
                        }
                        else
                            if (Args.Contains("labels!Currency!9"))
                        {
                            BotSession.Sessionradiantsouls += value;
                            return ($"Received {value}x Radiant Soul(s).");
                        }
                        else
                            if (Args.Contains("labels!Currency!13"))
                        {
                            BotSession.Sessioncrowns += value;
                            return ($"Received {value}x Crown(s).");
                        }
                        else
                            if (string.Join("", Args).Contains("labels!Harpoons"))
                        {
                            var fullArgs = string.Join("", Args);
                            if (fullArgs.Contains("76"))
                                return ($"Received {value}x Steel Harpoons.");
                            else
                            if (fullArgs.Contains("75"))
                                return ($"Received {value}x Iron Harpoons.");
                            else
                            if (fullArgs.Contains("20"))
                                return ($"Received {value}x Bronze Harpoons.");
                            else
                            if (fullArgs.Contains("19"))
                                return ($"Received {value}x Lead Harpoons.");
                            else
                            if (fullArgs.Contains("18"))
                                return ($"Received {value}x Copper Harpoons.");
                        }
                        else
                            if (string.Join("", Args).Contains("labels!Ammunition"))
                        {
                            return ($"Received {value}x Ammunition.");
                        }
                        return ($"Received {value}x Unkown Item(s).");
                    case "warmap:boxhitpoints":
                        value = help.ToInt(Args.First());
                        BotSession.Sessionhp += value;
                        return ($"Received {value}x HitPoint(s).");
                    case "warmap:boxtrap":
                        value = help.ToInt(Args.First());
                        BotSession.Sessionhp -= value;
                        return ($"- {value}x HitPoint(s).");
                    case "warmap:boxspeed":
                        value = help.ToInt(Args.First());
                        return ($"Received {value}x Speed Point(s).");
                    case "warmap:boxeuro":
                        value = help.ToInt(Args.First());
                        return ($"Received {value}x JackPot Dollar(s).");
                    case "warmap:boxep":
                        value = help.ToInt(Args.First());
                        BotSession.Sessionxp += value;
                        return ($"Received {value}x XP.");
                    case "warmap:boxmojo":
                        value = help.ToInt(Args.First());
                        BotSession.Sessionmojos += value;
                        return ($"Received {value}x Mojo(s).");
                    case "warmap:boxvoodoopoints":
                        value = help.ToInt(Args.First());
                        BotSession.Sessionhp += value;
                        return ($"Received {value}x Voodoopoint(s).");
                    case "warmap:notreasurechestkey":
                        if (!Bot.targetentityId.entityId.Equals(0.0) && Client.Entitys.ContainsKey(Bot.targetentityId.entityId))
                        {
                            var box = (Client.Entitys[Bot.targetentityId.entityId] as BoxInitMessage);
                            if (box.type == 5 || box.type == 15)
                            {
                                BotSettings.collectchests &= box.type != 5;
                                BotSettings.collecteventchests &= box.type != 15;
                            }
                            else
                            {
                                BotSettings.collectchests = false;
                                BotSettings.collecteventchests = false;
                            }
                            Bot.targetentityId = new Seafight.Messages.EntityInfo(0, 0);
                        }
                        return ("No Treasure Chest Keys left.");
                    case "raidmap_behemoth_start":
                        Account.Joining = true;
                        return ("Joining Raid Map!");
                    case "warmap:actionitemused2":
                        Account.Joining = true;
                        return ("Teleporting!");
                    case "warmap:actioncancelled":
                        if (Account.Joining)
                        {
                            Account.Joining = false;
                        }
                        return ("Join Canceld!");
                    case "warmap:actionitemDisabledBattle":
                        if (Account.Joining)
                        {
                            Account.Joining = false;
                            var point = Bot.MoveToCorner();
                            BotMethods.MoveTo(point.X, point.Y);
                        }
                        break;
                    case "warmap:actionitemDisabled":
                        if (Account.Joining)
                        {
                            Account.Joining = false;
                        }
                        break;
                    case "vodoo_scroll_start":
                        Account.Joining = true;
                        return ($"Joining {(BotSettings.lastBM.Replace("Map", "").Replace(" ", ""))} Bonus Map!");
                    case "vodoo_scroll_forbidden":
                        if (Account.Joining)
                        {
                            Account.Joining = false;
                            var map_ = Account.BonusMaps.Values.FirstOrDefault(bm => bm.mapName == BotSettings.lastBM);
                            if (map_ != null)
                            {
                                Account.BonusMaps.Remove(map_.mapId);
                            }
                        }
                        break;
                    case "vodoo_wave_destroy":
                        value = help.ToInt(Args.First());
                        BotSession.Sessionwaves++;
                        if (BotSettings.leaveBM)
                        {
                            BotSettings.canLeaveBM = true;
                        }
                        return ($"Bonus Map Wave {value} done!");
                    case "warmap:killvoodoo":
                        value = help.ToInt(Args.First());
                        BotSession.Sessionbmnpcs++;
                        return ($"NPC from attack wave {value} sunk!");
                    case "vodoo_wave_arrive":
                        value = help.ToInt(Args.First());
                        return ($"Bonus Map Wave {value} arrived!");
                    case "vodoo_wave_last":
                        BotSession.Sessionbms++;
                        return ("Bonus Map Finished!");
                    case "warmap:bonusmapreward":
                        value = help.ToInt(Args.First());
                        return ($"Received {MapHandler(value)}");
                    case "warmap:bonusmapwavepearls":
                        value = help.ToInt(Args.First());
                        BotSession.Sessionpearls += value;
                        return ($"Received {value}x Pearls.");
                    case "warmap:youropponentusersunk":
                        BotSettings.underAttack = false;
                        BotSession.Sessiondestroyedplayer++;
                        return ("Successfully destroyed Opponent.");
                    case "warmap:boardinghpthreshold":
                        if (!Bot.targetentityId.entityId.Equals(0.0) && Client.Entitys.ContainsKey(Bot.targetentityId.entityId) && Client.Entitys[Bot.targetentityId.entityId] is ShipInitMessage)
                        {
                            (Client.Entitys[Bot.targetentityId.entityId] as ShipInitMessage).boarded = false;
                        }
                        break;
                    case "warmap:boardingattacked":
                        if (!Bot.targetentityId.entityId.Equals(0.0) && Client.Entitys.ContainsKey(Bot.targetentityId.entityId) && Client.Entitys[Bot.targetentityId.entityId] is ShipInitMessage)
                        {
                            (Client.Entitys[Bot.targetentityId.entityId] as ShipInitMessage).boarded = true;
                            return ($"Boarded {(Client.Entitys[Bot.targetentityId.entityId] as ShipInitMessage).name}[{Bot.targetentityId.entityId}]");
                        }
                        break;
                    case "warmap:nocollectwithtortuga":
                        BotSettings.collectGlitters = false;
                        return ("Can NOT collect glitters while under Tortuga's light!");
                    case "warmap:pickuprestriction":
                        BotSettings.collectGlitters = false;
                        return "Can't collect glitters in this map";
                    case "warmap:nomapchange":
                        Account.NextMapID = -800;
                        return ("warmap:nomapchange");

                }
            }
            catch (Exception)
            {
            }
            return Message;
        }

        public static int AmmoHandler(string Ammo)
        {
            if (Ammo.Contains("Chain"))
                return 1;
            if (Ammo.Contains("Stones"))
                return 2;
            if (Ammo.Contains("Splinter"))
                return 3;
            if (Ammo.Contains("Fireballs"))
                return 4;
            if (Ammo.Contains("Hollows"))
                return 5;
            if (Ammo.Contains("Skull"))
                return 6;
            if (Ammo.Contains("Upgraded Shrapnel"))
                return 150;
            if (Ammo.Contains("Upgraded"))
                return 160;
            if (Ammo.Contains("Reinforced"))
                return 103;
            if (Ammo.Contains("Explosive"))
                return 51;
            if (Ammo.Contains("Shrapnel"))
                return 5000;
            if (Ammo.Contains("Pumpkin"))
                return 102;
            if (Ammo.Contains("Flares"))
                return 101;
            if (Ammo.Contains("Soccer"))
                return 104;
            if (Ammo.Contains("Phosphor"))
                return 180;
            if (Ammo.Contains("Snow"))
                return 100;
            if (Ammo.Contains("Burning"))
                return 184;
            if (Ammo.Contains("Ice"))
                return 182;
            if (Ammo.Contains("Heart"))
                return 189;
            if (Ammo.Contains("Shell"))
                return 190;
            if (Ammo.Contains("Kraken"))
                return 183;
            if (Ammo.Contains("Confetti"))
                return 170;
            if (Ammo.Contains("Scrap"))
                return 185;
            if (Ammo.Contains("Doom"))
                return 187;
            if (Ammo.Contains("Souleater"))
                return 188;
            if (Ammo.Contains("Blast"))
                return 191;
            if (Ammo.Contains("Voodoo"))
                return 186;

            return 5;
        }

        public static int HarpoonHandler(string Harpoon)
        {
            if (Harpoon.Contains("Copper"))
            {
                return 18;
            }
            if (Harpoon.Contains("Lead"))
            {
                return 19;
            }
            if (Harpoon.Contains("Bronze"))
            {
                return 20;
            }
            if (Harpoon.Contains("Iron"))
            {
                return 75;
            }
            if (Harpoon.Contains("Steel"))
            {
                return 76;
            }
            return 20;
        }

        public static string MapHandler(int MapID)
        {
            switch (MapID)
            {
                case 89:
                    return "Civil War Arena";
                case 90:
                    return "Elemental Armageddon";
                case 91:
                    return "Snowball Fight Arena";
                case 92:
                    return "Ghost Gantril's Graveyard";
                case 93:
                    return "Soccer Arena";
                case 94:
                    return "Bloodfin's Pool";
                case 95:
                    return "Nidhugsheim";
                case 96:
                    return "Death Arena";
                case 97:
                    return "Ancient Hotbed";
                case 98:
                    return "Hildisvini's Stable";
                case 99:
                    return "Kraken's Lair";
                case 100:
                    return "Green Map";
                case 101:
                    return "Red Map";
                case 102:
                    return "Blue Map";
                case 103:
                    return "Snowflake Map";
                case 104:
                    return "Winter Map";
                case 105:
                    return "Yellow Map";
                case 106:
                    return "Virgo Map";
                case 107:
                    return "Capricornus Map";
                case 108:
                    return "Sagittarius Map";
                case 109:
                    return "First white Map";
                case 110:
                    return "Second white Map";
                case 111:
                    return "Third white Map";
                case 112:
                    return "Cancer Map";
                case 113:
                    return "Leo Map";
                case 114:
                    return "Libra Map";
                case 115:
                    return "Taurus Map";
                case 116:
                    return "Aquarius Map";
                case 117:
                    return "Pumpkin Map";
                case 300:
                    return "Behemoth";
                case 301:
                    return "Sunpirates";
                case 302:
                    return "Commonwealth Raidmap";
                case 407:
                    return "Gantril's Lair";
                case 408:
                    return "Ladon's Lair";
                case 409:
                    return "Ogygia's Lair";
                case 410:
                    return "Hildisvini's Ice Cave";
                case 411:
                    return "Kraken's Lagoon";
                case 412:
                    return "Evil Snowman's Lair";
                case 500:
                    return "Nimbus";
                case 501:
                    return "Safeheaven";
                case 502:
                    return "Safeheaven";
                case 503:
                    return "Safeheaven";
                case 504:
                    return "Safeheaven";
                case 505:
                    return "Safeheaven";
                case 506:
                    return "Safeheaven";
                default:
                    return "Unknown Map";
            }
        }

        public static void loadMapNames()
        {
            var counter = 0;
            maps = new Dictionary<int, string>
            {
                { counter += 1, "1/1" },
                { counter += 1, "1/2" },
                { counter += 1, "1/3" },
                { counter += 1, "1/4" },
                { counter += 1, "2/1" },
                { counter += 1, "2/2" },
                { counter += 1, "2/3" },
                { counter += 1, "2/4" },
                { counter += 1, "3/1" },
                { counter += 1, "3/2" },
                { counter += 1, "3/3" },
                { counter += 1, "3/4" },
                { counter += 1, "4/1" },
                { counter += 1, "4/2" },
                { counter += 1, "4/3" },
                { counter += 1, "5/1" },
                { counter += 1, "5/2" },
                { counter += 1, "5/3" },
                { counter += 1, "6/1" },
                { counter += 1, "6/2" },
                { counter += 1, "6/3" },
                { counter += 1, "7/1" },
                { counter += 1, "7/2" },
                { counter += 1, "7/3" },
                { counter += 1, "7/4" },
                { counter += 1, "8/1" },
                { counter += 1, "8/2" },
                { counter += 1, "8/3" },
                { counter += 1, "8/4" },
                { counter += 1, "9/1" },
                { counter += 1, "9/2" },
                { counter += 1, "9/3" },
                { counter += 1, "9/4" },
                { counter += 1, "10/1" },
                { counter += 1, "10/2" },
                { counter += 1, "10/3" },
                { counter += 1, "11/1" },
                { counter += 1, "11/2" },
                { counter += 1, "12/1" },
                { counter += 1, "12/2" },
                { counter += 1, "13/1" },
                { counter += 1, "13/2" },
                { counter += 1, "14/1" },
                { counter += 1, "14/2" },
                { counter += 1, "15/1" },
                { counter += 1, "15/2" },
                { counter += 1, "16/1" },
                { counter += 1, "16/2" },
                { counter += 1, "17/1" },
                { 50, "18/1" },
                { 51, "19/1" },
                { 52, "20/1" },
                { 53, "21/1" },
                { 54, "21/2" },
                { 55, "22/1" },
                { 56, "22/2" },
                { 99, "krakenmap" },
                { 100, "unusedgreen" },
                { 101, "red" },
                { 102, "blue" },
                { 103, "birthday" },
                { 104, "winter" },
                { 105, "yellow" },
                { 106, "virgo" },
                { 107, "capricorn" },
                { 108, "sagittarius" },
                { 109, "white1" },
                { 110, "white2" },
                { 111, "white3" },
                { 112, "cancer" },
                { 113, "leo" },
                { 114, "libra" },
                { 115, "taurus" },
                { 116, "aquarius" },
                { 300, "behemoth" },
                { 301, "sunpirates" },
                { 302, "commonwealth raidmap" },
                { 401, "minigame" },
                { 402, "minigame" },
                { 403, "minigame" },
                { 404, "minigame" },
                { 405, "minigame" },
                { 406, "minigame" },
                { 500, "nimbus" },
                { 501, "safeheaven" },
                { 502, "safeheaven" },
                { 503, "safeheaven" },
                { 504, "safeheaven" },
                { 505, "safeheaven" },
                { 506, "safeheaven" }
            };
        }
    }
}
