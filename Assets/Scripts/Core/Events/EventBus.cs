using System;
using System.Collections.Generic;
using UnityEngine;

namespace MRMatchBlaster.Core.Events
{
    /// <summary>
    /// 事件总线系统 - 用于模块间的松耦合通信
    /// 采用单例模式，提供全局事件订阅和发布功能
    /// </summary>
    public class EventBus : MonoBehaviour
    {
        private static EventBus _instance;
        public static EventBus Instance
        {
            get
            {
                if (_instance == null)
                {
                    GameObject eventBusObject = new GameObject("EventBus");
                    _instance = eventBusObject.AddComponent<EventBus>();
                    DontDestroyOnLoad(eventBusObject);
                }
                return _instance;
            }
        }

        // 存储所有事件订阅者的字典
        private Dictionary<Type, List<object>> _eventSubscribers = new Dictionary<Type, List<object>>();

        /// <summary>
        /// 订阅事件
        /// </summary>
        /// <typeparam name="T">事件类型</typeparam>
        /// <param name="handler">事件处理函数</param>
        public void Subscribe<T>(Action<T> handler)
        {
            Type eventType = typeof(T);
            
            if (!_eventSubscribers.ContainsKey(eventType))
            {
                _eventSubscribers[eventType] = new List<object>();
            }
            
            _eventSubscribers[eventType].Add(handler);
        }

        /// <summary>
        /// 取消订阅事件
        /// </summary>
        /// <typeparam name="T">事件类型</typeparam>
        /// <param name="handler">事件处理函数</param>
        public void Unsubscribe<T>(Action<T> handler)
        {
            Type eventType = typeof(T);
            
            if (_eventSubscribers.ContainsKey(eventType))
            {
                _eventSubscribers[eventType].Remove(handler);
                
                // 如果没有订阅者了，移除这个事件类型
                if (_eventSubscribers[eventType].Count == 0)
                {
                    _eventSubscribers.Remove(eventType);
                }
            }
        }

        /// <summary>
        /// 发布事件
        /// </summary>
        /// <typeparam name="T">事件类型</typeparam>
        /// <param name="eventData">事件数据</param>
        public void Publish<T>(T eventData)
        {
            Type eventType = typeof(T);
            
            if (_eventSubscribers.ContainsKey(eventType))
            {
                // 创建订阅者列表的副本，避免在遍历过程中修改列表
                List<object> subscribers = new List<object>(_eventSubscribers[eventType]);
                
                foreach (object subscriber in subscribers)
                {
                    if (subscriber is Action<T> handler)
                    {
                        try
                        {
                            handler.Invoke(eventData);
                        }
                        catch (Exception e)
                        {
                            Debug.LogError($"事件处理出错: {e.Message}");
                        }
                    }
                }
            }
        }

        /// <summary>
        /// 清除所有事件订阅
        /// </summary>
        public void ClearAllSubscriptions()
        {
            _eventSubscribers.Clear();
        }

        private void OnDestroy()
        {
            ClearAllSubscriptions();
        }
    }
}