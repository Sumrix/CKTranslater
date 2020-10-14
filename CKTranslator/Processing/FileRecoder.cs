using System;
using System.IO;
using System.Text;

namespace CKTranslator.Processing
{
    public static class FileRecoder
    {
        private static readonly Encoding win1251;
        private static readonly Encoding win1252;
        private static byte[] resultContent = Array.Empty<byte>();

        static FileRecoder()
        {
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
            win1251 = Encoding.GetEncoding("windows-1251");
            win1252 = Encoding.GetEncoding("windows-1252");
        }

        public static Event Recode(FileContext context)
        {
            byte[] content = File.ReadAllBytes(context.FullFileName);
            Encoding encoding = EncodingDetector.Detect(content);

            if (encoding == win1252)
            {
                int length = (int)(content.Length * 1.5);
                if (resultContent.Length < length)
                {
                    resultContent = new byte[length];
                }

                using (MemoryStream mem = new MemoryStream(resultContent))
                {
                    foreach (byte b in content)
                    {
                        if (b >= 0x80)
                        {
                            byte[] buffer = charmap1[b - 0x80];
                            mem.Write(buffer, 0, buffer.Length);
                        }
                        else
                        {
                            mem.WriteByte(b);
                        }
                    }

                    length = (int)mem.Position;
                }

                using (FileStream stream = File.OpenWrite(context.FullFileName))
                {
                    stream.Write(resultContent, 0, length);
                }
            }

            return null;
        }

