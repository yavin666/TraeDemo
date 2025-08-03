using UnityEngine;
using MRMatchBlaster.Core;

namespace MRMatchBlaster.Core
{
    /// <summary>
    /// 场景启动器 - 负责场景的初始化和系统启动
    /// 这个组件应该挂载在每个场景的根 GameObject 上
    /// 职责：
    /// 1. 确保 GameManager 正确初始化
    /// 2. 创建和配置 GameConfig
    /// 3. 设置场景特定的配置
    /// 4. 处理场景加载完成后的初始化流程
    /// </summary>
    public class SceneBootstrapper : MonoBehaviour
    {
        #region 配置字段
        [Header("场景配置")]
        
        /// <summary>
        /// 游戏配置资源引用
        /// 如果为空，将使用默认配置
        /// </summary>
        [SerializeField]
        [Tooltip("游戏配置 ScriptableObject，如果为空将创建默认配置")]
        private GameConfig _gameConfig;
        
        /// <summary>
        /// 是否为测试场景
        /// 测试场景会启用额外的调试功能
        /// </summary>
        [SerializeField]
        [Tooltip("是否为测试场景，启用调试功能")]
        private bool _isTestScene = true;
        
        /// <summary>
        /// 是否自动启动游戏
        /// 测试场景通常需要自动启动
        /// </summary>
        [SerializeField]
        [Tooltip("是否在初始化完成后自动启动游戏")]
        private bool _autoStartGame = false;
        
        /// <summary>
        /// GameManager 预制体引用
        /// 如果场景中没有 GameManager，将实例化此预制体
        /// </summary>
        [SerializeField]
        [Tooltip("GameManager 预制体，如果场景中没有 GameManager 将实例化此预制体")]
        private GameObject _gameManagerPrefab;
        #endregion

        #region 私有字段
        /// <summary>
        /// GameManager 实例引用
        /// </summary>
        private GameManager _gameManager;
        
        /// <summary>
        /// 是否已经初始化完成
        /// </summary>
        private bool _isInitialized = false;
        #endregion

        #region Unity 生命周期
        /// <summary>
        /// 场景启动时的初始化
        /// 在所有 Awake 之后，Start 之前执行
        /// </summary>
        private void Awake()
        {
            Debug.Log("[SceneBootstrapper] 开始场景初始化");
            InitializeScene();
        }

        /// <summary>
        /// 场景启动完成后的处理
        /// </summary>
        private void Start()
        {
            if (_isInitialized)
            {
                PostInitialization();
            }
        }
        #endregion

        #region 初始化方法
        /// <summary>
        /// 初始化场景
        /// 确保所有必要的系统都正确设置
        /// </summary>
        private void InitializeScene()
        {
            try
            {
                // 1. 设置应用程序配置
                SetupApplicationSettings();
                
                // 2. 确保 GameManager 存在
                EnsureGameManager();
                
                // 3. 配置 GameManager
                ConfigureGameManager();
                
                // 4. 设置测试环境（如果是测试场景）
                if (_isTestScene)
                {
                    SetupTestEnvironment();
                }
                
                _isInitialized = true;
                Debug.Log("[SceneBootstrapper] 场景初始化完成");
            }
            catch (System.Exception ex)
            {
                Debug.LogError($"[SceneBootstrapper] 场景初始化失败: {ex.Message}");
                _isInitialized = false;
            }
        }

        /// <summary>
        /// 设置应用程序级别的配置
        /// 包括帧率、质量设置等
        /// </summary>
        private void SetupApplicationSettings()
        {
            // 设置目标帧率（Quest 3 推荐 90 FPS）
            Application.targetFrameRate = 90;
            
            // 设置 VSync（XR 中通常由 XR 系统管理）
            QualitySettings.vSyncCount = 0;
            
            // 防止屏幕休眠
            Screen.sleepTimeout = SleepTimeout.NeverSleep;
            
            Debug.Log("[SceneBootstrapper] 应用程序设置配置完成");
        }

