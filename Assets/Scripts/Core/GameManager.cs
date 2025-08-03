using System;
using UnityEngine;
using UnityEngine.Events;

namespace MRMatchBlaster.Core
{
    /// <summary>
    /// 游戏状态枚举
    /// 定义了 MR MatchBlaster 游戏的主要状态流转
    /// </summary>
    public enum GameState
    {
        /// <summary>初始化状态 - 加载资源、初始化 XR 系统</summary>
        Initializing,
        
        /// <summary>空间校准状态 - 设置空间锚点、房间边界识别</summary>
        SpatialCalibration,
        
        /// <summary>准备状态 - 生成初始几何体、等待玩家开始</summary>
        Ready,
        
        /// <summary>游戏进行状态 - 玩家可以发射、匹配、得分</summary>
        Playing,
        
        /// <summary>暂停状态 - 游戏暂停，保持当前状态</summary>
        Paused,
        
        /// <summary>游戏结束状态 - 显示分数、重置选项</summary>
        GameOver,
        
        /// <summary>错误状态 - XR 系统异常或其他错误</summary>
        Error
    }

    /// <summary>
    /// 游戏管理器 - MR MatchBlaster 的核心状态控制器
    /// 职责：
    /// 1. 管理游戏主状态流转（初始化 → 校准 → 准备 → 游戏 → 结束）
    /// 2. 协调各子系统的启动和关闭（空间系统、交互系统、匹配系统等）
    /// 3. 处理 XR 系统的生命周期事件
    /// 4. 提供统一的游戏事件接口
    /// </summary>
    public class GameManager : MonoBehaviour
    {
        #region 单例模式
        /// <summary>
        /// 单例实例 - 确保全局唯一的游戏管理器
        /// 使用单例是因为 GameManager 需要在整个游戏生命周期中保持唯一性
        /// </summary>
        public static GameManager Instance { get; private set; }
        #endregion

        #region 状态管理
        /// <summary>
        /// 当前游戏状态
        /// </summary>
        [SerializeField] private GameState _currentState = GameState.Initializing;
        
        /// <summary>
        /// 上一个游戏状态 - 用于状态回退和调试
        /// </summary>
        private GameState _previousState;
        
        /// <summary>
        /// 当前游戏状态（只读属性）
        /// </summary>
        public GameState CurrentState => _currentState;
        #endregion

        #region 配置数据
        /// <summary>
        /// 游戏配置数据 - 使用 ScriptableObject 实现数据驱动
        /// 包含游戏规则、性能设置、调试选项等
        /// </summary>
        [SerializeField] private GameConfig _gameConfig;
        
        /// <summary>
        /// 游戏配置数据（只读属性）
        /// </summary>
        public GameConfig GameConfig => _gameConfig;
        #endregion

        #region 系统引用
        /// <summary>
        /// 空间管理系统引用 - 负责空间锚点、房间边界等
        /// 使用接口实现依赖注入，便于测试和替换实现
        /// </summary>
        private ISpatialManager _spatialManager;
        
        /// <summary>
        /// 交互管理系统引用 - 负责手部追踪、发射逻辑等
        /// </summary>
        private IInteractionManager _interactionManager;
        
        /// <summary>
        /// 匹配系统引用 - 负责几何体匹配、消除逻辑等
        /// </summary>
        private IMatchSystem _matchSystem;
        
        /// <summary>
        /// 得分系统引用 - 负责分数计算、记录等
        /// </summary>
        private IScoreSystem _scoreSystem;
        #endregion

        #region 游戏事件
        /// <summary>
        /// 游戏状态改变事件 - 当游戏状态发生变化时触发
        /// 其他系统可以订阅此事件来响应状态变化
        /// </summary>
        public static event Action<GameState, GameState> OnGameStateChanged;
        
        /// <summary>
        /// 游戏开始事件 - 当游戏从 Ready 状态进入 Playing 状态时触发
        /// </summary>
        public static event Action OnGameStarted;
        
        /// <summary>
        /// 游戏结束事件 - 当游戏进入 GameOver 状态时触发
        /// </summary>
        public static event Action<int> OnGameEnded; // 参数为最终分数
        
        /// <summary>
        /// 游戏暂停事件 - 当游戏暂停或恢复时触发
        /// </summary>
        public static event Action<bool> OnGamePaused; // 参数为是否暂停
        
        /// <summary>
        /// 系统错误事件 - 当发生系统级错误时触发
        /// </summary>
        public static event Action<string> OnSystemError; // 参数为错误信息
        #endregion

        #region Unity 生命周期
        /// <summary>
        /// 初始化单例和基础设置
        /// </summary>
        private void Awake()
        {
            // 单例模式实现
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
                InitializeGameManager();
            }
            else
            {
                Destroy(gameObject);
            }
        }

