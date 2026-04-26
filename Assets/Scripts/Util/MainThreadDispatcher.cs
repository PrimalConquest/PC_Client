using System;
using System.Collections.Concurrent;
using System.Threading.Tasks;
using UnityEngine;

namespace PrimalConquest.Util
{
    // Routes async results back onto the Unity main thread.
    // Add a single instance of this MonoBehaviour to an always-alive GameObject.
    public class MainThreadDispatcher : MonoBehaviour
    {
        public static MainThreadDispatcher Instance { get; private set; }

        readonly ConcurrentQueue<Action> _queue = new();

        void Awake()
        {
            if (Instance != null && Instance != this) { Destroy(gameObject); return; }
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }

        void Update()
        {
            while (_queue.TryDequeue(out var action))
                action();
        }

        // Starts an async task on a background thread, then dispatches the result
        // (success or exception) back onto the main thread via the queue.
        public void Run<T>(
            Func<Task<T>> taskFactory,
            Action<T>     onSuccess,
            Action<Exception> onError = null)
        {
            Task.Run(async () =>
            {
                try
                {
                    var result = await taskFactory();
                    _queue.Enqueue(() => onSuccess(result));
                }
                catch (Exception ex)
                {
                    _queue.Enqueue(() => onError?.Invoke(ex));
                }
            });
        }
    }
}
