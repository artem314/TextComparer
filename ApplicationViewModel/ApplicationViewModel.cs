using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Collections.ObjectModel;
using System.Windows;
using System.Text.RegularExpressions;
using System.IO;
using System.Collections;

namespace TextComparer
{
    class ApplicationViewModel : INotifyPropertyChanged
    {
         
        IFileService fileService;
        IDialogService dialogService;

        public ObservableCollection<Text> TextsList { get; set; } 
        private Text _selectedText { get; set; }
        public Text SelectedText
        {
            get { return _selectedText; }
            set
            {
                _selectedText = value;
                OnPropertyChanged("SelectedText");
            }
        }

        private RelayCommand openCommand;
        public RelayCommand OpenCommand
        {
            get
            {
                return openCommand ??
                  (openCommand = new RelayCommand(async obj =>
                  {
                      try
                      {
                          if (dialogService.OpenFileDialog() == true)
                          {
                              var text = await fileService.OpenAsync(dialogService.FilePath);
                              TextsList.Add(text);
                          }
                      }
                      catch (Exception ex)
                      {
                          dialogService.ShowMessage(ex.Message);
                      }
                  }));
            }
        }


        private RelayCommand removeCommand;
        public RelayCommand RemoveCommand
        {
            get
            {
                return removeCommand ??
                  (removeCommand = new RelayCommand(obj =>
                  {
                      if(SelectedText!=null)
                      TextsList.Remove(SelectedText);
                  }));
            }
        }

        

        private List<string> StopWords { get; set; }

        private static bool NextCombination(IList<int> num, int n, int k)
        {
            bool finished;

            var changed = finished = false;

            if (k <= 0) return false;

            for (var i = k - 1; !finished && !changed; i--)
            {
                if (num[i] < n - 1 - (k - 1) + i)
                {
                    num[i]++;

                    if (i < k - 1)
                        for (var j = i + 1; j < k; j++)
                            num[j] = num[j - 1] + 1;
                    changed = true;
                }
                finished = i == 0;
            }

            return changed;
        }

        private static IEnumerable Combinations<T>(IEnumerable<T> elements, int k)
        {
            var elem = elements.ToArray();
            var size = elem.Length;

            if (k > size) yield break;

            var numbers = new int[k];

            for (var i = 0; i < k; i++)
                numbers[i] = i;

            do
            {
                yield return numbers.Select(n => elem[n]);
            } while (NextCombination(numbers, size, k));
        }

        private double VectorMethod(List<int> vect1, List<int> vect2)
        {
            double scalar = 0;
            double amod = 0;
            double bmod = 0;

            for(int i =0;i<vect1.Count;i++)
            {
                scalar = scalar + vect1[i] * vect2[i];

                amod = amod + Math.Pow(vect1[i], 2);
                bmod = bmod + Math.Pow(vect2[i], 2);
            }
            amod = Math.Sqrt(amod);
            bmod = Math.Sqrt(bmod);

            return  scalar / (amod * bmod);
         
        }

