using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BoxyBot.Seafight.Messages
{
    public class LootStub : Message //49.137
    {
        public const int ID = 26101;
        private int _version;
        public int amount;
        public int type;
        public int id;
        public int var_274;
        public bool var_704;

        public LootStub(Reader reader)
        {
            this._version = reader.ReadShort();
            this._version = 65535 & ((65535 & this._version) << 0 | (65535 & this._version) >> 16);
            this._version = this._version > 32767 ? (this._version - 65536) : (this._version);
            this.id = reader.ReadInt();
            this.id = this.id >> 13 | this.id << 19;
            this.amount = reader.ReadInt();
            this.amount = this.amount << 16 | this.amount >> 16;
            this.var_274 = reader.ReadInt();
            this.var_274 = this.var_274 << 16 | this.var_274 >> 16;
            this.type = reader.ReadShort();
            this.var_704 = reader.ReadBool();
        }

        public override byte[] Write()
        {
            throw new NotImplementedException();
        }
    }
}
