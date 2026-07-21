using System;
using System.Collections.Generic;
using UnityEngine;

namespace Poilink
{
    internal class UnityMainThreadDispatcher : MonoBehaviour
    {
        private static readonly Queue<Action> ExecutionQueue = new();
        private static UnityMainThreadDispatcher _instance;

        private void Update()
        {
            lock (ExecutionQueue)
            {
                while (ExecutionQueue.Count > 0)
                    try
                    {
                        ExecutionQueue.Dequeue().Invoke();
                    }
                    catch (Exception ex)
                    {
                        Debug.LogError($"[UnityMainThreadDispatcher] Error: {ex}");
                    }
            }
        }

        public static void Enqueue(Action action)
        {
            if (action == null)
                return;

            lock (ExecutionQueue)
            {
                ExecutionQueue.Enqueue(action);
            }
        }

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void Initialize()
        {
            if (_instance == null)
            {
                var go = new GameObject("PoilinkMainThreadDispatcher");
                _instance = go.AddComponent<UnityMainThreadDispatcher>();
                DontDestroyOnLoad(go);
            }
        }
    }
}
