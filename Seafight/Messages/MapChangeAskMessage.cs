using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BoxyBot.Seafight.Messages
{
    public class MapChangeAskMessage : Message //package_9.class_395;
    {
        public const int ID = -22747;
        private int _version;
        public int type;
        public int mapId;

        public MapChangeAskMessage(Reader reader)
        {
            this._version = reader.ReadShort();
            this._version = 65535 & ((65535 & this._version) >> 7 | (65535 & this._version) << 9);
            this._version = this._version > 32767 ? (int)(this._version - 65536) : (int)(this._version);  
            this.mapId = reader.ReadShort();
            this.mapId = 65535 & ((65535 & this.mapId) >> 15 | (65535 & this.mapId) << 1);
            this.mapId = this.mapId > 32767 ? (int)(this.mapId - 65536) : (int)(this.mapId);
            this.type = reader.ReadShort();
        }

        public override byte[] Write()
        {
            throw new NotImplementedException();
        }
    }
}
