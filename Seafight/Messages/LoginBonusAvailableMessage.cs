using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BoxyBot.Seafight.Messages
{
    public class LoginBonusAvailableMessage : Message //160.811;
    {
        public const int ID = 14108;
        private int _version;

        public LoginBonusAvailableMessage(Reader reader)
        {
            this._version = reader.ReadShort();
            this._version = (65535 & ((65535 & this._version) << 10 | (int)((uint)(65535 & this._version) >> 6)));
            this._version = ((this._version > 32767) ? (this._version - 65536) : this._version);
        }

        public override byte[] Write()
        {
            throw new NotImplementedException();
        }
    }
}
