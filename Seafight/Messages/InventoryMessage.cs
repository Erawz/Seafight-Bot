using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BoxyBot.Seafight.Messages
{
    public class InventoryMessage : Message //package_103.class_438;
    {
        public const int ID = 19024;
        private int _version;
        public int type;
        public List<InventoryItemStub> items;

        public InventoryMessage(Reader reader)
        {
            this._version = reader.ReadShort();
            this._version = (65535 & ((65535 & this._version) << 9 | (int)((uint)(65535 & this._version) >> 7)));
            this._version = ((this._version > 32767) ? (this._version - 65536) : this._version);
            this.items = new List<InventoryItemStub>();
            int i = 0;
            int num = reader.ReadShort();
            while (i < num)
            {
                reader.ReadShort();
                items.Add(new InventoryItemStub(reader));
                i++;
            } 
            this.type = reader.ReadShort();
        }

        public override byte[] Write()
        {
            throw new NotImplementedException();
        }
    }
}
