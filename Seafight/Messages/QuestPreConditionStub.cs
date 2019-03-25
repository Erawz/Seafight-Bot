using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BoxyBot.Seafight.Messages
{
    public class QuestPreConditionStub : Message //47.136
    {
        public const int ID = -10647;
        private int _version;
        public int type;
        public int id;
        public List<string> values;

        public QuestPreConditionStub(Reader reader)
        {
            this._version = reader.ReadShort();
            this._version = 65535 & ((65535 & this._version) << 6 | (65535 & this._version) >> 10);
			this.id = reader.ReadInt();
			this.id = this.id << 14 | this.id >> 18; this._version = this._version > 32767 ? (this._version - 65536) : (this._version);
            this.type = reader.ReadShort();
            this.values = new List<string>();
            var i = 0;
            var max = reader.ReadByte();
            while (i < max)
            {
                this.values.Add(reader.ReadString());
                i++;
            }

        }

        public override byte[] Write()
        {
            throw new NotImplementedException();
        }
    }
}
