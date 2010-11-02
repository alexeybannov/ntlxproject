using System;
using System.Collections.Generic;
using NXmlConnector.Model;

namespace NXmlConnector
{
    public class ErrorEventArgs : EventArgs
    {
        public Exception Error
        {
            get;
            private set;
        }

        public ErrorEventArgs(Exception error)
        {
            Error = error;
        }
    }

    public class LogEventArgs : EventArgs
    {
        public string Data
        {
            get;
            private set;
        }

        public LogEventArgs(string data)
        {
            Data = data;
        }
    }

    public class SecuritiesEventArgs : EventArgs
    {
        public List<Security> Securities
        {
            get;
            private set;
        }

        public SecuritiesEventArgs(Security[] securities)
        {
            Securities = new List<Security>(securities ?? new Security[0]);
        }
    }

    public class SecurityPermitChangedEventArgs : EventArgs
    {
        public Security Security
        {
            get;
            private set;
        }

        public bool Permit
        {
            get;
            private set;
        }

        public SecurityPermitChangedEventArgs(Security security, bool permit)
        {
            Security = security;
            Permit = permit;
        }
    }

    public class MarketsEventArgs : EventArgs
    {
        public List<Market> Markets
        {
            get;
            private set;
        }

        public MarketsEventArgs(Market[] markets)
        {
            Markets = new List<Market>(markets ?? new Market[0]);
        }
    }

    public class HistoryEventArgs : EventArgs
    {
        public int SecurityId
        {
            get;
            private set;
        }

        public int Period
        {
            get;
            private set;
        }

        public CandlesStatus Status
        {
            get;
            private set;
        }

        public List<Candle> Candles
        {
            get;
            private set;
        }

        public HistoryEventArgs(Candles candles)
        {
            SecurityId = candles.SecurityId;
            Period = candles.Period;
            Status = (CandlesStatus)candles.Status;
            Candles = new List<Candle>(candles.CandlesArray ?? new Candle[0]);
        }
    }

    public class ClientEventArgs : EventArgs
    {
        public Client ClientInfo
        {
            get;
            private set;
        }

        public ClientEventArgs(Client client)
        {
            ClientInfo = client;
        }
    }

    public class OrdersEventArgs : EventArgs
    {
        public List<Order> Orders
        {
            get;
            private set;
        }

        public OrdersEventArgs(Order[] orders)
        {
            Orders = new List<Order>(orders ?? new Order[0]);
        }
    }

    public class TicksEventArgs : EventArgs
    {
        public List<Tick> Ticks
        {
            get;
            private set;
        }

        public TicksEventArgs(Tick[] ticks)
        {
            Ticks = new List<Tick>(ticks ?? new Tick[0]);
        }
    }

    public class AllTradesEventArgs : EventArgs
    {
        public List<AllTrade> AllTrades
        {
            get;
            private set;
        }

        public AllTradesEventArgs(IEnumerable<AllTrade> allTrades)
        {
            AllTrades = new List<AllTrade>(allTrades);
        }
    }

    public class QuotationsEventArgs : EventArgs
    {
        public List<Quotation> Quotations
        {
            get;
            private set;
        }

        public QuotationsEventArgs(IEnumerable<Quotation> quotations)
        {
            Quotations = new List<Quotation>(quotations);
        }
    }

    public class QuotesEventArgs : EventArgs
    {
        public List<Quote> Quotes
        {
            get;
            private set;
        }

        public QuotesEventArgs(IEnumerable<Quote> quotes)
        {
            Quotes = new List<Quote>(quotes);
        }
    }

    public class TradesEventArgs : EventArgs
    {
        public List<Trade> Trades
        {
            get;
            private set;
        }

        public TradesEventArgs(Trade[] trades)
        {
            Trades = new List<Trade>(trades ?? new Trade[0]);
        }
    }

    public class PositionsEventArgs : EventArgs
    {
        public List<MoneyPosition> MoneyPositions
        {
            get;
            private set;
        }

        public List<SecurityPosition> SecurityPositions
        {
            get;
            private set;
        }

        public List<FortsPosition> FortsPositions
        {
            get;
            private set;
        }

        public List<FortsMoney> FortsMoney
        {
            get;
            private set;
        }

        public List<FortsCollateral> FortsCollaterals
        {
            get;
            private set;
        }

        public List<SpotLimit> SpotLimits
        {
            get;
            private set;
        }

        public PositionsEventArgs(Positions positions)
        {
            MoneyPositions = new List<MoneyPosition>(positions.MoneyPositions ?? new MoneyPosition[0]);
            SecurityPositions = new List<SecurityPosition>(positions.SecurityPositions ?? new SecurityPosition[0]);
            FortsPositions = new List<FortsPosition>(positions.FortsPositions ?? new FortsPosition[0]);
            FortsMoney = new List<FortsMoney>(positions.FortsMoney ?? new FortsMoney[0]);
            FortsCollaterals = new List<FortsCollateral>(positions.FortsCollaterals ?? new FortsCollateral[0]);
            SpotLimits = new List<SpotLimit>(positions.SpotLimits ?? new SpotLimit[0]);
        }
    }
}