        private static readonly byte[][] charmap =
        {
            new byte[]{ 0x88 },       // 0x80: € = €
            new byte[]{ 0x81 },       // 0x81
            new byte[]{ 0x82 },       // 0x82
            new byte[]{ 0x66 },       // 0x83: ƒ = f
            new byte[]{ 0x84 },       // 0x84
            new byte[]{ 0x85 },       // 0x85
            new byte[]{ 0x86 },       // 0x86
            new byte[]{ 0x87 },       // 0x87
            new byte[]{ 0x5E },       // 0x88: ˆ = ^
            new byte[]{ 0x89 },       // 0x89
            new byte[]{ 0x53, 0x68 }, // 0x8A: Š = Sh
            new byte[]{ 0x8B },       // 0x8B
            new byte[]{ 0x45 },       // 0x8C: Œ = E
            new byte[]{ 0x8D },       // 0x8D
            new byte[]{ 0x5A, 0x68 }, // 0x8E: Ž = Zh
            new byte[]{ 0x8F },       // 0x8F
            new byte[]{ 0x90 },       // 0x90
            new byte[]{ 0x91 },       // 0x91
            new byte[]{ 0x92 },       // 0x92
            new byte[]{ 0x93 },       // 0x93
            new byte[]{ 0x94 },       // 0x94
            new byte[]{ 0x95 },       // 0x95
            new byte[]{ 0x96 },       // 0x96
            new byte[]{ 0x97 },       // 0x97
            new byte[]{ 0x7E },       // 0x98: ˜ = ~
            new byte[]{ 0x99 },       // 0x99
            new byte[]{ 0x73, 0x68 }, // 0x9A: š = sh
            new byte[]{ 0x9B },       // 0x9B
            new byte[]{ 0x65 },       // 0x9C: œ = e
            new byte[]{ 0x9D },       // 0x9D
            new byte[]{ 0x7A, 0x68 }, // 0x9E: ž = zh
            new byte[]{ 0x59 },       // 0x9F: Ÿ = Y
            new byte[]{ 0xA0 },       // 0xA0
            new byte[]{ 0x21 },       // 0xA1: ¡ = !
            new byte[]{ 0x63 },       // 0xA2: ¢ = c
            new byte[]{ 0x4C },       // 0xA3: £ = L
            new byte[]{ 0xA4 },       // 0xA4
            new byte[]{ 0x59 },       // 0xA5: ¥ = Y
            new byte[]{ 0xA6 },       // 0xA6
            new byte[]{ 0xA7 },       // 0xA7
            new byte[]{ 0xA8 },       // 0xA8
            new byte[]{ 0xA9 },       // 0xA9
            new byte[]{ 0xB0 },       // 0xAA: ª = °
            new byte[]{ 0xAB },       // 0xAB
            new byte[]{ 0xAC },       // 0xAC
            new byte[]{ 0xAD },       // 0xAD
            new byte[]{ 0xAE },       // 0xAE
            new byte[]{ 0x2D },       // 0xAF: ¯ = -
            new byte[]{ 0xB0 },       // 0xB0
            new byte[]{ 0xB1 },       // 0xB1
            new byte[]{ 0x32 },       // 0xB2: ² = 2
            new byte[]{ 0x33 },       // 0xB3: ³ = 3
            new byte[]{ 0x27 },       // 0xB4: ´ = '
            new byte[]{ 0xB5 },       // 0xB5
            new byte[]{ 0xB6 },       // 0xB6
            new byte[]{ 0xB7 },       // 0xB7
            new byte[]{ 0x2C },       // 0xB8: ¸ = ,
            new byte[]{ 0x31 },       // 0xB9: ¹ = 1
            new byte[]{ 0xB0 },       // 0xBA: º = °
            new byte[]{ 0xBB },       // 0xBB
            new byte[]{ 0x31, 0x2F, 0x34 }, // 0xBC: ¼ = 1/4
            new byte[]{ 0x31, 0x2F, 0x32 }, // 0xBD: ½ = 1/2
            new byte[]{ 0x33, 0x2F, 0x34 }, // 0xBE: ¾ = 3/4
            new byte[]{ 0x3F },       // 0xBF: ¿ = ?
            new byte[]{ 0x41 },       // 0xC0: À = A
            new byte[]{ 0x41 },       // 0xC1: Á = A
            new byte[]{ 0x41 },       // 0xC2: Â = A
            new byte[]{ 0x41 },       // 0xC3: Ã = A
            new byte[]{ 0x41, 0x65 }, // 0xC4: Ä = Ae
            new byte[]{ 0x41 },       // 0xC5: Å = A
            new byte[]{ 0x41, 0x65 }, // 0xC6: Æ = Ae
            new byte[]{ 0x43 },       // 0xC7: Ç = C
            new byte[]{ 0x45 },       // 0xC8: È = E
            new byte[]{ 0x45 },       // 0xC9: É = E
            new byte[]{ 0x45 },       // 0xCA: Ê = E
            new byte[]{ 0x45 },       // 0xCB: Ë = E
            new byte[]{ 0x49 },       // 0xCC: Ì = I
            new byte[]{ 0x49 },       // 0xCD: Í = I
            new byte[]{ 0x49 },       // 0xCE: Î = I
            new byte[]{ 0x49 },       // 0xCF: Ï = I
            new byte[]{ 0x44, 0x68 }, // 0xD0: Ð = Dh
            new byte[]{ 0x4E },       // 0xD1: Ñ = N
            new byte[]{ 0x4F },       // 0xD2: Ò = O
            new byte[]{ 0x4F },       // 0xD3: Ó = O
            new byte[]{ 0x4F },       // 0xD4: Ô = O
            new byte[]{ 0x4F },       // 0xD5: Õ = O
            new byte[]{ 0x4F, 0x65 }, // 0xD6: Ö = Oe
            new byte[]{ 0xD7 },       // 0xD7
            new byte[]{ 0x4F, 0x65 }, // 0xD8: Ø = Oe
            new byte[]{ 0x55 },       // 0xD9: Ù = U
            new byte[]{ 0x55 },       // 0xDA: Ú = U
            new byte[]{ 0x55 },       // 0xDB: Û = U
            new byte[]{ 0x55, 0x65 }, // 0xDC: Ü = Ue
            new byte[]{ 0x59 },       // 0xDD: Ý = Y
            new byte[]{ 0x54, 0x68 }, // 0xDE: Þ = Th
            new byte[]{ 0x73, 0x73 }, // 0xDF: ß = ss
            new byte[]{ 0x61 },       // 0xE0: à = a
            new byte[]{ 0x61 },       // 0xE1: á = a
            new byte[]{ 0x61 },       // 0xE2: â = a
            new byte[]{ 0x61 },       // 0xE3: ã = a
            new byte[]{ 0x61, 0x65 }, // 0xE4: ä = ae
            new byte[]{ 0x61 },       // 0xE5: å = a
            new byte[]{ 0x61, 0x65 }, // 0xE6: æ = ae
            new byte[]{ 0x63 },       // 0xE7: ç = c
            new byte[]{ 0x65 },       // 0xE8: è = e
            new byte[]{ 0x65 },       // 0xE9: é = e
            new byte[]{ 0x65 },       // 0xEA: ê = e
            new byte[]{ 0x65 },       // 0xEB: ë = e
            new byte[]{ 0x69 },       // 0xEC: ì = i
            new byte[]{ 0x69 },       // 0xED: í = i
            new byte[]{ 0x69 },       // 0xEE: î = i
            new byte[]{ 0x69 },       // 0xEF: ï = i
            new byte[]{ 0x64, 0x68 }, // 0xF0: ð = dh
            new byte[]{ 0x6E },       // 0xF1: ñ = n
            new byte[]{ 0x6F },       // 0xF2: ò = o
            new byte[]{ 0x6F },       // 0xF3: ó = o
            new byte[]{ 0x6F },       // 0xF4: ô = o
            new byte[]{ 0x6F },       // 0xF5: õ = o
            new byte[]{ 0x6F, 0x65 }, // 0xF6: ö = oe
            new byte[]{ 0xF7 },       // 0xF7
            new byte[]{ 0x6F, 0x65 }, // 0xF8: ø = oe
            new byte[]{ 0x75 },       // 0xF9: ù = u
            new byte[]{ 0x75 },       // 0xFA: ú = u
            new byte[]{ 0x75 },       // 0xFB: û = u
            new byte[]{ 0x75, 0x65 }, // 0xFC: ü = ue
            new byte[]{ 0x79 },       // 0xFD: ý = y
            new byte[]{ 0x74, 0x68 }, // 0xFE: þ = th
            new byte[]{ 0x79 },       // 0xFF: ÿ = y
        };

