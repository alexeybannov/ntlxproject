#region usings

using System;
using System.Collections.Generic;
using System.Text;

#endregion

namespace ASC.Common.Utils
{
    public static class Transliter
    {
        private static readonly IDictionary<string, IDictionary<char, string>> dics;

        static Transliter()
        {
            dics = new Dictionary<string, IDictionary<char, string>>();
            var ruEnDic = new Dictionary<char, string>(33);
            ruEnDic['а'] = "A";
            ruEnDic['б'] = "B";
            ruEnDic['в'] = "V";
            ruEnDic['г'] = "G";
            ruEnDic['д'] = "D";
            ruEnDic['е'] = "E";
            ruEnDic['Ё'] = "Jo";
            ruEnDic['ж'] = "Zh";
            ruEnDic['з'] = "Z";
            ruEnDic['и'] = "I";
            ruEnDic['й'] = "J";
            ruEnDic['к'] = "K";
            ruEnDic['л'] = "L";
            ruEnDic['м'] = "M";
            ruEnDic['н'] = "N";
            ruEnDic['о'] = "O";
            ruEnDic['п'] = "P";
            ruEnDic['р'] = "R";
            ruEnDic['с'] = "S";
            ruEnDic['т'] = "T";
            ruEnDic['у'] = "U";
            ruEnDic['ф'] = "F";
            ruEnDic['х'] = "H";
            ruEnDic['ц'] = "C";
            ruEnDic['ч'] = "Ch";
            ruEnDic['ш'] = "Sh";
            ruEnDic['щ'] = "W";
            ruEnDic['ь'] = "";
            ruEnDic['ы'] = "Y";
            ruEnDic['ъ'] = "";
            ruEnDic['э'] = "E";
            ruEnDic['ю'] = "Ju";
            ruEnDic['я'] = "Ja";
            dics["ruen"] = ruEnDic;
        }

        public static string Translit(string sourceLang, string destLang, string str)
        {
            string dicCode = (sourceLang + destLang).ToLowerInvariant();
            if (!dics.ContainsKey(dicCode))
            {
                throw new NotSupportedException(string.Format("No dictionary translit from {0} to {1}.", sourceLang,
                                                              destLang));
            }
            IDictionary<char, string> dic = dics[dicCode];
            var destStr = new StringBuilder();
            for (int i = 0; i < str.Length; i++)
            {
                char symbol = str[i];
                char upperSymbol = char.ToUpper(symbol);
                if (dic.ContainsKey(upperSymbol))
                {
                    string destSymbol = dic[upperSymbol];
                    if (char.IsUpper(symbol))
                    {
                        if (i < str.Length - 1 && char.IsUpper(str[i + 1]))
                        {
                            destStr.Append(destSymbol.ToUpper());
                        }
                        else
                        {
                            destStr.Append(destSymbol);
                        }
                    }
                    else
                    {
                        destStr.Append(destSymbol.ToLower());
                    }
                }
                else
                {
                    destStr.Append(symbol);
                }
            }
            return destStr.ToString();
        }
    }
}