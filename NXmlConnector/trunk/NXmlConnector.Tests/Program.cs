using System;
using System.Threading;

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
                client.RecieveCandles += (s, e) => e.Candles.ForEach(c => Console.WriteLine("Candle: " + c));
                client.RecieveSecurities += (s, e) => e.Securities.ForEach(sec => Console.WriteLine("Security: " + sec));
                client.RecieveClient += (s, e) => Console.WriteLine("ClientInfo: " + e.ClientInfo);
                client.RecieveOrder += (s, e) => Console.WriteLine("Order: " + e.Order.TransactionId);
                client.RecieveTick += (s, e) => Console.WriteLine("Tick: " + e.Tick.Price);

                client.RecieveAllTrades += (s, e) => Console.WriteLine("RecieveAllTrades: " + e.AllTrades.Count);
                client.RecieveQuotations += (s, e) => Console.WriteLine("RecieveQuotations: " + e.Quotations.Count);
                client.RecieveQuotes += (s, e) => Console.WriteLine("RecieveQuotes: " + e.Quotes.Count);

                client.Connect("195.128.78.60", 3939, "TCONN0011", "E6CNxe");
                wait.WaitOne();
                wait.Reset();

                //var transId = client.NewOrder(new NewOrder(4, OrderType.Buy, 100));
                //client.CancelOrder(transId);
                //client.MakeOrDownOrder(transId);

                client.GetHistoryData(4, client.CandleKinds[0].Id, 20, false);
                client.GetHistoryData(4, client.CandleKinds[0].Id, 10, false);

                client.SubscribeTicks(4, 0);
                
                client.GetFortsPosition();
                client.GetClientLimits();
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
