#region usings

using System;
using System.Collections.Generic;

#endregion

namespace ASC.Notify.Model
{
    public sealed class ConstActionProvider
        : IActionProvider
    {
        private readonly Dictionary<string, INotifyAction> _Actions = new Dictionary<string, INotifyAction>();

        public ConstActionProvider(params INotifyAction[] actions)
        {
            foreach (INotifyAction action in actions)
            {
                if (action == null) throw new ArgumentException("objects");
                try
                {
                    _Actions.Add(action.ID, action);
                }
                catch (Exception exc)
                {
                    throw new ArgumentException("objects", exc);
                }
            }
        }

        #region IActionProvider 

        public INotifyAction[] GetActions()
        {
            return new List<INotifyAction>(_Actions.Values).ToArray();
        }

        public INotifyAction GetAction(string id)
        {
            INotifyAction action = null;
            _Actions.TryGetValue(id, out action);
            return action;
        }

        #endregion
    }
}