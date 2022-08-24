using System;
using System.Net;
using AdbSender;
using AdbSender.Models;

namespace AdbMouseFaker.Unit.Tests;

public abstract class AdbClientUnrollRefs : IAdbClient
{
    public abstract INoTransportAdbClient WithNoTransport();

    public abstract IAdbClient WithTransport(ReadOnlySpan<char> deviceSerial);

    public abstract IAdbClient WithTcpConnection(DnsEndPoint endPoint);

    public abstract IAdbClient WithUsbTransport();

    public abstract IAdbClient WithTcpTransport();

    public abstract IAdbClient WithAnyTransport();

    public abstract void Connect(DnsEndPoint endPoint);

    public abstract void ExecuteSendEvent(string device, EventType type, int code, int value);

    public void ExecuteSendEvent(ReadOnlySpan<char> inputDevice, in InputEvent inputEvent) => this.ExecuteSendEvent(
        new string(inputDevice),
        (EventType)inputEvent.Type,
        inputEvent.Code,
        inputEvent.Value
    );
}

