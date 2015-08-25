using System;
using System.Collections.Generic;
using System.Text;
using zyc.DataLib;

namespace zyc.ZProtocolAnalyzer
{
    public class ZProtocolProcess
    {
        #region 内部数据

        //常量
        public const byte S1 = 0xA5;
        public const byte S2 = 0x5A;
        public const byte FrameLength = 6;
        public const byte BlockLength = 15;
        public const byte BlockNumMax = 15;
        public const byte PackageLengthMax = FrameLength + BlockLength * BlockNumMax;
        //IO
        SerialIO SerialObject;
        //接收缓存
        CycleList<byte> Buffer;
        CycleList<ZFrame> ReceiveFrame;
        CycleList<ZBlock> ReceiveBlock;
        //发送缓存
        ZFrame SendFrame;
        CycleList<ZBlock> SendBlock;
        byte[] SendBuffer;
        //事件
        public delegate void ReceviceData();
        public event ReceviceData OnReceviceFrame;
        public event ReceviceData OnReceviceBlock;
        //统计数据
        int _ReceiveFrameNum;
        int _ReceiveBlockNum;
        int _SendFrameNum;
        int _SendBlockNum;
        int _LostByteNum;
        int _LostFrameNum;
        int _LostBlockNum;
        int _ErrorFrameNum;
        int _ErrorBlockNum;

        #endregion

        /// <summary>
        /// 初始化协议处理对象
        /// </summary>
        /// <param name="SerialObject">抽象输入输出流</param>
        /// <remarks>只有当流已打开有输入数据,再执行Start()之后,此处理对象才会运作.</remarks>
        public ZProtocolProcess(SerialIO SerialObject)
        {
            this.SerialObject = SerialObject;
            Reset();
        }

        #region 运行控制

        /// <summary>
        /// 不会改变运行状态,只重新初始化数据.
        /// </summary>
        public void Reset()
        {
            Buffer = new CycleList<byte>(PackageLengthMax * 2);
            Buffer.OnThrowObeject += new CycleList<byte>.ThrowObeject(Buffer_OnThrowObeject);

            ReceiveFrame = new CycleList<ZFrame>(5);
            ReceiveFrame.OnThrowObeject += new CycleList<ZFrame>.ThrowObeject(ReceiveFrame_OnThrowObeject);

            ReceiveBlock = new CycleList<ZBlock>(50);
            ReceiveBlock.OnThrowObeject += new CycleList<ZBlock>.ThrowObeject(ReceiveBlock_OnThrowObeject);

            ZFrame SendFrame = new ZFrame();
            SendBlock = new CycleList<ZBlock>(BlockNumMax);
            SendBuffer = new byte[PackageLengthMax];

            _ReceiveFrameNum = 0;
            _ReceiveBlockNum = 0;
            _LostByteNum = 0;
            _LostFrameNum = 0;
            _LostBlockNum = 0;
            _ErrorFrameNum = 0;
            _ErrorBlockNum = 0;
        }

        /// <summary>
        /// 只有执行此函数后才开始接收数据
        /// </summary>
        public void Start()
        {
            SerialObject.OnRecevice += new ReceviceByte(SerialObject_OnReceviceByte);
        }

        /// <summary>
        /// 停止接收数据
        /// </summary>
        public void Stop()
        {
            SerialObject.OnRecevice -= SerialObject_OnReceviceByte;
        }

        #endregion

        #region 丢失记录

        void Buffer_OnThrowObeject(byte ThrowedObject)
        {
            _LostByteNum++;
            //UNDONE:发出提示
        }
        void ReceiveBlock_OnThrowObeject(ZBlock ThrowedObject)
        {
            _LostBlockNum++;
            //UNDONE:发出提示
        }

        void ReceiveFrame_OnThrowObeject(ZFrame ThrowedObject)
        {
            _LostFrameNum++;
            //UNDONE:发出提示
        }

        #endregion

        #region 数据处理