        /// <summary>
        /// 开始游戏初始化流程
        /// </summary>
        private void Start()
        {
            StartGameInitialization();
        }

        /// <summary>
        /// 清理事件订阅
        /// </summary>
        private void OnDestroy()
        {
            if (Instance == this)
            {
                CleanupGameManager();
            }
        }
        #endregion

        #region 初始化方法
        /// <summary>
        /// 初始化游戏管理器
        /// 设置基础配置和依赖注入
        /// </summary>
        private void InitializeGameManager()
        {
            // 验证配置数据
            if (_gameConfig == null)
            {
                Debug.LogError("[GameManager] GameConfig 未设置！");
                ChangeState(GameState.Error);
                return;
            }

            // 初始化系统引用（依赖注入）
            InjectDependencies();
            
            Debug.Log("[GameManager] 游戏管理器初始化完成");
        }

        /// <summary>
        /// 依赖注入 - 获取各个子系统的引用
        /// 使用 Service Locator 模式或直接查找组件
        /// </summary>
        private void InjectDependencies()
        {
            // 注意：这里不使用 FindObjectOfType，而是通过 ServiceLocator 或预设引用
            // _spatialManager = ServiceLocator.Get<ISpatialManager>();
            // _interactionManager = ServiceLocator.Get<IInteractionManager>();
            // _matchSystem = ServiceLocator.Get<IMatchSystem>();
            // _scoreSystem = ServiceLocator.Get<IScoreSystem>();
            
            // 临时实现：直接查找（后续替换为依赖注入）
            Debug.Log("[GameManager] 依赖注入完成（临时实现）");
        }

        /// <summary>
        /// 开始游戏初始化流程
        /// 按顺序初始化各个子系统
        /// </summary>
        private void StartGameInitialization()
        {
            ChangeState(GameState.Initializing);
            
            // 异步初始化各个系统
            InitializeSystemsAsync();
        }

        /// <summary>
        /// 异步初始化各个子系统
        /// 避免阻塞主线程
        /// </summary>
        private async void InitializeSystemsAsync()
        {
            try
            {
                // 1. 初始化空间系统
                if (_spatialManager != null)
                {
                    // await _spatialManager.InitializeAsync();
                }
                
                // 2. 初始化交互系统
                if (_interactionManager != null)
                {
                    // await _interactionManager.InitializeAsync();
                }
                
                // 3. 初始化匹配系统
                if (_matchSystem != null)
                {
                    // await _matchSystem.InitializeAsync();
                }
                
                // 4. 初始化得分系统
                if (_scoreSystem != null)
                {
                    // await _scoreSystem.InitializeAsync();
                }
                
                // 初始化完成，进入空间校准状态
                ChangeState(GameState.SpatialCalibration);
            }
            catch (Exception ex)
            {
                Debug.LogError($"[GameManager] 系统初始化失败: {ex.Message}");
                OnSystemError?.Invoke(ex.Message);
                ChangeState(GameState.Error);
            }
        }
        #endregion

        #region 状态管理方法
        /// <summary>
        /// 改变游戏状态
        /// 包含状态验证和事件触发
        /// </summary>
        /// <param name="newState">新的游戏状态</param>
        public void ChangeState(GameState newState)
        {
            // 状态验证
            if (!IsValidStateTransition(_currentState, newState))
            {
                Debug.LogWarning($"[GameManager] 无效的状态转换: {_currentState} -> {newState}");
                return;
            }

            // 保存上一个状态
            _previousState = _currentState;
            _currentState = newState;

            // 触发状态改变事件
            OnGameStateChanged?.Invoke(_previousState, _currentState);
            
            // 处理状态进入逻辑
            OnStateEnter(_currentState);
            
            Debug.Log($"[GameManager] 状态改变: {_previousState} -> {_currentState}");
        }

        /// <summary>
        /// 验证状态转换是否有效
        /// 防止无效的状态跳转
        /// </summary>
        /// <param name="from">当前状态</param>
        /// <param name="to">目标状态</param>
        /// <returns>是否为有效转换</returns>
        private bool IsValidStateTransition(GameState from, GameState to)
        {
            // 错误状态可以从任何状态进入
            if (to == GameState.Error) return true;
            
            // 定义有效的状态转换规则
            return from switch
            {
                GameState.Initializing => to == GameState.SpatialCalibration || to == GameState.Error,
                GameState.SpatialCalibration => to == GameState.Ready || to == GameState.Error,
                GameState.Ready => to == GameState.Playing || to == GameState.Error,
                GameState.Playing => to == GameState.Paused || to == GameState.GameOver || to == GameState.Error,
                GameState.Paused => to == GameState.Playing || to == GameState.GameOver || to == GameState.Error,
                GameState.GameOver => to == GameState.Ready || to == GameState.Initializing || to == GameState.Error,
                GameState.Error => to == GameState.Initializing,
                _ => false
            };
        }

