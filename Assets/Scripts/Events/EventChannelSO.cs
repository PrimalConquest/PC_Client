using System;
using UnityEngine;

namespace PrimalConquest.Events
{
    // Generic base — not directly instantiable.
    // Create typed subclasses and put [CreateAssetMenu] on those.
    public abstract class EventChannelSO<T> : ScriptableObject
    {
        event Action<T> _onRaised;

        public void Raise(T value)            => _onRaised?.Invoke(value);
        public void Subscribe(Action<T> cb)   => _onRaised += cb;
        public void Unsubscribe(Action<T> cb) => _onRaised -= cb;
    }
}
