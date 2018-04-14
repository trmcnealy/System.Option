using System.Diagnostics;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;

namespace System.Option
{
    [DebuggerDisplay("ID = {Info.Id}, Name = {Info.Name}, Values = {Info.Values?.ToString()}, AliasID = {Info.AliasId}, AliasArgs = {Info.AliasArgs?.ToString()}")]
    public class Option
    {
        protected readonly OptionInfo Info;

        protected readonly OptionTable Owner;

        public Option(OptionInfo  info,
                      OptionTable owner)
        {
            Info  = info;
            Owner = owner;

            // Multi-level aliases are not supported. This just simplifies option
            // tracking, it is not an inherent limitation.
            Contract.Assert((Info == null || !GetAlias().IsValid() || !GetAlias().GetAlias().IsValid()),
                            "Multi-level aliases are not supported.");

            if(Info != null &&
               GetAliasArgs() != null)
            {
                Contract.Assert(GetAlias().IsValid(),
                                "Only alias options can have alias args.");
                Contract.Assert(GetKind() == OptionKind.FlagClass,
                                "Only Flag aliases can have alias args.");
                Contract.Assert(GetAlias().GetKind() != OptionKind.FlagClass,
                                "Cannot provide alias args to a flag option.");
            }
        }

        public bool IsValid()
        {
            return Info != null;
        }

        public int GetId()
        {
            Contract.Assert(Info != null,
                            "Must have a valid info!");
            return Info.Id;
        }

        public OptionKind GetKind()
        {
            Contract.Assert(Info != null,
                            "Must have a valid info!");
            return Info.Kind;
        }

        /// \brief Get the name of this option without any prefix.
        public string GetName()
        {
            Contract.Assert(Info != null,
                            "Must have a valid info!");
            return Info.Name;
        }

        public Option GetGroup()
        {
            Contract.Assert(Info != null,
                            "Must have a valid info!");
            Contract.Assert(Owner != null,
                            "Must have a valid owner!");
            return Owner.GetOption(Info.GroupId);
        }

        public Option GetAlias()
        {
            Contract.Assert(Info != null,
                            "Must have a valid info!");
            Contract.Assert(Owner != null,
                            "Must have a valid owner!");
            return Owner.GetOption(Info.AliasId);
        }

        /// \brief Get the alias arguments as a \0 separated list.
        /// E.g. ["foo", "bar"] would be returned as "foo\0bar\0".
        public string GetAliasArgs()
        {
            Contract.Assert(Info != null,
                            "Must have a valid info!");
            Contract.Assert((Info.AliasArgs == null || Info.AliasArgs[0] != '\0'),
                            "AliasArgs should be either 0 or non-empty.");
            return Info.AliasArgs;
        }

        /// \brief Get the default prefix for this option.
        public string GetPrefix()
        {
            return Info.Prefixes[0];
        }

        /// \brief Get the name of this option with the default prefix.
        public string GetPrefixedName()
        {
            var ret = GetPrefix();
            ret += GetName();
            return ret;
        }

        public int GetNumArgs()
        {
            return Info.Param;
        }

        public bool HasNoOptAsInput()
        {
            return (Info.Flags & DriverFlag.RenderAsInput) != DriverFlag.RenderAsInput;
        }

        public RenderStyleKind GetRenderStyle()
        {
            if((Info.Flags & DriverFlag.RenderJoined) == DriverFlag.RenderJoined)
            {
                return RenderStyleKind.RenderJoinedStyle;
            }

            if((Info.Flags & DriverFlag.RenderSeparate) == DriverFlag.RenderSeparate)
            {
                return RenderStyleKind.RenderSeparateStyle;
            }

            var kind = GetKind();

            if(kind == OptionKind.GroupClass ||
               kind == OptionKind.InputClass ||
               kind == OptionKind.UnknownClass)
                return RenderStyleKind.RenderValuesStyle;
            if(kind == OptionKind.JoinedClass ||
               kind == OptionKind.JoinedAndSeparateClass)
                return RenderStyleKind.RenderJoinedStyle;
            if(kind == OptionKind.CommaJoinedClass)
                return RenderStyleKind.RenderCommaJoinedStyle;
            if(kind == OptionKind.FlagClass ||
               kind == OptionKind.ValuesClass ||
               kind == OptionKind.SeparateClass ||
               kind == OptionKind.MultiArgClass ||
               kind == OptionKind.JoinedOrSeparateClass ||
               kind == OptionKind.RemainingArgsClass ||
               kind == OptionKind.RemainingArgsJoinedClass)
                return RenderStyleKind.RenderSeparateStyle;

            throw new Exception("Unexpected kind!");
        }

        /// Test if this option has the flag \a Val.
        public bool HasFlag(int val)
        {
            return (Info.Flags & val) != 0;
        }

