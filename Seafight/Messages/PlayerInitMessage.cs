using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BoxyBot.Seafight.Messages
{
    public class PlayerInitMessage : Message //net.bigpoint.seafight.com.module.ship.class_409;
    {
        public const int ID = -23420;
        public PositionStub position;
        public EntityInfo entityInfo; //name_4;
        public EntityTargetAction targetAction; //name_18;
        public ShipPointsStub pointsCurrent = new ShipPointsStub(0, 0);
        public ShipPointsStub pointsMax = new ShipPointsStub(0, 0);
        private int _version;
        public int var_53; //var_53;
        public int designId; //name_14;
        public int var_30; //var_30;
        public int var_25; //var_25;
        public List<PositionStub> route; //var_144;
        public List<int> list_1; //var_167;
        public string guild = ""; //var_89;
        public string username = ""; //name_13;
        public double speed;

        public PlayerInitMessage()
        {
        }

        public PlayerInitMessage(double entityId, int projectId, string username, string guild, PositionStub position, List<PositionStub> route)
        {
            this.entityInfo = new EntityInfo(entityId, projectId);
            this.username = username;
            this.guild = guild;
            this.position = position;
            this.route = route;
        }

        public PlayerInitMessage(Reader reader)
        {
            this._version = reader.ReadShort();
            this._version = (65535 & ((65535 & this._version) << 10 | (int)((uint)(65535 & this._version) >> 6)));
            this._version = ((this._version > 32767) ? (this._version - 65536) : this._version);
			this.route = new List<PositionStub>();
			var i = 0;
			var num = reader.ReadShort();
			while (i < num)
			{
				reader.ReadShort();
				this.route.Add(new PositionStub(reader));
				i++;
			}
            this.username = reader.ReadString();
			reader.ReadShort(); //ID
            this.targetAction = new EntityTargetAction(reader);
			this.var_25 = reader.ReadByte();
			this.var_25 = (255 & ((255 & this.var_25) << 4 | (int)((uint)(255 & this.var_25) >> 4)));
			this.var_25 = ((this.var_25 > 127) ? (this.var_25 - 256) : this.var_25);
            this.var_53 = reader.ReadByte();
            this.var_53 = (int)(255u & ((uint)(255 & this.var_53) >> 0 | (uint)((uint)(255 & this.var_53) << 8)));
            this.var_53 = ((this.var_53 > 127) ? (this.var_53 - 256) : this.var_53);
            this.guild = reader.ReadString();
			this.entityInfo = new EntityInfo(0, 0);
			this.entityInfo.projectId = reader.ReadShort();
			this.entityInfo.projectId = (int)(65535u & ((uint)(65535 & this.entityInfo.projectId) >> 12 | (uint)((uint)(65535 & this.entityInfo.projectId) << 4)));
			this.entityInfo.projectId = ((this.entityInfo.projectId > 32767) ? (this.entityInfo.projectId - 65536) : this.entityInfo.projectId);
			this.entityInfo.entityId = reader.ReadDouble();
			this.designId = reader.ReadShort();
			this.designId = (int)(65535u & ((uint)(65535 & this.designId) << 9 | (uint)((uint)(65535 & this.designId) >> 7)));
			this.designId = ((this.designId > 32767) ? (this.designId - 65536) : this.designId);
            this.var_30 = reader.ReadByte();
            this.var_30 = (255 & ((255 & this.var_30) << 6 | (int)((uint)(255 & this.var_30) >> 2)));
            this.var_30 = ((this.var_30 > 127) ? (this.var_30 - 256) : this.var_30);
            this.position = new PositionStub(0, 0);
			this.position.X = reader.ReadShort();
			this.position.X = (65535 & ((65535 & this.position.X) << 4 | (65535 & this.position.X) >> 12));
			this.position.X = ((this.position.X > 32767) ? (this.position.X - 65536) : this.position.X);
            this.position.Y = reader.ReadShort();
            this.position.Y = (65535 & ((65535 & this.position.Y) << 3 | (int)((uint)(65535 & this.position.Y) >> 13)));
            this.position.Y = ((this.position.Y > 32767) ? (this.position.Y - 65536) : this.position.Y);
            this.list_1 = new List<int>();
            i = 0;
            num = reader.ReadByte();
            while (i < num)
            {
                list_1.Add(reader.ReadShort());
                i++;
            }

        }

        public override byte[] Write()
        {
            return null;
        }  
    }
}
