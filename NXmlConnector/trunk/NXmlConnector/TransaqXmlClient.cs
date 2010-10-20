using System;
using System.Linq;
using System.Collections.Generic;
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

        public IList<CandleKind> CandleKinds
        {
            get { return candleKinds.AsReadOnly(); }
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


        private TransaqXmlClient()
        {
            NXmlConnector.NewData += NewData;

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

        public void GetHistoryData(string securityId, int period, int count, bool reset)
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

        public void Subscribe(IEnumerable<string> securityIds)
        {
            SendCommand(CommandSetSubscription.Subscribe(securityIds, securityIds, securityIds));
        }

        public void Unsubscribe(IEnumerable<string> securityIds)
        {
            SendCommand(CommandSetSubscription.Unsubscribe(securityIds, securityIds, securityIds));
        }

        public void SubscribeOnTrades(IEnumerable<string> securityIds)
        {
            SendCommand(CommandSetSubscription.Subscribe(securityIds, null, null));
        }

        public void SubscribeOnQuotations(IEnumerable<string> securityIds)
        {
            SendCommand(CommandSetSubscription.Subscribe(null, securityIds, null));
        }

        public void SubscribeOnQuotes(IEnumerable<string> securityIds)
        {
            SendCommand(CommandSetSubscription.Subscribe(null, null, securityIds));
        }

        public void UnsubscribeFromTrades(IEnumerable<string> securityIds)
        {
            SendCommand(CommandSetSubscription.Unsubscribe(securityIds, null, null));
        }

        public void UnsubscribeFromQuotations(IEnumerable<string> securityIds)
        {
            SendCommand(CommandSetSubscription.Unsubscribe(null, securityIds, null));
        }

        public void UnsubscribeFromQuotes(IEnumerable<string> securityIds)
        {
            SendCommand(CommandSetSubscription.Unsubscribe(null, null, securityIds));
        }

        public int NewOrder(NewOrder newOrder)
        {
            if (newOrder == null) throw new ArgumentNullException("newOrder");

            return SendCommand(new CommandNewOrder(newOrder, ClientInfo != null ? ClientInfo.Id : null)).TransactionId;
        }

        public void NewOrder(int transactionId)
        {
            SendCommand(new CommandCancelOrder(transactionId));
        }

        public event EventHandler Connected;

        public event EventHandler<ErrorEventArgs> ConnectionError;

        public event EventHandler Disconnected;

        public event EventHandler<SecuritiesEventArgs> RecieveSecurities;

        public event EventHandler<MarketsEventArgs> RecieveMarkets;

        public event EventHandler<CandlesEventArgs> RecieveCandles;

        public event EventHandler<ClientEventArgs> RecieveClientInfo;

        public event EventHandler<OrderEventArgs> RecieveOrder;

        public event EventHandler<ErrorEventArgs> InternalError;


        private void OnConnect()
        {
            ServerTimeDifference = TimeSpan.FromSeconds(SendCommand(new CommandGetServerTimeDifference()).Difference);

            var ev = Connected;
            if (ev != null) ev(this, EventArgs.Empty);
        }

        private void OnConnectionError(Exception error)
        {
            var ev = ConnectionError;
            if (ev != null) ev(this, new ErrorEventArgs(error));
        }

        private void OnDisconnect()
        {
            var ev = Disconnected;
            if (ev != null) ev(this, EventArgs.Empty);
        }

        private void OnServerStatus(ServerStatus status)
        {
            IsConnecting = false;
            if (status.Status == "true")
            {
                IsConnected = true;
                OnConnect();
            }
            else if (status.Status == "false")
            {
                IsConnected = false;
                OnDisconnect();
            }
            else if (status.Status == "error")
            {
                IsConnected = false;
                OnConnectionError(new NXmlConnectorException(status.ErrorText));
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
            if (ev != null) ev(this, new CandlesEventArgs(candles.SecurityId, candles.Period, candles.Status, candles.CandlesArray));
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
            var ev = RecieveClientInfo;
            if (ev != null) ev(this, new ClientEventArgs(client));
        }

        private void OnOrders(Orders orders)
        {
            var ev = RecieveOrder;
            if (ev != null) ev(this, new OrderEventArgs(orders.Order));
        }

        private Result SendCommand(Command command)
        {
            var commandStr = command.ToString();
            var resultStr = NXmlConnector.SendCommand(commandStr);
            var result = (Result)NXmlDeserializer.Deserialize(typeof(Result), resultStr);
            if (!result.Success) throw new NXmlConnectorException(result.ErrorText);
            return result;
        }

        private void NewData(string data)
        {
            try
            {
                parser.Parse(data);
            }
            catch (Exception ex)
            {
                OnError(ex);
            }
        }
    }
}
