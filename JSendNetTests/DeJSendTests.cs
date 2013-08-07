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
    }
}

