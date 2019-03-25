using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BoxyBot.Seafight.Messages
{
    public class BoardUserMessage : Message //package_90.class_316;
    {
        public const int ID = -11162;
        private int _version;
        public double entityId;
        public int projectId;

        public BoardUserMessage(Reader reader)
        {
            this._version = reader.ReadShort();
            this._version = 65535 & ((65535 & this._version) << 6 | (65535 & this._version) >> 10);
            this._version = this._version > 32767 ? (int)(this._version - 65536) : (int)(this._version);  
            this.projectId = reader.ReadShort();
            this.projectId = 65535 & ((65535 & this.projectId) >> 11 | (65535 & this.projectId) << 5);
            this.projectId = this.projectId > 32767 ? (int)(this.projectId - 65536) : (int)(this.projectId);
            this.entityId = reader.ReadDouble();
        }

        public BoardUserMessage(double entityId, int projectId)
        {
            this.entityId = entityId;
            this.projectId = projectId;
        }

        public override byte[] Write()
        {
            List<byte[]> Buffer = new List<byte[]>();
            Buffer.Add(Reader.WriteShort(ID));
            Buffer.Add(Reader.WriteShort(0));
            Buffer.Add(Reader.WriteShort((65535 & ((65535 & this.projectId) << 2 | (65535 & this.projectId) >> 14))));      
            Buffer.Add(Reader.WriteDouble(entityId));
            return Buffer.SelectMany(bytes => bytes).ToArray<byte>();
        }
    }
}