        /// getUnaliasedOption - Return the final option this option
        /// aliases (itself, if the option has no alias).
        public Option GetUnaliasedOption()
        {
            var alias = GetAlias();

            if(alias.IsValid())
            {
                return alias.GetUnaliasedOption();
            }

            return this;
        }

        /// getRenderName - Return the name to use when rendering this
        /// option.
        public string GetRenderName()
        {
            return GetUnaliasedOption().GetName();
        }

        /// matches - Predicate for whether this option is part of the
        /// given option (which may be a group).
        /// 
        /// Note that matches against options which are an alias should never be
        /// done -- aliases do not participate in matching and so such a query will
        /// always be false.
        public bool Matches(OptionSpecifier opt)
        {
            // Aliases are never considered in matching, look through them.
            var alias = GetAlias();

            if(alias.IsValid())
            {
                return alias.Matches(opt);
            }

            // Check exact match.
            if(GetId() == opt.GetId())
            {
                return true;
            }

            var @group = GetGroup();

            if(@group.IsValid())
            {
                return @group.Matches(opt);
            }

            return false;
        }

        /// accept - Potentially accept the current argument, returning a
        /// new Argument instance, or 0 if the option does not accept this
        /// argument (or the argument is missing values).
        /// 
        /// If the option accepts the current argument, accept() sets
        /// Index to the position where argument parsing should resume
        /// (even if the argument is missing values).
        /// 
        /// \param ArgSize The number of bytes taken up by the matched Option prefix
        /// and name. This is used to determine where joined values
        /// start.
        public Argument Accept(ArgumentList args,
                               ref int      index,
                               int          argSize)
        {
            var    unaliasedOption = GetUnaliasedOption();
            string spelling;

            // If the option was an alias, get the spelling from the unaliased one.
            if(GetId() == unaliasedOption.GetId())
            {
                spelling = args.GetArgString(index).Substring(0,
                                                              argSize);
            }
            else
            {
                spelling = args.MakeArgString(unaliasedOption.GetPrefix() + unaliasedOption.GetName());
            }

            var kind = GetKind();

            if(kind == OptionKind.FlagClass)
            {
                if(argSize != args.GetArgString(index).Length)
                {
                    return null;
                }

                var a = new Argument(unaliasedOption,
                                     spelling,
                                     index++);

                if(!string.IsNullOrEmpty(GetAliasArgs()))
                {
                    var vals = GetAliasArgs().Split(new[] {" "},
                                                    StringSplitOptions.RemoveEmptyEntries);

                    a.GetValues().AddRange(vals.Select(item => item));

                    //while (*Val != '\0')
                    //{
                    //    A.getValues().AddRange(Val);

                    //    // Move past the '\0' to the next argument.
                    //    Val += Val.Length + 1;
                    //}
                }

                if (unaliasedOption.GetKind() == OptionKind.JoinedClass &&
                   string.IsNullOrEmpty(GetAliasArgs()))
                {
                    a.GetValues().Add(string.Empty);
                }

                return a;
            }

            if(kind == OptionKind.JoinedClass)
            {
                string value = args.GetArgString(index).Substring(argSize);
                return new Argument(unaliasedOption,
                                    spelling,
                                    index++,
                                    value);
            }

            if(kind == OptionKind.CommaJoinedClass)
            {
                // Always matches.
                var str = args.GetArgString(index).Substring(argSize).
                               Split(new[] {','},
                                     StringSplitOptions.RemoveEmptyEntries); // + ArgSize;

                var a = new Argument(unaliasedOption,
                                     spelling,
                                     index++);

                a.GetValues().AddRange(str.Select(item => item));

                a.SetOwnsValues(true);

                return a;
            }

            if(kind == OptionKind.SeparateClass)
            {
                // Matches iff this is an exact match.
                // FIXME: Avoid strlen.
                if(argSize != args.GetArgString(index).Length)
                {
                    return null;
                }

                index += 2;
                if(index > args.GetNumInputArgStrings() ||
                   args.GetArgString(index - 1) == null)
                {
                    return null;
                }

                return new Argument(unaliasedOption,
                                    spelling,
                                    index - 2,
                                    args.GetArgString(index - 1));
            }

            if(kind == OptionKind.MultiArgClass)
            {
                // Matches iff this is an exact match.
                // FIXME: Avoid strlen.
                if(argSize != args.GetArgString(index).Length)
                {
                    return null;
                }

                index += 1 + GetNumArgs();
                if(index > args.GetNumInputArgStrings())
                {
                    return null;
                }

                var a = new Argument(unaliasedOption,
                                     spelling,
                                     index - 1 - GetNumArgs(),
                                     args.GetArgString(index - GetNumArgs()));
                for(var i = 1; i != GetNumArgs(); ++i)
                {
                    a.GetValues().Add(args.GetArgString(index - GetNumArgs() + i));
                }

                return a;
            }

            if(kind == OptionKind.JoinedOrSeparateClass)
            {
                // If this is not an exact match, it is a joined arg.
                // FIXME: Avoid strlen.
                if(argSize != args.GetArgString(index).Length)
                {
                    var value = args.GetArgString(index).Substring(argSize);
                    return new Argument(this,
                                        spelling,
                                        index++,
                                        value);
                }

                // Otherwise it must be separate.
                index += 2;
                if(index > args.GetNumInputArgStrings() ||
                   args.GetArgString(index - 1) == null)
                {
                    return null;
                }

                return new Argument(unaliasedOption,
                                    spelling,
                                    index - 2,
                                    args.GetArgString(index - 1));
            }

            if(kind == OptionKind.JoinedAndSeparateClass)
            {
                // Always matches.
                index += 2;
                if(index > args.GetNumInputArgStrings() ||
                   args.GetArgString(index - 1) == null)
                {
                    return null;
                }

                return new Argument(unaliasedOption,
                                    spelling,
                                    index - 2,
                                    args.GetArgString(index - 2).Substring(argSize),
                                    args.GetArgString(index - 1));
            }

            if(kind == OptionKind.RemainingArgsClass)
            {
                // Matches iff this is an exact match.
                // FIXME: Avoid strlen.
                if(argSize != args.GetArgString(index).Length)
                {
                    return null;
                }

                var a = new Argument(unaliasedOption,
                                     spelling,
                                     index++);
                while(index < args.GetNumInputArgStrings() &&
                      args.GetArgString(index) != null)
                {
                    a.GetValues().Add(args.GetArgString(index++));
                }

                return a;
            }

            if(kind == OptionKind.RemainingArgsJoinedClass)
            {
                var a = new Argument(unaliasedOption,
                                     spelling,
                                     index);
                if(argSize != args.GetArgString(index).Length)
                {
                    a.GetValues().Add(args.GetArgString(index).Substring(argSize));
                }

                index++;
                while(index < args.GetNumInputArgStrings() &&
                      args.GetArgString(index) != null)
                {
                    a.GetValues().Add(args.GetArgString(index++));
                }

                return a;
            }

            throw new Exception("Invalid option kind!");
        }

