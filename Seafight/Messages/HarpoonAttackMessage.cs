using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BoxyBot.Seafight.Messages
{
    public class HarpoonAttackMessage : Message //package_89.class_698;
    {
        public const int ID = 18609;
        private int _version;
        public int projectId;
        public double entityId;
        public int harpoonId;

        public HarpoonAttackMessage(Reader reader)
        {
            this._version = reader.ReadShort();
            this._version = (65535 & ((65535 & this._version) | (65535 & this._version) << 16));
            this._version = ((this._version > 32767) ? (this._version - 65536) : this._version);           
            this.projectId = reader.ReadShort();
            this.projectId = (65535 & ((65535 & this.projectId) << 12 | (int)((uint)(65535 & this.projectId) >> 5)));
            this.projectId = ((this.projectId > 32767) ? (this.projectId - 65536) : this.projectId);
            this.entityId = reader.ReadDouble();
            this.harpoonId = reader.ReadShort();
            this.harpoonId = (65535 & ((65535 & this.harpoonId) >> 14 | (int)((uint)(65535 & this.harpoonId) << 2)));
            this.harpoonId = ((this.harpoonId > 32767) ? (this.harpoonId - 65536) : this.harpoonId);
        }

        public HarpoonAttackMessage(double entityId, int projectId, int harpoonId)
        {
            this.entityId = entityId;
            this.projectId = projectId;
            this.harpoonId = harpoonId;
        }

        public override byte[] Write()
        {
            List<byte[]> Buffer = new List<byte[]>();
            Buffer.Add(Reader.WriteShort(ID));
            Buffer.Add(Reader.WriteShort(0));
            Buffer.Add(Reader.WriteShort((int)(65535u & ((uint)(65535 & this.harpoonId) << 4 | (uint)((uint)(65535 & this.harpoonId) >> 12)))));
            Buffer.Add(Reader.WriteShort((int)(65535u & ((65535 & this.projectId) << 3 | (65535 & this.projectId) >> 13))));  
            Buffer.Add(Reader.WriteDouble(entityId));
            return Buffer.SelectMany(bytes => bytes).ToArray<byte>();
        }
    }
}
