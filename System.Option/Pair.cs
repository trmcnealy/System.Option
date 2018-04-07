using System.Diagnostics;

namespace System
{
    [DebuggerDisplay("First = {First}, Second = {Second}")]
    public class Pair<TTy1, TTy2>
    {
        public TTy1 First;  // the first stored value
        public TTy2 Second; // the second stored value

        public Pair(TTy1 first, TTy2 second)
        {
            First = first;
            Second = second;
        }

        public Pair(ValueTuple<TTy1, TTy2> valueTuple)
        {
            First  = valueTuple.Item1;
            Second = valueTuple.Item2;
        }

        public Pair(Tuple<TTy1, TTy2> tuple)
        {
            First  = tuple.Item1;
            Second = tuple.Item2;
        }

        public void Swap(ref Pair<TTy1, TTy2> right)
        {
            Swap(ref First, ref right.First);
            Swap(ref Second, ref right.Second);
        }

        private static void Swap<TTy>(ref TTy left, ref TTy right)
        {
            TTy tmp = left;
            left  = right;
            right = tmp;
        }

        public static implicit operator (TTy1, TTy2)(Pair<TTy1, TTy2> pair)
        {
            return (pair.First, pair.Second);
        }

        public static implicit operator Tuple<TTy1, TTy2>(Pair<TTy1, TTy2> pair)
        {
            return new Tuple<TTy1, TTy2>(pair.First, pair.Second);
        }

        public static implicit operator Pair<TTy1, TTy2>(ValueTuple<TTy1, TTy2> valueTuple)
        {
            return new Pair<TTy1, TTy2>(valueTuple.Item1, valueTuple.Item2);
        }

        public static implicit operator Pair<TTy1, TTy2>(Tuple<TTy1, TTy2> tuple)
        {
            return new Pair<TTy1, TTy2>(tuple.Item1, tuple.Item2);
        }
    }
}