using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BoxyBot.Seafight.Messages
{
    public class UseTeleportMessage : Message //89.303;
    {
        public const int ID = -15498;
        private int _version;
        public int mapId;

        public UseTeleportMessage(Reader reader)
        {
            this._version = reader.ReadShort();
            this._version = 65535 & ((65535 & this._version) << 6 | (65535 & this._version) >> 10);
            this._version = this._version > 32767 ? (this._version - 65536) : (this._version);
            this.mapId = reader.ReadShort();
            this.mapId = 65535 & ((65535 & this.mapId) << 10 | (65535 & this.mapId) >> 6);
            this.mapId = this.mapId > 32767 ? (this.mapId - 65536) : (this.mapId);
        }

        public UseTeleportMessage(int mapId)
        {
            this.mapId = mapId;
        }

        public override byte[] Write()
        {
            List<byte[]> Buffer = new List<byte[]>();
            Buffer.Add(Reader.WriteShort(ID));
            Buffer.Add(Reader.WriteShort(0));
            Buffer.Add(Reader.WriteShort((65535 & ((65535 & this.mapId) >> 10 | (65535 & this.mapId) << 6))));
            return Buffer.SelectMany(bytes => bytes).ToArray<byte>();
        }
    }
}
