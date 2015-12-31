using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleAppForTranslator
{
    class main
    {
        static void Main(string[] args)
        {
            //new OriginalWords("hooray /r/dailyprogrammer!");

            /*
            var dic = new Dictionary<String, String> { {"GgG","a" } , {"GggGg","d"} , { "GggGG","e"} };
            Decoder decoder = new Decoder(dic, "GGGgGGG");
            Console.WriteLine(decoder.decode("GggGgGgG"));
            */

            /*
            var dic = new Dictionary<String, String> { {"GgG","a" } , {"GggGg","d"} , {"GggGG","e"} ,
                                                        {"GGGgg","g"} , {"GGGgG","h"} , {"GGGGg","i"} ,
                                                        {"GGGGG","l"} , {"ggg","m"} , {"GGg","o"} ,
                                                        {"Gggg","p"} , {"gG","r"} , {"ggG","y" } };

            Decoder decoder = new Decoder(dic,
                "GGGgGGGgGGggGGgGggG /gG/GggGgGgGGGGGgGGGGGggGGggggGGGgGGGgggGGgGggggggGggGGgG!");

            Console.WriteLine(decoder.startDecoding());
            */

            /*
            for (int run = 0; run < 10; run++)
            {
                //Encoder encoder = new Encoder("Hello, world!", 5, encryption: "g$G");
                Encoder encoder = new Encoder("Hello, world!");
                 Dictionary <String, String> dict = encoder.getTranslationGuide();
                foreach (KeyValuePair<string, string> pair in dict)
                {
                    Console.WriteLine("{0}: {1}", pair.Key, pair.Value);
                }
                Console.WriteLine("-----------------------------------------------");
                Console.WriteLine("Encrypted: " + encoder.startEncryption());
                Console.WriteLine("++++++++++++++++++++++++++");
                Decoder decoder = new Decoder(encoder.getTranslationGuide(), encoder.startEncryption());
                Console.WriteLine("Decrypted: " + decoder.startDecoding());
            }
            */

            
            Test test=new Test(500, "quotes.txt");
            List<String> failed = test.performTest();
            test.printResult(failed);
            Console.WriteLine(test.estimateAccuracy(failed,5));
            

            //Console.WriteLine(Test.averageAccuracy(20, 50, "quotes.txt", 3));
        }
    }

}
