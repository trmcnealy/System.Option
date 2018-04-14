using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;

namespace System.Option
{
    public class OptionTable
    {
        /// \brief The static option information table.
        private List<OptionInfo> _optionInfos;

        private bool _ignoreCase;

        private int _theInputOptionId;

        private int _theUnknownOptionId;

        /// The index of the first option which can be parsed (i.e., is not a
        /// special option like 'input' or 'unknown', and is not an option group).
        private int _firstSearchableIndex;

        /// The union of all option prefixes. If an argument does not begin with
        /// one of these, it is an input.
        private HashSet<string> _prefixesUnion = new HashSet<string>();

        private HashSet<char> _prefixChars = new HashSet<char>();

        private OptionInfo GetInfo(OptionSpecifier opt)
        {
            int id = opt.GetId();

            //Debug.Assert(id > 0  id - 1 < getNumOptions()  ,"Invalid Option ID.");
            return _optionInfos[id - 1];
        }

        public OptionTable(OptionInfo[] optionInfos,
                           bool         ignoreCase = false)
                : this(optionInfos.ToList(),
                       ignoreCase)
        {
        }

        public OptionTable(List<OptionInfo> optionInfos,
                           bool             ignoreCase = false)
        {
            _optionInfos = optionInfos;
            _ignoreCase  = ignoreCase;

            // Find start of normal options.
            for(int i = 0, e = GetNumOptions(); i != e; ++i)
            {
                byte kind = GetInfo(i + 1).Kind;
                if(kind == OptionKind.InputClass)
                {
                    Debug.Assert(_theInputOptionId == 0,
                                 "Cannot have multiple input options!");
                    _theInputOptionId = GetInfo(i + 1).Id;
                }
                else if(kind == OptionKind.UnknownClass)
                {
                    Debug.Assert(_theUnknownOptionId == 0,
                                 "Cannot have multiple unknown options!");
                    _theUnknownOptionId = GetInfo(i + 1).Id;
                }
                else if(kind != OptionKind.GroupClass)
                {
                    _firstSearchableIndex = i;
                    break;
                }
            }

            Debug.Assert(_firstSearchableIndex != 0,
                         "No searchable options?");

            //#if DEBUG
            //            // Check that everything after the first searchable option is a
            //            // regular option class.
            //            for (int i = FirstSearchableIndex, e = getNumOptions(); i != e; ++i)
            //            {
            //                OptionKind kind = (OptionKind)getInfo(i + 1).Kind;
            //                Debug.Assert((kind != OptionKind.InputClass &&
            //                              kind != OptionKind.UnknownClass &&
            //                              kind != OptionKind.GroupClass),
            //                             "Special options should be defined first!");
            //            }

            //            // Check that options are in order.
            //            for (int i = FirstSearchableIndex + 1, e = getNumOptions(); i != e; ++i)
            //            {
            //                if (!(getInfo(i) < getInfo(i + 1)))
            //                {
            //                    getOption(i).dump();
            //                    getOption(i + 1).dump();
            //                    throw new Exception("Options are not in order!");
            //                }
            //            }
            //#endif

            // Build prefixes.
            for(int i = _firstSearchableIndex + 1, e = GetNumOptions() + 1; i != e; ++i)
            {
                string[] p = GetInfo(i).Prefixes;

                if(p != null)
                {
                    foreach(string c in p)
                    {
                        if(!string.IsNullOrEmpty(c))
                        {
                            _prefixesUnion.Add(c);
                        }
                    }
                }
            }

            foreach(string I in _prefixesUnion)
            {
                foreach(char c in I)
                {
                    if(!_prefixChars.Contains(c))
                    {
                        _prefixChars.Add(c);
                    }
                }
            }
        }

        //public ~OptionTable();

        /// \brief Return the total number of option classes.
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int GetNumOptions()
        {
            return _optionInfos.Count;
        }

        /// \brief Get the given Opt's Option instance, lazily creating it
        /// if necessary.
        ///
        /// <returns>The option, or null for the INVALID option id.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Option GetOption(OptionSpecifier opt)
        {
            var id = opt.GetId();

            if(id == 0)
            {
                return new Option(null,
                                  null);
            }

            Debug.Assert((id - 1) < GetNumOptions(),
                         "Invalid ID.");

            return new Option(GetInfo(id),
                              this);
        }

