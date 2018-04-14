using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace System.Option.UnitTests
{
    //[Flags]
    //public enum OptionFlags : ushort
    //{
    //    None = 0,
    //    OptFlag1 = (1 << 4),
    //    OptFlag2 = (1 << 5),
    //    OptFlag3 = (1 << 6)
    //}

    public sealed class OptionFlags
    {
        private static readonly OptionFlags NoneType = new OptionFlags(1 << 0);
        private static readonly OptionFlags OptFlag1Type = new OptionFlags(1 << 4);
        private static readonly OptionFlags OptFlag2Type = new OptionFlags(1 << 5);
        private static readonly OptionFlags OptFlag3Type = new OptionFlags(1 << 6);

        public ushort Value
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get;
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            set;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private OptionFlags()
        {
            this.Value = 0;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private OptionFlags(ushort value)
        {
            this.Value = value;
        }

        public static OptionFlags None
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get { return NoneType; }
        }

        public static OptionFlags OptFlag1
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get { return OptFlag1Type; }
        }

        public static OptionFlags OptFlag2
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get { return OptFlag2Type; }
        }

        public static OptionFlags OptFlag3
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get { return OptFlag3Type; }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static implicit operator ushort(OptionFlags optionFlags)
        {
            return optionFlags.Value;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static implicit operator OptionFlags(ushort value)
        {
            return new OptionFlags(value);
        }

        public static OptionFlags operator ~(OptionFlags left)
        {
            return (ushort)(~left.Value);
        }

        public static OptionFlags operator <<(OptionFlags left,
                                              int right)
        {
            return (ushort)(left.Value << right);
        }

        public static OptionFlags operator >>(OptionFlags left,
                                              int right)
        {
            return (ushort)(left.Value >> right);
        }

        public static OptionFlags operator ^(OptionFlags left,
                                             OptionFlags right)
        {
            return (ushort)(left.Value ^ right.Value);
        }

        public static OptionFlags operator &(OptionFlags left,
                                             OptionFlags right)
        {
            return (ushort)(left.Value & right.Value);
        }

        public static OptionFlags operator |(OptionFlags left,
                                             OptionFlags right)
        {
            return (ushort)(left.Value | right.Value);
        }

        public bool Equals(OptionFlags other)
        {
            if (ReferenceEquals(null, other))
            {
                return false;
            }

            if (ReferenceEquals(this, other))
            {
                return true;
            }

            return Value == other.Value;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj))
            {
                return false;
            }

            if (ReferenceEquals(this, obj))
            {
                return true;
            }

            return obj is OptionFlags && Equals((OptionFlags)obj);
        }

        public override int GetHashCode()
        {
            return Value.GetHashCode();
        }

        public static bool operator ==(OptionFlags left,
                                       OptionFlags right)
        {
            return Equals(left, right);
        }

        public static bool operator ==(OptionFlags left,
                                       ushort right)
        {
            return left.Value == right;
        }

        public static bool operator ==(ushort left,
                                       OptionFlags right)
        {
            return left == right.Value;
        }

        public static bool operator !=(OptionFlags left,
                                       OptionFlags right)
        {
            return !Equals(left, right);
        }

        public static bool operator !=(OptionFlags left,
                                       ushort right)
        {
            return left.Value != right;
        }

        public static bool operator !=(ushort left,
                                       OptionFlags right)
        {
            return left != right.Value;
        }
    }

    public class OPT_ID
    {
        public const int OPT_INVALID = 0;
        public const int OPT_my_group = 1;
        public const int OPT_INPUT = 2;
        public const int OPT_UNKNOWN = 3;
        public const int OPT_A = 4;
        public const int OPT_B = 5;
        public const int OPT_Ceq = 6;
        public const int OPT_C = 7;
        public const int OPT_SLASH_C = 8;
        public const int OPT_D = 9;
        public const int OPT_E = 10;
        public const int OPT_F = 11;
        public const int OPT_G = 12;
        public const int OPT_H = 13;
        public const int OPT_I = 14;
        public const int OPT_Joo = 15;
        public const int OPT_J = 16;
        public const int OPT_K = 17;
        public const int OPT_SlurpJoined = 18;
        public const int OPT_Slurp = 19;
        public const int LastOption = 20;
    }

    [TestClass]
    public class UnitTest1
    {
        //private static readonly string NullChar = new string(new char[] { (char)0 });
        public static readonly string[] prefix_0 = new string[] { null };
        public static readonly string[] prefix_1 = new string[] { "-", null };
        public static readonly string[] prefix_2 = new string[] { "-", "--", null };
        public static readonly string[] prefix_3 = new string[] { "/", "-", null };

        public static OptionInfo[] InfoTable = new OptionInfo[]
        {
            //                prefixes, name,        iD,                  kind,                                      groupID,             aliasID,            aliasArgs,  flags,            param, helpText, metaVar, values
            new OptionInfo(null,     "my group",  OPT_ID.OPT_my_group, OptionKind.GroupClass,             OPT_ID.OPT_INVALID,  OPT_ID.OPT_INVALID, null,  OptionFlags.None,      0,                 null, null, null),
            new OptionInfo(prefix_0, "<input>",   OPT_ID.OPT_INPUT,    OptionKind.InputClass,             OPT_ID.OPT_INVALID,  OPT_ID.OPT_INVALID, null,  OptionFlags.None,      0,                 null, null, null),
            new OptionInfo(prefix_0, "<unknown>", OPT_ID.OPT_UNKNOWN,  OptionKind.UnknownClass,           OPT_ID.OPT_INVALID,  OPT_ID.OPT_INVALID, null,  OptionFlags.None,      0,                 null, null, null),
            new OptionInfo(prefix_1, "A",         OPT_ID.OPT_A,        OptionKind.FlagClass,              OPT_ID.OPT_INVALID,  OPT_ID.OPT_INVALID, null,  OptionFlags.OptFlag1,  0, "The A option", null, null),
            new OptionInfo(prefix_1, "B",         OPT_ID.OPT_B,        OptionKind.JoinedClass,            OPT_ID.OPT_INVALID,  OPT_ID.OPT_INVALID, null,  OptionFlags.OptFlag2,  0, "The B option", "B",  null),
            new OptionInfo(prefix_2, "C=",        OPT_ID.OPT_Ceq,      OptionKind.JoinedClass,            OPT_ID.OPT_INVALID,  OPT_ID.OPT_C,       null,  OptionFlags.OptFlag1,  0, null, null, null),
            new OptionInfo(prefix_1, "C",         OPT_ID.OPT_C,        OptionKind.SeparateClass,          OPT_ID.OPT_INVALID,  OPT_ID.OPT_INVALID, null,  OptionFlags.OptFlag1,  0, "The C option", "C", null),
            new OptionInfo(prefix_3, "C",         OPT_ID.OPT_SLASH_C,  OptionKind.SeparateClass,          OPT_ID.OPT_INVALID,  OPT_ID.OPT_INVALID, null,  OptionFlags.OptFlag3,  0, "The C option", "C", null),
            new OptionInfo(prefix_1, "D",         OPT_ID.OPT_D,        OptionKind.CommaJoinedClass,       OPT_ID.OPT_INVALID,  OPT_ID.OPT_INVALID, null,  OptionFlags.None,      0, "The D option", "D", null),
            new OptionInfo(prefix_1, "E",         OPT_ID.OPT_E,        OptionKind.MultiArgClass,          OPT_ID.OPT_INVALID,  OPT_ID.OPT_INVALID, null,  OptionFlags.OptFlag1 | OptionFlags.OptFlag2, 2, null, null, null),
            new OptionInfo(prefix_1, "F",         OPT_ID.OPT_F,        OptionKind.JoinedOrSeparateClass,  OPT_ID.OPT_INVALID,  OPT_ID.OPT_INVALID, null,  OptionFlags.None,      0, "The F option", "F", null),
            new OptionInfo(prefix_1, "G",         OPT_ID.OPT_G,        OptionKind.JoinedAndSeparateClass, OPT_ID.OPT_INVALID,  OPT_ID.OPT_INVALID, null,  OptionFlags.None,      0, "The G option", "G", null),
            new OptionInfo(prefix_1, "H",         OPT_ID.OPT_H,        OptionKind.FlagClass,              OPT_ID.OPT_INVALID,  OPT_ID.OPT_INVALID, null,  DriverFlag.HelpHidden, 0,                 null, null, null),
            new OptionInfo(prefix_1, "I",         OPT_ID.OPT_I,        OptionKind.FlagClass,              OPT_ID.OPT_my_group, OPT_ID.OPT_H,       null,  OptionFlags.None,      0,                 null, null, null),
            new OptionInfo(prefix_1, "Joo",       OPT_ID.OPT_Joo,      OptionKind.FlagClass,              OPT_ID.OPT_INVALID,  OPT_ID.OPT_B,       "bar", OptionFlags.None,      0,                 null, null, null),
            new OptionInfo(prefix_1, "J",         OPT_ID.OPT_J,        OptionKind.FlagClass,              OPT_ID.OPT_INVALID,  OPT_ID.OPT_B,       "foo", OptionFlags.None,      0,                 null, null, null),
            new OptionInfo(prefix_1, "K",         OPT_ID.OPT_K,        OptionKind.FlagClass,              OPT_ID.OPT_INVALID,  OPT_ID.OPT_B,       null,  OptionFlags.None,      0,                 null, null, null),
            new OptionInfo(prefix_1, "slurpjoined", OPT_ID.OPT_SlurpJoined, OptionKind.RemainingArgsJoinedClass, OPT_ID.OPT_INVALID, OPT_ID.OPT_INVALID, null, OptionFlags.None, 0,                 null, null, null),
            new OptionInfo(prefix_1, "slurp",     OPT_ID.OPT_Slurp, OptionKind.RemainingArgsClass, OPT_ID.OPT_INVALID, OPT_ID.OPT_INVALID, null, OptionFlags.None, 0, null, null, null)
        };

        string[] Args = {
                "-A",
                "-Bhi",
                "--C=desu",
                "-C", "bye",
                "-D,adena",
                "-E", "apple", "bloom",
                "-Fblarg",
                "-F", "42",
                "-Gchuu", "2"
        };

        [TestMethod]
        public void TestOptionParsing()
        {

            OptionTable T = new OptionTable(InfoTable);

            int FlagsToInclude = 0;
            int FlagsToExclude = 0;

            InputArgumentList al = T.ParseArgs(Args, out int missingArgIndex, out int missingArgCount, FlagsToInclude, FlagsToExclude);

            // Check they all exist.
            Assert.IsTrue(al.HasArg(OPT_ID.OPT_A));
            Assert.IsTrue(al.HasArg(OPT_ID.OPT_B));
            Assert.IsTrue(al.HasArg(OPT_ID.OPT_C));
            Assert.IsTrue(al.HasArg(OPT_ID.OPT_D));
            Assert.IsTrue(al.HasArg(OPT_ID.OPT_E));
            Assert.IsTrue(al.HasArg(OPT_ID.OPT_F));
            Assert.IsTrue(al.HasArg(OPT_ID.OPT_G));

            // Check the values.

            Assert.IsTrue("hi" == al.GetLastArgValue(OPT_ID.OPT_B));
            Assert.IsTrue("bye" == al.GetLastArgValue(OPT_ID.OPT_C));
            Assert.IsTrue("adena" == al.GetLastArgValue(OPT_ID.OPT_D));
            List<string> Es = al.GetAllArgValues(OPT_ID.OPT_E);
            Assert.IsTrue("apple" == Es[0]);
            Assert.IsTrue("bloom" == Es[1]);
            Assert.IsTrue("42" == al.GetLastArgValue(OPT_ID.OPT_F));
            List<string> Gs = al.GetAllArgValues(OPT_ID.OPT_G);
            Assert.IsTrue("chuu" == Gs[0]);
            Assert.IsTrue("2" == Gs[1]);

            // Check the help text.
            StringBuilder RSO = new StringBuilder();
            T.PrintHelp(RSO, "test", "title!");
            string Help = RSO.ToString();
            Assert.IsTrue(Help.Length != Help.IndexOf("-A", StringComparison.Ordinal));

            // Test aliases.
            var Cs = al.Filtered(OPT_ID.OPT_C);
            Assert.IsTrue(Cs.First() != Cs.Last());
            Assert.IsTrue("desu" == Cs.First().GetValue());
            List<string> ASL = new List<string>();
            Cs.First().Render(al, ASL);
            Assert.IsTrue(2u == ASL.Count);
            Assert.IsTrue("-C" == ASL[0]);
            Assert.IsTrue("desu" == ASL[1]);
        }

        [TestMethod]
        public void TestParseWithFlagExclusions()
        {
            OptionTable T = new OptionTable(InfoTable);
            

            // Exclude flag3 to avoid parsing as OPT_ID.OPT_SLASH_C.
            InputArgumentList AL = T.ParseArgs(Args, out int MAI, out int MAC, /*FlagsToInclude=*/0, /*FlagsToExclude=*/(int)OptionFlags.OptFlag3);
            Assert.IsTrue(AL.HasArg(OPT_ID.OPT_A));
            Assert.IsTrue(AL.HasArg(OPT_ID.OPT_C));
            Assert.IsTrue(!AL.HasArg(OPT_ID.OPT_SLASH_C));

            // Exclude flag1 to avoid parsing as OPT_ID.OPT_C.
            AL = T.ParseArgs(Args, out MAI, out MAC, /*FlagsToInclude=*/0, /*FlagsToExclude=*/(int)OptionFlags.OptFlag1);
            Assert.IsTrue(AL.HasArg(OPT_ID.OPT_B));
            Assert.IsTrue(!AL.HasArg(OPT_ID.OPT_C));
            Assert.IsTrue(AL.HasArg(OPT_ID.OPT_SLASH_C));

            string[] NewArgs = new string[] { "/C", "foo", "--C=bar" };
            AL = T.ParseArgs(NewArgs, out MAI, out MAC);
            Assert.IsTrue(AL.HasArg(OPT_ID.OPT_SLASH_C));
            Assert.IsTrue(AL.HasArg(OPT_ID.OPT_C));
            Assert.IsTrue("foo" == AL.GetLastArgValue(OPT_ID.OPT_SLASH_C));
            Assert.IsTrue("bar" == AL.GetLastArgValue(OPT_ID.OPT_C));
        }

        [TestMethod]
        public void TestParseAliasInGroup()
        {
            OptionTable T = new OptionTable(InfoTable);
            

            string[] MyArgs = new string[] { "-I" };
            InputArgumentList AL = T.ParseArgs(MyArgs, out int MAI, out int MAC);
            Assert.IsTrue(AL.HasArg(OPT_ID.OPT_H));
        }

        [TestMethod]
        public void TestAliasArgs()
        {
            OptionTable T = new OptionTable(InfoTable);
            

            string[] MyArgs = new string[] { "-J", "-Joo" };
            InputArgumentList AL = T.ParseArgs(MyArgs, out int MAI, out int MAC);
            Assert.IsTrue(AL.HasArg(OPT_ID.OPT_B));
            Assert.IsTrue("foo" == AL.GetAllArgValues(OPT_ID.OPT_B)[0]);
            Assert.IsTrue("bar" == AL.GetAllArgValues(OPT_ID.OPT_B)[1]);
        }

        [TestMethod]
        public void TestIgnoreCase()
        {
            OptionTable T = new OptionTable(InfoTable, true);
            

            string[] MyArgs = new string[] { "-a", "-joo" };
            InputArgumentList AL = T.ParseArgs(MyArgs, out int MAI, out int MAC);
            Assert.IsTrue(AL.HasArg(OPT_ID.OPT_A));
            Assert.IsTrue(AL.HasArg(OPT_ID.OPT_B));
        }

        [TestMethod]
        public void TestDoNotIgnoreCase()
        {
            OptionTable T = new OptionTable(InfoTable);
            

            string[] MyArgs = new string[] { "-a", "-joo" };
            InputArgumentList AL = T.ParseArgs(MyArgs, out int MAI, out int MAC);
            Assert.IsTrue(!AL.HasArg(OPT_ID.OPT_A));
            Assert.IsTrue(!AL.HasArg(OPT_ID.OPT_B));
        }

        [TestMethod]
        public void TestSlurpEmpty()
        {
            OptionTable T = new OptionTable(InfoTable);
            

            string[] MyArgs = new string[] { "-A", "-slurp" };
            InputArgumentList AL = T.ParseArgs(MyArgs, out int MAI, out int MAC);
            Assert.IsTrue(AL.HasArg(OPT_ID.OPT_A));
            Assert.IsTrue(AL.HasArg(OPT_ID.OPT_Slurp));
            Assert.IsTrue(0U == AL.GetAllArgValues(OPT_ID.OPT_Slurp).Count);
        }

        [TestMethod]
        public void TestSlurp()
        {
            OptionTable T = new OptionTable(InfoTable);
            

            string[] MyArgs = new string[] { "-A", "-slurp", "-B", "--", "foo" };
            InputArgumentList AL = T.ParseArgs(MyArgs, out int MAI, out int MAC);
            Assert.IsTrue(AL.Size() == 2U);
            Assert.IsTrue(AL.HasArg(OPT_ID.OPT_A));
            Assert.IsTrue(!AL.HasArg(OPT_ID.OPT_B));
            Assert.IsTrue(AL.HasArg(OPT_ID.OPT_Slurp));
            Assert.IsTrue(3U == AL.GetAllArgValues(OPT_ID.OPT_Slurp).Count);
            Assert.IsTrue("-B" == AL.GetAllArgValues(OPT_ID.OPT_Slurp)[0]);
            Assert.IsTrue("--" == AL.GetAllArgValues(OPT_ID.OPT_Slurp)[1]);
            Assert.IsTrue("foo" == AL.GetAllArgValues(OPT_ID.OPT_Slurp)[2]);
        }

        [TestMethod]
        public void TestSlurpJoinedEmpty()
        {
            OptionTable T = new OptionTable(InfoTable);
            

            string[] MyArgs = new string[] { "-A", "-slurpjoined" };
            InputArgumentList AL = T.ParseArgs(MyArgs, out int MAI, out int MAC);
            Assert.IsTrue(AL.HasArg(OPT_ID.OPT_A));
            Assert.IsTrue(AL.HasArg(OPT_ID.OPT_SlurpJoined));
            Assert.IsTrue(AL.GetAllArgValues(OPT_ID.OPT_SlurpJoined).Count == 0U);
        }

        [TestMethod]
        public void TestSlurpJoinedOneJoined()
        {
            OptionTable T = new OptionTable(InfoTable);
            

            string[] MyArgs = new string[] { "-A", "-slurpjoinedfoo" };
            InputArgumentList AL = T.ParseArgs(MyArgs, out int MAI, out int MAC);
            Assert.IsTrue(AL.HasArg(OPT_ID.OPT_A));
            Assert.IsTrue(AL.HasArg(OPT_ID.OPT_SlurpJoined));
            Assert.IsTrue(AL.GetAllArgValues(OPT_ID.OPT_SlurpJoined).Count == 1U);
            Assert.IsTrue(AL.GetAllArgValues(OPT_ID.OPT_SlurpJoined)[0] == "foo");
        }

        [TestMethod]
        public void TestSlurpJoinedAndSeparate()
        {
            OptionTable T = new OptionTable(InfoTable);
            

            string[] MyArgs = new string[] { "-A", "-slurpjoinedfoo", "bar", "baz" };
            InputArgumentList AL = T.ParseArgs(MyArgs, out int MAI, out int MAC);
            Assert.IsTrue(AL.HasArg(OPT_ID.OPT_A));
            Assert.IsTrue(AL.HasArg(OPT_ID.OPT_SlurpJoined));
            Assert.IsTrue(3U == AL.GetAllArgValues(OPT_ID.OPT_SlurpJoined).Count);
            Assert.IsTrue("foo" == AL.GetAllArgValues(OPT_ID.OPT_SlurpJoined)[0]);
            Assert.IsTrue("bar" == AL.GetAllArgValues(OPT_ID.OPT_SlurpJoined)[1]);
            Assert.IsTrue("baz" == AL.GetAllArgValues(OPT_ID.OPT_SlurpJoined)[2]);
        }

        [TestMethod]
        public void TestSlurpJoinedButSeparate()
        {
            OptionTable T = new OptionTable(InfoTable);
            

            string[] MyArgs = new string[] { "-A", "-slurpjoined", "foo", "bar", "baz" };
            InputArgumentList AL = T.ParseArgs(MyArgs, out int MAI, out int MAC);
            Assert.IsTrue(AL.HasArg(OPT_ID.OPT_A));
            Assert.IsTrue(AL.HasArg(OPT_ID.OPT_SlurpJoined));
            Assert.IsTrue(3U == AL.GetAllArgValues(OPT_ID.OPT_SlurpJoined).Count);
            Assert.IsTrue("foo" == AL.GetAllArgValues(OPT_ID.OPT_SlurpJoined)[0]);
            Assert.IsTrue("bar" == AL.GetAllArgValues(OPT_ID.OPT_SlurpJoined)[1]);
            Assert.IsTrue("baz" == AL.GetAllArgValues(OPT_ID.OPT_SlurpJoined)[2]);
        }

        [TestMethod]
        public void TestFlagAliasToJoined()
        {
            OptionTable T = new OptionTable(InfoTable);

            // Check that a flag alias provides an empty argument to a joined option.
            string[] MyArgs = new string[] { "-K" };
            InputArgumentList AL = T.ParseArgs(MyArgs, out int MAI, out int MAC);
            Assert.IsTrue(AL.Size() == 1U);
            Assert.IsTrue(AL.HasArg(OPT_ID.OPT_B));

            var allArgs = AL.GetAllArgValues(OPT_ID.OPT_B);
            Assert.IsTrue(1U == allArgs.Count);
            Assert.IsTrue("" == AL.GetAllArgValues(OPT_ID.OPT_B)[0]);
        }
    }
}