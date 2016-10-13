using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;

namespace ProjectS.CommonClasses.Util
{
    public class StreamUnity
    {
        //public enum StreamPackageTypes
        //{
        //    ByteCommand = 1,//字节命令
        //    Echo = 2//发送状态回声
        //};

        /// <summary>
        /// 创建可以进行 SOCKET 传输的不确定大小包
        /// </summary>
        /// <param name="type"></param>
        /// <param name="package_amount"></param>
        /// <param name="package_index"></param>
        /// <param name="data"></param>
        /// <param name="extra_data"></param>
        /// <returns></returns>
        //public static byte[] CreatePackage(string type, int package_amount, int package_index, byte[] data, byte[] extra_data)
        //{
        //    var serize = new Dictionary<string, object>();
        //    serize.Add(Unity.Package_Dic_Key_Type, type);
        //    serize.Add(Unity.Package_Dic_Key_Package_Amount, package_amount);
        //    serize.Add(Unity.Package_Dic_Key_Package_Index, package_index);
        //    serize.Add(Unity.Package_Dic_Key_Data, data);
        //    serize.Add(Unity.Package_Dic_Key_Data_Extra, extra_data);

        //    return PackagingDic(serize);
        //}
        public static byte[] CreatePackage(string type, int package_amount, int package_index, byte[] data, byte[] extra_data)
        {
            Unity unity = new Unity(type, package_amount, package_index, data, extra_data);
            return CreatePackage(unity);
        }

        //public static byte[] CreatePackage(Unity unity)
        //{
        //    var serize = new Dictionary<string, object>();
        //    serize.Add(Unity.Package_Dic_Key_Type, unity.Type);
        //    serize.Add(Unity.Package_Dic_Key_Package_Index, unity.PackageIndex);
        //    serize.Add(Unity.Package_Dic_Key_Data, unity.Data);
        //    serize.Add(Unity.Package_Dic_Key_Data_Extra, unity.DataExtra);

        //    return PackagingDic(serize);
        //}