        /// \brief Lookup the name of the given option.
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public string GetOptionName(OptionSpecifier id)
        {
            return GetInfo(id).Name;
        }

        /// \brief Get the kind of the given option.
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int GetOptionKind(OptionSpecifier id)
        {
            return GetInfo(id).Kind;
        }

        /// \brief Get the group id for the given option.
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int GetOptionGroupId(OptionSpecifier id)
        {
            return GetInfo(id).GroupId;
        }

        /// \brief Get the help text to use to describe this option.
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public string GetOptionHelpText(OptionSpecifier id)
        {
            return GetInfo(id).HelpText;
        }

        /// \brief Get the meta-variable name to use when describing
        /// this options values in the help text.
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public string GetOptionMetaVar(OptionSpecifier id)
        {
            return GetInfo(id).MetaVar;
        }

        /// Find possible value for given flags. This is used for shell
        /// autocompletion.
        ///
        /// \param [in] Option - Key flag like "-stdlib=" when "-stdlib=l"
        /// was passed to clang.
        ///
        /// \param [in] Arg - Value which we want to autocomplete like "l"
        /// when "-stdlib=l" was passed to clang.
        ///
        /// <returns>The vector of possible values.</returns>
        public List<string> SuggestValueCompletions(string option,
                                                    string arg)
        {
            // Search all options and return possible values.
            foreach(var In in _optionInfos.Skip(_firstSearchableIndex))
            {
                if(!OptionMatches(In,
                                  option))
                {
                    continue;
                }

                //var Candidates = new List<string>(8);

                var candidates = In.Values.Split(new[] {","},
                                                 StringSplitOptions.RemoveEmptyEntries).ToList();

                var result = new List<string>();

                foreach(var val in candidates)
                {
                    if(val.StartsWith(arg))
                    {
                        result.Add(val);
                    }
                }

                return result;
            }

            return new List<string>();
        }

        /// Find flags from OptionTable which starts with Cur.
        ///
        /// \param [in] Cur - String prefix that all returned flags need
        //  to start with.
        ///
        /// <returns>The vector of flags which start with Cur.</returns>
        public List<string> FindByPrefix(string cur,
                                         ushort disableFlags)
        {
            var ret = new List<string>();

            foreach(var In in _optionInfos.Skip(_firstSearchableIndex))
            {
                if(In.Prefixes == null ||
                   In.HelpText == null && In.GroupId == 0)
                {
                    continue;
                }

                if((In.Flags & disableFlags) != 0)
                {
                    continue;
                }

                for(var I = 0; I < In.Prefixes.Length; I++)
                {
                    var s = In.Prefixes[I] + In.Name + "\t";

                    if(In.HelpText != null)
                    {
                        s += In.HelpText;
                    }

                    if(s.StartsWith(cur))
                    {
                        ret.Add(s);
                    }
                }
            }

            return ret;
        }

        /// Find the OptTable option that most closely matches the given string.
        ///
        /// \param [in] Option - A string, such as "-stdlibs=l", that represents user
        /// input of an option that may not exist in the OptTable. Note that the
        /// string includes prefix dashes "-" as well as values "=l".
        /// \param [out] NearestString - The nearest option string found in the
        /// OptTable.
        /// \param [in] FlagsToInclude - Only find options with any of these flags.
        /// Zero is the default, which includes all flags.
        /// \param [in] FlagsToExclude - Don't find options with this flag. Zero
        /// is the default, and means exclude nothing.
        /// \param [in] MinimumLength - Don't find options shorter than this length.
        /// For example, a minimum length of 3 prevents "-x" from being considered
        /// near to "-S".
        ///
        /// <returns>The edit distance of the nearest string found.</returns>
        public int FindNearest(string option,
                               ref string nearestString,
                               int    flagsToInclude = 0,
                               int    flagsToExclude = 0,
                               int    minimumLength  = 4)

