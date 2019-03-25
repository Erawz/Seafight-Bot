using Fiddler;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Security;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace BoxyBot.Util
{
    public class HelpTools
    {
        public HelpTools()
        {
            _mapCoords = new Dictionary<int, int>();
        }

        public static void VoidMethod()
        {
        }

        private static Dictionary<int, int> _mapCoords;

        [DllImport("urlmon.dll", CharSet = CharSet.Ansi)]
        private static extern int UrlMkSetSessionOption(int int_4, string string_0, int int_5, int int_6);
        [DllImport("urlmon.dll")]
        [return: MarshalAs(UnmanagedType.Error)]
        private static extern int CoInternetSetFeatureEnabled(int int_4, [MarshalAs(UnmanagedType.U4)] int int_5, bool bool_0);
        [DllImport("wininet.dll", SetLastError = true)]
        public static extern bool InternetSetOption(IntPtr intptr_0, int int_4, IntPtr intptr_1, int int_5);
        [DllImport("KERNEL32.DLL", EntryPoint = "SetProcessWorkingSetSize", SetLastError = true, CallingConvention = CallingConvention.StdCall)]
        internal static extern bool SetProcessWorkingSetSize(IntPtr pProcess, int dwMinimumWorkingSetSize, int dwMaximumWorkingSetSize);
        [DllImport("KERNEL32.DLL", EntryPoint = "GetCurrentProcess", SetLastError = true, CallingConvention = CallingConvention.StdCall)]
        internal static extern IntPtr GetCurrentProcess();

        public static void CoInternetSetFeatureEnabled()
        {
            HelpTools.CoInternetSetFeatureEnabled(21, 2, true);
        }

        public static void UrlMkSetSessionOption(string string_0)
        {
            HelpTools.UrlMkSetSessionOption(268435457, string_0, string_0.Length, 0);
        }

        public static void ClearMemory(IntPtr proc)
        {
            if (Environment.OSVersion.Platform == PlatformID.Win32NT)
            {
                SetProcessWorkingSetSize(GetCurrentProcess(), -1, -1);
            }
        }

        public static bool IsVirtualMachine()
        {
            using (var searcher = new System.Management.ManagementObjectSearcher("Select * from Win32_ComputerSystem"))
            {
                using (var items = searcher.Get())
                {
                    foreach (var item in items)
                    {
                        string manufacturer = item["Manufacturer"].ToString().ToLower();
                        if ((manufacturer == "microsoft corporation" && item["Model"].ToString().ToUpperInvariant().Contains("VIRTUAL"))
                            || manufacturer.Contains("vmware")
                            || item["Model"].ToString() == "VirtualBox")
                        {
                            return true;
                        }
                    }
                }
            }
            return false;
        }

        public static string FromByteToString(int byteValue)
        {
            byte[] bytes = new byte[]
            {
                (byte)byteValue
            };
            return Encoding.GetEncoding(1252).GetString(bytes);
        }

        public static byte[] FromStringToByte(string stringValue)
        {
            return Encoding.GetEncoding(1252).GetBytes(stringValue);
        }

        public static void WriteToErrorLog(string text)
        {
            try
            {
                File.AppendAllText(Directory.GetCurrentDirectory() + "\\Error.log", text);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                BotMethods.WriteLine("There was an error while creating Error.log!\n\n" + ex.Message);
            }
        }

        public static Dictionary<int, int> GetMapCoords()
        {
            if (_mapCoords == null || _mapCoords.Count <= 2)
            {
                var _xList = new List<int>();
                var _yList = new List<int>();
                for (int x = 0; x < 600; x++)
                {
                    _xList.Add(x);
                }
                for (int y = -300; y < 300; y++)
                {
                    _yList.Add(y);
                }
                try
                {
                    foreach (var xItem in _xList)
                    {
                        foreach (var yItem in _yList)
                        {
                            if (!_mapCoords.ContainsKey(xItem))
                            {
                                _mapCoords.Add(xItem, yItem);
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    BotMethods.WriteLine(ex.ToString());
                }
            }
            return _mapCoords;
        }

        public static bool IsValidMapCoord(int x, int y)
        {
            if (GetMapCoords().Contains(new KeyValuePair<int, int>(x, y)))
            {
                return true;
            }
            return false;
        }

        public string InString(object input)
        {
            return input.ToString();
        }

        public string ToString(object input)
        {
            return Convert.ToString(input);
        }

        public bool ToBool(object input)
        {
            return Convert.ToBoolean(input);
        }

        public int ToInt(object input)
        {
            input = Regex.Replace(input.ToString(), @"[^0-9-]", "");
            return Convert.ToInt32(input);
        }

        public short ToShort(object input)
        {
            return Convert.ToInt16(input);
        }

        public double ToDouble(object input)
        {
            return Convert.ToDouble(input);
        }

        public Color ToColor(object input)
        {
            return (Color)input;
        }

        public float ToFloat(byte[] bytes)
        {
            try
            {
                Array.Reverse(bytes);
                return BitConverter.ToSingle(bytes, 0);
            }
            catch
            {
                return 0f;
            }
        }

        public string RandomString(int length)
        {
            string charPool = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvw xyz1234567890";
            StringBuilder rs = new StringBuilder();

            while (length > 0)
            {
                rs.Append(charPool[(int)(new Random(length).NextDouble() * charPool.Length)]);
                length--;
            }
            return rs.ToString();
        }

        public string GetTaskType(string line)
        {
            if (line.Contains("Box"))
                return TaskSystem.TaskTypes.BOXES;
            if (line.Contains("Chest"))
                return TaskSystem.TaskTypes.CHESTS;
            if (line.Contains("NPC"))
                return TaskSystem.TaskTypes.NPCS;
            if (line.Contains("Monster"))
                return TaskSystem.TaskTypes.MONSTERS;
            if (line.Contains("Bonus"))
                return TaskSystem.TaskTypes.BONUSMAP;
            if (line.Contains("Raid"))
                return TaskSystem.TaskTypes.RAID;
            return "null";
        }

        public bool IsValidJump(int MapID)
        {
            return !(MapID == 1 || MapID == 5 || MapID == 9 || MapID == 13 || MapID == 17 || MapID == 21 || MapID == 25 || MapID == 29 || MapID == 33 || MapID == 37 || MapID == 41 || MapID == 45 || MapID == 53 || MapID == 57 || MapID == 61 || MapID == 65 || MapID == 4 || MapID == 8 || MapID == 12 || MapID == 16 || MapID == 20 || MapID == 24 || MapID == 28 || MapID == 32 || MapID == 36 || MapID == 40 || MapID == 44 || MapID == 48 || MapID == 52 || MapID == 56 || MapID == 60 || MapID == 64 || MapID == 68);
        }

        public bool IsValidLeft(int MapID)
        {
            return !(MapID == 1 || MapID == 5 || MapID == 9 || MapID == 13 || MapID == 17 || MapID == 21 || MapID == 25 || MapID == 29 || MapID == 33 || MapID == 37 || MapID == 41 || MapID == 45 || MapID == 53 || MapID == 57 || MapID == 61 || MapID == 65);
        }

        public bool IsValidRight(int MapID)
        {
            return !(MapID == 4 || MapID == 8 || MapID == 12 || MapID == 16 || MapID == 20 || MapID == 24 || MapID == 28 || MapID == 32 || MapID == 36 || MapID == 40 || MapID == 44 || MapID == 48 || MapID == 52 || MapID == 56 || MapID == 60 || MapID == 64 || MapID == 68);
        }

        public bool IsSafeHeavenNeighbour(int MapID)
        {
            return (MapID == 5 || MapID == 16 || MapID == 25 || MapID == 40 || MapID == 49 || MapID == 64);
        }

        public bool LegitJump(int level, int mapId)
        {
            if (level > 5)
            {
                if (mapId < 5)
                {
                    return false;
                }
            }
            return true;
        }

        public bool IsFriendlyIsland(int relation)
        {
            if (relation == 1 || relation == 2 || relation == 3 || relation == 4 || relation == 5 || relation == 6)
            {
                return false;
            }
            return true;
        }

        public Point GetNextMathPoint(Point _point, Point point)
        {
            int num = (_point.Y - point.Y) / (_point.X - point.X);
            int y = _point.X * num + _point.Y;
            return new Point(num, y);
        }

        public bool IsOdd(int value)
        {
            return value % 2 != 0;
        }

        public bool IsNumeric(string input)
        {
            return int.TryParse(input, out int _out);
        }

        public byte[] GetBytes(string str)
        {
            byte[] bytes = new byte[str.Length * sizeof(char)];
            System.Buffer.BlockCopy(str.ToCharArray(), 0, bytes, 0, bytes.Length);
            return bytes;
        }

        public bool GetFileExistend(string fileName, string filePath = "CURRENT")
        {
            if (filePath == "CURRENT")
            {
                filePath = Directory.GetCurrentDirectory();
            }
            return (File.Exists(filePath + fileName));
        }

        public int GetProcessCount(string processName)
        {
            var counter = 0;
            foreach (var process in Process.GetProcesses())
            {
                if (process.ProcessName == processName || process.ProcessName.Contains(processName))
                {
                    counter++;
                }
            }
            return counter;
        }

        public int CalculateAverage(List<int> input)
        {
            return (int)((decimal)Sum(input) / input.Count);
        }

        public int Sum(List<int> input)
        {
            int result = 0;
            for (int i = 0; i < input.Count; i++)
            {
                result += input[i];
            }
            return result;
        }

        public Dictionary<string, int> SortDictionary(Dictionary<string, int> input)
        {
            var _out = new Dictionary<string, int>();
            var list = input.Keys.ToList();
            list.Sort();
            foreach (var i in list)
            {
                _out.Add(i, input[i]);
            }
            return _out;
        }

        public Dictionary<int, string> SortDictionary_2(Dictionary<int, string> input)
        {
            var _out = new Dictionary<int, string>();
            var list = input.Values.ToList();
            list.Sort();
            foreach (var i in list)
            {
                if (_out.FirstOrDefault(ii => ii.Value == i).Value == null)
                    _out.Add(input.FirstOrDefault(ii => ii.Value == i).Key, i);
            }
            return _out;
        }

        public Dictionary<T, T> ListsToDictionary<T>(List<T> keyList, List<T> valueList)
        {
            Dictionary<T, T> _out = new Dictionary<T, T>();
            try
            {
                if (valueList.Count < keyList.Count)
                {
                    MessageBox.Show($"There was an Error, while combining Lists!\nInfo: Keys({keyList.Count}) where longer than Values({valueList.Count})!", "Dictionary creation failed!");
                    return _out;
                }
                for (int i = 0; i < keyList.Count; i++)
                {
                    if (!_out.ContainsKey(keyList[i]))
                        _out.Add(keyList[i], valueList[i]);
                }
            }
            catch (Exception)
            {
                _out = new Dictionary<T, T>();
            }
            return _out;
        }

        public string Between(string input_1, string input_2, string input_3)
        {
            try
            {
                int start = input_1.IndexOf(input_2);
                int end = input_1.IndexOf(input_3, start);
                return input_1.Substring(start + input_2.Length, end - start - input_2.Length);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Parsing Error!\nhelp.Between cut off!\n" + ex.Message);
                return string.Empty;
            }
        }

        public void Replace(Session oSession, byte[] bytes)
        {
            oSession.utilCreateResponseAndBypassServer();
            oSession.ResponseBody = bytes;
        }

        private bool AcceptAllCertifications(object sender, System.Security.Cryptography.X509Certificates.X509Certificate certification, System.Security.Cryptography.X509Certificates.X509Chain chain, System.Net.Security.SslPolicyErrors sslPolicyErrors)
        {
            return true;
        }

        public List<string> ToList(string string_6, string string_7, string string_8)
        {
            List<string> list = new List<string>();
            int num = 0;
            int num2 = string_6.IndexOf(string_7) + string_7.Length;
            int num3 = string_6.IndexOf(string_8, num2);
            int num4 = num2;
            while (num2 != -1)
            {
                if (num3 == -1)
                {
                    break;
                }
                list.Add(string_6.Substring(num2, num3 - num2));
                num++;
                num2 = string_6.IndexOf(string_7, num2 + string_7.Length + 1) + string_7.Length;
                num3 = string_6.IndexOf(string_8, num2);
                if (num4 == num2)
                {
                    break;
                }
            }
            return list;
        }

        public string[] GridToCoords(int X, int Y)
        {
            int[] _loc3_ = GridToSector(X, Y);
            string _loc4_ = ((char)(65 + Math.Floor((double)(_loc3_[1] / 26)))).ToString();
            string _loc5_ = ((char)(65 + Math.Floor((double)(_loc3_[1] % 26)))).ToString();
            string _loc6_ = _loc3_[0].ToString();
            return new string[] { _loc6_, _loc4_, _loc5_ };
        }

        public int[] GridToSector(int X, int Y)
        {
            return new int[] { (int)Math.Round((double)(X + Y) / 10), (int)Math.Round((double)(X - Y) / 10) };
        }

        public string FillZeros(int param1, int param2)
        {
            var _loc3_ = "" + param1;
            var _loc4_ = param2 - _loc3_.Length;
            var _loc5_ = "";
            var _loc6_ = 0;
            while (_loc6_ < _loc4_)
            {
                _loc5_ = _loc5_ + "0";
                _loc6_++;
            }
            _loc3_ = _loc5_ + _loc3_;
            return _loc3_;
        }

        public double SimplySpeed(double Speed)
        {
            double result = 9.55;
            if (Speed > 9.5)
            {
                result = 7.5;
            }
            else if (Speed > 850.0)
            {
                result = 8.5;
            }
            return result;
        }

        public bool GridViewContains(DataGridView dataGridView_2, string word)
        {
            try
            {
                foreach (DataGridViewRow item in dataGridView_2.Rows)
                {
                    if (item.Cells[1].Value.ToString().Equals(word))
                    {
                        return true;
                    }
                }
            }
            catch (Exception)
            {
            }
            return false;
        }

        public string CreateWebClientRequest(string url)
        {
            WebClient _client = new WebClient()
            {
                Encoding = Encoding.UTF8
            };
            string result = "";
            try
            {
                result = _client.DownloadString(url);
            }
            catch (Exception ex)
            {
                BotMethods.WriteLine("Could not connect to request Url!\n" + ex.Message);
            }
            finally
            {
                if (_client != null)
                {
                    _client.Dispose();
                }
            }
            return result;
        }

        public string CreateWebRequest(string request, string url)
        {
            Uri uri = new Uri(url);
            WebResponse response;
            StreamReader reader;
            try
            {
                HttpWebRequest req = (HttpWebRequest)WebRequest.Create(uri);
                req.CookieContainer = new CookieContainer();
                req.CookieContainer.SetCookies(uri, request.Replace(";", ","));
                req.ServerCertificateValidationCallback += new RemoteCertificateValidationCallback(AcceptAllCertifications);
                response = req.GetResponse();
                reader = new StreamReader(response.GetResponseStream());
                string result = reader.ReadToEnd();
                reader.Close();
                response.Close();
                return result;
            }
            catch (Exception ex)
            {
                BotMethods.WriteLine(ex.ToString());
            }
            return "error";
        }

        public string GetCompileTime()
        {
            try
            {
                using (WebClient webclient = new WebClient())
                {
                    var cTime = webclient.DownloadString("https://int1.seafight.bigpoint.com/api/client/getCompileTime.php");
                    if (cTime.Length >= 10)
                    {
                        return cTime;
                    }
                    BotMethods.WriteLine("The compiletime response was empty, retrying!");
                    GetCompileTime();
                }
            }
            catch (Exception)
            {
                BotMethods.WriteLine("There was an error while getting Compiletime!\nRetrying in a few seconds.");
                System.Threading.Thread.Sleep(2500);
                GetCompileTime();
            }
            return "0";
        }

        public string SimplifyString(object input)
        {
            input = Regex.Replace((string)input, @"[^A-Za-z]", "");
            return (string)input;
        }

        public object ToPacket(object input)
        {
            input = Regex.Replace((string)input, @"[^A-Za-z|0-9-#;,@]", "");
            return input;
        }

        public string ToString(object[] input)
        {
            string output = ">>";
            for (int i = 0; i < input.Length; i++)
            {
                output = output + input[i] + "; >>";
            }
            return output;
        }

        public string[] FromMessage(string input)
        {
            string[] array = new string[3];
            string user = "";
            string date = "";
            string message = "";
            string[] string_2 = input.Split(';');
            user = string_2[0];
            date = string_2[2];
            message = string_2[3];
            array = new string[] { date, user, message };
            return array;
        }
    }
}
