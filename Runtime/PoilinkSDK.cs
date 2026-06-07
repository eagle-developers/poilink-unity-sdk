using System;
using System.Collections.Generic;

namespace Poilink
{
    public static partial class PoilinkSDK
    {
        public delegate void SuccessCallback();

        public delegate void ErrorCallback(int errorCode, string message);

        public delegate void RefreshTokenCallback(string refreshToken);

        public delegate void MissionChallengeCallback(string missionId);

        public delegate void RewardReceiveCallback(string grantId, string itemCode, int quantity);

        public delegate void GrantsCallback(List<PendingItemGrant> grants);

        public delegate void SyncCompleteCallback(SyncItemGrantsResult result);

        public class PendingItemGrant
        {
            public string GrantId { get; set; }
            public string ItemCode { get; set; }
            public int Quantity { get; set; }
            public long GrantedAtUnixMs { get; set; }
        }

        public class SyncItemGrantsResult
        {
            public int TotalSynced { get; set; }
            public int TotalMarked { get; set; }
            public int PageCount { get; set; }
        }

        public enum ShowMode
        {
            Fullscreen = 0,
            Embedded = 1,
        }

        public enum CycleType
        {
            Unspecified = 0,
            Once = 1,
            Daily = 2,
            Weekly = 3,
            Monthly = 4,
        }

        public enum RewardType
        {
            Unspecified = 0,
            Point = 1,
            AppOwnedItem = 2,
        }

        public enum ProgressMissionMode
        {
            Increase = 0,
            AtLeast = 1,
        }

        public class EmbeddedFrame
        {
            public int X { get; set; }
            public int Y { get; set; }
            public int Width { get; set; }
            public int Height { get; set; }
        }

        public class MissionListFilter
        {
            public CycleType CycleType { get; set; } = CycleType.Unspecified;
            public RewardType RewardType { get; set; } = RewardType.Unspecified;
            public string ProgressCode { get; set; }
        }

        public class MissionData
        {
            internal MissionData() {}
            public string InAppMissionId { get; internal set; }
            public string ProgressCode { get; internal set; }
            public string Title { get; internal set; }
            public string Details { get; internal set; }
            public long Point { get; internal set; }
            public int TargetProgress { get; internal set; }
            public int CurrentProgress { get; internal set; }
            public bool HasAchievement { get; internal set; }
            public bool IsClaimed { get; internal set; }
            public int DisplayOrder { get; internal set; }
            public CycleType CycleType { get; internal set; }
            public RewardType RewardType { get; internal set; }
            public string RewardItemCode { get; internal set; }
            public int RewardItemQuantity { get; internal set; }
        }

        public class WebPortalOptions
        {
            public float? Volume { get; set; }

            public SuccessCallback OnClose { get; set; }

            public SuccessCallback OnShown { get; set; }

            public ErrorCallback OnError { get; set; }

            public MissionChallengeCallback OnMissionChallenge { get; set; }

            public RewardReceiveCallback OnRewardReceive { get; set; }

            public ShowMode ShowMode { get; set; } = ShowMode.Fullscreen;

            public EmbeddedFrame EmbeddedFrame { get; set; }
        }

        public static void Initialize(SuccessCallback onSuccess = null, ErrorCallback onError = null)
        {
            _InternalInitialize(onSuccess, onError);
        }

        public static void Authenticate(string appUserId, SuccessCallback onSuccess = null,
            ErrorCallback onError = null)
        {
            _InternalAuthenticate(appUserId, onSuccess, onError);
        }

        public static void Unauthenticate()
        {
            _InternalUnauthenticate();
        }

        public static void ShowWebPortal(WebPortalOptions options = null)
        {
            _InternalShowWebPortal(options);
        }

        public static void CloseWebPortal()
        {
            _InternalCloseWebPortal();
        }

        public static void PreloadWebPortal(WebPortalOptions options = null)
        {
            _InternalPreloadWebPortal(options);
        }

        public static void GetRefreshToken(RefreshTokenCallback onSuccess, ErrorCallback onError = null)
        {
            _InternalGetRefreshToken(onSuccess, onError);
        }

        public static void SetRefreshToken(string appUserId, string refreshToken, SuccessCallback onSuccess = null,
            ErrorCallback onError = null)
        {
            _InternalSetRefreshToken(appUserId, refreshToken, onSuccess, onError);
        }

        public static void ProgressMission(string missionCode, int amount, ProgressMissionMode mode,
            ErrorCallback onError = null)
        {
            _InternalProgressMission(missionCode, amount, mode, onError);
        }

        public static void ProgressMissionImmediate(string missionCode, int amount, ProgressMissionMode mode,
            Action<List<MissionData>> onSuccess = null, ErrorCallback onError = null, string idempotencyKey = null)
        {
            _InternalProgressMissionImmediate(missionCode, amount, mode, onSuccess, onError, idempotencyKey);
        }

        public static List<MissionData> GetMissionList(MissionListFilter filter = null)
        {
            return _InternalGetMissionList(filter);
        }

        public static void SyncItemGrants(GrantsCallback onGrants, SyncCompleteCallback onComplete = null, ErrorCallback onError = null)
        {
            _InternalSyncItemGrants(onGrants, onComplete, onError);
        }
    }
}
