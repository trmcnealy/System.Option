using System.Runtime.CompilerServices;

namespace System.Option
{
    //[Flags]
    //public enum DriverFlag : ushort
    //{
    //    HelpHidden = 1 << 0,
    //    RenderAsInput = 1 << 1,
    //    RenderJoined = 1 << 2,
    //    RenderSeparate = 1 << 3
    //}

    public sealed class DriverFlag
    {
        private static readonly DriverFlag HelpHiddenType = new DriverFlag(1 << 0);

        private static readonly DriverFlag RenderAsInputType = new DriverFlag(1 << 1);

        private static readonly DriverFlag RenderJoinedType = new DriverFlag(1 << 2);

        private static readonly DriverFlag RenderSeparateType = new DriverFlag(1 << 3);

        public ushort Value
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get;
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            set;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private DriverFlag()
        {
            Value = 0;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private DriverFlag(ushort value)
        {
            Value = value;
        }

        public static DriverFlag HelpHidden
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get { return HelpHiddenType; }
        }

        public static DriverFlag RenderAsInput
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get { return RenderAsInputType; }
        }

        public static DriverFlag RenderJoined
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get { return RenderJoinedType; }
        }

        public static DriverFlag RenderSeparate
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get { return RenderSeparateType; }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static implicit operator ushort(DriverFlag driverFlag)
        {
            return driverFlag.Value;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static implicit operator DriverFlag(ushort value)
        {
            return new DriverFlag(value);
        }

        public static DriverFlag operator ~(DriverFlag left)
        {
            return (ushort)(~left.Value);
        }

        public static DriverFlag operator <<(DriverFlag left,
                                             int        right)
        {
            return (ushort)(left.Value << right);
        }

        public static DriverFlag operator >>(DriverFlag left,
                                             int        right)
        {
            return (ushort)(left.Value >> right);
        }

        public static DriverFlag operator ^(DriverFlag left,
                                            DriverFlag right)
        {
            return (ushort)(left.Value ^ right.Value);
        }

        public static DriverFlag operator &(DriverFlag left,
                                            DriverFlag right)
        {
            return (ushort)(left.Value & right.Value);
        }

        public static DriverFlag operator |(DriverFlag left,
                                            DriverFlag right)
        {
            return (ushort)(left.Value | right.Value);
        }

        public bool Equals(DriverFlag other)
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

            return obj is DriverFlag && Equals((DriverFlag)obj);
        }

        public override int GetHashCode()
        {
            return Value.GetHashCode();
        }

        public static bool operator ==(DriverFlag left,
                                       DriverFlag right)
        {
            return Equals(left,
                          right);
        }

        public static bool operator ==(DriverFlag left,
                                       ushort     right)
        {
            return left?.Value == right;
        }

        public static bool operator ==(ushort     left,
                                       DriverFlag right)
        {
            return left == right?.Value;
        }

        public static bool operator !=(DriverFlag left,
                                       DriverFlag right)
        {
            return !Equals(left,
                           right);
        }

        public static bool operator !=(DriverFlag left,
                                       ushort     right)
        {
            return left?.Value != right;
        }

        public static bool operator !=(ushort     left,
                                       DriverFlag right)
        {
            return left != right?.Value;
        }
    }
}