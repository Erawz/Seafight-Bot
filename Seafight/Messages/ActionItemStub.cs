using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BoxyBot.Seafight.Messages
{
    public class ActionItemStub : Message //package_89.class_311;
    {
        public const int ID = 21444;
        private int _version;
        public int var_271;
        public int amount;
        public int var_435;
        public int var_120;
        public int itemId;
        public bool active;

        public ActionItemStub(Reader reader)
        {
            this._version = reader.ReadShort();
            this._version = (int)(65535u & ((uint)(65535 & this._version) >> 7 | (uint)((uint)(65535 & this._version) << 9)));
            this._version = ((this._version > 32767) ? (this._version - 65536) : this._version);
            reader.ReadShort();
            reader.ReadShort();
            this.itemId = reader.ReadShort();
            this.itemId = (int)(65535u & ((uint)(65535 & this.itemId) >> 3 | (uint)((uint)(65535 & this.itemId) << 13)));
            this.itemId = ((this.itemId > 32767) ? (this.itemId - 65536) : this.itemId);
            this.amount = reader.ReadInt();
            this.amount = (this.amount >> 2 | (int)((uint)this.amount << 30));
            this.var_435 = reader.ReadByte();
            this.var_435 = (int)(255u & ((uint)(255 & this.var_435) << 8 | (uint)((uint)(255 & this.var_435) >> 0)));
            this.var_435 = ((this.var_435 > 127) ? (this.var_435 - 256) : this.var_435);
            this.var_120 = reader.ReadShort();
            this.var_120 = (65535 & ((65535 & this.var_120) << 7 | (65535 & this.var_120) >> 9));
            this.var_120 = ((this.var_120 > 32767) ? (this.var_120 - 65536) : this.var_120);
            this.active = reader.ReadBool();
            this.var_271 = reader.ReadShort();
            this.var_271 = (65535 & ((65535 & this.var_271) >> 8 | (int)((uint)(65535 & this.var_271) << 8)));
            this.var_271 = ((this.var_271 > 32767) ? (this.var_271 - 65536) : this.var_271);           
        }

        public override byte[] Write()
        {
            throw new NotImplementedException();
        }
    }
}
