using HydroNotifier.FunctionApp.Core;
using HydroNotifier.FunctionApp.Notifications;
using NUnit.Framework;
using NUnit.Framework.Internal;
using System;
using System.Collections.Generic;

namespace HydroNotifier.Tests
{
    public class SmsMessageBuilderTest
    {
        private readonly List<HydroData> _data = new List<HydroData>()
        {
            new HydroData("Řeka 1", new DateTime(2019, 1, 1, 13, 50, 20).ToString(), 2000.0),
            new HydroData("Řeka 2", new DateTime(2019, 1, 1, 13, 50, 20).ToString(), 3000.1)
        };

        [Test]
        public void BuildMessage_HighFlow()
        {
            var builder = new SmsMessageBuilder();
            var actual = builder.BuildMessage(_data, HydroStatus.High, new DateTime(2019, 1, 1, 13, 50, 30), "420735");

            Assert.AreEqual("420735", actual.to);
            Assert.AreEqual("HydroNotifier", actual.from);
            Assert.AreEqual("Jablunkov MVE, Stav: Vysoky prutok, Datum: 01.01.2019 13:50:30, Prutok: 5000,1 l/s", actual.text);
        }

        [Test]
        public void BuildMessage_LowFlow()
        {
            var builder = new SmsMessageBuilder();
            var actual = builder.BuildMessage(_data, HydroStatus.Low, new DateTime(2019, 1, 1, 13, 50, 30), "420735");

            Assert.AreEqual("420735", actual.to);
            Assert.AreEqual("HydroNotifier", actual.from);
            Assert.AreEqual("Jablunkov MVE, Stav: Nizky prutok, Datum: 01.01.2019 13:50:30, Prutok: 5000,1 l/s", actual.text);
        }

        [Test]
        public void BuildMessage_NormalFlow()
        {
            var builder = new SmsMessageBuilder();
            var actual = builder.BuildMessage(_data, HydroStatus.Normal, new DateTime(2019, 1, 1, 13, 50, 30), "420735");

            Assert.AreEqual("420735", actual.to);
            Assert.AreEqual("HydroNotifier", actual.from);
            Assert.AreEqual("Jablunkov MVE, Stav: Normalni prutok, Datum: 01.01.2019 13:50:30, Prutok: 5000,1 l/s", actual.text);
        }
    }
}
