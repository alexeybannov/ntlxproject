using System;
using System.Collections.Generic;
using System.IO;
using NXmlConnector.Model;
using NXmlConnector.Model.Commands;
using NXmlConnector.Properties;
using System.Diagnostics;

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

        public List<CandleKind> CandleKinds
        {
            get { return new List<CandleKind>(candleKinds); }
        }

        public Client ClientInfo
        {
            get;
            private set;
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


        private TransaqXmlClient()
        {
            NXmlConnector.NewData += OnNewData;

            IsConnected = false;
            IsConnecting = false;

            AutoPos = true;
            LogLevel = LogLevel.None;
            LogsDirectory = AppDomain.CurrentDomain.BaseDirectory;
            candleKinds = new List<CandleKind>();

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

        public void GetMarkets()
        {
            SendCommand(new CommandGetMarkets());
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

        public void GetClientLimits()
        {
            GetClientLimits(null);
        }

        public void GetClientLimits(string client)
        {
            SendCommand(new CommandGetClientLimits(client ?? (ClientInfo != null ? ClientInfo.Id : null)));
        }

        public void MakeOrDownOrder(int transactionId)
        {
            SendCommand(new CommandMakeOrDown(transactionId));
        }

        public void Subscribe(params int[] securityIds)
        {
            SendCommand(CommandSetSubscription.Subscribe(securityIds, securityIds, securityIds));
        }

        public void Subscribe(IEnumerable<int> alltradesSecurityIds, IEnumerable<int> quotationsSecurityIds, IEnumerable<int> quotesSecurityIds)
        {
            SendCommand(CommandSetSubscription.Subscribe(alltradesSecurityIds, quotationsSecurityIds, quotesSecurityIds));
        }

        public void Unsubscribe(params int[] securityIds)
        {
            SendCommand(CommandSetSubscription.Unsubscribe(securityIds, securityIds, securityIds));
        }

        public void Unsubscribe(IEnumerable<int> alltradesSecurityIds, IEnumerable<int> quotationsSecurityIds, IEnumerable<int> quotesSecurityIds)
        {
            SendCommand(CommandSetSubscription.Unsubscribe(alltradesSecurityIds, quotationsSecurityIds, quotesSecurityIds));
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

        public int NewOrder(NewOrder newOrder)
        {
            if (newOrder == null) throw new ArgumentNullException("newOrder");

            if (string.IsNullOrEmpty(newOrder.ClientId)) newOrder.ClientId = ClientInfo.Id;
            return SendCommand(new CommandNewOrder(newOrder)).TransactionId;
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

        public event EventHandler<TickEventArgs> RecieveTick;

        public event EventHandler<AllTradesEventArgs> RecieveAllTrades;

        public event EventHandler<QuotationsEventArgs> RecieveQuotations;

        public event EventHandler<QuotesEventArgs> RecieveQuotes;

        public event EventHandler<TradesEventArgs> RecieveTrades;

        public event EventHandler<PositionsEventArgs> RecievePositions;

        public event EventHandler<ErrorEventArgs> InternalError;


        private void OnConnect()
        {
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
            this.candleKinds.AddRange(kinds.Kinds ?? new CandleKind[0]);
        }

        private void OnMarkets(Markets markets)
        {
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
            ClientInfo = client;
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
            var ev = RecieveTick;
            if (ev != null) ev(this, new TickEventArgs(ticks.Tick));
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


        private Result SendCommand(Command command)
        {
            var commandStr = command.ToString();
            Debug.WriteLine(string.Format("Send command: {0}", commandStr));

            var resultStr = NXmlConnector.SendCommand(commandStr);
            
            Debug.WriteLine(string.Format("Result: {0}", resultStr));            
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
                Debug.WriteLine(string.Format("Recieve data: {0}", data));
                parser.Parse(data);
            }
            catch (Exception ex)
            {
                OnError(ex);
            }
        }
    }
}
