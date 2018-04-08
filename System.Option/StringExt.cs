using System.Runtime.CompilerServices;

namespace System
{
    public static class StringExt
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool Empty(this string @this)
        {
            return string.IsNullOrEmpty(@this);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string Slice(this string @this,
                                   int         start,
                                   int         end)
        {
            start = Math.Min(start,
                             @this.Length);
            end = Math.Min(Math.Max(start,
                                    end),
                           @this.Length);
            return @this.Substring(start,
                                   end - start);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static (string, string) SplitInTwo(this string @this,
                                                  char        separator)
        {
            int idx = @this.IndexOf(separator);

            if(idx == -1)
            {
                return (@this, string.Empty);
            }

            return (@this.Slice(0,
                                idx), @this.Slice(idx + 1,
                                                  -1));
        }

        // Compute the edit distance between the two given strings.
        public static int EditDistance(this string @this,
                                       string      other,
                                       bool        allowReplacements,
                                       int         maxEditDistance)
        {
            return ComputeEditDistance(@this,
                                       other,
                                       allowReplacements,
                                       maxEditDistance);
        }

        /// <summary>
        /// The algorithm implemented below is the "classic"
        /// dynamic-programming algorithm for computing the Levenshtein
        /// distance, which is described here:
        ///
        /// http://en.wikipedia.org/wiki/Levenshtein_distance
        ///
        /// Although the algorithm is typically described using an m x n
        /// array, only one row plus one element are used at a time, so this
        /// implementation just keeps one vector for the row.  To update one entry,
        /// only the entries to the left, top, and top-left are needed.  The left
        /// entry is in Row[x-1], the top entry is what's in Row[x] from the last
        /// iteration, and the top-left entry is stored in Previous.
        /// </summary>
        /// <param name="fromArray"></param>
        /// <param name="toArray"></param>
        /// <param name="allowReplacements"></param>
        /// <param name="maxEditDistance"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int ComputeEditDistance(string fromArray,
                                              string toArray,
                                              bool   allowReplacements = true,
                                              int    maxEditDistance   = 0)
        {
            int m = fromArray.Length;
            int n = toArray.Length;

            const int smallBufferSize = 64;

            int[] smallBuffer = new int[smallBufferSize];

            //int[] allocated;

            int[] row = smallBuffer;

            if(n + 1 > smallBufferSize)
            {
                row = new int[n + 1];

                //Array.Resize(Row);
            }

            for(int i = 1; i <= n; ++i)
            {
                row[i] = i;
            }

            int bestThisRow;
            int previous;

            for(int y = 1; y <= m; ++y)
            {
                row[0]      = y;
                bestThisRow = row[0];
                previous    = y - 1;

                int oldRow;

                for(int x = 1; x <= n; ++x)
                {
                    oldRow = row[x];

                    if(allowReplacements)
                    {
                        row[x] = Math.Min(previous + (fromArray[y - 1] == toArray[x - 1] ? 0 : 1),
                                          Math.Min(row[x - 1],
                                                   row[x]) + 1);
                    }
                    else
                    {
                        if(fromArray[y - 1] == toArray[x - 1])
                        {
                            row[x] = previous;
                        }
                        else
                        {
                            row[x] = Math.Min(row[x - 1],
                                              row[x]) + 1;
                        }
                    }

                    previous = oldRow;
                    bestThisRow = Math.Min(bestThisRow,
                                           row[x]);
                }

                if(maxEditDistance > 0 && bestThisRow > maxEditDistance)
                {
                    return maxEditDistance + 1;
                }
            }

            int result = row[n];
            return result;
        }
    }
}