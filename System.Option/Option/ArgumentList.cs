using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

using OptRange = System.Pair<int, int>;

namespace System.Option
{
    public class ArgumentList : IEnumerable<Argument>
    {
        private List<Argument> _args = new List<Argument>();

        public static OptRange EmptyRange()
        {
            return (int.MaxValue, 0);
        }

        /// The first and last index of each different OptSpecifier ID.
        Dictionary<int, OptRange> _optRanges = new Dictionary<int, OptRange>();

        /// Get the range of indexes in which options with the specified IDs might
        /// reside, or (0, 0) if there are no such options.
        public OptRange GetRange(IEnumerable<OptionSpecifier> ids)
        {
            OptRange r = EmptyRange();

            int key;
            foreach(var id in ids.Where(item => item != null))
            {
                key = id.GetId();

                if(_optRanges.ContainsKey(key))
                {
                    var I = _optRanges[key];

                    r.First = Math.Min(r.First,
                                       I.First);
                    r.Second = Math.Max(r.Second,
                                        I.Second);
                }
            }

            // Map an empty {-1, 0} range to {0, 0} so it can be used to form iterators.
            if(r.First == int.MaxValue)
            {
                r.First = 0;
            }

            return r;
        }

        public void Append(Argument a)
        {
            _args.Add(a);

            // Update ranges for the option and all of its groups.
            for(Option o = a.GetOption().GetUnaliasedOption(); o.IsValid(); o = o.GetGroup())
            {
                if(!_optRanges.ContainsKey(o.GetId()))
                {
                    _optRanges.Add(o.GetId(),
                                   new OptRange(_args.Count - 1,
                                                _args.Count));
                }
                else
                {
                    OptRange r = _optRanges[o.GetId()];
                    _optRanges[o.GetId()] = new OptRange(Math.Min(r.First,
                                                                  _args.Count - 1),
                                                         _args.Count);
                }
            }
        }

        public List<Argument> GetArgs()
        {
            return _args;
        }

        public int Size()
        {
            return _args.Count;
        }

        public IEnumerable<Argument> Filtered(params OptionSpecifier[] ids)
        {
            OptRange range = GetRange(ids);

            int b = range.First;
            int e = range.Second;

            return _args.Skip(b).Take(e - b);
        }

        public IEnumerable<Argument> filtered_reverse(OptionSpecifier[] ids)
        {
            OptRange range = GetRange(ids);

            var reverse = new List<Argument>(_args);
            reverse.Reverse();

            int b = reverse.Count - range.Second;
            int e = reverse.Count - range.First;

            return reverse.Skip(b).Take(e - b);
        }

        public void EraseArg(OptionSpecifier id)
        {
            _optRanges.Remove(id.GetId());
        }

        public bool HasArgNoClaim(params OptionSpecifier[] ids)
        {
            return GetLastArgNoClaim(ids) != null;
        }

        public bool HasArg(params OptionSpecifier[] ids)
        {
            return GetLastArg(ids) != null;
        }

        public Argument GetLastArg(params OptionSpecifier[] ids)
        {
            Argument res = null;

            var filteredIds = Filtered(ids);

            foreach(var a in filteredIds)
            {
                res = a;
                res.Claim();
            }

            return res;
        }

        public Argument GetLastArgNoClaim(params OptionSpecifier[] ids)
        {
            foreach(var a in filtered_reverse(ids))
            {
                return a;
            }

            return null;
        }

        public virtual string GetArgString(int index)
        {
            throw new NotImplementedException();
        }

        public virtual int GetNumInputArgStrings()
        {
            throw new NotImplementedException();
        }

        public string GetLastArgValue(OptionSpecifier id,
                                      string          Default = "")
        {
            var a = GetLastArg(id);

            if(a != null)
            {
                return a.GetValue();
            }

            return Default;
        }

        public List<string> GetAllArgValues(OptionSpecifier id)
        {
            var values = new List<string>(16);
            AddAllArgValues(values,
                            id);
            return new List<string>(values.Select(item => item.ToString()));
        }

        public bool HasFlag(OptionSpecifier pos,
                            OptionSpecifier neg,
                            bool            Default = true)
        {
            var a = GetLastArg(pos,
                               neg);
            if(a != null)
            {
                return a.GetOption().Matches(pos);
            }

            return Default;
        }

        public bool HasFlag(OptionSpecifier pos,
                            OptionSpecifier posAlias,
                            OptionSpecifier neg,
                            bool            Default = true)
        {
            var a = GetLastArg(pos,
                               posAlias,
                               neg);

            if(a != null)
            {
                return a.GetOption().Matches(pos) || a.GetOption().Matches(posAlias);
            }

            return Default;
        }

        /// AddLastArg - Render only the last argument match \p Id0, if present.
        public void AddLastArg(List<string>    output,
                               OptionSpecifier id0)
        {
            var a = GetLastArg(id0);

            if(a != null)
            {
                a.Claim();
                a.Render(this,
                         output);
            }
        }