        /// <summary>
        /// 状态进入处理
        /// 根据不同状态执行相应的逻辑
        /// </summary>
        /// <param name="state">进入的状态</param>
        private void OnStateEnter(GameState state)
        {
            switch (state)
            {
                case GameState.SpatialCalibration:
                    StartSpatialCalibration();
                    break;
                case GameState.Ready:
                    PrepareGame();
                    break;
                case GameState.Playing:
                    StartGame();
                    break;
                case GameState.Paused:
                    PauseGame();
                    break;
                case GameState.GameOver:
                    EndGame();
                    break;
                case GameState.Error:
                    HandleError();
                    break;
            }
        }
        #endregion

        #region 游戏流程控制方法
        /// <summary>
        /// 开始空间校准
        /// 设置空间锚点和房间边界
        /// </summary>
        private void StartSpatialCalibration()
        {
            Debug.Log("[GameManager] 开始空间校准");
            // 启动空间校准流程
            // _spatialManager?.StartCalibration();
            
            // 临时：直接进入准备状态
            ChangeState(GameState.Ready);
        }

        /// <summary>
        /// 准备游戏
        /// 生成初始几何体，等待玩家开始
        /// </summary>
        private void PrepareGame()
        {
            Debug.Log("[GameManager] 准备游戏");
            // 重置游戏数据
            // _scoreSystem?.ResetScore();
            // _matchSystem?.ResetGame();
            // 生成初始几何体
            // _spatialManager?.SpawnInitialGeometry();
        }

        /// <summary>
        /// 开始游戏
        /// 激活交互系统，开始计时
        /// </summary>
        private void StartGame()
        {
            Debug.Log("[GameManager] 游戏开始");
            // 激活交互系统
            // _interactionManager?.EnableInteraction();
            // 开始计时
            // _scoreSystem?.StartTimer();
            
            OnGameStarted?.Invoke();
        }

        /// <summary>
        /// 暂停游戏
        /// 暂停所有系统，保持当前状态
        /// </summary>
        private void PauseGame()
        {
            Debug.Log("[GameManager] 游戏暂停");
            // 暂停各个系统
            // _interactionManager?.DisableInteraction();
            // _scoreSystem?.PauseTimer();
            
            OnGamePaused?.Invoke(true);
        }

        /// <summary>
        /// 结束游戏
        /// 停止所有系统，显示结果
        /// </summary>
        private void EndGame()
        {
            Debug.Log("[GameManager] 游戏结束");
            // 停止各个系统
            // _interactionManager?.DisableInteraction();
            // int finalScore = _scoreSystem?.GetFinalScore() ?? 0;
            
            int finalScore = 0; // 临时值
            OnGameEnded?.Invoke(finalScore);
        }

        /// <summary>
        /// 处理错误状态
        /// 显示错误信息，提供重启选项
        /// </summary>
        private void HandleError()
        {
            Debug.LogError("[GameManager] 进入错误状态");
            // 停止所有系统
            // _interactionManager?.DisableInteraction();
            // 显示错误 UI
        }
        #endregion

        #region 公共接口方法
        /// <summary>
        /// 开始游戏（从 Ready 状态）
        /// 供外部调用的游戏开始接口
        /// </summary>
        public void StartGameplay()
        {
            if (_currentState == GameState.Ready)
            {
                ChangeState(GameState.Playing);
            }
            else
            {
                Debug.LogWarning("[GameManager] 只能在 Ready 状态下开始游戏");
            }
        }

        /// <summary>
        /// 暂停/恢复游戏
        /// 供外部调用的暂停切换接口
        /// </summary>
        public void TogglePause()
        {
            if (_currentState == GameState.Playing)
            {
                ChangeState(GameState.Paused);
            }
            else if (_currentState == GameState.Paused)
            {
                ChangeState(GameState.Playing);
                OnGamePaused?.Invoke(false);
            }
        }

        /// <summary>
        /// 重启游戏
        /// 供外部调用的游戏重启接口
        /// </summary>
        public void RestartGame()
        {
            ChangeState(GameState.Ready);
        }

        /// <summary>
        /// 退出游戏
        /// 供外部调用的游戏退出接口
        /// </summary>
        public void QuitGame()
        {
            ChangeState(GameState.GameOver);
        }
        #endregion

        #region 清理方法
        /// <summary>
        /// 清理游戏管理器
        /// 取消事件订阅，释放资源
        /// </summary>
        private void CleanupGameManager()
        {
            // 清理事件订阅
            OnGameStateChanged = null;
            OnGameStarted = null;
            OnGameEnded = null;
            OnGamePaused = null;
            OnSystemError = null;
            
            Debug.Log("[GameManager] 游戏管理器清理完成");
        }
        #endregion
    }
}