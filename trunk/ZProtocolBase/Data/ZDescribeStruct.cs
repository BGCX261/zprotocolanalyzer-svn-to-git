using System;
using System.Collections.Generic;
using System.Text;

namespace zyc.ZProtocolAnalyzer
{
    //协议数据描述结构
    public enum DataTypeEnum
    {
        U8, U16, U32, S16, S32, F32, C, Reserve//C代表字符型,长度另行表示.
    }

    public enum ShowTypeEnum
    {
        D, X, C//分别代表2,8,10,16,ASCII进制
    }

    public struct ZPartDescribe
    {
        public byte BlockWord;//包的命令字
        public DataTypeEnum DataType;
        public string Name;
        public byte Index;//节序号,从0算起,在包含保留位时,0为保留位.
        public ShowTypeEnum ShowType;
        public float Max, Min;
        public int CharLength;
    }

    public struct ZBlockDescribe
    {
        public string BlockName;
        public byte BlockWord;
        //public int ZPartNum;//包含的节描述信息个数
        public ZPartDescribe[] ZParts;//长度恒为13,因可能包含保留位即最多12个数据项,储存的数据个数应参考ZPartNum
    }

    public struct ZPackageDescribe
    {
        public string PackageName;
        //public int ZBlockNum;//一般情况下小于等于15
        public ZBlockDescribe[] ZBlocks;
    }
}