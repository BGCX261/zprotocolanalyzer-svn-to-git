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
        Dictionary<byte, ZBlockDescribe> ZPackageDescribes;//����Э���������Ϣ

        public ZDescribeProcess()
        {
            ZPackageDescribes = new Dictionary<byte, ZBlockDescribe>(50);
        }

        #region ���������ı�

        /// <summary>
        /// ����Э��������ļ�
        /// </summary>
        /// <param name="File">�������ļ���·�����ļ���</param>
        /// <returns>�ɹ�����������</returns>
        public int LoadDescribesFile(string File)
        {
            ZPackageDescribes.Clear();//���

            StreamReader SR = new StreamReader(File);//�����ļ�����

            //ѭ����ȡ����
            while (SR.EndOfStream == false)
            {
                string s = SR.ReadLine();//��ȡһ��

                if (s.Length == 0)//���Կ���
                {
                    continue;
                }
                if (s[0] == '#')//����ע����
                {
                    continue;
                }

                char[] c1 = { '$' };
                string[] s1 = s.Split(c1, StringSplitOptions.RemoveEmptyEntries);

                int k = 0;//��¼��ȡ��Part����
                ZBlockDescribe zpd = new ZBlockDescribe();//������Ϣ�ṹ
                zpd.ZParts = new ZPartDescribe[s1.Length-1];//��ʼ��Part����ռ�
                
                //ѭ������ÿһ��
                for (int i = 0; i < s1.Length; i++)
                {
                    ZPartDescribe z = GetPartInfo(s1[i]);//��������Ϣ
                    if (i == 0)//����ͷ��Ϣ
                    {
                        zpd.BlockWord = z.BlockWord;
                        zpd.BlockName = z.Name;
                    }
                    else//���汣��λ����������Ϣ
                    {
                        z.BlockWord = zpd.BlockWord;
                        z.Index = (byte)k;
                        zpd.ZParts[k] = z;
                        k++;
                    }
                }
                //zpd.ZPartNum = k;//����Part������Ϣ
                ZPackageDescribes.Add(zpd.BlockWord, zpd);//��ӵ��ֵ���
            }
            SR.Close();
            return ZPackageDescribes.Count;
        }

        //����Part��Ϣ
        private ZPartDescribe GetPartInfo(string s)
        {
            ZPartDescribe z = new ZPartDescribe();//����Part��Ϣ

            char[] c2 ={ '=' };
            string[] s2 = s.Split(c2, StringSplitOptions.RemoveEmptyEntries);

            for (int i = 0; i < s2.Length; i++)
            {
                StringType st = GetStringType(s2[i]);//��ȡ������
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
                        if (s3.Length == 2)//TODO:������������
                        {
                            if (s3[0].Length == 0)//����С
                            {
                                z.Min = float.MinValue;
                            }
                            else
                            {
                                z.Min = float.Parse(s3[0]);
                            }
                            if (s3[1].Length == 0)//�����
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
                        else if (s2[i] == "@")//����λ
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
        
        //���ȥ��ͷβ�����ַ���
        private string SubString(string s)
        {
            return s.Substring(1, s.Length - 2);
        }

        /// <summary>
        /// ������
        /// </summary>
        private enum StringType
        {
            Word, Name, Show, MinMax, DataType, Error//������,����,��ʾ��ʽ,ȡֵ��Χ,��������,��������
        }
        
        //ƥ�������
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

        #region ����ӿ�

        /// <summary>
        /// ��ѯָ�������ֵ����ݰ�������Ϣ
        /// </summary>
        /// <param name="PackageWord">������</param>
        /// <returns>���ݰ�������Ϣ</returns>
        public ZBlockDescribe Lookup(byte PackageWord)
        {
            return ZPackageDescribes[PackageWord];
        }

        /// <summary>
        /// ��ǰ��������ݰ�������Ϣ����
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
