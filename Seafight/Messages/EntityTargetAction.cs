using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BoxyBot.Seafight.Messages
{
    public class EntityTargetAction : Message //41.89
    {
        public const int ID = 25997;
        private int _version;
        public int action;
        public double entityId;

        public EntityTargetAction(Reader reader)
        {
            this._version = reader.ReadShort();
            this._version = 65535 & ((65535 & this._version) >> 11 | (65535 & this._version) << 5);
            this._version = this._version > 32767 ? (this._version - 65536) : (this._version);
            this.action = reader.ReadShort();
            this.action = 65535 & ((65535 & this.action) << 1 | (65535 & this.action) >> 15);
            this.action = this.action > 32767 ? (this.action - 65536) : (this.action);
            this.entityId = reader.ReadDouble();
        }

        public override byte[] Write()
        {
            throw new NotImplementedException();
        }
    }
}
