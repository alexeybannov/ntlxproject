using System;
using System.Linq;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using NXmlConnector.Model;
using NXmlConnector.Model.Commands;
using NXmlConnector.Properties;

namespace NXmlConnector
{
    public class TransaqXmlClient
    {
        private static object syncRoot = new object();

        private static TransaqXmlClient client;


        private NXmlParser parser;

        private bool autoPos;

        private LogLevel logLevel;

        private string logsDirectory;

        private string notesFileName;

        private List<CandleKind> candleKinds;

        private Dictionary<int, Security> securities;

        private Dictionary<string, Client> clients;

        private Dictionary<int, Market> markets;

        private List<int> subscriptions;


        public bool IsConnected
        {
            get;
            private set;
        }

        public bool IsConnecting
        {
            get;
            private set;
        }

        public bool AutoPos
        {
            get { return autoPos; }
            set
            {
                if (IsConnected || IsConnecting) throw new InvalidOperationException(Resources.InvalidOperationInConnected);
                autoPos = value;
            }
        }

        public LogLevel LogLevel
        {
            get { return logLevel; }
            set
            {
                if (IsConnected || IsConnecting) throw new InvalidOperationException(Resources.InvalidOperationInConnected);
                logLevel = value;
            }
        }

        public string LogsDirectory
        {
            get { return logsDirectory; }
            set
            {
                if (IsConnected || IsConnecting) throw new InvalidOperationException(Resources.InvalidOperationInConnected);
                if (!Directory.Exists(value)) throw new DirectoryNotFoundException(string.Format(Resources.DirectoryNotFound, LogsDirectory));
                logsDirectory = value;
            }
        }

        public string NotesFileName
        {
            get { return notesFileName; }
            set
            {
                if (IsConnected || IsConnecting) throw new InvalidOperationException(Resources.InvalidOperationInConnected);
                if (!File.Exists(value)) throw new FileNotFoundException(Resources.FileNotFound, value);
                notesFileName = value;
            }
        }

        public TimeSpan ServerTimeDifference
        {
            get;
            private set;
        }

        public bool Overnight
        {
            get;
            private set;
        }

        public List<CandleKind> CandleKinds
        {
            get { return new List<CandleKind>(candleKinds); }
        }

        public List<Market> Markets
        {
            get { return new List<Market>(markets.Values); }
        }

        public List<Security> Securities
        {
            get { return new List<Security>(securities.Values); }
        }

        public List<Client> Clients
        {
            get { return new List<Client>(clients.Values); }
        }

        public List<Security> Subscriptions
        {
            get { return subscriptions.Select(s => GetSecurity(s)).Where(s => s != null).ToList(); }
        }


        private TransaqXmlClient()
        {
            NXmlConnector.NewData += OnNewData;

            IsConnected = false;
            IsConnecting = false;

            AutoPos = true;
            LogLevel = LogLevel.None;
            LogsDirectory = AppDomain.CurrentDomain.BaseDirectory;
            candleKinds = new List<CandleKind>();
            securities = new Dictionary<int, Security>();
            clients = new Dictionary<string, Client>();
            markets = new Dictionary<int, Market>();
            subscriptions = new List<int>();

            parser = new NXmlParser();
            parser.RegisterCallback<ServerStatus>(OnServerStatus);
            parser.RegisterCallback<Securities>(OnSecurities);
            parser.RegisterCallback<Markets>(OnMarkets);
            parser.RegisterCallback<CandleKinds>(OnCandleKinds);
            parser.RegisterCallback<Error>(OnInternalError);
            parser.RegisterCallback<Candles>(OnCandles);
            parser.RegisterCallback<Client>(OnClient);
            parser.RegisterCallback<Orders>(OnOrders);
            parser.RegisterCallback<Ticks>(OnTicks);
            parser.RegisterCallback<AllTrades>(OnAllTrades);
            parser.RegisterCallback<Quotations>(OnQuotations);
            parser.RegisterCallback<Quotes>(OnQuotes);
            parser.RegisterCallback<Overnight>(OnOvernight);
            parser.RegisterCallback<Trades>(OnTrades);
            parser.RegisterCallback<Positions>(OnPositions);
            parser.RegisterCallback<MarketOrd>(OnMarketOrd);
        }


