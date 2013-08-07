using System;
using System.Collections.Generic;

namespace IStepaniuk.JSendNet
{
	public class DeJSend
	{
        public string GetData (string iData)
		{
			if (String.IsNullOrEmpty(iData)) {
				return "";			
			}
            var elements = iData.Split (new string[] { "==" }, StringSplitOptions.None);
            var s1 = elements[0];
            var s2 = elements.Length > 1 ? elements[1] : null;
            var sDataTmp1 = DecompressLZW(DecodeBinary(Decode847(s1)));
            string sDataTmp2 = null;
			if (!String.IsNullOrEmpty(s2))
				sDataTmp2 = DecompressLZW (DecodeBinary (Decode847 (s2)));
			String sData = "";
			
			
			if (!String.IsNullOrEmpty (sDataTmp2)) {
				for (Int32 i = 0; i < sDataTmp1.Length; i++) {
					Char sTmp1 = sDataTmp1[i];
					Int32 sTmp2 = (Int32)sDataTmp2[i];
					if (sTmp2 != 224)
						sData += (Char) ((Int32)sTmp1 + 256 * sTmp2); 
					else if ((Int32)sTmp1 > 127)
						sData += sTmp1.ToString();
					else
						sData += sTmp1;
				}
			} else {
				sData = sDataTmp1;
			}
			
			foreach (KeyValuePair<Byte, Char> pair in LookUpTable()) {
				String strFrom = "" ;
				strFrom += (Char)pair.Key;
				
				String strTo =  ""; 
				strTo += pair.Value;
				
				sData = sData.Replace (strFrom, strTo);
			}
			return sData.Substring (1);
			
		}

        private Dictionary<Byte, Char> LookUpTable(){
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

        private List<Char> Decode847 (string sChars)
		{
            var iByte = 7;
            var iMask = 0;
            var aCharCodes = new List<Char> ();
			
            for (var i = 0; i < sChars.Length; i++) {
                var iValue = (Int32)(sChars[i]);
				
				if (iValue == 61) {
					i++;
					iValue = (Int32)(sChars[i]) - 16;
				}
				
				if (iByte > 6) {
					iMask = iValue;
					iByte = 0;
					
				} else {
					Int32 pt = (Int32)Math.Ceiling (Math.Pow (2, iByte));
					if ((iMask & pt) == pt)
						iValue += 128;
					aCharCodes.Add ((Char)iValue);
					iByte++;
				}
			}
			return aCharCodes;
			
		}

		private List<Char> DecodeBinary (List<Char> aCharCodes)
		{
            var aCodes = new List<Char> ();
            var iDictCount = 256;
            var iBits = 8;
            var iRest = 0;
            var iRestLength = 0;
			
			for (Int32 i = 0; i < aCharCodes.Count; i++) {
				iRest = (iRest << 8) + (Int32)aCharCodes[i];
				iRestLength += 8;
				
				if (iRestLength >= iBits) {
					iRestLength -= iBits;
					aCodes.Add ((Char)(iRest >> iRestLength));
					iRest &= (1 << iRestLength) - 1;
					iDictCount++;
					if ((iDictCount >> iBits) > 0)
						iBits++;
				}
			}
			return aCodes;
			
		}

		private string DecompressLZW (List<Char> aCodes)
		{
            var sData = "";
            var oDictionary = new List<String> ();
			
			for (Int32 i = 0; i < 256; i++) {
                var ch = (Char) i;
				oDictionary.Add (ch.ToString ());
			}
            var sWord = "";
			
            for (var sKey = 0; sKey < aCodes.Count; sKey++) {
                var sElement = "";
                var iCode = aCodes[sKey];
				
				if (!(iCode >= 0 && iCode < oDictionary.Count)) {
					
					if (sWord.Length > 0) {
						sElement = sWord + sWord[0];
						
					} else {
						sElement = "";
					}
					
				} else {
					sElement = oDictionary[iCode];
				}
				sData += sElement;
				if (sKey > 0) {
                    var newEntry = "";
					if (sElement.Length > 0) {
						newEntry += sWord + sElement[0];
					}
					oDictionary.Add (newEntry);
				}
				sWord = sElement;
			}
			return sData;
		}
	}
}
