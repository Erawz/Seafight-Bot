using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BoxyBot.Seafight.Messages
{
    public class MapChangeRequestMessage : Message //package_9.class_812;
    {
        public const int ID = 25926;
        private int _version;
        public int mapId;

        public MapChangeRequestMessage(Reader reader)
        {
            this._version = reader.ReadShort();
            this._version = 65535 & ((65535 & this._version) >> 14 | (65535 & this._version) << 2);
            this._version = this._version > 32767 ? (int)(this._version - 65536) : (int)(this._version);
            this.mapId = reader.ReadShort();
            this.mapId = 65535 & ((65535 & this.mapId) >> 9 | (65535 & this.mapId) << 7);
            this.mapId = this.mapId > 32767 ? (int)(this.mapId - 65536) : (int)(this.mapId);
        }

        public MapChangeRequestMessage(int mapId)
        {
            this.mapId = mapId;
        }

        public override byte[] Write()
        {
            List<byte[]> Buffer = new List<byte[]>();
            Buffer.Add(Reader.WriteShort(ID));
            Buffer.Add(Reader.WriteShort(0));
            Buffer.Add(Reader.WriteShort((65535 & ((65535 & this.mapId) << 9 | (65535 & this.mapId) >> 7))));
            return Buffer.SelectMany(bytes => bytes).ToArray<byte>();
        }
    }
}
