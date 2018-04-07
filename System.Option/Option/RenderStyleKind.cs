using System.Runtime.CompilerServices;

namespace System.Option
{
    //public enum RenderStyleKind
    //{
    //    RenderCommaJoinedStyle,
    //    RenderJoinedStyle,
    //    RenderSeparateStyle,
    //    RenderValuesStyle
    //}

    public sealed class RenderStyleKind
    {
        private static readonly RenderStyleKind RenderCommaJoinedStyleType = new RenderStyleKind(0);

        private static readonly RenderStyleKind RenderJoinedStyleType = new RenderStyleKind(1);

        private static readonly RenderStyleKind RenderSeparateStyleType = new RenderStyleKind(2);

        private static readonly RenderStyleKind RenderValuesStyleType = new RenderStyleKind(3);

        public int Value
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get;
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            set;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private RenderStyleKind()
        {
            Value = 0;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private RenderStyleKind(int value)
        {
            Value = value;
        }

        public static RenderStyleKind RenderCommaJoinedStyle
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get { return RenderCommaJoinedStyleType; }
        }

        public static RenderStyleKind RenderJoinedStyle
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get { return RenderJoinedStyleType; }
        }

        public static RenderStyleKind RenderSeparateStyle
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get { return RenderSeparateStyleType; }
        }

        public static RenderStyleKind RenderValuesStyle
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get { return RenderValuesStyleType; }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static implicit operator int(RenderStyleKind renderStyleKind)
        {
            return renderStyleKind.Value;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static implicit operator RenderStyleKind(int value)
        {
            return new RenderStyleKind(value);
        }

        public static RenderStyleKind operator ~(RenderStyleKind left)
        {
            return (byte)(~left.Value);
        }

        public static RenderStyleKind operator <<(RenderStyleKind left,
                                                  int             right)
        {
            return (left?.Value << right);
        }

        public static RenderStyleKind operator >>(RenderStyleKind left,
                                                  int             right)
        {
            return (left.Value >> right);
        }

        public static RenderStyleKind operator ^(RenderStyleKind left,
                                                 RenderStyleKind right)
        {
            return (left.Value ^ right.Value);
        }

        public static RenderStyleKind operator &(RenderStyleKind left,
                                                 RenderStyleKind right)
        {
            return (left.Value & right.Value);
        }

        public static RenderStyleKind operator |(RenderStyleKind left,
                                                 RenderStyleKind right)
        {
            return (left.Value | right.Value);
        }

        public bool Equals(RenderStyleKind other)
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

            return obj is RenderStyleKind && Equals((RenderStyleKind)obj);
        }

        public override int GetHashCode()
        {
            return Value.GetHashCode();
        }

        public static bool operator ==(RenderStyleKind left,
                                       RenderStyleKind right)
        {
            return Equals(left,
                          right);
        }

        public static bool operator ==(RenderStyleKind left,
                                       int             right)
        {
            return left?.Value == right;
        }

        public static bool operator ==(int             left,
                                       RenderStyleKind right)
        {
            return left == right?.Value;
        }

        public static bool operator !=(RenderStyleKind left,
                                       RenderStyleKind right)
        {
            return !Equals(left,
                           right);
        }

        public static bool operator !=(RenderStyleKind left,
                                       int             right)
        {
            return left?.Value != right;
        }

        public static bool operator !=(int             left,
                                       RenderStyleKind right)
        {
            return left != right?.Value;
        }
    }
}