using System;
using System.Collections.Generic;
using System.Text;
using zyc.AutoPilotTester;

namespace zyc.ZProtocolAnalyzer
{
    class Converter
    {
        public static DataBlock CreateDataBlock(ZBlockDescribe zBlockDescribe)
        {
            DataBlock dataBlock = new DataBlock();
            dataBlock.Title = zBlockDescribe.BlockName;
            dataBlock.TitleTips = "0x" + zBlockDescribe.BlockWord.ToString("X");
            for (int i = 0; i < zBlockDescribe.ZParts.Length; i++)
            {
                DataWidget dataWidget = new DataWidget();
                dataWidget.Title = zBlockDescribe.ZParts[i].Name;
                dataWidget.TitleTips = zBlockDescribe.ZParts[i].DataType.ToString();
                dataWidget.TextTips = zBlockDescribe.ZParts[i].ShowType.ToString();
                dataWidget.DataString = "0";
                dataBlock.AddDataWidget(dataWidget);
            }
            dataBlock.SetPartsWidth(100);
            return dataBlock;
        }

        private static DataString CreateDataString(ZBlockDescribe zBlockDescribe)
        {
            DataString dataString = new DataString();
            dataString.BlockWord = zBlockDescribe.BlockWord;
            dataString.Data = new string[zBlockDescribe.ZParts.Length];
            dataString.Value = new float[zBlockDescribe.ZParts.Length];
            return dataString;
        }

        public static DataString GetDataString(ZBlock zBlock, ZBlockDescribe zBlockDescribe)
        {
            DataString dataString = CreateDataString(zBlockDescribe);
            int p = 0;
            for (int i = 0; i < zBlockDescribe.ZParts.Length; i++)//根据描述信息及数据,循环完成数据的读取及格式化.
            {
                int I; uint U; float F; string S;
                switch (zBlockDescribe.ZParts[i].DataType)
                {
                    case DataTypeEnum.U8:
                        U = zBlock.Data[p]; p += 1; F = U;
                        S = ToStr(U, zBlockDescribe.ZParts[i].ShowType);
                        break;
                    case DataTypeEnum.U16:
                        U = BitConverter.ToUInt16(zBlock.Data, p); p += 2; F = U;
                        S = ToStr(U, zBlockDescribe.ZParts[i].ShowType);
                        break;
                    case DataTypeEnum.U32:
                        U = BitConverter.ToUInt32(zBlock.Data, p); p += 4; F = U;
                        S = ToStr(U, zBlockDescribe.ZParts[i].ShowType);
                        break;
                    case DataTypeEnum.S16:
                        I = BitConverter.ToInt16(zBlock.Data, p); p += 2; F = I;
                        S = ToStr(I, zBlockDescribe.ZParts[i].ShowType);
                        break;
                    case DataTypeEnum.S32:
                        I = BitConverter.ToInt32(zBlock.Data, p); p += 4; F = I;
                        S = ToStr(I, zBlockDescribe.ZParts[i].ShowType);
                        break;
                    case DataTypeEnum.F32:
                        F = BitConverter.ToSingle(zBlock.Data, p); p += 4;
                        S = ToStr(F, zBlockDescribe.ZParts[i].ShowType);
                        break;
                    case DataTypeEnum.C:
                        F = 0;
                        char[] cs = new char[12];
                        int n = 0;
                        for (n = 0; n < zBlockDescribe.ZParts[i].CharLength / 8; n++)
                        {
                            char c = (char)zBlock.Data[n];
                            if (c == 0)
                            {
                                break;
                            }
                            cs[n] = c;
                        }
                        S = new string(cs, 0, n);
                        break;
                    case DataTypeEnum.Reserve:
                        U = zBlock.Reserve; F = U;
                        S = ToStr(U, zBlockDescribe.ZParts[i].ShowType);
                        break;
                    default:
                        F = 0;
                        S = "";
                        break;
                }
                dataString.Data[i] = S;
                dataString.Value[i] = F;
            }
            return dataString;
        }

        public static void UpdateDataBlock(DataBlock dataBlock, DataString dataString)
        {
            for (int i = 0; i < dataString.Data.Length; i++)
            {
                dataBlock.Parts[i].DataString=dataString.Data[i];
            }
        }

