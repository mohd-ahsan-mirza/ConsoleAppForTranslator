using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace ConsoleAppForTranslator
{
    public class Decoder:OriginalWords //Decoder class
    {
        private Dictionary<String,String> keys; //Stores encrypted string as key and the decrypted string as value 

        // line=Text provided by the user. keys= translation guide provided by the user.
        public Decoder(Dictionary<String, String> keys,String line) : base(line)
        {
            this.keys = keys;
        }

        private string decode(String word,int previousWordIndex=2) // Decrypts single word
        {
            int index = previousWordIndex; // Was used for recursion. The initial index will always be 2 in current implementation 

            String translatedWord = ""; //The string that will be returned
            bool translated = false; //Checks whether the string is translated

            while(word.Length!=0) //The chars gets substracted from word as they get decrypted
            {
                //Executes until a segment of the word matches a key in keys
                while (!translated)
                {
                    String segment=""; //The segment of the word String

                    try {

                        segment = word.Substring(0, index);

                        if (keys.ContainsKey(segment)) //if segment is contained as key in keys
                        {
                            previousWordIndex = index; //The current index becomes previous

                            translated = true; // The segment has been translated successfully

                            string value;
                            keys.TryGetValue(segment, out value); //Decrypted string
                            translatedWord = translatedWord + value; //Concatenate in the string that will be returned
                        }
                        else
                            index++; // Increase the index by one (For changing the segment)
                        
                    }
                    catch(ArgumentOutOfRangeException e) //Decryption of the word failed
                    {
                        translated = true; //To get out of the loop
                        index--; // Before this operation the index = word.Length
                        translatedWord = translatedWord + word.Substring(0, index); //Concatenate the failed string
                    }
                }

                translated = false;//Continue operation for the rest of the string
                word = word.Substring(index); // Subtracting the string that was decrypted above
                index = 2; //Initial Index
            }
            return translatedWord;
            

        }

        public string startDecoding()
        {
            String translatedSentence = ""; //The decrypted string that will be returned 

            foreach(String temp in this.originalWords)
            {
                String word = temp;

                String start = ""; //The first char of word String
                String end = ""; //The last char of the word String

                if (word.Length > 1) 
                {
                    if (!Char.IsLetter(Convert.ToChar(word.Substring(0, 1))))//If the first char is not a letter
                    {
                        start = word.Substring(0, 1); //Store that char
                        word = word.Substring(1); //Subtract that char
                    }
                    //Same operation for last char as above
                    if (!Char.IsLetter(Convert.ToChar(word.Substring(word.Length - 1))))
                    {
                        end = word.Substring(word.Length - 1); 
                        word = word.Substring(0, word.Length - 1);
                    }
                }
                //Putting everything together
                translatedSentence = translatedSentence + start + this.decode(word) + end;
            }
            return translatedSentence;
        }
    }

    public class Encoder : OriginalWords
    {
  
        String wordsUsedForEncrytion; //Chars that will be used for encryption (Only letters)
        int maxencryptionChars; // Encryption strength. (Max chars for one single encrypted char)
        Dictionary<String, String> encryptionGuide; //keys=decrypted letters values=encrypted chars

        public Encoder(string line,int maxLength=5,string encryption="") : base(line)
        {
            // the encryption strength has to be minimum 5 and max 52
            maxLength = (maxLength < 5) ? 5 : maxLength;
            maxLength = (maxLength > 52) ? 52 : maxLength;
            //Ignoring any other chars other than alphabets for encryption
            encryption = (encryption.Length!=0) ? new string(encryption.Where(Char.IsLetter).ToArray()):encryption;

            if (encryption.Length < 2)//If the encryption chars were not provided (Minimum is 2)
            {
                const string allowedChars = "ABCDEFGHJKLMNOPQRSTUVWXYZabcdefghijkmnopqrstuvwxyz";
                maxLength = (maxLength>allowedChars.Length) ? allowedChars.Length-2 : maxLength;
                Random random = new Random();
                for (int run=0;run<maxLength;run++) { //chars from allowedChar=maxLength
                    
                    //Getting a single random char from the allowed chars above
                    String currentChar = allowedChars.Substring(random.Next(allowedChars.Length), 1);
                    while (encryption.Contains(currentChar))//Loops until it finds a unique char 
                        currentChar = allowedChars.Substring(random.Next(allowedChars.Length), 1);
                    
                    //Adds the unique char in the encryption string 
                    encryption = encryption + allowedChars.Substring(random.Next(allowedChars.Length),1);

                }
            }

            //Assigning private variables
            this.wordsUsedForEncrytion = encryption;
            this.maxencryptionChars = maxLength;
            //Calling function below
            this.encryptionGuide = this.generateEncryptionCodes();

            //Console.WriteLine("^^^^^^^^^^ EncryptionwWords:"+wordsUsedForEncrytion+"^^^^^^^^^^^");
        }

        //This function uses multithreading in case this.generateKeys() gets stuck and has be to restarted
        private Dictionary<String,String> generateEncryptionCodes(int time=1000)
        {
            //Minmimum time allowed for the operation to complete
            if (time < 1000)
                time = 1000;

            Dictionary<String,String> value = new Dictionary<String,String>();
            Thread t = new Thread(()=>value=this.generateKeys()); //Creates a new thread
            t.Start();//Starts a new thread

            if (!t.Join(time)) //give the operation (time)s to complete
            {
                t.Abort();//Abort the function in case it fails
                return this.generateEncryptionCodes(); //Start all over again
            }
            else
            {
                t.Abort();
                return value;
            }
        }

        /*This function does the heavy work for generating the keys.It can sometimes get stuck 
        /while generating really unique and random encryption keys. That is why the function above is used.*/
        private Dictionary<String,String> generateKeys()
        {
            Dictionary<String, String> result = new Dictionary<String, String>(); //Returned variable

            foreach(String word in this.originalWords)
            {
                char[] letters = word.ToCharArray(); //Splits the string word in a char array

                foreach(char letter in letters) //Indiviual chars in the array above
                {
                    //If the letter is a digit or letter(Only those chars are encrypted) and if it hasn't been encypted yet
                    if (Char.IsLetterOrDigit(letter) && !result.ContainsKey(letter.ToString()))
                    {

                        bool encryptionExist = true; //If the encrypted key for letter that gets generated already exists
                        String finalKey = "";

                        while (encryptionExist)
                        {
                            finalKey = ""; // The encryption key that will get generated

                            Random random = new Random();
                            //String length for the to be generated encryption key
                            int length = random.Next(2, this.maxencryptionChars + 1);
                            //Gets a random char from the encrytion chars used and add them in finalKey
                            for (int run = 0; run < length; run++) { finalKey = finalKey + this.wordsUsedForEncrytion.Substring(random.Next(this.wordsUsedForEncrytion.Length), 1); }

                            //Making sure that the generated key is unique enough so it can be decrypted later on
                            encryptionExist = result.Values.Any((x => Regex.IsMatch(x,finalKey) || Regex.IsMatch(finalKey,x)));
                        }
                        
                        result.Add(letter.ToString(), finalKey);

                    }

                }

            }
            return result;
        }

        //Reverses the key value relationship in encryptionGuide and returns the resultant for decryption
        public Dictionary<String,String> getTranslationGuide()
        {
            return this.encryptionGuide.ToDictionary(kp => kp.Value, kp => kp.Key);
        }

        public string startEncryption()
        {
            String result = ""; //Encrypted string that will be returned
            
            foreach(String words in this.originalWords) //Each string in the originalWords list
            {
                
                foreach(char ch in words.ToCharArray()) //Each char in the words string
                {
                    if (Char.IsLetterOrDigit(ch))
                    {
                        String temp;
                        //Keys already generated in the constructor
                        encryptionGuide.TryGetValue(ch.ToString(), out temp); 
                        result = result + temp;//Concatenating the values
                    }
                    else
                        result = result + ch.ToString();
                }
            }

            return result;
        }

    }

}
