using System;
using System.Collections.Generic;
using System.Text;
using zyc.DataLib;
using System.IO;
using System.Text.RegularExpressions;

namespace zyc.ZProtocolAnalyzer
{
    public class ZDescribeProcess
    {
        Dictionary<byte, ZBlockDescribe> ZPackageDescribes;//储存协议包描述信息

        public ZDescribeProcess()
        {
            ZPackageDescribes = new Dictionary<byte, ZBlockDescribe>(50);
        }

        #region 解析描述文本

        /// <summary>
        /// 解析协议包描述文件
        /// </summary>
        /// <param name="File">待解析文件的路径及文件名</param>
        /// <returns>成功解析的条数</returns>
        public int LoadDescribesFile(string File)
        {
            ZPackageDescribes.Clear();//清空

            StreamReader SR = new StreamReader(File);//描述文件对象

            //循环读取解析
            while (SR.EndOfStream == false)
            {
                string s = SR.ReadLine();//读取一行

                if (s.Length == 0)//忽略空行
                {
                    continue;
                }
                if (s[0] == '#')//忽略注释行
                {
                    continue;
                }

                char[] c1 = { '$' };
                string[] s1 = s.Split(c1, StringSplitOptions.RemoveEmptyEntries);

                int k = 0;//记录读取的Part个数
                ZBlockDescribe zpd = new ZBlockDescribe();//描述信息结构
                zpd.ZParts = new ZPartDescribe[s1.Length-1];//初始化Part储存空间
                
                //循环解析每一段
                for (int i = 0; i < s1.Length; i++)
                {
                    ZPartDescribe z = GetPartInfo(s1[i]);//解析段信息
                    if (i == 0)//保存头信息
                    {
                        zpd.BlockWord = z.BlockWord;
                        zpd.BlockName = z.Name;
                    }
                    else//保存保留位及数据区信息
                    {
                        z.BlockWord = zpd.BlockWord;
                        z.Index = (byte)k;
                        zpd.ZParts[k] = z;
                        k++;
                    }
                }
                //zpd.ZPartNum = k;//储存Part个数信息
                ZPackageDescribes.Add(zpd.BlockWord, zpd);//添加到字典中
            }
            SR.Close();
            return ZPackageDescribes.Count;
        }

        //解析Part信息
        private ZPartDescribe GetPartInfo(string s)
        {
            ZPartDescribe z = new ZPartDescribe();//储存Part信息

            char[] c2 ={ '=' };
            string[] s2 = s.Split(c2, StringSplitOptions.RemoveEmptyEntries);

            for (int i = 0; i < s2.Length; i++)
            {
                StringType st = GetStringType(s2[i]);//获取节类型
                switch (st)
                {
                    case StringType.Word:
                        z.BlockWord = byte.Parse(s2[i].Substring(2),System.Globalization.NumberStyles.HexNumber);
                        break;
                    case StringType.Name:
                        z.Name = SubString(s2[i]);
                        break;
                    case StringType.Show:
                        if (s2[i] == "<X>")
                        {
                            z.ShowType = ShowTypeEnum.X;
                        }
                        if (s2[i] == "<D>")
                        {
                            z.ShowType = ShowTypeEnum.D;
                        }
                        if (s2[i] == "<C>")
                        {
                            z.ShowType = ShowTypeEnum.C;
                        }
                        break;
                    case StringType.MinMax:
                        char[] c3 ={ ',' };
                        string[] s3 = SubString(s2[i]).Split(c3, StringSplitOptions.None);
                        if (s3.Length == 2)//TODO:考虑其他可能
                        {
                            if (s3[0].Length == 0)//无穷小
                            {
                                z.Min = float.MinValue;
                            }
                            else
                            {
                                z.Min = float.Parse(s3[0]);
                            }
                            if (s3[1].Length == 0)//无穷大
                            {
                                z.Max = float.MaxValue;
                            }
                            else
                            {
                                z.Max = float.Parse(s3[1]);
                            }
                        }
                        break;
                    case StringType.DataType:
                        if (s2[i] == "U8")
                        {
                            z.DataType = DataTypeEnum.U8;
                        }
                        else if (s2[i] == "U16")
                        {
                            z.DataType = DataTypeEnum.U16;
                        }
                        else if (s2[i] == "U32")
                        {
                            z.DataType = DataTypeEnum.U32;
                        }
                        else if (s2[i] == "S16")
                        {
                            z.DataType = DataTypeEnum.S16;
                        }
                        else if (s2[i] == "S32")
                        {
                            z.DataType = DataTypeEnum.S32;
                        }
                        else if (s2[i] == "F32")
                        {
                            z.DataType = DataTypeEnum.F32;
                        }
                        else if (s2[i][0] == 'C')
                        {
                            z.DataType = DataTypeEnum.C;
                            z.CharLength = int.Parse(s2[i].Substring(1, s2[i].Length - 1));
                        }
                        else if (s2[i] == "@")//保留位
                        {
                            z.DataType = DataTypeEnum.Reserve;
                        }
                        break;
                    case StringType.Error:
                        //UNDONE
                        break;
                    default:
                        break;
                }
            }
            return z;
        }
        
        //获得去掉头尾的自字符串
        private string SubString(string s)
        {
            return s.Substring(1, s.Length - 2);
        }

        /// <summary>
        /// 节类型
        /// </summary>
        private enum StringType
        {
            Word, Name, Show, MinMax, DataType, Error//命令字,名称,显示格式,取值范围,数据类型,错误类型
        }
        
        //匹配节类型
        private StringType GetStringType(string s)
        {
            Regex Word, Name, Show, MinMax, DataType;
            Word = new Regex("^0x[0-9a-fA-F]{1,2}$");
            //Name = new Regex("^\"[a-zA-Z\\u2E80-\\u9FFF]{1}[\\w\\u2E80-\\u9FFF]{0,}\"$");
            Name = new Regex("^\"[a-zA-Z\\u0000-\\uFFFF-9]{1}[\\w\\u2E80-\\u9FFF]{0,}\"$");
            Show = new Regex("^<[XOBDC]{1}>$");
            MinMax = new Regex("^\\([0-9\\.]*,[0-9\\.]*\\)$");
            DataType = new Regex("^\\@|U8|U16|U32|S16|S32|F32|C[0-9]{1,2}$");
            if (Word.IsMatch(s, 0))
            {
                return StringType.Word;
            }
            if (Name.IsMatch(s, 0))
            {
                return StringType.Name;
            }
            if (Show.IsMatch(s, 0))
            {
                return StringType.Show;
            }
            if (MinMax.IsMatch(s, 0))
            {
                return StringType.MinMax;
            }
            if (DataType.IsMatch(s, 0))
            {
                return StringType.DataType;
            }
            return StringType.Error;
        }

        #endregion

        #region 对外接口

        /// <summary>
        /// 查询指定命令字的数据包描述信息
        /// </summary>
        /// <param name="PackageWord">命令字</param>
        /// <returns>数据包描述信息</returns>
        public ZBlockDescribe Lookup(byte PackageWord)
        {
            return ZPackageDescribes[PackageWord];
        }

        /// <summary>
        /// 当前储存的数据包描述信息条数
        /// </summary>
        public int ZPackageDescribeNum
        {
            get { return ZPackageDescribes.Count; }

        }

        public Dictionary<byte, ZBlockDescribe>.Enumerator GetEnumerator()
        {
            return ZPackageDescribes.GetEnumerator();
        }

        #endregion
    }
}
