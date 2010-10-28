using System;
using System.Threading;
using NXmlConnector.Model;

namespace NXmlConnector.Tests
{
    class Program
    {
        private static ManualResetEvent wait = new ManualResetEvent(false);

        static void Main(string[] args)
        {
            var client = TransaqXmlClient.GetTransaqXmlClient();
            try
            {
                client.LogLevel = LogLevel.Trace;
                client.InternalError += (s, e) => Console.WriteLine("Error: " + e.Error.Message);
                client.Connected += (s, e) =>
                {
                    client.CandleKinds.ForEach(k => Console.WriteLine("Candle Kind: " + k.Name));
                    Console.WriteLine("Server time difference: " + client.ServerTimeDifference);
                    Console.WriteLine("Connected");
                    wait.Set();
                };
                client.ConnectionError += (s, e) =>
                {
                    Console.WriteLine("ConnectionError: {0}", e.Error.Message);
                    wait.Set();
                };
                client.Disconnected += (s, e) =>
                {
                    Console.WriteLine("Disconnected");
                    wait.Set();
                };
                client.RecieveMarkets += (s, e) => e.Markets.ForEach(m => Console.WriteLine("Market: " + m));
                client.RecieveCandles += (s, e) => Console.WriteLine("Candle: " + e.Candles.Count + " rows");
                client.RecieveSecurities += (s, e) => Console.WriteLine("Securities: " + e.Securities.Count + " rows");
                client.RecieveClient += (s, e) => Console.WriteLine("ClientInfo: " + e.ClientInfo);
                client.RecieveOrders += (s, e) => Console.WriteLine("Orders: " + e.Orders.Count + " rows");
                client.RecieveTicks += (s, e) => Console.WriteLine("Tick: " + e.Ticks.Count + " rows");

                client.RecieveAllTrades += (s, e) => Console.WriteLine("RecieveAllTrades: " + e.AllTrades.Count + " rows");
                client.RecieveQuotations += (s, e) => Console.WriteLine("RecieveQuotations: " + e.Quotations.Count + " rows");
                client.RecieveQuotes += (s, e) => Console.WriteLine("RecieveQuotes: " + e.Quotes.Count + " rows");

                client.RecieveTrades += (s, e) => Console.WriteLine("RecieveTrades: " + e.Trades.Count + " rows");
                client.RecievePositions += (s, e) => Console.WriteLine("RecievePositions: ok");

                client.Connect("195.128.78.60", 3939, "TCONN0011", "E6CNxe");
                wait.WaitOne();
                wait.Reset();

                //var transId = client.NewOrder(new NewOrder(4, OrderType.B, 100));
                //client.CancelOrder(transId);
                //client.MakeOrDownOrder(transId);

                if (0 < client.CandleKinds.Count)
                {
                    client.GetHistoryData(4, client.CandleKinds[0].Id, 20, false);
                    client.GetHistoryData(4, client.CandleKinds[0].Id, 10, false);
                }
                //client.SubscribeTicks(4, 0);

                client.Subscribe(Subscription.All, 4);

                //client.GetFortsPosition();
                //client.GetClientLimits();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            Console.ReadKey();
            if (client.IsConnected)
            {
                client.Disconnect();
                wait.WaitOne();
            }
        }
    }
}
