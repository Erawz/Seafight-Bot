using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BoxyBot.Seafight.Messages
{
    public class ShipEntityInfoType : Message //129.560;
    {
        public const int ID = -12585;
        private int _version;
        public int type;
        public bool var_867;

        public ShipEntityInfoType(Reader reader)
        {
            this._version = reader.ReadShort();
            this._version = 65535 & ((65535 & this._version) << 6 | (65535 & this._version) >> 10);
            this._version = this._version > 32767 ? (this._version - 65536) : (this._version);
            reader.ReadShort();
            this._version = reader.ReadShort();
            this._version = 65535 & ((65535 & this._version) << 6 | (65535 & this._version) >> 10);
            this._version = this._version > 32767 ? (this._version - 65536) : (this._version);
            this.type = reader.ReadShort();
            this.var_867 = reader.ReadBool();
        }

        public override byte[] Write()
        {
            throw new NotImplementedException();
        }
    }
}
