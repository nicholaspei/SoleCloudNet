using SolrNet;
using SolrNet.Cloud;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApplication1
{
    public class SolrCloudProvider:  ISolrOperationsProvider
    {
        public string LastOperation { get; set; }

        public string LastUrl { get; set; }

        public string Key {
            get { return "Fake"; }
        }

        public void Dispose() {
        }

        public SolrCloudState GetCloudState() {
            var replica1 = new SolrCloudReplica(true, true, "leader1", "http://172.17.3.138:7574/solr/gettingstarted_shard1_replica1");
            var replica2 = new SolrCloudReplica(true, false, "replica1", "http://172.17.3.138:8983/solr/gettingstarted_shard1_replica1");
            var replica3 = new SolrCloudReplica(false, false, "replica2", "http://172.17.3.137:8983/solr/gettingstarted_shard1_replica1");
            var replica4 = new SolrCloudReplica(true, true, "leader2", "http://172.17.3.137:7574/solr/gettingstarted_shard2_replica1");
            var replica5 = new SolrCloudReplica(true, false, "replica3", "http://172.17.3.138:8983/solr/gettingstarted_shard2_replica1");
            var replica6 = new SolrCloudReplica(false, false, "replica4", "http://172.17.3.137:8983/solr/gettingstarted_shard2_replica1");
            var shard1 = new SolrCloudShard(true, "shard1", null, null,
                new Dictionary<string, SolrCloudReplica>(StringComparer.OrdinalIgnoreCase) {
                    {replica1.Name, replica1},
                    {replica2.Name, replica2},
                    {replica3.Name, replica3}
                });
            var shard2 = new SolrCloudShard(true, "shard2", null, null,
                new Dictionary<string, SolrCloudReplica>(StringComparer.OrdinalIgnoreCase) {
                    {replica4.Name, replica4},
                    {replica5.Name, replica5},
                    {replica6.Name, replica6}
                });
            var collection1 = new SolrCloudCollection("gettingstarted", new SolrCloudRouter("implicit"),
                new Dictionary<string, SolrCloudShard>(StringComparer.OrdinalIgnoreCase) {
                    {shard1.Name, shard1},
                    {shard2.Name, shard2}
                });
            var collection2 = new SolrCloudCollection("gettingstarted", new SolrCloudRouter("implicit"),
                new Dictionary<string, SolrCloudShard>(StringComparer.OrdinalIgnoreCase) {
                    {shard1.Name, shard1},
                    {shard2.Name, shard2}
                });
            return new SolrCloudState(
                new Dictionary<string, SolrCloudCollection>(StringComparer.OrdinalIgnoreCase) {
                    {collection1.Name, collection1}                
                });
        }

        public void Init() {
        }

        public ISolrBasicOperations<T> GetBasicOperations<T>(string url) {
            LastUrl = url;
            return new FakeOperations<T>(this);          
        }

        public ISolrOperations<T> GetOperations<T>(string url) {
            LastUrl = url;
            return new FakeOperations<T>(this);          
        }
    }
}
