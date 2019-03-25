using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BoxyBot.Seafight.Messages
{
    public class MapChangeDelayedMessage : Message //package_9.class_566;
    {
        public const int ID = 27378;
        private int _version;
        public int mapId;

        public MapChangeDelayedMessage(Reader reader)
        {
            this._version = reader.ReadShort();
            this._version = (65535 & ((65535 & this._version) << 5 | (int)((uint)(65535 & this._version) >> 11)));
            this._version = ((this._version > 32767) ? (this._version - 65536) : this._version);
            this.mapId = reader.ReadShort();
            this.mapId = (65535 & ((65535 & this.mapId) >> 11 | (int)((uint)(65535 & this.mapId) << 5)));
            this.mapId = ((this.mapId > 32767) ? (this.mapId - 65536) : this.mapId);
        }

        public MapChangeDelayedMessage(int mapId)
        {
            this.mapId = mapId;
        }

        public override byte[] Write()
        {
            List<byte[]> Buffer = new List<byte[]>();
            Buffer.Add(Reader.WriteShort(ID));
            Buffer.Add(Reader.WriteShort(0));
            Buffer.Add(Reader.WriteShort((int)(65535u & ((uint)(65535 & this.mapId) << 11 | (uint)((uint)(65535 & this.mapId) >> 5)))));
            return Buffer.SelectMany(bytes => bytes).ToArray<byte>();
        }
    }
}
