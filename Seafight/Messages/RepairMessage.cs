using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BoxyBot.Seafight.Messages
{
    public class RepairMessage : Message //package_7.class_300;
    {
        public const int ID = -20989;
        private int _version;
        public int modus;  

        public RepairMessage(Reader reader)
        {
            this._version = reader.ReadShort();
            this._version = (int)(65535 & ((uint)(65535 & this._version) >> 5 | (uint)((uint)(65535 & this._version) << 11)));
            this._version = ((this._version > 32767) ? (this._version - 65536) : this._version);
            this.modus = reader.ReadShort();
        }

        public RepairMessage(int modus)
        {
            this.modus = modus;
        }

        public override byte[] Write()
        {
            List<byte[]> Buffer = new List<byte[]>();
            Buffer.Add(Reader.WriteShort(ID));
            Buffer.Add(Reader.WriteShort(0));
            Buffer.Add(Reader.WriteShort(this.modus));
            return Buffer.SelectMany(bytes => bytes).ToArray<byte>();
        }
    }
}
