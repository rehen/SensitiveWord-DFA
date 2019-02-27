using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using System.IO;


namespace SensitiveWord_DFA
{
    

    class Program
    {
        static Hashtable sensitiveWordMap;
        static HashSet<string> sensitiveWord;
        static bool minMatch = true;
        static string sensitiveWordsLibPath = @"F:\wkpls\csharp\SensitiveWord-DFA\SensitiveWord-DFA\sensitive words.txt";

        static void Test() {
            MakeSensitiveWordMap();
            string test = "草你妈 三天之内杀了你";

            
            WordHandler h = new WordHandler(test);
            h.ChangeSensitiveWords();
            Console.WriteLine(h.newStr);
            
            
            
        }

        static void MakeSensitiveWordMap() {
            sensitiveWordMap = new Hashtable(sensitiveWord.Count);
            Hashtable newMap, nowMap;

            foreach(string word in sensitiveWord)
            {
                nowMap = sensitiveWordMap;
                for(int i = 0; i < word.Length; ++i)
                {
                    char ch = word[i];
                    if (nowMap.ContainsKey(ch))
                    {
                        nowMap = (Hashtable)nowMap[ch];
                    }
                    else
                    {
                        newMap = new Hashtable();
                        newMap.Add("isEnd", 0);
                        nowMap.Add(ch, newMap);
                        nowMap = newMap;
                    }

                    if (i == word.Length - 1)
                    {
                        nowMap["isEnd"] = 1;
                    }
                }
            }
        }

        public static int FindSensitiveWord(string txt, ref int startIndex) {
            Hashtable nowMap = sensitiveWordMap;

            bool end = false, found = false;
            int len = 0;

            for(int i = startIndex; i < txt.Length; ++i)
            {
                char ch = txt[i];
                if (!nowMap.Contains(ch)) {
                    if(!found)
                        continue;
                    else
                    {
                        if (end)
                            return len;
                        else
                        {
                            len = 0;
                            found = false;
                            nowMap = sensitiveWordMap;
                            --i;
                        }  
                    }
                }
                else
                {
                    if (!found)
                    {
                        found = true;
                        startIndex = i;
                    }
                    len += 1;
                    nowMap = (Hashtable) nowMap[ch];
                    if ((int)(nowMap["isEnd"]) == 1)
                    {
                        end = true;
                        if (minMatch)
                            return len;
                    }
                    
                    
                }
            }
            if (end)
                return len;
            else
                return 0;
        }


        static void Main(string[] args) {
            //prepare
            string str = File.ReadAllText(sensitiveWordsLibPath, Encoding.Default);
            string[] words = Regex.Split(str, @"\s+");
            sensitiveWord = new HashSet<string>(words);
            MakeSensitiveWordMap();

            //a new task
            string raw = Console.ReadLine();
            WordHandler handler = new WordHandler(raw);
            Console.WriteLine("\nafter processed:\n");
            handler.ChangeSensitiveWords();
            Console.WriteLine(handler.newStr);


            Console.ReadKey();
        }
    }



    class WordHandler
    {
        public string rawStr { get; private set; }
        public string newStr { get; private set; }
        public char replaceChar = '*';
        public HashSet<string> sensitiveWords = new HashSet<string>();

        public WordHandler(string str) {
            rawStr = str;
            newStr = str;
        }

        public void ChangeSensitiveWords() {
            int index = 0, len = 0;
            while (index + len < rawStr.Length)
            {

                index += len;
                len = Program.FindSensitiveWord(rawStr, ref index);
                //Console.WriteLine("index: {0}, len: {1}", index, len);
                if (len == 0)
                {
                    //Console.WriteLine("break");
                    break;
                }
                sensitiveWords.Add(rawStr.Substring(index, len));
            }
            ReplaceWords();
        }

        public void ReplaceWords() {
            foreach (string s in sensitiveWords)
            {
                //Console.WriteLine(s);
                string re = "";
                for (int i = 0; i < s.Length; i++)
                    re += replaceChar;
                newStr = newStr.Replace(s, re);
            }
        }


    }
}
