using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace System.Option
{
    /// \brief A concrete instance of a particular driver option.
    /// 
    /// The Argument class encodes just enough information to be able to
    /// derive the argument values efficiently.
    [DebuggerDisplay("Index = {_index}, Name = {_spelling}")]
    public class Argument
    {
        /// \brief The index at which this argument appears in the containing
        /// ArgumentList.
        private readonly int _index;

        /// \brief The option this argument is an instance of.
        private readonly Option _opt;

        /// \brief How this instance of the option was spelled.
        private readonly string _spelling;

        /// \brief The argument values, as C strings.
        private readonly List<string> _values = new List<string>(2);

        /// \brief The argument this argument was derived from (during tool chain
        /// argument translation), if any.
        private Argument _baseArg;

        /// \brief Was this argument used to effect compilation?
        /// 
        /// This is used for generating "argument unused" diagnostics.
        private bool _claimed;

        /// \brief Does this argument own its values?
        private bool _ownsValues;

        public Argument(Option   opt,
                        string   spelling,
                        int      index,
                        Argument baseArg = null)
        {
            _opt        = opt;
            _baseArg    = baseArg;
            _spelling   = spelling;
            _index      = index;
            _claimed    = false;
            _ownsValues = false;
        }

        public Argument(Option   opt,
                        string   spelling,
                        int      index,
                        string   value0,
                        Argument baseArg = null)
        {
            _opt        = opt;
            _baseArg    = baseArg;
            _spelling   = spelling;
            _index      = index;
            _claimed    = false;
            _ownsValues = false;
            _values.Add(value0);
        }

        public Argument(Option   opt,
                        string   spelling,
                        int      index,
                        string   value0,
                        string   value1,
                        Argument baseArg = null)
        {
            _opt        = opt;
            _baseArg    = baseArg;
            _spelling   = spelling;
            _index      = index;
            _claimed    = false;
            _ownsValues = false;
            _values.Add(value0);
            _values.Add(value1);
        }

        public void Dispose()
        {
            if(_ownsValues)
            {
                _values.Clear();
            }
        }

        public Option GetOption()
        {
            return _opt;
        }

        public string GetSpelling()
        {
            return _spelling;
        }

        public int GetIndex()
        {
            return _index;
        }

        public Argument GetBaseArg()
        {
            return _baseArg ?? this;
        }

        public void SetBaseArg(Argument baseArg)
        {
            _baseArg = baseArg;
        }

        public bool GetOwnsValues()
        {
            return _ownsValues;
        }

        public void SetOwnsValues(bool value)
        {
            _ownsValues = value;
        }

        public bool IsClaimed()
        {
            return GetBaseArg()._claimed;
        }

        public void Claim()
        {
            GetBaseArg()._claimed = true;
        }

        public int GetNumValues()
        {
            return _values.Count;
        }

        public string GetValue(int n = 0)
        {
            return _values[n];
        }

        public List<string> GetValues()
        {
            return _values;
        }

        public bool ContainsValue(string value)
        {
            for(int i = 0, e = GetNumValues(); i != e; ++i)
            {
                if(_values[i] == value)
                {
                    return true;
                }
            }

            return false;
        }

        /// \brief Append the argument onto the given array as strings.
        public void Render(ArgumentList args,
                           List<string> output)
        {
            var renderStyleKind = GetOption().GetRenderStyle();

            if(renderStyleKind == RenderStyleKind.RenderValuesStyle)
            {
                output.AddRange(_values);
                return;
            }

            if(renderStyleKind == RenderStyleKind.RenderCommaJoinedStyle)
            {
                var os = new StringBuilder(256);
                os.Append(GetSpelling());
                for(int i = 0, e = GetNumValues(); i != e; ++i)
                {
                    if(i != 0)
                    {
                        os.Append(',');
                    }

                    os.Append(GetValue(i));
                }

                output.Add(args.MakeArgString(os.ToString()));
                return;
            }

            if(renderStyleKind == RenderStyleKind.RenderJoinedStyle)
            {
                output.Add(args.GetOrMakeJoinedArgString(GetIndex(),
                                                         GetSpelling(),
                                                         GetValue()));
                output.AddRange(_values.Skip(1));
                return;
            }

            if(renderStyleKind == RenderStyleKind.RenderSeparateStyle)
            {
                output.Add(args.MakeArgString(GetSpelling()));
                output.AddRange(_values);
            }
        }

        /// \brief Append the argument, render as an input, onto the given
        /// array as strings.
        /// 
        /// The distinction is that some options only render their values
        /// when rendered as a input (e.g., Xlinker).
        public void RenderAsInput(ArgumentList args,
                                  List<string> output)
        {
            if(!GetOption().HasNoOptAsInput())
            {
                Render(args,
                       output);
                return;
            }

            output.AddRange(_values);
        }

        public void Print(StringBuilder o)
        {
            o.Append("<");

            o.Append(" Opt:");
            _opt.Print(o);

            o.Append($" Index: {_index}");

            o.Append(" Values: [");
            for(int i = 0, e = _values.Count; i != e; ++i)
            {
                if(i != 0)
                {
                    o.Append(", ");
                }

                o.Append($"'{_values[i]}'");
            }

            o.Append("]>\n");
        }

        public void Dump()
        {
            StringBuilder o = new StringBuilder();
            Print(o);
            Debug.WriteLine(o);
        }

        /// \brief Return a formatted version of the argument and
        /// its values, for debugging and diagnostics.
        public string GetAsString(ArgumentList args)
        {
            var os = new StringBuilder(256);

            var asl = new List<string>();
            Render(args,
                   asl);

            foreach(var it in asl)
            {
                os.Append(' ');
                os.Append(it);
            }

            return os.ToString().Substring(1);
        }

        
        public override string ToString()
        {
            StringBuilder o = new StringBuilder();
            Print(o);
            return o.ToString();
        }
    }
}