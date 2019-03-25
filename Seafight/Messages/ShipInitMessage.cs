using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BoxyBot.Seafight.Messages
{
    public class ShipInitMessage : Message //net.bigpoint.seafight.com.module.ship.class_533;
    {
        public const int ID = 23872;
        private int _version;
        public ShipPointsStub pointsCurrent; //var_170;
        public ShipPointsStub pointsMax; //var_240;
        public PositionStub position; //name_6;
        public EntityInfo entityInfo; //name_4;
        public EntityInfo taggingEntity; //var_
        public int var_30; //var_30;
        public int var_865; //var_865;
        public int var_53; //var_53;
        public int var_906; //var_90X;
        public int var_496; //var_496;
        public int nameId; //var_17;
        public int var_737; //var_737;
        public int designId; //name_14;
        public int name_28; //name_28;
        public int var_184; //var_184;
        public List<int> var_167; //var_167;
        public List<PositionStub> route; //var_114;
        public List<ShipEntityInfoType> name_32; //name_32;
        public string name = "NPC";
        public double speed;
        public double boardinAttackValue;
        public bool moving;
        public bool boarded;
        public bool boardable;
        public bool usePowder = false;
        public bool usePlates = false;
        public bool useBoard = false;
        public int ammoId = 5;

        public ShipInitMessage()
        {
        }

        public ShipInitMessage(double entityId, int projectId, int nameId, int hp, int maxHp, PositionStub position, List<PositionStub> route)
        {
            this.entityInfo = new EntityInfo(entityId, projectId);
            this.nameId = nameId;
            if (Bot.NPCs.ContainsKey(nameId))
            {
                this.name = Bot.NPCs[nameId];
            }
            this.pointsCurrent = new ShipPointsStub(hp, 0);
            this.pointsMax = new ShipPointsStub(maxHp, 0);
            this.position = position;
            this.route = route;
        }

        public ShipInitMessage(Reader reader)
        {
            this._version = reader.ReadShort();
            this._version = (int)(65535u & ((uint)(65535 & this._version) >> 4 | (uint)((uint)(65535 & this._version) << 12)));
            this._version = ((this._version > 32767) ? (this._version - 65536) : this._version);
			this.var_167 = new List<int>();
			var i = 0;
			var num = reader.ReadByte();
			while (i < num)
			{
				var_167.Add(reader.ReadShort());
				i++;
			}
			this.name_28 = reader.ReadByte();
			this.name_28 = (int)(255u & ((uint)(255 & this.name_28) << 1 | (uint)((uint)(255 & this.name_28) >> 7)));
			this.name_28 = ((this.name_28 > 127) ? (this.name_28 - 256) : this.name_28);
            this.var_53 = reader.ReadShort();
			this.route = new List<PositionStub>();
			i = 0;
			num = reader.ReadShort();
			while (i < num)
			{
				reader.ReadShort();
				route.Add(new PositionStub(reader));
				i++;
			}
            this.var_737 = reader.ReadShort();
			this.position = new PositionStub(0, 0);
			this.position.X = reader.ReadShort();
			this.position.X = (int)(65535u & ((uint)(65535 & this.position.X) << 11 | (uint)((uint)(65535 & this.position.X) >> 5)));
			this.position.X = ((this.position.X > 32767) ? (this.position.X - 65536) : this.position.X);
			this.position.Y = reader.ReadShort();
			this.position.Y = (65535 & ((65535 & this.position.Y) << 15 | (int)((uint)(65535 & this.position.Y) >> 1)));
			this.position.Y = ((this.position.Y > 32767) ? (this.position.Y - 65536) : this.position.Y);
			reader.ReadShort(); //ID
			this.pointsCurrent = new ShipPointsStub(reader);
			this.usePlates = reader.ReadBool();
			this.entityInfo = new EntityInfo(0, 0);
			this.entityInfo.projectId = reader.ReadShort();
			this.entityInfo.projectId = (65535 & ((65535 & this.entityInfo.projectId) >> 15 | (int)((uint)(65535 & this.entityInfo.projectId) << 1)));
			this.entityInfo.projectId = ((this.entityInfo.projectId > 32767) ? (this.entityInfo.projectId - 65536) : this.entityInfo.projectId);
			this.entityInfo.entityId = reader.ReadDouble();
			this.taggingEntity = new EntityInfo(0, 0);
			this.taggingEntity.projectId = reader.ReadShort();
			this.taggingEntity.projectId = (65535 & ((65535 & this.taggingEntity.projectId) >> 15 | (int)((uint)(65535 & this.taggingEntity.projectId) << 1)));
			this.taggingEntity.projectId = ((this.taggingEntity.projectId > 32767) ? (this.taggingEntity.projectId - 65536) : this.taggingEntity.projectId);
			this.taggingEntity.entityId = reader.ReadDouble();
            this.usePowder = reader.ReadBool();
			this.designId = reader.ReadShort();
			this.designId = (65535 & ((65535 & this.designId) >> 11 | (int)((uint)(65535 & this.designId) << 5)));
			this.designId = ((this.designId > 32767) ? (this.designId - 65536) : this.designId);
			this.var_865 = reader.ReadShort();
			this.var_906 = reader.ReadShort();
			this.name_32 = new List<ShipEntityInfoType>();
			i = 0;
			num = reader.ReadByte();
			while (i < num)
			{
				reader.ReadShort();
				name_32.Add(new ShipEntityInfoType(reader));
				i++;
			}
			this.name = reader.ReadString();
			reader.ReadShort(); //ID
			this.pointsMax = new ShipPointsStub(reader);
			this.nameId = reader.ReadShort();
			this.nameId = (int)(65535u & ((uint)(65535 & this.nameId) << 4 | (uint)((uint)(65535 & this.nameId) >> 12)));
			this.nameId = ((this.nameId > 32767) ? (this.nameId - 65536) : this.nameId);
            this.useBoard = reader.ReadBool();
            this.var_30 = reader.ReadByte();
            this.var_30 = (int)(255u & ((uint)(255 & this.var_30) >> 3 | (uint)((uint)(255 & this.var_30) << 5)));
            this.var_30 = ((this.var_30 > 127) ? (this.var_30 - 256) : this.var_30);
            this.var_496 = reader.ReadShort();
            this.var_496 = (int)(65535u & ((uint)(65535 & this.var_496) << 0 | (uint)((uint)(65535 & this.var_496) >> 16)));
            this.var_496 = ((this.var_496 > 32767) ? (this.var_496 - 65536) : this.var_496);
            this.var_184 = reader.ReadShort();
        }

        public override byte[] Write()
        {
            return null;
        } 
    }
}
