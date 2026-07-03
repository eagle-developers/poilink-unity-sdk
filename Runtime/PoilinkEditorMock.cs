#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Poilink
{
    internal static class PoilinkEditorMock
    {
        internal static Func<Task> InitializeBehavior;
        internal static Func<string, Task> AuthenticateBehavior;
        internal static Action UnauthenticateBehavior;
        internal static Func<Task<string>> GetRefreshTokenBehavior;
        internal static Func<string, string, Task> SetRefreshTokenBehavior;
        internal static Func<string, int, PoilinkSDK.ProgressMissionMode, Task> ProgressMissionBehavior;
        internal static Func<string, int, PoilinkSDK.ProgressMissionMode, string, Task<List<PoilinkSDK.MissionData>>> ProgressMissionImmediateBehavior;
        internal static Func<PoilinkSDK.MissionListFilter, List<PoilinkSDK.MissionData>> GetMissionListBehavior;
        internal static Action<PoilinkSDK.WebPortalOptions> ShowWebPortalBehavior;
        internal static Action CloseWebPortalBehavior;
        internal static Action<PoilinkSDK.WebPortalOptions> PreloadWebPortalBehavior;
        internal static Func<PoilinkSDK.ListPendingItemGrantsResult> ListPendingItemGrantsBehavior;
        internal static Func<string[], PoilinkSDK.MarkItemGrantsConsumedResult> MarkItemGrantsConsumedBehavior;

        internal const string DefaultRefreshToken = "mock_refresh_token_12345";

        static PoilinkEditorMock()
        {
            Reset();
        }

        internal static void Reset()
        {
            InitializeBehavior = () => Task.CompletedTask;
            AuthenticateBehavior = _ => Task.CompletedTask;
            UnauthenticateBehavior = () => { };
            GetRefreshTokenBehavior = () => Task.FromResult(DefaultRefreshToken);
            SetRefreshTokenBehavior = (_, __) => Task.CompletedTask;
            ProgressMissionBehavior = (_, __, ___) => Task.CompletedTask;
            ProgressMissionImmediateBehavior = (_, __, ___, ____) => Task.FromResult(new List<PoilinkSDK.MissionData>());
            GetMissionListBehavior = _ => new List<PoilinkSDK.MissionData>();
            ShowWebPortalBehavior = opts => opts?.OnClose?.Invoke();
            CloseWebPortalBehavior = () => { };
            PreloadWebPortalBehavior = _ => { };
            ListPendingItemGrantsBehavior = () => new PoilinkSDK.ListPendingItemGrantsResult();
            MarkItemGrantsConsumedBehavior = ids => new PoilinkSDK.MarkItemGrantsConsumedResult
            {
                MarkedGrantIds = new List<string>(ids ?? new string[0]),
            };
        }

        internal static async void Dispatch(Func<Task> factory, PoilinkSDK.SuccessCallback onSuccess, PoilinkSDK.ErrorCallback onError, PoilinkErrorCode unexpectedErrorCode = PoilinkErrorCode.ConfigurationError)
        {
            try
            {
                var task = factory();
                if (task != null) await task.ConfigureAwait(false);
                onSuccess?.Invoke();
            }
            catch (PoilinkException ex)
            {
                onError?.Invoke(ex.ErrorCodeValue, ex.Message);
            }
            catch (Exception ex)
            {
                onError?.Invoke((int)unexpectedErrorCode, ex.Message);
            }
        }

        internal static async void DispatchResult(Func<Task<List<PoilinkSDK.MissionData>>> factory, Action<List<PoilinkSDK.MissionData>> onSuccess, PoilinkSDK.ErrorCallback onError, PoilinkErrorCode unexpectedErrorCode = PoilinkErrorCode.ConfigurationError)
        {
            try
            {
                var task = factory();
                var result = task != null ? await task.ConfigureAwait(false) : new List<PoilinkSDK.MissionData>();
                onSuccess?.Invoke(result);
            }
            catch (PoilinkException ex)
            {
                onError?.Invoke(ex.ErrorCodeValue, ex.Message);
            }
            catch (Exception ex)
            {
                onError?.Invoke((int)unexpectedErrorCode, ex.Message);
            }
        }

        internal static async void DispatchString(Func<Task<string>> factory, PoilinkSDK.RefreshTokenCallback onSuccess, PoilinkSDK.ErrorCallback onError, PoilinkErrorCode unexpectedErrorCode = PoilinkErrorCode.ConfigurationError)
        {
            try
            {
                var task = factory();
                var result = task != null ? await task.ConfigureAwait(false) : null;
                onSuccess?.Invoke(result);
            }
            catch (PoilinkException ex)
            {
                onError?.Invoke(ex.ErrorCodeValue, ex.Message);
            }
            catch (Exception ex)
            {
                onError?.Invoke((int)unexpectedErrorCode, ex.Message);
            }
        }
    }
}
#endif
