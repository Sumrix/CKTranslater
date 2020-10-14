using System.Text;

namespace CKTranslator
{
    /// <summary>
    /// Определение кодировки файлов.
    /// 
    /// 2 эвристики:
    /// 1. Русские буквы не могут быть рядом с английскими.
    /// 2. Русские буквы должны стоять кучками.
    /// </summary>
    public class EncodingDetector
    {
        private static readonly Encoding win1251;
        private static readonly Encoding win1252;

        static EncodingDetector()
        {
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
            win1251 = Encoding.GetEncoding("windows-1251");
            win1252 = Encoding.GetEncoding("windows-1252");
        }

        private enum SymbolType
        {
            EnglishLetter,
            RussianLetter,
            NotLetter
        }

        public static Encoding Detect(byte[] content)
        {
            SymbolType lastSymbolType = SymbolType.NotLetter;

            int win1252Score = 0;
            int win1251Score = 0;
            int rusLetterNum = 0;

            foreach (byte symbol in content)
            {
                if (IsEnglishLetter(symbol))
                {
                    if (lastSymbolType == SymbolType.RussianLetter)
                    {
                        win1252Score++;
                    }
                    rusLetterNum = 0;
                    lastSymbolType = SymbolType.EnglishLetter;
                }
                else if (IsRussianLetter(symbol))
                {
                    switch (lastSymbolType)
                    {
                        case SymbolType.EnglishLetter:
                            win1252Score++;
                            rusLetterNum = 0;
                            break;
                        case SymbolType.RussianLetter:
                            if (rusLetterNum >= 2)
                            {
                                win1251Score++;
                                rusLetterNum = 1;
                            }
                            else
                            {
                                rusLetterNum++;
                            }
                            break;
                        case SymbolType.NotLetter:
                            rusLetterNum = 1;
                            break;
                    }
                    lastSymbolType = SymbolType.RussianLetter;
                }
                else
                {
                    lastSymbolType = SymbolType.NotLetter;
                }
            }

            if (win1252Score * 2 < win1251Score)
            {
                return win1251;
            }

            return win1252;
        }

        private static bool IsEnglishLetter(byte symbol)
        {
            return 0x41 <= symbol && symbol <= 0x5A || // A-Z
                   0x61 <= symbol && symbol <= 0x7A;   // a-z 
        }

        private static bool IsRussianLetter(byte symbol)
        {
            return 0xC0 <= symbol;
        }
    }
}
