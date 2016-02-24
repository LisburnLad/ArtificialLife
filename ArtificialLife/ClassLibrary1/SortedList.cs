using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

///
/// a list that will sort items as they are added and allow duplicate keys
/// taken from: 
/// http://stackoverflow.com/questions/11801314/equivalent-to-a-sorted-dictionary-that-allows-duplicate-keys
/// 

namespace ArtificialLife
{
  public class SortedList<TKey, TValue> : SortedSet<Tuple<TKey, TValue>>
      where TKey : IComparable
  {
    private class TupleComparer : Comparer<Tuple<TKey, TValue>>
    {
      public override int Compare(Tuple<TKey, TValue> x, Tuple<TKey, TValue> y)
      {
        if (x == null || y == null) return 0;

        // If the keys are the same we don't care about the order.
        // Return 1 so that duplicates are not ignored.
        return x.Item1.Equals(y.Item1)
            ? 1
            : Comparer<TKey>.Default.Compare(x.Item1, y.Item1);
      }      
    }

    public SortedList() 
    : base(new TupleComparer()) 
    { }

    public void Add(TKey key, TValue value)
    {
      Add(new Tuple<TKey, TValue>(key, value));
    }
  }
}
