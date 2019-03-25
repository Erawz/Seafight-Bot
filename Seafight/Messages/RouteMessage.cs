using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BoxyBot.Seafight.Messages
{
    public class RouteMessage : Message //package_9.class466
    {
        public const int ID = -25017;
        private int _version;
        public double entityId;
        public int projectId;
        public List<PositionStub> route;

        public RouteMessage(Reader reader)
        {
            this._version = reader.ReadShort();
            this._version = 65535 & ((65535 & this._version) >> 4 | (65535 & this._version) << 12);
            this._version = this._version > 32767 ? (this._version - 65536) : (this._version); 
            this.route = new List<PositionStub>();
            var i = 0;
            var max = reader.ReadShort();
            while (i < max)
            {
                reader.ReadShort();
                this.route.Add(new PositionStub(reader));
                i++;
            }
            this.projectId = reader.ReadShort();
            this.projectId = 65535 & ((65535 & this.projectId) >> 15 | (65535 & this.projectId) << 1);
            this.projectId = this.projectId > 32767 ? (this.projectId - 65536) : (this.projectId);  
            this.entityId = reader.ReadDouble();
        }

        public override byte[] Write()
        {
            return null;
        }
    }
}
