﻿using System;
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

        private List<int[]> Combinations = new List<int[]>();

        void combinationUtil(int[] arr, int[] data, int start, int end, int index, int r)
        {
            if (index == r)
            {
                int[] tmpArr = new int[data.Length];
                for (int j = 0; j < r; j++)
                {
                    tmpArr[j] = data[j];
                }
                    
                Combinations.Add(tmpArr);
                return;
            }

            for (int i = start; i <= end && end - i + 1 >= r - index; i++)
            {
                data[index] = arr[i];
                combinationUtil(arr, data, i + 1,end, index + 1, r);
            }
        }

        void Combination(int[] arr, int n, int r)
        {
            int[] data = new int[r];
            combinationUtil(arr, data, 0, n - 1, 0, r);
        }
        /// <summary>
        /// проводит частотный анализ слов двух текстов, на выходе % совпавших
        /// </summary>
        /// <param name=""></param>
        private async void CompareText()
        {
            if (TextsList.Count < 2)
            {
                MessageBox.Show("Нужно выбрать как минимум 2 текста");
                return;
            }

            int[] n = new int[TextsList.Count];
            k = 0;
            foreach (Text text in TextsList)
            {
                text.Normalize(StopWords);
                n[k] = k;
                k++;
            }

            Combination(n, n.Length, 2);

            for (int i = 0;i < Combinations.Count;i++)
            {
                string Statistic = "";
                string writePath = "";
                int[] indexes = Combinations[i];
                TextComparator textComparator = new TextComparator(TextsList[indexes[0]].WordsNormal, TextsList[indexes[1]].WordsNormal);

                Task<double> tCos = Task<double>.Run(()=> textComparator.CosMethod());
                Task<double> tCustom = Task<double>.Run(() => textComparator.CustomMethod());

                await Task.WhenAll(new[] { tCos, tCustom });

                double pCos = tCos.Result;//  textComparator.CosMethod();
                double CustomMethod = tCustom.Result;
                
                Dictionary<Word, double[]> Table = textComparator.Table;

                foreach(var stat in Table)
                {
                    Statistic = Statistic + stat.Key.sourceWord + "    " + stat.Value[0] + "   " + stat.Value[1] + '\n';
                }
                writePath = writePath + TextsList[indexes[0]].Title +" " +TextsList[indexes[1]].Title + ".txt";
                try
                {
                    FileStream fs = new FileStream(writePath, FileMode.Create);
                    using (StreamWriter sw = new StreamWriter(fs))
                    {
                        //sw.Write("Оригинальный метод: p=" + percentSame + '\n');
                        sw.WriteLine("Количество слов");
                        sw.WriteLine(TextsList[indexes[0]].Title +" : " + TextsList[indexes[0]].WordsNormal.Count);
                        sw.WriteLine(TextsList[indexes[1]].Title + " : " + TextsList[indexes[1]].WordsNormal.Count);
                        sw.Write("Метод Косинусов: p=" + pCos.ToString("0.000") + "\n");
                        sw.Write("Авторский метод: p=" + CustomMethod.ToString("0.000") + "\n\n");
                        sw.Write(Statistic);
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                }

            }

            MessageBox.Show("Сравнение завершено");
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
