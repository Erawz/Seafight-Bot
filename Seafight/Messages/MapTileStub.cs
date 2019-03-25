using System;

namespace BoxyBot.Seafight.Messages
{
    public class MapTileStub : Message //9.120
    {
        public const int ID = -29992;
        private int _version;
        public PositionStub position;
        public int tileId;

        public MapTileStub(Reader reader)
        {
            this._version = reader.ReadShort();
            this._version = 65535 & ((65535 & this._version) >> 5 | (65535 & this._version) << 11);
            this._version = this._version > 32767 ? (this._version - 65536) : (this._version); 
            this.position = new PositionStub(0, 0);
            this.position.Y = reader.ReadByte();
            this.position.Y = 255 & ((255 & this.position.Y) >> 4 | (255 & this.position.Y) << 4);
            this.position.Y = this.position.Y > 127 ? (this.position.Y - 256) : (this.position.Y);
            this.position.X = reader.ReadByte();
            this.position.X = 255 & ((255 & this.position.X) >> 1 | (255 & this.position.X) << 7);
            this.position.X = this.position.X > 127 ? (this.position.X - 256) : (this.position.X);
            this.tileId = reader.ReadShort();
            this.tileId = 65535 & ((65535 & this.tileId) << 10 | (65535 & this.tileId) >> 6);
            this.tileId = this.tileId > 32767 ? (this.tileId - 65536) : (this.tileId);
        }

        public override byte[] Write()
        {
            throw new NotImplementedException();
        }
    }
}
