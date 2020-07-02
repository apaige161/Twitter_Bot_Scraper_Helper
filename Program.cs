using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Tweetinvi;
using Tweetinvi.Models;
using Tweetinvi.Parameters;
using System.Diagnostics;
using HtmlAgilityPack;
using System.Text.RegularExpressions;

namespace TwitterBotDotNetScraperHelper
{
    class Program
    {
        static void Main(string[] args)
        {
            //login
            //read passwords from file here using your own file path

            string pathOfApiKeys = @".\api_keys.txt";
            //read file and put contents into array
            string[] allKeys = File.ReadAllLines(pathOfApiKeys);

            string ApiKey = allKeys[0];
            string ApiKeySecret = allKeys[1];
            string AccessToken = allKeys[2];
            string AccessTokenSecret = allKeys[3];

            // Set up your credentials (https://apps.twitter.com)
            Auth.SetUserCredentials(ApiKey, ApiKeySecret, AccessToken, AccessTokenSecret);

            //Login
            var user = User.GetAuthenticatedUser();
            if (user != null)
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("Login Successful");
                Console.ResetColor();
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Could Not Login, Check Credentials or Internet Connection");
                Console.ResetColor();
            }
            Console.WriteLine("\n");


            Console.WriteLine("This program will post results and then post the new results every 4 hours");
            //post news headline now from scrapped website
            while (true)
            {
                //post news headline (popular) now from scrapped website

                //add time
                DateTime currentTime = DateTime.Now;
                DateTime newTime = DateTime.Now
                    //.AddDays()
                    //.AddHours()
                    .AddMinutes(4);

                //scrape data
                HtmlAgilityPack.HtmlWeb web = new HtmlAgilityPack.HtmlWeb();
                HtmlAgilityPack.HtmlDocument doc = web.Load("https://www.engadget.com/");
                //grab html from home page
                var NewsArticles = doc.DocumentNode.SelectNodes("//span[@class='th-underline']");

                //loop and print headlines
                int index = 1;
                List<string> newsList = new List<string>();
                foreach (var NewsArticle in NewsArticles)
                {
                    //add each article to a list
                    //trim the blank space
                    string atricleText = NewsArticle.InnerText.Replace("\n", "").Trim();
                    newsList.Add(atricleText);

                    if(index < 15)
                    Console.WriteLine($"TOP STORY: {atricleText}");

                    index++;
                }

                //post articles
                //TODO: grab all uppercase letters of the new article and add that word as a hastag
                //TODO: Post a random article out of the top few articles

                //Print hashtags to be posted
                string input = newsList[9];
                string firstWordOfArticle = input.Substring(0, input.IndexOf(" ")); // Result is "first word of article"
                string allHashtags = $"#Engadget #ProjectWebScrape #{firstWordOfArticle}";
                string textToTweet = $"TOP STORY: { input } {allHashtags}";
                Console.WriteLine("\n");
                Console.WriteLine($"Story to Publish: {textToTweet}");

                //shows dynamic hashtags
                Console.Write($"The Hashtags being sent with the tweet: ");
                Console.ForegroundColor = ConsoleColor.Blue;
                Console.WriteLine($"{allHashtags}");
                Console.ResetColor();

                //post a tweet
                Tweet.PublishTweet(textToTweet);
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("First TOP STORY posted");
                Console.ResetColor();

                TextWorker(newTime, currentTime, textToTweet);
                

                //wait and run again
                Console.WriteLine("Program will run again in 4");

                

                


            }//end of while loop
           





        }//end of main

        public static void TextWorker(DateTime newTime, DateTime currentTime, string textToTweet)
        {
            //datetime compare for while loop
            int timeCompare = DateTime.Compare(newTime, currentTime);

            /*  timeCompare:
             *  <0 − If date1 is earlier than date2
             *  0 − If date1 is the same as date2
             *  >0 − If date1 is later than date2
            */

            //waits until addTime is later than currentTime
            while (timeCompare > 0)
            {
                //end while loop, should end when timeCompare is changed
                Console.WriteLine("It is not time to post yet, the program will try again in 60 seconds...");
                Thread.Sleep(60000);   //wait for 60 seconds
                timeCompare = DateTime.Compare(newTime, DateTime.Now);

                /*****post tweet******/
                //runs program after while loop completes
                if (timeCompare < 0) //currentTime is later than addTime
                {
                    //post a tweet
                    Tweet.PublishTweet(textToTweet);
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine("First TOP STORY posted");
                    Console.ResetColor();
                    break;
                }
            }
        } //tweet text
    }
}
