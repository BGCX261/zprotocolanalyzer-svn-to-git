using System;
using System.Collections.Generic;
using System.Text;

namespace zyc.ZProtocolAnalyzer
{
    //Э�����������ṹ
    public enum DataTypeEnum
    {
        U8, U16, U32, S16, S32, F32, C, Reserve//C�����ַ���,�������б�ʾ.
    }

    public enum ShowTypeEnum
    {
        D, X, C//�ֱ����2,8,10,16,ASCII����
    }

    public struct ZPartDescribe
    {
        public byte BlockWord;//����������
        public DataTypeEnum DataType;
        public string Name;
        public byte Index;//�����,��0����,�ڰ�������λʱ,0Ϊ����λ.
        public ShowTypeEnum ShowType;
        public float Max, Min;
        public int CharLength;
    }

    public struct ZBlockDescribe
    {
        public string BlockName;
        public byte BlockWord;
        //public int ZPartNum;//�����Ľ�������Ϣ����
        public ZPartDescribe[] ZParts;//���Ⱥ�Ϊ13,����ܰ�������λ�����12��������,��������ݸ���Ӧ�ο�ZPartNum
    }

    public struct ZPackageDescribe
    {
        public string PackageName;
        //public int ZBlockNum;//һ�������С�ڵ���15
        public ZBlockDescribe[] ZBlocks;
    }
}