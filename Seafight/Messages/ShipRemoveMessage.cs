using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BoxyBot.Seafight.Messages
{
    class ShipRemoveMessage : Message //net.bigpoint.seafight.com.module.ship.class_520;
    {
        public const int ID = -6123;
        private int _version;
        public double entityId;
        public int projectId;
        public double attackerentityId;
        public int attackerprojectId; 
        public int animationID;
        

        public ShipRemoveMessage(Reader reader)
        {
            this._version = reader.ReadShort();
            this._version = (65535 & ((65535 & this._version) << 9 | (int)((uint)(65535 & this._version) >> 7)));
            this._version = ((this._version > 32767) ? (this._version - 65536) : this._version);
            this.attackerprojectId = reader.ReadShort();
            this.attackerprojectId = (int)(65535 & ((65535 & this.attackerprojectId) >> 15 | (65535 & this.attackerprojectId) << 1));
            this.attackerprojectId = ((this.attackerprojectId > 32767) ? (this.attackerprojectId - 65536) : this.attackerprojectId);
            this.attackerentityId = reader.ReadDouble();
            this.animationID = reader.ReadShort();
            this.projectId = reader.ReadShort();
            this.projectId = (int)(65535 & ((65535 & this.projectId) >> 15 | (65535 & this.projectId) << 1));
            this.projectId = ((this.projectId > 32767) ? (this.projectId - 65536) : this.projectId); 
            this.entityId = reader.ReadDouble();
        }

        public override byte[] Write()
        {
            return null;
        }       
    }
}
