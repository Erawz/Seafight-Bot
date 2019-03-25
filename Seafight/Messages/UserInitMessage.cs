using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BoxyBot.Seafight.Messages
{
    public class UserInitMessage : Message //package_7.class_15
    {
        public const int ID = -11537;
        private int _version;
        public bool hasPremium; //640;
        public bool hasTreasureHunter; //667;
        public bool isRepairing; //706;
        public double maxXp; //var_684;
        public double xp; //var_643;
        public ShipPointsStub currentHealth; //var_170;
        public PositionStub position; //name_6;
        public EntityInfo entityInfo; //name_4;
        public EntityTargetAction targetAction;//name_18;
        public int maxBp; //var_279;
        public int var_53; //var_53;
        public int level; //var_25;
        public int bp; //var_287;
        public int designId; //name_14;
        public int medallionId; //name_12;
        public int carpenterType; //var_157;
        public int var_30; //var_30;
        public string guild = ""; //var_89;
        public string username = ""; //name_13;
        public List<PositionStub> route; //

        public UserInitMessage()
        {
        }

        public UserInitMessage(Reader reader)
        {
            this._version = reader.ReadShort();
            this._version = (int)(65535u & ((uint)(65535 & this._version) >> 7 | (uint)((uint)(65535 & this._version) << 9)));
            this._version = ((this._version > 32767) ? (this._version - 65536) : this._version);
			this.var_53 = reader.ReadByte();
			this.var_53 = (int)(255u & ((uint)(255 & this.var_53) >> 0 | (uint)((uint)(255 & this.var_53) << 8)));
			this.var_53 = ((this.var_53 > 127) ? (this.var_53 - 256) : this.var_53);
			this.entityInfo = new EntityInfo(0, 0);
			this.entityInfo.projectId = reader.ReadShort();
			this.entityInfo.projectId = (65535 & ((65535 & this.entityInfo.projectId) << 7 | (int)((uint)(65535 & this.entityInfo.projectId) >> 9)));
			this.entityInfo.projectId = ((this.entityInfo.projectId > 32767) ? (this.entityInfo.projectId - 65536) : this.entityInfo.projectId);
			this.entityInfo.entityId = reader.ReadDouble();
			this.bp = reader.ReadInt();
			this.bp = (int)((uint)this.bp << 3 | (uint)((uint)this.bp >> 29));
			this.position = new PositionStub(0, 0);
			this.position.X = reader.ReadShort();
			this.position.X = (65535 & ((65535 & this.position.X) << 13 | (int)((uint)(65535 & this.position.X) >> 3)));
			this.position.X = ((this.position.X > 32767) ? (this.position.X - 65536) : this.position.X);
			this.position.Y = reader.ReadShort();
			this.position.Y = (int)(65535u & ((uint)(65535 & this.position.Y) >> 9 | (uint)((uint)(65535 & this.position.Y) << 7)));
			this.position.Y = ((this.position.Y > 32767) ? (this.position.Y - 65536) : this.position.Y);
			this.level = reader.ReadByte();
			this.level = (255 & ((255 & this.level) << 0 | (int)((uint)(255 & this.level) >> 8)));
			this.level = ((this.level > 127) ? (this.level - 256) : this.level);
			this.designId = reader.ReadShort();
			this.designId = (int)(65535u & ((uint)(65535 & this.designId) << 11 | (uint)((uint)(65535 & this.designId) >> 5)));
			this.designId = ((this.designId > 32767) ? (this.designId - 65536) : this.designId);
			this.carpenterType = reader.ReadByte();
			this.carpenterType = (255 & ((255 & this.carpenterType) << 5 | (int)((uint)(255 & this.carpenterType) >> 3)));
			this.carpenterType = ((this.carpenterType > 127) ? (this.carpenterType - 256) : this.carpenterType);
            this.xp = reader.ReadDouble();
            this.hasTreasureHunter = reader.ReadBool();
			reader.ReadShort();
			this.currentHealth = new ShipPointsStub(reader);
			this.guild = reader.ReadString();
			this.username = reader.ReadString();
            this.maxXp = reader.ReadDouble();
			this.maxBp = reader.ReadInt();
			this.maxBp = (int)((uint)this.maxBp << 14 | (uint)((uint)this.maxBp >> 18));
            this.hasPremium = reader.ReadBool();
			this.medallionId = reader.ReadByte();
            this.medallionId = (255 & ((255 & this.medallionId) >> 6 | (int)((uint)(255 & this.medallionId) << 2)));
            this.medallionId = ((this.medallionId > 127) ? (this.medallionId - 256) : this.medallionId);
			reader.ReadShort(); //ID
			this.targetAction = new EntityTargetAction(reader);
            this.var_30 = reader.ReadByte();
            this.var_30 = (255 & ((255 & this.var_30) >> 2 | (int)((uint)(255 & this.var_30) << 6)));
            this.var_30 = ((this.var_30 > 127) ? (this.var_30 - 256) : this.var_30);
            this.isRepairing = reader.ReadBool();
        }

        public override byte[] Write()
        {
            return null;
        }
    }
}
