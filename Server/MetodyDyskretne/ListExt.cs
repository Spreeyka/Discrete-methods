using System;
using System.Collections.Generic;
using System.Text;

namespace MetodyDyskretne
{
    public static class ListExt
    {
        // O(1) 
        public static void RemoveBySwap<T>(this List<T> list, int index)
        {
            list[index] = list[list.Count - 1];
            list.RemoveAt(list.Count - 1);
        }
    }
}
