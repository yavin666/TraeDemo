using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MRMatchBlaster.Core.Data
{
    /// <summary>
    /// 几何体类型枚举
    /// </summary>
    public enum ShapeType
    {
        /// <summary>
        /// 立方体
        /// </summary>
        Cube,
        
        /// <summary>
        /// 球体
        /// </summary>
        Sphere,
        
        /// <summary>
        /// 三角形（三角锥）
        /// </summary>
        Triangle
    }
    
    /// <summary>
    /// 几何体颜色枚举
    /// </summary>
    public enum ShapeColor
    {
        /// <summary>
        /// 红色
        /// </summary>
        Red,
        
        /// <summary>
        /// 绿色
        /// </summary>
        Green,
        
        /// <summary>
        /// 蓝色
        /// </summary>
        Blue
    }
    
    /// <summary>
    /// 匹配类型枚举
    /// </summary>
    public enum MatchType
    {
        /// <summary>
        /// 颜色匹配
        /// </summary>
        Color,
        
        /// <summary>
        /// 形状匹配
        /// </summary>
        Shape
    }
    
    /// <summary>
    /// 手部类型枚举
    /// </summary>
    public enum HandType
    {
        /// <summary>
        /// 左手
        /// </summary>
        Left,
        
        /// <summary>
        /// 右手
        /// </summary>
        Right
    }
    
    /// <summary>
    /// 手势类型枚举
    /// </summary>
    public enum GestureType
    {
        /// <summary>
        /// 抓取手势
        /// </summary>
        Grab,
        
        /// <summary>
        /// 释放手势
        /// </summary>
        Release
    }
    
    /// <summary>
    /// 几何体配置数据
    /// </summary>
    [System.Serializable]
    public class ShapeConfig
    {
        [Header("几何体属性")]
        public ShapeType shapeType;
        public ShapeColor shapeColor;
        
        [Header("生成设置")]
        [Range(0f, 1f)]
        public float spawnWeight = 1f;
        
        [Header("资源引用")]
        public GameObject prefab;
        public Material material;
        
        /// <summary>
        /// 获取颜色值
        /// </summary>
        /// <returns>对应的Unity Color</returns>
        public Color GetUnityColor()
        {
            switch (shapeColor)
            {
                case ShapeColor.Red:
                    return Color.red;
                case ShapeColor.Green:
                    return Color.green;
                case ShapeColor.Blue:
                    return Color.blue;
                default:
                    return Color.white;
            }
        }
        
        /// <summary>
        /// 检查配置是否有效
        /// </summary>
        /// <returns>配置是否完整</returns>
        public bool IsValid()
        {
            return prefab != null && material != null && spawnWeight > 0f;
        }
    }
}