        public static TransaqXmlClient GetTransaqXmlClient()
        {
            if (client == null)
            {
                lock (syncRoot)
                {
                    if (client == null) client = new TransaqXmlClient();
                }
            }
            return client;
        }


        public void Connect(string host, int port, string login, string password)
        {
            Connect(host, port, login, password, null);
        }

        public void Connect(string host, int port, string login, string password, Proxy proxy)
        {
            if (IsConnected || IsConnecting) return;

            IsConnecting = true;
            var command = new CommandConnect()
            {
                Host = host,
                Port = port,
                Login = login,
                Password = password,
                AutoPos = this.AutoPos,
                LogsDirectory = this.LogsDirectory.EndsWith("\\") ? this.LogsDirectory : this.LogsDirectory + "\\",
                LogLevel = this.LogLevel,
                NotesFileName = this.NotesFileName,
                Proxy = proxy,
            };
            try
            {
                SendCommand(command);
            }
            catch
            {
                IsConnecting = false;
                throw;
            }
        }

        public void Disconnect()
        {
            if (!IsConnected || IsConnecting) return;

            SendCommand(new CommandDisconnect());
        }

        public void ChangePassword(string oldPassword, string newPassword)
        {
            SendCommand(new CommandChangePassword(oldPassword, newPassword));
        }

        public Market GetMarket(int id)
        {
            return markets.ContainsKey(id) ? markets[id] : null;
        }

        public void GetMarkets()
        {
            SendCommand(new CommandGetMarkets());
        }

        public Security GetSecurity(int id)
        {
            return securities.ContainsKey(id) ? securities[id] : null;
        }

        public void GetSecurities()
        {
            SendCommand(new CommandGetSecurities());
        }

        public void GetHistoryData(int securityId, int period, int count, bool reset)
        {
            var command = new CommandGetHistoryData()
            {
                SecurityId = securityId,
                Period = period,
                Count = count,
                Reset = reset,
            };
            SendCommand(command);
        }

        public void GetFortsPosition()
        {
            GetFortsPosition(null);
        }

        public void GetFortsPosition(string client)
        {
            SendCommand(new CommandGetFortsPosition(client));
        }
        /*
        public void GetClientLimits()
        {
            GetClientLimits(GetDefaultClientId());
        }

        public void GetClientLimits(string client)
        {
            SendCommand(new CommandGetClientLimits(client));
        }

        public void GetLeverageControl(params int[] securityIds)
        {
            SendCommand(new CommandGetLeverageControl(GetDefaultClientId(), securityIds));
        }

        public void GetLeverageControl(string client, params int[] securityIds)
        {
            SendCommand(new CommandGetLeverageControl(client, securityIds));
        }
        public void Subscribe(IEnumerable<int> alltradesSecurityIds, IEnumerable<int> quotationsSecurityIds, IEnumerable<int> quotesSecurityIds)
        {
            SendCommand(CommandSetSubscription.Subscribe(alltradesSecurityIds, quotationsSecurityIds, quotesSecurityIds));
        }
        public void Unsubscribe(IEnumerable<int> alltradesSecurityIds, IEnumerable<int> quotationsSecurityIds, IEnumerable<int> quotesSecurityIds)
        {
            SendCommand(CommandSetSubscription.Unsubscribe(alltradesSecurityIds, quotationsSecurityIds, quotesSecurityIds));
        }
*/
        public void MakeOrDownOrder(int transactionId)
        {
            SendCommand(new CommandMakeOrDown(transactionId));
        }

        public void Subscribe(params int[] securityIds)
        {
            foreach (var id in securityIds)
            {
                if (!subscriptions.Contains(id)) subscriptions.Add(id);
            }
            SendCommand(CommandSetSubscription.Subscribe(securityIds, securityIds, securityIds));
        }

