using System;
using System.Collections.Generic;

namespace IStepaniuk.JSendNet
{
    public class DeJSend
    {
        public string GetData (string input)
        {
            if (String.IsNullOrEmpty (input)) {
                return "";			
            }
            var elements = input.Split (new string[] { "==" }, StringSplitOptions.None);
            var s1 = elements [0];
            var s2 = elements.Length > 1 ? elements [1] : null;
            var sDataTmp1 = DecompressLZW (DecodeBinary(Decode847(s1)));
            string sDataTmp2 = null;
            if (!String.IsNullOrEmpty (s2))
                sDataTmp2 = DecompressLZW (DecodeBinary (Decode847 (s2)));
            string result = "";
			
			
            if (!String.IsNullOrEmpty (sDataTmp2)) {
                for (Int32 i = 0; i < sDataTmp1.Length; i++) {
                    var sTmp1 = sDataTmp1 [i];
                    var sTmp2 = (char)(Int32)sDataTmp2 [i];
                    if (sTmp2 != 224)
                        result += (Char)((Int32)sTmp1 + 256 * sTmp2);
                    else
                        result += sTmp1;
                }
            } else {
                result = sDataTmp1;
            }
			
            foreach (KeyValuePair<Byte, Char> pair in LookUpTable()) {
                result = result.Replace ((char)pair.Key, pair.Value);
            }

            return result.Substring (1);
        }

        private Dictionary<Byte, Char> LookUpTable ()
        {
            return new Dictionary<Byte, Char> {
                { 128, (Char) 8364},
                { 130, (Char) 8218},
                { 131, (Char) 402},
                { 132, (Char) 8222},
                { 133, (Char) 8230},
                { 134, (Char) 8224},
                { 135, (Char) 8225},
                { 136, (Char) 710},
                { 137, (Char) 8240},
                { 138, (Char) 352},
                { 139, (Char) 8249},
                { 140, (Char) 338},
                { 142, (Char) 381},
                { 145, (Char) 8216},
                { 146, (Char) 8217},
                { 147, (Char) 8220},
                { 148, (Char) 8221},
                { 149, (Char) 8226},
                { 150, (Char) 8211},
                { 151, (Char) 8212},
                { 152, (Char) 732},
                { 153, (Char) 8482},
                { 154, (Char) 353},
                { 155, (Char) 8250},
                { 156, (Char) 339},
                { 158, (Char) 382},
                { 159, (Char) 376}
            };
        }

        private List<Char> Decode847 (string input)
        {
            var iByte = 7;
            var mask = 0;
            var charCodes = new List<Char> ();
			
            for (var i = 0; i < input.Length; i++) {
                var value = (Int32)(input [i]);
				
                if (value == 61) {
                    i++;
                    value = (Int32)(input [i]) - 16;
                }
				
                if (iByte > 6) {
                    mask = value;
                    iByte = 0;
					
                } else {
                    var nextPowerOfTwo = (Int32)Math.Ceiling (Math.Pow (2, iByte));
                    if ((mask & nextPowerOfTwo) == nextPowerOfTwo)
                        value += 128;
                    charCodes.Add ((Char)value);
                    iByte++;
                }
            }
            return charCodes;
			
        }

        private List<Char> DecodeBinary (List<Char> input)
        {
            var codes = new List<Char> ();
            var dictCount = 256;
            var bits = 8;
            var rest = 0;
            var restLength = 0;
			
            foreach (var i in input) {
                rest = (rest << 8) + (Int32)i;
                restLength += 8;
				
                if (restLength >= bits) {
                    restLength -= bits;
                    codes.Add ((Char)(rest >> restLength));
                    rest &= (1 << restLength) - 1;
                    dictCount++;
                    if ((dictCount >> bits) > 0)
                        bits++;
                }
            }
            return codes;
			
        }

        private string DecompressLZW (List<Char> input)
        {
            var data = "";
            var dictionary = new List<String> ();
			
            for (Int32 i = 0; i < 256; i++) {
                var ch = (Char)i;
                dictionary.Add (ch.ToString ());
            }
            var sWord = "";
			
            for (var sKey = 0; sKey < input.Count; sKey++) {
                var sElement = "";
                var iCode = input [sKey];
				
                if (!(iCode >= 0 && iCode < dictionary.Count)) {
					
                    if (sWord.Length > 0) {
                        sElement = sWord + sWord [0];
						
                    } else {
                        sElement = "";
                    }
					
                } else {
                    sElement = dictionary [iCode];
                }
                data += sElement;
                if (sKey > 0) {
                    var newEntry = "";
                    if (sElement.Length > 0) {
                        newEntry += sWord + sElement [0];
                    }
                    dictionary.Add (newEntry);
                }
                sWord = sElement;
            }
            return data;
        }
    }
}
