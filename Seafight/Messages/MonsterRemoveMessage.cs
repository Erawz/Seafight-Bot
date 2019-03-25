using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BoxyBot.Seafight.Messages
{
    public class MonsterRemoveMessage //package_122.class_428;
    {
        public const int ID = 427;
        private int _version;
        public int projectId;
        public double entityId;

        public MonsterRemoveMessage(Reader reader)
        {
            this._version = reader.ReadShort();
            this._version = (65535 & ((65535 & this._version) << 2 | (int)((uint)(65535 & this._version) >> 14)));
            this._version = ((this._version > 32767) ? (this._version - 65536) : this._version);
            this.projectId = reader.ReadShort();
            this.projectId = (65535 & ((65535 & this.projectId) << 1 | (int)((uint)(65535 & this.projectId) >> 15)));
            this.projectId = ((this.projectId > 32767) ? (this.projectId - 65536) : this.projectId);  
            this.entityId = reader.ReadDouble();
        }
    }
}
