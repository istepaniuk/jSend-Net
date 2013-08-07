using System;
using NUnit.Framework;
using IStepaniuk.JSendNet;

namespace IStepaniuk.JSendNetTests
{
    [TestFixture()]
    public class DeJSendTests
    {
        private DeJSend deJSend;

        [TestFixtureSetUp]
        public void SetUp(){
            deJSend = new DeJSend();
        }

        [Test]
        public void HelloWorldIntegrationTest ()
        {
            const string compressedHelloWorld = "0J$MCxq0@w7\r=A\b";

            var result = deJSend.GetData(compressedHelloWorld);

            Assert.IsTrue(result == "Hello, world!");
        }

        [Test]
        public void ChineseChinaIntegrationTest ()
        {
            const string compressedChinaInChinese = "\bJ}@==`@=";

            var result = deJSend.GetData(compressedChinaInChinese);

            Assert.IsTrue(result == "臺灣");
        }

        [Test]
        public void LongStringIntegrationTests ()
        {
            const string compressedLongString = "TJ \b\rcJrN\a\f#I8N :0\f=7AjrG\bAbXQN!\nFDCF)\f^m8#\a3I<=l 0M\ffxdDZ 4GsC=l 6=69J\fgCad";

            var result = deJSend.GetData(compressedLongString);

            Assert.IsTrue(result == "A larger string that further tests that the decompression algorithm is working correctly");
        }

    }
}

