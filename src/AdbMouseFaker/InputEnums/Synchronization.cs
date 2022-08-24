namespace AdbMouseFaker.InputEnums;

// ReSharper disable InconsistentNaming
public enum Synchronization
{
    SYN_REPORT    = 0,
    SYN_CONFIG    = 1,
    SYN_MT_REPORT = 2,
    SYN_DROPPED   = 3,
    SYN_MAX       = 0xf,
    SYN_CNT       = SYN_MAX + 1,
}
