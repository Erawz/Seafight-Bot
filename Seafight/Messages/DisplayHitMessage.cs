using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BoxyBot.Seafight.Messages
{
    public class DisplayHitMessage : Message //package_89.class_437;
    {
        public const int ID = -29182;
        private int _version;
        public bool criticalhit; //var_1468;
        public ShipPointsStub damage; //var_134;
        public EntityInfo defender; //var_250;
        public EntityInfo attacker; //var_50;
        public int animationId; //var_128;
        public int effectId; //var_85;

        public const int HITEFFECT_NONE = 0;
        public const int HITEFFECT_BLACKPOWDER = 1;
        public const int HITEFFECT_ARMOURPLATES = 2;
        public const int HITEFFECT_BOTH = 3;
        public const int HITEFFECT__MAX = 4;

        public DisplayHitMessage(Reader reader)
        {
            this._version = reader.ReadShort();
            this._version = 65535 & ((65535 & this._version) >> 8 | (65535 & this._version) << 8);
            this._version = this._version > 32767 ? (int)(this._version - 65536) : (int)(this._version);
			this.attacker = new EntityInfo(0.0, 0);
			this.attacker.projectId = reader.ReadShort();
			this.attacker.projectId = 65535 & ((65535 & this.attacker.projectId) << 5 | (65535 & this.attacker.projectId) >> 11);
			this.attacker.projectId = this.attacker.projectId > 32767 ? (int)(this.attacker.projectId - 65536) : (int)(this.attacker.projectId);
            this.attacker.entityId = reader.ReadDouble();
			this.defender = new EntityInfo(0.0, 0);
			this.defender.projectId = reader.ReadShort();
			this.defender.projectId = 65535 & ((65535 & this.defender.projectId) << 5 | (65535 & this.defender.projectId) >> 11);
			this.defender.projectId = this.defender.projectId > 32767 ? (int)(this.defender.projectId - 65536) : (int)(this.defender.projectId);
			this.defender.entityId = reader.ReadDouble();
            this.effectId = reader.ReadShort();
            this.criticalhit = reader.ReadBool();
            this.animationId = reader.ReadShort();
            reader.ReadShort();
            this.damage = new ShipPointsStub(reader);
        }

        public override byte[] Write()
        {
            return null;
        }
    }
}
