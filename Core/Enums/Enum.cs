namespace Core.Enums
{
    public static class Enum
    {
        public enum LogLevel
        {
            Trace = 0,
            Debug = 1,
            Info = 2,
            Warning = 3,
            Error = 4,
            Critical = 5
        }

        public enum TenantStatus : byte
        {
            Trial = 1,
            Active = 2,
            Suspended = 3
        }

        public enum CustomerStatus : byte
        {
            Active = 1,
            Inactive = 2,
            Blocked = 3
        }

        public enum MonitorCheckResult : byte
        {
            Up = 1,
            Down = 2,
            Timeout = 3,
            Error = 4
        }
    }
}
