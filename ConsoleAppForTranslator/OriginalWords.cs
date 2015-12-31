using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections;
using System.Text.RegularExpressions;

namespace ConsoleAppForTranslator
{
    public class OriginalWords //Base class for encoder and decoder. Stores words from the strings
    {
        protected List<String> originalWords; // List which stores the original strings

        public OriginalWords(String line)
        {
            //Splitting the string and storing it in a list
            this.originalWords = new List<String>(Regex.Split(line, @"(?<=[.,;\\/ ])"));

            //originalWords.ForEach(i => Console.Write("{0}\n",i));
        }



    }
}
