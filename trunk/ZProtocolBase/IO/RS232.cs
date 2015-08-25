using System;
using System.Collections.Generic;
using System.Text;
using System.IO.Ports;
using zyc.DataLib;
using System.Threading;

namespace zyc.ZProtocolAnalyzer
{
    public class RS232 : SerialIO
    {
        SerialPort SP;

        int _ReceiveByteNum;
        int _SendByteNum;

        #region Init

        public RS232(string comString, int baudRate, int dataBits, StopBits stopBits)
        {
            SP = new SerialPort(comString, baudRate, Parity.None, dataBits, stopBits);
        }

        public RS232(string comString)
            : this(comString, 9600, 8, StopBits.One)
        { }
        public RS232(string comString, int baudRate)
            : this(comString, baudRate, 8, StopBits.One)
        { }

        #endregion

        #region SerialIO 成员

        public event ReceviceByte OnRecevice;

        /// <summary>
        /// 打开RS232串口流
        /// </summary>
        public void Open()
        {
            SP.Open();
            SP.DataReceived += new SerialDataReceivedEventHandler(SP_DataReceived);

            _ReceiveByteNum = 0;
            _SendByteNum = 0;
        }

        /// <summary>
        /// 关闭串口流
        /// </summary>
        public void Close()
        {
            SP.Close();
        }

        public void WriteBytes(byte[] Bytes, int Start, int Count)
        {
            SP.Write(Bytes, Start, Count);
            _SendByteNum += Count;
        }

        public byte ReadByte()
        {
            while (SP.BytesToRead == 0)
            {
                Thread.Sleep(10);
            }
            _ReceiveByteNum++;
            return (byte)SP.ReadByte();
        }

        public void ReadBytes(byte[] Bytes, int Start, int Count)
        {
            int read = SP.Read(Bytes, Start, Count);
            for (int i = Start+read; i < Start + Count; i++)
            {
                Bytes[i] = ReadByte();
            }
        }

        public bool IsOpen
        {
            get { return SP.IsOpen; }
        }

        /// <summary>
        /// 缓存中的字节数
        /// </summary>
        public int BufferByteNum
        {
            get { return SP.BytesToRead; }
        }

        public int ReceiveByteNum
        {
            get { return _ReceiveByteNum; }
        }

        public int SendByteNum
        {
            get { return _SendByteNum; }
        }

        #endregion

        #region Event

        void SP_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            if (e.EventType == SerialData.Chars)
            {
                if (OnRecevice != null)
                    OnRecevice();
            }
        }
        #endregion
    }
}
