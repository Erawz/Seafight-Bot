using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BoxyBot.Seafight.Messages
{
    public class ShipPointsStub : Message //ship_90;
    {
        public const int ID = 7643;
        private int _version;
        public int hitpoints;
        public int voodoopoints;

        public ShipPointsStub(int hitpoints, int voodoopoints)
        {
            this.hitpoints = hitpoints;
            this.voodoopoints = voodoopoints;
        }

        public ShipPointsStub(Reader reader)
        {
            this._version = reader.ReadShort();
            this._version = 65535 & ((65535 & this._version) << 1 | (65535 & this._version) >> 15);
            this._version = this._version > 32767 ? (int)(this._version - 65536) : (int)(this._version); 
            this.voodoopoints = reader.ReadInt();
            this.voodoopoints = this.voodoopoints << 16 | this.voodoopoints >> 16;
            this.hitpoints = reader.ReadInt();
            this.hitpoints = this.hitpoints << 14 | this.hitpoints >> 18;
        }

        public override byte[] Write()
        {
            throw new NotImplementedException();
        }
    }
}
