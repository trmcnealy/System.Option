using System.Collections.Generic;
using System.Linq;

namespace System.Option
{
    public sealed class InputArgumentList : ArgumentList
    {
        /// List of argument strings used by the contained Args.
        ///
        /// This is mutable since we treat the ArgList as being the list
        /// of Args, and allow routines to add new strings (to have a
        /// convenient place to store the memory) via MakeIndex.
        private List<string> _argStrings = new List<string>();

        /// Strings for synthesized arguments.
        ///
        /// This is mutable since we treat the ArgList as being the list
        /// of Args, and allow routines to add new strings (to have a
        /// convenient place to store the memory) via MakeIndex.
        private List<string> _synthesizedStrings = new List<string>();

        /// The number of original input argument strings.
        private int _numInputArgStrings;

        /// Release allocated arguments.
        private void ReleaseMemory()
        {
            throw new NotImplementedException();

            //for (Arg *A : *this)
            //delete A;
        }

        public InputArgumentList(IEnumerable<string> args)
        {
            _argStrings.AddRange(args);
            _numInputArgStrings = _argStrings.Count;
        }

        //public ~InputArgumentList() { releaseMemory(); }

        public override string GetArgString(int index)
        {
            return _argStrings[index];
        }

        public override int GetNumInputArgStrings()
        {
            return _numInputArgStrings;
        }

        /// @name Arg Synthesis
        /// @{
        /// MakeIndex - Get an index for the given string(s).
        public int MakeIndex(string string0)
        {
            int index = _argStrings.Count;

            // Tuck away so we have a reliable const char *.
            _synthesizedStrings.Add(string0);
            _argStrings.Add(_synthesizedStrings.Last());

            return index;
        }

        public int MakeIndex(string string0,
                             string string1)
        {
            int index0 = MakeIndex(string0);
            int index1 = MakeIndex(string1);

            //Debug.Assert(Index0 + 1 == Index1 && "Unexpected non-consecutive indices!");
            //(void) Index1;
            return index0;
        }

        //using ArgumentList::MakeArgString;
        public override string MakeArgStringRef(string str)
        {
            return GetArgString(MakeIndex(str));
        }

        //public override string ToString()
        //{
        //    StringBuilder o = new StringBuilder();
        //    Print(o);
        //    return o.ToString();
        //}
    }
}