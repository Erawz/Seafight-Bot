using System;
using System.Collections.Generic;
using System.IO;

namespace BoxyBot.Seafight.Messages
{
    public class MapStub : Message //package_9.class_17;
    {
        public const int ID = -11723;
        private int _version;
        public int theme; //var_729;
        public int mapId; //name_10;
        public int type; //name_5;
        public int width; //var_40;
        public int height; //var_33;
        public string map; //var_201;
        public bool needReconnect; //var_1220;
        public List<MapTileStub> list_0; //var_628;

        public MapStub(Reader reader)
        {
            this._version = reader.ReadShort();
            this._version = 65535 & ((65535 & this._version) >> 13 | (65535 & this._version) << 3);
            this._version = this._version > 32767 ? (this._version - 65536) : (this._version);
            this.needReconnect = reader.ReadBool();
            this.height = reader.ReadShort();
            this.height = 65535 & ((65535 & this.height) >> 5 | (65535 & this.height) << 11);
            this.height = this.height > 32767 ? (int)(this.height - 65536) : (int)(this.height);
            this.list_0 = new List<MapTileStub>();
            int i = 0;
            int max = reader.ReadShort();
            while (i < max)
            {
                reader.ReadShort();
                var maptile = new MapTileStub(reader);
                this.list_0.Add(maptile);
                i++;
            }
            this.map = reader.ReadString();
            this.width = reader.ReadShort();
            this.width = 65535 & ((65535 & this.width) << 8 | (65535 & this.width) >> 8);
            this.width = this.width > 32767 ? (int)(this.width - 65536) : (int)(this.width);
            this.type = reader.ReadShort();
            this.mapId = reader.ReadShort();
            this.mapId = 65535 & ((65535 & this.mapId) >> 13 | (65535 & this.mapId) << 3);
            this.mapId = this.mapId > 32767 ? (int)(this.mapId - 65536) : (int)(this.mapId);          
            this.theme = reader.ReadShort();
        }

        public override byte[] Write()
        {
            throw new NotImplementedException();
        }
    }
}
