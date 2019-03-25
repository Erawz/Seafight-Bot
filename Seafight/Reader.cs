using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BoxyBot.Seafight
{
    public class Reader
    {
        public int Index { get; set; }
        public byte[] Buffer { get; private set; }

        public Reader(int index, byte[] buffer)
        {
            this.Index = index;
            this.Buffer = buffer;
        }

        public int ReadByte()
        {
            int result;
            try
            {
                result = (int)this.Buffer[this.Index];
                this.Index++;
            }
            catch (Exception)
            {
                result = 0;
            }
            return result;
        }

        public int ReadShort()
        {
            int result;
            try
            {
                result = (int)((short)(((int)this.Buffer[this.Index] << 8) + (int)this.Buffer[1 + this.Index]));
                this.Index += 2;
            }
            catch (Exception)
            {
                result = 0;
            }
            return result;
        }

        public int ReadInt()
        {
            int result;
            try
            {
                result = ((int)this.Buffer[this.Index] << 24) + ((int)this.Buffer[1 + this.Index] << 16) + ((int)this.Buffer[2 + this.Index] << 8) + (int)this.Buffer[3 + this.Index];
                this.Index += 4;
            }
            catch (Exception)
            {
                result = 0;
            }
            return result;
        }

        public double ReadDouble()
        {
            byte[] array = new byte[8];
            double result;
            try
            {
                for (int i = 8; i > 0; i--)
                {
                    array[i - 1] = this.Buffer[this.Index];
                    this.Index++;
                }
                result = BitConverter.ToDouble(array, 0);
            }
            catch (Exception)
            {
                result = 0.0;
            }
            return result;
        }

        public double ReadFloat()
        {
            byte[] array = new byte[4];
            double result;
            try
            {
                for (int i = 4; i > 0; i--)
                {
                    array[i - 1] = this.Buffer[this.Index];
                    this.Index++;
                }
                result = (double)BitConverter.ToSingle(array, 0);
            }
            catch (Exception)
            {
                result = 0.0;
            }
            return result;
        }

        public string ReadString()
        {
            string result = "";
            int num = ReadShort();
            if (num > 0)
            {
                byte[] array = new byte[num];
                try
                {
                    for (int i = 0; i < array.Length; i++)
                    {
                        array[i] = this.Buffer[this.Index];
                        this.Index++;
                    }
                    result = Encoding.UTF8.GetString(array);
                }
                catch (Exception)
                {
                    result = "";
                }
            }
            return result;
        }

        public bool ReadBool()
        {
            bool result;
            try
            {
                result = (this.Buffer[this.Index] == 1);
                this.Index++;
            }
            catch (Exception)
            {
                result = false;
            }
            return result;
        }

        public byte[] ReadBytes(int count)
        {
            byte[] result;
            try
            {
                result = this.Buffer.Take(count).ToArray();
                this.Index += count;
            }
            catch (Exception)
            {
                result = null;
            }
            return result;
        }  

        public static byte[] WriteShort(int value)
        {
            return BitConverter.GetBytes((short)value).Reverse<byte>().ToArray<byte>();
        }

        public static byte[] WriteDouble(double value)
        {
            return BitConverter.GetBytes(value).Reverse<byte>().ToArray<byte>();
        }
        
        public static byte[] WriteInt(int value)
        {
            return BitConverter.GetBytes(value).Reverse<byte>().ToArray<byte>();
        }

        public static byte[] WriteBool(bool value)
        {
            return BitConverter.GetBytes(value).Reverse<byte>().ToArray<byte>();
        }

        public static byte[] WriteByte(int value)
        {
            return BitConverter.GetBytes(value).Reverse<byte>().ToArray<byte>();
        }

        public static byte[] WriteString(string value)
        {
            List<byte[]> Buffer = new List<byte[]>
            {
                BitConverter.GetBytes((short)Encoding.ASCII.GetBytes(value).Length).Reverse<byte>().ToArray<byte>(),
                Encoding.ASCII.GetBytes(value)
            };
            return Buffer.SelectMany(bytes => bytes).ToArray<byte>();
        }
    }
}
