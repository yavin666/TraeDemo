using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MRMatchBlaster.Core.Data
{
    /// <summary>
    /// 游戏配置 ScriptableObject
    /// 包含游戏的核心配置参数，支持数据驱动的游戏设计
    /// </summary>
    [CreateAssetMenu(fileName = "GameConfig", menuName = "MR MatchBlaster/Game Config")]
    public class GameConfig : ScriptableObject
    {
        [Header("游戏时间设置")]
        [Tooltip("游戏总时长（秒）")]
        public float gameTimeInSeconds = 180f;
        
        [Header("几何体生成设置")]
        [Tooltip("场景中最大几何体数量")]
        public int maxShapesInScene = 50;
        
        [Tooltip("几何体生成间隔（秒）")]
        public float spawnInterval = 2f;
        
        [Tooltip("几何体生成区域大小")]
        public Vector3 spawnAreaSize = new Vector3(4f, 3f, 4f);
        
        [Tooltip("几何体生成区域中心偏移")]
        public Vector3 spawnAreaOffset = Vector3.zero;
        
        [Header("几何体物理设置")]
        [Tooltip("几何体默认大小")]
        public float defaultShapeSize = 0.2f;
        
        [Tooltip("几何体发射速度")]
        public float shootForce = 10f;
        
        [Tooltip("几何体重力缩放")]
        public float gravityScale = 1f;
        
        [Header("几何体配置")]
        [Tooltip("可用的几何体配置列表")]
        public ShapeConfig[] availableShapes;
        
        [Header("匹配设置")]
        [Tooltip("触发三消的最小匹配数量")]
        public int minMatchCount = 3;
        
        [Tooltip("匹配检测范围")]
        public float matchDetectionRadius = 1f;
        
        [Header("调试设置")]
        [Tooltip("启用调试日志")]
        public bool enableDebugLogs = true;
        
        [Tooltip("显示生成区域Gizmos")]
        public bool showSpawnAreaGizmos = true;
        
        #region 验证方法
        
        /// <summary>
        /// 验证配置是否有效
        /// </summary>
        /// <returns>配置是否完整有效</returns>
        public bool ValidateConfig()
        {
            // 检查基础参数
            if (gameTimeInSeconds <= 0f)
            {
                Debug.LogError("[GameConfig] 游戏时间必须大于0");
                return false;
            }
            
            if (maxShapesInScene <= 0)
            {
                Debug.LogError("[GameConfig] 最大几何体数量必须大于0");
                return false;
            }
            
            if (spawnInterval <= 0f)
            {
                Debug.LogError("[GameConfig] 生成间隔必须大于0");
                return false;
            }
            
            // 检查几何体配置
            if (availableShapes == null || availableShapes.Length == 0)
            {
                Debug.LogError("[GameConfig] 必须配置至少一个几何体");
                return false;
            }
            
            // 检查每个几何体配置
            for (int i = 0; i < availableShapes.Length; i++)
            {
                if (availableShapes[i] == null)
                {
                    Debug.LogError($"[GameConfig] 几何体配置 {i} 为空");
                    return false;
                }
                
                if (!availableShapes[i].IsValid())
                {
                    Debug.LogError($"[GameConfig] 几何体配置 {i} 无效");
                    return false;
                }
            }
            
            return true;
        }
        
        /// <summary>
        /// 获取随机几何体配置
        /// </summary>
        /// <returns>随机选择的几何体配置</returns>
        public ShapeConfig GetRandomShapeConfig()
        {
            if (availableShapes == null || availableShapes.Length == 0)
            {
                Debug.LogError("[GameConfig] 没有可用的几何体配置");
                return null;
            }
            
            // 计算总权重
            float totalWeight = 0f;
            foreach (var shape in availableShapes)
            {
                if (shape != null && shape.IsValid())
                {
                    totalWeight += shape.spawnWeight;
                }
            }
            
            if (totalWeight <= 0f)
            {
                // 如果没有权重，随机选择一个
                return availableShapes[Random.Range(0, availableShapes.Length)];
            }
            
            // 基于权重随机选择
            float randomValue = Random.Range(0f, totalWeight);
            float currentWeight = 0f;
            
            foreach (var shape in availableShapes)
            {
                if (shape != null && shape.IsValid())
                {
                    currentWeight += shape.spawnWeight;
                    if (randomValue <= currentWeight)
                    {
                        return shape;
                    }
                }
            }
            
            // 备用方案：返回第一个有效配置
            foreach (var shape in availableShapes)
            {
                if (shape != null && shape.IsValid())
                {
                    return shape;
                }
            }
            
            return null;
        }
        
        /// <summary>
        /// 获取指定类型和颜色的几何体配置
        /// </summary>
        /// <param name="shapeType">几何体类型</param>
        /// <param name="shapeColor">几何体颜色</param>
        /// <returns>匹配的几何体配置</returns>
        public ShapeConfig GetShapeConfig(ShapeType shapeType, ShapeColor shapeColor)
        {
            if (availableShapes == null) return null;
            
            foreach (var shape in availableShapes)
            {
                if (shape != null && shape.shapeType == shapeType && shape.shapeColor == shapeColor)
                {
                    return shape;
                }
            }
            
            return null;
        }
        
        /// <summary>
        /// 获取生成区域的边界
        /// </summary>
        /// <returns>生成区域的Bounds</returns>
        public Bounds GetSpawnAreaBounds()
        {
            return new Bounds(spawnAreaOffset, spawnAreaSize);
        }
        
        #endregion
        
        #region Unity编辑器方法
        
        private void OnValidate()
        {
            // 确保参数在合理范围内
            gameTimeInSeconds = Mathf.Max(1f, gameTimeInSeconds);
            maxShapesInScene = Mathf.Max(1, maxShapesInScene);
            spawnInterval = Mathf.Max(0.1f, spawnInterval);
            defaultShapeSize = Mathf.Max(0.01f, defaultShapeSize);
            shootForce = Mathf.Max(0.1f, shootForce);
            minMatchCount = Mathf.Max(2, minMatchCount);
            matchDetectionRadius = Mathf.Max(0.1f, matchDetectionRadius);
        }
        
        #endregion
    }
}