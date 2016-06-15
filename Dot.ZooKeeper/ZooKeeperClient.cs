using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Threading;
using Dot.Collections.Concurrent;
using Dot.Extension;
using Dot.Pattern;
using Dot.Threading.Atomic;
using Dot.ZooKeeper.Subscribe;
using log4net;
using Org.Apache.Zookeeper.Data;
using ZooKeeperNet;

namespace Dot.ZooKeeper
{
    public sealed class ZooKeeperClient : IWatcher, IDisposable
    {
        #region Fields

        private static readonly object PROCESS_EVENT_LOCK = new object();
        private static readonly object WAIT_KEEPER_STATE_LOCK = new object();
        private string _connectionString;
        private int _sessionTimeoutMs;
        private int _connectTimeoutMs;
        private AtomicEnum<KeeperState> _connectionState = new AtomicEnum<KeeperState>(KeeperState.Unknown);
        private AtomicReference<ZooKeeperNet.ZooKeeper> _connection = new AtomicReference<ZooKeeperNet.ZooKeeper>(null);
        private AtomicBoolean _shutdown = new AtomicBoolean(true);
        private ConcurrentHashSet<IStateListener> _stateListeners = new ConcurrentHashSet<IStateListener>();
        private ConcurrentDictionary<string, List<IChildListener>> _childListeners = new ConcurrentDictionary<string, List<IChildListener>>();
        private ConcurrentDictionary<string, List<IDataListener>> _dataListeners = new ConcurrentDictionary<string, List<IDataListener>>();
        private ZooKeeperEventExecutor _eventExecutor = new ZooKeeperEventExecutor(true);
        private ILog _logger = LogManager.GetLogger("ZooKeeperClient");

        #endregion

        #region Props

        public static ZooKeeperClient Instance
        {
            get
            {
                if (Singleton<ZooKeeperClient>.Instance == null)
                {
                    var config = ConfigurationManager.GetSection("zookeeper") as ZooKeeperConfig;
                    config = config ?? ZooKeeperConfig.Default;

                    var client = new ZooKeeperClient(config.ConnectionString, config.SessionTimeoutMs, config.ConnectTimeoutMs);
                    if (Singleton<ZooKeeperClient>.Replace(client) == false)
                        client.Dispose();
                }

                return Singleton<ZooKeeperClient>.Instance;
            }
        }

        public bool IsConnected { get { return _connectionState.Value == KeeperState.SyncConnected; } }

        #endregion

        #region Ctors

        private ZooKeeperClient(string connectionString, int sessionTimeoutMs = 20000, int connectTimeoutMs = int.MaxValue)
        {
            _connectionString = connectionString;
            _sessionTimeoutMs = sessionTimeoutMs;
            _connectTimeoutMs = connectTimeoutMs;
            _shutdown.Set(false);
            this.Connect();
        }

        #endregion

        #region Methods

        /// <summary>
        /// 连接到服务器（线程安全）
        /// </summary>
        private void Connect()
        {
            _logger.Info("Connect to zookeeper server");
            var newConnection = new ZooKeeperNet.ZooKeeper(_connectionString, TimeSpan.FromMilliseconds(_sessionTimeoutMs), this);
            if (_connection.CompareAndSet(null, newConnection) == false)
                newConnection.Dispose();

            if (this.WaitUntilConnected() == false)
            {
                this.Dispose();
                _logger.Error("Unable to connect to zookeeper server within timout: " + _connectTimeoutMs);
                throw new Exception("Unable to connect to zookeeper server within timout: " + _connectTimeoutMs);
            }
        }

        /// <summary>
        /// 断开与服务器的连接（线程安全）
        /// </summary>
        private void Disconnect()
        {
            _logger.Info("Disconnect to zookeeper server");
            var currentConnection = _connection.Value;
            if (currentConnection != null && _connection.CompareAndSet(currentConnection, null))
                currentConnection.Dispose();
        }

        /// <summary>
        /// 重新连接到服务器（线程安全）
        /// </summary>
        private void Reconnect()
        {
            _logger.Info("Reconnect to zookeeper server");
            this.Disconnect();
            this.Connect();
        }

        #endregion

        #region Util methods

        private bool WaitUntilKeeperState(KeeperState keeperState)
        {
            lock (WAIT_KEEPER_STATE_LOCK)
            {
                var elapsed = 0;
                while (_connectionState.Value != keeperState)
                {
                    Thread.Sleep(1000);
                    elapsed += 1000;
                    if (elapsed >= _connectTimeoutMs)
                        return false;
                }
                return true;
            }
        }

