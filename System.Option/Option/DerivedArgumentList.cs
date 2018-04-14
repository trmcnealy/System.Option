using System.Collections.Generic;
using System.Linq;

namespace System.Option
{
    public sealed class DerivedArgumentList : ArgumentList
    {
        private readonly InputArgumentList _baseArgs;

        /// The list of arguments we synthesized.
        private List<Argument> _synthesizedArgs;

        /// Construct a new derived arg list from \p BaseArgs.
        public DerivedArgumentList(InputArgumentList baseArgs)
        {
            _baseArgs = baseArgs;
        }

        public override string GetArgString(int index)
        {
            return _baseArgs.GetArgString(index);
        }

        public override int GetNumInputArgStrings()
        {
            return _baseArgs.GetNumInputArgStrings();
        }

        public InputArgumentList GetBaseArgs()
        {
            return _baseArgs;
        }

        /// @name Arg Synthesis
        /// @{
        /// AddSynthesizedArg - Add a argument to the list of synthesized arguments
        /// (to be freed).
        public void AddSynthesizedArg(Argument a)
        {
            _synthesizedArgs.Add(new Argument(a));
        }

        //using ArgList::MakeArgString;
        public override string MakeArgStringRef(string str)
        {
            return _baseArgs.MakeArgString(str);
        }

        /// AddFlagArg - Construct a new FlagArg for the given option \p Id and
        /// append it to the argument list.
        public void AddFlagArg(Argument baseArg,
                               Option   opt)
        {
            Append(MakeFlagArg(baseArg,
                               opt));
        }

        /// AddPositionalArg - Construct a new Positional arg for the given option
        /// \p Id, with the provided \p Value and append it to the argument
        /// list.
        public void AddPositionalArg(Argument baseArg,
                                     Option   opt,
                                     string   value)
        {
            Append(MakePositionalArg(baseArg,
                                     opt,
                                     value));
        }

        /// AddSeparateArg - Construct a new Positional arg for the given option
        /// \p Id, with the provided \p Value and append it to the argument
        /// list.
        private void AddSeparateArg(Argument baseArg,
                                    Option   opt,
                                    string   value)
        {
            Append(MakeSeparateArg(baseArg,
                                   opt,
                                   value));
        }

        /// AddJoinedArg - Construct a new Positional arg for the given option
        /// \p Id, with the provided \p Value and append it to the argument list.
        public void AddJoinedArg(Argument baseArg,
                                 Option   opt,
                                 string   value)
        {
            Append(MakeJoinedArg(baseArg,
                                 opt,
                                 value));
        }

        /// MakeFlagArg - Construct a new FlagArg for the given option \p Id.
        public Argument MakeFlagArg(Argument baseArg,
                                    Option   opt)
        {
            _synthesizedArgs.Add(new Argument(opt,
                                             MakeArgString(opt.GetPrefix() + opt.GetName()),
                                             _baseArgs.MakeIndex(opt.GetName()),
                                             baseArg));
            return _synthesizedArgs.Last();
        }

        /// MakePositionalArg - Construct a new Positional arg for the
        /// given option \p Id, with the provided \p Value.
        public Argument MakePositionalArg(Argument baseArg,
                                          Option   opt,
                                          string   value)
        {
            int index = _baseArgs.MakeIndex(value);
            _synthesizedArgs.Add(
                                new Argument(opt,
                                             MakeArgString(opt.GetPrefix() + opt.GetName()),
                                             index,
                                             _baseArgs.GetArgString(index),
                                             baseArg));
            return _synthesizedArgs.Last();
        }

        /// MakeSeparateArg - Construct a new Positional arg for the
        /// given option \p Id, with the provided \p Value.
        public Argument MakeSeparateArg(Argument baseArg,
                                        Option   opt,
                                        string   value)
        {
            int index = _baseArgs.MakeIndex(opt.GetName(),
                                           value);
            _synthesizedArgs.Add(
                                new Argument(opt,
                                             MakeArgString(opt.GetPrefix() + opt.GetName()),
                                             index,
                                             _baseArgs.GetArgString(index + 1),
                                             baseArg));
            return _synthesizedArgs.Last();
        }

        /// MakeJoinedArg - Construct a new Positional arg for the
        /// given option \p Id, with the provided \p Value.
        public Argument MakeJoinedArg(Argument baseArg,
                                      Option   opt,
                                      string   value)
        {
            int index = _baseArgs.MakeIndex(opt.GetName() + value);
            _synthesizedArgs.Add(new Argument(
                                             opt,
                                             MakeArgString(opt.GetPrefix() + opt.GetName()),
                                             index,
                                             _baseArgs.GetArgString(index) + opt.GetName().Length,
                                             baseArg));
            return _synthesizedArgs.Last();
        }
    }
}