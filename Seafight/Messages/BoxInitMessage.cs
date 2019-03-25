using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BoxyBot.Seafight.Messages
{
    public class BoxInitMessage : Message //package_125.class_439;
    {
        public const int ID = 14265;
        private int _version;
        public int type;
        public bool visible;
        public PositionStub position;
        public double entityId;
        public int projectId;

        public BoxInitMessage(double entityId, int projectId, int type, PositionStub position, bool visible)
        {
            this.entityId = entityId;
            this.projectId = projectId;
            this.type = type;
            if (position == null)
            {
                this.position = new PositionStub(0, 0);
            } else
            {
                this.position = position;
            }
            this.visible = visible;
        }

        public BoxInitMessage(Reader reader)
        {
            this._version = reader.ReadShort();
            this._version = (int)(65535u & ((uint)(65535 & this._version) >> 6 | (uint)((uint)(65535 & this._version) << 10)));
            this._version = ((this._version > 32767) ? (this._version - 65536) : this._version);
			this.type = reader.ReadShort();
            this.position = new PositionStub(0, 0);
			this.position.X = reader.ReadShort();
			this.position.X = 65535 & ((65535 & this.position.X) >> 12 | (65535 & this.position.X) << 4);
			this.position.X = this.position.X > 32767 ? (int)(this.position.X - 65536) : (int)(this.position.X);
            this.position.Y = reader.ReadShort();
			this.position.Y = 65535 & ((65535 & this.position.Y) << 12 | (65535 & this.position.Y) >> 4);
			this.position.Y = this.position.Y > 32767 ? (int)(this.position.Y - 65536) : (int)(this.position.Y);
			this.projectId = reader.ReadShort();
			this.projectId = (65535 & ((65535 & this.projectId) >> 9 | (65535 & this.projectId) << 7));
			this.projectId = this.projectId > 32767 ? (int)(this.projectId - 65536) : (int)(this.projectId);
			this.entityId = reader.ReadDouble();
            this.visible = reader.ReadBool(); 
        }

        public override byte[] Write()
        {
            return null;
        }

        
    }
}