        public void Print(StringBuilder o)
        {
            o.Append("<");

            var kind = GetKind();

            if(kind == OptionKind.GroupClass)
            {
                o.Append("GroupClass");
            }
            else if(kind == OptionKind.InputClass)
            {
                o.Append("InputClass");
            }
            else if(kind == OptionKind.UnknownClass)
            {
                o.Append("UnknownClass");
            }
            else if(kind == OptionKind.FlagClass)
            {
                o.Append("FlagClass");
            }
            else if(kind == OptionKind.JoinedClass)
            {
                o.Append("JoinedClass");
            }
            else if(kind == OptionKind.ValuesClass)
            {
                o.Append("ValuesClass");
            }
            else if(kind == OptionKind.SeparateClass)
            {
                o.Append("SeparateClass");
            }
            else if(kind == OptionKind.CommaJoinedClass)
            {
                o.Append("CommaJoinedClass");
            }
            else if(kind == OptionKind.MultiArgClass)
            {
                o.Append("MultiArgClass");
            }
            else if(kind == OptionKind.JoinedOrSeparateClass)
            {
                o.Append("JoinedOrSeparateClass");
            }
            else if(kind == OptionKind.JoinedAndSeparateClass)
            {
                o.Append("JoinedAndSeparateClass");
            }
            else if(kind == OptionKind.RemainingArgsClass)
            {
                o.Append("RemainingArgsClass");
            }
            else if(kind == OptionKind.RemainingArgsJoinedClass)
            {
                o.Append("RemainingArgsJoinedClass");
            }

            if(Info.Prefixes != null)
            {
                o.Append(" Prefixes:[");

                string pre;
                for(var i = 0; i < Info.Prefixes.Length; i++)
                {
                    pre = Info.Prefixes[i];

                    if(!string.IsNullOrEmpty(pre))
                    {
                        o.Append($"\" {pre} {(i + 1 < Info.Prefixes.Length ? "\"" : "\", ")}");
                    }
                }

                o.Append(']');
            }

            o.Append($" Name:\" {GetName()} \"");

            var @group = GetGroup();
            if(@group.IsValid())
            {
                o.Append(" Group:");
                @group.Print(o);
            }

            var alias = GetAlias();
            if(alias.IsValid())
            {
                o.Append(" Alias:");
                alias.Print(o);
            }

            if(GetKind() == OptionKind.MultiArgClass)
            {
                o.Append($" NumArgs:{GetNumArgs()}");
            }

            o.Append(">\n");
        }

        public void Dump()
        {
            StringBuilder o = new StringBuilder();
            Print(o);
            Debug.WriteLine(o);
        }

        public override string ToString()
        {
            StringBuilder o = new StringBuilder();
            Print(o);
            return o.ToString();
        }
    }
}