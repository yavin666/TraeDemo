using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MRMatchBlaster.Core.Data
{
    /// <summary>
    /// 得分配置 ScriptableObject
    /// 包含游戏得分系统的所有配置参数
    /// </summary>
    [CreateAssetMenu(fileName = "ScoreConfig", menuName = "MR MatchBlaster/Score Config")]
    public class ScoreConfig : ScriptableObject
    {
        [Header("基础得分设置")]
        [Tooltip("3个匹配的基础得分")]
        public int match3Score = 10;
        
        [Tooltip("4个匹配的基础得分")]
        public int match4Score = 20;
        
        [Tooltip("5个及以上匹配的基础得分")]
        public int match5PlusScore = 40;
        
        [Header("连击设置")]
        [Tooltip("连击倍率")]
        public float comboMultiplier = 1.5f;
        
        [Tooltip("连击时间窗口（秒）")]
        public float comboTimeWindow = 3f;
        
        [Tooltip("最大连击倍率")]
        public float maxComboMultiplier = 5f;
        
        [Header("特殊得分")]
        [Tooltip("颜色匹配额外得分")]
        public int colorMatchBonus = 5;
        
        [Tooltip("形状匹配额外得分")]
        public int shapeMatchBonus = 5;
        
        [Tooltip("完美匹配额外得分（同时匹配颜色和形状）")]
        public int perfectMatchBonus = 20;
        
        [Header("时间奖励")]
        [Tooltip("快速匹配时间阈值（秒）")]
        public float quickMatchThreshold = 1f;
        
        [Tooltip("快速匹配奖励得分")]
        public int quickMatchBonus = 10;
        
        [Header("调试设置")]
        [Tooltip("启用得分调试日志")]
        public bool enableScoreDebugLogs = true;
        
        #region 得分计算方法
        
        /// <summary>
        /// 计算基础匹配得分
        /// </summary>
        /// <param name="matchCount">匹配数量</param>
        /// <returns>基础得分</returns>
        public int CalculateBaseScore(int matchCount)
        {
            switch (matchCount)
            {
                case 3:
                    return match3Score;
                case 4:
                    return match4Score;
                case int n when n >= 5:
                    return match5PlusScore + (n - 5) * 10; // 超过5个每个额外+10分
                default:
                    return 0;
            }
        }
        
        /// <summary>
        /// 计算匹配类型奖励
        /// </summary>
        /// <param name="matchType">匹配类型</param>
        /// <param name="hasColorMatch">是否有颜色匹配</param>
        /// <param name="hasShapeMatch">是否有形状匹配</param>
        /// <returns>类型奖励得分</returns>
        public int CalculateMatchTypeBonus(MatchType matchType, bool hasColorMatch = false, bool hasShapeMatch = false)
        {
            int bonus = 0;
            
            // 检查是否是完美匹配（同时匹配颜色和形状）
            if (hasColorMatch && hasShapeMatch)
            {
                bonus += perfectMatchBonus;
            }
            else
            {
                // 单一类型匹配奖励
                switch (matchType)
                {
                    case MatchType.Color:
                        bonus += colorMatchBonus;
                        break;
                    case MatchType.Shape:
                        bonus += shapeMatchBonus;
                        break;
                }
            }
            
            return bonus;
        }
        
        /// <summary>
        /// 计算连击倍率
        /// </summary>
        /// <param name="comboCount">当前连击数</param>
        /// <returns>连击倍率</returns>
        public float CalculateComboMultiplier(int comboCount)
        {
            if (comboCount <= 1)
            {
                return 1f;
            }
            
            float multiplier = 1f + (comboCount - 1) * (comboMultiplier - 1f);
            return Mathf.Min(multiplier, maxComboMultiplier);
        }
        
        /// <summary>
        /// 计算时间奖励
        /// </summary>
        /// <param name="matchTime">匹配用时</param>
        /// <returns>时间奖励得分</returns>
        public int CalculateTimeBonus(float matchTime)
        {
            if (matchTime <= quickMatchThreshold)
            {
                return quickMatchBonus;
            }
            
            return 0;
        }
        
        /// <summary>
        /// 计算总得分
        /// </summary>
        /// <param name="matchCount">匹配数量</param>
        /// <param name="matchType">匹配类型</param>
        /// <param name="comboCount">连击数</param>
        /// <param name="matchTime">匹配用时</param>
        /// <param name="hasColorMatch">是否有颜色匹配</param>
        /// <param name="hasShapeMatch">是否有形状匹配</param>
        /// <returns>总得分</returns>
        public int CalculateTotalScore(int matchCount, MatchType matchType, int comboCount = 1, 
            float matchTime = 0f, bool hasColorMatch = false, bool hasShapeMatch = false)
        {
            // 基础得分
            int baseScore = CalculateBaseScore(matchCount);
            
            // 类型奖励
            int typeBonus = CalculateMatchTypeBonus(matchType, hasColorMatch, hasShapeMatch);
            
            // 时间奖励
            int timeBonus = CalculateTimeBonus(matchTime);
            
            // 连击倍率
            float comboMultiplier = CalculateComboMultiplier(comboCount);
            
            // 计算总分
            int totalScore = Mathf.RoundToInt((baseScore + typeBonus + timeBonus) * comboMultiplier);
            
            if (enableScoreDebugLogs)
            {
                Debug.Log($"[ScoreConfig] 得分计算: 基础={baseScore}, 类型奖励={typeBonus}, 时间奖励={timeBonus}, 连击倍率={comboMultiplier:F2}, 总分={totalScore}");
            }
            
            return totalScore;
        }
        
        #endregion
        
        #region 验证方法
        
        /// <summary>
        /// 验证配置是否有效
        /// </summary>
        /// <returns>配置是否有效</returns>
        public bool ValidateConfig()
        {
            if (match3Score < 0 || match4Score < 0 || match5PlusScore < 0)
            {
                Debug.LogError("[ScoreConfig] 基础得分不能为负数");
                return false;
            }
            
            if (comboMultiplier < 1f)
            {
                Debug.LogError("[ScoreConfig] 连击倍率不能小于1");
                return false;
            }
            
            if (comboTimeWindow <= 0f)
            {
                Debug.LogError("[ScoreConfig] 连击时间窗口必须大于0");
                return false;
            }
            
            if (maxComboMultiplier < comboMultiplier)
            {
                Debug.LogError("[ScoreConfig] 最大连击倍率不能小于基础连击倍率");
                return false;
            }
            
            return true;
        }
        
        #endregion
        
        #region Unity编辑器方法
        
        private void OnValidate()
        {
            // 确保参数在合理范围内
            match3Score = Mathf.Max(0, match3Score);
            match4Score = Mathf.Max(0, match4Score);
            match5PlusScore = Mathf.Max(0, match5PlusScore);
            comboMultiplier = Mathf.Max(1f, comboMultiplier);
            comboTimeWindow = Mathf.Max(0.1f, comboTimeWindow);
            maxComboMultiplier = Mathf.Max(comboMultiplier, maxComboMultiplier);
            colorMatchBonus = Mathf.Max(0, colorMatchBonus);
            shapeMatchBonus = Mathf.Max(0, shapeMatchBonus);
            perfectMatchBonus = Mathf.Max(0, perfectMatchBonus);
            quickMatchThreshold = Mathf.Max(0.1f, quickMatchThreshold);
            quickMatchBonus = Mathf.Max(0, quickMatchBonus);
        }
        
        #endregion
    }
}