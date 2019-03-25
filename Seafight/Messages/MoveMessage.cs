using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BoxyBot.Seafight.Messages
{
    public class MoveMessage : Message //package9_class612;
    {
        public const int ID = 9754;
        private int _version;
        public int X;
        public int Y; 

        public MoveMessage(Reader reader)
        {
            this._version = reader.ReadShort();
            this._version = (65535 & ((65535 & this._version) << 2 | (int)((uint)(65535 & this._version) >> 14)));
            this._version = ((this._version > 32767) ? (this._version - 65536) : this._version);
            this.X = reader.ReadShort();
            this.X = (65535 & ((65535 & this.X) >> 1 | (65535 & this.X) << 15));
            this.X = ((this.X > 32767) ? (this.X - 65536) : this.X);
            this.Y = reader.ReadShort();
            this.Y = (65535 & ((65535 & this.Y) << 3 | (65535 & this.Y) >> 13));
            this.Y = ((this.Y > 32767) ? (this.Y - 65536) : this.Y);    
        }

        public MoveMessage(int X, int Y)
        {
            this.X = X;
            this.Y = Y;
        }

        public override byte[] Write()
        {
            List<byte[]> Buffer = new List<byte[]>();
            Buffer.Add(Reader.WriteShort(ID));
            Buffer.Add(Reader.WriteShort(0));
            Buffer.Add(Reader.WriteShort((65535 & ((65535 & this.X) >> 15 | (65535 & this.X) << 1))));
            Buffer.Add(Reader.WriteShort((65535 & ((65535 & this.Y) << 8 | (65535 & this.Y) >> 8))));
            return Buffer.SelectMany(bytes => bytes).ToArray<byte>();
        }
    }
}
