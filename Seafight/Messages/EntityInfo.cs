using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BoxyBot.Seafight.Messages
{
    public class EntityInfo : Message //41.84
    {
        public const int ID = -754;
        private int _version;
        public double entityId;
        public int projectId;

        public EntityInfo(double entityId, int projectId)
        {
            this.entityId = entityId;
            this.projectId = projectId;
        }

        public EntityInfo(Reader reader)
        {
            this._version = reader.ReadShort();
            this._version = 65535 & ((65535 & this._version) << 8 | (65535 & this._version) >> 8);
            this._version = this._version > 32767 ? (this._version - 65536) : (this._version);
            this.entityId = reader.ReadDouble();
            this.projectId = reader.ReadShort();
            this.projectId = 65535 & ((65535 & this.projectId) << 10 | (65535 & this.projectId) >> 6);
            this.projectId = this.projectId > 32767 ? (this.projectId - 65536) : (this.projectId); 
        }

        public override byte[] Write()
        {
            throw new NotImplementedException();
        }
    }
}
