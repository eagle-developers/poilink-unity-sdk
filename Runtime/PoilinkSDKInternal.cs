using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;
using AOT;
using UnityEngine;

namespace Poilink
{
    public static partial class PoilinkSDK
    {
#if UNITY_IOS && !UNITY_EDITOR
        private const string DllName = "__Internal";
#elif UNITY_ANDROID && !UNITY_EDITOR
        private const string DllName = "poilink";
#else
        private const string DllName = "PoilinkSDKMock";
#endif

        [StructLayout(LayoutKind.Sequential)]
        private struct WebPortalOptionsNative
        {
            public float volume;
            public IntPtr onClose;
            public IntPtr onError;
            public IntPtr onShown;
            public int showMode;
            public int embeddedX;
            public int embeddedY;
            public int embeddedWidth;
            public int embeddedHeight;
            public IntPtr onMissionChallenge;
            public IntPtr onRewardReceive;
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct MissionDataNative
        {
            public IntPtr inAppMissionId;
            public IntPtr progressCode;
            public IntPtr title;
            public IntPtr details;
            public long point;
            public int targetProgress;
            public int currentProgress;
            public int hasAchievement;
            public int isClaimed;
            public int displayOrder;
            public int cycleType;
            public int rewardType;
            public IntPtr rewardItemCode;
            public int rewardItemQuantity;
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct MissionListNative
        {
            public IntPtr items;
            public int count;
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct MissionListFilterNative
        {
            public int cycleType;
            public int rewardType;
            public IntPtr progressCode;
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct MissionProgressResultNative
        {
            public IntPtr missions;
            public int count;
        }

#pragma warning disable IDE0051
#pragma warning disable CS0626

#if UNITY_ANDROID && !UNITY_EDITOR
        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        private static extern void SetActivity(IntPtr activity);

        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        private static extern void SetConfig(string clientId, string clientSecret);
#endif

#if UNITY_IOS && !UNITY_EDITOR
        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        private static extern void SetConfig(string clientId, string clientSecret);
#endif

        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        private static extern void Initialize(
            SuccessCallbackNative onSuccess,
            ErrorCallbackNative onError
        );

        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        private static extern void Authenticate(
            string appUserId,
            SuccessCallbackNative onSuccess,
            ErrorCallbackNative onError
        );

        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl, EntryPoint = "Unauthenticate")]
        private static extern void _Unauthenticate();

        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl, EntryPoint = "ShowWebPortal")]
        private static extern void _ShowWebPortal(in WebPortalOptionsNative options);

        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl, EntryPoint = "CloseWebPortal")]
        private static extern void _CloseWebPortal();

        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl, EntryPoint = "PreloadWebPortal")]
        private static extern void _PreloadWebPortal(in WebPortalOptionsNative options);

        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        private static extern void GetRefreshToken(
            RefreshTokenCallbackNative onSuccess,
            ErrorCallbackNative onError
        );

        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        private static extern void SetRefreshToken(
            string appUserId,
            string refreshToken,
            SuccessCallbackNative onSuccess,
            ErrorCallbackNative onError
        );

        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        private static extern void ProgressMission(
            string missionCode,
            int amount,
            int mode,
            long userData,
            ProgressMissionCompleteCallbackNative onComplete,
            ProgressMissionErrorCallbackNative onError
        );

        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        private static extern void ProgressMissionImmediate(
            string missionCode,
            int amount,
            int mode,
            MissionProgressResultCallbackNative onSuccess,
            ErrorCallbackNative onError,
            string idempotencyKey
        );

        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl, EntryPoint = "GetMissionList")]
        private static extern MissionListNative _GetMissionList(IntPtr filterPtr);

#pragma warning restore CS0626
#pragma warning restore IDE0051

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate void SuccessCallbackNative();

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate void ErrorCallbackNative(int errorCode, IntPtr messagePtr);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate void RefreshTokenCallbackNative(IntPtr stringPtr);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate void MissionChallengeCallbackNative(IntPtr missionIdPtr);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate void RewardReceiveCallbackNative(IntPtr grantIdPtr, IntPtr itemCodePtr, int quantity, IntPtr missionCodePtr);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate void MissionProgressResultCallbackNative(IntPtr resultPtr);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate void ProgressMissionCompleteCallbackNative(long userData);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate void ProgressMissionErrorCallbackNative(long userData, int errorCode, IntPtr messagePtr);

        private static SuccessCallback _initSuccessCallback;
        private static ErrorCallback _initErrorCallback;
        private static SuccessCallback _authSuccessCallback;
        private static ErrorCallback _authErrorCallback;
        private static RefreshTokenCallback _getRefreshTokenSuccessCallback;
        private static ErrorCallback _getRefreshTokenErrorCallback;
        private static SuccessCallback _setRefreshTokenSuccessCallback;
        private static ErrorCallback _setRefreshTokenErrorCallback;
        private static readonly System.Collections.Generic.Dictionary<long, ErrorCallback> _progressMissionCallbacks = new System.Collections.Generic.Dictionary<long, ErrorCallback>();
        private static long _nextProgressMissionHandle;
        private static Action<List<MissionData>> _progressMissionImmediateSuccessCallback;
        private static ErrorCallback _progressMissionImmediateErrorCallback;
        private static SuccessCallback _showWebPortalOnCloseCallback;
        private static SuccessCallback _showWebPortalOnShownCallback;
        private static ErrorCallback _showWebPortalOnErrorCallback;
        private static MissionChallengeCallback _showWebPortalOnMissionChallengeCallback;
        private static RewardReceiveCallback _showWebPortalOnRewardReceiveCallback;
