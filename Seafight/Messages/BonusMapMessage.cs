using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BoxyBot.Seafight.Messages
{
    public class BonusMapMessage : Message //package_9.class_829;
    {
        public const int ID = 23020;
        private int _version;
        public List<BonusMapStub> maps;

        public BonusMapMessage(Reader reader)
        {
            this._version = reader.ReadShort();
            this._version = (int)(65535u & ((uint)(65535 & this._version) >> 11 | (uint)((uint)(65535 & this._version) << 5)));
            this._version = ((this._version > 32767) ? (this._version - 65536) : this._version);
            this.maps = new List<BonusMapStub>();
            int i = 0;
            int num = reader.ReadByte();
            while (i < num)
            {
                reader.ReadShort();
                maps.Add(new BonusMapStub(reader));
                i++;
            }
        }

        public override byte[] Write()
        {
            throw new NotImplementedException();
        }
    }
}