        /// <summary>
        /// 确保场景中存在 GameManager
        /// 如果不存在，则创建一个
        /// </summary>
        private void EnsureGameManager()
        {
            // 首先尝试查找现有的 GameManager
            _gameManager = GameManager.Instance;
            
            if (_gameManager == null)
            {
                // 尝试在场景中查找 GameManager GameObject
                GameObject gameManagerGO = GameObject.FindGameObjectWithTag("GameController");
                
                if (gameManagerGO == null && _gameManagerPrefab != null)
                {
                    // 如果场景中没有，且有预制体引用，则实例化预制体
                    gameManagerGO = Instantiate(_gameManagerPrefab);
                    gameManagerGO.name = "GameManager";
                    Debug.Log("[SceneBootstrapper] 从预制体创建 GameManager");
                }
                else if (gameManagerGO == null)
                {
                    // 如果都没有，创建一个新的 GameObject
                    gameManagerGO = new GameObject("GameManager");
                    gameManagerGO.tag = "GameController";
                    Debug.Log("[SceneBootstrapper] 创建新的 GameManager GameObject");
                }
                
                // 确保 GameManager 组件存在
                _gameManager = gameManagerGO.GetComponent<GameManager>();
                if (_gameManager == null)
                {
                    _gameManager = gameManagerGO.AddComponent<GameManager>();
                    Debug.Log("[SceneBootstrapper] 添加 GameManager 组件");
                }
            }
            
            Debug.Log("[SceneBootstrapper] GameManager 确认存在");
        }