        /// <summary>
        /// проводит частотный анализ слов двух текстов, на выходе % совпавших
        /// </summary>
        /// <param name=""></param>
        private void CompareText()
        {
            if (TextsList.Count < 2)
            {
                MessageBox.Show("Нужно выбрать как минимум 2 текста");
                return;
            }
                
            List<NormText> NormalizedTexts = new List<NormText>();

            foreach (Text text in TextsList)
            {
                NormText normText = new NormText(StopWords,text.Content);
                NormalizedTexts.Add(normText);
            }

            int k = 2;
            int[] n = new int[NormalizedTexts.Count];
            for(int i=0;i<n.GetLength(0);i++)
            {
                n[i] = i;
            }

            int[][] combs = new int[1][];
            combs[0] = new int[] { 0, 1 }; 
            //k = 0;
            //foreach (IEnumerable<int> i in Combinations(n, k))
            //{
            //    combs[k] = new int[2];
            //    combs[k] = i;
            //    k++;
            //}
               

            for(int i = 0;i < combs.Length;i++)
            {
                string Statistic = "";
                string writePath = "";
                int s = 0;// количество ненулевых строк
                List<double> p = new List<double>();
                int[] indexes = combs[i];

                List<int> vector1 = new List<int>();
                List<int> vector2 = new List<int>();

                foreach (var word in NormalizedTexts[indexes[0]].Table)
                {
                    var Table2 = NormalizedTexts[indexes[1]].Table;
                    if (Table2.ContainsKey(word.Key))
                    {
                        Statistic = Statistic + word.Key + '\t' + word.Value + " " + Table2[word.Key] + '\n';
                        double pi = (double)Math.Min(word.Value, Table2[word.Key]) / (double)Math.Max(word.Value, Table2[word.Key]);
                        vector1.Add(word.Value);
                        vector2.Add(Table2[word.Key]);
                        p.Add(pi);

                        s++;
                    }
                }

                double pf = 0.3;

                List<double> pSelected = new List<double>(p.FindAll(cfg => cfg <= pf));
                double percentSame = (double)pSelected.Count * 100 / s;

                writePath = writePath + TextsList[indexes[0]].Title +" " +TextsList[indexes[1]].Title + ".txt";
                try
                {
                    FileStream fs = new FileStream(writePath, FileMode.Create);
                    using (StreamWriter sw = new StreamWriter(fs))
                    {
                        sw.Write("Оригинальный метод: p=" + percentSame + '\n');
                        sw.Write("Метод Косинусов: p=" + VectorMethod(vector1,vector2) + '\n');
                        sw.Write("Количество слов" + '\n' + NormalizedTexts[indexes[0]].Table.Count + '\n');
                        sw.Write(NormalizedTexts[indexes[1]].Table.Count + "\n\n");
                        sw.Write(Statistic);
                    }
                    Console.WriteLine("Запись выполнена");
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                }

            }

            MessageBox.Show("Сравнение завершено");
            //foreach (var word in NormalizedTexts[0].Table)
            //{
            //    checkWords(NormalizedTexts, word.Key);
            //    if (Table2.ContainsKey(word.Key))
            //    {
            //        Statistic = Statistic + word.Key + "\t\t\t\t" + word.Value + " " + Table2[word.Key] + '\n';
            //        double pi = (double)Math.Min(word.Value, Table2[word.Key]) / (double)Math.Max(word.Value, Table2[word.Key]);
            //        p.Add(pi);
            //        s++;
            //    }
            //}

            //try
            //{
            //    using (StreamWriter sw = new StreamWriter(writePath, false, System.Text.Encoding.UTF8))
            //    {
            //        sw.WriteLine(Statistic);
            //    }
            //    Console.WriteLine("Запись выполнена");
            //}
            //catch (Exception e)
            //{
            //    Console.WriteLine(e.Message);
            //}

            //double pf = 0.5;

            //List<double> pSelected = new List<double>(p.FindAll(cfg => cfg <= pf));
            //double percentSame = (double)pSelected.Count * 100 / s;

        }

    private int k { get; set; }
    private RelayCommand _compareCommand;
    public RelayCommand CompareCommand
        {
            get
            {
                return _compareCommand ??
                  (_compareCommand = new RelayCommand(obj =>
                  {
                      CompareText();             
                  }));
            }
        }
    
        //private string[] words;

        private async Task LoadWordListAsync()
        {
            string path = "StopWords.txt";

            using (StreamReader sr = new StreamReader(path, System.Text.Encoding.UTF8))
            {
                string line;
                while ((line = await sr.ReadLineAsync()) != null)
                {
                    StopWords.Add(line);
                }
            }

            //words = new string[221];

            //using (StreamReader sr = new StreamReader(path, System.Text.Encoding.UTF8))
            //{
            //    int k = 0;
            //    string line;
            //    while ((line = await sr.ReadLineAsync()) != null)
            //    {
            //        words[k] = line;
            //        k++;
            //    }
            //}

            //IEnumerable<string> duplicates = words.Intersect(words);
            //try
            //{
            //    using (StreamWriter sw = new StreamWriter(path1, true, System.Text.Encoding.UTF8))
            //    {
            //        foreach(string str in duplicates)
            //        {
            //            await sw.WriteLineAsync(str);
            //        }

            //    }
            //}
            //catch (Exception e)
            //{
            //    Console.WriteLine(e.Message);
            //}

        }
        public ApplicationViewModel(IDialogService dialogService, IFileService fileService)
        {
            StopWords = new List<string>();
            TextsList = new ObservableCollection<Text>();
            LoadWordListAsync();

            this.dialogService = dialogService;
            this.fileService = fileService;

        }

        //static int[] GetPrefix(string s)
        //{
        //    int[] result = new int[s.Length];
        //    result[0] = 0;
        //    int index = 0;

        //    for (int i = 1; i < s.Length; i++)
        //    {
        //        while (index >= 0 && s[index] != s[i]) { index--; }
        //        index++;
        //        result[i] = index;
        //    }

        //    return result;
        //}

        //static int FindSubstring(string pattern, string text)
        //{
        //    int res = 0;

        //    //res = i - index + 1  чтобы прикрутить позиции

        //    int[] pf = GetPrefix(pattern);
        //    int index = 0;

        //    for (int i = 0; i < text.Length; i++)
        //    {
        //        while (index > 0 && pattern[index] != text[i])
        //        {
        //            index = pf[index - 1];
        //        }
        //        if (pattern[index] == text[i])
        //            index++;
        //        if (index == pattern.Length)
        //        {
        //             index = 0;
        //             res++;
        //        }
        //    }

        //    return res;
        //}


        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged([CallerMemberName]string prop = "")
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(prop));
        }
    }
}
