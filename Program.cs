using System;
using System.Collections.Generic;
using Elasticsearch.Net;
using Nest;
using Newtonsoft.Json;

namespace LoadData
{
    public class Content
    {
        public int ContentId { get; set; }
        public DateTime PostDate { get; set; }
        public string ContentText { get; set; }
    }

    public class Recipes
    {
        public List<string> Directions { get; set; }
        public double Fat { get; set; }
        public DateTime Date { get; set; }
        public List<string> Categories { get; set; }
        public double Calories { get; set; }
        //public string Description { get; set; }
        public double Protein { get; set; }
        public double rating { get; set; }
        public string Title { get; set; }
        public List<string> Ingredients { get; set; }
        public double Sodium { get; set; }
    }

    public class Direction
    {
        public int DirectionNo { get; set; }
        public string DirectionName { get; set; }
    }

    public class Ingredients
    {
        public int Quantity { get; set; }
        public string Steps { get; set; }
    }
    class Program
    {
        public static Uri node;
        public static ConnectionSettings settings;
        public static ElasticClient client;

        static void Main(string[] args)
        {
            Program p = new Program();
            p.TestElasticSearch();
            Console.WriteLine("Done");
            Console.ReadLine();
        }

        private void TestElasticSearch()
        {
            var uris = new[]
            {
                new Uri("http://localhost:9200"),
                new Uri("http://localhost:9201"),
                new Uri("http://localhost:9202"),
            };

             var connectionPool = new SniffingConnectionPool(uris);
             settings = new ConnectionSettings(connectionPool)
                .DefaultIndex("recipes");

             client = new ElasticClient(settings);


            TestInsert();

            //TestTermQuery();

            //TestMatchPhrase();

            //TestFilter();
        }

        private void TestTermQuery()
        {
            var result = client.Search<Content>(s =>
                s.From(0).Size(10000).Type("content").Query(q => q.Term(t => t.ContentId, 2)));
            /*
            GET contentidx/content/_search
            {
              "query": {
                "match":{
                  "contentText":"Louis"
                }
              }
            }
             */

            string[] matchTerms =
            {
                "The quick",  // will find two entries.  Two with "the" and one with "quick"(but that has "the" as well with a score of 2)
                "Football",
                "Hockey",
                "Chicago Bears",
                "St. Louis"
            };

            // Match terms would come from what the user typed in
            foreach (var term in matchTerms)
            {
                result = client.Search<Content>(s =>
                   s
                   .From(0)
                   .Size(10000)
                   .Type("content")
                   .Query(q => q.Match(mq => mq.Field(f => f.ContentText).Query(term))));
                // print out the result.
            }
        }

        private void TestMatchPhrase()
        {
            // Exact phrase matching
            string[] matchPhrases =
            {
                "The quick",
                "Louis Blues",
                "Chicago Bears"
            };

            // Match terms would come from what the user typed in
            foreach (var phrase in matchPhrases)
            {
                var result = client.Search<Content>(s =>
                   s
                   .From(0)
                   .Size(10000)
                   .Type("content")
                   .Query(q => q.MatchPhrase(mq => mq.Field(f => f.ContentText).Query(phrase))));
                // print out the result.
            }
        }

        private void TestFilter()
        {
            var result = client.Search<Content>(s =>
                s
                .From(0)
                .Size(10000)
                .Type("content")
                .Query(q => q
                    .Bool(b => b
                        .Filter(filter => filter.Range(m => m.Field(fld => fld.ContentId).GreaterThanOrEquals(4)))
                        )
                    ));
            // print out the result.            
        }

        private async void TestInsert()
        {
            // Insert data
           
            var json = System.IO.File.ReadAllText("C:/Users/ehass/Documents/Dev/Data/test/sample data.json");

            IEnumerable<Recipes> results = JsonConvert.DeserializeObject<IEnumerable<Recipes>>(json);

            foreach (var item in results)
            {
                Console.WriteLine("added value - "+item.Title);
                
                client.IndexDocument(item);

                await client.IndexDocumentAsync(item);
            }

        }

        public object TestDeleteIndex()
        {
            var response = client.DeleteIndex("contentidx");
            return response;
        }

    }
}
