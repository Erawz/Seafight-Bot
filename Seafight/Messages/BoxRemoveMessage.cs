using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BoxyBot.Seafight.Messages
{
    public class BoxRemoveMessage : Message //package_125.class_491;
    {
        public const int ID = 4370;
        private int _version;
        public double entityId;
        public int projectId;

        public BoxRemoveMessage(Reader reader)
        {
            this._version = reader.ReadShort();
            this._version = (int)(65535u & ((uint)(65535 & this._version) >> 2 | (uint)((uint)(65535 & this._version) << 14)));
            this._version = ((this._version > 32767) ? (this._version - 65536) : this._version);
            this.projectId = reader.ReadShort();
            this.projectId = (int)(65535 & ((65535 & this.projectId) >> 6 | (65535 & this.projectId) << 10));
            this.projectId = ((this.projectId > 32767) ? (this.projectId - 65536) : this.projectId);
            this.entityId = reader.ReadDouble();
        }

        public override byte[] Write()
        {
            throw new NotImplementedException();
        }
    }
}
