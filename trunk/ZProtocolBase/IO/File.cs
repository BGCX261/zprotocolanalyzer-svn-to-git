using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace zyc.ZProtocolAnalyzer
{
    /// <summary>
    /// 文件输入输出流
    /// </summary>
    public class File : SerialIO
    {
        string InputFile, OutputFile;
        FileStream InFS = null, OutFS = null;

        Queue<byte> InBuffer;//输入缓存
        int _ReceiveNum, _SendNum;

        /// <summary>
        /// 初始化文件输入输出流
        /// </summary>
        /// <param name="InputFile">输入文件,二进制模式,为null时关闭输入.</param>
        /// <param name="OutputFile">输出文件,二进制模式,为null时关闭输出.</param>
        public File(string InputFile, string OutputFile)
        {
            this.InputFile = InputFile;
            this.OutputFile = OutputFile;
        }


        #region SerialIO 成员

        public event ReceviceByte OnRecevice;

        /// <summary>
        /// 开启流
        /// </summary>
        public void Open()
        {
            if (InputFile != null && InputFile != "")
            {
                InFS = new FileStream(InputFile, FileMode.Open);
            }
            if (OutputFile != null && OutputFile != "")
            {
                OutFS = new FileStream(OutputFile, FileMode.Create);
            }
            InBuffer = new Queue<byte>(512);
        }

        /// <summary>
        /// 关闭流
        /// </summary>
        public void Close()
        {
            if (InFS != null)
                InFS.Close();
            if (OutFS != null)
            {
                OutFS.Flush();
                OutFS.Close();
            }
        }

        /// <summary>
        /// 写输出流,OutputFile有效时可用(不会抛出异常).
        /// </summary>
        /// <param name="Bytes">待写数据</param>
        /// <param name="Start">起始位置</param>
        /// <param name="Count">数量</param>
        public void WriteBytes(byte[] Bytes, int Start, int Count)
        {
            if (OutFS != null)
            {
                OutFS.Write(Bytes, Start, Count);
                _SendNum += Count;
                OutFS.Flush();
            }
        }

        /// <summary>
        /// 读取单个字节
        /// </summary>
        /// <returns>结果,无法读取时返回0.</returns>
        public byte ReadByte()
        {
            if (InBuffer.Count == 0)
            {
                Read(1);
            }
            if (InBuffer.Count > 0)
            {
                return InBuffer.Dequeue();
            }
            else
            {
                return 0;
            }
        }

        /// <summary>
        /// 读取指定个字节,无法读取的以0填充.
        /// </summary>
        /// <param name="Bytes">保存数组</param>
        /// <param name="Start">起始位置</param>
        /// <param name="Count">个数</param>
        public void ReadBytes(byte[] Bytes, int Start, int Count)
        {
            int d = 0;
            d = Count - InBuffer.Count;
            if (d > 0)
            {
                Read(d);
            }
            if (Count > InBuffer.Count)
            {
                Count = InBuffer.Count;
            }
            for (int i = 0; i < Count; i++)
            {
                Bytes[i] = InBuffer.Dequeue();
            }
        }

        /// <summary>
        /// 判断流是否已打开,在此当输入输出流均关闭时才为关闭.
        /// </summary>
        public bool IsOpen
        {
            get
            {
                if (InFS == null && OutFS == null)
                {
                    return false;
                }
                else
                {
                    return true;
                }
            }
        }

        /// <summary>
        /// 已收到缓存中的字节数
        /// </summary>
        public int BufferByteNum
        {
            get { return InBuffer.Count; }
        }

        public int ReceiveByteNum
        {
            get { return _ReceiveNum; }
        }

        public int SendByteNum
        {
            get { return _SendNum; }
        }

        #endregion

        /// <summary>
        /// 将数据流读入缓存
        /// </summary>
        /// <param name="Num">要读取的字节数</param>
        /// <returns>已读取的字节数,如果等于0则说明已读到尾</returns>
        int Read(int Num)
        {
            int i;
            for (i = 0; i < Num; i++)
            {
                int k = InFS.ReadByte();
                if (k == -1)
                {
                    break;
                }
                InBuffer.Enqueue((byte)k);
            }
            _ReceiveNum += i;
            return i;
        }

        /// <summary>
        /// 将数据流读入缓存,并引发事件.
        /// </summary>
        /// <param name="Num">要读取的字节数</param>
        /// <returns>已读取的字节数,如果等于0则说明已读到尾</returns>
        public int ReadStream(int Num)
        {
            Num = Read(Num);
            if (InBuffer.Count > 1)
            {
                if (OnRecevice != null)
                {
                    OnRecevice();
                }
            }
            return Num;
        }

        /// <summary>
        /// 索引到指定位置
        /// </summary>
        /// <param name="Offset">距开始的偏移量</param>
        public void Seek(long Offset)
        {
            InFS.Seek(Offset, SeekOrigin.Begin);
        }
    }
}
