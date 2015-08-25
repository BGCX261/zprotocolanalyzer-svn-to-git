using System;
using System.Collections.Generic;
using System.Text;

namespace zyc.ZProtocolAnalyzer
{
    /// <summary>
    /// Э��֡ͷ���ݽṹ
    /// </summary>
    [Serializable]
    public struct ZPackage
    {
        public ZFrame Frame;
        public ZBlock[] Blocks;
    }

    /// <summary>
    /// Э��֡ͷ���ݽṹ
    /// </summary>
    [Serializable]
    public struct ZFrame
    {
        public byte S1, S2, Source, Target, Reserve, PackageNum;
    }

    /// <summary>
    /// Э�����ݿ�ṹ
    /// </summary>
    [Serializable]
    public struct ZBlock
    {
        public byte Word, Reserve;
        public byte[] Data;
        public byte CheckSum;
    }

    /// <summary>
    /// ���������ݽṹ
    /// </summary>
    public struct DataString
    {
        public byte BlockWord;
        public string[] Data;
        public float[] Value;
    }
}