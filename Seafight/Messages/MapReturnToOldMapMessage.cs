using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BoxyBot.Seafight.Messages
{
    class MapReturnToOldMapMessage : Message //package_9.class_692;
    {
        public const int ID = -1196;
        private int _version;

        public MapReturnToOldMapMessage()
        {
        }

        public MapReturnToOldMapMessage(Reader reader)
        {
            this._version = reader.ReadShort();
            this._version = (int)(65535u & ((uint)(65535 & this._version) >> 6 | (uint)((uint)(65535 & this._version) << 10)));
            this._version = ((this._version > 32767) ? (this._version - 65536) : this._version);
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
