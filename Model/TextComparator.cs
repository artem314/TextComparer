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

        public Dictionary<Word, int[]> Table;
        public TextComparator(List<Word>words1, List<Word> words2)
        {
            _words1 = words1;
            _words2 = words2;
        }

        public double CosMethod()
        {
            var WordsUnion = _words1.Union(_words2);
           
            WordsUnion = WordsUnion.Distinct();

            Table = new Dictionary<Word, int[]>();

            foreach(var word in WordsUnion)
            {
                var count1 = _words1.Where(cfg => cfg.stemmedWord == word.stemmedWord);
                var count2 = _words2.Where(cfg => cfg.stemmedWord == word.stemmedWord);

                Table.Add(new Word { stemmedWord = word.stemmedWord,sourceWord=word.sourceWord } , new int[] { count1.Count(), count2.Count() });
            }

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

    }
}
