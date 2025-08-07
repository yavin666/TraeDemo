using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using MRMatchBlaster.Core;
using MRMatchBlaster.Core.Events;

namespace MRMatchBlaster.UI
{
    /// <summary>
    /// UI管理器
    /// 负责管理游戏中所有UI界面的显示、隐藏和状态切换
    /// </summary>
    public class UIManager : MonoBehaviour
    {
        [Header("UI面板引用")]
        [SerializeField, Tooltip("加载界面")]
        private GameObject loadingPanel;
        
        [SerializeField, Tooltip("主菜单界面")]
        private GameObject mainMenuPanel;
        
        [SerializeField, Tooltip("游戏界面")]
        private GameObject gamePanel;
        
        [SerializeField, Tooltip("暂停界面")]
        private GameObject pausePanel;
        
        [SerializeField, Tooltip("游戏结束界面")]
        private GameObject gameOverPanel;
        
        [Header("加载界面组件")]
        [SerializeField, Tooltip("加载进度条")]
        private Slider loadingProgressBar;
        
        [SerializeField, Tooltip("加载状态文本")]
        private Text loadingStatusText;
        
        [Header("游戏界面组件")]
        [SerializeField, Tooltip("得分显示")]
        private Text scoreText;
        
        [SerializeField, Tooltip("时间显示")]
        private Text timeText;
        
        [SerializeField, Tooltip("连击显示")]
        private Text comboText;
        
        [SerializeField, Tooltip("连击面板")]
        private GameObject comboPanel;
        
        [Header("游戏结束界面组件")]
        [SerializeField, Tooltip("最终得分显示")]
        private Text finalScoreText;
        
        [SerializeField, Tooltip("统计信息显示")]
        private Text statsText;
        
        [Header("按钮引用")]
        [SerializeField, Tooltip("开始游戏按钮")]
        private Button startGameButton;
        
        [SerializeField, Tooltip("暂停按钮")]
        private Button pauseButton;
        
        [SerializeField, Tooltip("继续游戏按钮")]
        private Button resumeButton;
        
        [SerializeField, Tooltip("重新开始按钮")]
        private Button restartButton;
        
        [SerializeField, Tooltip("退出游戏按钮")]
        private Button quitButton;
        
        // 私有变量
        private Dictionary<GameState, GameObject> statePanels;
        private GameState currentUIState;
        private Coroutine loadingCoroutine;
        
        #region Unity生命周期
        
