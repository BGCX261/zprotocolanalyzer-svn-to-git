using System;
using System.Collections.Generic;
using System.Text;

namespace zyc.ZProtocolAnalyzer
{
    /// <summary>
    /// 协议帧头数据结构
    /// </summary>
    [Serializable]
    public struct ZPackage
    {
        public ZFrame Frame;
        public ZBlock[] Blocks;
    }

    /// <summary>
    /// 协议帧头数据结构
    /// </summary>
    [Serializable]
    public struct ZFrame
    {
        public byte S1, S2, Source, Target, Reserve, PackageNum;
    }

    /// <summary>
    /// 协议数据块结构
    /// </summary>
    [Serializable]
    public struct ZBlock
    {
        public byte Word, Reserve;
        public byte[] Data;
        public byte CheckSum;
    }

    /// <summary>
    /// 解析后数据结构
    /// </summary>
    public struct DataString
    {
        public byte BlockWord;
        public string[] Data;
        public float[] Value;
    }
}