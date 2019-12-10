using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;

namespace TextComparer
{
   public class Text : INotifyPropertyChanged
    {
        private string _title;
        private string _content;

        public string Title
        {
            get { return _title; }
            set
            {
                _title = value;
                OnPropertyChanged("Title");
            }
        }

        public string Content
        {
            get { return _content; }
            set
            {
                _content = value;
                OnPropertyChanged("Content");
            }
        }
        public Dictionary<string, Word> Table { get; set; }

        private List<string> StopWords { get; set; }
        public List<Word> WordsNormal { get; set; }

        private string _sourceText { get; set; }

        public Text()
        {
            Table = new Dictionary<string, Word>(StringComparer.OrdinalIgnoreCase);
        }

        public void Normalize(List<string> list)
        {
            StopWords = list;
            _sourceText = Content;
            _sourceText = _sourceText.ToLower();

            Task<List<Word>> task = new Task<List<Word>>(() => NormalizeText(_sourceText));
            task.Start();
            task.Wait();
            WordsNormal = new List<Word>();
            WordsNormal = task.Result;

            //Task<Dictionary<string, Word>> TableTask = new Task<Dictionary<string, Word>>(() => GetWordsTable(task.Result));
            //TableTask.Start();
            //TableTask.Wait();

            //Table = TableTask.Result;
        }

        public List<Word> NormalizeText(string text)
        {
            //дополнить список разделителей
            char[] delimiterChars = { ' ', ',', '.', ':', ';', '\t', '(', ')', '{', '}', '"', '–', '\n' };
            //дополнить список слов

            List<string> words = new List<string>(text.Split(delimiterChars));
            foreach (string str in StopWords)
            {
                words.RemoveAll(cfg => cfg == str);
            }

            string reg = "[0-9]*";
            words = words
           .Select(x => Regex.Replace(x, reg, ""))
           .ToList();

            words.RemoveAll(cfg => cfg == "");

            List<Word> wordsList = new List<Word>();

            foreach (string str in words)
            {
                Word word = new Word();
                word.sourceWord = str;
                word.stemmedWord = Porter.TransformingWord(str);
                wordsList.Add(word);
            }

            return wordsList;
        }

        private Dictionary<string, Word> GetWordsTable(List<Word> words)
        {
            Dictionary<string, Word> Table = new Dictionary<string, Word>();

           for(int i = 0;  i < words.Count; i++)
           {
                words[i].stemmedWord = Porter.TransformingWord(words[i].sourceWord);
           }

            foreach (Word word in words)
            {
                if (Table.ContainsKey(word.stemmedWord))
                {
                    Table[word.stemmedWord].count++;
                }
                else
                {
                    Table.Add(word.stemmedWord, new Word { sourceWord=word.sourceWord,stemmedWord = word.stemmedWord,count=1});
                }
            }

            return Table;
        }

        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged([CallerMemberName]string prop = "")
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(prop));
        }
    }
}
