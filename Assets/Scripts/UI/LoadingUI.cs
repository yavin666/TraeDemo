using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using MRMatchBlaster.Core;
using MRMatchBlaster.Core.Events;

namespace MRMatchBlaster.UI
{
    /// <summary>
    /// 加载界面UI组件
    /// 专门处理游戏初始化阶段的加载显示
    /// </summary>
    public class LoadingUI : MonoBehaviour
    {
        [Header("UI组件引用")]
        [SerializeField, Tooltip("加载进度条")]
        private Slider progressBar;
        
        [SerializeField, Tooltip("进度百分比文本")]
        private Text progressText;
        
        [SerializeField, Tooltip("加载状态描述文本")]
        private Text statusText;
        
        [SerializeField, Tooltip("加载动画图标")]
        private Image loadingIcon;
        
        [SerializeField, Tooltip("背景图片")]
        private Image backgroundImage;
        
        [Header("动画设置")]
        [SerializeField, Tooltip("图标旋转速度（度/秒）")]
        private float iconRotationSpeed = 360f;
        
        [SerializeField, Tooltip("进度条填充动画速度")]
        private float progressAnimationSpeed = 2f;
        
        [SerializeField, Tooltip("文本淡入淡出速度")]
        private float textFadeSpeed = 2f;
        
        [Header("加载步骤配置")]
        [SerializeField, Tooltip("加载步骤描述")]
        private string[] loadingSteps = {
            "初始化游戏系统...",
            "配置XR环境...",
            "加载游戏资源...",
            "初始化输入系统...",
            "准备UI界面...",
            "启动游戏管理器...",
            "加载完成！"
        };
        
        [SerializeField, Tooltip("每个步骤的持续时间（秒）")]
        private float[] stepDurations = { 0.5f, 0.3f, 0.4f, 0.3f, 0.3f, 0.2f, 0.5f };
        
        // 私有变量
        private float currentProgress = 0f;
        private float targetProgress = 0f;
        private int currentStepIndex = 0;
        private bool isLoading = false;
        private Coroutine loadingCoroutine;
        private Coroutine iconAnimationCoroutine;
        
        #region Unity生命周期
        
        /// <summary>
        /// 初始化组件
        /// </summary>
        private void Awake()
        {
            InitializeUI();
        }
        
        /// <summary>
        /// 开始时自动启动加载
        /// </summary>
        private void Start()
        {
            StartLoading();
        }
        
        /// <summary>
        /// 订阅事件
        /// </summary>
        private void OnEnable()
        {
            EventBus.Instance.Subscribe<GameStateChangedEvent>(OnGameStateChanged);
        }
        
        /// <summary>
        /// 取消订阅事件
        /// </summary>
        private void OnDisable()
        {
            EventBus.Instance.Unsubscribe<GameStateChangedEvent>(OnGameStateChanged);
        }
        
        /// <summary>
        /// 更新动画
        /// </summary>
        private void Update()
        {
            if (isLoading)
            {
                UpdateProgressAnimation();
            }
        }
        
        #endregion
        
        #region 初始化方法
        
        /// <summary>
        /// 初始化UI组件
        /// </summary>
        private void InitializeUI()
        {
            // 验证步骤配置
            if (stepDurations.Length != loadingSteps.Length)
            {
                Debug.LogWarning("[LoadingUI] 加载步骤数量与持续时间数量不匹配，使用默认配置");
                stepDurations = new float[loadingSteps.Length];
                for (int i = 0; i < stepDurations.Length; i++)
                {
                    stepDurations[i] = 0.5f; // 默认每步0.5秒
                }
            }
            
            // 初始化UI状态
            SetProgress(0f);
            SetStatus(loadingSteps.Length > 0 ? loadingSteps[0] : "加载中...");
            
            // 启动图标动画
            StartIconAnimation();
        }
        
        #endregion
        
        #region 事件处理
        
        /// <summary>
        /// 处理游戏状态变化
        /// </summary>
        /// <param name="gameStateEvent">游戏状态事件</param>
        private void OnGameStateChanged(GameStateChangedEvent gameStateEvent)
        {
            if (gameStateEvent.NewState != GameState.Initializing)
            {
                StopLoading();
            }
        }
        
        #endregion
        
        #region 加载控制
        
        /// <summary>
        /// 开始加载流程
        /// </summary>
        public void StartLoading()
        {
            if (isLoading) return;
            
            isLoading = true;
            currentProgress = 0f;
            targetProgress = 0f;
            currentStepIndex = 0;
            
            if (loadingCoroutine != null)
            {
                StopCoroutine(loadingCoroutine);
            }
            
            loadingCoroutine = StartCoroutine(LoadingSequence());
            
            Debug.Log("[LoadingUI] 开始加载流程");
        }
        
        /// <summary>
        /// 停止加载流程
        /// </summary>
        public void StopLoading()
        {
            if (!isLoading) return;
            
            isLoading = false;
            
            if (loadingCoroutine != null)
            {
                StopCoroutine(loadingCoroutine);
                loadingCoroutine = null;
            }
            
            StopIconAnimation();
            
            Debug.Log("[LoadingUI] 停止加载流程");
        }
        