        public void AddLastArg(List<string>    output,
                               OptionSpecifier id0,
                               OptionSpecifier id1)
        {
            var a = GetLastArg(id0,
                               id1);

            if(a != null)
            {
                a.Claim();
                a.Render(this,
                         output);
            }
        }

        /// AddAllArgsExcept - Render all arguments matching any of the given ids
        /// and not matching any of the excluded ids.
        public void AddAllArgsExcept(List<string>          output,
                                     List<OptionSpecifier> ids,
                                     List<OptionSpecifier> excludeIds)
        {
            foreach(var argument in _args)
            {
                var excluded = false;
                foreach(var id in excludeIds.Where(item => item != null))
                {
                    if(argument.GetOption().Matches(id))
                    {
                        excluded = true;
                        break;
                    }
                }

                if(!excluded)
                {
                    foreach(var id in ids.Where(item => item != null))
                    {
                        if(argument.GetOption().Matches(id))
                        {
                            argument.Claim();
                            argument.Render(this,
                                            output);
                            break;
                        }
                    }
                }
            }
        }

        /// AddAllArgs - Render all arguments matching any of the given ids.
        public void AddAllArgs(List<string>          output,
                               List<OptionSpecifier> ids)
        {
            var exclude = new List<OptionSpecifier>();
            AddAllArgsExcept(output,
                             ids,
                             exclude);
        }

        /// AddAllArgs - Render all arguments matching the given ids.
        public void AddAllArgs(List<string>    output,
                               OptionSpecifier id0,
                               OptionSpecifier id1 = null,
                               OptionSpecifier id2 = null)
        {
            var filteredIds = Filtered(id0,
                                       id1,
                                       id2);

            foreach(var argument in filteredIds)
            {
                argument.Claim();
                argument.Render(this,
                                output);
            }
        }

        /// AddAllArgValues - Render the argument values of all arguments
        /// matching the given ids.
        public void AddAllArgValues(List<string>    output,
                                    OptionSpecifier id0,
                                    OptionSpecifier id1 = null,
                                    OptionSpecifier id2 = null)
        {
            var filteredIds = Filtered(id0,
                                       id1,
                                       id2);

            foreach(var argument in filteredIds)
            {
                argument.Claim();
                var values = argument.GetValues();
                output.AddRange(values);
            }
        }

        /// AddAllArgsTranslated - Render all the arguments matching the
        /// given ids, but forced to separate args and using the provided
        /// name instead of the first option value.
        ///
        /// \param Joined - If true, render the argument as joined with
        /// the option specifier.
        public void AddAllArgsTranslated(List<string>    output,
                                         OptionSpecifier id0,
                                         string          translation,
                                         bool            joined = false)
        {
            var filteredIds = Filtered(id0);

            foreach(var argument in filteredIds)
            {
                argument.Claim();

                if(joined)
                {
                    output.Add(MakeArgString(translation + argument.GetValue()));
                }
                else
                {
                    output.Add(translation);
                    output.Add(argument.GetValue());
                }
            }
        }

        /// ClaimAllArgs - Claim all arguments which match the given
        /// option id.
        public void ClaimAllArgs(OptionSpecifier id0)
        {
            var filteredIds = Filtered(id0);

            foreach(var argument in filteredIds)
            {
                argument.Claim();
            }
        }

        /// ClaimAllArgs - Claim all arguments.
        ///
        public void ClaimAllArgs()
        {
            foreach(var argument in _args)
            {
                if(!argument.IsClaimed())
                {
                    argument.Claim();
                }
            }
        }

        /// @}
        /// @name Argument Synthesis
        /// @{
        /// Construct a constant string pointer whose
        /// lifetime will match that of the ArgList.       
        public virtual string MakeArgStringRef(string str)
        {
            throw new NotImplementedException();
        }

        public string MakeArgString(string str)
        {
            return MakeArgStringRef(str);
        }

        /// \brief Create an arg string for (\p LHS + \p RHS), reusing the
        /// string at \p Index if possible.
        public string GetOrMakeJoinedArgString(int    index,
                                               string lhs,
                                               string rhs)
        {
            var cur = GetArgString(index);

            if(cur.Length == lhs.Length + rhs.Length &&
               cur.StartsWith(lhs) &&
               cur.EndsWith(rhs))
            {
                return cur;
            }

            return MakeArgString(lhs + rhs);
        }

        public void Print(StringBuilder o)
        {
            foreach(var a in _args)
            {
                o.Append("* ");
                a.Print(o);
            }
        }

        public void Dump()
        {
            var o = new StringBuilder();
            Print(o);
            Debug.WriteLine(o);
        }

        public IEnumerator<Argument> GetEnumerator()
        {
            return _args.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}