        /// <summary>
        /// 配置 GameManager
        /// 设置游戏配置和其他必要参数
        /// </summary>
        private void ConfigureGameManager()
        {
            if (_gameManager == null)
            {
                Debug.LogError("[SceneBootstrapper] GameManager 不存在，无法配置");
                return;
            }

            // 如果没有配置 GameConfig，创建一个默认的
            if (_gameConfig == null)
            {
                _gameConfig = CreateDefaultGameConfig();
            }

            // 通过反射设置 GameConfig（因为 GameManager 中的字段是私有的）
            var gameConfigField = typeof(GameManager).GetField("_gameConfig", 
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            
            if (gameConfigField != null)
            {
                gameConfigField.SetValue(_gameManager, _gameConfig);
                Debug.Log("[SceneBootstrapper] GameConfig 配置完成");
            }
            else
            {
                Debug.LogWarning("[SceneBootstrapper] 无法设置 GameConfig，字段不存在");
            }
        }

        /// <summary>
        /// 创建默认的游戏配置
        /// 用于测试和开发环境
        /// </summary>
        private GameConfig CreateDefaultGameConfig()
        {
            var config = ScriptableObject.CreateInstance<GameConfig>();
            
            // 设置测试友好的默认值
            if (_isTestScene)
            {
                // 测试场景的配置
                var enableDebugField = typeof(GameConfig).GetField("enableDebugMode", 
                    System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
                enableDebugField?.SetValue(config, true);
                
                var showPerformanceField = typeof(GameConfig).GetField("showPerformanceStats", 
                    System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
                showPerformanceField?.SetValue(config, true);
                
                var verboseLoggingField = typeof(GameConfig).GetField("enableVerboseLogging", 
                    System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
                verboseLoggingField?.SetValue(config, true);
            }
            
            Debug.Log("[SceneBootstrapper] 创建默认 GameConfig");
            return config;
        }

        /// <summary>
        /// 设置测试环境
        /// 启用调试功能和测试辅助工具
        /// </summary>
        private void SetupTestEnvironment()
        {
            Debug.Log("[SceneBootstrapper] 设置测试环境");
            
            // 创建调试信息显示
            CreateDebugUI();
            
            // 订阅 GameManager 事件用于调试
            SubscribeToGameManagerEvents();
            
            // 设置测试用的输入处理
            SetupTestInput();
        }

        /// <summary>
        /// 创建调试 UI
        /// 显示游戏状态和性能信息
        /// </summary>
        private void CreateDebugUI()
        {
            // 创建一个简单的调试信息显示
            GameObject debugCanvas = new GameObject("DebugCanvas");
            Canvas canvas = debugCanvas.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvas.sortingOrder = 1000; // 确保在最上层
            
            debugCanvas.AddComponent<UnityEngine.UI.CanvasScaler>();
            debugCanvas.AddComponent<UnityEngine.UI.GraphicRaycaster>();
            
            // 创建状态显示文本
            GameObject statusText = new GameObject("StatusText");
            statusText.transform.SetParent(debugCanvas.transform);
            
            var text = statusText.AddComponent<UnityEngine.UI.Text>();
            text.text = "游戏状态: 初始化中...";
            text.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            text.fontSize = 24;
            text.color = Color.white;
            
            var rectTransform = statusText.GetComponent<RectTransform>();
            rectTransform.anchorMin = new Vector2(0, 1);
            rectTransform.anchorMax = new Vector2(0, 1);
            rectTransform.pivot = new Vector2(0, 1);
            rectTransform.anchoredPosition = new Vector2(10, -10);
            rectTransform.sizeDelta = new Vector2(400, 50);
            
            Debug.Log("[SceneBootstrapper] 调试 UI 创建完成");
        }

        /// <summary>
        /// 订阅 GameManager 事件
        /// 用于调试和状态监控
        /// </summary>
        private void SubscribeToGameManagerEvents()
        {
            GameManager.OnGameStateChanged += OnGameStateChanged;
            GameManager.OnGameStarted += OnGameStarted;
            GameManager.OnGameEnded += OnGameEnded;
            GameManager.OnSystemError += OnSystemError;
            
            Debug.Log("[SceneBootstrapper] GameManager 事件订阅完成");
        }

        /// <summary>
        /// 设置测试输入
        /// 添加键盘快捷键用于测试
        /// </summary>
        private void SetupTestInput()
        {
            // 在测试场景中，可以添加键盘输入来模拟 XR 交互
            // 这里只是预留接口，具体实现可以后续添加
            Debug.Log("[SceneBootstrapper] 测试输入设置完成");
        }
        #endregion

        #region 后初始化处理
        /// <summary>
        /// 初始化完成后的处理
        /// 执行需要在所有系统就绪后进行的操作
        /// </summary>
        private void PostInitialization()
        {
            Debug.Log("[SceneBootstrapper] 执行后初始化处理");
            
            // 如果设置了自动启动，等待一帧后启动游戏
            if (_autoStartGame)
            {
                Invoke(nameof(AutoStartGame), 1f);
            }
        }

        /// <summary>
        /// 自动启动游戏
        /// 用于测试场景的自动化流程
        /// </summary>
        private void AutoStartGame()
        {
            if (_gameManager != null && _gameManager.CurrentState == GameState.Ready)
            {
                _gameManager.StartGameplay();
                Debug.Log("[SceneBootstrapper] 自动启动游戏");
            }
        }
        #endregion

        #region 事件处理
        /// <summary>
        /// 游戏状态改变事件处理
        /// </summary>
        /// <param name="previousState">上一个状态</param>
        /// <param name="newState">新状态</param>
        private void OnGameStateChanged(GameState previousState, GameState newState)
        {
            Debug.Log($"[SceneBootstrapper] 游戏状态改变: {previousState} -> {newState}");
            
            // 更新调试 UI
            UpdateDebugUI($"游戏状态: {newState}");
        }

        /// <summary>
        /// 游戏开始事件处理
        /// </summary>
        private void OnGameStarted()
        {
            Debug.Log("[SceneBootstrapper] 游戏开始");
            UpdateDebugUI("游戏状态: 游戏进行中");
        }

        /// <summary>
        /// 游戏结束事件处理
        /// </summary>
        /// <param name="finalScore">最终分数</param>
        private void OnGameEnded(int finalScore)
        {
            Debug.Log($"[SceneBootstrapper] 游戏结束，最终分数: {finalScore}");
            UpdateDebugUI($"游戏结束 - 分数: {finalScore}");
        }

        /// <summary>
        /// 系统错误事件处理
        /// </summary>
        /// <param name="errorMessage">错误信息</param>
        private void OnSystemError(string errorMessage)
        {
            Debug.LogError($"[SceneBootstrapper] 系统错误: {errorMessage}");
            UpdateDebugUI($"系统错误: {errorMessage}");
        }

        /// <summary>
        /// 更新调试 UI 显示
        /// </summary>
        /// <param name="message">要显示的消息</param>
        private void UpdateDebugUI(string message)
        {
            var statusText = GameObject.Find("StatusText");
            if (statusText != null)
            {
                var text = statusText.GetComponent<UnityEngine.UI.Text>();
                if (text != null)
                {
                    text.text = message;
                }
            }
        }
        #endregion

        #region 清理方法
        /// <summary>
        /// 组件销毁时的清理
        /// </summary>
        private void OnDestroy()
        {
            // 取消事件订阅
            GameManager.OnGameStateChanged -= OnGameStateChanged;
            GameManager.OnGameStarted -= OnGameStarted;
            GameManager.OnGameEnded -= OnGameEnded;
            GameManager.OnSystemError -= OnSystemError;
            
            Debug.Log("[SceneBootstrapper] 场景启动器清理完成");
        }
        #endregion

        #region 公共接口
        /// <summary>
        /// 手动重新初始化场景
        /// 用于调试和测试
        /// </summary>
        [ContextMenu("重新初始化场景")]
        public void ReinitializeScene()
        {
            _isInitialized = false;
            InitializeScene();
            if (_isInitialized)
            {
                PostInitialization();
            }
        }

        /// <summary>
        /// 获取当前的游戏配置
        /// </summary>
        /// <returns>游戏配置</returns>
        public GameConfig GetGameConfig()
        {
            return _gameConfig;
        }

        /// <summary>
        /// 设置游戏配置
        /// </summary>
        /// <param name="config">新的游戏配置</param>
        public void SetGameConfig(GameConfig config)
        {
            _gameConfig = config;
            if (_isInitialized && _gameManager != null)
            {
                ConfigureGameManager();
            }
        }
        #endregion
    }
}