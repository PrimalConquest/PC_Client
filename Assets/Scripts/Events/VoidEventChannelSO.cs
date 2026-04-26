using System;
using UnityEngine;

namespace PrimalConquest.Events
{
    [CreateAssetMenu(menuName = "Events/Void Event Channel", fileName = "VoidEventChannel")]
    public class VoidEventChannelSO : ScriptableObject
    {
        event Action _onRaised;

        public void Raise()               => _onRaised?.Invoke();
        public void Subscribe(Action cb)   => _onRaised += cb;
        public void Unsubscribe(Action cb) => _onRaised -= cb;
    }
}
