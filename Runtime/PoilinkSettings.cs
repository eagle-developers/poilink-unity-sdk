using UnityEngine;

namespace Poilink
{
    [CreateAssetMenu(fileName = "PoilinkSettings", menuName = "Poilink/Settings")]
    public class PoilinkSettings : ScriptableObject
    {
        private static PoilinkSettings _instance;

        [Header("Credentials")] [Tooltip("Poilink CLIENT_ID (Required)")]
        public string clientId = "";

        [Tooltip("Poilink CLIENT_SECRET (Required)")]
        public string clientSecret = "";

        public static PoilinkSettings Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = Resources.Load<PoilinkSettings>("PoilinkSettings");
#if UNITY_EDITOR
                    if (_instance == null)
                        Debug.LogWarning(
                            "[PoilinkSDK] Settings asset not found. Please create one via Assets > Create > Poilink > Settings");
#endif
                }

                return _instance;
            }
        }
    }
}
