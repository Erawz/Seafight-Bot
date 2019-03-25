using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BoxyBot.Seafight.Messages
{
    public class AmsUpdateMessage : Message //net.bigpoint.seafight.com.module.ship.class_485;
    {
        public const int ID = -6815;
        private int _version;
        public int space;
        public double entityId;
        public int projectId;
        public List<AmsUpdateValue> amsUpdate;

        public AmsUpdateMessage(Reader reader)
        {
            this._version = reader.ReadShort();
            this._version = (65535 & ((65535 & this._version) << 15 | (int)((uint)(65535 & this._version) >> 1)));
            this._version = ((this._version > 32767) ? (this._version - 65536) : this._version);
            this.amsUpdate = new List<AmsUpdateValue>();
            int i = 0;
            int max = reader.ReadByte();
            while (i < max)
            {
                reader.ReadShort();
                amsUpdate.Add(new AmsUpdateValue(reader));
                i++;
            }
			this.projectId = reader.ReadShort();
			this.projectId = 65535 & ((65535 & this.projectId) << 11 | (65535 & this.projectId) >> 5);
			this.projectId = this.projectId > 32767 ? (int)(this.projectId - 65536) : (int)(this.projectId);
            this.entityId = reader.ReadDouble();
            this.space = reader.ReadShort();
        }

        public override byte[] Write()
        {
            throw new NotImplementedException();
        }
    }
}