        public static byte[] CreatePackage(Unity unity)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                IFormatter formatter = new BinaryFormatter();
                formatter.Serialize(ms, unity);
                return ms.GetBuffer();
            }
        }

        //public static byte[] ObjectToBytes(object obj)
        //{
        //    using (MemoryStream ms = new MemoryStream())
        //    {
        //        IFormatter formatter = new BinaryFormatter();
        //        formatter.Serialize(ms, obj);
        //        return ms.GetBuffer();
        //    }
        //}


        //public static byte[] PackagingDic(Dictionary<string, object> dic)
        //{
        //    var json = JsonConvert.SerializeObject(dic);
        //    var buffer = Encoding.UTF8.GetBytes(json);

        //    return buffer;
        //}

        //public static Dictionary<string, object> ExtractDic(byte[] package)
        //{
        //    var raw_data = Encoding.UTF8.GetString(package);

        //    raw_data = raw_data.Trim();

        //    var dic_data = JsonConvert.DeserializeObject<Dictionary<string, object>>(raw_data);

        //    return dic_data;
        //}

        /// <summary>
        /// byte[] -> Unity
        /// </summary>
        /// <param name="package"></param>
        /// <returns></returns>
        //public static Unity UnityComeTransform(byte[] package)
        //{
        //    var dic = ExtractDic(package);

        //    string type = (String)dic[Unity.Package_Dic_Key_Type];
        //    Int32 amount = (Int32)dic[Unity.Package_Dic_Key_Package_Amount];
        //    int index = (int)dic[Unity.Package_Dic_Key_Package_Index];
        //    byte[] data = (byte[])dic[Unity.Package_Dic_Key_Data];
        //    byte[] date_extra = (byte[])dic[Unity.Package_Dic_Key_Data_Extra];

        //    var unity = new Unity((String)dic[Unity.Package_Dic_Key_Type], 
        //        (int)dic[Unity.Package_Dic_Key_Package_Amount], 
        //            (int)dic[Unity.Package_Dic_Key_Package_Index], 
        //                (byte[])dic[Unity.Package_Dic_Key_Data], 
        //                    (byte[])dic[Unity.Package_Dic_Key_Data_Extra]);

        //    return unity;
        //}
        public static Unity UnityComeTransform(byte[] package)
        {
            using (MemoryStream ms = new MemoryStream(package))
            {
                IFormatter formatter = new BinaryFormatter();
                return (Unity)formatter.Deserialize(ms);
            }
        }

        //public static object BytesToObject(byte[] Bytes)
        //{
        //    using (MemoryStream ms = new MemoryStream(Bytes))
        //    {
        //        IFormatter formatter = new BinaryFormatter();
        //        return formatter.Deserialize(ms);
        //    }
        //}

        //public static String extractDicValue(Dictionary<string, object> dic, string key)
        //{
        //    if (dic.ContainsKey(key))
        //        return (String)dic[key];
        //    else
        //        return null;
        //}

        /// <summary>
        /// 创建 BYTECOMMAND 包
        /// </summary>
        /// <param name="command"></param>
        /// <returns></returns>
        public static byte[] CreateByteCommandPackage(byte command)
        {
            var data = new byte[1];
            data[0] = command;

            return CreatePackage(Unity.Package_Type_ByteCommand, 1, 0, data, null);
        }

        /// <summary>
        /// 从原始数据包获取 BYTE COMMAND 字节
        /// </summary>
        /// <param name="package"></param>
        /// <returns></returns>
        //public static byte ExtractByteCommandPackage(byte[] package)
        //{
        //    //var raw_data = Encoding.UTF8.GetString(package);

        //    //var dic_data = JsonConvert.DeserializeObject<Dictionary<string, object>>(raw_data);

        //    //var target = (byte[])dic_data["data"];

        //    var dic_data = ExtractDic(package);
        //    var target = (byte[])dic_data[Unity.Package_Dic_Key_Data];

        //    return target[0];
        //}
        public static byte ExtractByteCommandPackage(byte[] package)
        {
            var data = UnityComeTransform(package).Data;
            return data[0];
        }

        //public static bool CheckEchoStatus(byte[] package)
        //{
        //    var dic_data = ExtractDic(package);
        //    var result = (string)dic_data[Unity.Package_Dic_Key_Status];

        //    if (result.Equals(Unity.Package_Status_Success))
        //        return true;
        //    else
        //        return false;
        //}
        public static bool CheckEchoStatus(byte[] package)
        {
            var data = UnityComeTransform(package).Data;

            var result = Encoding.UTF8.GetString(data);
            result = result.Trim();

            if (result.Equals(Unity.Package_Status_Success))
                return true;
            else
                return false;
        }

        public static byte[] CreateEchoPackage(byte command)
        {
            var buffer = Encoding.UTF8.GetBytes(Unity.Package_Status_Success);
            return CreatePackage(Unity.Package_Type_Echo, 1, 0, buffer, null);
        }



    }

    /// <summary>
    /// 网络包实体
    /// </summary>
    [ Serializable ]
    public class Unity
    {
        public const string Package_Dic_Key_Type = "Type";
        public const string Package_Dic_Key_Package_Amount = "PackageAmount";
        public const string Package_Dic_Key_Package_Index = "PackageIndex";
        public const string Package_Dic_Key_Data = "Data";
        public const string Package_Dic_Key_Data_Extra = "DataExtra";
        //public const string Package_Dic_Key_Status = "Status";

        public const string Package_Type_ByteCommand = "ByteCommand";
        public const string Package_Type_Echo = "Echo";

        public const string Package_Status_Success = "Success";
        public const string Package_Status_Failed = "failed";

        private string type;
        private int packageAmount;
        private int packageIndex;
        private byte[] data;
        private byte[] dataExtra;

        public string Type { get { return type; } }
        public int PackageAmount { get { return packageAmount; } }
        public int PackageIndex { get { return packageIndex; } }
        public byte[] Data { get { return data; } }
        public byte[] DataExtra { get { return dataExtra; } }

        public Unity(String type, int packageAmount, int packageIndex, byte[] data, byte[] dataExtra)
        {
            this.type = type;
            this.packageAmount = packageAmount;
            this.packageIndex = packageIndex;
            this.data = data;
            this.dataExtra = dataExtra;
        }
    }

}