        public void Unsubscribe(params int[] securityIds)
        {
            foreach (var id in securityIds)
            {
                if (subscriptions.Contains(id)) subscriptions.Remove(id);
            }
            SendCommand(CommandSetSubscription.Unsubscribe(securityIds, securityIds, securityIds));
        }

        public void SubscribeTicks(int securityId, int tradeNo)
        {
            SubscribeTicks(securityId, tradeNo, false);
        }

        public void SubscribeTicks(int securityId, int tradeNo, bool filter)
        {
            SubscribeTicks(new[] { securityId }, new[] { tradeNo }, filter);
        }

        public void SubscribeTicks(int[] securityIds, int[] tradeNos, bool filter)
        {
            if (securityIds.Length != tradeNos.Length) throw new ArgumentException("Не соответствуют размеры массивов securityIds и tradeNos.");
            SendCommand(new CommandSubscribeTicks(securityIds, tradeNos, filter));
        }

        public void UnsubscribeTicks()
        {
            SendCommand(new CommandSubscribeTicks());
        }

        public int NewOrder(NewOrder order)
        {
            if (order == null) throw new ArgumentNullException("order");

            if (string.IsNullOrEmpty(order.ClientId)) order.ClientId = GetDefaultClientId();
            return SendCommand(new CommandNewOrder(order)).TransactionId;
        }

        public void CancelOrder(int transactionId)
        {
            SendCommand(new CommandCancelOrder(transactionId));
        }

        public event EventHandler Connected;

        public event EventHandler<ErrorEventArgs> ConnectionError;

        public event EventHandler Disconnected;

        public event EventHandler<SecuritiesEventArgs> RecieveSecurities;

        public event EventHandler<MarketsEventArgs> RecieveMarkets;

        public event EventHandler<CandlesEventArgs> RecieveCandles;

        public event EventHandler<ClientEventArgs> RecieveClient;

        public event EventHandler<OrdersEventArgs> RecieveOrders;

        public event EventHandler<TicksEventArgs> RecieveTicks;

        public event EventHandler<AllTradesEventArgs> RecieveAllTrades;

        public event EventHandler<QuotationsEventArgs> RecieveQuotations;

        public event EventHandler<QuotesEventArgs> RecieveQuotes;

        public event EventHandler<TradesEventArgs> RecieveTrades;

        public event EventHandler<PositionsEventArgs> RecievePositions;

        public event EventHandler<SecurityEventArgs> ChangeSecurityPermit;

        public event EventHandler<ErrorEventArgs> InternalError;

        public event EventHandler<LogEventArgs> Logging;


        private string GetDefaultClientId()
        {
            return Clients.Count == 1 ? Clients[0].Id : null;
        }

        private void OnConnect()
        {
            if (IsConnected) return;

            IsConnected = true;
            try
            {
                ServerTimeDifference = TimeSpan.FromSeconds(SendCommand(new CommandGetServerTimeDifference()).Difference);
            }
            catch (Exception ex)
            {
                OnError(ex);
            }

            var ev = Connected;
            if (ev != null) ev(this, EventArgs.Empty);
        }

        private void OnConnectionError(string errorText)
        {
            IsConnected = false;
            var ev = ConnectionError;
            if (ev != null) ev(this, new ErrorEventArgs(new NXmlConnectorException(errorText)));
        }

        private void OnDisconnect()
        {
            if (!IsConnected) return;

            IsConnected = false;
            var ev = Disconnected;
            if (ev != null) ev(this, EventArgs.Empty);
        }

        private void OnServerStatus(ServerStatus status)
        {
            IsConnecting = false;
            if (status.Status == "true")
            {
                OnConnect();
            }
            else if (status.Status == "false")
            {
                OnDisconnect();
            }
            else if (status.Status == "error")
            {
                OnConnectionError(status.ErrorText);
            }
            else
            {
                throw new ArgumentOutOfRangeException("ServerStatus");
            }
        }

