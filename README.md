jSEND
====
jSEND is a javascript compression library created and maintained by 
Michael Kortstiege - a web developer from near Hanover, Germany.

This .NET C# class provides text-to-binary decoding & unpacking for 
jSEND packed data. It is the reimplementation of it's PHP version and 
keeps a similar structure.

The original package includes a PHP class for doing the server side
decompression. More information on http://jsend.org/

DeJSEND usage example:

    String data = context.Request.Form["data"];
    String unpackedData = DejSEND.GetData(data); 

License: Dual licensed under the MIT or GPL Version 2 licenses.
