using System.Collections.Generic;
using HydroNotifier.FunctionApp.Core;
using HydroNotifier.FunctionApp.Utils;
using Moq;
using NUnit.Framework;

namespace HydroNotifier.Tests
{
    public class HydroStatusCalculatorTest
    {
        private readonly List<HydroData> _lowFlowData = new List<HydroData>()
        {
            new HydroData(string.Empty, string.Empty, 100.0M),
            new HydroData(string.Empty, string.Empty, 200.0M),
        };

        private readonly List<HydroData> _normalFlowData = new List<HydroData>()
        {
            new HydroData(string.Empty, string.Empty, 1000.0M),
            new HydroData(string.Empty, string.Empty, 2000.0M),
        };

        private readonly List<HydroData> _highFlowData = new List<HydroData>()
        {
            new HydroData(string.Empty, string.Empty, 10000.0M),
            new HydroData(string.Empty, string.Empty, 20000.0M),
        };

        private readonly Mock<ITelemetry> _telemetry = new Mock<ITelemetry>();

        [Test]
        public void GetCurrentStatus_UnknownToLow()
        {
            var calc = new HydroStatusCalculator(_telemetry.Object);
            var actual = calc.GetCurrentStatus(_lowFlowData, HydroStatus.Unknown);
            Assert.AreEqual(HydroStatus.Low, actual);
        }

        [Test]
        public void GetCurrentStatus_UnknownToNormal()
        {
            var calc = new HydroStatusCalculator(_telemetry.Object);
            var actual = calc.GetCurrentStatus(_normalFlowData, HydroStatus.Unknown);
            Assert.AreEqual(HydroStatus.Normal, actual);
        }

        [Test]
        public void GetCurrentStatus_UnknownToHigh()
        {
            var calc = new HydroStatusCalculator(_telemetry.Object);
            var actual = calc.GetCurrentStatus(_highFlowData, HydroStatus.Unknown);
            Assert.AreEqual(HydroStatus.High, actual);
        }

        [Test]
        public void GetCurrentStatus_LowToNormal()
        {
            var calc = new HydroStatusCalculator(_telemetry.Object);
            var actual = calc.GetCurrentStatus(_normalFlowData, HydroStatus.Low);
            Assert.AreEqual(HydroStatus.Normal, actual);
        }

        [Test]
        public void GetCurrentStatus_LowToHigh()
        {
            var calc = new HydroStatusCalculator(_telemetry.Object);
            var actual = calc.GetCurrentStatus(_highFlowData, HydroStatus.Low);
            Assert.AreEqual(HydroStatus.High, actual);
        }

        [Test]
        public void GetCurrentStatus_NormalToLow()
        {
            var calc = new HydroStatusCalculator(_telemetry.Object);
            var actual = calc.GetCurrentStatus(_lowFlowData, HydroStatus.Normal);
            Assert.AreEqual(HydroStatus.Low, actual);
        }

        [Test]
        public void GetCurrentStatus_NormalToHigh()
        {
            var calc = new HydroStatusCalculator(_telemetry.Object);
            var actual = calc.GetCurrentStatus(_highFlowData, HydroStatus.Normal);
            Assert.AreEqual(HydroStatus.High, actual);
        }

        [Test]
        public void GetCurrentStatus_HighToLow()
        {
            var calc = new HydroStatusCalculator(_telemetry.Object);
            var actual = calc.GetCurrentStatus(_lowFlowData, HydroStatus.High);
            Assert.AreEqual(HydroStatus.Low, actual);
        }

        [Test]
        public void GetCurrentStatus_HighToNormal()
        {
            var calc = new HydroStatusCalculator(_telemetry.Object);
            var actual = calc.GetCurrentStatus(_normalFlowData, HydroStatus.High);
            Assert.AreEqual(HydroStatus.Normal, actual);
        }
    }
}
