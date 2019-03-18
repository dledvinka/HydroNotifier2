using HydroNotifier.Core;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace HydroNotifier.Tests
{
    public class SmsMessageBuilderTest
    {
        [Test]
        public void BuildMessage_HighFlow()
        {
            HydroData d1 = new HydroData("Řeka 1", new DateTime(2019, 1, 1, 13, 50, 20).ToString(), 2000.0);
            HydroData d2 = new HydroData("Řeka 2", new DateTime(2019, 1, 1, 13, 50, 20).ToString(), 3000.1);

            var builder = new SmsMessageBuilder();
            var actual = builder.BuildMessage(d1, d2, currentStatus: HydroStatus.High, new DateTime(2019, 1, 1, 13, 50, 30), "420735");

            Assert.AreEqual("420735", actual.to);
            Assert.AreEqual("HydroNotifier", actual.from);
            Assert.AreEqual("Jablunkov MVE, Stav: Vysoky prutok, Datum: 01.01.2019 13:50:30, Prutok: 5000,1 l/s", actual.text);
        }

        [Test]
        public void BuildMessage_LowFlow()
        {
            HydroData d1 = new HydroData("Řeka 1", new DateTime(2019, 1, 1, 13, 50, 20).ToString(), 2000.0);
            HydroData d2 = new HydroData("Řeka 2", new DateTime(2019, 1, 1, 13, 50, 20).ToString(), 3000.1);

            var builder = new SmsMessageBuilder();
            var actual = builder.BuildMessage(d1, d2, currentStatus: HydroStatus.Low, new DateTime(2019, 1, 1, 13, 50, 30), "420735");

            Assert.AreEqual("420735", actual.to);
            Assert.AreEqual("HydroNotifier", actual.from);
            Assert.AreEqual("Jablunkov MVE, Stav: Nizky prutok, Datum: 01.01.2019 13:50:30, Prutok: 5000,1 l/s", actual.text);
        }

        [Test]
        public void BuildMessage_NormalFlow()
        {
            HydroData d1 = new HydroData("Řeka 1", new DateTime(2019, 1, 1, 13, 50, 20).ToString(), 2000.0);
            HydroData d2 = new HydroData("Řeka 2", new DateTime(2019, 1, 1, 13, 50, 20).ToString(), 3000.1);

            var builder = new SmsMessageBuilder();
            var actual = builder.BuildMessage(d1, d2, currentStatus: HydroStatus.Normal, new DateTime(2019, 1, 1, 13, 50, 30), "420735");

            Assert.AreEqual("420735", actual.to);
            Assert.AreEqual("HydroNotifier", actual.from);
            Assert.AreEqual("Jablunkov MVE, Stav: Normalni prutok, Datum: 01.01.2019 13:50:30, Prutok: 5000,1 l/s", actual.text);
        }
    }
}
