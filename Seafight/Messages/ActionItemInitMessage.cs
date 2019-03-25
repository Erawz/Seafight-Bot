using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BoxyBot.Seafight.Messages
{
    public class ActionItemInitMessage : Message //package_89.class_347;
    {
        public const int ID = -25941;
        private int _version;
        public List<ActionItemStub> items;

        public ActionItemInitMessage(Reader reader)
        {
            this._version = reader.ReadShort();
            this._version = (65535 & ((65535 & this._version) << 14 | (int)((uint)(65535 & this._version) >> 2)));
            this._version = ((this._version > 32767) ? (this._version - 65536) : this._version);
            this.items = new List<ActionItemStub>();
            int i = 0;
            int num = reader.ReadByte();
            while (i < num)
            {
                reader.ReadShort();
                items.Add(new ActionItemStub(reader));
                i++;
            }
        }

        public override byte[] Write()
        {
            throw new NotImplementedException();
        }
    }
}
