namespace AdbMouseFaker
{
    public static class SendEventConstant
    {
        public const int EV_SYN = 0x00,
                         EV_KEY = 0x01,
                         EV_ABS = 0x03,
                         SYN_REPORT = 0x00,
                         BTN_TOUCH = 0x14A,
                         ABS_MT_POSITION_X = 0x35,
                         ABS_MT_POSITION_Y = 0x36,
                         ABS_MT_TRACKING_ID = 0x39,
                         DEFAULT_TRACKING_ID = 1,
                         RELEASE_TRACKING_ID = -1;
    }
}
