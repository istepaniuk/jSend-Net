jSend
====
jSend is a javascript compression library created by 
Michael Kortstiege - a web developer from near Hanover, Germany.

This repo contains the original front-end JavaScript library that compresses strings. And 
also the original PHP class that decompresses the jSend packed data in the server.

I also added a .NET C# class that provides text-to-binary decoding & unpacking for 
jSend packed data. It is an implementation of the PHP version and keeps a similar structure. 

I covered the C# class with some tests and did some refactoring. The code could still be much cleaner though.

The original site was jsend.org but it seems to be long gone. I wrote a post on this on my blog some time ago: 
http://blog.istepaniuk.com/client-side-compression-with-javascript/

DejSEND usage example (server side):

    String data = context.Request.Form["data"];
    String unpackedData = new DejSEND().GetData(data); 

License: Dual licensed under the MIT or GPL Version 2 licenses.
