using System;
using System.Threading.Tasks;
using UnityEngine;

namespace MRMatchBlaster.Core
{
    /// <summary>
    /// 空间管理系统接口
    /// 负责 MR 空间相关的功能：空间锚点、房间边界、几何体生成等
    /// 使用接口实现依赖注入，便于测试和替换不同的空间管理实现
    /// </summary>
    public interface ISpatialManager
    {
        /// <summary>
        /// 空间校准完成事件
        /// </summary>
        event Action OnSpatialCalibrationCompleted;
        
        /// <summary>
        /// 空间锚点丢失事件
        /// </summary>
        event Action OnSpatialAnchorLost;
        
        /// <summary>
        /// 异步初始化空间管理系统
        /// </summary>
        /// <returns>初始化任务</returns>
        Task InitializeAsync();
        
        /// <summary>
        /// 开始空间校准流程
        /// </summary>
        void StartCalibration();
        
        /// <summary>
        /// 生成初始几何体布局
        /// </summary>
        void SpawnInitialGeometry();
        
        /// <summary>
        /// 检查位置是否在有效游戏区域内
        /// </summary>
        /// <param name="position">世界坐标位置</param>
        /// <returns>是否在有效区域内</returns>
        bool IsPositionInGameArea(Vector3 position);
        
        /// <summary>
        /// 获取随机的有效生成位置
        /// </summary>
        /// <returns>有效的生成位置</returns>
        Vector3 GetRandomSpawnPosition();
    }

    /// <summary>
    /// 交互管理系统接口
    /// 负责 XR 交互相关功能：手部追踪、发射逻辑、手势识别等
    /// </summary>
    public interface IInteractionManager
    {
        /// <summary>
        /// 发射事件 - 当玩家发射几何体时触发
        /// </summary>
        event Action<Vector3, Vector3> OnShoot; // 参数：发射位置、发射方向
        
        /// <summary>
        /// 手势识别事件 - 当识别到特定手势时触发
        /// </summary>
        event Action<string> OnGestureRecognized; // 参数：手势名称
        
        /// <summary>
        /// 异步初始化交互管理系统
        /// </summary>
        /// <returns>初始化任务</returns>
        Task InitializeAsync();
        
        /// <summary>
        /// 启用交互功能
        /// </summary>
        void EnableInteraction();
        
        /// <summary>
        /// 禁用交互功能
        /// </summary>
        void DisableInteraction();
        
        /// <summary>
        /// 获取当前手中的几何体类型
        /// </summary>
        /// <returns>几何体类型</returns>
        GeometryType GetCurrentGeometryType();
        
        /// <summary>
        /// 切换手中的几何体类型
        /// </summary>
        void SwitchGeometryType();
        
        /// <summary>
        /// 触发触觉反馈
        /// </summary>
        /// <param name="intensity">反馈强度 (0-1)</param>
        /// <param name="duration">持续时间 (秒)</param>
        void TriggerHapticFeedback(float intensity, float duration);
    }

    /// <summary>
    /// 匹配系统接口
    /// 负责几何体匹配逻辑：匹配检测、消除处理、连击计算等
    /// </summary>
    public interface IMatchSystem
    {
        /// <summary>
        /// 匹配成功事件 - 当检测到有效匹配时触发
        /// </summary>
        event Action<MatchResult> OnMatchFound; // 参数：匹配结果
        
        /// <summary>
        /// 几何体消除事件 - 当几何体被消除时触发
        /// </summary>
        event Action<GeometryObject[]> OnGeometryDestroyed; // 参数：被消除的几何体数组
        
        /// <summary>
        /// 异步初始化匹配系统
        /// </summary>
        /// <returns>初始化任务</returns>
        Task InitializeAsync();
        
        /// <summary>
        /// 重置游戏状态
        /// </summary>
        void ResetGame();
        
        /// <summary>
        /// 检查指定位置的匹配情况
        /// </summary>
        /// <param name="position">检查位置</param>
        /// <param name="geometryType">几何体类型</param>
        /// <returns>匹配结果</returns>
        MatchResult CheckMatch(Vector3 position, GeometryType geometryType);
        
        /// <summary>
        /// 处理几何体碰撞
        /// </summary>
        /// <param name="newGeometry">新的几何体</param>
        /// <param name="hitPosition">碰撞位置</param>
        void ProcessGeometryCollision(GeometryObject newGeometry, Vector3 hitPosition);
        
        /// <summary>
        /// 获取当前连击数
        /// </summary>
        /// <returns>连击数</returns>
        int GetCurrentCombo();
    }

    /// <summary>
    /// 得分系统接口
    /// 负责分数计算、记录、排行榜等功能
    /// </summary>
    public interface IScoreSystem
    {
        /// <summary>
        /// 分数改变事件 - 当分数发生变化时触发
        /// </summary>
        event Action<int> OnScoreChanged; // 参数：新的分数
        
