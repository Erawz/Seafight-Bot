using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BoxyBot.Seafight.Messages
{
    public class PositionStub : Message //9_91;
    {
        public const int ID = -2764;
        private int _version;
        public int X; //8
        public int Y; //9
        public double dX;
        public double dY;   

        public PositionStub(Reader reader)
        {
            this._version = reader.ReadShort();
            this._version = 65535 & ((65535 & this._version) << 13 | (65535 & this._version) >> 3);
            this._version = this._version > 32767 ? (this._version - 65536) : (this._version);
			this.X = reader.ReadShort();
			this.X = 65535 & ((65535 & this.X) >> 6 | (65535 & this.X) << 10);
			this.X = this.X > 32767 ? (this.X - 65536) : (this.X);
			this.Y = reader.ReadShort();
            this.Y = 65535 & ((65535 & this.Y) >> 7 | (65535 & this.Y) << 9);
            this.Y = this.Y > 32767 ? (this.Y - 65536) : (this.Y);          
        }

        public PositionStub(int X, int Y)
        {
            this.X = X;
            this.Y = Y;
        }

        public override byte[] Write()
        {
            throw new NotImplementedException();
        }
    }
}
