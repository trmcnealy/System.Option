namespace System.Option
{
    /// OptionSpecifier - Wrapper class for abstracting references to option IDs.
    public class OptionSpecifier
    {
        private readonly int _id;

        public OptionSpecifier()
        {
        }

        public OptionSpecifier(int id)
        {
            _id = id;
        }

        public OptionSpecifier(Option opt)
        {
            _id = opt.GetId();
        }

        public static implicit operator OptionSpecifier(int id)
        {
            return new OptionSpecifier(id);
        }

        public static implicit operator OptionSpecifier(Option opt)
        {
            return new OptionSpecifier(opt);
        }

        public bool IsValid()
        {
            return _id != 0;
        }

        public int GetId()
        {
            return _id;
        }

        public static bool operator ==(OptionSpecifier left,
                                       OptionSpecifier right)
        {
            return left?._id == right?.GetId();
        }

        public static bool operator !=(OptionSpecifier left,
                                       OptionSpecifier right)
        {
            return !(left == right);
        }

        protected bool Equals(OptionSpecifier other)
        {
            return _id == other?._id;
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

            if(obj.GetType() != GetType())
            {
                return false;
            }

            return Equals((OptionSpecifier)obj);
        }

        public override int GetHashCode()
        {
            return _id;
        }

        public override string ToString()
        {
            return _id.ToString();
        }
    }
}