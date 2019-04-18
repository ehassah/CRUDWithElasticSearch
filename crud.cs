using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Elasticsearch.Net;
using Nest;

namespace LoadData
{
    class ConnectionToES
    {
        //for establishing connection to ElasticSearch
        public static ElasticClient EsClient()
        {
            ConnectionSettings connectionSettings;
            ElasticClient elasticClient;
            connectionSettings = new ConnectionSettings(new Uri("http://localhost:9200/"));
            elasticClient = new ElasticClient(connectionSettings);
            return elasticClient;
        }
    }
    class Crud
    {
        //getting top 1000 recipies
        public static List<Recipes> GetAllRecipies()
        {
            //getting all the records
            List<Recipes> result = (List<Recipes>)ConnectionToES.EsClient().Search<Recipes>(s => s
           .Index("recipes")
           .Type("recipes")
           .From(0)
           .Size(1000)
           .Query(q => q.MatchAll())).Documents;

            foreach (var item in result)
            {
                Console.WriteLine(item.Title);
            }
            return result;
        }

        public static void MatchText()
        {
            List<Recipes> result = (List<Recipes>)ConnectionToES.EsClient().Search<Recipes>(s => s
           .Index("recipes")
           .Type("recipes")
           .From(0)
           .Size(1000)
           .Query(q => q.MatchAll())).Documents;
            string[] matchTerms =
            {
                "Cucumber-Yogurt Salad with Mint ",  // will find two entries.  Two with "the" and one with "quick"(but that has "the" as well with a score of 2)
                "Roast Beef Salad with Cabbage and Horseradish ",
                "Asian Noodles with Barbecued Duck Confit ",
                "Sausage Fennel Stuffing ",
                "Cranberry, Quince, and Pearl Onion Compote "
            };

            // Match terms would come from what the user typed in
            foreach (var term in matchTerms)
            {
                result = (List<Recipes>)ConnectionToES.EsClient().Search<Recipes>(s =>
                   s
                   .From(0)
                   .Size(10000)
                   .Type("recipies")
                   .Query(q => q.Match(mq => mq.Field(f => f.Title).Query(term))));
                // print out the result.
            }
        }
    }
}