        /// <summary>
        /// 加载序列协程
        /// </summary>
        /// <returns>协程</returns>
        private IEnumerator LoadingSequence()
        {
            float totalDuration = 0f;
            foreach (float duration in stepDurations)
            {
                totalDuration += duration;
            }
            
            float elapsedTime = 0f;
            
            for (int i = 0; i < loadingSteps.Length && isLoading; i++)
            {
                currentStepIndex = i;
                SetStatus(loadingSteps[i]);
                
                float stepStartTime = elapsedTime;
                float stepEndTime = elapsedTime + stepDurations[i];
                
                // 执行当前步骤
                float stepElapsed = 0f;
                while (stepElapsed < stepDurations[i] && isLoading)
                {
                    stepElapsed += Time.deltaTime;
                    elapsedTime += Time.deltaTime;
                    
                    // 更新目标进度
                    targetProgress = elapsedTime / totalDuration;
                    
                    yield return null;
                }
                
                elapsedTime = stepEndTime; // 确保精确的时间
            }
            
            // 确保进度达到100%
            targetProgress = 1f;
            
            // 等待进度动画完成
            while (currentProgress < 0.99f && isLoading)
            {
                yield return null;
            }
            
            // 完成加载
            if (isLoading)
            {
                SetProgress(1f);
                SetStatus("加载完成！");
                
                // 等待一小段时间显示完成状态
                yield return new WaitForSeconds(0.5f);
                
                // 通知GameManager加载完成
                if (GameManager.Instance != null)
                {
                    GameManager.Instance.OnInitializationComplete();
                }
            }
        }
        
        #endregion
        
        #region UI更新方法
        
        /// <summary>
        /// 设置进度值
        /// </summary>
        /// <param name="progress">进度值（0-1）</param>
        public void SetProgress(float progress)
        {
            progress = Mathf.Clamp01(progress);
            currentProgress = progress;
            
            if (progressBar != null)
            {
                progressBar.value = progress;
            }
            
            if (progressText != null)
            {
                progressText.text = $"{Mathf.RoundToInt(progress * 100)}%";
            }
        }
        
        /// <summary>
        /// 设置状态文本
        /// </summary>
        /// <param name="status">状态描述</param>
        public void SetStatus(string status)
        {
            if (statusText != null)
            {
                statusText.text = status;
            }
        }
        
        /// <summary>
        /// 更新进度动画
        /// </summary>
        private void UpdateProgressAnimation()
        {
            if (Mathf.Abs(currentProgress - targetProgress) > 0.01f)
            {
                currentProgress = Mathf.MoveTowards(currentProgress, targetProgress, 
                    progressAnimationSpeed * Time.deltaTime);
                SetProgress(currentProgress);
            }
        }
        
        #endregion
        
        #region 动画控制
        
        /// <summary>
        /// 开始图标旋转动画
        /// </summary>
        private void StartIconAnimation()
        {
            if (loadingIcon == null) return;
            
            if (iconAnimationCoroutine != null)
            {
                StopCoroutine(iconAnimationCoroutine);
            }
            
            iconAnimationCoroutine = StartCoroutine(IconRotationAnimation());
        }
        
        /// <summary>
        /// 停止图标动画
        /// </summary>
        private void StopIconAnimation()
        {
            if (iconAnimationCoroutine != null)
            {
                StopCoroutine(iconAnimationCoroutine);
                iconAnimationCoroutine = null;
            }
        }
        
        /// <summary>
        /// 图标旋转动画协程
        /// </summary>
        /// <returns>协程</returns>
        private IEnumerator IconRotationAnimation()
        {
            while (loadingIcon != null)
            {
                loadingIcon.transform.Rotate(0, 0, iconRotationSpeed * Time.deltaTime);
                yield return null;
            }
        }
        
        #endregion
        
        #region 公共接口
        
        /// <summary>
        /// 手动设置加载进度
        /// </summary>
        /// <param name="progress">进度值（0-1）</param>
        /// <param name="status">状态描述</param>
        public void UpdateLoadingProgress(float progress, string status = "")
        {
            targetProgress = Mathf.Clamp01(progress);
            
            if (!string.IsNullOrEmpty(status))
            {
                SetStatus(status);
            }
        }
        
        /// <summary>
        /// 获取当前进度
        /// </summary>
        /// <returns>当前进度值（0-1）</returns>
        public float GetCurrentProgress()
        {
            return currentProgress;
        }
        
        /// <summary>
        /// 检查是否正在加载
        /// </summary>
        /// <returns>是否正在加载</returns>
        public bool IsLoading()
        {
            return isLoading;
        }
        
        /// <summary>
        /// 获取当前步骤索引
        /// </summary>
        /// <returns>当前步骤索引</returns>
        public int GetCurrentStepIndex()
        {
            return currentStepIndex;
        }
        
        /// <summary>
        /// 获取当前步骤描述
        /// </summary>
        /// <returns>当前步骤描述</returns>
        public string GetCurrentStepDescription()
        {
            if (currentStepIndex >= 0 && currentStepIndex < loadingSteps.Length)
            {
                return loadingSteps[currentStepIndex];
            }
            return "未知步骤";
        }
        
        #endregion
    }
}