namespace HydroNotifier.Tests;

using System.Collections.Generic;
using FluentAssertions;
using HydroNotifier.Core.Entities;
using HydroNotifier.Core.Utils;
using Moq;
using NUnit.Framework;

public class HydroStatusCalculatorTest
{
    private readonly List<HydroData> _highFlowData =
    [
        new (string.Empty, string.Empty, 10000.0),
        new (string.Empty, string.Empty, 20000.0)
    ];

    private readonly List<HydroData> _lowFlowData =
    [
        new (string.Empty, string.Empty, 100.0),
        new (string.Empty, string.Empty, 200.0)
    ];

    private readonly List<HydroData> _normalFlowData =
    [
        new (string.Empty, string.Empty, 1000.0),
        new (string.Empty, string.Empty, 2000.0)
    ];

    private readonly Mock<ITelemetry> _telemetryMock = new Mock<ITelemetry>();

    [Test]
    public void GetCurrentStatus_UnknownToLow()
    {
        var calc = new HydroStatusCalculator(_telemetryMock.Object);
        var actual = calc.GetCurrentStatus(_lowFlowData, HydroStatus.Unknown);
        actual.Should().Be(HydroStatus.Low);
    }

    [Test]
    public void GetCurrentStatus_UnknownToNormal()
    {
        var calc = new HydroStatusCalculator(_telemetryMock.Object);
        var actual = calc.GetCurrentStatus(_normalFlowData, HydroStatus.Unknown);
        actual.Should().Be(HydroStatus.Normal);
    }

    [Test]
    public void GetCurrentStatus_UnknownToHigh()
    {
        var calc = new HydroStatusCalculator(_telemetryMock.Object);
        var actual = calc.GetCurrentStatus(_highFlowData, HydroStatus.Unknown);
        actual.Should().Be(HydroStatus.High);
    }

    [Test]
    public void GetCurrentStatus_LowToNormal()
    {
        var calc = new HydroStatusCalculator(_telemetryMock.Object);
        var actual = calc.GetCurrentStatus(_normalFlowData, HydroStatus.Low);
        actual.Should().Be(HydroStatus.Normal);
    }

    [Test]
    public void GetCurrentStatus_LowToHigh()
    {
        var calc = new HydroStatusCalculator(_telemetryMock.Object);
        var actual = calc.GetCurrentStatus(_highFlowData, HydroStatus.Low);
        actual.Should().Be(HydroStatus.High);
    }

    [Test]
    public void GetCurrentStatus_NormalToLow()
    {
        var calc = new HydroStatusCalculator(_telemetryMock.Object);
        var actual = calc.GetCurrentStatus(_lowFlowData, HydroStatus.Normal);
        actual.Should().Be(HydroStatus.Low);
    }

    [Test]
    public void GetCurrentStatus_NormalToHigh()
    {
        var calc = new HydroStatusCalculator(_telemetryMock.Object);
        var actual = calc.GetCurrentStatus(_highFlowData, HydroStatus.Normal);
        actual.Should().Be(HydroStatus.High);
    }

    [Test]
    public void GetCurrentStatus_HighToLow()
    {
        var calc = new HydroStatusCalculator(_telemetryMock.Object);
        var actual = calc.GetCurrentStatus(_lowFlowData, HydroStatus.High);
        actual.Should().Be(HydroStatus.Low);
    }

    [Test]
    public void GetCurrentStatus_HighToNormal()
    {
        var calc = new HydroStatusCalculator(_telemetryMock.Object);
        var actual = calc.GetCurrentStatus(_normalFlowData, HydroStatus.High);
        actual.Should().Be(HydroStatus.Normal);
    }
}