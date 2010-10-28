using System;

namespace NXmlConnector.Model
{
    [Flags]
    public enum Subscription
    {
        None = 0,
        Trades = 1,
        Quotations = 2,
        Quotes = 4,

        All = Trades | Quotes | Quotations
    }
}
