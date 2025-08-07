using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MRMatchBlaster.Core.Data
{
    /// <summary>
    /// 游戏会话数据
    /// 存储当前游戏会话的所有状态信息
    /// </summary>
    [System.Serializable]
    public class GameSessionData
    {
        [Header("基础游戏信息")]
        [Tooltip("当前得分")]
        public int currentScore;
        
        [Tooltip("剩余时间（秒）")]
        public float remainingTime;
        
        [Tooltip("场景中几何体数量")]
        public int shapesInScene;
        
        [Tooltip("当前游戏状态")]
        public GameState currentState;
        
        [Header("连击信息")]
        [Tooltip("当前连击数")]
        public int comboCount;
        
        [Tooltip("上次匹配时间")]
        public float lastMatchTime;
        
        [Tooltip("连击开始时间")]
        public float comboStartTime;
        
        [Header("统计信息")]
        [Tooltip("总匹配次数")]
        public int totalMatches;
        
        [Tooltip("总发射次数")]
        public int totalShots;
        
        [Tooltip("最高连击数")]
        public int maxCombo;
        
        [Tooltip("游戏开始时间")]
        public float gameStartTime;
        
        [Header("匹配统计")]
        [Tooltip("3个匹配次数")]
        public int match3Count;
        
        [Tooltip("4个匹配次数")]
        public int match4Count;
        
        [Tooltip("5个及以上匹配次数")]
        public int match5PlusCount;
        
        [Tooltip("颜色匹配次数")]
        public int colorMatchCount;
        
        [Tooltip("形状匹配次数")]
        public int shapeMatchCount;
        
        [Tooltip("完美匹配次数")]
        public int perfectMatchCount;
        
        #region 初始化方法
        
        /// <summary>
        /// 使用游戏配置初始化数据
        /// </summary>
        /// <param name="config">游戏配置</param>
        public void Initialize(GameConfig config)
        {
            if (config == null)
            {
                Debug.LogError("[GameSessionData] GameConfig为空，无法初始化");
                return;
            }
            
            // 重置基础信息
            currentScore = 0;
            remainingTime = config.gameTimeInSeconds;
            shapesInScene = 0;
            currentState = GameState.Initializing;
            
            // 重置连击信息
            comboCount = 0;
            lastMatchTime = 0f;
            comboStartTime = 0f;
            
            // 重置统计信息
            totalMatches = 0;
            totalShots = 0;
            maxCombo = 0;
            gameStartTime = Time.time;
            
            // 重置匹配统计
            match3Count = 0;
            match4Count = 0;
            match5PlusCount = 0;
            colorMatchCount = 0;
            shapeMatchCount = 0;
            perfectMatchCount = 0;
            
            Debug.Log("[GameSessionData] 游戏会话数据初始化完成");
        }
        
        /// <summary>
        /// 重置为默认值
        /// </summary>
        public void Reset()
        {
            currentScore = 0;
            remainingTime = 180f; // 默认3分钟
            shapesInScene = 0;
            currentState = GameState.Initializing;
            comboCount = 0;
            lastMatchTime = 0f;
            comboStartTime = 0f;
            totalMatches = 0;
            totalShots = 0;
            maxCombo = 0;
            gameStartTime = Time.time;
            match3Count = 0;
            match4Count = 0;
            match5PlusCount = 0;
            colorMatchCount = 0;
            shapeMatchCount = 0;
            perfectMatchCount = 0;
        }
        
        #endregion
        
        #region 得分管理
        
        /// <summary>
        /// 增加得分
        /// </summary>
        /// <param name="score">要增加的分数</param>
        public void AddScore(int score)
        {
            currentScore += score;
            currentScore = Mathf.Max(0, currentScore); // 确保得分不为负
        }
        
        /// <summary>
        /// 设置得分
        /// </summary>
        /// <param name="score">新的分数</param>
        public void SetScore(int score)
        {
            currentScore = Mathf.Max(0, score);
        }
        
        #endregion
        
        #region 时间管理
        
        /// <summary>
        /// 更新剩余时间
        /// </summary>
        /// <param name="deltaTime">时间增量</param>
        /// <returns>是否还有剩余时间</returns>
        public bool UpdateTime(float deltaTime)
        {
            remainingTime -= deltaTime;
            remainingTime = Mathf.Max(0f, remainingTime);
            return remainingTime > 0f;
        }
        
        /// <summary>
        /// 检查时间是否耗尽
        /// </summary>
        /// <returns>时间是否耗尽</returns>
        public bool IsTimeUp()
        {
            return remainingTime <= 0f;
        }
        
        /// <summary>
        /// 获取游戏进行时间
        /// </summary>
        /// <returns>游戏进行的总时间</returns>
        public float GetElapsedTime()
        {
            return Time.time - gameStartTime;
        }
        
        #endregion
        
        #region 连击管理
        
        /// <summary>
        /// 增加连击
        /// </summary>
        /// <param name="scoreConfig">得分配置</param>
        public void AddCombo(ScoreConfig scoreConfig)
        {
            float currentTime = Time.time;
            
            // 检查是否在连击时间窗口内
            if (comboCount == 0 || (currentTime - lastMatchTime) <= scoreConfig.comboTimeWindow)
            {
                if (comboCount == 0)
                {
                    comboStartTime = currentTime;
                }
                
                comboCount++;
                maxCombo = Mathf.Max(maxCombo, comboCount);
            }
            else
            {
                // 连击中断，重新开始
                comboCount = 1;
                comboStartTime = currentTime;
            }
            
            lastMatchTime = currentTime;
        }
        
        /// <summary>
        /// 重置连击
        /// </summary>
        public void ResetCombo()
        {
            comboCount = 0;
            lastMatchTime = 0f;
            comboStartTime = 0f;
        }
        
        /// <summary>
        /// 检查连击是否有效
        /// </summary>
        /// <param name="scoreConfig">得分配置</param>
        /// <returns>连击是否仍然有效</returns>
        public bool IsComboActive(ScoreConfig scoreConfig)
        {
            if (comboCount <= 0) return false;
            
            float timeSinceLastMatch = Time.time - lastMatchTime;
            return timeSinceLastMatch <= scoreConfig.comboTimeWindow;
        }
        
        #endregion
        
        #region 统计管理
        
        /// <summary>
        /// 记录匹配
        /// </summary>
        /// <param name="matchCount">匹配数量</param>
        /// <param name="matchType">匹配类型</param>
        /// <param name="hasColorMatch">是否有颜色匹配</param>
        /// <param name="hasShapeMatch">是否有形状匹配</param>
        public void RecordMatch(int matchCount, MatchType matchType, bool hasColorMatch = false, bool hasShapeMatch = false)
        {
            totalMatches++;
            
            // 记录匹配数量统计
            switch (matchCount)
            {
                case 3:
                    match3Count++;
                    break;
                case 4:
                    match4Count++;
                    break;
                case int n when n >= 5:
                    match5PlusCount++;
                    break;
            }
            
            // 记录匹配类型统计
            if (hasColorMatch && hasShapeMatch)
            {
                perfectMatchCount++;
            }
            else
            {
                switch (matchType)
                {
                    case MatchType.Color:
                        colorMatchCount++;
                        break;
                    case MatchType.Shape:
                        shapeMatchCount++;
                        break;
                }
            }
        }
        
        /// <summary>
        /// 记录发射
        /// </summary>
        public void RecordShot()
        {
            totalShots++;
        }
        
        /// <summary>
        /// 获取命中率
        /// </summary>
        /// <returns>命中率（0-1）</returns>
        public float GetAccuracy()
        {
            if (totalShots == 0) return 0f;
            return (float)totalMatches / totalShots;
        }
        
        #endregion
        
        #region 调试方法
        
        /// <summary>
        /// 获取调试信息字符串
        /// </summary>
        /// <returns>格式化的调试信息</returns>
        public string GetDebugInfo()
        {
            return $"得分: {currentScore}, 剩余时间: {remainingTime:F1}s, 连击: {comboCount}, 几何体: {shapesInScene}, 状态: {currentState}";
        }
        
        /// <summary>
        /// 获取详细统计信息
        /// </summary>
        /// <returns>详细统计信息字符串</returns>
        public string GetDetailedStats()
        {
            return $"总匹配: {totalMatches}, 总发射: {totalShots}, 命中率: {GetAccuracy():P1}, 最高连击: {maxCombo}, 游戏时长: {GetElapsedTime():F1}s";
        }
        
        #endregion
    }
}