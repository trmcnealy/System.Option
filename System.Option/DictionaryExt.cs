using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace System
{
    public static class DictionaryExt
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Pair<T1, T2> Insert<T1, T2>(this Dictionary<T1, T2> dictionary, Pair<T1, T2> keyValue)
        {

            if(!dictionary.ContainsKey(keyValue.First))
            {
                dictionary.Add(keyValue.First, keyValue.Second);
            }

            return new Pair<T1, T2>(keyValue.First, dictionary[keyValue.First]);
        }
    }
}