        private bool WaitUntilConnected()
        {
            return this.WaitUntilKeeperState(KeeperState.SyncConnected);
        }

        private T RetryUntilConnected<T>(Func<T> func)
        {
            while (true)
            {
                try
                {
                    return func();
                }
                catch (ZooKeeperNet.KeeperException.ConnectionLossException)
                {
                    Console.WriteLine("ConnectionLossException");
                    Thread.Yield();
                    this.WaitUntilConnected();
                }
                catch (ZooKeeperNet.KeeperException.SessionExpiredException)
                {
                    Console.WriteLine("SessionExpiredException");
                    Thread.Yield();
                    this.WaitUntilConnected();
                }
                catch
                {
                    throw;
                }
            }
        }

        private void RetryUntilConnected(Action action)
        {
            while (true)
            {
                try
                {
                    action();
                    return;
                }
                catch (ZooKeeperNet.KeeperException.ConnectionLossException)
                {
                    Console.WriteLine("ConnectionLossException");
                    Thread.Yield();
                    this.WaitUntilConnected();
                }
                catch (ZooKeeperNet.KeeperException.SessionExpiredException)
                {
                    Console.WriteLine("SessionExpiredException");
                    Thread.Yield();
                    this.WaitUntilConnected();
                }
                catch
                {
                    throw;
                }
            }
        }

        #endregion

        #region ZooKeeper api wrap

        /// <summary>
        /// 创建节点。
        /// 
        /// 该方法处理的契约：
        /// 1. 创建之前将判断节点是否存在（不触发监听）。如果不存在则创建；如果存在则不做任何操作，即使 data 或 mode 不一样。
        /// 2. 如果连接断开或连接过期，将一直重试直到连接成功。
        /// </summary>
        /// <param name="path">节点路径</param>
        /// <param name="data">节点数据</param>
        /// <param name="mode">节点持久类型（永久或瞬时）</param>
        /// <returns>已创建的节点路径</returns>
        public string Create(string path, byte[] data, CreateMode mode)
        {
            return this.RetryUntilConnected(() =>
            {
                if (this.Exists(path, false) == false)
                    return _connection.Value.Create(path, data, Ids.OPEN_ACL_UNSAFE, mode);
                else
                    return path;
            });
        }

        public string CreateOrReplace(string path, byte[] data, CreateMode mode)
        {
            if (this.Exists(path, false) == true)
                this.Delete(path);

            return this.Create(path, data, mode);
        }

        /// <summary>
        /// 创建瞬时节点
        /// </summary>
        /// <param name="path">节点路径</param>
        /// <param name="data">节点数据</param>
        /// <returns>节点路径</returns>
        public string CreateEphemeral(string path, byte[] data)
        {
            return this.Create(path, data, CreateMode.Ephemeral);
        }

        /// <summary>
        /// 创建瞬时节点，节点路径末尾自动追加序列号
        /// </summary>
        /// <param name="path">节点路径</param>
        /// <param name="data">节点数据</param>
        /// <returns>节点路径</returns>
        public string CreateEphemeralSequential(string path, byte[] data)
        {
            return this.Create(path, data, CreateMode.EphemeralSequential);
        }

        /// <summary>
        /// 创建永久节点
        /// </summary>
        /// <param name="path">节点路径</param>
        /// <param name="data">节点数据</param>
        /// <returns>节点路径</returns>
        public string CreatePersistent(string path, byte[] data)
        {
            return this.Create(path, data, CreateMode.Persistent);
        }

        /// <summary>
        /// 创建永久节点，节点路径末尾自动追加序列号
        /// </summary>
        /// <param name="path">节点路径</param>
        /// <param name="data">节点数据</param>
        /// <returns>节点路径</returns>
        public string CreatePersistentSequential(string path, byte[] data)
        {
            return this.Create(path, data, CreateMode.PersistentSequential);
        }

        /// <summary>
        /// 删除节点。
        /// 
        /// 该方法处理的契约：
        /// 1. 如果删除成功，返回 true
        /// 2. 如果捕获 NoNodeException 异常，返回 false
        /// 3. 如果捕获其它异常，抛出异常
        /// 4. 如果连接断开或连接过期，将一直重试直到连接成功。
        /// </summary>
        /// <param name="path">待删除的节点路径</param>
        /// <returns>是否删除成功</returns>
        public bool Delete(string path)
        {
            return this.RetryUntilConnected(() =>
            {
                try
                {
                    _connection.Value.Delete(path, -1);
                    return true;
                }
                catch (KeeperException.NoNodeException)
                {
                    return false;
                }
            });
        }

