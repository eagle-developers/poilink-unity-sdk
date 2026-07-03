using System.Collections.Generic;
using System.Threading.Tasks;

namespace Poilink
{
    public static partial class PoilinkSDK
    {
        public static Task InitializeAsync()
        {
            var tcs = new TaskCompletionSource<bool>(TaskCreationOptions.RunContinuationsAsynchronously);

            Initialize(
                () => tcs.TrySetResult(true),
                (errorCode, message) => tcs.TrySetException(new PoilinkException(errorCode, message))
            );

            return tcs.Task;
        }

        public static Task AuthenticateAsync(string appUserId)
        {
            var tcs = new TaskCompletionSource<bool>(TaskCreationOptions.RunContinuationsAsynchronously);

            Authenticate(
                appUserId,
                () => tcs.TrySetResult(true),
                (errorCode, message) => tcs.TrySetException(new PoilinkException(errorCode, message))
            );

            return tcs.Task;
        }

        public static Task<string> GetRefreshTokenAsync()
        {
            var tcs = new TaskCompletionSource<string>(TaskCreationOptions.RunContinuationsAsynchronously);

            GetRefreshToken(
                refreshToken => tcs.TrySetResult(refreshToken),
                (errorCode, message) => tcs.TrySetException(new PoilinkException(errorCode, message))
            );

            return tcs.Task;
        }

        public static Task SetRefreshTokenAsync(string appUserId, string refreshToken)
        {
            var tcs = new TaskCompletionSource<bool>(TaskCreationOptions.RunContinuationsAsynchronously);

            SetRefreshToken(
                appUserId,
                refreshToken,
                () => tcs.TrySetResult(true),
                (errorCode, message) => tcs.TrySetException(new PoilinkException(errorCode, message))
            );

            return tcs.Task;
        }

        public static Task<List<MissionData>> ProgressMissionImmediateAsync(string missionCode, int amount,
            ProgressMissionMode mode, string idempotencyKey = null)
        {
            var tcs = new TaskCompletionSource<List<MissionData>>(TaskCreationOptions.RunContinuationsAsynchronously);

            ProgressMissionImmediate(
                missionCode,
                amount,
                mode,
                result => tcs.TrySetResult(result),
                (errorCode, message) => tcs.TrySetException(new PoilinkException(errorCode, message)),
                idempotencyKey
            );

            return tcs.Task;
        }
    }
}
