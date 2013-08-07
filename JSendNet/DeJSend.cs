using System;
using System.Collections.Generic;

namespace IStepaniuk.JSendNet
{
	public static class DeJSend
	{
		public static String GetData (String iData)
		{
			if (String.IsNullOrEmpty(iData)) {
				return "";			
			}
			String[] elements = iData.Split (new String[] { "==" }, StringSplitOptions.None);
			String s1 = elements[0];
			String s2 = elements.Length > 1 ? elements[1] : null;
			String sDataTmp1 = DecompressLZW(DecodeBinary(Decode847(s1)));
			String sDataTmp2 = null;
			if (!String.IsNullOrEmpty(s2))
				sDataTmp2 = DecompressLZW (DecodeBinary (Decode847 (s2)));
			String sData = "";
			Dictionary<Byte, Char> aLookup = new Dictionary<Byte, Char> ();
			
			aLookup.Add (128, (Char) 8364);
			aLookup.Add (130, (Char) 8218);
			aLookup.Add (131, (Char) 402);
			aLookup.Add (132, (Char) 8222);
			aLookup.Add (133, (Char) 8230);
			aLookup.Add (134, (Char) 8224);
			aLookup.Add (135, (Char) 8225);
			aLookup.Add (136, (Char) 710);
			aLookup.Add (137, (Char) 8240);
			aLookup.Add (138, (Char) 352);
			aLookup.Add (139, (Char) 8249);
			aLookup.Add (140, (Char) 338);
			aLookup.Add (142, (Char) 381);
			aLookup.Add (145, (Char) 8216);
			aLookup.Add (146, (Char) 8217);
			aLookup.Add (147, (Char) 8220);
			aLookup.Add (148, (Char) 8221);
			aLookup.Add (149, (Char) 8226);
			aLookup.Add (150, (Char) 8211);
			aLookup.Add (151, (Char) 8212);
			aLookup.Add (152, (Char) 732);
			aLookup.Add (153, (Char) 8482);
			aLookup.Add (154, (Char) 353);
			aLookup.Add (155, (Char) 8250);
			aLookup.Add (156, (Char) 339);
			aLookup.Add (158, (Char) 382);
			aLookup.Add (159, (Char) 376);
			
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
			
			foreach (KeyValuePair<Byte, Char> pair in aLookup) {
				String strFrom = "" ;
				strFrom += (Char)pair.Key;
				
				String strTo =  ""; 
				strTo += pair.Value;
				
				sData = sData.Replace (strFrom, strTo);
			}
			return sData.Substring (1);
			
		}

		private static List<Char> Decode847 (String sChars)
		{
			Int32 iByte = 7;
			Int32 iMask = 0;
			List<Char> aCharCodes = new List<Char> ();
			
			for (Int32 i = 0; i < sChars.Length; i++) {
				Int32 iValue = (Int32)(sChars[i]);
				
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

		private static List<Char> DecodeBinary (List<Char> aCharCodes)
		{
			List<Char> aCodes = new List<Char> ();
			Int32 iDictCount = 256;
			Int32 iBits = 8;
			Int32 iRest = 0;
			Int32 iRestLength = 0;
			
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

		private static String DecompressLZW (List<Char> aCodes)
		{
			String sData = "";
			List<String> oDictionary = new List<String> ();
			
			for (Int32 i = 0; i < 256; i++) {
				Char ch = (Char) i;
				oDictionary.Add (ch.ToString ());
			}
			String sWord = "";
			
			for (Int32 sKey = 0; sKey < aCodes.Count; sKey++) {
				String sElement = "";
				Int32 iCode = aCodes[sKey];
				
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
					String newEntry = "";
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
