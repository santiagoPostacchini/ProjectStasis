using System;
using System.Collections.Generic;
using UnityEngine;

namespace Events
{
    public static class EventManager
    {
        // Inicializo aquí mismo la tabla de eventos
        private static Dictionary<string, Action<string, GameObject>> _events = new Dictionary<string, Action<string, GameObject>>();
    
        public static void Subscribe(string eventName, Action<string, GameObject> handler)
        {
            if (string.IsNullOrEmpty(eventName) || handler == null)
            {
                Debug.LogWarning($"[EventManager] Subscribe inválido: '{eventName}' / handler null");
                return;
            }

            if (!_events.TryAdd(eventName, handler))
                _events[eventName] += handler;
        }

        public static void Unsubscribe(string eventName, Action<string, GameObject> handler)
        {
            if (string.IsNullOrEmpty(eventName) || handler == null) return;
            if (!_events.ContainsKey(eventName)) return;

            _events[eventName] -= handler;
            if (_events[eventName] == null)
                _events.Remove(eventName);
        }

        public static void TriggerEvent(string eventName, GameObject sender)
        {
            if (string.IsNullOrEmpty(eventName)) return;

            if (_events.TryGetValue(eventName, out var handlers))
                handlers.Invoke(eventName, sender);
        }
    }
}