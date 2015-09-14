using System.Collections.Generic;
using Microsoft.Practices.ServiceLocation;
using SolrNet.Attributes;
using SolrNet.Cloud;
using SolrNet.Cloud.ZooKeeperClient;
using CloudStartup = SolrNet.Cloud.Startup;
using SolrNet;
using System;
using SolrNet.Commands.Parameters;
using System.Threading;

namespace ConsoleApplication1
{
    class Program {
        private static SolrCloudOperations<SerialNavigation> operations;
        static void Main(string[] args)
        {
            var endpointProvider = new ZooKeeperEndpointProvider();
            var currentpoint = endpointProvider.GetActiveEndPoint();
            var provider = new SolrCloudStateProvider(currentpoint.ServerAddress.ToString());           
            var opProvider = new OperationsProvider();
            for (var i = 0; i < 1000; i++) {              
                Console.WriteLine("Start at:" + DateTime.Now);
                              
                    provider.Init("serialnavigation"); 
                    if(operations==null) 
                     operations = new SolrCloudOperations<SerialNavigation>(provider, opProvider, "serialnavigation");

                    var results = operations.Query(new SolrQuery("id:1655"), new QueryOptions
                    {
                        Rows = 1,
                        Start = 0
                    });

                    var id = results[0].Id;
                    Console.WriteLine(id);
            }
             

                 // ------------------------------------------------------------------------------------

                //    Console.WriteLine("3 at:" + DateTime.Now);
                //    CloudStartup.Init<SerialNavigation>(provider, "serialnavigation");

                //    var solr = ServiceLocator.Current.GetInstance<ISolrOperations<SerialNavigation>>();
                   
                //        try
                //        {
                //    var results = solr.Query(new SolrQuery("id:1655"), new QueryOptions
                //    {
                //        Rows = 1,
                //        Start = 0
                //    });
                //   var  id = results[0].Id;            
                //    Console.WriteLine(id);
                //}
                //      catch (Exception ex)
                //      {
                //          Console.WriteLine(ex.StackTrace);
                //          Console.ReadKey();
                //      }
            
         
         
                Console.WriteLine("End at:" + DateTime.Now);
            
            Console.ReadKey();
        }
    }
    public class FakeEntity
    {
        [SolrField("id")]
        public string Id { get; set; }

        [SolrField("carid")]
        public int CarId { get; set; }

        [SolrField("cityId")]
        public int CityId { get; set; }

        [SolrField("dealers")]
        public string Dealers { get; set; } 
    }

    public class SerialNavigation
    {
        [SolrUniqueKey("id")]
        public int Id { get; set; }

        [SolrField("serialName")]
        public string SerialName { get; set; }

        [SolrField("html")]
        public string Html { get; set; }

    }

    public class OperationsProvider : ISolrOperationsProvider
    {
        public ISolrBasicOperations<T> GetBasicOperations<T>(string url)
        {
            return SolrNet.SolrNet.GetBasicServer<T>(url);
        }

        public ISolrOperations<T> GetOperations<T>(string url)
        {
            return SolrNet.SolrNet.GetServer<T>(url);
        }
    }
}