        /// <summary>
        /// 递归删除节点及其所有子节点。
        /// </summary>
        /// <param name="path">待删除的节点路径</param>
        /// <returns>是否删除成功</returns>
        public bool DeleteRecursive(string path)
        {
            IEnumerable<string> children;
            try
            {
                children = this.GetChildren(path, false);
            }
            catch (KeeperException.NoNodeException)
            {
                return true;
            }

            foreach (string subPath in children)
            {
                if (this.DeleteRecursive(path + "/" + subPath) == false)
                    return false;
            }

            return this.Delete(path);
        }

        /// <summary>
        /// 判断节点是否存在。
        /// </summary>
        /// <param name="path">节点路径</param>
        /// <param name="watch">是否监听</param>
        /// <returns></returns>
        public bool Exists(string path, bool watch)
        {
            return this.GetStat(path, watch) != null;
        }

        /// <summary>
        /// 获取节点的子节点列表
        ///  
        /// 该方法处理的契约：
        /// 1. 如果节点存在，则返回节点的子节点枚举数。
        /// 2. 如果节点不存在，将抛出异常。
        /// 3. 如果连接中断，将一直重试连接成功。
        /// </summary>
        /// <param name="path">节点路径</param>
        /// <param name="watch">是否监视</param>
        /// <returns>子节点枚举数</returns>
        public IEnumerable<string> GetChildren(string path, bool watch)
        {
            return this.RetryUntilConnected(() => _connection.Value.GetChildren(path, watch));
        }

        /// <summary>
        /// 获取节点状态。
        /// 
        /// 该方法处理的契约：
        /// 1. 如果节点存在，则返回节点状态。
        /// 2. 如果节点不存在，则返回 null。
        /// 3. 如果连接中断，将一直重试连接成功。
        /// </summary>
        /// <param name="path">节点路径</param>
        /// <param name="watch">是否监视</param>
        /// <returns>节点状态</returns>
        public Stat GetStat(string path, bool watch)
        {
            return this.RetryUntilConnected(() => _connection.Value.Exists(path, watch));
        }

        /// <summary>
        /// 获取节点数据。
        /// 
        /// 该方法处理的契约：
        /// 1. 如果节点存在，则返回节点数据。
        /// 2. 如果节点不存在，则抛出异常。
        /// 3. 如果指定了 Stat 实例，节点的状态信息将保存于该 Stat 实例中
        /// 4. 如果连接中断，将一直重试连接成功。
        /// </summary>
        /// <param name="path">节点路径</param>
        /// <param name="watch">是否监视</param>
        /// <param name="stat">状态实例</param>
        /// <returns>节点数据</returns>
        public byte[] GetData(string path, bool watch, Stat stat)
        {
            return this.RetryUntilConnected(() => _connection.Value.GetData(path, watch, stat));
        }

        /// <summary>
        /// 设置节点数据。
        /// 
        /// 该方法处理的契约：
        /// 1. 如果节点存在，当 version == -1 时，直接替换节点数据；当 version 为其它值时，则 version 相等才替换。
        /// 2. 如果节点不存在，则抛出异常
        /// 3. 如果连接中断，将一直重试连接成功。
        /// </summary>
        /// <param name="path">节点路径</param>
        /// <param name="data">节点数据</param>
        /// <param name="version">版本号</param>
        /// <returns>节点状态信息</returns>
        public Stat SetData(string path, byte[] data, int version = -1)
        {
            return this.RetryUntilConnected(() => _connection.Value.SetData(path, data, version));
        }

        /// <summary>
        /// 获取节点的创建时间
        /// </summary>
        /// <param name="path">节点路径</param>
        /// <returns>创建时间</returns>
        public long GetCreateTime(string path)
        {
            var stat = this.GetStat(path, false);
            return stat == null ? -1 : stat.Ctime;
        }

        #endregion

        #region Subscribe event listener

        public void SubscribeStateListener(IStateListener listener)
        {
            _stateListeners.TryAdd(listener);
        }

        public void UnsubscribeStateListener(IStateListener listener)
        {
            _stateListeners.TryRemove(listener);
        }

        public List<string> SubscribeChildListener(IChildListener listener)
        {
            _childListeners.GetOrAdd(listener.GroupPath, new List<IChildListener>())
                           .Add(listener);

            return this.GetChildren(listener.GroupPath, true).ToList();
        }

        public void UnsubscribeChildListener(IChildListener listener)
        {
            _childListeners.GetOrAdd(listener.GroupPath, new List<IChildListener>())
                           .Remove(listener);
        }

