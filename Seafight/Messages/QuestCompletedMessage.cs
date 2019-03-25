using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BoxyBot.Seafight.Messages
{
    public class QuestCompletedMessage : Message //47.563
    {
        public const int ID = -19088;
        private int _version;
        public int questId;

        public QuestCompletedMessage(Reader reader)
        {
            this._version = reader.ReadShort();
            this._version = 65535 & ((65535 & this._version) >> 4 | (65535 & this._version) << 12);
            this._version = this._version > 32767 ? (this._version - 65536) : (this._version);
            this.questId = reader.ReadShort();
            this.questId = 65535 & ((65535 & this.questId) >> 4 | (65535 & this.questId) << 12);
            this.questId = this.questId > 32767 ? (this.questId - 65536) : (this.questId);
        }

        public override byte[] Write()
        {
            throw new NotImplementedException();
        }
    }
}
