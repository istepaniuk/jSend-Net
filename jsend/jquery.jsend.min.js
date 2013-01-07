/*!
 * jSEND      v2.0.0
 * 
 * Date:      Fri Sep 24 19:35:11 2010 +0100 
 
 * Author:    Michael Kortstiege, Copyright 2010
 * Website:   http://jsend.org/ 
 *  
 * License:   Dual licensed under the MIT or GPL Version 2 licenses.
 *            (http://jsend.org/license)
 *
 * Credits:   See http://jsend.org/about/
 *   
 */
(function(x){x.jSEND=function(l){function y(j){for(var f=[],i=256,b={},c=0;c<256;c++)b[String.fromCharCode(c)]=c;var d="";c=0;for(var m=j.length;c<m;c++){var e=j.charAt(c),s=d+e;if(b[s])d=s;else{f.push(b[d]);b[s]=i++;d=""+e}}d!=""&&f.push(b[d]);return f}function v(j){for(var f=256,i=[],b=8,c=0,d=0,m=0,e=j.length;m<e;m++){c=(c<<b)+j[m];d+=b;f++;for(f>>b&&b++;d>7;){d-=8;i.push(c>>d);c&=(1<<d)-1}}i.push(d?c<<8-d:"");return i}function w(j){for(var f=[],i=0,b=0,c="",d=0,m=j.length;d<m;d++){var e=j[d];
if(e>127){e-=128;b+=Math.pow(2,i)}e==0||e==34||e==37||e==38||e==39||e==43||e==61||e==92?f.push("="+String.fromCharCode(e+16)):f.push(String.fromCharCode(e));i++;if(i>6){c+=b==0||b==34||b==37||b==38||b==39||b==43||b==61||b==92?"="+String.fromCharCode(b+16)+f.join(""):String.fromCharCode(b)+f.join("");f=[];i=b=0}}c+=b==0||b==34||b==37||b==38||b==39||b==43||b==61||b==92?"="+String.fromCharCode(b+16)+f.join(""):String.fromCharCode(b)+f.join("");return c}l=String.fromCharCode(74)+l;for(var k="",g=0,t=
0,h=0;h<256;h++)k+=String.fromCharCode(224);var z=256,p={};for(h=0;h<256;h++)p[String.fromCharCode(h)]=h;var u=[],n="",o="";h=0;for(var A=l.length;h<A;h++){var q=l.charAt(h),a=q.charCodeAt(0);if(a>255){var r=a;switch(a){case 8364:a=128;break;case 8218:a=130;break;case 402:a=131;break;case 8222:a=132;break;case 8230:a=133;break;case 8224:a=134;break;case 8225:a=135;break;case 710:a=136;break;case 8240:a=137;break;case 352:a=138;break;case 8249:a=139;break;case 338:a=140;break;case 381:a=142;break;
case 8216:a=145;break;case 8217:a=146;break;case 8220:a=147;break;case 8221:a=148;break;case 8226:a=149;break;case 8211:a=150;break;case 8212:a=151;break;case 732:a=152;break;case 8482:a=153;break;case 353:a=154;break;case 8250:a=155;break;case 339:a=156;break;case 382:a=158;break;case 376:a=159;break}if(r!=a){q=String.fromCharCode(a);t++;g++;if(g>=256){n+=k;g=0}}else{if(g>0){n+=k.substr(0,g);g=0}n+=String.fromCharCode(parseInt(a/256))}}else{t++;g++;if(g>=256){n+=k;g=0}}r=o+q;if(p[r])o=r;else{if(a>
255)q=String.fromCharCode(a%256);u.push(p[o]);p[r]=z++;o=""+q}}o!=""&&u.push(p[o]);if(g>0)n+=k.substr(0,g);k=w(v(u));if(l.length!=t){l=w(v(y(n)));return k+"=="+l}else return k}})(jQuery);