        //接收数据
        void SerialObject_OnReceviceByte()
        {
            while (SerialObject!=null&&SerialObject.BufferByteNum > 0)
            {
                Buffer.InsertObject(SerialObject.ReadByte());
                while (Buffer.Amount > 0)
                {
                    if (Buffer[0] != S1)
                    {
                        Buffer.FetchObject();
                        _LostByteNum++;
                    }
                    else
                    {
                        break;
                    }
                }
                if (CheckFrame())
                {
                    AnalyseProtocol();
                }
            }
        }

        //帧头校验
        private bool CheckFrame()
        {
            if (Buffer.Amount >= FrameLength && Buffer[0] == S1 && Buffer[1] == S2)
            {
                byte n = Buffer[5];
                if (n <= BlockNumMax)
                {
                    if (Buffer.Amount >= FrameLength + n * BlockLength)
                    {
                        return true;
                    }
                }
                else
                {
                    _ErrorFrameNum++;
                    //UNDONE:发出提示
                }
            }
            return false;
        }

        //协议解析
        private void AnalyseProtocol()
        {
            //解析帧头
            ZFrame f;
            f.S1 = Buffer.FetchObject();
            f.S2 = Buffer.FetchObject();
            f.Source = Buffer.FetchObject();
            f.Target = Buffer.FetchObject();
            f.Reserve = Buffer.FetchObject();
            f.PackageNum = Buffer.FetchObject();
            //插入帧头数据
            ReceiveFrame.InsertObject(f);
            _ReceiveFrameNum++;
            //引发事件
            if (OnReceviceFrame != null)
                OnReceviceFrame();

            //循环读取数据包
            for (int i = 0; i < f.PackageNum; i++)
            {
                //校验和
                byte sum = 0;
                for (int j = 0; j < BlockLength - 1; j++)
                {
                    sum += Buffer[j];
                }
                if (sum == Buffer[BlockLength - 1])
                {
                    ZBlock p;
                    p.Data = new byte[12];
                    p.Word = Buffer.FetchObject();
                    p.Reserve = Buffer.FetchObject();
                    for (int j = 0; j < BlockLength - 3; j++)
                    {
                        p.Data[j] = Buffer.FetchObject();
                    }
                    p.CheckSum = Buffer.FetchObject();
                    ReceiveBlock.InsertObject(p);
                    _ReceiveBlockNum++;
                }
                else
                {
                    for (int j = 0; j < BlockLength; j++)
                    {
                        Buffer.FetchObject();
                    }
                    _ErrorBlockNum++;
                }
            }
            //引发事件
            if (ReceiveBlock.Amount > 0 && OnReceviceBlock != null)
                OnReceviceBlock();
        }

        #endregion

        #region 读取接口

        /// <summary>
        /// 读取接收到缓存中的帧头
        /// </summary>
        public ZFrame FetchFrame()
        {
            return ReceiveFrame.FetchObject();
        }

        /// <summary>
        /// 读取接收到缓存中的数据包
        /// </summary>
        public ZBlock FetchBlock()
        {
            return ReceiveBlock.FetchObject();
        }

        /// <summary>
        /// 缓存中的帧头个数
        /// </summary>
        public int BufferFrameNum
        {
            get { return ReceiveFrame.Amount; }
        }

        /// <summary>
        /// 缓存中的数据包个数
        /// </summary>
        public int BufferBlockNum
        {
            get { return ReceiveBlock.Amount; }
        }

        #endregion

        #region 发送接口

        /// <summary>
        /// 写待发送帧头
        /// </summary>
        /// <param name="Source">源地址</param>
        /// <param name="Target">目标地址</param>
        /// <param name="Reserve">保留位</param>
        public void WriteFrame(byte Source, byte Target, byte Reserve)
        {
            SendFrame.S1 = S1;
            SendFrame.S2 = S2;
            SendFrame.Source = Source;
            SendFrame.Target = Target;
            SendFrame.Reserve = Reserve;
        }

