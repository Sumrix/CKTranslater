using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace Core
{
    /// <summary>
    ///     File encoding detection.
    ///     <para>
    ///         2 heuristics are used:
    ///         <list type="number">
    ///             <item>Russian letters cannot be next to English ones.</item>
    ///             <item>Russian letters should be in groups.</item>
    ///         </list>
    ///     </para>
    /// </summary>
    public class EncodingDetector
    {
        private static readonly Encoding win1251;
        private static readonly Encoding win1252;

        static EncodingDetector()
        {
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
            EncodingDetector.win1251 = Encoding.GetEncoding("windows-1251");
            EncodingDetector.win1252 = Encoding.GetEncoding("windows-1252");
        }

        public static Encoding Detect(IEnumerable<byte> content)
        {
            SymbolType lastSymbolType = SymbolType.NotLetter;

            int win1252Score = 0;
            int win1251Score = 0;
            int rusLetterNum = 0;

            foreach (byte symbol in content)
            {
                if (EncodingDetector.IsEnglishLetter(symbol))
                {
                    if (lastSymbolType == SymbolType.RussianLetter)
                    {
                        win1252Score++;
                    }

                    rusLetterNum = 0;
                    lastSymbolType = SymbolType.EnglishLetter;
                }
                else if (EncodingDetector.IsRussianLetter(symbol))
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
                        default:
                            throw new InvalidEnumArgumentException(nameof(lastSymbolType), (int) lastSymbolType,
                                typeof(SymbolType));
                    }

                    lastSymbolType = SymbolType.RussianLetter;
                }
                else
                {
                    lastSymbolType = SymbolType.NotLetter;
                }
            }

            return win1252Score * 2 < win1251Score
                ? EncodingDetector.win1251
                : EncodingDetector.win1252;
        }

        private static bool IsEnglishLetter(byte symbol)
        {
            return 0x41 <= symbol && symbol <= 0x5A || // A-Z
                   0x61 <= symbol && symbol <= 0x7A; // a-z 
        }

        private static bool IsRussianLetter(byte symbol)
        {
            return 0xC0 <= symbol;
        }

        private enum SymbolType
        {
            EnglishLetter,
            RussianLetter,
            NotLetter
        }
    }
}