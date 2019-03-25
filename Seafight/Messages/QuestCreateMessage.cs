using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BoxyBot.Seafight.Messages
{
    public class QuestCreateMessage : Message //47.730
    {
        public const int ID = 13024;
        private int _version;
        public int questId;
        public int var_125;
        public bool var_688;
        public List<QuestConditionStub> conditions;
        public List<QuestPreConditionStub> preConditions;
        public List<LootStub> rewards;

        public QuestCreateMessage(Reader reader)
        {
            this._version = reader.ReadShort();
            this._version = 65535 & ((65535 & this._version) >> 11 | (65535 & this._version) << 5);
            this._version = this._version > 32767 ? (this._version - 65536) : (this._version);
			this.questId = reader.ReadShort();
			this.questId = 65535 & ((65535 & this.questId) << 10 | (65535 & this.questId) >> 6);
			this.questId = this.questId > 32767 ? (this.questId - 65536) : (this.questId);
			this.var_125 = reader.ReadInt();
			this.var_125 = this.var_125 >> 16 | this.var_125 << 16;
			this.var_688 = reader.ReadBool();
            this.rewards = new List<LootStub>();
            var i = 0;
            var max = reader.ReadByte();
            while (i < max)
            {
                reader.ReadShort();
                this.rewards.Add(new LootStub(reader));
                i++;
            }
            this.preConditions = new List<QuestPreConditionStub>();
            i = 0;
            max = reader.ReadByte();
            while (i < max)
            {
                reader.ReadShort();
                this.preConditions.Add(new QuestPreConditionStub(reader));
                i++;
            }
			this.conditions = new List<QuestConditionStub>();
			i = 0;
			max = reader.ReadByte();
			while (i < max)
			{
				reader.ReadShort();
				this.conditions.Add(new QuestConditionStub(reader));
				i++;
			}
        }

        public override byte[] Write()
        {
            throw new NotImplementedException();
        }
    }
}
