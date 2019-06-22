using System.Collections.Generic;

namespace System
{
    public static class Extensions
    {

        // We cannot take credit for this brilliance but we can credit the author:
        // https://stackoverflow.com/a/30248074/7147234
        
        // Used to convert a list of list of anything into chunks determed by the size parameter
        public static List<List<T>> SplitList<T>(this List<T> me, int size = 3)
        {
            var list = new List<List<T>>();
            for (int i = 0; i < me.Count; i += size)
                list.Add(me.GetRange(i, Math.Min(size, me.Count - i)));
            return list;
        }


    }
}
