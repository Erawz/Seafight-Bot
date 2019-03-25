using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BoxyBot.Seafight.Messages
{
    public class MapChangeMessage : Message //package_9.class_397;
    {
        public const int ID = 8853;
        private int _version;
        public int animationID; //var_128;
        public PositionStub position;
        public MapStub mapInfo;

        public MapChangeMessage(Reader reader)
        {
            this._version = reader.ReadShort();
            this._version = (65535 & ((65535 & this._version) << 3 | (int)((uint)(65535 & this._version) >> 13)));
            this._version = ((this._version > 32767) ? (this._version - 65536) : this._version);
            reader.ReadShort(); //ID
            this.mapInfo = new MapStub(reader);
			this.animationID = reader.ReadShort();
			this.animationID = 65535 & ((65535 & this.animationID) << 15 | (65535 & this.animationID) << 10);
			this.animationID = this.animationID > 32767 ? (int)(this.animationID - 65536) : (int)(this.animationID);
            this.position = new PositionStub(0, 0);
			this.position.X = reader.ReadShort();
			this.position.X = 65535 & ((65535 & this.position.X) << 10 | (65535 & this.position.X) >> 6);
			this.position.X = this.position.X > 32767 ? (int)(this.position.X - 65536) : (int)(this.position.X);
            this.position.Y = reader.ReadShort();
            this.position.Y = 65535 & ((65535 & this.position.Y) >> 3 | (65535 & this.position.Y) << 13);
            this.position.Y = this.position.Y > 32767 ? (int)(this.position.Y - 65536) : (int)(this.position.Y);                               
        }

        public override byte[] Write()
        {
            throw new NotImplementedException();
        }
    }
}