        /// <summary>
        /// 连击改变事件 - 当连击数发生变化时触发
        /// </summary>
        event Action<int> OnComboChanged; // 参数：新的连击数
        
        /// <summary>
        /// 异步初始化得分系统
        /// </summary>
        /// <returns>初始化任务</returns>
        Task InitializeAsync();
        
        /// <summary>
        /// 重置分数
        /// </summary>
        void ResetScore();
        
        /// <summary>
        /// 开始计时
        /// </summary>
        void StartTimer();
        
        /// <summary>
        /// 暂停计时
        /// </summary>
        void PauseTimer();
        
        /// <summary>
        /// 添加分数
        /// </summary>
        /// <param name="points">分数</param>
        /// <param name="comboMultiplier">连击倍率</param>
        void AddScore(int points, float comboMultiplier = 1f);
        
        /// <summary>
        /// 获取当前分数
        /// </summary>
        /// <returns>当前分数</returns>
        int GetCurrentScore();
        
        /// <summary>
        /// 获取最终分数（包含时间奖励）
        /// </summary>
        /// <returns>最终分数</returns>
        int GetFinalScore();
        
        /// <summary>
        /// 获取剩余时间
        /// </summary>
        /// <returns>剩余时间（秒）</returns>
        float GetRemainingTime();
    }

    /// <summary>
    /// 几何体类型枚举
    /// 定义游戏中可用的几何体形状
    /// </summary>
    public enum GeometryType
    {
        /// <summary>立方体</summary>
        Cube,
        /// <summary>球体</summary>
        Sphere,
        /// <summary>三角形</summary>
        Triangle
    }

    /// <summary>
    /// 几何体颜色枚举
    /// 定义游戏中可用的几何体颜色
    /// </summary>
    public enum GeometryColor
    {
        /// <summary>红色</summary>
        Red,
        /// <summary>绿色</summary>
        Green,
        /// <summary>蓝色</summary>
        Blue
    }

    /// <summary>
    /// 匹配结果数据结构
    /// 包含匹配检测的详细信息
    /// </summary>
    [Serializable]
    public struct MatchResult
    {
        /// <summary>是否找到匹配</summary>
        public bool hasMatch;
        
        /// <summary>匹配的几何体数量</summary>
        public int matchCount;
        
        /// <summary>匹配的几何体列表</summary>
        public GeometryObject[] matchedGeometry;
        
        /// <summary>匹配类型（形状匹配或颜色匹配）</summary>
        public MatchType matchType;
        
        /// <summary>是否为连击</summary>
        public bool isCombo;
        
        /// <summary>基础得分</summary>
        public int baseScore;
    }

    /// <summary>
    /// 匹配类型枚举
    /// 定义不同的匹配规则
    /// </summary>
    public enum MatchType
    {
        /// <summary>无匹配</summary>
        None,
        /// <summary>形状匹配</summary>
        Shape,
        /// <summary>颜色匹配</summary>
        Color,
        /// <summary>形状和颜色都匹配</summary>
        Both
    }

    /// <summary>
    /// 几何体对象数据结构
    /// 表示游戏中的一个几何体实例
    /// </summary>
    [Serializable]
    public class GeometryObject
    {
        /// <summary>几何体的 GameObject 引用</summary>
        public GameObject gameObject;
        
        /// <summary>几何体类型</summary>
        public GeometryType type;
        
        /// <summary>几何体颜色</summary>
        public GeometryColor color;
        
        /// <summary>世界坐标位置</summary>
        public Vector3 position;
        
        /// <summary>是否为玩家发射的几何体</summary>
        public bool isPlayerShot;
        
        /// <summary>创建时间戳</summary>
        public float creationTime;
        
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="go">GameObject 引用</param>
        /// <param name="geometryType">几何体类型</param>
        /// <param name="geometryColor">几何体颜色</param>
        public GeometryObject(GameObject go, GeometryType geometryType, GeometryColor geometryColor)
        {
            gameObject = go;
            type = geometryType;
            color = geometryColor;
            position = go.transform.position;
            isPlayerShot = false;
            creationTime = Time.time;
        }
        
        /// <summary>
        /// 检查是否与另一个几何体匹配
        /// </summary>
        /// <param name="other">另一个几何体</param>
        /// <returns>匹配类型</returns>
        public MatchType GetMatchType(GeometryObject other)
        {
            bool shapeMatch = type == other.type;
            bool colorMatch = color == other.color;
            
            if (shapeMatch && colorMatch)
                return MatchType.Both;
            else if (shapeMatch)
                return MatchType.Shape;
            else if (colorMatch)
                return MatchType.Color;
            else
                return MatchType.None;
        }
    }
}