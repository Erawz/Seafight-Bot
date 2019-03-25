using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BoxyBot.Seafight.Messages
{
    public class ActionItemUpdateMessage : Message //package_89.class_470;
    {
        public const int ID = -10404;
        private int _version;
        public ActionItemStub item;

        public ActionItemUpdateMessage(Reader reader)
        {
            this._version = reader.ReadShort();
            this._version = (65535 & ((65535 & this._version) << 6 | (int)((uint)(65535 & this._version) >> 10)));
            this._version = ((this._version > 32767) ? (this._version - 65536) : this._version);
            reader.ReadShort();
            this.item = new ActionItemStub(reader);
        }

        public override byte[] Write()
        {
            throw new NotImplementedException();
        }
    }
}
