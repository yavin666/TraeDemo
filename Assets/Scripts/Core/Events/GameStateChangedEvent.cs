using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MRMatchBlaster.Core.Events
{
    /// <summary>
    /// 游戏状态变更事件
    /// 当游戏状态发生改变时触发此事件
    /// </summary>
    public class GameStateChangedEvent
    {
        /// <summary>
        /// 之前的游戏状态
        /// </summary>
        public GameState PreviousState { get; set; }
        
        /// <summary>
        /// 新的游戏状态
        /// </summary>
        public GameState NewState { get; set; }
        
        /// <summary>
        /// 状态变更的时间戳
        /// </summary>
        public float StateChangeTime { get; set; }
        
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="previousState">之前的状态</param>
        /// <param name="newState">新状态</param>
        public GameStateChangedEvent(GameState previousState, GameState newState)
        {
            PreviousState = previousState;
            NewState = newState;
            StateChangeTime = Time.time;
        }
    }
}