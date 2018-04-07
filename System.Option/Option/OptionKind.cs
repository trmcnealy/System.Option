using System.Runtime.CompilerServices;

namespace System.Option
{
    //public enum OptionClass : byte
    //{
    //    GroupClass = 0,
    //    InputClass,
    //    UnknownClass,
    //    FlagClass,
    //    JoinedClass,
    //    ValuesClass,
    //    SeparateClass,
    //    RemainingArgsClass,
    //    RemainingArgsJoinedClass,
    //    CommaJoinedClass,
    //    MultiArgClass,
    //    JoinedOrSeparateClass,
    //    JoinedAndSeparateClass
    //}

    public sealed class OptionKind : IEquatable<OptionKind>
    {
        private static readonly OptionKind GroupClassType = new OptionKind(0);

        private static readonly OptionKind InputClassType = new OptionKind(1);

        private static readonly OptionKind UnknownClassType = new OptionKind(2);

        private static readonly OptionKind FlagClassType = new OptionKind(3);

        private static readonly OptionKind JoinedClassType = new OptionKind(4);

        private static readonly OptionKind ValuesClassType = new OptionKind(5);

        private static readonly OptionKind SeparateClassType = new OptionKind(6);

        private static readonly OptionKind RemainingArgsClassType = new OptionKind(7);

        private static readonly OptionKind RemainingArgsJoinedClassType = new OptionKind(8);

        private static readonly OptionKind CommaJoinedClassType = new OptionKind(9);

        private static readonly OptionKind MultiArgClassType = new OptionKind(10);

        private static readonly OptionKind JoinedOrSeparateClassType = new OptionKind(11);

        private static readonly OptionKind JoinedAndSeparateClassType = new OptionKind(12);

        public byte Value
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get;
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            set;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private OptionKind()
        {
            Value = 0;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private OptionKind(byte value)
        {
            Value = value;
        }

        public static OptionKind GroupClass
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get { return GroupClassType; }
        }

        public static OptionKind InputClass
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get { return InputClassType; }
        }

        public static OptionKind UnknownClass
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get { return UnknownClassType; }
        }

        public static OptionKind FlagClass
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get { return FlagClassType; }
        }

        public static OptionKind JoinedClass
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get { return JoinedClassType; }
        }

        public static OptionKind ValuesClass
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get { return ValuesClassType; }
        }

        public static OptionKind SeparateClass
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get { return SeparateClassType; }
        }

        public static OptionKind RemainingArgsClass
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get { return RemainingArgsClassType; }
        }

        public static OptionKind RemainingArgsJoinedClass
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get { return RemainingArgsJoinedClassType; }
        }

        public static OptionKind CommaJoinedClass
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get { return CommaJoinedClassType; }
        }

        public static OptionKind MultiArgClass
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get { return MultiArgClassType; }
        }

        public static OptionKind JoinedOrSeparateClass
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get { return JoinedOrSeparateClassType; }
        }

        public static OptionKind JoinedAndSeparateClass
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get { return JoinedAndSeparateClassType; }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static implicit operator byte(OptionKind optionClass)
        {
            return optionClass.Value;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static implicit operator OptionKind(byte value)
        {
            return new OptionKind(value);
        }

        public static OptionKind operator ~(OptionKind left)
        {
            return (byte)(~left.Value);
        }

        public static OptionKind operator <<(OptionKind left,
                                             int        right)
        {
            return (byte)(left.Value << right);
        }

        public static OptionKind operator >>(OptionKind left,
                                             int        right)
        {
            return (byte)(left.Value >> right);
        }

        public static OptionKind operator ^(OptionKind left,
                                            OptionKind right)
        {
            return (byte)(left.Value ^ right.Value);
        }

        public static OptionKind operator &(OptionKind left,
                                            OptionKind right)
        {
            return (byte)(left.Value & right.Value);
        }

        public static OptionKind operator |(OptionKind left,
                                            OptionKind right)
        {
            return (byte)(left.Value | right.Value);
        }

        public bool Equals(OptionKind other)
        {
            if(ReferenceEquals(null,
                               other))
            {
                return false;
            }

            if(ReferenceEquals(this,
                               other))
            {
                return true;
            }

            return Value == other.Value;
        }

        public override bool Equals(object obj)
        {
            if(ReferenceEquals(null,
                               obj))
            {
                return false;
            }

            if(ReferenceEquals(this,
                               obj))
            {
                return true;
            }

            return obj is OptionKind && Equals((OptionKind)obj);
        }

        public override int GetHashCode()
        {
            return Value.GetHashCode();
        }

        public static bool operator ==(OptionKind left,
                                       OptionKind right)
        {
            return Equals(left,
                          right);
        }

        public static bool operator ==(OptionKind left,
                                       byte       right)
        {
            return left?.Value == right;
        }

        public static bool operator ==(byte       left,
                                       OptionKind right)
        {
            return left == right?.Value;
        }

        public static bool operator !=(OptionKind left,
                                       OptionKind right)
        {
            return !Equals(left,
                           right);
        }

        public static bool operator !=(OptionKind left,
                                       byte       right)
        {
            return left?.Value != right;
        }

        public static bool operator !=(byte       left,
                                       OptionKind right)
        {
            return left != right?.Value;
        }
    }
}