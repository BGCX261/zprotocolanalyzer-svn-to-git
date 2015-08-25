using System;
using System.Collections.Generic;
using System.Text;

namespace zyc.ZProtocolAnalyzer
{
    public delegate void ReceviceByte();
    public interface SerialIO
    {
        event ReceviceByte OnRecevice;
        void Open();
        void Close();
        void WriteBytes(byte[] Bytes,int Start,int Count);
        byte ReadByte();
        void ReadBytes(byte[] Bytes, int Start, int Count);
        bool IsOpen { get;}
        int BufferByteNum { get;}
        int ReceiveByteNum { get;}
        int SendByteNum { get;}
    }
}
