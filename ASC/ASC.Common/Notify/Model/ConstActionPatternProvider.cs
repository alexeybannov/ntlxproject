#region usings

using System;
using System.Collections.Generic;
using ASC.Notify.Patterns;

#endregion

namespace ASC.Notify.Model
{
    public sealed class ConstActionPatternProvider
        : IActionPatternProvider
    {
        private readonly Dictionary<INotifyAction, Dictionary<string, IPattern>> _Accords =
            new Dictionary<INotifyAction, Dictionary<string, IPattern>>();

        private readonly Dictionary<INotifyAction, IPattern> _DefaultPatterns =
            new Dictionary<INotifyAction, IPattern>();

        public ConstActionPatternProvider(
            KeyValuePair<INotifyAction, KeyValuePair<string, IPattern>>[] accordings
            )
        {
            if (accordings == null) throw new ArgumentNullException("accordings");
            try
            {
                foreach (var kvp in accordings)
                {
                    if (kvp.Value.Key != null)
                    {
                        Dictionary<string, IPattern> accord = null;
                        if (_Accords.ContainsKey(kvp.Key))
                            accord = _Accords[kvp.Key];
                        else
                        {
                            accord = new Dictionary<string, IPattern>();
                            _Accords.Add(kvp.Key, accord);
                        }
                        accord.Add(kvp.Value.Key, kvp.Value.Value);
                    }
                    else
                    {
                        if (!_DefaultPatterns.ContainsKey(kvp.Key))
                            _DefaultPatterns.Add(kvp.Key, kvp.Value.Value);
                    }
                }
            }
            catch (Exception exc)
            {
                throw new ArgumentException("accordings", exc);
            }
        }

        #region IActionPatternProvider

        public IPattern GetPattern(INotifyAction action, string senderName)
        {
            if (action == null) throw new ArgumentNullException("action");
            if (senderName == null) throw new ArgumentNullException("senderName");
            IPattern pattern = null;
            Dictionary<string, IPattern> accord = null;
            if (_Accords.TryGetValue(action, out accord))
                accord.TryGetValue(senderName, out pattern);
            return pattern ?? GetPattern(action);
        }

        public IPattern GetPattern(INotifyAction action)
        {
            if (action == null) throw new ArgumentNullException("action");
            IPattern pattern = null;
            _DefaultPatterns.TryGetValue(action, out pattern);
            return pattern;
        }

        public GetPatternCallback GetPatternMethod { get; set; }

        #endregion
    }
}