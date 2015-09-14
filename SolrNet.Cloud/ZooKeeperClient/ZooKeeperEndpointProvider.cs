// 

using System;
using System.Collections.Generic;
using System.Net;

namespace SolrNet.Cloud.ZooKeeperClient {
    public  class ZooKeeperEndpointProvider {

        private const int port = 2181;
        private const int retryCeiling = 10;
        private readonly TimeSpan defaultbackoffInterval = new TimeSpan(0, 2, 0);
        private readonly List<string> ips = new List<string> { "172.17.3.138", "172.17.3.137", "172.17.3.136" };
        private ZooKeeperEndpoints zkEndpoints = null;

        public ZooKeeperEndpointProvider() {
            zkEndpoints = new ZooKeeperEndpoints(ips.ConvertAll(x => IPAddress.Parse(x))
             .ConvertAll(y => new IPEndPoint(y, port)));
        }

        public ZooKeeperEndpoint GetActiveEndPoint() {
            zkEndpoints.GetNextAvailableEndpoint();

            return zkEndpoints.CurrentEndPoint;
        }
    }
}