#if (UNITY_ANDROID || UNITY_IOS) && !UNITY_EDITOR
        private static SuccessCallbackNative _showWebPortalOnCloseNativeCallback;
        private static SuccessCallbackNative _showWebPortalOnShownNativeCallback;
        private static ErrorCallbackNative _showWebPortalOnErrorNativeCallback;
        private static MissionChallengeCallbackNative _showWebPortalOnMissionChallengeNativeCallback;
        private static RewardReceiveCallbackNative _showWebPortalOnRewardReceiveNativeCallback;
#endif

        [MonoPInvokeCallback(typeof(SuccessCallbackNative))]
        private static void OnShowWebPortalClose()
        {
            var callback = _showWebPortalOnCloseCallback;
            _showWebPortalOnCloseCallback = null;
            _showWebPortalOnShownCallback = null;
            _showWebPortalOnErrorCallback = null;
            _showWebPortalOnMissionChallengeCallback = null;
            _showWebPortalOnRewardReceiveCallback = null;
#if (UNITY_ANDROID || UNITY_IOS) && !UNITY_EDITOR
            _showWebPortalOnCloseNativeCallback = null;
            _showWebPortalOnShownNativeCallback = null;
            _showWebPortalOnErrorNativeCallback = null;
            _showWebPortalOnMissionChallengeNativeCallback = null;
            _showWebPortalOnRewardReceiveNativeCallback = null;
#endif

            if (callback != null)
            {
                UnityMainThreadDispatcher.Enqueue(() => callback());
            }
        }

        [MonoPInvokeCallback(typeof(SuccessCallbackNative))]
        private static void OnShowWebPortalShown()
        {
            var callback = _showWebPortalOnShownCallback;
            _showWebPortalOnShownCallback = null;
            if (callback != null)
            {
                UnityMainThreadDispatcher.Enqueue(() => callback());
            }
        }

        [MonoPInvokeCallback(typeof(ErrorCallbackNative))]
        private static void OnShowWebPortalError(int errorCode, IntPtr messagePtr)
        {
            var message = Marshal.PtrToStringAnsi(messagePtr) ?? string.Empty;
            var callback = _showWebPortalOnErrorCallback;

            if (callback != null)
            {
                UnityMainThreadDispatcher.Enqueue(() => callback(errorCode, message));
            }
        }

        [MonoPInvokeCallback(typeof(MissionChallengeCallbackNative))]
        private static void OnShowWebPortalMissionChallenge(IntPtr missionIdPtr)
        {
            var missionId = Marshal.PtrToStringAnsi(missionIdPtr) ?? string.Empty;

            var callback = _showWebPortalOnMissionChallengeCallback;

            if (callback != null)
            {
                UnityMainThreadDispatcher.Enqueue(() =>
                {
                    callback(missionId);
                });
            }
        }

        [MonoPInvokeCallback(typeof(RewardReceiveCallbackNative))]
        private static void OnShowWebPortalRewardReceive(IntPtr grantIdPtr, IntPtr itemCodePtr, int quantity, IntPtr missionCodePtr)
        {
            var grantId = Marshal.PtrToStringAnsi(grantIdPtr) ?? string.Empty;
            var itemCode = Marshal.PtrToStringAnsi(itemCodePtr) ?? string.Empty;
            var missionCode = Marshal.PtrToStringAnsi(missionCodePtr) ?? string.Empty;

            var callback = _showWebPortalOnRewardReceiveCallback;

            if (callback != null)
            {
                UnityMainThreadDispatcher.Enqueue(() =>
                {
                    callback(grantId, itemCode, quantity, missionCode);
                    AutoMarkRewardConsumed(grantId);
                });
            }
        }

        internal class ListPendingItemGrantsResult
        {
            public List<PendingItemGrant> Grants { get; set; } = new List<PendingItemGrant>();
            public bool HasMore { get; set; }
        }

        internal class MarkItemGrantsConsumedResult
        {
            public List<string> MarkedGrantIds { get; set; } = new List<string>();
            public List<string> AlreadyConsumedOrUnknownGrantIds { get; set; } = new List<string>();
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct PendingItemGrantDataNative
        {
            public IntPtr grantId;
            public IntPtr itemCode;
            public int quantity;
            public long grantedAtUnixMs;
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct ListPendingItemGrantsResultDataNative
        {
            public IntPtr grants;
            public int count;
            public int hasMore;
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct MarkItemGrantsConsumedResultDataNative
        {
            public IntPtr markedGrantIds;
            public int markedCount;
            public IntPtr alreadyConsumedOrUnknownGrantIds;
            public int alreadyConsumedOrUnknownCount;
        }

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate void ListPendingItemGrantsCallbackNative(IntPtr resultPtr);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate void MarkItemGrantsConsumedCallbackNative(IntPtr resultPtr);

#pragma warning disable IDE0051
#pragma warning disable CS0626
        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        private static extern void ListPendingItemGrants(
            ListPendingItemGrantsCallbackNative onSuccess,
            ErrorCallbackNative onError
        );

        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        private static extern void MarkItemGrantsConsumed(
            [In, MarshalAs(UnmanagedType.LPArray, ArraySubType = UnmanagedType.LPStr)] string[] grantIds,
            int count,
            MarkItemGrantsConsumedCallbackNative onSuccess,
            ErrorCallbackNative onError
        );
#pragma warning restore CS0626
#pragma warning restore IDE0051

        private class ListGrantsPending
        {
            public Action<ListPendingItemGrantsResult> OnSuccess;
            public ErrorCallback OnError;
        }

        private class MarkGrantsPending
        {
            public Action<MarkItemGrantsConsumedResult> OnSuccess;
            public ErrorCallback OnError;
        }

        // TODO: RemoteConfig から取得できるようにする (現状はハードコード)
        private const int ItemGrantMaxBatchSize = 50;

        private static readonly Queue<ListGrantsPending> _listGrantsQueue = new Queue<ListGrantsPending>();
        private static readonly Queue<MarkGrantsPending> _markGrantsQueue = new Queue<MarkGrantsPending>();
        private static readonly object _itemGrantLock = new object();

        private static readonly ListPendingItemGrantsCallbackNative _listGrantsSuccessNative = OnListPendingItemGrantsSuccess;
        private static readonly ErrorCallbackNative _listGrantsErrorNative = OnListPendingItemGrantsError;
        private static readonly MarkItemGrantsConsumedCallbackNative _markGrantsSuccessNative = OnMarkItemGrantsConsumedSuccess;
        private static readonly ErrorCallbackNative _markGrantsErrorNative = OnMarkItemGrantsConsumedError;

        internal static void _InternalSyncItemGrants(GrantsCallback onGrants, SyncCompleteCallback onComplete, ErrorCallback onError)
        {
            SyncItemGrantsPage(onGrants, onComplete, onError, 0, 0, 0);
        }

        private static void SyncItemGrantsPage(GrantsCallback onGrants, SyncCompleteCallback onComplete, ErrorCallback onError,
            int totalSynced, int totalMarked, int pageCount)
        {
            _InternalListPendingItemGrants(
                page =>
                {
                    int pc = pageCount + 1;
                    if (page.Grants.Count > 0)
                    {
                        int ts = totalSynced + page.Grants.Count;
                        onGrants?.Invoke(page.Grants);
                        var ids = page.Grants.ConvertAll(g => g.GrantId).ToArray();
                        _InternalMarkItemGrantsConsumed(ids,
                            marked =>
                            {
                                int tm = totalMarked + marked.MarkedGrantIds.Count;
                                if (page.HasMore)
                                {
                                    SyncItemGrantsPage(onGrants, onComplete, onError, ts, tm, pc);
                                }
                                else
                                {
                                    onComplete?.Invoke(new SyncItemGrantsResult { TotalSynced = ts, TotalMarked = tm, PageCount = pc });
                                }
                            },
                            onError);
                    }
                    else
                    {
                        onComplete?.Invoke(new SyncItemGrantsResult { TotalSynced = totalSynced, TotalMarked = totalMarked, PageCount = pc });
                    }
                },
                onError);
        }

        private static void AutoMarkRewardConsumed(string grantId)
        {
            if (string.IsNullOrEmpty(grantId)) return;
            _InternalMarkItemGrantsConsumed(new[] { grantId }, _ => { }, (_, __) => { });
        }

        internal static void _InternalListPendingItemGrants(Action<ListPendingItemGrantsResult> onSuccess, ErrorCallback onError)
        {
#if UNITY_EDITOR
            try
            {
                var result = PoilinkEditorMock.ListPendingItemGrantsBehavior?.Invoke() ?? new ListPendingItemGrantsResult();
                onSuccess?.Invoke(result);
            }
            catch (PoilinkException ex)
            {
                onError?.Invoke(ex.ErrorCodeValue, ex.Message);
            }
            catch (Exception ex)
            {
                onError?.Invoke((int)PoilinkErrorCode.Network, ex.Message);
            }
#else
            lock (_itemGrantLock)
            {
                _listGrantsQueue.Enqueue(new ListGrantsPending { OnSuccess = onSuccess, OnError = onError });
            }
            ListPendingItemGrants(_listGrantsSuccessNative, _listGrantsErrorNative);
#endif
        }

        internal static void _InternalMarkItemGrantsConsumed(string[] grantIds, Action<MarkItemGrantsConsumedResult> onSuccess, ErrorCallback onError)
        {
#if UNITY_EDITOR
            try
            {
                if (grantIds == null || grantIds.Length == 0 || grantIds.Length > ItemGrantMaxBatchSize)
                {
                    throw new PoilinkException(PoilinkErrorCode.ValidationError,
                        $"grantIds must contain 1..{ItemGrantMaxBatchSize} items (got {grantIds?.Length ?? 0})");
                }
                var result = PoilinkEditorMock.MarkItemGrantsConsumedBehavior?.Invoke(grantIds)
                             ?? new MarkItemGrantsConsumedResult { MarkedGrantIds = new List<string>(grantIds) };
                onSuccess?.Invoke(result);
            }
            catch (PoilinkException ex)
            {
                onError?.Invoke(ex.ErrorCodeValue, ex.Message);
            }
            catch (Exception ex)
            {
                onError?.Invoke((int)PoilinkErrorCode.Network, ex.Message);
            }
#else
            lock (_itemGrantLock)
            {
                _markGrantsQueue.Enqueue(new MarkGrantsPending { OnSuccess = onSuccess, OnError = onError });
            }
            MarkItemGrantsConsumed(grantIds ?? new string[0], grantIds?.Length ?? 0, _markGrantsSuccessNative, _markGrantsErrorNative);
#endif
        }

        [MonoPInvokeCallback(typeof(ListPendingItemGrantsCallbackNative))]
        private static void OnListPendingItemGrantsSuccess(IntPtr resultPtr)
        {
            var result = MarshalListResult(resultPtr);
            ListGrantsPending pending;
            lock (_itemGrantLock)
            {
                pending = _listGrantsQueue.Count > 0 ? _listGrantsQueue.Dequeue() : null;
            }
            if (pending?.OnSuccess != null)
            {
                UnityMainThreadDispatcher.Enqueue(() => pending.OnSuccess(result));
            }
        }

        [MonoPInvokeCallback(typeof(ErrorCallbackNative))]
        private static void OnListPendingItemGrantsError(int errorCode, IntPtr messagePtr)
        {
            var message = Marshal.PtrToStringAnsi(messagePtr) ?? string.Empty;
            ListGrantsPending pending;
            lock (_itemGrantLock)
            {
                pending = _listGrantsQueue.Count > 0 ? _listGrantsQueue.Dequeue() : null;
            }
            if (pending?.OnError != null)
            {
                UnityMainThreadDispatcher.Enqueue(() => pending.OnError(errorCode, message));
            }
        }

        [MonoPInvokeCallback(typeof(MarkItemGrantsConsumedCallbackNative))]
        private static void OnMarkItemGrantsConsumedSuccess(IntPtr resultPtr)
        {
            var result = MarshalMarkResult(resultPtr);
            MarkGrantsPending pending;
            lock (_itemGrantLock)
            {
                pending = _markGrantsQueue.Count > 0 ? _markGrantsQueue.Dequeue() : null;
            }
            if (pending?.OnSuccess != null)
            {
                UnityMainThreadDispatcher.Enqueue(() => pending.OnSuccess(result));
            }
        }

        [MonoPInvokeCallback(typeof(ErrorCallbackNative))]
        private static void OnMarkItemGrantsConsumedError(int errorCode, IntPtr messagePtr)
        {
            var message = Marshal.PtrToStringAnsi(messagePtr) ?? string.Empty;
            MarkGrantsPending pending;
            lock (_itemGrantLock)
            {
                pending = _markGrantsQueue.Count > 0 ? _markGrantsQueue.Dequeue() : null;
            }
            if (pending?.OnError != null)
            {
                UnityMainThreadDispatcher.Enqueue(() => pending.OnError(errorCode, message));
            }
        }

        private static ListPendingItemGrantsResult MarshalListResult(IntPtr resultPtr)
        {
            var result = new ListPendingItemGrantsResult();
            if (resultPtr == IntPtr.Zero) return result;
            var native = Marshal.PtrToStructure<ListPendingItemGrantsResultDataNative>(resultPtr);
            result.HasMore = native.hasMore != 0;
            if (native.grants != IntPtr.Zero && native.count > 0)
            {
                int stride = Marshal.SizeOf<PendingItemGrantDataNative>();
                for (int i = 0; i < native.count; i++)
                {
                    IntPtr itemPtr = new IntPtr(native.grants.ToInt64() + (long)i * stride);
                    var g = Marshal.PtrToStructure<PendingItemGrantDataNative>(itemPtr);
                    result.Grants.Add(new PendingItemGrant
                    {
                        GrantId = PtrToStringUtf8(g.grantId) ?? string.Empty,
                        ItemCode = PtrToStringUtf8(g.itemCode) ?? string.Empty,
                        Quantity = g.quantity,
                        GrantedAtUnixMs = g.grantedAtUnixMs,
                    });
                }
            }
            return result;
        }

        private static MarkItemGrantsConsumedResult MarshalMarkResult(IntPtr resultPtr)
        {
            var result = new MarkItemGrantsConsumedResult();
            if (resultPtr == IntPtr.Zero) return result;
            var native = Marshal.PtrToStructure<MarkItemGrantsConsumedResultDataNative>(resultPtr);
            CopyStringArray(native.markedGrantIds, native.markedCount, result.MarkedGrantIds);
            CopyStringArray(native.alreadyConsumedOrUnknownGrantIds, native.alreadyConsumedOrUnknownCount, result.AlreadyConsumedOrUnknownGrantIds);
            return result;
        }

        private static void CopyStringArray(IntPtr ptrArray, int count, List<string> dest)
        {
            if (ptrArray == IntPtr.Zero || count <= 0) return;
            int stride = IntPtr.Size;
            for (int i = 0; i < count; i++)
            {
                IntPtr slot = new IntPtr(ptrArray.ToInt64() + (long)i * stride);
                IntPtr strPtr = Marshal.ReadIntPtr(slot);
                dest.Add(PtrToStringUtf8(strPtr) ?? string.Empty);
            }
        }

        internal static IntPtr AllocUtf8(string s)
        {
            if (s == null) return IntPtr.Zero;
            byte[] bytes = Encoding.UTF8.GetBytes(s);
            IntPtr ptr = Marshal.AllocHGlobal(bytes.Length + 1);
            Marshal.Copy(bytes, 0, ptr, bytes.Length);
            Marshal.WriteByte(ptr, bytes.Length, 0);
            return ptr;
        }

        internal static string PtrToStringUtf8(IntPtr ptr)
        {
            if (ptr == IntPtr.Zero) return null;
            int len = 0;
            while (Marshal.ReadByte(ptr, len) != 0) len++;
            if (len == 0) return string.Empty;
            byte[] buf = new byte[len];
            Marshal.Copy(ptr, buf, 0, len);
            return Encoding.UTF8.GetString(buf);
        }

        private static List<MissionData> MarshalProgressResult(IntPtr resultPtr)
        {
            var progressedMissions = new List<MissionData>();
            if (resultPtr == IntPtr.Zero) return progressedMissions;

            var native = Marshal.PtrToStructure<MissionProgressResultNative>(resultPtr);
            if (native.missions != IntPtr.Zero && native.count > 0)
            {
                int stride = Marshal.SizeOf<MissionDataNative>();
                for (int i = 0; i < native.count; i++)
                {
                    IntPtr itemPtr = new IntPtr(native.missions.ToInt64() + (long)i * stride);
                    var n = Marshal.PtrToStructure<MissionDataNative>(itemPtr);
                    progressedMissions.Add(new MissionData
                    {
                        InAppMissionId = PtrToStringUtf8(n.inAppMissionId) ?? string.Empty,
                        ProgressCode = PtrToStringUtf8(n.progressCode) ?? string.Empty,
                        Title = PtrToStringUtf8(n.title) ?? string.Empty,
                        Details = PtrToStringUtf8(n.details) ?? string.Empty,
                        Point = n.point,
                        TargetProgress = n.targetProgress,
                        CurrentProgress = n.currentProgress,
                        HasAchievement = n.hasAchievement != 0,
                        IsClaimed = n.isClaimed != 0,
                        DisplayOrder = n.displayOrder,
                        CycleType = (CycleType)n.cycleType,
                        RewardType = (RewardType)n.rewardType,
                        RewardItemCode = PtrToStringUtf8(n.rewardItemCode) ?? string.Empty,
                        RewardItemQuantity = n.rewardItemQuantity,
                    });
                }
            }
            return progressedMissions;
        }

        [MonoPInvokeCallback(typeof(SuccessCallbackNative))]
        private static void OnInitializeSuccess()
        {
            UnityMainThreadDispatcher.Enqueue(() =>
            {
                _initSuccessCallback?.Invoke();
            });
        }

        [MonoPInvokeCallback(typeof(ErrorCallbackNative))]
        private static void OnInitializeError(int errorCode, IntPtr messagePtr)
        {
            var message = Marshal.PtrToStringAnsi(messagePtr) ?? string.Empty;
            UnityMainThreadDispatcher.Enqueue(() =>
            {
                _initErrorCallback?.Invoke(errorCode, message);
            });
        }

        [MonoPInvokeCallback(typeof(SuccessCallbackNative))]
        private static void OnAuthenticateSuccess()
        {
            UnityMainThreadDispatcher.Enqueue(() =>
            {
                _authSuccessCallback?.Invoke();
            });
        }

        [MonoPInvokeCallback(typeof(ErrorCallbackNative))]
        private static void OnAuthenticateError(int errorCode, IntPtr messagePtr)
        {
            var message = Marshal.PtrToStringAnsi(messagePtr) ?? string.Empty;
            UnityMainThreadDispatcher.Enqueue(() =>
            {
                _authErrorCallback?.Invoke(errorCode, message);
            });
        }

        [MonoPInvokeCallback(typeof(RefreshTokenCallbackNative))]
        private static void OnGetRefreshTokenSuccess(IntPtr stringPtr)
        {
            var refreshToken = Marshal.PtrToStringAnsi(stringPtr) ?? string.Empty;
            UnityMainThreadDispatcher.Enqueue(() =>
            {
                _getRefreshTokenSuccessCallback?.Invoke(refreshToken);
            });
        }

        [MonoPInvokeCallback(typeof(ErrorCallbackNative))]
        private static void OnGetRefreshTokenError(int errorCode, IntPtr messagePtr)
        {
            var message = Marshal.PtrToStringAnsi(messagePtr) ?? string.Empty;
            UnityMainThreadDispatcher.Enqueue(() =>
            {
                _getRefreshTokenErrorCallback?.Invoke(errorCode, message);
            });
        }

        [MonoPInvokeCallback(typeof(SuccessCallbackNative))]
        private static void OnSetRefreshTokenSuccess()
        {
            UnityMainThreadDispatcher.Enqueue(() =>
            {
                _setRefreshTokenSuccessCallback?.Invoke();
            });
        }

        [MonoPInvokeCallback(typeof(ErrorCallbackNative))]
        private static void OnSetRefreshTokenError(int errorCode, IntPtr messagePtr)
        {
            var message = Marshal.PtrToStringAnsi(messagePtr) ?? string.Empty;
            UnityMainThreadDispatcher.Enqueue(() =>
            {
                _setRefreshTokenErrorCallback?.Invoke(errorCode, message);
            });
        }

        [MonoPInvokeCallback(typeof(ProgressMissionCompleteCallbackNative))]
        private static void OnProgressMissionComplete(long userData)
        {
            lock (_progressMissionCallbacks)
            {
                _progressMissionCallbacks.Remove(userData);
            }
        }

        [MonoPInvokeCallback(typeof(ProgressMissionErrorCallbackNative))]
        private static void OnProgressMissionError(long userData, int errorCode, IntPtr messagePtr)
        {
            ErrorCallback cb = null;
            lock (_progressMissionCallbacks)
            {
                _progressMissionCallbacks.TryGetValue(userData, out cb);
                _progressMissionCallbacks.Remove(userData);
            }
            if (cb == null) return;
            var message = Marshal.PtrToStringAnsi(messagePtr) ?? string.Empty;
            UnityMainThreadDispatcher.Enqueue(() => cb(errorCode, message));
        }

        [MonoPInvokeCallback(typeof(MissionProgressResultCallbackNative))]
        private static void OnProgressMissionImmediateSuccess(IntPtr resultPtr)
        {
            var result = MarshalProgressResult(resultPtr);
            UnityMainThreadDispatcher.Enqueue(() =>
            {
                _progressMissionImmediateSuccessCallback?.Invoke(result);
            });
        }

        [MonoPInvokeCallback(typeof(ErrorCallbackNative))]
        private static void OnProgressMissionImmediateError(int errorCode, IntPtr messagePtr)
        {
            var message = Marshal.PtrToStringAnsi(messagePtr) ?? string.Empty;
            UnityMainThreadDispatcher.Enqueue(() =>
            {
                _progressMissionImmediateErrorCallback?.Invoke(errorCode, message);
            });
        }

        private static void _InternalInitialize(SuccessCallback onSuccess, ErrorCallback onError)
        {
            var settings = PoilinkSettings.Instance;
            if (settings != null && !string.IsNullOrEmpty(settings.clientId) && !string.IsNullOrEmpty(settings.clientSecret))
            {
#if (UNITY_ANDROID || UNITY_IOS) && !UNITY_EDITOR
                SetConfig(settings.clientId, settings.clientSecret);
#endif
            }

#if UNITY_EDITOR
            PoilinkEditorMock.Dispatch(() => PoilinkEditorMock.InitializeBehavior(), onSuccess, onError);
#else
#if UNITY_ANDROID
            try
            {
                using (AndroidJavaClass unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer"))
                {
                    using (AndroidJavaObject currentActivity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity"))
                    {
                        IntPtr activityPtr = currentActivity.GetRawObject();
                        SetActivity(activityPtr);
                    }
                }
            }
            catch (System.Exception e)
            {
                onError?.Invoke(1001, "Failed to set Activity");
                return;
            }
#endif

            _initSuccessCallback = onSuccess;
            _initErrorCallback = onError;

            var successWrapper = new SuccessCallbackNative(OnInitializeSuccess);
            var errorWrapper = new ErrorCallbackNative(OnInitializeError);

            Initialize(successWrapper, errorWrapper);
#endif
        }

        private static void _InternalAuthenticate(string appUserId, SuccessCallback onSuccess, ErrorCallback onError)
        {
#if UNITY_EDITOR
            PoilinkEditorMock.Dispatch(() => PoilinkEditorMock.AuthenticateBehavior(appUserId), onSuccess, onError, PoilinkErrorCode.AuthenticationFailed);
#else
            _authSuccessCallback = onSuccess;
            _authErrorCallback = onError;

            var successWrapper = new SuccessCallbackNative(OnAuthenticateSuccess);
            var errorWrapper = new ErrorCallbackNative(OnAuthenticateError);

            Authenticate(appUserId, successWrapper, errorWrapper);
#endif
        }

        private static void _InternalUnauthenticate()
        {
#if UNITY_EDITOR
            PoilinkEditorMock.UnauthenticateBehavior?.Invoke();
#else
            _Unauthenticate();
#endif
        }

        private static void _InternalShowWebPortal(WebPortalOptions options)
        {
#if UNITY_EDITOR
            PoilinkEditorMock.ShowWebPortalBehavior(options);
#else
            _showWebPortalOnCloseCallback = options?.OnClose;
            _showWebPortalOnShownCallback = options?.OnShown;
            _showWebPortalOnErrorCallback = options?.OnError;
            _showWebPortalOnMissionChallengeCallback = options?.OnMissionChallenge;
            _showWebPortalOnRewardReceiveCallback = options?.OnRewardReceive;

            var nativeOptions = new WebPortalOptionsNative();

            nativeOptions.showMode = (int)(options?.ShowMode ?? ShowMode.Fullscreen);
            if (options?.ShowMode == ShowMode.Embedded && options.EmbeddedFrame != null)
            {
                nativeOptions.embeddedX = options.EmbeddedFrame.X;
                nativeOptions.embeddedY = options.EmbeddedFrame.Y;
                nativeOptions.embeddedWidth = options.EmbeddedFrame.Width;
                nativeOptions.embeddedHeight = options.EmbeddedFrame.Height;
            }

            nativeOptions.volume = -1f;
            if (options?.Volume != null)
            {
                nativeOptions.volume = Mathf.Clamp01(options.Volume.Value);
            }

            if (options?.OnClose != null)
            {
                _showWebPortalOnCloseNativeCallback = new SuccessCallbackNative(OnShowWebPortalClose);
                nativeOptions.onClose = Marshal.GetFunctionPointerForDelegate(_showWebPortalOnCloseNativeCallback);
            }
            else
            {
                _showWebPortalOnCloseNativeCallback = null;
                nativeOptions.onClose = IntPtr.Zero;
            }

            if (options?.OnError != null)
            {
                _showWebPortalOnErrorNativeCallback = new ErrorCallbackNative(OnShowWebPortalError);
                nativeOptions.onError = Marshal.GetFunctionPointerForDelegate(_showWebPortalOnErrorNativeCallback);
            }
            else
            {
                _showWebPortalOnErrorNativeCallback = null;
                nativeOptions.onError = IntPtr.Zero;
            }

            if (options?.OnShown != null)
            {
                _showWebPortalOnShownNativeCallback = new SuccessCallbackNative(OnShowWebPortalShown);
                nativeOptions.onShown = Marshal.GetFunctionPointerForDelegate(_showWebPortalOnShownNativeCallback);
            }
            else
            {
                _showWebPortalOnShownNativeCallback = null;
                nativeOptions.onShown = IntPtr.Zero;
            }

            if (options?.OnMissionChallenge != null)
            {
                _showWebPortalOnMissionChallengeNativeCallback = new MissionChallengeCallbackNative(OnShowWebPortalMissionChallenge);
                nativeOptions.onMissionChallenge = Marshal.GetFunctionPointerForDelegate(_showWebPortalOnMissionChallengeNativeCallback);
            }
            else
            {
                _showWebPortalOnMissionChallengeNativeCallback = null;
                nativeOptions.onMissionChallenge = IntPtr.Zero;
            }

            if (options?.OnRewardReceive != null)
            {
                _showWebPortalOnRewardReceiveNativeCallback = new RewardReceiveCallbackNative(OnShowWebPortalRewardReceive);
                nativeOptions.onRewardReceive = Marshal.GetFunctionPointerForDelegate(_showWebPortalOnRewardReceiveNativeCallback);
            }
            else
            {
                _showWebPortalOnRewardReceiveNativeCallback = null;
                nativeOptions.onRewardReceive = IntPtr.Zero;
            }

            _ShowWebPortal(in nativeOptions);
#endif
        }

        private static void _InternalCloseWebPortal()
        {
#if UNITY_EDITOR
            PoilinkEditorMock.CloseWebPortalBehavior();
#else
            _CloseWebPortal();
#endif
        }

        private static void _InternalPreloadWebPortal(WebPortalOptions options)
        {
#if UNITY_EDITOR
            PoilinkEditorMock.PreloadWebPortalBehavior?.Invoke(options);
#else
            var nativeOptions = new WebPortalOptionsNative();

            nativeOptions.showMode = (int)(options?.ShowMode ?? ShowMode.Fullscreen);
            if (options?.ShowMode == ShowMode.Embedded && options.EmbeddedFrame != null)
            {
                nativeOptions.embeddedX = options.EmbeddedFrame.X;
                nativeOptions.embeddedY = options.EmbeddedFrame.Y;
                nativeOptions.embeddedWidth = options.EmbeddedFrame.Width;
                nativeOptions.embeddedHeight = options.EmbeddedFrame.Height;
            }

            nativeOptions.volume = -1f;
            if (options?.Volume != null)
            {
                nativeOptions.volume = Mathf.Clamp01(options.Volume.Value);
            }

            _showWebPortalOnCloseCallback = options?.OnClose;
            _showWebPortalOnShownCallback = options?.OnShown;
            _showWebPortalOnErrorCallback = options?.OnError;
            _showWebPortalOnMissionChallengeCallback = options?.OnMissionChallenge;
            _showWebPortalOnRewardReceiveCallback = options?.OnRewardReceive;

            if (options?.OnClose != null)
            {
                _showWebPortalOnCloseNativeCallback = new SuccessCallbackNative(OnShowWebPortalClose);
                nativeOptions.onClose = Marshal.GetFunctionPointerForDelegate(_showWebPortalOnCloseNativeCallback);
            }
            else
            {
                _showWebPortalOnCloseNativeCallback = null;
                nativeOptions.onClose = IntPtr.Zero;
            }

            if (options?.OnShown != null)
            {
                _showWebPortalOnShownNativeCallback = new SuccessCallbackNative(OnShowWebPortalShown);
                nativeOptions.onShown = Marshal.GetFunctionPointerForDelegate(_showWebPortalOnShownNativeCallback);
            }
            else
            {
                _showWebPortalOnShownNativeCallback = null;
                nativeOptions.onShown = IntPtr.Zero;
            }

            if (options?.OnError != null)
            {
                _showWebPortalOnErrorNativeCallback = new ErrorCallbackNative(OnShowWebPortalError);
                nativeOptions.onError = Marshal.GetFunctionPointerForDelegate(_showWebPortalOnErrorNativeCallback);
            }
            else
            {
                _showWebPortalOnErrorNativeCallback = null;
                nativeOptions.onError = IntPtr.Zero;
            }

            if (options?.OnMissionChallenge != null)
            {
                _showWebPortalOnMissionChallengeNativeCallback = new MissionChallengeCallbackNative(OnShowWebPortalMissionChallenge);
                nativeOptions.onMissionChallenge = Marshal.GetFunctionPointerForDelegate(_showWebPortalOnMissionChallengeNativeCallback);
            }
            else
            {
                _showWebPortalOnMissionChallengeNativeCallback = null;
                nativeOptions.onMissionChallenge = IntPtr.Zero;
            }

            if (options?.OnRewardReceive != null)
            {
                _showWebPortalOnRewardReceiveNativeCallback = new RewardReceiveCallbackNative(OnShowWebPortalRewardReceive);
                nativeOptions.onRewardReceive = Marshal.GetFunctionPointerForDelegate(_showWebPortalOnRewardReceiveNativeCallback);
            }
            else
            {
                _showWebPortalOnRewardReceiveNativeCallback = null;
                nativeOptions.onRewardReceive = IntPtr.Zero;
            }

            _PreloadWebPortal(in nativeOptions);
#endif
        }

        private static void _InternalGetRefreshToken(RefreshTokenCallback onSuccess, ErrorCallback onError)
        {
#if UNITY_EDITOR
            PoilinkEditorMock.DispatchString(() => PoilinkEditorMock.GetRefreshTokenBehavior(), onSuccess, onError, PoilinkErrorCode.AuthenticationFailed);
#else
            _getRefreshTokenSuccessCallback = onSuccess;
            _getRefreshTokenErrorCallback = onError;

            var successWrapper = new RefreshTokenCallbackNative(OnGetRefreshTokenSuccess);
            var errorWrapper = new ErrorCallbackNative(OnGetRefreshTokenError);
            GetRefreshToken(successWrapper, errorWrapper);
#endif
        }

        private static void _InternalSetRefreshToken(string appUserId, string refreshToken, SuccessCallback onSuccess, ErrorCallback onError)
        {
#if UNITY_EDITOR
            PoilinkEditorMock.Dispatch(() => PoilinkEditorMock.SetRefreshTokenBehavior(appUserId, refreshToken), onSuccess, onError, PoilinkErrorCode.AuthenticationFailed);
#else
            _setRefreshTokenSuccessCallback = onSuccess;
            _setRefreshTokenErrorCallback = onError;

            var successWrapper = new SuccessCallbackNative(OnSetRefreshTokenSuccess);
            var errorWrapper = new ErrorCallbackNative(OnSetRefreshTokenError);

            SetRefreshToken(appUserId, refreshToken, successWrapper, errorWrapper);
#endif
        }

        private static void _InternalProgressMission(string missionCode, int amount, ProgressMissionMode mode, ErrorCallback onError)
        {
#if UNITY_EDITOR
            PoilinkEditorMock.Dispatch(() =>
            {
                if (amount <= 0)
                    throw new PoilinkException(PoilinkErrorCode.ValidationError, "amount must be greater than 0");
                return PoilinkEditorMock.ProgressMissionBehavior(missionCode, amount, mode);
            }, null, onError, PoilinkErrorCode.StorageError);
#else
            long handle = System.Threading.Interlocked.Increment(ref _nextProgressMissionHandle);
            lock (_progressMissionCallbacks)
            {
                _progressMissionCallbacks[handle] = onError;
            }

            var completeWrapper = new ProgressMissionCompleteCallbackNative(OnProgressMissionComplete);
            var errorWrapper = new ProgressMissionErrorCallbackNative(OnProgressMissionError);

            ProgressMission(missionCode, amount, (int)mode, handle, completeWrapper, errorWrapper);
#endif
        }

        private static void _InternalProgressMissionImmediate(string missionCode, int amount, ProgressMissionMode mode,
            Action<List<MissionData>> onSuccess, ErrorCallback onError, string idempotencyKey)
        {
#if UNITY_EDITOR
            PoilinkEditorMock.DispatchResult(() =>
            {
                if (amount <= 0)
                    throw new PoilinkException(PoilinkErrorCode.ValidationError, "amount must be greater than 0");
                return PoilinkEditorMock.ProgressMissionImmediateBehavior(missionCode, amount, mode, idempotencyKey);
            }, onSuccess, onError, PoilinkErrorCode.Network);
#else
            _progressMissionImmediateSuccessCallback = onSuccess;
            _progressMissionImmediateErrorCallback = onError;

            var successWrapper = new MissionProgressResultCallbackNative(OnProgressMissionImmediateSuccess);
            var errorWrapper = new ErrorCallbackNative(OnProgressMissionImmediateError);

            ProgressMissionImmediate(missionCode, amount, (int)mode, successWrapper, errorWrapper, idempotencyKey);
#endif
        }

        private static List<MissionData> _InternalGetMissionList(MissionListFilter filter)
        {
#if UNITY_EDITOR
            return PoilinkEditorMock.GetMissionListBehavior(filter);
#else
            IntPtr progressCodePtr = IntPtr.Zero;
            IntPtr filterPtr = IntPtr.Zero;
            try
            {
                if (filter != null)
                {
                    var native = new MissionListFilterNative
                    {
                        cycleType = (int)filter.CycleType,
                        rewardType = (int)filter.RewardType,
                        progressCode = IntPtr.Zero,
                    };
                    if (!string.IsNullOrEmpty(filter.ProgressCode))
                    {
                        progressCodePtr = AllocUtf8(filter.ProgressCode);
                        native.progressCode = progressCodePtr;
                    }
                    filterPtr = Marshal.AllocHGlobal(Marshal.SizeOf<MissionListFilterNative>());
                    Marshal.StructureToPtr(native, filterPtr, false);
                }

                MissionListNative result = _GetMissionList(filterPtr);

                var list = new List<MissionData>(result.count);
                if (result.items != IntPtr.Zero && result.count > 0)
                {
                    int stride = Marshal.SizeOf<MissionDataNative>();
                    for (int i = 0; i < result.count; i++)
                    {
                        IntPtr itemPtr = new IntPtr(result.items.ToInt64() + (long)i * stride);
                        var n = Marshal.PtrToStructure<MissionDataNative>(itemPtr);
                        list.Add(new MissionData
                        {
                            InAppMissionId = PtrToStringUtf8(n.inAppMissionId) ?? string.Empty,
                            ProgressCode = PtrToStringUtf8(n.progressCode) ?? string.Empty,
                            Title = PtrToStringUtf8(n.title) ?? string.Empty,
                            Details = PtrToStringUtf8(n.details) ?? string.Empty,
                            Point = n.point,
                            TargetProgress = n.targetProgress,
                            CurrentProgress = n.currentProgress,
                            HasAchievement = n.hasAchievement != 0,
                            IsClaimed = n.isClaimed != 0,
                            DisplayOrder = n.displayOrder,
                            CycleType = (CycleType)n.cycleType,
                            RewardType = (RewardType)n.rewardType,
                            RewardItemCode = PtrToStringUtf8(n.rewardItemCode) ?? string.Empty,
                            RewardItemQuantity = n.rewardItemQuantity,
                        });
                    }
                }
                return list;
            }
            finally
            {
                if (filterPtr != IntPtr.Zero) Marshal.FreeHGlobal(filterPtr);
                if (progressCodePtr != IntPtr.Zero) Marshal.FreeHGlobal(progressCodePtr);
            }
#endif
        }
    }
}
