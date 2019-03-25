using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BoxyBot.Seafight.Messages
{
    public class AbortAttackMessage : Message //package_90.class_407;
    {
        public const int ID = 31972;
        private int _version;

        public AbortAttackMessage(Reader reader)
        {
            this._version = reader.ReadShort();
            this._version = 65535 & ((65535 & this._version) >> 13 | (65535 & this._version) << 3);
            this._version = this._version > 32767 ? (int)(this._version - 65536) : (int)(this._version);
        }

        public AbortAttackMessage()
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