        {
            //assert(!Option.empty());

            // Consider each option as a candidate, finding the closest match.
            int bestDistance = int.MaxValue;

            foreach(OptionInfo candidateInfo in _optionInfos.Skip(_firstSearchableIndex))
            {
                string candidateName = candidateInfo.Name;

                // Ignore option candidates with empty names, such as "--", or names
                // that do not meet the minimum length.
                if(!string.IsNullOrEmpty(candidateName) || candidateName.Length < minimumLength)
                {
                    continue;
                }

                // If FlagsToInclude were specified, ignore options that don't include
                // those flags.
                if(flagsToInclude > 0 && (candidateInfo.Flags & flagsToInclude) == 0)
                {
                    continue;
                }

                // Ignore options that contain the FlagsToExclude.
                if((candidateInfo.Flags & flagsToExclude) != 0)
                {
                    continue;
                }

                // Ignore positional argument option candidates (which do not
                // have prefixes).
                if(candidateInfo.Prefixes == null)
                {
                    continue;
                }

                // Find the most appropriate prefix. For example, if a user asks for
                // "--helm", suggest "--help" over "-help".
                string prefix = candidateInfo.Prefixes[0];

                for(int p = 1; candidateInfo.Prefixes[p] != null; p++)
                {
                    if(option.StartsWith(candidateInfo.Prefixes[p]))
                        prefix = candidateInfo.Prefixes[p];
                }

                // Check if the candidate ends with a character commonly used when
                // delimiting an option from its value, such as '=' or ':'. If it does,
                // attempt to split the given option based on that delimiter.
                string delimiter = "";
                char   last      = candidateName.Last();

                if(last == '=' || last == ':')
                {
                    delimiter += last;
                }

                string lhs = string.Empty;
                string rhs = string.Empty;

                if(string.IsNullOrEmpty(delimiter))
                {
                    lhs = option;
                }
                else
                {
                    (lhs, rhs) = option.SplitInTwo(last);
                }

                string normalizedName = (lhs.Skip(prefix.Length) + delimiter);

                int distance = candidateName.EditDistance(normalizedName, /*AllowReplacements=*/
                                                          true,           /*MaxEditDistance=*/
                                                          bestDistance);

                if(distance < bestDistance)
                {
                    bestDistance  = distance;
                    nearestString = (prefix + candidateName + rhs);
                }
            }

            return bestDistance;
        }

