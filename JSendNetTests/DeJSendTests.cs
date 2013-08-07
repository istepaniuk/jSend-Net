using System;
using NUnit.Framework;
using IStepaniuk.JSendNet;

namespace IStepaniuk.JSendNetTests
{
    [TestFixture()]
    public class DeJSendTests
    {
        [Test()]
        public void HelloWorldIntegrationTest ()
        {
            const string compressedHelloWorld = "0J$MCxq0@w7\r=A\b";

            var result = DeJSend.GetData(compressedHelloWorld);

            Assert.IsTrue(result == "Hello, world!");
        }

        [Test()]
        public void ChineseChinaIntegrationTest ()
        {
            const string compressedChinaInChinese = "\bJ}@==`@=";

            var result = DeJSend.GetData(compressedChinaInChinese);

            Assert.IsTrue(result == "臺灣");
        }
    }
}

