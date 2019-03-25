using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BoxyBot.Seafight.Messages
{
    public class LogoutMessage : Message //7.119
    {
        public const int ID = 5633;
        private int _version;
        public int duration;

        public LogoutMessage(Reader reader)
        {
            this._version = reader.ReadShort();
            this._version = 65535 & ((65535 & this._version) << 5 | (65535 & this._version) >> 11);
            this._version = this._version > 32767 ? (this._version - 65536) : (this._version);
            this.duration = reader.ReadShort();
            this.duration = 65535 & ((65535 & this.duration) >> 1 | (65535 & this.duration) << 15);
            this.duration = this.duration > 32767 ? (this.duration - 65536) : (this.duration);
        }

        public LogoutMessage(int duration)
        {
            this.duration = duration;
        }

        public override byte[] Write()
        {
            List<byte[]> buffer = new List<byte[]>();
            buffer.Add(Reader.WriteShort(ID));
            buffer.Add(Reader.WriteShort(0));
            buffer.Add(Reader.WriteShort(65535 & ((65535 & this.duration) << 14 | (65535 & this.duration) >> 2)));
            return buffer.SelectMany(bytes => bytes).ToArray<byte>();
        }
    }
}