        /// Add Values to Option's Values class
        ///
        /// \param [in] Option - Prefix + Name of the flag which Values will be
        ///  changed. For example, "-analyzer-checker".
        /// \param [in] Values - String of Values seperated by ",", such as
        ///  "foo, bar..", where foo and bar is the argument which the Option flag
        ///  takes
        ///
        /// <returns>true in success, and false in fail.</returns>
        public bool AddValues(string option,
                              string values)
        {
            for(int I = _firstSearchableIndex, e = _optionInfos.Count; I < e; I++)
            {
                OptionInfo In = _optionInfos[I];
                if(OptionMatches(In,
                                 option))
                {
                    In.Values = values;
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="prefixes"></param>
        /// <param name="arg"></param>
        /// <returns></returns>
        public static bool IsInput(HashSet<string> prefixes,
                                   string          arg)
        {
            if(arg == "-")
            {
                return true;
            }

            foreach(var I in prefixes)
            {
                if(arg.StartsWith(I))
                {
                    return false;
                }
            }

            return true;
        }

        /// <returns>Matched size. 0 means no match.</returns>
        public static int MatchOption(OptionInfo I,
                                      string     str,
                                      bool       ignoreCase)
        {
            foreach(var pre in I.Prefixes)
            {
                if(pre != null)
                {
                    if(str.StartsWith(pre))
                    {
                        string rest = str.Substring(pre.Length);

                        bool matched = ignoreCase
                                               ? rest.StartsWith(I.Name,
                                                                 StringComparison.InvariantCultureIgnoreCase)
                                               : rest.StartsWith(I.Name);
                        if(matched)
                        {
                            return pre.Length + I.Name.Length;
                        }
                    }
                }
            }

            return 0;
        }

        // Returns true if one of the Prefixes + In.Names matches Option
        public static bool OptionMatches(OptionInfo In,
                                         string     option)
        {
            if(In.Values != null && In.Prefixes != null)
            {
                for(uint I = 0; In.Prefixes[I] != null; I++)
                {
                    if(option == In.Prefixes[I] + In.Name)
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        /// \brief Parse a single argument; returning the new argument and
        /// updating Index.
        ///
        /// \param [in,out] Index - The current parsing position in the argument
        /// string list; on return this will be the index of the next argument
        /// string to parse.
        /// \param [in] FlagsToInclude - Only parse options with any of these flags.
        /// Zero is the default which includes all flags.
        /// \param [in] FlagsToExclude - Don't parse options with this flag.  Zero
        /// is the default and means exclude nothing.
        ///
        /// <returns>The parsed argument, or 0 if the argument is missing values (in which case Index still points at the conceptual next argument string to parse).</returns>
        public Argument ParseOneArg(ArgumentList args,
                                    ref int      index,
                                    int          flagsToInclude = 0,
                                    int          flagsToExclude = 0)
        {
            int    prev = index;
            string str  = args.GetArgString(index);

            // Anything that doesn't start with PrefixesUnion is an input, as is '-'
            // itself.
            if(IsInput(_prefixesUnion,
                       str))
            {
                return new Argument(GetOption(_theInputOptionId),
                                    str,
                                    index++,
                                    str);
            }

            IEnumerable<OptionInfo> startEnd = _optionInfos.Skip(_firstSearchableIndex);

            string name = string.Concat(str.SkipWhile(item => _prefixChars.Any(chars => chars == item)));

            // Search for the first next option which could be a prefix.
            startEnd = startEnd.SkipWhile(item => !name.StartsWith(item.Name.ToString(),
                                                                   StringComparison.InvariantCultureIgnoreCase));

            // Options are stored in sorted order, with '\0' at the end of the
            // alphabet. Since the only options which can accept a string must
            // prefix it, we iteratively search for the next option which could
            // be a prefix.
            //
            // FIXME: This is searching much more than necessary, but I am
            // blanking on the simplest way to make it fast. We can solve this
            // problem when we move to TableGen.

            // Scan for first option which is a proper prefix.
            int i         = 0;
            var startList = startEnd.ToList();

            for(; i < startList.Count; i++)
            {
                int argSize = 0;

                // Scan for first option which is a proper prefix.
                for(; i < startList.Count; i++)
                {
                    argSize = MatchOption(startList[i],
                                          str,
                                          _ignoreCase);
                    if(argSize > 0)
                    {
                        break;
                    }
                }

                if(i == startList.Count)
                {
                    break;
                }

                Option opt = new Option(startList[i],
                                        this);

                if(flagsToInclude != 0 && !opt.HasFlag(flagsToInclude))
                {
                    continue;
                }

                if(opt.HasFlag(flagsToExclude))
                {
                    continue;
                }

                // See if this option matches.
                Argument a = opt.Accept(args,
                                        ref index,
                                        argSize);
                if(a != null)
                {
                    return a;
                }

                // Otherwise, see if this argument was missing values.
                if(prev != index)
                {
                    return null;
                }
            }

            // If we failed to find an option and this arg started with /, then it's
            // probably an input path.
            if(str[0] == '/')
            {
                return new Argument(GetOption(_theInputOptionId),
                                    str,
                                    index++,
                                    str);
            }

            return new Argument(GetOption(_theUnknownOptionId),
                                str,
                                index++,
                                str);
        }

        /// \brief Parse an list of arguments into an InputArgumentList.
        ///
        /// The resulting InputArgumentList will reference the strings in [\p ArgBegin,
        /// \p ArgEnd), and their lifetime should extend past that of the returned
        /// InputArgumentList.
        ///
        /// The only error that can occur in this routine is if an argument is
        /// missing values; in this case \p MissingArgCount will be non-zero.
        ///
        /// \param MissingArgIndex - On error, the index of the option which could
        /// not be parsed.
        /// \param MissingArgCount - On error, the number of missing options.
        /// \param FlagsToInclude - Only parse options with any of these flags.
        /// Zero is the default which includes all flags.
        /// \param FlagsToExclude - Don't parse options with this flag.  Zero
        /// is the default and means exclude nothing.
        /// <returns>An InputArgumentList; on error this will contain all the options which could be parsed.</returns>
        public InputArgumentList ParseArgs(string[] argArr,
                                           out int  missingArgIndex,
                                           out int  missingArgCount,
                                           int      flagsToInclude = 0,
                                           int      flagsToExclude = 0)
        {
            return ParseArgs(argArr.ToList(),
                             out missingArgIndex,
                             out missingArgCount,
                             flagsToInclude,
                             flagsToExclude);
        }

        public InputArgumentList ParseArgs(List<string> argArr,
                                           out int      missingArgIndex,
                                           out int      missingArgCount,
                                           int          flagsToInclude = 0,
                                           int          flagsToExclude = 0)
        {
            InputArgumentList args = new InputArgumentList(argArr);

            // FIXME: Handle '@' args (or at least error on them).

            missingArgIndex = missingArgCount = 0;
            int index                         = 0;
            int end                           = argArr.Count;

            while(index < end)
            {
                // Ingore nullptrs, they are response file's EOL markers
                if(args.GetArgString(index) == null)
                {
                    ++index;
                    continue;
                }

                // Ignore empty arguments (other things may still take them as arguments).
                string str = args.GetArgString(index);

                if(string.IsNullOrEmpty(str))
                {
                    ++index;
                    continue;
                }

                int prev = index;

                Argument a = ParseOneArg(args,
                                         ref index,
                                         flagsToInclude,
                                         flagsToExclude);

                //Debug.Assert(index > Prev && "Parser failed to consume argument.");

                // Check for missing argument error.
                if(a == null)
                {
                    //Debug.Assert(index >= End && "Unexpected parser error.");
                    //Debug.Assert(index - Prev - 1 && "No missing arguments!");
                    missingArgIndex = prev;
                    missingArgCount = index - prev - 1;
                    break;
                }

                args.Append(a);
            }

            return args;
        }

        public struct Info
        {
            public string Name;

            public string HelpText;

            public Info(string name,
                        string helpText)
            {
                Name     = name;
                HelpText = helpText;
            }
        }

        public static void PrintHelpOptionList(StringBuilder os,
                                               string        title,
                                               List<Info>    optionHelp)
        {
            os.Append(title + ":\n");

            // Find the maximum option length.
            var optionFieldWidth = 0;
            for(int i = 0, e = optionHelp.Count; i != e; ++i)
            {
                // Limit the amount of padding we are willing to give up for alignment.
                var length = optionHelp[i].Name.Length;
                if(length <= 23)
                {
                    optionFieldWidth = Math.Max(optionFieldWidth,
                                                length);
                }
            }

            var initialPad = 2;
            for(int i = 0, e = optionHelp.Count; i != e; ++i)
            {
                var option = optionHelp[i].Name;
                var pad    = optionFieldWidth - option.Length;
                os.Append($"  {option}");

                // Break on long option names.
                if(pad < 0)
                {
                    os.Append("\n");
                    pad = optionFieldWidth + initialPad;
                }

                for(var j = 0; j < pad + 1; j++)
                {
                    os.Append(" ");
                }

                os.Append(optionHelp[i].HelpText + '\n');
            }
        }

        public static string GetOptionHelpGroup(OptionTable     opts,
                                                OptionSpecifier id)
        {
            var groupId = opts.GetOptionGroupId(id);

            // If not in a group, return the default help group.
            if(groupId == 0)
            {
                return "OPTIONS";
            }

            // Abuse the help text of the option groups to store the "help group"
            // name.
            //
            // FIXME: Split out option groups.
            var groupHelp = opts.GetOptionHelpText(groupId);
            if(!string.IsNullOrEmpty(groupHelp))
            {
                return groupHelp;
            }

            // Otherwise keep looking.
            return GetOptionHelpGroup(opts,
                                      groupId);
        }

        public static string GetOptionHelpName(OptionTable     opts,
                                               OptionSpecifier id)
        {
            var    o    = opts.GetOption(id);
            var    name = o.GetPrefixedName();
            string metaVarName;

            // Add metavar, if used.

            var kind = o.GetKind();

            if(kind == OptionKind.GroupClass ||
               kind == OptionKind.InputClass ||
               kind == OptionKind.UnknownClass)
            {
                throw new Exception("Invalid option with help text.");
            }

            if(kind == OptionKind.MultiArgClass)
            {
                metaVarName = opts.GetOptionMetaVar(id);
                if(!string.IsNullOrEmpty(metaVarName))
                {
                    // For MultiArgs, metavar is full list of all argument names.
                    name += ' ';
                    name += metaVarName;
                }
                else
                {
                    // For MultiArgs<N>, if metavar not supplied, print <value> N times.
                    for(int i = 0, e = o.GetNumArgs(); i < e; ++i)
                    {
                        name += " <value>";
                    }
                }
            }
            else if(kind == OptionKind.FlagClass)
            {
            }
            else if(kind == OptionKind.ValuesClass)
            {
            }
            else if(kind == OptionKind.SeparateClass ||
                    kind == OptionKind.JoinedOrSeparateClass ||
                    kind == OptionKind.RemainingArgsClass ||
                    kind == OptionKind.RemainingArgsJoinedClass)
            {
                name        += ' ';
                metaVarName =  opts.GetOptionMetaVar(id);
                if(!string.IsNullOrEmpty(metaVarName))
                {
                    name += metaVarName;
                }
                else
                {
                    name += "<value>";
                }
            }
            else if(kind == OptionKind.JoinedClass ||
                    kind == OptionKind.CommaJoinedClass ||
                    kind == OptionKind.JoinedAndSeparateClass)
            {
                metaVarName = opts.GetOptionMetaVar(id);
                if(!string.IsNullOrEmpty(metaVarName))
                {
                    name += metaVarName;
                }
                else
                {
                    name += "<value>";
                }
            }

            return name;
        }

        /// \brief Render the help text for an option table.
        ///
        /// \param OS - The stream to write the help text to.
        /// \param Name - The name to use in the usage line.
        /// \param Title - The title to use in the usage line.
        /// \param FlagsToInclude - If non-zero, only include options with any
        ///                         of these flags set.
        /// \param FlagsToExclude - Exclude options with any of these flags set.
        public void PrintHelp(StringBuilder os,
                              string        name,
                              string        title,
                              int           flagsToInclude,
                              int           flagsToExclude)
        {
            os.Append("OVERVIEW: " + title + "\n");
            os.Append('\n');
            os.Append("USAGE: " + name + " [options] <inputs>\n");
            os.Append('\n');

            // Render help text into a map of group-name to a list of (option, help)
            // pairs.
            var groupedOptionHelp = new Dictionary<string, List<Info>>();

            for(int i = 0, e = GetNumOptions(); i != e; ++i)
            {
                var id = i + 1;

                // FIXME: Split out option groups.
                if(GetOptionKind(id) == (int)OptionKind.GroupClass)
                {
                    continue;
                }

                int flags = GetInfo(id).Flags;
                if(flagsToInclude == 1 &&
                   (flags & flagsToInclude) != 0)
                {
                    continue;
                }

                if((flags & flagsToExclude) != 0)
                {
                    continue;
                }

                var text = GetOptionHelpText(id);
                if(!string.IsNullOrEmpty(text))
                {
                    var helpGroup = GetOptionHelpGroup(this,
                                                       id);
                    var optName = GetOptionHelpName(this,
                                                    id);

                    if(!groupedOptionHelp.ContainsKey(helpGroup))
                    {
                        groupedOptionHelp.Add(helpGroup,
                                              new List<Info>()
                                              {
                                                      new Info(optName,
                                                               text)
                                              });
                    }
                    else
                    {
                        groupedOptionHelp[helpGroup].Add(new Info(optName,
                                                                  text));
                    }
                }
            }

            foreach(var it in groupedOptionHelp)
            {
                os.Append("\n");

                PrintHelpOptionList(os,
                                    it.Key,
                                    it.Value);
            }

            //OS.flush();
        }

        public void PrintHelp(StringBuilder os,
                              string        name,
                              string        title,
                              bool          showHidden = false)
        {
            PrintHelp(os,
                      name,
                      title,
                      0,
                      showHidden ? 0 : (int)DriverFlag.HelpHidden); //Exclude
        }
    }
}