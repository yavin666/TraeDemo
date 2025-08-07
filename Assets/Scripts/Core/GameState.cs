using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MRMatchBlaster.Core
{
    /// <summary>
    /// 游戏状态枚举，定义游戏的五个核心状态
    /// </summary>
    public enum GameState
    {
        /// <summary>
        /// 初始化状态 - 加载资源和RoomMesh数据
        /// </summary>
        Initializing,
        
        /// <summary>
        /// 准备状态 - 生成初始几何体并显示游戏UI
        /// </summary>
        Ready,
        
        /// <summary>
        /// 游戏进行状态 - 处理玩家输入和游戏逻辑
        /// </summary>
        Playing,
        
        /// <summary>
        /// 暂停状态 - 显示暂停菜单并停止游戏逻辑
        /// </summary>
        Paused,
        
        /// <summary>
        /// 游戏结束状态 - 显示结果并提供重启选项
        /// </summary>
        GameOver
    }
}