        private static readonly byte[][] charmap1 =
        {
            new byte[]{ 0x88 },       // 0x80: € = €
            new byte[]{ 0x81 },       // 0x81
            new byte[]{ 0x82 },       // 0x82
            new byte[]{ 0x66 },       // 0x83: ƒ = f
            new byte[]{ 0x84 },       // 0x84
            new byte[]{ 0x85 },       // 0x85
            new byte[]{ 0x86 },       // 0x86
            new byte[]{ 0x87 },       // 0x87
            new byte[]{ 0x5E },       // 0x88: ˆ = ^
            new byte[]{ 0x89 },       // 0x89
            new byte[]{ 0x53, 0x31 }, // 0x8A: Š = S1
            new byte[]{ 0x8B },       // 0x8B
            new byte[]{ 0x45, 0x35 }, // 0x8C: Œ = E5
            new byte[]{ 0x8D },       // 0x8D
            new byte[]{ 0x5A, 0x31 }, // 0x8E: Ž = Z1
            new byte[]{ 0x8F },       // 0x8F
            new byte[]{ 0x90 },       // 0x90
            new byte[]{ 0x91 },       // 0x91
            new byte[]{ 0x92 },       // 0x92
            new byte[]{ 0x93 },       // 0x93
            new byte[]{ 0x94 },       // 0x94
            new byte[]{ 0x95 },       // 0x95
            new byte[]{ 0x96 },       // 0x96
            new byte[]{ 0x97 },       // 0x97
            new byte[]{ 0x7E },       // 0x98: ˜ = ~
            new byte[]{ 0x99 },       // 0x99
            new byte[]{ 0x73, 0x31 }, // 0x9A: š = s1
            new byte[]{ 0x9B },       // 0x9B
            new byte[]{ 0x65, 0x35 }, // 0x9C: œ = e5
            new byte[]{ 0x9D },       // 0x9D
            new byte[]{ 0x7A, 0x31 }, // 0x9E: ž = z1
            new byte[]{ 0x59, 0x32 }, // 0x9F: Ÿ = Y2
            new byte[]{ 0xA0 },       // 0xA0
            new byte[]{ 0x21 },       // 0xA1: ¡ = !
            new byte[]{ 0x63 },       // 0xA2: ¢ = c
            new byte[]{ 0x4C },       // 0xA3: £ = L
            new byte[]{ 0xA4 },       // 0xA4
            new byte[]{ 0x59 },       // 0xA5: ¥ = Y
            new byte[]{ 0xA6 },       // 0xA6
            new byte[]{ 0xA7 },       // 0xA7
            new byte[]{ 0xA8 },       // 0xA8
            new byte[]{ 0xA9 },       // 0xA9
            new byte[]{ 0xB0 },       // 0xAA: ª = °
            new byte[]{ 0xAB },       // 0xAB
            new byte[]{ 0xAC },       // 0xAC
            new byte[]{ 0xAD },       // 0xAD
            new byte[]{ 0xAE },       // 0xAE
            new byte[]{ 0x2D },       // 0xAF: ¯ = -
            new byte[]{ 0xB0 },       // 0xB0
            new byte[]{ 0xB1 },       // 0xB1
            new byte[]{ 0x32 },       // 0xB2: ² = 2
            new byte[]{ 0x33 },       // 0xB3: ³ = 3
            new byte[]{ 0x27 },       // 0xB4: ´ = '
            new byte[]{ 0xB5 },       // 0xB5
            new byte[]{ 0xB6 },       // 0xB6
            new byte[]{ 0xB7 },       // 0xB7
            new byte[]{ 0x2C },       // 0xB8: ¸ = ,
            new byte[]{ 0x31 },       // 0xB9: ¹ = 1
            new byte[]{ 0xB0 },       // 0xBA: º = °
            new byte[]{ 0xBB },       // 0xBB
            new byte[]{ 0x31, 0x2F, 0x34 }, // 0xBC: ¼ = 1/4
            new byte[]{ 0x31, 0x2F, 0x32 }, // 0xBD: ½ = 1/2
            new byte[]{ 0x33, 0x2F, 0x34 }, // 0xBE: ¾ = 3/4
            new byte[]{ 0x3F },       // 0xBF: ¿ = ?
            new byte[]{ 0x41, 0x31 }, // 0xC0: À = A1
            new byte[]{ 0x41, 0x32 }, // 0xC1: Á = A2
            new byte[]{ 0x41, 0x33 }, // 0xC2: Â = A3
            new byte[]{ 0x41, 0x34 }, // 0xC3: Ã = A4
            new byte[]{ 0x41, 0x35 }, // 0xC4: Ä = A5
            new byte[]{ 0x41, 0x36 }, // 0xC5: Å = A6
            new byte[]{ 0x41, 0x37 }, // 0xC6: Æ = A7
            new byte[]{ 0x43, 0x31 }, // 0xC7: Ç = C1
            new byte[]{ 0x45, 0x31 }, // 0xC8: È = E1
            new byte[]{ 0x45, 0x32 }, // 0xC9: É = E2
            new byte[]{ 0x45, 0x33 }, // 0xCA: Ê = E3
            new byte[]{ 0x45, 0x34 }, // 0xCB: Ë = E4
            new byte[]{ 0x49, 0x31 }, // 0xCC: Ì = I1
            new byte[]{ 0x49, 0x32 }, // 0xCD: Í = I2
            new byte[]{ 0x49, 0x33 }, // 0xCE: Î = I3
            new byte[]{ 0x49, 0x34 }, // 0xCF: Ï = I4
            new byte[]{ 0x44, 0x31 }, // 0xD0: Ð = D1
            new byte[]{ 0x4E, 0x31 }, // 0xD1: Ñ = N1
            new byte[]{ 0x4F, 0x31 }, // 0xD2: Ò = O1
            new byte[]{ 0x4F, 0x32 }, // 0xD3: Ó = O2
            new byte[]{ 0x4F, 0x33 }, // 0xD4: Ô = O3
            new byte[]{ 0x4F, 0x34 }, // 0xD5: Õ = O4
            new byte[]{ 0x4F, 0x35 }, // 0xD6: Ö = O5
            new byte[]{ 0xD7 },       // 0xD7
            new byte[]{ 0x4F, 0x36 }, // 0xD8: Ø = O6
            new byte[]{ 0x55, 0x31 }, // 0xD9: Ù = U1
            new byte[]{ 0x55, 0x32 }, // 0xDA: Ú = U2
            new byte[]{ 0x55, 0x33 }, // 0xDB: Û = U3
            new byte[]{ 0x55, 0x34 }, // 0xDC: Ü = U4
            new byte[]{ 0x59, 0x31 }, // 0xDD: Ý = Y1
            new byte[]{ 0x54, 0x31 }, // 0xDE: Þ = T1
            new byte[]{ 0x73, 0x32 }, // 0xDF: ß = s2
            new byte[]{ 0x61, 0x31 }, // 0xE0: à = a1
            new byte[]{ 0x61, 0x32 }, // 0xE1: á = a2
            new byte[]{ 0x61, 0x33 }, // 0xE2: â = a3
            new byte[]{ 0x61, 0x34 }, // 0xE3: ã = a4
            new byte[]{ 0x61, 0x35 }, // 0xE4: ä = a5
            new byte[]{ 0x61, 0x36 }, // 0xE5: å = a6
            new byte[]{ 0x61, 0x37 }, // 0xE6: æ = a7
            new byte[]{ 0x63, 0x31 }, // 0xE7: ç = c1
            new byte[]{ 0x65, 0x31 }, // 0xE8: è = e1
            new byte[]{ 0x65, 0x32 }, // 0xE9: é = e2
            new byte[]{ 0x65, 0x33 }, // 0xEA: ê = e3
            new byte[]{ 0x65, 0x34 }, // 0xEB: ë = e4
            new byte[]{ 0x69, 0x31 }, // 0xEC: ì = i1
            new byte[]{ 0x69, 0x32 }, // 0xED: í = i2
            new byte[]{ 0x69, 0x33 }, // 0xEE: î = i3
            new byte[]{ 0x69, 0x34 }, // 0xEF: ï = i4
            new byte[]{ 0x64, 0x31 }, // 0xF0: ð = d1
            new byte[]{ 0x6E, 0x31 }, // 0xF1: ñ = n1
            new byte[]{ 0x6F, 0x31 }, // 0xF2: ò = o1
            new byte[]{ 0x6F, 0x32 }, // 0xF3: ó = o2
            new byte[]{ 0x6F, 0x33 }, // 0xF4: ô = o3
            new byte[]{ 0x6F, 0x34 }, // 0xF5: õ = o4
            new byte[]{ 0x6F, 0x35 }, // 0xF6: ö = o5
            new byte[]{ 0xF7 },       // 0xF7
            new byte[]{ 0x6F, 0x36 }, // 0xF8: ø = o6
            new byte[]{ 0x75, 0x31 }, // 0xF9: ù = u1
            new byte[]{ 0x75, 0x32 }, // 0xFA: ú = u2
            new byte[]{ 0x75, 0x33 }, // 0xFB: û = u3
            new byte[]{ 0x75, 0x34 }, // 0xFC: ü = u4
            new byte[]{ 0x79, 0x31 }, // 0xFD: ý = y1
            new byte[]{ 0x74, 0x31 }, // 0xFE: þ = t1
            new byte[]{ 0x79, 0x32 }, // 0xFF: ÿ = y2
        };
    }
}
