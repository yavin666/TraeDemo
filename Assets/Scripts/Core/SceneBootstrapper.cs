using System.Collections;
using UnityEngine;
using MRMatchBlaster.Core.Events;

namespace MRMatchBlaster.Core
{
    /// <summary>
    /// 场景启动器 - 统一管理场景初始化流程
    /// 负责按顺序初始化各个系统组件，确保依赖关系正确
    /// </summary>
    public class SceneBootstrapper : MonoBehaviour
    {
        [Header("初始化设置")]
        [SerializeField] private bool _autoStart = true;
        [SerializeField] private float _initializationDelay = 0.5f;
        
        [Header("系统组件引用")]
        [SerializeField] private GameManager _gameManager;
        
        [Header("调试信息")]
        [SerializeField] private bool _enableDebugLogs = true;
        
        // 初始化状态
        private bool _isInitialized = false;
        private bool _isInitializing = false;
        
        #region Unity生命周期
        
        private void Awake()
        {
            if (_enableDebugLogs)
            {
                Debug.Log("[SceneBootstrapper] 场景启动器唤醒");
            }
            
            // 确保GameManager存在
            if (_gameManager == null)
            {
                _gameManager = FindObjectOfType<GameManager>();
                if (_gameManager == null)
                {
                    if (_enableDebugLogs)
                    {
                        Debug.LogWarning("[SceneBootstrapper] 未找到GameManager，将自动创建");
                    }
                    
                    GameObject gameManagerObject = new GameObject("GameManager");
                    _gameManager = gameManagerObject.AddComponent<GameManager>();
                }
            }
        }
        
        private void Start()
        {
            if (_autoStart && !_isInitialized && !_isInitializing)
            {
                StartCoroutine(InitializeSceneCoroutine());
            }
        }
        
        #endregion
        
        #region 初始化流程
        
        /// <summary>
        /// 手动开始场景初始化
        /// </summary>
        public void StartInitialization()
        {
            if (!_isInitialized && !_isInitializing)
            {
                StartCoroutine(InitializeSceneCoroutine());
            }
            else
            {
                if (_enableDebugLogs)
                {
                    Debug.LogWarning("[SceneBootstrapper] 场景已经初始化或正在初始化中");
                }
            }
        }
        
        /// <summary>
        /// 场景初始化协程
        /// </summary>
        private IEnumerator InitializeSceneCoroutine()
        {
            _isInitializing = true;
            
            if (_enableDebugLogs)
            {
                Debug.Log("[SceneBootstrapper] 开始场景初始化流程");
            }
            
            // 等待初始化延迟
            yield return new WaitForSeconds(_initializationDelay);
            
            // 步骤1: 初始化事件总线
            yield return StartCoroutine(InitializeEventBus());
            
            // 步骤2: 初始化核心系统
            yield return StartCoroutine(InitializeCoreSystems());
            
            // 步骤3: 初始化XR系统
            yield return StartCoroutine(InitializeXRSystems());
            
            // 步骤4: 初始化游戏系统
            yield return StartCoroutine(InitializeGameSystems());
            
            // 步骤5: 初始化UI系统
            yield return StartCoroutine(InitializeUISystems());
            
            // 步骤6: 完成初始化
            yield return StartCoroutine(FinalizeInitialization());
            
            _isInitialized = true;
            _isInitializing = false;
            
            if (_enableDebugLogs)
            {
                Debug.Log("[SceneBootstrapper] 场景初始化完成");
            }
        }
        
        /// <summary>
        /// 初始化事件总线
        /// </summary>
        private IEnumerator InitializeEventBus()
        {
            if (_enableDebugLogs)
            {
                Debug.Log("[SceneBootstrapper] 初始化事件总线");
            }
            
            // 确保EventBus实例存在
            var eventBus = EventBus.Instance;
            
            yield return new WaitForEndOfFrame();
        }
        
        /// <summary>
        /// 初始化核心系统
        /// </summary>
        private IEnumerator InitializeCoreSystems()
        {
            if (_enableDebugLogs)
            {
                Debug.Log("[SceneBootstrapper] 初始化核心系统");
            }
            
            // 确保GameManager已初始化
            if (_gameManager == null)
            {
                Debug.LogError("[SceneBootstrapper] GameManager未找到！");
                yield break;
            }
            
            yield return new WaitForEndOfFrame();
        }
        
        /// <summary>
        /// 初始化XR系统
        /// </summary>
        private IEnumerator InitializeXRSystems()
        {
            if (_enableDebugLogs)
            {
                Debug.Log("[SceneBootstrapper] 初始化XR系统");
            }
            
            // TODO: 初始化XR Interaction Toolkit组件
            // TODO: 初始化OpenXR设置
            // TODO: 初始化手部追踪
            
            yield return new WaitForEndOfFrame();
        }
        
        /// <summary>
        /// 初始化游戏系统
        /// </summary>
        private IEnumerator InitializeGameSystems()
        {
            if (_enableDebugLogs)
            {
                Debug.Log("[SceneBootstrapper] 初始化游戏系统");
            }
            
            // TODO: 初始化几何体生成器
            // TODO: 初始化匹配系统
            // TODO: 初始化得分系统
            
            yield return new WaitForEndOfFrame();
        }
        
        /// <summary>
        /// 初始化UI系统
        /// </summary>
        private IEnumerator InitializeUISystems()
        {
            if (_enableDebugLogs)
            {
                Debug.Log("[SceneBootstrapper] 初始化UI系统");
            }
            
            // TODO: 初始化UI管理器
            // TODO: 初始化World-Space Canvas
            // TODO: 初始化加载UI
            
            yield return new WaitForEndOfFrame();
        }
        
        /// <summary>
        /// 完成初始化
        /// </summary>
        private IEnumerator FinalizeInitialization()
        {
            if (_enableDebugLogs)
            {
                Debug.Log("[SceneBootstrapper] 完成初始化设置");
            }
            
            // 发布场景初始化完成事件
            EventBus.Instance.Publish(new SceneInitializedEvent());
            
            yield return new WaitForEndOfFrame();
        }
        
        #endregion
        
        #region 公共接口
        
        /// <summary>
        /// 检查是否已初始化
        /// </summary>
        public bool IsInitialized => _isInitialized;
        
        /// <summary>
        /// 检查是否正在初始化
        /// </summary>
        public bool IsInitializing => _isInitializing;
        
        /// <summary>
        /// 获取GameManager引用
        /// </summary>
        public GameManager GameManager => _gameManager;
        
        #endregion
        
        #region 调试方法
        
        /// <summary>
        /// 重新初始化场景（仅用于调试）
        /// </summary>
        [ContextMenu("重新初始化场景")]
        public void ReinitializeScene()
        {
            if (Application.isPlaying)
            {
                _isInitialized = false;
                _isInitializing = false;
                StartInitialization();
            }
        }
        
        #endregion
    }
    
    /// <summary>
    /// 场景初始化完成事件
    /// </summary>
    public class SceneInitializedEvent
    {
        public float InitializationTime { get; set; }
        
        public SceneInitializedEvent()
        {
            InitializationTime = Time.time;
        }
    }
}