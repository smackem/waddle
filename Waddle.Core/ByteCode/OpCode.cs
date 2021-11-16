namespace Waddle.Core.ByteCode
{
    public enum OpCode
    {
        PushI32,
        LoadLocalI32,
        StoreLocalI32,
        Pop,
        AddI32,
        SubI32,
        MulI32,
        DivI32,
        Ret,
        Call,
        Branch,
        BranchZero,
        EqI32,
        NeI32,
        GtI32,
        GeI32,
        LtI32,
        LeI32,
    }
}