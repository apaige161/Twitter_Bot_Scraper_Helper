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
using Tweetinvi.Core.Extensions;

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


            Console.WriteLine("This program will post results and then post the new results every 4 minutes");

            //post news headline now from scrapped website
            while (true)
            {
                //post news headline (popular starts at newsList[0], latest starts at newsList[9]) now from scrapped website

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

                //default article to post
                string articleToPost = newsList[0];

                //Post a random article out of the top few articles
                Random randomArticle = new Random();
                articleToPost = newsList[randomArticle.Next(15)];

                //print the random article in blue 
                Console.ForegroundColor = ConsoleColor.Blue;
                Console.WriteLine(articleToPost);
                Console.ResetColor();


                //grab all uppercase letters of the new article and add that word as a hastag
                string[] split = articleToPost.Split(' ');
                string[] listOfHashtags = new string[split.Length];
                for (int i = 0; i < split.Length; i++)
                {
                    if(split[i].Any(char.IsUpper))
                    {
                        listOfHashtags[i] = "#" + split[i] + " ";
                        //Console.WriteLine(split[i]);
                    }
                }
                //join the list together
                string generatedHashTags = string.Join("", listOfHashtags);
                //Console.WriteLine(generatedHashTags);

                //replace quotes and special char
                //fixed formatting issues but need to refactor LATER
                string cleanHashtags = generatedHashTags.Replace("’", "");
                string cleanHashtags1 = cleanHashtags.Replace("‘", "");
                string cleanHashtags2 = cleanHashtags1.Replace("'", "");



                //TODO: LATER: turns the text into just the quote 
                /*
                string[] resultApostStart = articleToPost.Split('‘');
                if(resultApostStart[1] != null)
                {
                    Console.WriteLine(resultApostStart[1]);
                }
                
                string[] resultApostEnd = articleToPost.Split('’');


                //and puts it into a string with no spaces
                */


                //Print hashtags to be posted
                //string input = articleToPost;
                //string firstWordOfArticle = input.Substring(0, input.IndexOf(" ")); // Result is "first word of article"
                string engadgetHashtags = $"#Engadget #ProjectWebScrape";
                Console.ForegroundColor = ConsoleColor.Green;
                string textToTweet = $"TOP STORY: { articleToPost } \n { engadgetHashtags } { cleanHashtags2 }";
                Console.ResetColor();
                Console.WriteLine("\n");
                Console.WriteLine($"Story to Publish: { textToTweet }");

                //shows dynamic hashtags
                Console.Write($"The Hashtags being sent with the tweet: ");
                Console.ForegroundColor = ConsoleColor.Blue;
                Console.WriteLine($"{ engadgetHashtags } { cleanHashtags2 }");
                Console.ResetColor();

                
                //post the news story
                    //this is here to post as soon as the program runs
                Tweet.PublishTweet(textToTweet);
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("First TOP STORY posted");
                Console.ResetColor();
                
                Wait(newTime, currentTime);
                

                //wait and run again
                Console.WriteLine("Program will run again in 4");

                

                



            }//end of while loop

        }//end of main

        public static void Wait(DateTime newTime, DateTime currentTime)
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
                
                Console.WriteLine("It is not time to post yet, the program will try again in 60 seconds...");
                Thread.Sleep(60000);   //wait for 60 seconds
                timeCompare = DateTime.Compare(newTime, DateTime.Now);

                /*****post tweet******/
                //runs program after while loop completes
                if (timeCompare < 0) //currentTime is later than addTime
                {
                    /*
                    //post a tweet
                    Tweet.PublishTweet(textToTweet);
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine("First TOP STORY posted");
                    Console.ResetColor();
                    */
                    break;
                }
            }
        } //tweet text

        public static void ArticleInQuotesHashtag (string _articleToPost)
        {
            //grab text inside quotes, join them as a string in a single word
            string[] split = _articleToPost.Split(' ');
            string[] splitIntoQuotes = new string[split.Length];

            //need conditional if inside quotes, 
                //do something

            for (int i = 0; i < split.Length; i++)
            {
                //print
                Console.WriteLine(splitIntoQuotes[i]);
                
            }
            //join the list together
            //string generatedHashTags = string.Join("", splitIntoQuotes);
        }
    }

}
