#region usings

using System;

#endregion

namespace ASC.Common.Security.Authorizing
{
    [Serializable]
    public class Ace
    {
        public Ace(Guid actionId, AceType reaction)
        {
            if (actionId == Guid.Empty) throw new ArgumentException("actionId");
            ActionId = actionId;
            Reaction = reaction;
        }

        public Guid ActionId { get; set; }

        public AceType Reaction { get; set; }
    }
}