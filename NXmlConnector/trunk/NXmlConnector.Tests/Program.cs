using System;
using System.Linq;
using NXmlConnector.Model;
using NXmlConnector.Model.Commands;
using System.Text;

namespace NXmlConnector.Tests
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                //var xml = "<candlekinds><kind><period>4</period><id>7</id></kind></candlekinds>";
                //xml = Encoding.UTF8.GetString(Encoding.Convert(Encoding.Unicode, Encoding.UTF8, Encoding.Unicode.GetBytes(xml)));
                //var obj = NXmlDeserializer.Deserialize(typeof(CandleKinds), xml);

                var client = TransaqXmlClient.GetTransaqXmlClient();
                client.InternalError += (s, e) => Console.WriteLine("Error: " + e.Error.Message);
                client.Connect("195.128.78.60", 3939, "TCONN0011", "E6CNxe");
                client.Connected += (s, e) => Console.WriteLine("Connected");
                client.ConnectionError += (s, e) => Console.WriteLine("ConnectionError: {0}", e.Error.Message);
                client.Disconnected += (s, e) => Console.WriteLine("Disconnected");
                
                client.RecieveMarkets += (s, e) => e.Markets.ForEach(m => Console.WriteLine("Market: " + m));
                client.RecieveCandles += (s, e) => e.Candles.ForEach(c => Console.WriteLine("Candle: " + c));
                client.RecieveSecurities += (s, e) => e.Securities.ForEach(sec => Console.WriteLine("Security: " + sec));
                client.RecieveClientInfo += (s, e) => Console.WriteLine("ClientInfo: " + e.ClientInfo);

                client.NewOrder(new NewOrder());

                Console.ReadKey();
                client.Disconnect();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                Console.ReadKey();
            }
        }
    }
}