        public void SubscribeDataListener(IDataListener listener)
        {
            _dataListeners.GetOrAdd(listener.ServicePath, new List<IDataListener>())
                          .Add(listener);
        }

        public void UnsubscribeDataListener(IDataListener listener)
        {
            _dataListeners.GetOrAdd(listener.ServicePath, new List<IDataListener>())
                          .Remove(listener);
        }

        #endregion

        #region Process state changed event

        private void ProcessStateChanged(WatchedEvent @event)
        {
            _logger.Info("ZooKeeper state changed, current state = " + @event.State);
            if (_shutdown.Value == true) return;
            if (_connectionState.Value == @event.State) return;

            try
            {
                _connectionState.Set(@event.State);
                this.FireStateChangedEvent(@event.State);
                if (@event.State == KeeperState.Expired)
                {
                    this.Reconnect();
                    this.FireNewSessionEvent();
                }
            }
            catch
            {
                throw;
            }
        }

        private void FireStateChangedEvent(KeeperState state)
        {
            foreach (var listener in _stateListeners)
            {
                Action<ZooKeeperEvent> handler = e => listener.OnStateChanged(state);
                var description = string.Format("KeeperState changed to {0}, send to {1}", state, listener);
                var @event = new ZooKeeperEvent(description, handler);
                _eventExecutor.Add(@event);
            }
        }

        private void FireNewSessionEvent()
        {
            foreach (var listener in _stateListeners)
            {
                Action<ZooKeeperEvent> handler = e => listener.OnNewSession();
                var description = string.Format("New session event send to {0}", listener);
                var @event = new ZooKeeperEvent(description, handler);
                _eventExecutor.Add(@event);
            }
        }

        #endregion

        #region Process children changed event

        private void ProcessChildrenChanged(WatchedEvent @event)
        {
            List<IChildListener> listeners;
            _childListeners.TryGetValue(@event.Path, out listeners);
            this.FireChildrenChangedEvent(@event.Path, listeners);
        }

        private void FireChildrenChangedEvent(string dir, IEnumerable<IChildListener> listeners)
        {
            var children = _connection.Value.GetChildren(dir, true).Select(child => dir + "/" + child).ToList();
            foreach (var listener in listeners)
            {
                Action<ZooKeeperEvent> handler = e =>
                {
                    Console.WriteLine(e.Description);
                    listener.OnChildrenChanged(children);
                };

                var description = string.Format("Children of path {0} changed, send to {1}", dir, listener);
                var @event = new ZooKeeperEvent(description, handler);
                _eventExecutor.Add(@event);
            }
        }

        #endregion

        #region Process data changed event

        private void ProcessDataChanged(WatchedEvent @event)
        {
            List<IDataListener> listeners;
            _dataListeners.TryGetValue(@event.Path, out listeners);
            this.FireDataChangedEvent(@event.Path, listeners);
        }

        private void FireDataChangedEvent(string path, IEnumerable<IDataListener> listeners)
        {
            var bytes = _connection.Value.GetData(path, true, null);
            foreach (var listener in listeners)
            {
                Action<ZooKeeperEvent> handler = e =>
                {
                    Console.WriteLine(e.Description);
                    listener.OnDataChanged(path, bytes);
                };

                var description = string.Format("data of node {0} changed, send to {1}", path, listener);
                var @event = new ZooKeeperEvent(description, handler);
                _eventExecutor.Add(@event);
            }
        }

        #endregion

        #region IWatcher members

        public void Process(WatchedEvent @event)
        {
            _logger.Info("Process event, type = " + @event.Type);

            lock (PROCESS_EVENT_LOCK)
            {
                if (_shutdown.Value == true)
                    return;

                bool stateChanged = string.IsNullOrEmpty(@event.Path);
                bool childrenChanged = @event.Type == EventType.NodeChildrenChanged || @event.Type == EventType.NodeCreated || @event.Type == EventType.NodeDeleted;
                bool dataChanged = @event.Type == EventType.NodeDataChanged;

                if (stateChanged)
                    this.ProcessStateChanged(@event);
                if (childrenChanged)
                    this.ProcessChildrenChanged(@event);
                if (dataChanged)
                    this.ProcessDataChanged(@event);
            }
        }

        #endregion

        #region IDisposable members

        public void Dispose()
        {
            if (_shutdown.CompareAndSet(false, true))
            {
                _eventExecutor.Shutdown();
                this.Disconnect();
            }
        }

        #endregion
    }
}