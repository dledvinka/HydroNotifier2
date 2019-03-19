using System;
using System.Collections.Generic;
using System.Text;
using HydroNotifier.FunctionApp.Core;
using NUnit.Framework;

namespace HydroNotifier.Tests
{
    public class HydroStatusCalculatorTest
    {
        private readonly List<HydroData> _lowFlowData = new List<HydroData>()
        {
            new HydroData(string.Empty, string.Empty, 100.0),
            new HydroData(string.Empty, string.Empty, 200.0),
        };

        private readonly List<HydroData> _normalFlowData = new List<HydroData>()
        {
            new HydroData(string.Empty, string.Empty, 1000.0),
            new HydroData(string.Empty, string.Empty, 2000.0),
        };

        private readonly List<HydroData> _highFlowData = new List<HydroData>()
        {
            new HydroData(string.Empty, string.Empty, 10000.0),
            new HydroData(string.Empty, string.Empty, 20000.0),
        };

        [Test]
        public void GetCurrentStatus_UnknownToLow()
        {
            var calc = new HydroStatusCalculator();
            var actual = calc.GetCurrentStatus(_lowFlowData, HydroStatus.Unknown);
            Assert.AreEqual(HydroStatus.Low, actual);
        }

        [Test]
        public void GetCurrentStatus_UnknownToNormal()
        {
            var calc = new HydroStatusCalculator();
            var actual = calc.GetCurrentStatus(_normalFlowData, HydroStatus.Unknown);
            Assert.AreEqual(HydroStatus.Normal, actual);
        }

        [Test]
        public void GetCurrentStatus_UnknownToHigh()
        {
            var calc = new HydroStatusCalculator();
            var actual = calc.GetCurrentStatus(_highFlowData, HydroStatus.Unknown);
            Assert.AreEqual(HydroStatus.High, actual);
        }

        [Test]
        public void GetCurrentStatus_LowToNormal()
        {
            var calc = new HydroStatusCalculator();
            var actual = calc.GetCurrentStatus(_normalFlowData, HydroStatus.Low);
            Assert.AreEqual(HydroStatus.Normal, actual);
        }

        [Test]
        public void GetCurrentStatus_LowToHigh()
        {
            var calc = new HydroStatusCalculator();
            var actual = calc.GetCurrentStatus(_highFlowData, HydroStatus.Low);
            Assert.AreEqual(HydroStatus.High, actual);
        }

        [Test]
        public void GetCurrentStatus_NormalToLow()
        {
            var calc = new HydroStatusCalculator();
            var actual = calc.GetCurrentStatus(_lowFlowData, HydroStatus.Normal);
            Assert.AreEqual(HydroStatus.Low, actual);
        }

        [Test]
        public void GetCurrentStatus_NormalToHigh()
        {
            var calc = new HydroStatusCalculator();
            var actual = calc.GetCurrentStatus(_highFlowData, HydroStatus.Normal);
            Assert.AreEqual(HydroStatus.High, actual);
        }

        [Test]
        public void GetCurrentStatus_HighToLow()
        {
            var calc = new HydroStatusCalculator();
            var actual = calc.GetCurrentStatus(_lowFlowData, HydroStatus.High);
            Assert.AreEqual(HydroStatus.Low, actual);
        }

        [Test]
        public void GetCurrentStatus_HighToNormal()
        {
            var calc = new HydroStatusCalculator();
            var actual = calc.GetCurrentStatus(_normalFlowData, HydroStatus.High);
            Assert.AreEqual(HydroStatus.Normal, actual);
        }
    }
}
