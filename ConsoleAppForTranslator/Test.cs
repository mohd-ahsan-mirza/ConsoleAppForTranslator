using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using System.Threading;

namespace ConsoleAppForTranslator
{
    //Test class
    public class Test
    {

        private int noOfTests;//No of tests
        List<String> data = new List<String>(); //The list of test strings

        public Test(int no, String filename)
        {
            //Assigning variables
            noOfTests = no;
            data = new List<String>();

            string line;
            int currentElement = 0;

            // Read the file and addd it line by line.
            System.IO.StreamReader file =
               new System.IO.StreamReader(filename);
            while ((line = file.ReadLine()) != null)
            {
                if (!line.Contains("*"))
                    //Concatenting the string until a new one starts(Marked by*)
                    this.data[currentElement] = this.data.ElementAt(currentElement) + line;
                else
                {
                    //Adding a new string in the list and the index of that string
                    currentElement = this.data.Count;
                    this.data.Add("");
                }
            }
            file.Close();
        }

        //Returns a random index of the test string in the list 
        private int randomNumberGenerator(HashSet<int> numbersGeneratedPreviously, Random random)
        {
            int result = -1;

            //Generates a unique random number
            do
                result = random.Next(0, this.data.Count);
            while (numbersGeneratedPreviously.Contains(result));

            return result;
        }
        
        //Function for calling the Encoder class
        private String encoder(String original,int maxlength,out Dictionary<String, String> dictionary,string encryptionLetters ="")
        {
            Encoder e = new Encoder(original, maxlength, encryptionLetters);
            dictionary = e.getTranslationGuide();

            return e.startEncryption();
        }

        //Function for calling the Decoder class
        private String decoder(Dictionary<String,String> translationGuide,string originalWord)
        {
            Decoder d = new Decoder(translationGuide, originalWord);

            return d.startDecoding();
        }

        //Test Method. Runs one single test
        private List<String> singleTest(Random random,HashSet<int> quoteIndexesUsed,List<String> failedTests)
        {
           
            // Once all the test strings in the list have been used 
            quoteIndexesUsed = (quoteIndexesUsed.Count == this.data.Count) ? new HashSet<int>() : quoteIndexesUsed;
            int quoteIndex = this.randomNumberGenerator(quoteIndexesUsed, random);//Index of the test string
            String quote = this.data.ElementAt(quoteIndex); //Test string

            Dictionary<String, String> translationGuide;
            //Encrypting the test string with random strength
            string encryptedString = this.encoder(quote, random.Next(5, 52), out translationGuide);

            string decryptedString = this.decoder(translationGuide, quote);//Decyrpting the encrypted test string

            //Comparing the original and the decrypted string.
            try
            {
                Assert.AreEqual(quote, decryptedString);
            }
            catch (AssertionException e)//In case unit test fails
            {
                //Console.WriteLine("Comes in here");
                failedTests.Add(e.Message);//Adds in the failedTests list that was passed as argument
            }

            return failedTests; //Returns the list that was passed as argument
        }


        public List<String> performTest()
        {
            Random random = new Random();
            HashSet<int> quoteIndexesUsed = new HashSet<int>();//For indexes that have been used
            List<String> failedTests=new List<String>();//Failed test messages list

            Task[] tasks = new Task[this.noOfTests];
            for (int run = 0; run < this.noOfTests; run++)
            {
                //Creates threads equal to number of tests to be performed
                tasks[run] = Task.Factory.StartNew(() =>
                {
                    Thread thread = new Thread(() => this.singleTest(random, quoteIndexesUsed, failedTests));
                    thread.Start();
                    thread.Join();
                });
                
            }
            while (tasks.Any(t => !t.IsCompleted)) { } //spin wait

            return failedTests;

        }

        public void printResult(List<String> failedTests)
        {
            if (failedTests.Count != 0)
            {
                Console.WriteLine("Sorry :( No of tests failed:" + failedTests.Count + "\n @@@@@@@@@@@@@@@@@@@@@@@@@@@@");

                failedTests.ForEach(i => Console.WriteLine("****************\n{0}*****************", i));
            }
            else
                Console.WriteLine("Congrats! The Decoder and Encoder passed all tests");
        }

        //Estimates the accuracy of the Decoder and Encoder class for a given no of tests in percentage 
        public double estimateAccuracy(List<String> failedTests,int precision)
        {
            precision = (precision<2) ? 2:precision;

            return Math.Round(100-((double)failedTests.Count / (double)this.noOfTests * 100),precision);
        }

        //Average accuracy for a given sample size
        public static double averageAccuracy(int sampleSize,int noOfTests,string filename,int precision)
        {
            precision = (precision < 2) ? 2 : precision;

            double sum = 0;

            for (int run = 0; run < sampleSize; run++)
            {
                Test test = new Test(noOfTests, filename);
                sum = sum + test.estimateAccuracy(test.performTest(), precision);
            }

            return Math.Round(sum / (double)sampleSize, precision);
        }
        

    }
}
