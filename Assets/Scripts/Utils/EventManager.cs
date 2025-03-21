using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using UnityEngine;

namespace HM
{

    public enum EventType
    {
        None,
        OnCharacterDead,
        OnGameStart,
        OnGameFinish,
        OnGameOver,
        etc,
    }

    public interface IEventListner
    {
        void OnEvent(EventType eventType, Component sender, object param = null);
    }
    public class EventManager : Singleton<EventManager>
    {
        private Dictionary<EventType, List<IEventListner>> _eventListners = new Dictionary<EventType, List<IEventListner>>();
        
        public void AddEvent(EventType eventType, IEventListner listner)
        {
            if (!_eventListners.ContainsKey(eventType))
            {
                _eventListners[eventType] = new List<IEventListner>();
            }
            _eventListners[eventType].Add(listner);
        }

        public void InvokeEvent(EventType eventType, Component sender, object param = null)
        {
            if (_eventListners.TryGetValue(eventType, out var listners))
            {
                foreach (var listner in listners)
                {
                    listner?.OnEvent(eventType, sender, param);
                }
            }
        }

        public void RemoveEvent(EventType eventType, IEventListner listner)
        {
            if (_eventListners.TryGetValue(eventType, out var listners))
            {
                listners.Remove(listner);

                if (listners.Count == 0)
                {
                    _eventListners.Remove(eventType);
                }
            }
        }

        private void EnsureIntegrity()
        {
            foreach (var eventType in _eventListners.Keys.ToList())
            {
                var listners = _eventListners[eventType].Where(listner => listner != null).ToList();

                if (listners.Count > 0)
                {
                    _eventListners[eventType] = listners;
                }
                else
                {
                    _eventListners.Remove(eventType);
                }
            }
        }

    }
}