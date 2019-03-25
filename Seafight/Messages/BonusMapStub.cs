using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BoxyBot.Seafight.Messages
{
    public class BonusMapStub : Message //package_9.class_729;
    {
        public const int ID = -28939;
        private int _version;
        public int mapId; //name_7;
        public int amount; //var_10;
        public int currentWave; //var_450;
        public string mapName = "Unkown Map";

        public BonusMapStub(Reader reader)
        {
            this._version = reader.ReadShort();
            this._version = (65535 & ((65535 & this._version) << 11 | (int)((uint)(65535 & this._version) >> 5)));
            this._version = ((this._version > 32767) ? (this._version - 65536) : this._version);
            this.currentWave = reader.ReadByte();
            this.currentWave = (255 & ((255 & this.currentWave) >> 0 | (int)((uint)(255 & this.currentWave) << 8)));
            this.currentWave = ((this.currentWave > 127) ? (this.currentWave - 256) : this.currentWave);
			this.amount = reader.ReadShort();
			this.amount = (int)(65535u & ((uint)(65535 & this.amount) << 1 | (uint)((uint)(65535 & this.amount) >> 15)));
			this.amount = ((this.amount > 32767) ? (this.amount - 65536) : this.amount);
			this.mapId = reader.ReadShort();
			this.mapId = (65535 & ((65535 & this.mapId) << 4 | (int)((uint)(65535 & this.mapId) >> 12)));
			this.mapId = ((this.mapId > 32767) ? (this.mapId - 65536) : this.mapId);
		}

        public override byte[] Write()
        {
            throw new NotImplementedException();
        }
    }
}
