using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace TextComparer
{
   public class NormText
    {
        private List<string> StopWords { get; set; }
        public Dictionary<string, int> Table { get; set; }
        private string _sourceText { get; set; }

        public NormText(List<string> list,string SourceText)
        {
            StopWords = list;
            Table = new Dictionary<string, int>();
            _sourceText = SourceText;
            Normalize();
        }

        private void Normalize()
        {
            Task<List<string>> task = new Task<List<string>>(()=> NormalizeText(_sourceText));
            task.Start();
            task.Wait();

            Task<Dictionary<string, int>> TableTask = new Task<Dictionary<string, int>>(()=> GetWordsTable(task.Result));
            TableTask.Start();
            TableTask.Wait();

            Table = TableTask.Result;
         
        }

        private List<string> NormalizeText(string text)
        {
            //дополнить список разделителей
            char[] delimiterChars = { ' ', ',', '.', ':', ';', '\t', '(', ')', '{', '}', '"', '-', '\n' };
            //дополнить список слов

            List<string> words = new List<string>(text.Split(delimiterChars));

            foreach (string str in StopWords)
            {
                words.RemoveAll(cfg => cfg == str);
            }

            string reg = "[0-9]*";

            var numb = words
           .Select(x => Regex.Replace(x, reg, ""))
           .ToList();

            words.RemoveAll(cfg => cfg == "");

            return words;
        }

        private Dictionary<string, int> GetWordsTable(List<string> words)
        {
            Dictionary<string, int> Table = new Dictionary<string, int>();

            foreach (string word in words)
            {
                if (Table.ContainsKey(word))
                {
                    Table[word]++;
                }
                else
                {
                    Table.Add(word, 1);
                }
            }

            return Table;
        }


    }
}
