using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BoxyBot.Seafight.Messages
{
    public class ActionItemAttackMessage : Message //package_89.class_769;
    {
        public const int ID = -32141;
        private int _version;
        public int itemId;
        public double entityId;
        public int projectId;

        public ActionItemAttackMessage(Reader reader)
        {
            this._version = reader.ReadShort(); 
            this.projectId = reader.ReadShort();
            this.projectId = 65535 & ((65535 & projectId) >> 4 | (65535 & projectId) << 12);
            this.projectId = projectId > 32767 ? (int)(projectId - 65536) : (int)(projectId);
            this.entityId = reader.ReadDouble();
            this.itemId = reader.ReadShort();
        }

        public ActionItemAttackMessage(int RocketID, double entityId, int projectId)
        {
            this.itemId = RocketID;
            this.entityId = entityId;
            this.projectId = projectId;
        }

        public override byte[] Write()
        {
            List<byte[]> Buffer = new List<byte[]>();
            Buffer.Add(Reader.WriteShort(ID));
            Buffer.Add(Reader.WriteShort(0));
            Buffer.Add(Reader.WriteShort(itemId));
            Buffer.Add(Reader.WriteShort(65535 & ((65535 & projectId) >> 7 | (65535 & projectId) << 9)));
            Buffer.Add(Reader.WriteDouble(entityId));
            return Buffer.SelectMany(bytes => bytes).ToArray<byte>();
        }
    }
}
