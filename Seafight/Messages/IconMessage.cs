using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BoxyBot.Seafight.Messages
{
    public class IconMessage : Message //package_115.class_399
    {
        public const int ID = 22412;
        private int _version;
        public string iconText;
        public int actionType;
        public int type;
        public int var_344;
        public double runTime;
        public double fullTime;

        public IconMessage(Reader reader)
        {
            this._version = reader.ReadShort();
            this._version = (65535 & ((65535 & this._version) << 10 | (int)((uint)(65535 & this._version) >> 6)));
            this._version = ((this._version > 32767) ? (this._version - 65536) : this._version);
            this.actionType = reader.ReadShort();
            this.type = reader.ReadShort();
            this.iconText = reader.ReadString();
            this.runTime = reader.ReadDouble();
			this.var_344 = reader.ReadByte();
            this.var_344 = (255 & ((255 & this.var_344) >> 6 | (int)((uint)(255 & this.var_344) << 2)));
            this.var_344 = ((this.var_344 > 127) ? (this.var_344 - 256) : this.var_344);
            this.fullTime = reader.ReadDouble();
                                                                 
        }

        public override byte[] Write()
        {
            throw new NotImplementedException();
        }
    }
}
