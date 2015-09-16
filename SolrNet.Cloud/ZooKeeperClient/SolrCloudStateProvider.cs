using System;
using System.IO;
using System.Text;
using Enyim.Caching;
using ZooKeeperNet;

namespace SolrNet.Cloud.ZooKeeperClient {
    public class SolrCloudStateProvider : ISolrCloudStateProvider, IWatcher {
        private static readonly string SECTION_NAME = "enyim.com/memcached";
        public SolrCloudStateProvider(string zooKeeperConnection)
        {
            if (string.IsNullOrEmpty(zooKeeperConnection))
                throw new ArgumentNullException("zooKeeperConnection");
            this.zooKeeperConnection = zooKeeperConnection;
            syncLock = new object();
            Key = zooKeeperConnection;
        }

        private bool isDisposed;

        private bool isInitialized;

        private SolrCloudState state;

        private readonly object syncLock;

        private IZooKeeper zooKeeper;

      

        private readonly string zooKeeperConnection;

        public string Key { get; private set; }

        public void Dispose()
        {
            lock (syncLock)
                if (!isDisposed && zooKeeper != null)
                {
                    zooKeeper.Dispose();
                    isDisposed = true;
                }
        }

        public SolrCloudState GetCloudState() {
            return state;
        }

        public void Init()
        {
            lock (syncLock)
                if (!isInitialized)
                {
                    Update();
                    isInitialized = true;
                }
        }

        public void Init(string collection) {
            lock (syncLock)
                if (!isInitialized)
                {
                    Update(collection);
                    isInitialized = true;
                }
        }

        void IWatcher.Process(WatchedEvent @event)
        {
            if (@event.Type == EventType.NodeDataChanged)
                lock (syncLock)
                    try
                    {
                        Update();
                    }
                    catch { }
        }

        private void Update()
        {
            //if (zooKeeper == null)
            //    zooKeeper = new ZooKeeper(
            //        zooKeeperConnection, 
            //        TimeSpan.FromSeconds(10), this);
            //var currentState = zooKeeper.State.State;
            //state = SolrCloudStateParser.Parse(
            //    Encoding.Default.GetString(
            //        zooKeeper.GetData("/collections/dealerprice/state.json", true, null)));
        //    state=SolrCloudStateParser.Parse(GetState());
        }

        private void Update(string collection)
        {         
            //if (zooKeeper == null)
            //    zooKeeper = new ZooKeeper(
            //        zooKeeperConnection,
            //        TimeSpan.FromSeconds(10), this);
            //var data = Encoding.Default.GetString(
            //  zooKeeper.GetData(string.Format("/collections/{0}/state.json", collection), true, null));
            var data = GetJson();        
            state = SolrCloudStateParser.Parse(data);
            
            //    state=SolrCloudStateParser.Parse(GetState());
        }

        private string GetJson() {
            var internalContainer = new MemcachedClient(SECTION_NAME);
            var text = internalContainer.Get("solrcloud-state");
            if(text==null)
            { 
            var baseDirectory = AppDomain.CurrentDomain.BaseDirectory;
             text = System.IO.File.ReadAllText(baseDirectory+"\\clusterstate.json");
            }
            return text.ToString();
        }
    }
}
