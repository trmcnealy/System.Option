using System.Linq;

namespace System.Option
{
    public static class OptionTableExt
    {
        public static ushort GetOptionAlias(this OptionTable optionTable, ushort optId)
        {
            var alias = optionTable._optionInfos.FirstOrDefault(item => item.Id == optId);

            return alias?.AliasId ?? optId;
        }
    }
}