using UnityEngine;

namespace MRMatchBlaster.Core
{
    /// <summary>
    /// 游戏配置数据 - 使用 ScriptableObject 实现数据驱动设计
    /// 包含游戏规则、性能设置、调试选项等配置信息
    /// 优势：
    /// 1. 可在 Inspector 中直接编辑，无需重新编译
    /// 2. 支持版本控制，便于团队协作
    /// 3. 运行时只读，避免意外修改
    /// 4. 可创建多个配置文件用于不同难度或测试场景
    /// </summary>
    [CreateAssetMenu(fileName = "GameConfig", menuName = "MR MatchBlaster/Game Config", order = 1)]
    public class GameConfig : ScriptableObject
    {
        #region 游戏规则配置
        [Header("游戏规则配置")]
        
        /// <summary>
        /// 匹配所需的最小几何体数量
        /// </summary>
        [SerializeField, Range(3, 10)]
        [Tooltip("触发消除所需的最小匹配数量")]
        public int minMatchCount = 3;
        
        /// <summary>
        /// 最大发射次数限制（0 表示无限制）
        /// </summary>
        [SerializeField, Range(0, 100)]
        [Tooltip("最大发射次数限制，0 表示无限制")]
        public int maxShootCount = 30;
        
        /// <summary>
        /// 游戏时间限制（秒，0 表示无限制）
        /// </summary>
        [SerializeField, Range(0, 600)]
        [Tooltip("游戏时间限制（秒），0 表示无限制")]
        public float gameTimeLimit = 180f;
        
        /// <summary>
        /// 场景中几何体的最大数量
        /// </summary>
        [SerializeField, Range(50, 200)]
        [Tooltip("场景中几何体的最大数量，超过此数量游戏结束")]
        public int maxGeometryCount = 100;
        #endregion

        #region 得分配置
        [Header("得分配置")]
        
        /// <summary>
        /// 基础得分配置 - 根据匹配数量给分
        /// </summary>
        [SerializeField]
        [Tooltip("基础得分配置，索引对应匹配数量-3（如索引0对应3个匹配）")]
        public int[] baseScores = { 10, 20, 40, 80, 160 }; // 3,4,5,6,7个匹配对应的分数
        
        /// <summary>
        /// 连击倍率
        /// </summary>
        [SerializeField, Range(1f, 3f)]
        [Tooltip("连续匹配的分数倍率")]
        public float comboMultiplier = 1.5f;
        
        /// <summary>
        /// 时间奖励系数
        /// </summary>
        [SerializeField, Range(0f, 2f)]
        [Tooltip("剩余时间的奖励系数")]
        public float timeBonus = 1.0f;
        #endregion

        #region 性能配置
        [Header("性能配置")]
        
        /// <summary>
        /// 目标帧率
        /// </summary>
        [SerializeField, Range(60, 120)]
        [Tooltip("目标帧率，Quest 3 推荐 90 FPS")]
        public int targetFrameRate = 90;
        
        /// <summary>
        /// 最大粒子数量
        /// </summary>
        [SerializeField, Range(100, 1000)]
        [Tooltip("同时存在的最大粒子数量，用于控制特效性能")]
        public int maxParticleCount = 500;
        
        /// <summary>
        /// 启用动态分辨率缩放
        /// </summary>
        [SerializeField]
        [Tooltip("是否启用动态分辨率缩放以维持帧率")]
        public bool enableDynamicResolution = true;
        
        /// <summary>
        /// 启用固定注视点渲染
        /// </summary>
        [SerializeField]
        [Tooltip("是否启用固定注视点渲染以提升性能")]
        public bool enableFixedFoveatedRendering = true;
        #endregion

        #region 空间配置
        [Header("空间配置")]
        
        /// <summary>
        /// 游戏区域大小（米）
        /// </summary>
        [SerializeField]
        [Tooltip("游戏区域的大小（米），用于限制几何体生成范围")]
        public Vector3 gameAreaSize = new Vector3(2f, 2f, 2f);
        
        /// <summary>
        /// 几何体生成的最小距离（米）
        /// </summary>
        [SerializeField, Range(0.3f, 2f)]
        [Tooltip("几何体距离玩家的最小距离（米）")]
        public float minSpawnDistance = 0.5f;
        
