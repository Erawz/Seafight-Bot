using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BoxyBot.Seafight.Messages
{
    public class MapReturnToMapMessage : Message //package package_9.class_741;
    {
        public const int ID = -27400;
        private int _version;

        public MapReturnToMapMessage(Reader reader)
        {
            this._version = reader.ReadShort();
            this._version = 65535 & ((65535 & this._version) >> 1 | (65535 & this._version) << 15);
            this._version = this._version > 32767 ? (int)(this._version - 65536) : (int)(this._version);
        }

        public MapReturnToMapMessage()
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
