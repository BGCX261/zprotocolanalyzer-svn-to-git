using System;
using System.Collections.Generic;
using System.Text;

namespace zyc.ZProtocolAnalyzer
{
    public class SerialIOSwitch : SerialIO
    {
        SerialIO CurrSerialObj;
        public SerialIO RS232Obj, FileObj;

        public enum ReadWay
        {
            None, RS232, File
        }

        ReadWay _ReadSwitch;
        public ReadWay ReadSwitch
        {
            get
            {
                return _ReadSwitch;
            }
            set
            {
                if (_ReadSwitch == ReadWay.RS232)
                {
                    RS232Obj.OnRecevice -= SerialIOSwitch_OnRecevice;
                }
                else if (_ReadSwitch == ReadWay.File)
                {
                    FileObj.OnRecevice -= SerialIOSwitch_OnRecevice;
                }
                if (value == ReadWay.RS232 && RS232Obj != null)
                {
                    RS232Obj.OnRecevice += SerialIOSwitch_OnRecevice;
                    _ReadSwitch = ReadWay.RS232;
                }
                else if (value == ReadWay.File && FileObj != null)
                {
                    FileObj.OnRecevice += SerialIOSwitch_OnRecevice;
                    _ReadSwitch = ReadWay.File;
                }
                else
                {
                    _ReadSwitch = ReadWay.None;
                }
            }
        }

        bool _IsLog;
        public bool IsLog
        {
            get { return _IsLog; }
            set
            {
                if (value == false)
                {
                    _IsLog = false;
                }
                else if (FileObj != null && FileObj.IsOpen == true)
                {
                    _IsLog = true;
                }
            }
        }

        /// <summary>
        /// 初始化串行流切换组件对象
        /// </summary>
        /// <param name="SerialIOs">要管理的流对象</param>
        /// <remarks>需要在初始化此对象前初始化传入的流对象,不必打开,可以使用此对象的打开函数一起打开所有流对象</remarks>
        public SerialIOSwitch(SerialIO RS232Obj, SerialIO FileObj)
        {
            this.RS232Obj = RS232Obj;
            this.FileObj = FileObj;
            if (RS232Obj != null)
            {
                ReadSwitch = ReadWay.RS232;
                CurrSerialObj = RS232Obj;
            }
            else if (FileObj != null)
            {
                ReadSwitch = ReadWay.File;
                CurrSerialObj = FileObj;
            }
            else
            {
                ReadSwitch = ReadWay.None;
                CurrSerialObj = null;
            }
        }

        public SerialIOSwitch() : this(null, null) { }

        //用于在读入流接收到数据时引发上一层
        void SerialIOSwitch_OnRecevice()
        {
            if (OnRecevice != null)
                OnRecevice();
        }

        #region SerialIO 成员

        public event ReceviceByte OnRecevice;

        public void Open()
        {
            if (RS232Obj != null && RS232Obj.IsOpen == false)
            {
                RS232Obj.Open();
            }
            if (FileObj != null && FileObj.IsOpen == false)
            {
                FileObj.Open();
            }
        }

        public void Close()
        {
            if (RS232Obj != null && RS232Obj.IsOpen == true)
            {
                RS232Obj.Close();
            }
            if (FileObj != null && FileObj.IsOpen == true)
            {
                FileObj.Close();
            }
        }

        public void WriteBytes(byte[] Bytes, int Start, int Count)
        {
            if (RS232Obj != null && RS232Obj.IsOpen == true)
            {
                RS232Obj.WriteBytes(Bytes, Start, Count);
            }
        }

        public byte ReadByte()
        {
            byte b = 0;
            if (ReadSwitch == ReadWay.RS232)
            {
                b = RS232Obj.ReadByte();
            }
            else if (ReadSwitch == ReadWay.File)
            {
                b = FileObj.ReadByte();
            }
            byte[] bs = { b };
            if (ReadSwitch != ReadWay.None && IsLog == true && FileObj != null && FileObj.IsOpen == true)
            {
                FileObj.WriteBytes(bs, 0, 1);
            }
            return b;
        }

        public void ReadBytes(byte[] Bytes, int Start, int Count)
        {
            if (ReadSwitch == ReadWay.RS232)
            {
                RS232Obj.ReadBytes(Bytes, Start, Count);
            }
            else if (ReadSwitch == ReadWay.File)
            {
                FileObj.ReadBytes(Bytes, Start, Count);
            }
            if (ReadSwitch != ReadWay.None && IsLog == true && FileObj != null && FileObj.IsOpen == true)
            {
                FileObj.WriteBytes(Bytes, Start, Count);
            }
        }

        /// <summary>
        /// 只有所有的被管理流对象都打开时才为打开
        /// </summary>
        public bool IsOpen
        {
            get
            {
                if (RS232Obj != null)
                {
                    return RS232Obj.IsOpen;
                }
                else
                {
                    return false;
                }
            }
        }

        /// <summary>
        /// 读入流中缓存的字节数
        /// </summary>
        public int BufferByteNum
        {
            get
            {
                if (CurrSerialObj != null)
                {
                    return CurrSerialObj.BufferByteNum;
                }
                return 0;
            }
        }

        /// <summary>
        /// 为输出流发送的字节数
        /// </summary>
        public int ReceiveByteNum
        {
            get
            {
                if (CurrSerialObj != null)
                {
                    return CurrSerialObj.ReceiveByteNum;
                }
                return 0;
            }
        }

        /// <summary>
        /// 为读入流发送的字节数
        /// </summary>
        public int SendByteNum
        {
            get
            {
                if (CurrSerialObj != null)
                {
                    return CurrSerialObj.SendByteNum;
                }
                return 0;
            }
        }

        #endregion
    }
}
