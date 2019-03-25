using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BoxyBot.Seafight.Messages
{
    public class QuestConditionStub : Message //47.129
    {
        public const int ID = -28618;
        private int _version;
        public int id;      //name_7
        public int type;    //name_5
        public int state;   //name_16
        public List<string> values;
        public PositionStub position;
        public Dictionary<int, int> entitys;

        public QuestConditionStub(int id, int type, int state, List<string> values)
        {
            this.id = id;
            this.type = type;
            this.state = state;
            this.values = values;
        }

        public QuestConditionStub(Reader reader)
        {
            this._version = reader.ReadShort();
            this._version = 65535 & ((65535 & this._version) >> 11 | (65535 & this._version) << 5);
            this._version = this._version > 32767 ? (this._version - 65536) : (this._version);
            this.values = new List<string>();
            var i = 0;
            var max = reader.ReadByte();
            while (i < max)
            {
                this.values.Add(reader.ReadString());
                i++;
            }
            this.id = reader.ReadInt();
            this.id = this.id >> 7 | this.id << 25;
            this.state = reader.ReadShort();
            this.type = reader.ReadShort();
        }

        public override byte[] Write()
        {
            throw new NotImplementedException();
        }
    }
}
