jSEND
====
jSEND is a javascript compression library created and maintained by 
Michael Kortstiege - a web developer from near Hanover, Germany.

This .NET C# class provides text-to-binary decoding & unpacking for 
jSEND packed data. It is the reimplementation of it's PHP version and 
keeps a similar structure.

The original package includes a PHP class for doing the server side
decompression. The original site was jsend.org but it seems to be a parked domain now.
I wrote a post on this on my blog some time ago: 
http://blog.istepaniuk.com/client-side-compression-with-javascript/

DeJSEND usage example:

    String data = context.Request.Form["data"];
    String unpackedData = new DejSEND().GetData(data); 

License: Dual licensed under the MIT or GPL Version 2 licenses.
