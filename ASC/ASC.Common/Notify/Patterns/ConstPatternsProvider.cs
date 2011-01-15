#region usings

using System;
using System.Collections.Generic;

#endregion

namespace ASC.Notify.Patterns
{
    public sealed class ConstPatternProvider
        : IPatternProvider
    {
        internal readonly Dictionary<string, IPatternFormatter> _Formatters =
            new Dictionary<string, IPatternFormatter>();

        internal readonly Dictionary<string, IPattern> _Patterns = new Dictionary<string, IPattern>();

        public ConstPatternProvider(params KeyValuePair<IPattern, IPatternFormatter>[] patterns)
        {
            foreach (var kvp in patterns)
            {
                try
                {
                    _Patterns.Add(kvp.Key.ID, kvp.Key);
                    _Formatters.Add(kvp.Key.ID, kvp.Value);
                }
                catch (Exception exc)
                {
                    throw new ArgumentException("patterns", exc);
                }
            }
        }

        #region IPatternProvider

        public IPattern GetPattern(string id)
        {
            IPattern pattern = null;
            _Patterns.TryGetValue(id, out pattern);
            return pattern;
        }

        public IPattern[] GetPatterns()
        {
            return new List<IPattern>(_Patterns.Values).ToArray();
        }

        public IPatternFormatter GetFormatter(IPattern pattern)
        {
            if (pattern == null) throw new ArgumentNullException("pattern");
            IPatternFormatter formatter = null;
            _Formatters.TryGetValue(pattern.ID, out formatter);
            return formatter;
        }

        #endregion
    }
}