using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BoxyBot.Seafight.Messages
{
    public class QuestMessage : Message // 47.899
    {
        public const int ID = -27920;
        private int _version;
        public int questId;
        public int type;

        public const int TYPE_DETAILS = 0;
        public const int TYPE_ACCEPT = 1;
        public const int TYPE_QUIT = 2;
        public const int TYPE_BUY = 3;
        public const int TYPE_REDEEM = 4;
        public const int TYPE__MAX = 5;

        public QuestMessage(Reader reader)
        {
            this._version = reader.ReadShort();
            this._version = 65535 & ((65535 & this._version) >> 1 | (65535 & this._version) << 15);
            this._version = this._version > 32767 ? (this._version - 65536) : (this._version);
            this.questId = reader.ReadShort();
            this.questId = 65535 & ((65535 & this.questId) >> 12 | (65535 & this.questId) << 4);
            this.questId = this.questId > 32767 ? (this.questId - 65536) : (this.questId);  
            this.type = reader.ReadShort();
        }

        public QuestMessage(int questId, int type)
        {
            this.questId = questId;
            this.type = type;
        }

        public override byte[] Write()
        {
            List<byte[]> Buffer = new List<byte[]>();
            Buffer.Add(Reader.WriteShort(ID));
            Buffer.Add(Reader.WriteShort(0));
            Buffer.Add(Reader.WriteShort(65535 & ((65535 & this.questId) << 12 | (65535 & this.questId) >> 4)));
            Buffer.Add(Reader.WriteShort(type));
            return Buffer.SelectMany(bytes => bytes).ToArray<byte>();
        }
    }
}
