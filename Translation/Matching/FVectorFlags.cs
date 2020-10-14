using System;

namespace Translation.Matching
{
    [Flags]
    public enum FVectorFlags : uint
    {
        Last01 = 1 << 2,
        Last0 = 1 << 1,
        ValueType = 1 << 0,
    }
}
