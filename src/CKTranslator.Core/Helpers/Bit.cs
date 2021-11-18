namespace CKTranslator.Core
{
    /// <summary>
    ///     Класс реализует битовые операции
    /// </summary>
    public static class Bit
    {
        public static uint GetFirst(uint num, int length)
        {
            return num & Bit.Ones(length);
        }

        public static uint GetRange(uint num, int start, int length)
        {
            return (num & (Bit.Ones(length) << start)) >> start;
        }

        public static uint MaxNum(int bitCount)
        {
            return Bit.Ones(bitCount);
        }

        public static uint Ones(int length)
        {
            return uint.MaxValue >> (32 - length);
        }

        public static int OnesCount(uint num)
        {
            ulong result = num - ((num >> 1) & 0x5555555555555555UL);
            result = (result & 0x3333333333333333UL) + ((result >> 2) & 0x3333333333333333UL);
            return (int)(unchecked(((result + (result >> 4)) & 0xF0F0F0F0F0F0F0FUL) * 0x101010101010101UL) >> 56);
        }

        public static string ToString(uint num, int length)
        {
            char[] s = new char[length];
            uint maskOne = 1u << (length * 2 - 1);
            uint numOne = 1u << (length - 1);
            for (int i = 0; i < length; i++)
            {
                s[i] = (num & maskOne) == 0 ? (num & numOne) == 0 ? '0' : '1' : '-';
                maskOne >>= 1;
                numOne >>= 1;
            }

            return new string(s);
        }
    }
}