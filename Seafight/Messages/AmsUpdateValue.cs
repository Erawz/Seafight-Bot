using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BoxyBot.Seafight.Messages
{
    public class AmsUpdateValue : Message //net.bigpoint.seafight.com.module.ship.class_825;
    {
        public const int ID = 13559;
        private int _version;
        public double value;
        public int key;

        public AmsUpdateValue(int Key, double Value)
        {
            this.key = Key;
            this.value = Value;
        }

        public AmsUpdateValue(Reader reader)
        {
            this._version = reader.ReadShort();
            this._version = (65535 & ((65535 & this._version) << 1 | (int)((uint)(65535 & this._version) >> 15)));
            this._version = ((this._version > 32767) ? (this._version - 65536) : this._version);
            this.value = reader.ReadDouble();
            this.key = reader.ReadShort();           
        }

        public override byte[] Write()
        {
            throw new NotImplementedException();
        }
    }
}
