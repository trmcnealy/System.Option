using System.Diagnostics;

namespace System.Option
{
    /// \brief Entry for a single option instance in the option data table.
    [DebuggerDisplay("ID = {Id}, Name = {Name == null ? null : Name.ToString()}, Values = {Values == null ? null : Values.ToString()}, AliasID = {AliasId}, AliasArgs = {AliasArgs== null ? null : AliasArgs.ToString()}")]
    public sealed class OptionInfo
    {
        public string[] Prefixes;

        public string Name;

        public string HelpText;

        public string MetaVar;

        public int Id;

        public byte Kind;

        public byte Param;

        public ushort Flags;

        public ushort GroupId;

        public ushort AliasId;

        public string AliasArgs;

        public string Values;

        public OptionInfo(string[] prefixes,
                          string   name,
                          int      iD,
                          byte     kind,
                          ushort   groupId,
                          ushort   aliasId,
                          string   aliasArgs,
                          ushort   flags,
                          byte     param,
                          string   helpText,
                          string   metaVar,
                          string   values)
        {
            Prefixes  = prefixes;
            Name      = name;
            Id        = iD;
            Kind      = kind;
            GroupId   = groupId;
            AliasId   = aliasId;
            AliasArgs = aliasArgs;
            Flags     = flags;
            Param     = param;
            HelpText  = helpText;
            MetaVar   = metaVar;
            Values    = values;
        }

        public static bool operator <(OptionInfo left,
                                      OptionInfo right)
        {
            if(left == right)
            {
                return false;
            }

            var n = StrCmpOptionName(left.Name,
                                     right.Name);

            if(n != 0)
            {
                return n < 0;
            }

            for(var i = 0; i < left.Prefixes.Length; i++)
            {
                n = StrCmpOptionName(left.Prefixes[i],
                                     right.Prefixes[i]);

                if(n != 0)
                {
                    return n < 0;
                }
            }

            // Names are the same, check that classes are in order; exactly one
            // should be joined, and it should succeed the other.
            var avar = left.Kind == OptionKind.JoinedClass ? 1 : 0;
            var bvar = right.Kind == OptionKind.JoinedClass ? 1 : 0;
            Debug.Assert((avar ^ bvar) == 0,
                         "Unexpected classes for options with same name.");
            return right.Kind == OptionKind.JoinedClass;
        }

        public static bool operator >(OptionInfo left,
                                      OptionInfo right)
        {
            return right < left;
        }

        // Support lower_bound between info and an option name.
        public static bool operator <(OptionInfo I,
                                      string     name)
        {
            return StrCmpOptionNameIgnoreCase(I.Name,
                                              name) < 0;
        }

        // Support lower_bound between info and an option name.
        public static bool operator >(OptionInfo I,
                                      string     name)
        {
            return StrCmpOptionNameIgnoreCase(I.Name,
                                              name) > 0;
        }

        internal static int StrCmpOptionNameIgnoreCase(string a,
                                                       string b)
        {
            //var option1 = String.Compare(a,
            //                             b,
            //                             StringComparison.InvariantCulture);
            //var option2 = String.Compare(a,
            //                             b,
            //                             StringComparison.InvariantCultureIgnoreCase);
            //var option3 = String.Compare(a,
            //                             b,
            //                             StringComparison.Ordinal);
            //var option4 = String.Compare(a,
            //                             b,
            //                             StringComparison.OrdinalIgnoreCase);

            string localA = a + '\0';
            string localB = b + '\0';

            int aIndex = 0;
            int bIndex = 0;

            char x = localA[aIndex];
            char y = localB[bIndex];

            char aChar = char.ToLower(x);
            char bChar = char.ToLower(y);

            while(aChar == bChar)
            {
                if(aChar == '\0')
                    return 0;

                if(bChar == '\0')
                    break;

                aIndex++;
                bIndex++;

                x = localA[aIndex];
                y = localB[bIndex];

                aChar = char.ToLower(x);
                bChar = char.ToLower(y);
            }

            if(aChar == '\0') // A is a prefix of B.
                return 1;
            if(bChar == '\0') // B is a prefix of A.
                return -1;

            // Otherwise lexicographic.
            return (aChar < bChar) ? -1 : 1;
        }

        internal static int Strcmp(string a,
                                   string b)
        {
            string src = a + '\0';
            string dst = b + '\0';

            int aIndex = 0;
            int bIndex = 0;

            int ret = src[aIndex] - dst[bIndex];

            while(ret == 0)
            {
                if(src[aIndex] == '\0')
                    return 0;

                if(dst[bIndex] == '\0')
                    break;

                ++aIndex;
                ++bIndex;

                ret = src[aIndex] - dst[bIndex];
            }

            if(ret < 0)
            {
                ret = -1;
            }

            else if(ret > 0)
            {
                ret = 1;
            }

            return ret;
        }

        internal static int StrCmpOptionName(string a,
                                             string b)
        {
            int n = StrCmpOptionNameIgnoreCase(a,
                                               b);

            if(n == 1)
            {
                return n;
            }

            return Strcmp(a,
                          b); //String.Compare(A, B, StringComparison.InvariantCulture));
        }

        //internal static int StrCmpOptionNameIgnoreCase(string left,
        //                                               string right)
        //{
        //    var A = (left + '\0').ToCharArray();
        //    var B = (right + '\0').ToCharArray();

        //    var ALen = A.Length;
        //    var BLen = B.Length;

        //    var X = 0; //(sbyte*) Unsafe.AsPointer(ref A);
        //    var Y = 0; //(sbyte*) Unsafe.AsPointer(ref B);

        //    var a = char.ToLower(A[X]);
        //    var b = char.ToLower(B[Y]);

        //    while (a == b) // && X < ALen && Y < BLen)
        //    {
        //        if (a == '\0')
        //        {
        //            return 0;
        //        }

        //        a = char.ToLower(A[++X]);
        //        b = char.ToLower(B[++Y]);
        //    }

        //    if (a == '\0') // A is a prefix of B.
        //    {
        //        return 1;
        //    }

        //    if (b == '\0') // B is a prefix of A.
        //    {
        //        return -1;
        //    }

        //    // Otherwise lexicographic.
        //    return a < b ?
        //               -1 :
        //               1;
        //}

        //internal static int StrCmpOptionName(string A,
        //                                     string B)
        //{
        //    var N = StrCmpOptionNameIgnoreCase(A, B);

        //    if (N != 0)
        //    {
        //        return N;
        //    }

        //    return string.CompareOrdinal(A, B);
        //}
    }
}