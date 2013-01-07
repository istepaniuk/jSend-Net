/* -----------------------------------------------------------------------------
 * jSEND      v2.0.0
 * -----------------------------------------------------------------------------
 * Date:      Fri Sep 24 19:35:11 2010 +0100 
 *  
 * Summary:   This plugin provides compression & binary-to-text encoding
 *            for use in XMLHTTPRequest/AJAX post requests
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
 * X/HTML     <script type="text/javascript" src="http://ajax.googleapis.com/ajax/libs/jquery/1.4/jquery.min.js"><script>
 *            <script type="text/javascript" src="jsend.min.js"></script>
 *   
 * JS         var str = "String to Squeeze, Encode & Deliver"; 
 *            var data = $.jSEND(str);
 *            // Send data to server
 * -----------------------------------------------------------------------------    
 */
 
 function encodeToHex(str){
    var r="";
    var e=str.length;
    var c=0;
    var h;
    while(c<e){
        h = str.charCodeAt(c++).toString();
//        while(h.length<3) h="0"+h;
        r+=h+' ';
    }
    return r;
}
 
 
 
(function($)
{
  $.jSEND = function(sData)
  {
    /* ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
       MAIN SQUEEZE
    ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~ */
    sData = String.fromCharCode(74)+sData;
    console.log("sData + 74:",sData)
    /* --------
       Init
    -------- */ 
    var iCount = 256;
    var sFill = '';
    var iFillCount = 0;
    var iEmptyCount = 0;
    for (var i = 0; i < iCount; i++)
      sFill += String.fromCharCode(224);
     
  
    var iDictSize = iCount;
    var oDictionary = {};
    for (var i = 0; i < iCount; i++)
      oDictionary[String.fromCharCode(i)] = i;
      
//    console.log("oDictionary:",oDictionary)
  
    var aCodes = [];
    var sData2 = ''; //String.fromCharCode(224);
    var sPattern = '';
    /* --------
       Go
    -------- */ 
    for (var i = 0, iLn = sData.length; i < iLn; i++) 
    {
        var sChar = sData.charAt(i);
        var iCode = sChar.charCodeAt(0);
        /* --------------------
         Handle UCS Chars
        -------------------- */ 
        if (iCode > 255) {
            /* ------------------------------------------------------------
              Replace some UCS Chars with their ANSI (128-159) pendants
            ------------------------------------------------------------ */ 
            var iChk = iCode;
            switch (iCode) {
                case 8364: iCode = 128; break; case 8218: iCode = 130; break; case 402:  iCode = 131; break;
                case 8222: iCode = 132; break; case 8230: iCode = 133; break; case 8224: iCode = 134; break;
                case 8225: iCode = 135; break; case 710:  iCode = 136; break; case 8240: iCode = 137; break;
                case 352:  iCode = 138; break; case 8249: iCode = 139; break; case 338:  iCode = 140; break;
                case 381:  iCode = 142; break; case 8216: iCode = 145; break; case 8217: iCode = 146; break;
                case 8220: iCode = 147; break; case 8221: iCode = 148; break; case 8226: iCode = 149; break;
                case 8211: iCode = 150; break; case 8212: iCode = 151; break; case 732:  iCode = 152; break;
                case 8482: iCode = 153; break; case 353:  iCode = 154; break; case 8250: iCode = 155; break;
                case 339:  iCode = 156; break; case 382:  iCode = 158; break; case 376:  iCode = 159; break;
            }
            
            if (iChk != iCode) {
                //console.log("iCode changed:", iChk, iCode);
                sChar = String.fromCharCode(iCode);
                iEmptyCount++;
                iFillCount++;
                if (iFillCount >= iCount) {
                  sData2 += sFill;
                  iFillCount = 0;
                }
            }
            else {
               if (iFillCount > 0) { 
                 sData2 += sFill.substr(0,iFillCount);
                 iFillCount = 0;
               }
               sData2 += String.fromCharCode(parseInt(iCode / 256));
            }//else
        } //if
        
      
        
        /* ---------------------------
           Handle ASCII/ANSI Chars
        -------------------------- */ 
        else {
            iEmptyCount++;
            iFillCount++;
            if (iFillCount >= iCount) {
              sData2 += sFill;
              iFillCount = 0;
            }
        } 
        
        //console.log("sChar pre LZW: ", sChar, sChar.charCodeAt(0) );
                
        
        /* -------------------------
          Start LZW Compression
        ------------------------- */    
        var sCombined = sPattern + sChar;  
        if (oDictionary[sCombined])
              sPattern = sCombined;  
        else 
        {   
            if (iCode > 255)
             	  sChar = String.fromCharCode(iCode % 256); 
            aCodes.push(oDictionary[sPattern]);
            oDictionary[sCombined] = iDictSize++;
            sPattern = '' + sChar;
        }  
        /* -----------------------
          End LZW Compression
       ----------------------- */    
       
      
    }// for ( i..
    
    /* ----------
       Flush
    ---------- */ 
    if (sPattern != '')
        aCodes.push(oDictionary[sPattern]);
    if (iFillCount > 0)
      sData2 += sFill.substr(0,iFillCount);
      
    /* ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
       SUB-SQUEEZE & DOUBLE ENCODING
    ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~ */
   // console.log("aCodes after flush:", aCodes) ;
   // console.log("sData2:", sData.length != iEmptyCount, sData2 ) ;
   // console.log("aCodes bin:", encodeBinary(aCodes)) ;

  //  console.log("oDictionary end:",    oDictionary) ;

        
    var sChars = ecode847(encodeBinary(aCodes));
    if (sData.length != iEmptyCount)
    {
      var sChars2 = ecode847(encodeBinary(compressLZW(sData2)));
  //    console.log("final:", sChars) ;
      return sChars + '==' + sChars2;
    }
    else {
      //  console.log("final:", encodeToHex(sChars)) ;
        return sChars; 
    }
    
}
      
})(jQuery);






    /* ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~

       FUNCTIONS
    ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~ */
    /* ------------------
       LZW Compressor
    ------------------ */ 
    function compressLZW(sData)
    {
      var aCodes = [];  
      var iDictSize = 256; 
      var oDictionary = {};
      for (var i = 0; i < 256; i++)
        oDictionary[String.fromCharCode(i)] = i; 
      var sPattern = '';
      for (var i = 0, iLn = sData.length; i < iLn; i++) 
      {
      	var sChar = sData.charAt(i); 
        var sCombined = sPattern + sChar;  
        if (oDictionary[sCombined])
            sPattern = sCombined;  
        else 
        {   
            aCodes.push(oDictionary[sPattern]);
            oDictionary[sCombined] = iDictSize++;
            sPattern = '' + sChar;
        }  
      }
      if (sPattern != '')
          aCodes.push(oDictionary[sPattern]);
      return aCodes;
    }
    /* ------------------
       Binary Encoder
    ------------------ */ 
    function encodeBinary(aCodes) 
    {
      var iDictCount = 256;
      var aCharCodes = [];
      var iBits = 8;
      var iRest = 0;
      var iRestLength = 0;
      for(var i=0, iLn = aCodes.length; i < iLn; i++) 
      {
        iRest = (iRest << iBits) + aCodes[i];
        iRestLength += iBits;
        iDictCount++;
        if (iDictCount >> iBits) 
            iBits++;
        while (iRestLength > 7) 
        {
          iRestLength -= 8;
          aCharCodes.push(iRest >> iRestLength);
          iRest &= (1 << iRestLength) - 1;
        }
      }
      aCharCodes.push(iRestLength ? iRest << (8 - iRestLength) : '');
      return aCharCodes;
    }
     /* ------------------
       847enc Encoder
    ------------------ */ 
    function ecode847(aCharCodes) 
    {
      var aTmp = [];
      var iCount = 0;
      var iChar = 0;
      var sChars = '';
      for(var i=0, iLn = aCharCodes.length; i < iLn; i++) 
      {
        var iValue = aCharCodes[i];
        if (iValue > 127) {
          iValue -= 128;
          iChar += Math.pow(2,iCount);
        }
        if (iValue == 0  || iValue == 34 || iValue == 37 || iValue == 38 || 
            iValue == 39 || iValue == 43 || iValue == 61 || iValue == 92)
          aTmp.push('='+String.fromCharCode((iValue+16)));
        else
          aTmp.push(String.fromCharCode(iValue));
        iCount++;
        if (iCount > 6) 
        {
          if (iChar == 0  || iChar == 34 || iChar == 37 || iChar == 38 || 
              iChar == 39 || iChar == 43 || iChar == 61 || iChar == 92)
            sChars += ('=' + String.fromCharCode((iChar+16)) + aTmp.join(''));
          else
            sChars += (String.fromCharCode(iChar) + aTmp.join(''));
          aTmp = [];
          iChar = 0;
          iCount = 0;
        }
      }
      if (iChar == 0  || iChar == 34 || iChar == 37 || iChar == 38 || 
          iChar == 39 || iChar == 43 || iChar == 61 || iChar == 92)
        sChars += ('=' + String.fromCharCode((iChar+16)) + aTmp.join(''));
      else
        sChars += (String.fromCharCode(iChar) + aTmp.join(''));
      return sChars;
    }




