using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BoxyBot.Seafight.Messages
{
    public class DisplayResourceIDMessage : Message //package_7.class_432;
    {
        public const int ID = -3374;
        private int _version;
        public bool var_736;
        public string message;
        public List<string> messageArgs;

        public DisplayResourceIDMessage(Reader reader)
        {
            this._version = reader.ReadShort();
            this._version = (int)(65535u & ((uint)(65535 & this._version) >> 4 | (uint)((uint)(65535 & this._version) << 12)));
            this._version = ((this._version > 32767) ? (this._version - 65536) : this._version);
            reader.ReadShort();
            this._version = reader.ReadShort();
            this._version = (int)(65535u & ((uint)(65535 & this._version) >> 5 | (uint)((uint)(65535 & this._version) << 11)));
            this._version = ((this._version > 32767) ? (this._version - 65536) : this._version);  
            this.messageArgs = new List<string>();
            int i = 0;
            int num = reader.ReadByte();
            while (i < num)
            {
                messageArgs.Add(reader.ReadString());
                i++;
            }
            this.message = reader.ReadString();
            this.var_736 = reader.ReadBool();
        }

        public override byte[] Write()
        {
            return null;
        }   
    }
}
