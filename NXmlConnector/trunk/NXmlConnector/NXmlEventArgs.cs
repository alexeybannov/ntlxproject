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

    public class CandlesEventArgs : EventArgs
    {
        public string SecurityId
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

        public CandlesEventArgs(string securityId, int period, CandlesStatus status, Candle[] candles)
        {
            SecurityId = securityId;
            Period = period;
            Status = status;
            Candles = new List<Candle>(candles ?? new Candle[0]);
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

    public class OrderEventArgs : EventArgs
    {
        public Order Order
        {
            get;
            private set;
        }

        public OrderEventArgs(Order order)
        {
            Order = order;
        }
    }
}
