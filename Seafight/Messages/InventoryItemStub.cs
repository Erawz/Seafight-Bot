using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BoxyBot.Seafight.Messages
{
    public class InventoryItemStub : Message //package_103.class_622;
    {
        public const int ID = 17154;
        private int _version;
        public double value;
        public int key;

        public InventoryItemStub(Reader reader)
        {
            this._version = reader.ReadShort();
            this._version = (int)(65535u & ((uint)(65535 & this._version) >> 9 | (uint)((uint)(65535 & this._version) << 7)));
            this._version = ((this._version > 32767) ? (this._version - 65536) : this._version);
            reader.ReadShort();
            reader.ReadShort();
            this.key = reader.ReadShort();
            this.key = (65535 & ((65535 & this.key) >> 3 | (int)((uint)(65535 & this.key) << 13)));
            this.key = ((this.key > 32767) ? (this.key - 65536) : this.key);
            this.value = reader.ReadDouble();
        }

        public override byte[] Write()
        {
            throw new NotImplementedException();
        }
    }
}