        /// <summary>
        /// 写待发送帧头
        /// </summary>
        ///<param name="zFrame">帧头必须符合规范</param>
        public void WriteFrame(ZFrame zFrame)
        {
            SendFrame = zFrame;
        }

        /// <summary>
        /// 添加待发送数据包
        /// </summary>
        /// <param name="Word">命令字</param>
        /// <param name="Reserve">保留位</param>
        /// <param name="Data">数据</param>
        /// <remarks>连续添加不应超过最多数据包个数(15)</remarks>
        public void AddBlock(byte Word, byte Reserve, byte[] Data)
        {
            if (Data == null || Data.Length != BlockLength - 3)
            {
                throw new Exception("Data Length!=12");
            }
            ZBlock p = new ZBlock();
            p.Word = Word;
            p.Reserve = Reserve;
            p.Data = Data;
            SendBlock.InsertObject(p);
        }

        /// <summary>
        /// 添加待发送数据包
        /// </summary>
        /// <remarks>连续添加不应超过最多数据包个数(15)</remarks>
        public void AddBlock(ZBlock zBlock)
        {
            if (zBlock.Data == null || zBlock.Data.Length != BlockLength - 3)
            {
                throw new Exception("Data Length!=12");
            }
            SendBlock.InsertObject(zBlock);
        }

        /// <summary>
        /// 发送数据,执行完WriteFrame及AddPackage后执行此函数才发送数据
        /// </summary>
        public void Send()
        {
            SendFrame.PackageNum = (byte)SendBlock.Amount;
            SendBuffer[0] = SendFrame.S1;
            SendBuffer[1] = SendFrame.S2;
            SendBuffer[2] = SendFrame.Source;
            SendBuffer[3] = SendFrame.Target;
            SendBuffer[4] = SendFrame.Reserve;
            SendBuffer[5] = SendFrame.PackageNum;
            int j = FrameLength;
            ZBlock p;
            for (int i = 0; i < SendFrame.PackageNum; i++)
            {
                byte sum = 0;
                p = SendBlock.FetchObject();

                SendBuffer[j] = p.Word;
                sum += SendBuffer[j];
                j++;

                SendBuffer[j] = p.Reserve;
                sum += SendBuffer[j];
                j++;

                for (int k = 0; k < BlockLength - 3; k++)
                {
                    SendBuffer[j] = p.Data[k];
                    sum += SendBuffer[j];
                    j++;
                }
                SendBuffer[j] = sum;
                j++;
            }

            SerialObject.WriteBytes(SendBuffer, 0, j);
            _SendFrameNum++;
            _SendBlockNum += SendFrame.PackageNum;
        }

        /// <summary>
        /// 清除缓存中的待发送数据包
        /// </summary>
        void ClearPackage()
        {
            SendBlock.Clear();
        }

        /// <summary>
        /// 待发送的数据包个数
        /// </summary>
        public int PackageNumWaitToSend
        {
            get { return SendBlock.Amount; }
        }

        #endregion

        #region 统计数据接口

        //Byte
        public int ReceiveByteNum
        {
            get { return SerialObject.ReceiveByteNum; }
        }

        public int SendByteNum
        {
            get { return SerialObject.SendByteNum; }
        }

        public int LostByteNum
        {
            get { return _LostByteNum; }
        }

        //Frame
        public int ReceiveFrameNum
        {
            get { return _ReceiveFrameNum; }
        }

        public int SendFrameNum
        {
            get { return _SendFrameNum; }
        }

        public int ErrorFrameNum
        {
            get { return _ErrorFrameNum; }
        }

        public int LostFrameNum
        {
            get { return _LostFrameNum; }
        }

        //Block
        public int ReceiveBlockNum
        {
            get { return _ReceiveBlockNum; }
        }

        public int SendBlockNum
        {
            get { return _SendBlockNum; }
        }

        public int ErrorBlockNum
        {
            get { return _ErrorBlockNum; }
        }

        public int LostBlockNum
        {
            get { return _LostBlockNum; }
        }

        #endregion
    }
}