        public static ZBlock GetZBlock(DataBlock dataBlock, ZBlockDescribe zBlockDescribe)
        {
            ZBlock zBlock = new ZBlock();
            zBlock.Data = new byte[12];
            zBlock.Word = zBlockDescribe.BlockWord;
            
            int p = 0;
            for (int i = 0; i < zBlockDescribe.ZParts.Length; i++)
            {
                if (zBlockDescribe.ZParts[i].DataType == DataTypeEnum.C)//认为显示类型也为C
                {
                    for (int j = 0; j < dataBlock.Parts[i].DataString.Length && j < zBlockDescribe.ZParts[i].CharLength; j++)
                    {
                        zBlock.Data[p] = (byte)dataBlock.Parts[i].DataString[j];
                        p++;
                    }
                }
                else if (zBlockDescribe.ZParts[i].DataType == DataTypeEnum.F32)
                {
                    float f = float.Parse(dataBlock.Parts[i].DataString);//TODO:异常处理,取值分析
                    byte[] bs = BitConverter.GetBytes(f);
                    for (int j = 0; j < bs.Length; j++)
                    {
                        zBlock.Data[p] = bs[j];
                        p++;
                    }
                }
                else if (zBlockDescribe.ZParts[i].DataType == DataTypeEnum.Reserve)
                {
                    byte b = 0;
                    if (zBlockDescribe.ZParts[i].ShowType == ShowTypeEnum.C)
                    {
                        b = (byte)dataBlock.Parts[i].DataString[0];
                    }
                    else if (zBlockDescribe.ZParts[i].ShowType == ShowTypeEnum.D)
                    {
                        b = byte.Parse(dataBlock.Parts[i].DataString);
                    }
                    else if (zBlockDescribe.ZParts[i].ShowType == ShowTypeEnum.X)
                    {
                        b = byte.Parse(dataBlock.Parts[i].DataString, System.Globalization.NumberStyles.HexNumber);
                    }
                    zBlock.Reserve = b;
                }
                else if (zBlockDescribe.ZParts[i].DataType == DataTypeEnum.U8)
                {
                    byte b = 0;
                    if (zBlockDescribe.ZParts[i].ShowType == ShowTypeEnum.C)
                    {
                        b = (byte)dataBlock.Parts[i].DataString[0];
                    }
                    else if (zBlockDescribe.ZParts[i].ShowType == ShowTypeEnum.D)
                    {
                        b = byte.Parse(dataBlock.Parts[i].DataString);
                    }
                    else if (zBlockDescribe.ZParts[i].ShowType == ShowTypeEnum.X)
                    {
                        b = byte.Parse(dataBlock.Parts[i].DataString, System.Globalization.NumberStyles.HexNumber);
                    }
                    zBlock.Data[p] = b;
                    p++;
                }
                else if (zBlockDescribe.ZParts[i].DataType == DataTypeEnum.U16)
                {
                    UInt16 u = 0;
                    if (zBlockDescribe.ZParts[i].ShowType == ShowTypeEnum.D)
                    {
                        u = UInt16.Parse(dataBlock.Parts[i].DataString);
                    }
                    else if (zBlockDescribe.ZParts[i].ShowType == ShowTypeEnum.X)
                    {
                        u = UInt16.Parse(dataBlock.Parts[i].DataString, System.Globalization.NumberStyles.HexNumber);
                    }
                    byte[] bs = BitConverter.GetBytes(u);
                    for (int j = 0; j < bs.Length; j++)
                    {
                        zBlock.Data[p] = bs[j];
                        p++;
                    }
                }
                else if (zBlockDescribe.ZParts[i].DataType == DataTypeEnum.U32)
                {
                    UInt32 u = 0;
                    if (zBlockDescribe.ZParts[i].ShowType == ShowTypeEnum.D)
                    {
                        u = UInt32.Parse(dataBlock.Parts[i].DataString);
                    }
                    else if (zBlockDescribe.ZParts[i].ShowType == ShowTypeEnum.X)
                    {
                        u = UInt32.Parse(dataBlock.Parts[i].DataString, System.Globalization.NumberStyles.HexNumber);
                    }
                    byte[] bs = BitConverter.GetBytes(u);
                    for (int j = 0; j < bs.Length; j++)
                    {
                        zBlock.Data[p] = bs[j];
                        p++;
                    }
                }
                else if (zBlockDescribe.ZParts[i].DataType == DataTypeEnum.S16)//认为其只有十进制显示模式
                {
                    Int16 u = Int16.Parse(dataBlock.Parts[i].DataString);
                    byte[] bs = BitConverter.GetBytes(u);
                    for (int j = 0; j < bs.Length; j++)
                    {
                        zBlock.Data[p] = bs[j];
                        p++;
                    }
                }
                else if (zBlockDescribe.ZParts[i].DataType == DataTypeEnum.S32)//认为其只有十进制显示模式
                {
                    Int32 u = Int32.Parse(dataBlock.Parts[i].DataString);
                    byte[] bs = BitConverter.GetBytes(u);
                    for (int j = 0; j < bs.Length; j++)
                    {
                        zBlock.Data[p] = bs[j];
                        p++;
                    }
                }
            }
            return zBlock;
        }

        #region 格式化函数

        private static string ToStr(uint U, ShowTypeEnum ShowType)
        {
            switch (ShowType)
            {
                case ShowTypeEnum.D:
                    return U.ToString();
                case ShowTypeEnum.X:
                    return U.ToString("X");
                case ShowTypeEnum.C:
                    char c = (char)U;
                    return c.ToString();
                default:
                    break;
            }
            return "";
        }

        private static string ToStr(int I, ShowTypeEnum ShowType)
        {
            switch (ShowType)
            {
                case ShowTypeEnum.D:
                    return I.ToString();
                case ShowTypeEnum.X:
                    return I.ToString("X");
                case ShowTypeEnum.C:
                    byte c = (byte)I;
                    return c.ToString();
                default:
                    break;
            }
            return "";
        }

        private static string ToStr(float F, ShowTypeEnum ShowType)
        {
            switch (ShowType)
            {
                case ShowTypeEnum.D:
                    return F.ToString();
                case ShowTypeEnum.X:
                    return F.ToString("X");
                case ShowTypeEnum.C:
                    byte c = (byte)F;
                    return c.ToString();
                default:
                    break;
            }
            return "";
        }

        #endregion
    }
}