        /// <summary>
        /// 初始化UI管理器
        /// </summary>
        private void Awake()
        {
            InitializePanelDictionary();
            SetupButtonListeners();
            
            // 初始状态显示加载界面
            ShowPanel(GameState.Initializing);
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
        
        #endregion
        
        #region 初始化方法
        
        /// <summary>
        /// 初始化面板字典
        /// </summary>
        private void InitializePanelDictionary()
        {
            statePanels = new Dictionary<GameState, GameObject>
            {
                { GameState.Initializing, loadingPanel },
                { GameState.Ready, mainMenuPanel },
                { GameState.Playing, gamePanel },
                { GameState.Paused, pausePanel },
                { GameState.GameOver, gameOverPanel }
            };
            
            // 验证所有面板都已分配
            foreach (var kvp in statePanels)
            {
                if (kvp.Value == null)
                {
                    Debug.LogWarning($"[UIManager] {kvp.Key} 状态对应的UI面板未分配");
                }
            }
        }
        
        /// <summary>
        /// 设置按钮监听器
        /// </summary>
        private void SetupButtonListeners()
        {
            if (startGameButton != null)
                startGameButton.onClick.AddListener(OnStartGameClicked);
            
            if (pauseButton != null)
                pauseButton.onClick.AddListener(OnPauseClicked);
            
            if (resumeButton != null)
                resumeButton.onClick.AddListener(OnResumeClicked);
            
            if (restartButton != null)
                restartButton.onClick.AddListener(OnRestartClicked);
            
            if (quitButton != null)
                quitButton.onClick.AddListener(OnQuitClicked);
        }
        
        #endregion
        
        #region 事件处理
        
        /// <summary>
        /// 处理游戏状态变化事件
        /// </summary>
        /// <param name="gameStateEvent">游戏状态变化事件</param>
        private void OnGameStateChanged(GameStateChangedEvent gameStateEvent)
        {
            ShowPanel(gameStateEvent.NewState);
        }
        
        #endregion
        
        #region UI面板管理
        
        /// <summary>
        /// 显示指定状态对应的UI面板
        /// </summary>
        /// <param name="gameState">游戏状态</param>
        public void ShowPanel(GameState gameState)
        {
            // 隐藏所有面板
            foreach (var panel in statePanels.Values)
            {
                if (panel != null)
                    panel.SetActive(false);
            }
            
            // 显示目标面板
            if (statePanels.TryGetValue(gameState, out GameObject targetPanel) && targetPanel != null)
            {
                targetPanel.SetActive(true);
                currentUIState = gameState;
                
                // 根据状态执行特殊逻辑
                OnPanelShown(gameState);
            }
            else
            {
                Debug.LogWarning($"[UIManager] 未找到状态 {gameState} 对应的UI面板");
            }
        }
        
        /// <summary>
        /// 面板显示后的特殊处理
        /// </summary>
        /// <param name="gameState">游戏状态</param>
        private void OnPanelShown(GameState gameState)
        {
            switch (gameState)
            {
                case GameState.Initializing:
                    StartLoadingAnimation();
                    break;
                    
                case GameState.Ready:
                    StopLoadingAnimation();
                    break;
                    
                case GameState.Playing:
                    InitializeGameUI();
                    break;
                    
                case GameState.GameOver:
                    UpdateGameOverUI();
                    break;
            }
        }
        
        #endregion
        
        #region 加载界面管理
        
        /// <summary>
        /// 开始加载动画
        /// </summary>
        private void StartLoadingAnimation()
        {
            if (loadingCoroutine != null)
            {
                StopCoroutine(loadingCoroutine);
            }
            
            loadingCoroutine = StartCoroutine(LoadingAnimationCoroutine());
        }
        
        /// <summary>
        /// 停止加载动画
        /// </summary>
        private void StopLoadingAnimation()
        {
            if (loadingCoroutine != null)
            {
                StopCoroutine(loadingCoroutine);
                loadingCoroutine = null;
            }
        }
        
        /// <summary>
        /// 加载动画协程
        /// </summary>
        /// <returns>协程</returns>
        private IEnumerator LoadingAnimationCoroutine()
        {
            float progress = 0f;
            string[] loadingSteps = {
                "初始化游戏系统...",
                "加载XR环境...",
                "准备游戏资源...",
                "初始化UI系统...",
                "准备就绪！"
            };
            
            int currentStep = 0;
            
            while (progress < 1f && currentUIState == GameState.Initializing)
            {
                // 更新进度
                progress += Time.deltaTime * 0.5f; // 2秒完成加载
                
                // 更新进度条
                if (loadingProgressBar != null)
                {
                    loadingProgressBar.value = progress;
                }
                
                // 更新状态文本
                int stepIndex = Mathf.FloorToInt(progress * loadingSteps.Length);
                stepIndex = Mathf.Clamp(stepIndex, 0, loadingSteps.Length - 1);
                
                if (stepIndex != currentStep && loadingStatusText != null)
                {
                    currentStep = stepIndex;
                    loadingStatusText.text = loadingSteps[currentStep];
                }
                
                yield return null;
            }
            
            // 确保进度条满格
            if (loadingProgressBar != null)
            {
                loadingProgressBar.value = 1f;
            }
            
            if (loadingStatusText != null)
            {
                loadingStatusText.text = "加载完成！";
            }
        }
        
        /// <summary>
        /// 更新加载进度
        /// </summary>
        /// <param name="progress">进度值（0-1）</param>
        /// <param name="status">状态文本</param>
        public void UpdateLoadingProgress(float progress, string status = "")
        {
            if (loadingProgressBar != null)
            {
                loadingProgressBar.value = Mathf.Clamp01(progress);
            }
            
            if (!string.IsNullOrEmpty(status) && loadingStatusText != null)
            {
                loadingStatusText.text = status;
            }
        }
        
        #endregion
        
        #region 游戏界面管理
        
        /// <summary>
        /// 初始化游戏UI
        /// </summary>
        private void InitializeGameUI()
        {
            UpdateScore(0);
            UpdateTime(180f); // 默认3分钟
            UpdateCombo(0);
            
            if (comboPanel != null)
            {
                comboPanel.SetActive(false);
            }
        }
        
        /// <summary>
        /// 更新得分显示
        /// </summary>
        /// <param name="score">当前得分</param>
        public void UpdateScore(int score)
        {
            if (scoreText != null)
            {
                scoreText.text = $"得分: {score:N0}";
            }
        }
        
        /// <summary>
        /// 更新时间显示
        /// </summary>
        /// <param name="remainingTime">剩余时间（秒）</param>
        public void UpdateTime(float remainingTime)
        {
            if (timeText != null)
            {
                int minutes = Mathf.FloorToInt(remainingTime / 60f);
                int seconds = Mathf.FloorToInt(remainingTime % 60f);
                timeText.text = $"时间: {minutes:00}:{seconds:00}";
                
                // 时间不足时变红
                if (remainingTime <= 30f)
                {
                    timeText.color = Color.red;
                }
                else if (remainingTime <= 60f)
                {
                    timeText.color = Color.yellow;
                }
                else
                {
                    timeText.color = Color.white;
                }
            }
        }
        
        /// <summary>
        /// 更新连击显示
        /// </summary>
        /// <param name="comboCount">连击数</param>
        public void UpdateCombo(int comboCount)
        {
            if (comboText != null)
            {
                comboText.text = $"连击: {comboCount}";
            }
            
            // 显示/隐藏连击面板
            if (comboPanel != null)
            {
                comboPanel.SetActive(comboCount > 1);
            }
        }
        
        #endregion
        
        #region 游戏结束界面管理
        
        /// <summary>
        /// 更新游戏结束UI
        /// </summary>
        private void UpdateGameOverUI()
        {
            // 这里可以从GameManager获取最终数据
            // 暂时使用占位数据
            if (finalScoreText != null)
            {
                finalScoreText.text = "最终得分: 0";
            }
            
            if (statsText != null)
            {
                statsText.text = "游戏统计:\n总匹配: 0\n命中率: 0%\n最高连击: 0";
            }
        }
        
        /// <summary>
        /// 更新游戏结束数据
        /// </summary>
        /// <param name="finalScore">最终得分</param>
        /// <param name="totalMatches">总匹配数</param>
        /// <param name="accuracy">命中率</param>
        /// <param name="maxCombo">最高连击</param>
        public void UpdateGameOverData(int finalScore, int totalMatches, float accuracy, int maxCombo)
        {
            if (finalScoreText != null)
            {
                finalScoreText.text = $"最终得分: {finalScore:N0}";
            }
            
            if (statsText != null)
            {
                statsText.text = $"游戏统计:\n总匹配: {totalMatches}\n命中率: {accuracy:P1}\n最高连击: {maxCombo}";
            }
        }
        
        #endregion
        
        #region 按钮事件处理
        
        /// <summary>
        /// 开始游戏按钮点击
        /// </summary>
        private void OnStartGameClicked()
        {
            GameManager.Instance?.StartGame();
        }
        
        /// <summary>
        /// 暂停按钮点击
        /// </summary>
        private void OnPauseClicked()
        {
            GameManager.Instance?.PauseGame();
        }
        
        /// <summary>
        /// 继续游戏按钮点击
        /// </summary>
        private void OnResumeClicked()
        {
            GameManager.Instance?.ResumeGame();
        }
        
        /// <summary>
        /// 重新开始按钮点击
        /// </summary>
        private void OnRestartClicked()
        {
            GameManager.Instance?.RestartGame();
        }
        
        /// <summary>
        /// 退出游戏按钮点击
        /// </summary>
        private void OnQuitClicked()
        {
            #if UNITY_EDITOR
                UnityEditor.EditorApplication.isPlaying = false;
            #else
                Application.Quit();
            #endif
        }
        
        #endregion
        
        #region 公共接口
        
        /// <summary>
        /// 获取当前UI状态
        /// </summary>
        /// <returns>当前UI状态</returns>
        public GameState GetCurrentUIState()
        {
            return currentUIState;
        }
        
        /// <summary>
        /// 检查指定面板是否激活
        /// </summary>
        /// <param name="gameState">游戏状态</param>
        /// <returns>面板是否激活</returns>
        public bool IsPanelActive(GameState gameState)
        {
            if (statePanels.TryGetValue(gameState, out GameObject panel))
            {
                return panel != null && panel.activeInHierarchy;
            }
            return false;
        }
        
        #endregion
    }
}