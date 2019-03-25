using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BoxyBot.Seafight.Messages
{
    public class ActionItemUseMessage : Message //package_88.class_312;
    {
        public const int ID = -2324;
        private int _version;
        public int itemId;

        public ActionItemUseMessage(Reader reader)
        {
            this._version = reader.ReadShort();
            this._version = (65535 & ((65535 & this._version) << 12 | (int)((uint)(65535 & this._version) >> 4)));
            this._version = ((this._version > 32767) ? (this._version - 65536) : this._version);
            this.itemId = reader.ReadShort();
            this.itemId = (65535 & ((65535 & this.itemId) << 10 | (65535 & this.itemId) >> 6));
            this.itemId = ((this.itemId > 32767) ? (this.itemId - 65536) : this.itemId);
        }

        public ActionItemUseMessage(int itemId)
        {
            this.itemId = itemId;
        }

        public override byte[] Write()
        {
            List<byte[]> Buffer = new List<byte[]>();
            Buffer.Add(Reader.WriteShort(ID));
            Buffer.Add(Reader.WriteShort(0));
            Buffer.Add(Reader.WriteShort((int)(65535 & ((65535 & this.itemId) << 13 | (65535 & this.itemId) >> 3))));
            return Buffer.SelectMany(bytes => bytes).ToArray<byte>();
        }
    }
}
