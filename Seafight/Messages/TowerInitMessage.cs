using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BoxyBot.Seafight.Messages
{
    public class TowerInitMessage : Message //net.bigpoint.seafight.com.module.ship.class_404;
    {
        public const int ID = -21405;
        private int _version;
        public EntityInfo entityInfo; //name_4;
        public PositionStub position;
        public ShipPointsStub pointsCurrent; //var_240;
        public ShipPointsStub pointsMax; //var_170;
        public EntityTargetAction targetAction; //name_18;
        public int towerId; //var_431;
        public int name_28; //name_28;
        public int designID; //name_14;
        public string guild; //var_89;
        
        public TowerInitMessage(Reader reader)
        {
            this._version = reader.ReadShort();
            this._version = 65535 & ((65535 & this._version) >> 3 | (65535 & this._version) << 13);
            this._version = this._version > 32767 ? (int)(this._version - 65536) : (int)(this._version);
			reader.ReadShort();
			this.targetAction = new EntityTargetAction(reader);
			this.position = new PositionStub(0, 0);
			this.position.X = reader.ReadShort();
			this.position.X = 65535 & ((65535 & this.position.X) >> 3 | (65535 & this.position.X) << 13);
			this.position.X = this.position.X > 32767 ? (int)(this.position.X - 65536) : (int)(this.position.X);
            this.position.Y = reader.ReadShort();
			this.position.Y = 65535 & ((65535 & this.position.Y) << 8 | (65535 & this.position.Y) >> 8);
			this.position.Y = this.position.Y > 32767 ? (int)(this.position.Y - 65536) : (int)(this.position.Y);
			reader.ReadShort();
			this.pointsMax = new ShipPointsStub(reader);
			this.entityInfo = new EntityInfo(0, 0);
			this.entityInfo.projectId = reader.ReadShort();
			this.entityInfo.projectId = 65535 & ((65535 & this.entityInfo.projectId) >> 12 | (65535 & this.entityInfo.projectId) << 4);
			this.entityInfo.projectId = this.entityInfo.projectId > 32767 ? (int)(this.entityInfo.projectId - 65536) : (int)(this.entityInfo.projectId);
			this.entityInfo.entityId = reader.ReadDouble();
			this.designID = reader.ReadShort();
			this.designID = 65535 & ((65535 & this.designID) >> 15 | (65535 & this.designID) << 1);
			this.designID = this.designID > 32767 ? (int)(this.designID - 65536) : (int)(this.designID);
			reader.ReadShort();
			this.pointsCurrent = new ShipPointsStub(reader);
            this.name_28 = reader.ReadInt();
            this.name_28 = this.name_28 >> 15 | this.name_28 << 27;
            this.guild = reader.ReadString();
            this.towerId = reader.ReadByte();
            this.towerId = 255 & ((255 & this.towerId) << 2 | (255 & this.towerId) >> 6);
            this.towerId = this.towerId > 127 ? (int)(this.towerId - 256) : (int)(this.towerId);
        }

        public override byte[] Write()
        {
            throw new NotImplementedException();
        }
    }
}
