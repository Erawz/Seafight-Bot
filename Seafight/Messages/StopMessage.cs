using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BoxyBot.Seafight.Messages
{
    public class StopMessage : Message //9.437
    {
        public const int ID = 27649;
        private int _version;
        public int distance;
        public double entityId;
        public int projectId;
        public PositionStub position;

        public StopMessage(Reader reader)
        {
            this._version = reader.ReadShort();
            this._version = (65535 & ((65535 & this._version) << 13 | (int)((uint)(65535 & this._version) >> 3)));
            this._version = ((this._version > 32767) ? (this._version - 65536) : this._version);
			this.position = new PositionStub(0, 0);
			this.position.X = reader.ReadShort();
			this.position.X = (65535 & ((65535 & this.position.X) >> 0 | (int)((uint)(65535 & this.position.X) << 16)));
			this.position.X = ((this.position.X > 32767) ? (this.position.X - 65536) : this.position.X); 
			this.position.Y = reader.ReadShort();
			this.position.Y = (int)(65535u & ((uint)(65535 & this.position.Y) >> 14 | (uint)((uint)(65535 & this.position.Y) << 2)));
			this.position.Y = ((this.position.Y > 32767) ? (this.position.Y - 65536) : this.position.Y);
            this.distance = reader.ReadByte();
            this.distance = (int)(255u & ((uint)(255 & this.distance) << 4 | (uint)((uint)(255 & this.distance) >> 4)));
            this.distance = ((this.distance > 127) ? (this.distance - 256) : this.distance);
            this.projectId = reader.ReadShort();
            this.projectId = (65535 & ((65535 & this.projectId) >> 3 | (int)((uint)(65535 & this.projectId) << 13)));
            this.projectId = ((this.projectId > 32767) ? (this.projectId - 65536) : this.projectId);
            this.entityId = reader.ReadDouble();        
        }

        public override byte[] Write()
        {
            throw new NotImplementedException();
        }
    }
}
