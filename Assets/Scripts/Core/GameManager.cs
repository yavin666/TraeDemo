using System.Collections;
using UnityEngine;
using MRMatchBlaster.Core.Events;

namespace MRMatchBlaster.Core
{
    /// <summary>
    /// 游戏管理器 - 核心状态机
    /// 负责管理游戏的五个状态和状态间的切换逻辑
    /// </summary>
    public class GameManager : MonoBehaviour
    {
        [Header("游戏状态")]
        [SerializeField] private GameState _currentState = GameState.Initializing;
        
        [Header("调试信息")]
        [SerializeField] private bool _enableDebugLogs = true;
        
        // 单例实例
        private static GameManager _instance;
        public static GameManager Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = FindObjectOfType<GameManager>();
                    if (_instance == null)
                    {
                        GameObject gameManagerObject = new GameObject("GameManager");
                        _instance = gameManagerObject.AddComponent<GameManager>();
                    }
                }
                return _instance;
            }
        }
        
        // 属性访问器
        public GameState CurrentState => _currentState;
        
        // 状态切换标志
        private bool _isTransitioning = false;
        
        #region Unity生命周期
        
        private void Awake()
        {
            // 确保单例
            if (_instance == null)
            {
                _instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else if (_instance != this)
            {
                Destroy(gameObject);
                return;
            }
            
            InitializeGameManager();
        }
        
        private void Start()
        {
            // 开始初始化状态
            StartCoroutine(InitializeGameCoroutine());
        }
        
        #endregion
        
        #region 初始化
        
        /// <summary>
        /// 初始化游戏管理器
        /// </summary>
        private void InitializeGameManager()
        {
            if (_enableDebugLogs)
            {
                Debug.Log("[GameManager] 游戏管理器初始化完成");
            }
        }
        
        /// <summary>
        /// 游戏初始化协程
        /// </summary>
        private IEnumerator InitializeGameCoroutine()
        {
            if (_enableDebugLogs)
            {
                Debug.Log("[GameManager] 开始游戏初始化流程");
            }
            
            // 等待LoadingUI完成加载流程
            // LoadingUI会调用OnInitializationComplete()来通知完成
            yield break;
        }
        
        /// <summary>
        /// 初始化完成回调
        /// 由LoadingUI调用，表示加载流程已完成
        /// </summary>
        public void OnInitializationComplete()
        {
            if (_enableDebugLogs)
            {
                Debug.Log("[GameManager] 初始化完成，切换到Ready状态");
            }
            ChangeState(GameState.Ready);
        }
        
        #endregion
        
        #region 状态管理
        
        /// <summary>
        /// 改变游戏状态
        /// </summary>
        /// <param name="newState">新的游戏状态</param>
        public void ChangeState(GameState newState)
        {
            if (_isTransitioning)
            {
                if (_enableDebugLogs)
                {
                    Debug.LogWarning("[GameManager] 状态正在切换中，忽略新的状态切换请求");
                }
                return;
            }
            
            if (_currentState == newState)
            {
                if (_enableDebugLogs)
                {
                    Debug.LogWarning($"[GameManager] 尝试切换到相同状态: {newState}");
                }
                return;
            }
            
            StartCoroutine(ChangeStateCoroutine(newState));
        }
        
        /// <summary>
        /// 状态切换协程
        /// </summary>
        /// <param name="newState">新状态</param>
        private IEnumerator ChangeStateCoroutine(GameState newState)
        {
            _isTransitioning = true;
            
            GameState previousState = _currentState;
            
            if (_enableDebugLogs)
            {
                Debug.Log($"[GameManager] 状态切换: {previousState} -> {newState}");
            }
            
            // 退出当前状态
            yield return StartCoroutine(ExitState(previousState));
            
            // 更新状态
            _currentState = newState;
            
            // 发布状态变更事件
            EventBus.Instance.Publish(new GameStateChangedEvent(previousState, newState));
            
            // 进入新状态
            yield return StartCoroutine(EnterState(newState));
            
            _isTransitioning = false;
            
            if (_enableDebugLogs)
            {
                Debug.Log($"[GameManager] 状态切换完成: {newState}");
            }
        }
        
        /// <summary>
        /// 退出当前状态
        /// </summary>
        /// <param name="state">要退出的状态</param>
        private IEnumerator ExitState(GameState state)
        {
            switch (state)
            {
                case GameState.Initializing:
                    yield return ExitInitializingState();
                    break;
                case GameState.Ready:
                    yield return ExitReadyState();
                    break;
                case GameState.Playing:
                    yield return ExitPlayingState();
                    break;
                case GameState.Paused:
                    yield return ExitPausedState();
                    break;
                case GameState.GameOver:
                    yield return ExitGameOverState();
                    break;
            }
        }
        
        /// <summary>
        /// 进入新状态
        /// </summary>
        /// <param name="state">要进入的状态</param>
        private IEnumerator EnterState(GameState state)
        {
            switch (state)
            {
                case GameState.Initializing:
                    yield return EnterInitializingState();
                    break;
                case GameState.Ready:
                    yield return EnterReadyState();
                    break;
                case GameState.Playing:
                    yield return EnterPlayingState();
                    break;
                case GameState.Paused:
                    yield return EnterPausedState();
                    break;
                case GameState.GameOver:
                    yield return EnterGameOverState();
                    break;
            }
        }
        
        #endregion
        
        #region 状态处理方法
        
        // 初始化状态
        private IEnumerator EnterInitializingState()
        {
            if (_enableDebugLogs) Debug.Log("[GameManager] 进入初始化状态");
            yield return null;
        }
        
        private IEnumerator ExitInitializingState()
        {
            if (_enableDebugLogs) Debug.Log("[GameManager] 退出初始化状态");
            yield return null;
        }
        
        // 准备状态
        private IEnumerator EnterReadyState()
        {
            if (_enableDebugLogs) Debug.Log("[GameManager] 进入准备状态");
            yield return null;
        }
        
        private IEnumerator ExitReadyState()
        {
            if (_enableDebugLogs) Debug.Log("[GameManager] 退出准备状态");
            yield return null;
        }
        
        // 游戏进行状态
        private IEnumerator EnterPlayingState()
        {
            if (_enableDebugLogs) Debug.Log("[GameManager] 进入游戏进行状态");
            yield return null;
        }
        
        private IEnumerator ExitPlayingState()
        {
            if (_enableDebugLogs) Debug.Log("[GameManager] 退出游戏进行状态");
            yield return null;
        }
        
        // 暂停状态
        private IEnumerator EnterPausedState()
        {
            if (_enableDebugLogs) Debug.Log("[GameManager] 进入暂停状态");
            yield return null;
        }
        
        private IEnumerator ExitPausedState()
        {
            if (_enableDebugLogs) Debug.Log("[GameManager] 退出暂停状态");
            yield return null;
        }
        
        // 游戏结束状态
        private IEnumerator EnterGameOverState()
        {
            if (_enableDebugLogs) Debug.Log("[GameManager] 进入游戏结束状态");
            yield return null;
        }
        
        private IEnumerator ExitGameOverState()
        {
            if (_enableDebugLogs) Debug.Log("[GameManager] 退出游戏结束状态");
            yield return null;
        }
        
        #endregion
        
        #region 公共接口
        
        /// <summary>
        /// 开始游戏
        /// </summary>
        public void StartGame()
        {
            if (_currentState == GameState.Ready)
            {
                ChangeState(GameState.Playing);
            }
        }
        
        /// <summary>
        /// 暂停游戏
        /// </summary>
        public void PauseGame()
        {
            if (_currentState == GameState.Playing)
            {
                ChangeState(GameState.Paused);
            }
        }
        
        /// <summary>
        /// 恢复游戏
        /// </summary>
        public void ResumeGame()
        {
            if (_currentState == GameState.Paused)
            {
                ChangeState(GameState.Playing);
            }
        }
        
        /// <summary>
        /// 结束游戏
        /// </summary>
        public void EndGame()
        {
            if (_currentState == GameState.Playing || _currentState == GameState.Paused)
            {
                ChangeState(GameState.GameOver);
            }
        }
        
        /// <summary>
        /// 重新开始游戏
        /// </summary>
        public void RestartGame()
        {
            ChangeState(GameState.Initializing);
        }
        
        #endregion
    }
}