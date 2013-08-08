using System;
using System.Collections.Generic;

namespace IStepaniuk.JSendNet
{
    public class DeJSend
    {
        private const string PartSeparator = "==";

        public string GetData (string input)
        {
            if (String.IsNullOrEmpty (input)) {
                return String.Empty;			
            }

            var elements = input.Split (new string[] { PartSeparator }, StringSplitOptions.None);
            var hasSecondPart = elements.Length > 1;
            var sDataTmp1 = DecompressLZW (DecodeBinary(Decode847( elements [0])));

            string result = String.Empty;
            if (hasSecondPart) {
                var sDataTmp2 = DecompressLZW (DecodeBinary (Decode847 (elements[1])));
                for (var i = 0; i < sDataTmp1.Length; i++) {
                    var sTmp1 = sDataTmp1 [i];
                    var sTmp2 = sDataTmp2 [i];
                    if (sTmp2 != 224)
                        result += (Char)((Int32)sTmp1 + 256 * sTmp2);
                    else
                        result += sTmp1;
                }
            } else {
                result = sDataTmp1;
            }
			
            return LookUpTableReplace (result).Substring(1);
        }

        private string LookUpTableReplace (string input)
        {
            var table = new Dictionary<int, int> {
                { 128, 8364 }, { 130, 8218 }, { 131, 402  }, { 132, 8222 }, 
                { 133, 8230 }, { 134, 8224 }, { 135, 8225 }, { 136, 710  },
                { 137, 8240 }, { 138, 352  }, { 139, 8249 }, { 140, 338  },
                { 142, 381  }, { 145, 8216 }, { 146, 8217 }, { 147, 8220 },
                { 148, 8221 }, { 149, 8226 }, { 150, 8211 }, { 151, 8212 },
                { 152, 732  }, { 153, 8482 }, { 154, 353  }, { 155, 8250 },
                { 156, 339  }, { 158, 382  }, { 159, 376  } 
            };
            foreach (var pair in table) {
                input = input.Replace ((char)pair.Key, (char)pair.Value);
            }
            return input;
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

        private string DecompressLZW (List<Char> inpsut)
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
