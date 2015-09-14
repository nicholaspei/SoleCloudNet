﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using System.Xml.XPath;
using SolrNet.Commands.Parameters;
using SolrNet.Impl;

namespace SolrNet.Cloud.CollectionsAdmin {
    public class SolrCollectionsAdmin : LowLevelSolr, ISolrCollectionsAdmin
    {
        private const string adminHandler = "/admin/collections";

        public SolrCollectionsAdmin(ISolrConnection connection, ISolrHeaderResponseParser headerParser)
            : base(connection, headerParser)
        {
        }

        public ResponseHeader CreateCollection(string collection, string routerName = null, int? numShards = null, string configName = null, string shards = null, int? maxShardsPerNode = null)
        {
            var solrParams = new SolrParams()
                .AddRequired("action", "create")
                .AddRequired("name", collection)
                .AddOptional("router.name", routerName)
                .AddOptional("numShards", numShards.ToString())
                .AddOptional("shards", shards)
                .AddOptional("maxShardsPerNode", maxShardsPerNode.ToString());

            return SendAndParseHeader(adminHandler, solrParams);
        }

        public ResponseHeader DeleteCollection(string collection) {
            return SendAndParseHeader(adminHandler, new SolrParams()
                .AddRequired("action", "delete")
                .AddRequired("name", collection));
        }

        public ResponseHeader CreateShard(string collection, string shard)
        {
            return SendAndParseHeader(adminHandler, new SolrParams()
                .AddRequired("action", "createshard")
                .AddRequired("collection", collection)
                .AddRequired("shard", shard));
        }

        public ResponseHeader DeleteShard(string collection, string shard)
        {
            return SendAndParseHeader(adminHandler, new SolrParams()
                .AddRequired("action", "deleteshard")
                .AddRequired("collection", collection)
                .AddRequired("shard", shard));
        }

        public ResponseHeader ReloadCollection(string collection) {
            return SendAndParseHeader(adminHandler, new SolrParams()
                .AddRequired("action", "reload")
                .AddRequired("name", collection));
        }

        public SolrCloudState GetClusterStatus(string collection, string shard = null) {
            var json = SendRaw(adminHandler, new SolrParams()
                .AddRequired("wt", "json")
                .AddRequired("action", "clustertsate")
                .AddOptional("shard", shard));

            var status = SolrCloudStateParser.Parse(json);

            return status;
        }

        public List<string> ListCollections() {
            var results = Send(adminHandler, new SolrParams()
                .AddRequired("action", "list"));

            var paramNodes = results.XPathSelectElements("response/arr[@name='collections']/str");

            return paramNodes.Select(n => n.Value).ToList();
        }
    }
}