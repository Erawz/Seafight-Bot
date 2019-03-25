using BoxyBot.Seafight;
using BoxyBot.Seafight.Messages;
using BoxyBot.Seafight.Constants;
using BoxyBot.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Drawing;
using System.Threading;

namespace BoxyBot
{
    public static class Client
    {
        private static HelpTools help = new HelpTools();

        public static Dictionary<double, Message> Entitys = new Dictionary<double, Message>();
        public static Dictionary<double, List<PositionStub>> EntityMovementList = new Dictionary<double, List<PositionStub>>();
        public static Dictionary<double, Message> EntityAttackingList = new Dictionary<double, Message>();

        public static DateTime lastPacket;

        public static void Out(byte[] buffer)
        {
            Reader reader = new Reader(0, buffer);
            while (reader.Index < buffer.Length - 2)
            {
                int Length = reader.ReadShort();
                if (Length > 0)
                {
                    int ID = reader.ReadShort();
                    BotMethods.WriteLine("OUT: " + ID.ToString());
                    if (Length > 2)
                        reader.Index += buffer.Length - 2;
                    else
                        reader.Index += 2;
                }
            }
        }

        public static void Read(byte[] buffer)
        {
            if (buffer.Length != 0)
            {
                Client.lastPacket = DateTime.Now;
            }
            Reader reader = new Reader(0, buffer);
            while (reader.Index < buffer.Length - 2)
            {
                try
                {
                    var _length = reader.ReadShort();
                    if (_length > 0)
                    {
                        var _id = reader.ReadShort();
                        if (_id == ClientInitMessage.ID)
                        {
                            ClientInitMessage clientInit = new ClientInitMessage(reader);
                            Account.Map = clientInit.mapInfo.map;
                            Account.MapID = clientInit.mapInfo.mapId;
                        }
                        else if (_id == UserInitMessage.ID)
                        {
                            UserInitMessage userInit = new UserInitMessage(reader);
                            Account.HP = userInit.currentHealth.hitpoints;
                            Account.VP = userInit.currentHealth.voodoopoints;
                            Account.XP = userInit.xp;
                            Account.MaxXP = userInit.maxXp;
                            Account.BP = userInit.bp;
                            Account.MaxBP = userInit.maxBp;
                            Account.Position = userInit.position;
                            Account.Username = userInit.username;
                            Account.Guild = userInit.guild;
                            Account.ProjectID = userInit.entityInfo.projectId;
                            Account.MedallionId = userInit.medallionId;
                            Account.TargetX = userInit.position.X;
                            Account.TargetY = userInit.position.Y;
                            Account.DesignId = userInit.designId;
                            Account.RepMaatType = userInit.carpenterType;
                            Account.Premium = userInit.hasPremium;
                            Account.TreasureHunter = userInit.hasTreasureHunter;
                            Account.Repairing = userInit.isRepairing;
                            if (Account.gClass != null)
                            {
                                Account.gClass = userInit;
                            }
                            Form1.form1.updateFormText = true;
                        }
                        else if (_id == BonusMapMessage.ID)
                        {
                            BonusMapMessage bonusMaps = new BonusMapMessage(reader);
                            Account.BonusMaps.Clear();
                            foreach (var item in bonusMaps.maps)
                            {
                                item.mapName = BotHandlers.MapHandler(item.mapId);
                                if (!Account.BonusMaps.ContainsKey(item.mapId))
                                {
                                    Account.BonusMaps.Add(item.mapId, item);
                                }
                                else
                                {
                                    Account.BonusMaps[item.mapId] = item;
                                }
                            }
                            Form1.form1.bonusmap();
                        }
                        else if (_id == ActionItemInitMessage.ID)
                        {
                            ActionItemInitMessage actionItemInit = new ActionItemInitMessage(reader);
                            foreach (var item in actionItemInit.items)
                            {
                                if (!Account.Items.ContainsKey(item.itemId))
                                {
                                    Account.Items.Add(item.itemId, item);
                                }
                                else
                                {
                                    Account.Items[item.itemId] = item;
                                }
                            }
                        }
                        else if (_id == ActionItemUpdateMessage.ID)
                        {
                            ActionItemUpdateMessage actionItemUpdate = new ActionItemUpdateMessage(reader);
                            if (Account.Items.ContainsKey(actionItemUpdate.item.itemId) && Account.Items[actionItemUpdate.item.itemId] is ActionItemStub)
                            {
                                var item = (ActionItemStub)Account.Items[actionItemUpdate.item.itemId];
                                item.active = actionItemUpdate.item.active;
                                item.amount = actionItemUpdate.item.amount;
                            }
                            else
                            {
                                if (!Account.Items.ContainsKey(actionItemUpdate.item.itemId))
                                    Account.Items.Add(actionItemUpdate.item.itemId, actionItemUpdate.item);
                            }
                        }
                        else if (_id == InventoryMessage.ID)
                        {
                            InventoryMessage inventory = new InventoryMessage(reader);
                            foreach (var item in inventory.items)
                            {
                                if (inventory.type == 4)
                                {
                                    if (item.key == 1)
                                    {
                                        Account.Gold = (int)item.value;
                                    }
                                    else if (item.key == 2)
                                    {
                                        Account.Pearls = (int)item.value;
                                    }
                                    else if (item.key == 3)
                                    {
                                        Account.Mojos = (int)item.value;
                                    }
                                    else if (item.key == 4)
                                    {
                                        Account.Crystals = (int)item.value;
                                    }
                                    else if (item.key == 8)
                                    {
                                        Account.CursedSouls = (int)item.value;
                                    }
                                    else if (item.key == 9)
                                    {
                                        Account.RadianSouls = (int)item.value;
                                    }
                                    else if (item.key == 13)
                                    {
                                        Account.Crowns = (int)item.value;
                                    }
                                }
                                else if (inventory.type == 9 || inventory.type == 2)
                                {
                                    if (!Account.Ammo.ContainsKey(item.key))
                                    {
                                        Account.Ammo.Add(item.key, item);
                                    }
                                    else
                                    {
                                        Account.Ammo[item.key] = item;
                                    }
                                }
                                else if (inventory.type == 10)
                                {
                                    if (item.key == 77)
                                    {
                                        Account.Keys = (int)item.value;
                                    }
                                    else if (item.key == 86)
                                    {
                                        Account.EventKeys = (int)item.value;
                                    }
                                }
                            }
                        }
                        else if (_id == MonsterInitMessage.ID)
                        {
                            MonsterInitMessage monsterInit = new MonsterInitMessage(reader);
                            if (monsterInit.nameId == 33 || monsterInit.nameId == 34 || monsterInit.nameId == 38 || monsterInit.nameId == 39 || monsterInit.nameId == 40 || monsterInit.nameId == 41 || monsterInit.nameId == 52 || monsterInit.nameId == 53)
                            {
                                monsterInit.nameId = 26;
                            }
                            if (monsterInit.nameId == 30)
                            {
                                monsterInit.nameId = 35;
                            }
                            if (Bot.Monsters.ContainsKey(monsterInit.nameId))
                            {
                                monsterInit.name = Bot.Monsters[monsterInit.nameId];
                            }
                            if (!Entitys.ContainsKey(monsterInit.entityInfo.entityId))
                            {
                                Entitys.Add(monsterInit.entityInfo.entityId, monsterInit);
                            }
                            else
                            {
                                Entitys[monsterInit.entityInfo.entityId] = monsterInit;
                            }
                        }
                        else if (_id == BoxInitMessage.ID)
                        {
                            BoxInitMessage boxInit = new BoxInitMessage(reader);
                            if (!Entitys.ContainsKey(boxInit.entityId))
                            {
                                Entitys.Add(boxInit.entityId, boxInit);
                            }
                            else
                            {
                                Entitys[boxInit.entityId] = boxInit;
                            }
                        }
                        else if (_id == ShipInitMessage.ID)
                        {
                            ShipInitMessage shipInit = new ShipInitMessage(reader);
                            if (Bot.NPCs.ContainsKey(shipInit.nameId))
                            {
                                shipInit.name = Bot.NPCs[shipInit.nameId];
                            }
                            if (!Entitys.ContainsKey(shipInit.entityInfo.entityId))
                            {
                                Entitys.Add(shipInit.entityInfo.entityId, shipInit);
                            }
                            else
                            {
                                Entitys[shipInit.entityInfo.entityId] = shipInit;
                            }
                        }
                        else if (_id == PlayerInitMessage.ID)
                        {
                            PlayerInitMessage playerInit = new PlayerInitMessage(reader);
                            if (!Entitys.ContainsKey(playerInit.entityInfo.entityId))
                            {
                                Entitys.Add(playerInit.entityInfo.entityId, playerInit);
                            }
                            else
                            {
                                Entitys[playerInit.entityInfo.entityId] = playerInit;
                            }
                            if (BotSettings.attackOnSight && BotSettings.ShootBackType >= 2 && Bot.Running && !BotLogic.OnAttackRunning)
                            {
                                if (BotSettings.attackOnSightShips.ContainsKey(playerInit.entityInfo.entityId))
                                {
                                    Bot.underAttackBy = playerInit.entityInfo;
                                    BotSettings.underAttack = true;
                                    BotMethods.WriteLine("Spotted Listed UID!");
                                }
                                if (BotSettings.attackOnSightGuilds.ContainsKey(playerInit.guild))
                                {
                                    Bot.underAttackBy = playerInit.entityInfo;
                                    BotSettings.underAttack = true;
                                    BotMethods.WriteLine("Spotted Listed Guild Member!");
                                }
                                if (!BotLogic.OnAttackRunning && Bot.Running)
                                {
                                    BotLogic.StartonAttackThread();
                                }
                            }
                        }
                        else if (_id == TowerInitMessage.ID)
                        {
                            TowerInitMessage towerInit = new TowerInitMessage(reader);
                            if (!Entitys.ContainsKey(towerInit.entityInfo.entityId))
                            {
                                Entitys.Add(towerInit.entityInfo.entityId, towerInit);
                            }
                            else
                            {
                                Entitys[towerInit.entityInfo.entityId] = towerInit;
                            }
                            if (Bot.userGuildTower == null)
                            {
                                Bot.userGuildTower = towerInit;
                                BotMethods.WriteLine("Found Guild Island.");
                            }
                        }
                        else if (_id == AmsUpdateMessage.ID)
                        {
                            AmsUpdateMessage amsUpdate = new AmsUpdateMessage(reader);
                            if (amsUpdate.entityId.Equals(Account.UserID))
                            {
                                foreach (AmsUpdateValue update in amsUpdate.amsUpdate)
                                {
                                    if (update.key == (int)AmsTypes.CannonRange)
                                    { //range
                                        Account.CanonRange = update.value;
                                    }
                                    else
                                        if (update.key == (int)AmsTypes.CannonReloadTime)
                                    { //reloadtime
                                        Account.ReloadTime = update.value;
                                    }
                                    else
                                        if (update.key == (int)AmsTypes.HarpoonRange)
                                    {
                                        Account.HarpoonRange = update.value;
                                    }
                                    else
                                        if (update.key == (int)AmsTypes.Hitpoints)
                                    { //HP
                                        if (amsUpdate.space == 1)
                                            Account.MaxHP = (int)update.value;
                                        if (amsUpdate.space == 0)
                                            Account.HP = (int)update.value;
                                    }
                                    else
                                        if (update.key == (int)AmsTypes.Speed)
                                    { //Speed
                                        Account.Speed = update.value / 100;
                                    }
                                    else
                                        if (update.key == (int)AmsTypes.ViewDistance)
                                    { //VD
                                        Account.ViewDistance = update.value;
                                    }
                                    else
                                        if (update.key == (int)AmsTypes.VoodooPoints)
                                    { //VP
                                        if (amsUpdate.space == 1)
                                            Account.MaxVP = (int)update.value;
                                        if (amsUpdate.space == 0)
                                            Account.VP = (int)update.value;
                                    }
                                    else
                                        if (update.key == (int)AmsTypes.BoardingThreshold)
                                    {
                                        Account.BoardHPLimit = update.value;
                                    }
                                    else
                                        if (update.key == (int)AmsTypes.BoardingAttackCombatValue)
                                    {
                                        Account.BoardingAttackValue = update.value;
                                    }
                                    else
                                        if (update.key == (int)AmsTypes.ElitePoints)
                                    {
                                        Account.ELP = update.value;
                                    }
                                }
                            }
                            else
                            {
                                if (Entitys.ContainsKey(amsUpdate.entityId))
                                {
                                    foreach (AmsUpdateValue update in amsUpdate.amsUpdate)
                                    {
                                        if (Client.Entitys[amsUpdate.entityId] is ShipInitMessage)
                                        {
                                            var ship = (ShipInitMessage)Entitys[amsUpdate.entityId];
                                            if (update.key == (int)AmsTypes.Hitpoints)
                                            { //HP
                                                if (amsUpdate.space == 1)
                                                    ship.pointsMax.hitpoints = (int)update.value;
                                                if (amsUpdate.space == 0)
                                                    ship.pointsCurrent.hitpoints = (int)update.value;
                                            }
                                            else
                                            if (update.key == (int)AmsTypes.Speed)
                                            {//Speed
                                                ship.speed = update.value / 100;
                                            }
                                            else
                                            if (update.key == (int)AmsTypes.VoodooPoints)
                                            {//VP
                                                if (amsUpdate.space == 1)
                                                    ship.pointsMax.voodoopoints = (int)update.value;
                                                if (amsUpdate.space == 0)
                                                    ship.pointsCurrent.voodoopoints = (int)update.value;
                                            }
                                            else
                                            if (update.key == (int)AmsTypes.BoardingAttackCombatValue)
                                            {
                                                ship.boardinAttackValue = update.value;
                                            }
                                        }
                                        if (Client.Entitys[amsUpdate.entityId] is MonsterInitMessage)
                                        {
                                            var monster = (MonsterInitMessage)Entitys[amsUpdate.entityId];
                                            if (update.key == (int)AmsTypes.Hitpoints)
                                            {
                                                if (amsUpdate.space == 1)
                                                    monster.maxHitpoints = (int)update.value;
                                                if (amsUpdate.space == 0)
                                                    monster.hitpoints = (int)update.value;
                                            }
                                        }
                                        if (Client.Entitys[amsUpdate.entityId] is PlayerInitMessage)
                                        {
                                            var player = (PlayerInitMessage)Entitys[amsUpdate.entityId];
                                            if (update.key == (int)AmsTypes.Hitpoints)
                                            {
                                                if (update.key == (int)AmsTypes.Hitpoints)
                                                { //HP
                                                    if (amsUpdate.space == 1)
                                                        player.pointsMax.hitpoints = (int)update.value;
                                                    if (amsUpdate.space == 0)
                                                        player.pointsCurrent.hitpoints = (int)update.value;
                                                }
                                                else
                                                if (update.key == (int)AmsTypes.Speed)
                                                {//Speed
                                                    player.speed = update.value / 100;
                                                }
                                                else
                                                if (update.key == (int)AmsTypes.VoodooPoints)
                                                {//VP
                                                    if (amsUpdate.space == 1)
                                                        player.pointsMax.voodoopoints = (int)update.value;
                                                    if (amsUpdate.space == 0)
                                                        player.pointsCurrent.voodoopoints = (int)update.value;
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                        else if (_id == LoginBonusAvailableMessage.ID)
                        {
                            reader.Index += _length - 2;
                            if (BotSettings.acceptLoginBonus && !BotSession.Sessionacceptedloginbonus)
                            {
                                BotMethods.AcceptLoginBonus();
                                BotSession.Sessionacceptedloginbonus = true;
                                BotMethods.WriteLine("Accepting login bonus.");
                            }
                        }
                        else if (_id == RouteMessage.ID)
                        {
                            RouteMessage route = new RouteMessage(reader);
                            if (route.entityId.Equals(Account.UserID))
                            {
                                if (Account.MapID == 500)
                                {
                                    Account.MapID = 0;
                                }
                                Account.Route.Clear();
                                if (route.route.Count > 0)
                                {
                                    Account.TargetX = route.route.Last().X;
                                    Account.TargetY = route.route.Last().Y;
                                }
                                Account.Route = route.route;
                            }
                            else
                            {
                                if (Entitys.ContainsKey(route.entityId))
                                {
                                    if (Entitys[route.entityId] is ShipInitMessage)
                                    {
                                        var ship = (ShipInitMessage)Entitys[route.entityId];
                                        ship.route.Clear();
                                        ship.route = route.route;
                                    }
                                    if (Entitys[route.entityId] is PlayerInitMessage)
                                    {
                                        var player = (PlayerInitMessage)Entitys[route.entityId];
                                        player.route.Clear();
                                        player.route = route.route;
                                    }
                                }
                            }
                            if (EntityMovementList.ContainsKey(route.entityId))
                            {
                                EntityMovementList[route.entityId] = route.route;
                            }
                            else
                            {
                                EntityMovementList.Add(route.entityId, route.route);
                            }
                        }
                        else if (_id == StopMessage.ID)
                        {
                            StopMessage stopShip = new StopMessage(reader);
                            if (stopShip.entityId.Equals(Account.UserID))
                            {
                                Account.Position.dX = (double)stopShip.position.X;
                                Account.Position.dY = (double)stopShip.position.Y;
                                Account.Position = stopShip.position;
                            }
                            else
                            {
                                if (Entitys.ContainsKey(stopShip.entityId))
                                {
                                    if (Entitys[stopShip.entityId] is ShipInitMessage)
                                    {
                                        var ship = (ShipInitMessage)Entitys[stopShip.entityId];
                                        ship.route.Clear();
                                        ship.position.dX = (double)stopShip.position.X;
                                        ship.position.dY = (double)stopShip.position.Y;
                                        ship.position = stopShip.position;
                                        if (!Client.EntityMovementList.ContainsKey(ship.entityInfo.entityId))
                                        {
                                            Client.EntityMovementList.Add(ship.entityInfo.entityId, new List<PositionStub>() { ship.position });
                                        }
                                    }
                                    if (Entitys[stopShip.entityId] is PlayerInitMessage)
                                    {
                                        var player = (PlayerInitMessage)Entitys[stopShip.entityId];
                                        player.position.dX = (double)stopShip.position.X;
                                        player.position.dY = (double)stopShip.position.Y;
                                        player.position = stopShip.position;
                                    }
                                }
                            }
                        }
                        else if (_id == DisplayHitMessage.ID)
                        {
                            DisplayHitMessage displayHit = new DisplayHitMessage(reader);
                            if (displayHit.attacker.entityId.Equals(Account.UserID))
                            {
                                BotLogic.attacking = 15;
                                if (Entitys.ContainsKey(displayHit.defender.entityId) && Entitys[displayHit.defender.entityId] is ShipInitMessage)
                                {
                                    var ship = (ShipInitMessage)Entitys[displayHit.defender.entityId];
                                    BotMethods.WriteLine(string.Concat(new object[]
                                    {
                                    "Attacked ",
                                    ship.name,
                                    "[",
                                    ship.entityInfo.entityId,
                                    "] by ",
                                    "[",
                                    Account.Guild,
                                    "] ",
                                    Account.Username,
                                    " - ",
                                    "HP: ",
                                    ship.pointsCurrent.hitpoints > displayHit.damage.hitpoints ? ship.pointsCurrent.hitpoints - displayHit.damage.hitpoints : 0,
                                    "/",
                                    ship.pointsMax.hitpoints,
                                    " - Damage: ",
                                    displayHit.damage.hitpoints + (displayHit.damage.voodoopoints > 0 ? displayHit.damage.voodoopoints : 0),
                                    }));
                                    BotSession.Canondamage = (displayHit.damage.voodoopoints + displayHit.damage.hitpoints);
                                    if (ship.pointsCurrent.hitpoints + ship.pointsCurrent.voodoopoints <= displayHit.damage.voodoopoints + displayHit.damage.hitpoints)
                                    {
                                        Bot.targetentityId.entityId = 0.0;
                                    }
                                }
                                else if (Entitys.ContainsKey(displayHit.defender.entityId) && Entitys[displayHit.defender.entityId] is MonsterInitMessage)
                                {
                                    var monster = (MonsterInitMessage)Entitys[displayHit.defender.entityId];
                                    BotMethods.WriteLine(string.Concat(new object[]
                                    {
                                    "Attacked ",
                                    monster.name,
                                    "[",
                                    monster.entityInfo.entityId,
                                    "] by ",
                                    "[",
                                    Account.Guild,
                                    "] ",
                                    Account.Username,
                                    " - ",
                                    "HP: ",
                                    monster.hitpoints - displayHit.damage.hitpoints,
                                    "/",
                                    monster.maxHitpoints,
                                    " - Damage: ",
                                    displayHit.damage.hitpoints
                                    }));
                                    BotSession.Sessionharpoondamage = displayHit.damage.hitpoints;
                                    if (monster.hitpoints <= displayHit.damage.hitpoints)
                                    {
                                        Bot.targetentityId.entityId = 0.0;
                                    }
                                }
                                else if (Entitys.ContainsKey(displayHit.defender.entityId) && Entitys[displayHit.defender.entityId] is PlayerInitMessage)
                                {
                                    var player = (PlayerInitMessage)Entitys[displayHit.defender.entityId];
                                    BotMethods.WriteLine(string.Concat(new object[]
                                    {
                                    "Attacked ",
                                    player.guild.Length > 0 ? "[" + player.guild + "] " : "",
                                    player.username,
                                    "[",
                                    player.entityInfo.entityId,
                                    "] by ",
                                    "[",
                                    Account.Guild,
                                    "] ",
                                    Account.Username,
                                    " - ",
                                    "HP: ",
                                    player.pointsCurrent.hitpoints > displayHit.damage.hitpoints ? player.pointsCurrent.hitpoints - displayHit.damage.hitpoints : 0,
                                    "/",
                                    player.pointsMax.hitpoints,
                                    " - VP: ",
                                    player.pointsCurrent.voodoopoints > 0 ? player.pointsCurrent.voodoopoints - displayHit.damage.voodoopoints : 0,
                                    "/",
                                    player.pointsMax.voodoopoints,
                                    " - Damage: ",
                                    displayHit.damage.hitpoints + displayHit.damage.voodoopoints
                                    }));
                                    BotSession.Sessioncanondamage = (displayHit.damage.hitpoints + (displayHit.damage.voodoopoints > 0 ? displayHit.damage.voodoopoints : 0));
                                    if (player.pointsCurrent.hitpoints + player.pointsCurrent.voodoopoints <= displayHit.damage.hitpoints + displayHit.damage.voodoopoints)
                                    {
                                        Bot.underAttackBy = new EntityInfo(0, 0);
                                    }
                                }
                                if (BotSession.attackRetrys.ContainsKey(displayHit.defender.entityId))
                                {
                                    BotSession.attackRetrys.Remove(displayHit.defender.entityId);
                                }
                            }
                            if (displayHit.defender.entityId.Equals(Account.UserID))
                            {
                                if (Entitys.ContainsKey(displayHit.attacker.entityId) && Entitys[displayHit.attacker.entityId] is PlayerInitMessage)
                                {
                                    var player = (PlayerInitMessage)Entitys[displayHit.attacker.entityId];
                                    if (displayHit.animationId != 4)
                                    {
                                        BotLogic.attacking = 15;
                                        BotMethods.WriteLine(string.Concat(new object[]
                                        {
                                        "Under Attack by ",
                                        player.guild.Length > 0 ? "[" + player.guild + "] " : "",
                                        player.username,
                                        "[",
                                        player.entityInfo.entityId,
                                        "] - ",
                                        "Damage: ",
                                        displayHit.damage.voodoopoints + displayHit.damage.hitpoints
                                        }));
                                    }
                                    else
                                    {
                                        BotMethods.WriteLine(string.Concat(new object[]
                                        {
                                        "Healed by ",
                                        player.guild.Length > 0 ? "[" + player.guild + "] " : "",
                                        player.username,
                                        "[",
                                        player.entityInfo.entityId,
                                        "] - ",
                                        "Amount: ",
                                        displayHit.damage.hitpoints
                                        }));
                                    }
                                }
                                if (!Entitys.ContainsKey(displayHit.attacker.entityId) && Account.OnBM)
                                {
                                    var position = new PositionStub(0, 0);
                                    var route = new List<PositionStub>();
                                    if (EntityMovementList.ContainsKey(displayHit.attacker.entityId))
                                    {
                                        route = (List<PositionStub>)EntityMovementList[displayHit.attacker.entityId];
                                        position = route.FirstOrDefault();
                                    }
                                    var ship = new ShipInitMessage(displayHit.attacker.entityId, displayHit.attacker.projectId, -1, 10000, 10000, position, route);
                                    ship.name = "Bonusmap-NPC";
                                    Client.Entitys.Add(displayHit.attacker.entityId, ship);
                                    if (Account.OnBM)
                                    {
                                        var point = Bot.MoveToFarestCorner();
                                        BotMethods.MoveTo(point.X, point.Y);
                                    }
                                }
                                else if (BotSession.attackRetrys.ContainsKey(displayHit.attacker.entityId))
                                {
                                    BotSession.attackRetrys.Remove(displayHit.attacker.entityId);
                                }
                                if (!Bot.targetentityId.entityId.Equals(displayHit.attacker.entityId) && displayHit.animationId != 271 && Account.IsSunk && !Account.Repairing && !Account.OnBM && !BotLogic.OnAttackRunning)
                                {
                                    Bot.targetentityId = displayHit.attacker;
                                    BotLogic.StartRepairThread();
                                }
                                if (Account.OnBM && Account.GetCurrentHpPercent < BotSettings.repathpbm && !Account.Repairing && !BotLogic.OnAttackRunning)
                                {
                                    Bot.underAttackBy = displayHit.attacker;
                                    BotLogic.StartRepairThread();
                                }
                                if (BotSettings.ShootBackType == 3 && Client.Entitys.ContainsKey(displayHit.attacker.entityId) && !BotLogic.OnAttackRunning && Bot.Running && Client.Entitys[displayHit.attacker.entityId] is PlayerInitMessage)
                                {
                                    var player = (PlayerInitMessage)Client.Entitys[displayHit.attacker.entityId];
                                    if (player.guild == Account.Guild)
                                    {
                                        goto IL_CHANGE_BACK;
                                    }
                                    BotSession.Sessionattackedbyplayer++;
                                    if ((player.pointsCurrent.hitpoints < Account.HP || displayHit.damage.hitpoints + displayHit.damage.voodoopoints < BotSession.Canondamage) || (player.pointsMax.hitpoints + player.pointsMax.voodoopoints < Account.MaxHP + Account.MaxVP && player.pointsCurrent.hitpoints <= Account.HP))
                                    {
                                        Bot.underAttackBy = player.entityInfo;
                                        BotSettings.underAttack = true;
                                        BotSettings.ShootBackType = 2;
                                        BotLogic.StartonAttackThread();
                                        BotMethods.WriteLine("Enemy is weak enough Shooting back!");
                                        Thread.Sleep(600);
                                        goto IL_CHANGE_BACK;
                                    }
                                    if (player.speed * 100 < (Account.Speed * 100 - ((Account.Speed * 100) * 0.25)) && Account.GetCurrentHpPercent > 20)
                                    {
                                        Bot.underAttackBy = player.entityInfo;
                                        BotSettings.underAttack = true;
                                        BotSettings.ShootBackType = 1;
                                        BotMethods.WriteLine("Enemy is to strong but not as fast Fleeing!");
                                        BotLogic.StartonAttackThread();
                                        Thread.Sleep(600);
                                        goto IL_CHANGE_BACK;
                                    }
                                    if (Account.GetCurrentHpPercent < 20 && player.pointsCurrent.hitpoints < BotSession.Canondamage)
                                    {
                                        Bot.underAttackBy = player.entityInfo;
                                        BotSettings.underAttack = true;
                                        BotSettings.ShootBackType = 2;
                                        BotLogic.StartonAttackThread();
                                        BotMethods.WriteLine("Enemy has low Hitpoints Shooting back!");
                                        Thread.Sleep(600);
                                        goto IL_CHANGE_BACK;
                                    }
                                    Bot.underAttackBy = new EntityInfo(0.0, 0);
                                    BotSettings.underAttack = false;
                                    BotMethods.WriteLine("Enemy is too strong and too fast Ignoring!");
                                IL_CHANGE_BACK:
                                    BotSettings.ShootBackType = 3;
                                    goto IL_SKIP_ATTACK;
                                }
                                if (Client.Entitys.ContainsKey(displayHit.attacker.entityId) && !BotLogic.OnAttackRunning && Bot.Running && ((BotSettings.ShootBackType > 0 && BotSettings.ShootBackType < 3 && !(Client.Entitys[displayHit.attacker.entityId] is TowerInitMessage)) || BotSettings.avoidIslands) && ((Client.Entitys[displayHit.attacker.entityId] is TowerInitMessage) || (Client.Entitys[displayHit.attacker.entityId] is PlayerInitMessage)))
                                {
                                    Bot.underAttackBy = new EntityInfo(0.0, 0);
                                    Bot.underAttackBy = displayHit.attacker;
                                    if (Client.Entitys[displayHit.attacker.entityId] is TowerInitMessage && BotSettings.avoidIslands)
                                    {
                                        BotSettings.escapingIsland = true;
                                        BotSession.Sessionescapedisland++;
                                        BotLogic.StartonAttackThread();
                                    }
                                    if (Client.Entitys[displayHit.attacker.entityId] is PlayerInitMessage && BotSettings.ShootBackType > 0)
                                    {
                                        BotSettings.underAttack = true;
                                        BotSession.Sessionattackedbyplayer++;
                                        BotLogic.StartonAttackThread();
                                    }
                                }
                            IL_SKIP_ATTACK:
                                if (BotSettings.avoidBeheNPCs && Account.MapID == 300 && !displayHit.attacker.entityId.Equals(Bot.targetentityId.entityId) && Client.Entitys.ContainsKey(displayHit.attacker.entityId) && Account.Route.Count < 2 && BotLogic.attacking > 2 && Client.Entitys[displayHit.attacker.entityId] is ShipInitMessage)
                                {
                                    if (!Client.EntityAttackingList.ContainsKey(displayHit.attacker.entityId))
                                    {
                                        Client.EntityAttackingList.Add(displayHit.attacker.entityId, Client.Entitys[displayHit.attacker.entityId]);
                                    }
                                    if (Client.EntityAttackingList.Count >= (Client.Entitys.ToList().OfType<ShipInitMessage>().ToList().Count - 2))
                                    {
                                        var distance = 0.0;
                                        var closerDistance = 6000.0;
                                        var position = new Point(0, 0);
                                        foreach (var _ship in Client.EntityAttackingList.Values.OfType<ShipInitMessage>().ToList())
                                        {
                                            distance = BotCalculator.CalculateDistance(_ship.position);
                                            if (distance < closerDistance)
                                            {
                                                closerDistance = distance;
                                                var _angle = Bot.GetAngle(_ship.position.X, _ship.position.Y);
                                                if (_angle == 1)
                                                {
                                                    position = new Point(Account.Position.X + (_ship.position.X - Account.Position.X), Account.Position.Y);
                                                }
                                                else if (_angle == 2)
                                                {
                                                    position = new Point(Account.Position.X, Account.Position.Y - (_ship.position.Y - Account.Position.Y));
                                                }
                                                else if (_angle == 3)
                                                {
                                                    position = new Point(Account.Position.X - (Account.Position.X - _ship.position.X), Account.Position.Y);
                                                }
                                                else if (_angle == 4)
                                                {
                                                    position = new Point(Account.Position.X, Account.Position.Y + (Account.Position.Y - _ship.position.Y));
                                                }
                                            }
                                        }
                                        if (position.X != 0)
                                        {
                                            BotMethods.MoveTo(position.X, position.Y);
                                            Client.EntityAttackingList.Clear();
                                        }
                                    }
                                }
                            }
                            if (BotSettings.fleeIfEnemyNearby && Client.Entitys.ContainsKey(displayHit.attacker.entityId) && Client.Entitys[displayHit.attacker.entityId] is PlayerInitMessage && Client.Entitys.ContainsKey(displayHit.defender.entityId) && Client.Entitys[displayHit.defender.entityId] is PlayerInitMessage && !displayHit.defender.entityId.Equals(Account.UserID) && !displayHit.attacker.entityId.Equals(Account.UserID) && !BotLogic.OnAttackRunning)
                            {
                                var _attacker = (PlayerInitMessage)Client.Entitys[displayHit.attacker.entityId];
                                var _defender = (PlayerInitMessage)Client.Entitys[displayHit.defender.entityId];
                                if (_attacker.guild != Account.Guild || _defender.guild == Account.Guild || (Account.Guild.Length <= 0 && _attacker.guild.Length > 0 && _defender.guild.Length <= 0))
                                {
                                    BotMethods.WriteLine("ENEMY NEARBY! FLEEING!");
                                    BotSettings.underAttack = true;
                                    Bot.underAttackBy = _attacker.entityInfo;
                                    BotLogic.StartAvoidEnemyThread();
                                }
                            }
                        }
                        else if (_id == AbortAttackMessage.ID)
                        {
                            new AbortAttackMessage(reader);
                            if (!Bot.targetentityId.entityId.Equals(0.0))
                            {
                                double entityId = Bot.targetentityId.entityId;
                                if (Client.Entitys.ContainsKey(entityId) && Client.Entitys[entityId] is ShipInitMessage)
                                {
                                    if (BotSession.attackRetrys.ContainsKey(entityId))
                                    {
                                        var copy = BotSession.attackRetrys;
                                        double entity = entityId;
                                        int count = copy[entity];
                                        copy[entity] = count + 1;
                                        if (BotSession.attackRetrys[entityId] >= 8)
                                        {
                                            Bot.targetentityId = new EntityInfo(0.0, 0);
                                            if (Client.Entitys.ContainsKey(entityId))
                                            {
                                                Client.Entitys.Remove(entityId);
                                            }
                                            BotSession.attackRetrys.Remove(entityId);
                                        }
                                    }
                                    else
                                    {
                                        BotSession.attackRetrys.Add(entityId, 1);
                                    }
                                }
                                else
                                {
                                    Bot.targetentityId = new EntityInfo(0.0, 0);
                                }
                            }
                            BotLogic.attacking = 2;
                        }
                        else if (_id == DisplayResourceIDMessage.ID)
                        {
                            DisplayResourceIDMessage displayResourceId = new DisplayResourceIDMessage(reader);
                            BotMethods.WriteLine(BotHandlers.MessageHandler(displayResourceId.message, displayResourceId.messageArgs));
                        }
                        else if (_id == IconMessage.ID)
                        {
                            IconMessage icon = new IconMessage(reader);
                            if (icon.type == 48)
                            {
                                Account.SpawnQueue = icon.iconText;
                                Form1.form1.updateFormText = true;
                            }
                            else if (icon.type == 51)
                            {
                                if (icon.actionType == 2)
                                {
                                    Account.Poisioned = true;
                                }
                                else if (icon.actionType == 1)
                                {
                                    Account.Poisioned = false;
                                }
                            }
                            else if (icon.type == 53)
                            {
                                Account.NPCLeft = icon.iconText;
                            }
                            else if (icon.type == 98)
                            {
                                if (icon.actionType == 2)
                                {
                                    Account.RejoinCurrentMap = true;
                                }
                                else if (icon.actionType == 1)
                                {
                                    Account.RejoinCurrentMap = false;
                                }
                            }
                        }
                        else if (_id == BoxRemoveMessage.ID)
                        {
                            BoxRemoveMessage boxRemove = new BoxRemoveMessage(reader);
                            if (Bot.targetentityId.entityId.Equals(boxRemove.entityId))
                            {
                                if (Client.Entitys.ContainsKey(boxRemove.entityId))
                                {
                                    var box = (BoxInitMessage)Client.Entitys[boxRemove.entityId];
                                    if (BotCalculator.CalculateDistance(box.position) < 5)
                                    {
                                        Account.Position.X = box.position.X;
                                        Account.Position.Y = box.position.Y;
                                    }
                                    if (box.type == 5 || box.type == 15)
                                    {
                                        BotSession.Sessionchests++;
                                        if (BotSettings.rebuyKeys && box.type == 5 && Account.Keys <= 1 && Account.Pearls >= 1600)
                                        {
                                            new Thread(new ThreadStart(() => Bot.RebuyKeys(false))).Start();
                                        }
                                    }
                                    else
                                    {
                                        BotSession.Sessionglitters++;
                                    }
                                }
                                else
                                {
                                    BotSession.Sessionglitters++;
                                }
                            }
                            if (Client.Entitys.ContainsKey(boxRemove.entityId))
                            {
                                Client.Entitys.Remove(boxRemove.entityId);
                            }
                        }
                        else if (_id == ShipRemoveMessage.ID)
                        {
                            ShipRemoveMessage shipRemove = new ShipRemoveMessage(reader);
                            if (Bot.targetentityId.entityId.Equals(shipRemove.entityId))
                            {
                                Bot.targetentityId = new EntityInfo(0, 0);
                                BotLogic.attacking = 2;
                            }
                            if (Bot.underAttackBy.entityId.Equals(shipRemove.entityId))
                            {
                                BotSettings.underAttack = false;
                                BotSettings.escapingIsland = false;
                                Bot.underAttackBy = new EntityInfo(0, 0);
                                Bot.targetentityId = new EntityInfo(0, 0);
                            }
                            if (Entitys.ContainsKey(shipRemove.entityId))
                            {
                                Entitys.Remove(shipRemove.entityId);
                            }
                            if (EntityMovementList.ContainsKey(shipRemove.entityId))
                            {
                                EntityMovementList.Remove(shipRemove.entityId);
                            }
                            if (EntityAttackingList.ContainsKey(shipRemove.entityId))
                            {
                                EntityAttackingList.Remove(shipRemove.entityId);
                            }
                        }
                        else if (_id == MonsterRemoveMessage.ID)
                        {
                            MonsterRemoveMessage monsterRemove = new MonsterRemoveMessage(reader);
                            if (Bot.targetentityId.entityId .Equals(monsterRemove.entityId))
                            {
                                Bot.targetentityId = new EntityInfo(0, 0);
                            }
                            if (Entitys.ContainsKey(monsterRemove.entityId))
                            {
                                Entitys.Remove(monsterRemove.entityId);
                            }
                        }
                        else if (_id == RepairMessage.ID)
                        {
                            RepairMessage repair = new RepairMessage(reader);
                            if (repair.modus == 0)
                            {
                                Account.Repairing = true;
                            }
                            else
                            {
                                Account.Repairing = false;
                            }
                            BotMethods.WriteLine(Account.Repairing ? "Start Repair." : "Stop Repair.");
                        }
                        else if (_id == MapChangeAskMessage.ID)
                        {
                            MapChangeAskMessage mapChangeAsk = new MapChangeAskMessage(reader);
                            if (mapChangeAsk.type == 0)
                            {
                                Account.JumpAvailable = true;
                                Account.NextMapID = mapChangeAsk.mapId;
                                if (BotSettings.jumpMapIfAvailable && BotSettings.jumpMaps && mapChangeAsk.mapId < 500 || BotSettings.needMapSwitch)
                                {
                                    BotMethods.JumpMap(mapChangeAsk.mapId);
                                    BotMethods.WriteLine("Jumping to next Map [" + BotHandlers.maps[mapChangeAsk.mapId] + "].");
                                    Bot.userGuildTower = null;
                                }
                            }
                        }
                        else if (_id == MapChangeMessage.ID)
                        {
                            MapChangeMessage mapChange = new MapChangeMessage(reader);
                            Account.LastMapID = Account.MapID;
                            Account.NextMapID = -1;
                            Account.JumpAvailable = false;
                            Account.Route.Clear();
                            BotSettings.needMapSwitch = false;
                            bool flag = Account.MapID == 500 && mapChange.mapInfo.mapId == 500;
                            Account.MapID = mapChange.mapInfo.mapId;
                            Account.Map = ((Account.MapID == 500) ? "Nimbus" : mapChange.mapInfo.map);
                            Client.Entitys.Clear();
                            Client.EntityMovementList.Clear();
                            if (Account.OnBM || Account.OnRaid || Account.MapID > 500)
                            {
                                Account.Map = BotHandlers.MapHandler(mapChange.mapInfo.mapId);
                                if (Account.OnRaid && BotSettings.RaidType <= 0)
                                {
                                    BotSettings.RaidType = Account.RaidMedallion == 68 ? 3 : Account.RaidMedallion == 30 ? 2 : 1;
                                }
                                if (Account.OnBM && BotSettings.autoJoinBM)
                                {
                                    BotSettings.lastBM = Account.Map;
                                }
                            }
                            if (Account.MapID == 500 && !flag)
                            {
                                BotSession.Sessiondeaths++;
                                BotMethods.WriteLine("You were sunk!");
                                BotLogic.StartBotThread();
                            }
                            else
                            {
                                BotMethods.WriteLine("Map changed " + Account.Map + "[" + Account.MapID + "]");
                            }
                        }
                        else if (_id == QuestCompletedMessage.ID)
                        {
                            QuestCompletedMessage questCompleted = new QuestCompletedMessage(reader);
                            if (Bot.quests.ContainsKey(BotSession.currentQuest))
                            {
                                Bot.quests[BotSession.currentQuest] = false;
                            }
                            BotMethods.WriteLine("Quest " + BotSession.currentQuest + " finished.");
                            if (BotSession.currentQuest.Contains("Daily"))
                            {
                                BotSession.Sessioncandodailyquest = false;
                            }
                            Form1.form1.UpdateQuestList(BotSession.currentQuest);
                            BotSession.currentQuest = "";
                            Bot.currentQuest = null;
                        }
                        else if (_id == QuestOngoingMessage.ID)
                        {
                            QuestOngoingMessage questInfo = new QuestOngoingMessage(reader);
                            if (questInfo.questInfo.type == 10 || questInfo.questInfo.type == 11 || questInfo.questInfo.type == 12 && Bot.Running)
                            {
                                if (Bot.currentQuest == null)
                                {
                                    Bot.currentQuest = questInfo.questInfo;
                                    Bot.UpdateQuest();
                                }
                                else if (questInfo.questInfo.id > Bot.currentQuest.id)
                                {
                                    Bot.currentQuest = questInfo.questInfo;
                                    Bot.UpdateQuest();
                                }
                            }
                        }
                        else if (_id == QuestCreateMessage.ID)
                        {
                            QuestCreateMessage questCreate = new QuestCreateMessage(reader);
                            if (Bot.Running && BotSession.currentQuest.Length > 2 && Bot.Quests.ContainsKey(BotSession.currentQuest) && Bot.Quests[BotSession.currentQuest] == questCreate.questId)
                            {
                                foreach (var questCondtion in questCreate.conditions)
                                {
                                    if (questCondtion.type == 12 || questCondtion.type == 11)
                                    {
                                        if (Bot.currentQuest == null)
                                        {
                                            Bot.currentQuest = questCondtion;
                                            Bot.UpdateQuest();
                                        }
                                        else if (questCondtion.id > Bot.currentQuest.id)
                                        {
                                            Bot.currentQuest = questCondtion;
                                            Bot.UpdateQuest();
                                        }
                                    }
                                }
                                foreach (var questPreCondition in questCreate.preConditions)
                                {
                                    if (questPreCondition.type == 11 || questPreCondition.type == 12)
                                    {
                                        if (Bot.currentQuest == null)
                                        {
                                            Bot.currentQuest = new QuestConditionStub(questPreCondition.id, questPreCondition.type, 1, questPreCondition.values);
                                            Bot.UpdateQuest();
                                        }
                                        else if (questPreCondition.id > Bot.currentQuest.id)
                                        {
                                            Bot.currentQuest = new QuestConditionStub(questPreCondition.id, questPreCondition.type, 1, questPreCondition.values);
                                            Bot.UpdateQuest();
                                        }
                                    }
                                }
                            }
                        }
                        else if (_length > 2)
                        {
                            reader.Index += _length - 2;
                        }
                    }
                }
                catch (Exception ex)
                {
                    BotMethods.WriteLine(ex.Message);
                }
            }
        }
    }
}
