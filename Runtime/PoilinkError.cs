using System;

namespace Poilink
{
    public enum PoilinkErrorCode
    {
        NotInitialized = 1001,
        ConfigurationError = 1002,
        AuthenticationFailed = 1003,
        Network = 1004,
        InvalidResponse = 1005,
        StorageError = 1006,
        Timeout = 1007,
        ConnectionRefused = 1008,
        HttpServerError = 1009,
        ParseError = 1010,
        ValidationError = 1011,
        AuthRecoverable = 1012,
        AlreadyAuthenticated = 1013,
        RenderProcessGone = 1014,
    }

    public class PoilinkException : Exception
    {
        public PoilinkException(int errorCode, string message) : base(message)
        {
            ErrorCodeValue = errorCode;
            ErrorCode = (PoilinkErrorCode)errorCode;
        }

        public PoilinkException(PoilinkErrorCode errorCode, string message) : base(message)
        {
            ErrorCode = errorCode;
            ErrorCodeValue = (int)errorCode;
        }

        public PoilinkErrorCode ErrorCode { get; }

        public int ErrorCodeValue { get; }

        public override string ToString()
        {
            return $"PoilinkException: [{ErrorCode}({ErrorCodeValue})] {Message}";
        }
    }
}
