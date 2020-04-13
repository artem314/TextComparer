using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TextComparer
{
   public class TextComparator 
    {
        private List<Word> _words1;
        private List<Word> _words2;

        public Dictionary<Word, double[]> Table;
        public TextComparator(List<Word>words1, List<Word> words2)
        {
            _words1 = words1;
            _words2 = words2;

            var WordsUnion = _words1.Union(_words2);
            WordsUnion = WordsUnion.Distinct();
            Table = new Dictionary<Word, double[]>();
            foreach (var word in WordsUnion)
            {
                var count1 = _words1.Where(cfg => cfg.stemmedWord == word.stemmedWord);
                var count2 = _words2.Where(cfg => cfg.stemmedWord == word.stemmedWord);

                Table.Add(new Word { stemmedWord = word.stemmedWord, sourceWord = word.sourceWord }, new double[] { count1.Count(), count2.Count() });
            }

        }

        public double CosMethod()
        {
            double scalar = 0;
            double amod = 0;
            double bmod = 0;

            foreach (var t in Table)
            {
                scalar = scalar + t.Value[0] * t.Value[1];
                amod = amod + Math.Pow(t.Value[0], 2);
                bmod = bmod + Math.Pow(t.Value[1], 2);
            }

            amod = Math.Sqrt(amod);
            bmod = Math.Sqrt(bmod);

            return  scalar / (amod * bmod);
        }

        public double CustomMethod()
        {
            IEnumerable<Word> WordsUnion = _words1.Union(_words2);
            WordsUnion = WordsUnion.Distinct();

            double max1st = Table.Values.Max(count => count[0]);
            double min1st = (max1st - 1 )/ 2;
            double max2nd = Table.Values.Max(count => count[1]);
            double min2nd = (max2nd - 1) / 2;

            double L1=0;
            double L2 = 0;

            foreach (var word in WordsUnion)
            {
                var count1 = _words1.Where(cfg => cfg.stemmedWord == word.stemmedWord);
                var count2 = _words2.Where(cfg => cfg.stemmedWord == word.stemmedWord);

                if (count1.Count() == 1 && count2.Count() == 1)
                    continue;


                if (count1.Count() == 0 || count2.Count() == 0)
                    continue;

                if (count1.Count() > min1st && count1.Count() <= max1st)
                {
                    L1 = L1 + count1.Count();
                }

                if (count2.Count() > min2nd && count2.Count() <= max2nd)
                {
                    L2 = L2 + count2.Count();
                }
            }

            L1 = L1 /Table.Count;
            L2 = L2 / Table.Count;

            return L1 + L2 - L1 * L2;
        }

    }
}
