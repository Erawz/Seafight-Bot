using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BoxyBot.Seafight.Messages
{
    public class ChangeDesignMessage : Message //package_7.class_723
    {
        public const int ID = -17565;
        private int _version;
        public int designId;
        public int medallionId;

        public ChangeDesignMessage(Reader reader)
        {
            this._version = reader.ReadShort();
            this._version = 65535 & ((65535 & this._version) << 4 | (65535 & this._version) >> 12);
            this._version = this._version > 32767 ? (int)(this._version - 65536) : (int)(this._version); 
            this.designId = reader.ReadShort();
            this.designId = 65535 & ((65535 & this.designId) << 9 | (65535 & this.designId) >> 7);
            this.designId = this.designId > 32767 ? (int)(this.designId - 65536) : (int)(this.designId);
            this.medallionId = reader.ReadShort();
            this.medallionId = 65535 & ((65535 & this.medallionId) >> 0 | (65535 & this.medallionId) << 16);
            this.medallionId = this.medallionId > 32767 ? (int)(this.medallionId - 65536) : (int)(this.medallionId);
        }

        public ChangeDesignMessage(int designId, int medallionId)
        {
            this.designId = designId;
            this.medallionId = medallionId;
        }

        public override byte[] Write()
        {
            List<byte[]> Buffer = new List<byte[]>();
            Buffer.Add(Reader.WriteShort(ID));
            Buffer.Add(Reader.WriteShort(0));
            Buffer.Add(Reader.WriteShort(65535 & ((65535 & this.medallionId) << 9 | (65535 & this.medallionId) >> 7)));
			Buffer.Add(Reader.WriteShort((int)(65535 & ((65535 & this.designId) << 0 | (65535 & this.designId) >> 16))));          
            return Buffer.SelectMany(bytes => bytes).ToArray<byte>();
        }
    }
}
