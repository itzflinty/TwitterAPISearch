using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.IO;
using Tweetinvi;
using Newtonsoft.Json;
using System.Threading;

/*
    Tweetinvi Documentation - https://linvi.github.io/tweetinvi/dist/twitter-api-v2/search.html
    Basic setup of TwitterAPI V2.0- https://www.youtube.com/watch?v=aOlp3vXohB0
    Passing data into Json - https://stackoverflow.com/questions/16921652/how-to-write-a-json-file-in-c?answertab=active#tab-top
*/

namespace Synergi_nflint
{
    public class Program
    {
        // Set Up Credentials
        private static string APIKey = "JXkFJRkRuWxC7UpZkHuWW1VfI";
        private static string APISecret = "ohYvd81kYcTsU8HTSL1C1fjdfC6Lz7OUOzsAumbYJZcs7Uv1zw";
        private static string APIToken = "264278328-hFFF2I3OIlc03nE8sQGI3BkBFSN9i3bMMjgbipBV";
        private static string APITokenSecret = "r5VbtAlLg9y6Kc6hKgbN2lotnyWBkvF0jvlStOq9uMiUT";

        // Set up where tweets file will be output
        //Do this here so the Timestamp does not update over time causing many files to be created.
        private static readonly string FilePathArchivedTweets = @".\Archive\tweets_" + DateTime.Now.ToString("MM_dd_yyyy_HH_mm_ss") + ".json";

        static async Task Main(string[] args)
        {
            // Declare variables
            int TweetCount;
            string SearchTerm;

            List<TweetModel> tweetContents = new List<TweetModel>();

            // Create User to access Twitter Search API
            Logging.OutputLog("Information", "Setting Up User Credentials");
            TwitterClient UserClient = new TwitterClient(APIKey, APISecret, APIToken, APITokenSecret);

            Logging.OutputLog("Information", "Take in Search Term");
            Console.WriteLine("What Would you like to search for?");

            SearchTerm = Console.ReadLine();
            Logging.OutputLog("Information", "Chosen Search Term: " + SearchTerm);

            TweetCount = GetTweetCount();

            Logging.OutputLog("Information", "Scanning for tweets about " + SearchTerm);
            var searchResponse = await UserClient.SearchV2.SearchTweetsAsync(SearchTerm);

            var tweets = searchResponse.Tweets;

            Logging.OutputLog("Information", "Take in Search Term");
            for (int i = 0; i < tweets.Length && i < TweetCount; i++)
            {
                tweetContents.Add(new TweetModel()
                {
                    //Select Items of Tweet to Archive
                    id = i + 1,
                    createdOn = tweets[i].CreatedAt.ToString(),
                    tweetText = tweets[i].Text
                }); ; ;

            }
            //Check if API was able to find tweets
            if (tweetContents.Count == 0)
            {
                Console.WriteLine("Unable to find tweets about {0}", SearchTerm);
                TweetCount = 0;
            }
            Logging.OutputLog("Information", "Serializing Tweets");
            string json = JsonConvert.SerializeObject(tweetContents, Formatting.Indented);

            Logging.OutputLog("Information", "Writing Tweets to Archive");
            try
            {
                // Write Json output to file
                File.WriteAllText(FilePathArchivedTweets, json);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Failed to Write to Archive");
                Logging.OutputLog("Warning", "Failed to Write to Archive");
                Logging.OutputLog("Warning", ex.Message);
                throw;
            }


            Logging.OutputLog("SUCCESS", "Successfully Archived Tweets");
            Console.WriteLine("{0} Tweets about {1} have been archived", TweetCount, SearchTerm);
            Thread.Sleep(2000);
        }

        public static int GetTweetCount()
        {
            Logging.OutputLog("Information", "Take in Number of Tweets to recieve");
            for (int attempted = 0; attempted < 3; attempted++)
            {
                // Twitter API can only pull up to 100 tweets
                Console.WriteLine("How many tweets? (1 - 100)");
                try
                {
                    var tweetCount = int.Parse(Console.ReadLine());
                    Logging.OutputLog("Information", tweetCount + " tweets requested");
                    // Check the user wants at least 1 tweet
                    if (tweetCount < 1)
                    {
                        Console.WriteLine("Chosen value is below minimum search amount.");
                        Console.WriteLine("Value changed to 1.");
                        tweetCount = 1;
                    }
                    else if (tweetCount > 100)
                    {
                        Console.WriteLine("Chosen value is above maximum search amount.");
                        Console.WriteLine("Value changed to 100.");
                        tweetCount = 100;
                    }
                    return tweetCount;
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Value entered was not a valid number");
                    Logging.OutputLog("Warning", ex.Message);
                }
            }
            throw new AggregateException();
        }
    }
}