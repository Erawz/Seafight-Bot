using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BoxyBot.Seafight.Messages
{
    public class MonsterInitMessage : Message //package_124.class_485
    {
        public const int ID = -7712;
        private int _version;
        public EntityInfo entityInfo; //name_4;
        public EntityInfo taggingEntity; //var_
        public PositionStub position;
        public int maxHitpoints; //var_149;
        public int nameId; //NAME LOOKUP, var_476;, param2
        public int hitpoints; //var_110;
        public int var_30; //var_30;
        public int type; //name_5;
        public int var_452;  //var_452;
        public int harpoonId = 20;
        public string name;

        public MonsterInitMessage()
        {
        }

        public MonsterInitMessage(double entityId, int projectId, int nameId, int hp, int maxHp, PositionStub position)
        {
            this.entityInfo = new EntityInfo(entityId, projectId);
            this.nameId = nameId;
            if (Bot.Monsters.ContainsKey(nameId))
            {
                this.name = Bot.Monsters[nameId];
            }
            this.hitpoints = hp;
            this.maxHitpoints = maxHp;
            this.position = position;
        }

        public MonsterInitMessage(Reader reader)
        {
            this._version = reader.ReadShort();
            this._version = (65535 & ((65535 & this._version) << 10 | (int)((uint)(65535 & this._version) >> 6)));
            this._version = ((this._version > 32767) ? (this._version - 65536) : this._version);
			this.taggingEntity = new EntityInfo(0, 0);	
			this.taggingEntity.projectId = reader.ReadShort();
			this.taggingEntity.projectId = (65535 & ((65535 & this.taggingEntity.projectId) >> 5 | (int)((uint)(65535 & this.taggingEntity.projectId) << 11)));
			this.taggingEntity.projectId = ((this.taggingEntity.projectId > 32767) ? (this.taggingEntity.projectId - 65536) : this.taggingEntity.projectId);
            this.taggingEntity.entityId = reader.ReadDouble();
			this.entityInfo = new EntityInfo(0, 0);
			this.entityInfo.projectId = reader.ReadShort();
			this.entityInfo.projectId = (65535 & ((65535 & this.entityInfo.projectId) >> 5 | (int)((uint)(65535 & this.entityInfo.projectId) << 11)));
			this.entityInfo.projectId = ((this.entityInfo.projectId > 32767) ? (this.entityInfo.projectId - 65536) : this.entityInfo.projectId);
            this.entityInfo.entityId = reader.ReadDouble();
			this.var_30 = reader.ReadByte();
			this.var_30 = (int)(255u & ((uint)(255 & this.var_30) >> 5 | (uint)((uint)(255 & this.var_30) << 3)));
			this.var_30 = ((this.var_30 > 127) ? (this.var_30 - 256) : this.var_30);
			this.nameId = reader.ReadByte();
			this.nameId = (int)(255u & ((uint)(255 & this.nameId) << 7 | (uint)((uint)(255 & this.nameId) >> 1)));
			this.nameId = ((this.nameId > 127) ? (this.nameId - 256) : this.nameId);
            this.type = reader.ReadShort();
			this.hitpoints = reader.ReadInt();
			this.hitpoints = (int)((uint)this.hitpoints << 13 | (uint)((uint)this.hitpoints >> 19));
			this.maxHitpoints = reader.ReadInt();
			this.maxHitpoints = (int)((uint)this.maxHitpoints << 9 | (uint)((uint)this.maxHitpoints >> 23));
            this.position = new PositionStub(0, 0);
			this.position.X = reader.ReadShort();
			this.position.X = (65535 & ((65535 & this.position.X) >> 8 | (65535 & this.position.X) << 8));
			this.position.X = ((this.position.X > 32767) ? (this.position.X - 65536) : this.position.X);
			this.position.Y = reader.ReadShort();
			this.position.Y = (int)(65535u & ((uint)(65535 & this.position.Y) >> 1 | (uint)((uint)(65535 & this.position.Y) << 15)));
			this.position.Y = ((this.position.Y > 32767) ? (this.position.Y - 65536) : this.position.Y);
			this.var_452 = reader.ReadByte();
			this.var_452 = (255 & ((255 & this.var_452) >> 4 | (int)((uint)(255 & this.var_452) << 4)));
			this.var_452 = ((this.var_452 > 127) ? (this.var_452 - 256) : this.var_452);
        }

        public override byte[] Write()
        {
            throw new NotImplementedException();
        }
    }
}
