using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BoxyBot.Seafight.Messages
{
    public class AcceptLoginBonusMessage : Message //160.814;
    {
        public const int ID = -27003;
        private int _version;

        public AcceptLoginBonusMessage(Reader reader)
        {
            this._version = reader.ReadShort();
            this._version = 65535 & ((65535 & this._version) << 5 | (65535 & this._version) >> 11);
            this._version = this._version > 32767 ? (this._version - 65536) : (this._version);
        }

        public AcceptLoginBonusMessage()
        {
        }

        public override byte[] Write()
        {
            List<byte[]> Buffer = new List<byte[]>();
            Buffer.Add(Reader.WriteShort(ID));
            Buffer.Add(Reader.WriteShort(0));
            return Buffer.SelectMany(bytes => bytes).ToArray<byte>();
        }
    }
}
