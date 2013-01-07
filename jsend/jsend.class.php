<?php
/* -----------------------------------------------------------------------------
 * jSEND      PHP class v1.0.0
 * -----------------------------------------------------------------------------
 * Date:      Fri Sep 24 19:35:11 2010 +0100 
 *
 * Summary:   This class provides text-to-binary decoding & unpacking
 *            for jSEND
 *   
 * Author:    Michael Kortstiege, Copyright 2010
 * Website:   http://jsend.org/ 
 *  
 * License:   Dual licensed under the MIT or GPL Version 2 licenses.
 *            (http://jsend.org/license/)
 *
 * Credits:   See http://jsend.org/about/
 *   
 * -----------------------------------------------------------------------------
 * USAGE
 * -----------------------------------------------------------------------------
 *            include('jsend.class.php');
          $data = $_POST["data"];
 *            // Checks, Validation etc. 
 *            $jSEND = new jSEND();
 *            $str = $jSEND->getData($data); 
 * -----------------------------------------------------------------------------    
 */

function strToHex($string)
{
    $hex='';
    for ($i=0; $i < strlen($string); $i++)
    {
        $hex .= " ".(ord($string[$i]));
    }
    return $hex;
}

class jSEND
{
  public function getData($s)
  {
    /* ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
       DOUBLE DECODE & DECOMPRESS STRING(S)
    ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~ */
    list($s1,$s2) = explode('==',$s); 
    $sDataTmp1 = self::decompressLZW(self::decodeBinary(self::decode847($s1)));
    if ($s2) 
      $sDataTmp2 = self::decompressLZW(self::decodeBinary(self::decode847($s2)));

    /* ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
       REGENERATE DATA
    ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~ */
    $sDebug = '';
    $sData = '';
    $aLookup = array(
        128 => 8364, 130 => 8218, 131 => 402,  132 => 8222, 133 => 8230, 134 => 8224,
        135 => 8225, 136 => 710,  137 => 8240, 138 => 352,  139 => 8249, 140 => 338,
        142 => 381,  145 => 8216, 146 => 8217, 147 => 8220, 148 => 8221, 149 => 8226,
        150 => 8211, 151 => 8212, 152 => 732,  153 => 8482, 154 => 353,  155 => 8250,
        156 => 339,  158 => 382,  159 => 376
    );
    /* -------------------------------------------------------
       Merge strings (only if UTF-8 chars were used) 
    ------------------------------------------------------- */
    if ($sDataTmp2) 
    {
      // Cache file (in RAM) for better performance!
      //$aUtf8 = file('jsend.class.utf8chars.php', FILE_IGNORE_NEW_LINES);
      for($i = 0; $i < strlen($sDataTmp1); $i++) 
      {
        $sTmp1 = substr($sDataTmp1,$i,1);
        $sTmp2 = ord(substr($sDataTmp2,$i,1));
        if ($sTmp2 != 224)
          //$sData .= $aUtf8[(ord($sTmp1) + 256 * $sTmp2) - 256 + 1];
          //$sData .= mb_convert_encoding('&#' . (ord($sTmp1) + 256 * $sTmp2) . ';', 'UTF-8', 'HTML-ENTITIES');
          $sData .= self::unichr((ord($sTmp1) + 256 * $sTmp2));
        else
          if (ord($sTmp1) > 127)
            $sData .= utf8_encode($sTmp1);
          else
            $sData .= $sTmp1;
      }
    }
    else
      $sData = utf8_encode($sDataTmp1);
    /* -----------------------------
       ANSI Chars 128-159 to UCS
    ----------------------------- */  
//    $sDebug = "%DEBUG_JS%: ".strToHex($sDataTmp1) . " enc8> " .strToHex($sData) ;
    foreach ($aLookup as $sKey => $iValue)
      $sData = str_replace(chr(194).chr($sKey),self::unichr($iValue),$sData);
      //$sData = str_replace(chr(194).chr($sKey),html_entity_decode('&#'.$iValue.';',ENT_NOQUOTES,'UTF-8'),$sData);
    return substr($sData,1);//.$sDebug;
  }
  
  /* ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
     PRIVATE FUNCTIONS
  ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~ */
  /* -------------------
   * 847enc Decoder
  ------------------- */ 
  public static function decode847($sChars) 
  {
    $iByte = 7;
    $iMask = 0;
    $aCharCodes = array();
    for($i = 0; $i < strlen($sChars); $i++) 
    {
      $iValue = ord(substr($sChars,$i,1));
      if ($iValue == 61) {
        $i++;
        $iValue = ord(substr($sChars,$i,1)) - 16;
      }
      
      if ($iByte > 6) 
      {
        $iMask = $iValue;
        $iByte = 0;
      }
      else 
      {
        $pt = pow(2,$iByte);
        if (($iMask & $pt) === $pt)
          $iValue += 128;  
  
        $aCharCodes[] = $iValue;
        $iByte++;
      }
    }
    return $aCharCodes;
  }
  /* ------------------
   * Binary Decoder
  ------------------ */ 
  public static function decodeBinary($aCharCodes) 
  {
    $aCodes = array();
    $iDictCount = 256;
    $iBits = 8;
    $iRest = 0;
    $iRestLength = 0;
    for($i = 0; $i < count($aCharCodes); $i++) 
    {
      $iRest = ($iRest << 8) + $aCharCodes[$i];
      $iRestLength += 8;
      if ($iRestLength >= $iBits) 
      {
        $iRestLength -= $iBits;
        $aCodes[] = $iRest >> $iRestLength;
        $iRest &= (1 << $iRestLength) - 1;
        $iDictCount++;
        if ($iDictCount >> $iBits)
          $iBits++;
      }
    }
    return $aCodes;
  }
  /* --------------------
   * LZW Decompressor
  -------------------- */ 
  public static function decompressLZW($aCodes) 
  {
    $sData = '';
    $oDictionary = range("\x0000", "\xff");
    foreach ($aCodes as $sKey => $iCode) 
    {
      $sElement = $oDictionary[$iCode]; 
      if (!isset($sElement))
          $sElement = $sWord . $sWord[0];
      $sData .= $sElement;
      if ($sKey)
          $oDictionary[] = $sWord . $sElement[0];
      $sWord = $sElement;
    }
    return $sData;
  }
  /* --------------------
   * Unicode chr
  -------------------- */ 
  public function unichr($iCode) {
    if ($iCode <= 0x7F) {
        return chr($iCode);
    } else if ($iCode <= 0x7FF) {
        return chr(0xC0 | $iCode >> 6)  . chr(0x80 | $iCode & 0x3F);
    } else if ($iCode <= 0xFFFF) {
        return chr(0xE0 | $iCode >> 12) . chr(0x80 | $iCode >> 6 & 0x3F)
                                        . chr(0x80 | $iCode & 0x3F);
    } else if ($iCode <= 0x10FFFF) {
        return chr(0xF0 | $iCode >> 18) . chr(0x80 | $iCode >> 12 & 0x3F)
                                        . chr(0x80 | $iCode >> 6 & 0x3F)
                                        . chr(0x80 | $iCode & 0x3F);
    } else {
        return false;
    }
  }
}
?>
