using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TextComparer
{
  public  class Word : IEquatable<Word>
    {
        public bool Equals(Word other)
        {

            //Check whether the compared object is null. 
            if (Object.ReferenceEquals(other, null)) return false;

            //Check whether the compared object references the same data. 
            if (Object.ReferenceEquals(this, other)) return true;

            //Check whether the products' properties are equal. 
            return stemmedWord.Equals(other.stemmedWord);
        }

        // If Equals() returns true for a pair of objects  
        // then GetHashCode() must return the same value for these objects. 

        public override int GetHashCode()
        {

            //Get hash code for the Name field if it is not null. 
            int hashProductName = stemmedWord == null ? 0 : stemmedWord.GetHashCode();


            //Calculate the hash code for the product. 
            return hashProductName;
        }

        public string sourceWord {get;set;}
        public string stemmedWord { get; set; }
        public int count { get; set; }

    }
}