        /// <summary>
        /// 几何体生成的最大距离（米）
        /// </summary>
        [SerializeField, Range(1f, 5f)]
        [Tooltip("几何体距离玩家的最大距离（米）")]
        public float maxSpawnDistance = 3f;
        
        /// <summary>
        /// 空间锚点稳定性阈值
        /// </summary>
        [SerializeField, Range(0.01f, 0.1f)]
        [Tooltip("空间锚点位置变化的稳定性阈值（米）")]
        public float anchorStabilityThreshold = 0.02f;
        #endregion

        #region 交互配置
        [Header("交互配置")]
        
        /// <summary>
        /// 发射力度
        /// </summary>
        [SerializeField, Range(1f, 20f)]
        [Tooltip("几何体发射的力度")]
        public float shootForce = 10f;
        
        /// <summary>
        /// 手势识别灵敏度
        /// </summary>
        [SerializeField, Range(0.1f, 2f)]
        [Tooltip("手势识别的灵敏度")]
        public float gestureSensitivity = 1f;
        
        /// <summary>
        /// 震动反馈强度
        /// </summary>
        [SerializeField, Range(0f, 1f)]
        [Tooltip("触觉反馈的强度")]
        public float hapticFeedbackIntensity = 0.7f;
        
        /// <summary>
        /// 启用手部追踪
        /// </summary>
        [SerializeField]
        [Tooltip("是否启用手部追踪（需要 Quest 3 支持）")]
        public bool enableHandTracking = true;
        #endregion

        #region 调试配置
        [Header("调试配置")]
        
        /// <summary>
        /// 启用调试模式
        /// </summary>
        [SerializeField]
        [Tooltip("是否启用调试模式，显示额外的调试信息")]
        public bool enableDebugMode = false;
        
        /// <summary>
        /// 显示性能统计
        /// </summary>
        [SerializeField]
        [Tooltip("是否显示实时性能统计信息")]
        public bool showPerformanceStats = false;
        
        /// <summary>
        /// 显示空间网格
        /// </summary>
        [SerializeField]
        [Tooltip("是否显示空间网格用于调试")]
        public bool showSpatialMesh = false;
        
        /// <summary>
        /// 启用详细日志
        /// </summary>
        [SerializeField]
        [Tooltip("是否启用详细的控制台日志输出")]
        public bool enableVerboseLogging = false;
        #endregion

        #region 验证方法
        /// <summary>
        /// 验证配置数据的有效性
        /// 在 Inspector 中修改值时自动调用
        /// </summary>
        private void OnValidate()
        {
            // 确保基础得分数组不为空
            if (baseScores == null || baseScores.Length == 0)
            {
                baseScores = new int[] { 10, 20, 40, 80, 160 };
            }
            
            // 确保游戏区域大小为正值
            gameAreaSize = new Vector3(
                Mathf.Max(0.5f, gameAreaSize.x),
                Mathf.Max(0.5f, gameAreaSize.y),
                Mathf.Max(0.5f, gameAreaSize.z)
            );
            
            // 确保最小距离小于最大距离
            if (minSpawnDistance >= maxSpawnDistance)
            {
                maxSpawnDistance = minSpawnDistance + 0.5f;
            }
        }
        
        /// <summary>
        /// 获取指定匹配数量的得分
        /// </summary>
        /// <param name="matchCount">匹配数量</param>
        /// <returns>对应的得分</returns>
        public int GetScoreForMatch(int matchCount)
        {
            int index = matchCount - 3; // 索引从3个匹配开始
            if (index >= 0 && index < baseScores.Length)
            {
                return baseScores[index];
            }
            
            // 超出范围时使用最高分数
            return baseScores.Length > 0 ? baseScores[baseScores.Length - 1] : 0;
        }
        
        /// <summary>
        /// 检查是否启用了性能优化功能
        /// </summary>
        /// <returns>是否启用性能优化</returns>
        public bool IsPerformanceOptimizationEnabled()
        {
            return enableDynamicResolution || enableFixedFoveatedRendering;
        }
        #endregion
    }
}