        private void OnCandleKinds(CandleKinds kinds)
        {
            this.candleKinds.Clear();
            this.candleKinds.AddRange(kinds.Kinds ?? new CandleKind[0]);
        }

        private void OnMarkets(Markets markets)
        {
            if (markets.MarketsArray != null)
            {
                foreach (var market in markets.MarketsArray)
                {
                    this.markets[market.Id] = market;
                }
            }

            var ev = RecieveMarkets;
            if (ev != null) ev(this, new MarketsEventArgs(markets.MarketsArray));
        }

        private void OnCandles(Candles candles)
        {
            var ev = RecieveCandles;
            if (ev != null) ev(this, new CandlesEventArgs(candles));
        }

        private void OnSecurities(Securities securities)
        {
            if (securities.SecuritiesArray != null)
            {
                foreach (var security in securities.SecuritiesArray)
                {
                    this.securities[security.Id] = security;
                }
            }
            var ev = RecieveSecurities;
            if (ev != null) ev(this, new SecuritiesEventArgs(securities.SecuritiesArray));
        }

        private void OnInternalError(Error error)
        {
            OnError(new NXmlConnectorException(error.ErrorText));
        }

        private void OnError(Exception error)
        {
            var ev = InternalError;
            if (ev != null) ev(this, new ErrorEventArgs(error));
        }

        private void OnClient(Client client)
        {
            if (client.Remove) clients.Remove(client.Id);
            else clients[client.Id] = client;

            var ev = RecieveClient;
            if (ev != null) ev(this, new ClientEventArgs(client));
        }

        private void OnOrders(Orders orders)
        {
            var ev = RecieveOrders;
            if (ev != null) ev(this, new OrdersEventArgs(orders.OrderArray));
        }

        private void OnTicks(Ticks ticks)
        {
            var ev = RecieveTicks;
            if (ev != null) ev(this, new TicksEventArgs(ticks.TickArray));
        }

        private void OnAllTrades(AllTrades allTrades)
        {
            var ev = RecieveAllTrades;
            if (ev != null) ev(this, new AllTradesEventArgs(allTrades.TradesArray));
        }

        private void OnQuotations(Quotations quotations)
        {
            var ev = RecieveQuotations;
            if (ev != null) ev(this, new QuotationsEventArgs(quotations.QuotationsArray));
        }

        private void OnQuotes(Quotes quotes)
        {
            var ev = RecieveQuotes;
            if (ev != null) ev(this, new QuotesEventArgs(quotes.QuotesArray));
        }

        private void OnOvernight(Overnight overnight)
        {
            Overnight = overnight.Status;
        }

        private void OnTrades(Trades trades)
        {
            var ev = RecieveTrades;
            if (ev != null) ev(this, new TradesEventArgs(trades.TradesArray));
        }

        private void OnPositions(Positions positions)
        {
            var ev = RecievePositions;
            if (ev != null) ev(this, new PositionsEventArgs(positions));
        }

        private void OnMarketOrd(MarketOrd marketOrd)
        {
            Security security = null;
            if (securities.TryGetValue(marketOrd.SecurityId, out security))
            {
                security.Permit = marketOrd.permit == YesNo.yes;
                var ev = ChangeSecurityPermit;
                if (ev != null) ev(this, new SecurityEventArgs(security));
            }
        }

        private void OnLogging(string data)
        {
            var ev = Logging;
            if (ev != null) ev(this, new LogEventArgs(data));
        }


        private Result SendCommand(Command command)
        {
            var commandStr = command.ToString();
            OnLogging(commandStr);

            var resultStr = NXmlConnector.SendCommand(commandStr);

            OnLogging(resultStr);
            var result = (Result)NXmlDeserializer.Deserialize(typeof(Result), resultStr);

            if (!result.Success)
            {
                throw new NXmlConnectorException(result.ErrorText);
            }
            return result;
        }

        private void OnNewData(string data)
        {
            try
            {
                OnLogging(data);
                parser.Parse(data);
            }
            catch (Exception ex)
            {
                OnError(ex);
            }
        }
    }
}
