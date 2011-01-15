#region usings

using System;

#endregion

namespace ASC.Notify.Messages
{
    [Flags]
    public enum SendResult
        : short
    {
        Ok = 1,

        Incomplete = 2,

        IncorrectRecipient = 4,

        Impossible = 8,

        Inprogress = 16,

        Prevented = 32
    }
}