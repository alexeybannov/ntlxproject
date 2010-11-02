using System;
using System.Collections.Generic;
using System.Drawing;
using System.Threading;
using System.Windows.Forms;
using System.Xml.Linq;
using NXmlConnector;
using NXmlConnector.Model;

namespace Transaq2NinjaTrader
{
    public partial class MainForm : Form
    {
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new MainForm());
        }


        private TransaqXmlClient transaq;

        private NinjaClient ninja;

        private SynchronizationContext ctx = new WindowsFormsSynchronizationContext();

        private bool needClose = false;


        public MainForm()
        {
            InitializeComponent();
            
            transaq = TransaqXmlClient.GetTransaqXmlClient();
            transaq.Connected += (s, e) => ctx.Post(transaq_Connected, e);
            transaq.ConnectionError += (s, e) => ctx.Post(transaq_ConnectionError, e);
            transaq.Disconnected += (s, e) => ctx.Post(transaq_Disconnected, e);
            transaq.InternalError += (s, e) => ctx.Post(transaq_Logging, e);
            transaq.Logging += (s, e) => ctx.Post(transaq_Logging, e);
            transaq.RecieveSecurities += (s, e) => ctx.Post(transaq_RecieveSecurities, e);
            transaq.RecieveHistory += transaq_RecieveCandles;
            transaq.RecieveQuotations += (s, e) => ctx.Post(transaq_RecieveQuotations, e);
            
            dataGridViewSecurities.CurrentCellDirtyStateChanged += dataGridViewSecurities_CurrentCellDirtyStateChanged;
            dateTimePickerHistoryStart.Value = DateTime.Now.Date.AddHours(9);
            dateTimePickerHistoryEnd.Value = DateTime.Now.Date.AddHours(18).AddMinutes(45);

            ninja = new NinjaClient();
        }

        void transaq_RecieveQuotations(object state)
        {
            var e = (QuotationsEventArgs)state;
            foreach (var q in e.Quotations)
            {
                var s = transaq.GetSecurity(q.SecurityId);
                ninja.SendLast(s.Code, q.Last, q.Quantity);
                ninja.SendBid(s.Code, q.Bid, q.NumBids);
                ninja.SendAsk(s.Code, q.Offer, q.NumOffers);
            }
        }

        void transaq_RecieveCandles(object sender, HistoryEventArgs e)
        {
            var start = dateTimePickerHistoryStart.Value;
            var end = dateTimePickerHistoryEnd.Value;
            var stop = false;
            var result = new List<Candle>();

            foreach (var c in e.Candles)
            {
                if (start <= c.Date && c.Date <= end)
                {
                    result.Add(c);
                }
                if (c.Date < start)
                {
                    stop = true;
                }
            }
            if (!stop && e.Status == CandlesStatus.EndOfRequest)
            {
                transaq.GetHistoryData(e.SecurityId, e.Period, 100, false);
            }

            ctx.Post(OnRecieveCandles, new object[] { result, e.SecurityId });
        }

        void OnRecieveCandles(object state)
        {
            var candles = (List<Candle>)((object[])state)[0];
            var security = transaq.GetSecurity((int)((object[])state)[1]);

            //send history to ninja
            candles.ForEach(c => ninja.SendHistory(security.Code, c.Close, c.High, c.Low, c.Volume, c.Date));

            ListViewItem item = null;
            foreach (ListViewItem i in listViewHistory.Items)
            {
                if (security.Name.Equals(i.SubItems[0].Text))
                {
                    item = i;
                    break;
                }
            }
            if (item == null)
            {
                item = new ListViewItem(new[] { security.Name, "0" });
                listViewHistory.Items.Add(item);
            }
            item.SubItems[1].Text = (candles.Count + int.Parse(item.SubItems[1].Text)).ToString();
        }

        void dataGridViewSecurities_CurrentCellDirtyStateChanged(object sender, EventArgs e)
        {
            try
            {
                if (dataGridViewSecurities.IsCurrentCellDirty)
                {
                    dataGridViewSecurities.CommitEdit(DataGridViewDataErrorContexts.Commit);
                }
            }
            catch { }
        }

        void transaq_RecieveSecurities(object state)
        {
            foreach (var s in ((SecuritiesEventArgs)state).Securities)
            {
                var r = transaq.IsConnected ? FindSecurity(s.Id) : null;
                if (r == null)
                {
                    var index = dataGridViewSecurities.Rows.Add();
                    r = dataGridViewSecurities.Rows[index];
                }
                r.Cells[SecId.Index].Value = s.Id;
                r.Cells[SecCode.Index].Value = s.Code;
                r.Cells[SecName.Index].Value = s.Name;
                r.Cells[SecType.Index].Value = s.SecurityType;
                r.Cells[SecMarket.Index].Value = transaq.GetMarket(s.Market);
            }
        }

        DataGridViewRow FindSecurity(int id)
        {
            foreach (DataGridViewRow r in dataGridViewSecurities.Rows)
            {
                if (id.Equals(r.Cells[SecId.Index].Value)) return r;
            }
            return null;
        }

        void transaq_Logging(object state)
        {
            var data = string.Empty;
            if (state is LogEventArgs)
            {
                data = XDocument.Parse(((LogEventArgs)state).Data).ToString();
            }
            else if (state is LogEventArgs)
            {
                data = ((ErrorEventArgs)state).Error.ToString();
            }
            if (!string.IsNullOrEmpty(data))
            {
                richTextBoxLog.AppendText(data);
                richTextBoxLog.AppendText(Environment.NewLine);
                richTextBoxLog.AppendText(Environment.NewLine);
                richTextBoxLog.ScrollToCaret();
            }
        }

        void transaq_Disconnected(object state)
        {
            if (needClose)
            {
                Close();
            }
            else
            {
                buttonConnect.Text = "Подключиться";
                buttonConnect.Enabled = true;
            }
        }

        void transaq_ConnectionError(object state)
        {
            buttonConnect.Enabled = true;
        }

        void transaq_Connected(object state)
        {
            buttonConnect.Text = "Отключиться";
            buttonConnect.Enabled = true;
            comboBoxKind.Items.AddRange(transaq.CandleKinds.ToArray());
            comboBoxKind.SelectedIndex = 0;
        }

        private void buttonClearLog_Click(object sender, EventArgs e)
        {
            richTextBoxLog.Clear();
        }

        private void buttonConnect_Click(object sender, EventArgs e)
        {
            try
            {
                buttonConnect.Enabled = false;
                if (!transaq.IsConnected)
                {
                    transaq.AutoPos = checkBoxAutoPos.Checked;
                    transaq.Connect(textBoxServer.Text, (int)numericUpDownPort.Value, textBoxLogin.Text, textBoxPassword.Text);
                }
                else
                {
                    transaq.Disconnect();
                }
            }
            catch
            {
                buttonConnect.Enabled = true;
                throw;
            }
        }

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            needClose = true;
            e.Cancel = transaq.IsConnected || transaq.IsConnecting;
            transaq.Disconnect();
        }

        private void buttonSubscribe_Click(object sender, EventArgs e)
        {
            var selected = GetChecked();
            transaq.Subscribe(Subscription.Quotations, selected);
            foreach (var id in selected)
            {
                var row = FindSecurity(id);
                row.DefaultCellStyle.BackColor = Color.Yellow;
            }
        }

        private void buttonUnsubscribe_Click(object sender, EventArgs e)
        {
            var selected = GetChecked();
            transaq.Unsubscribe(Subscription.Quotations, GetChecked());
            foreach (var id in selected)
            {
                var row = FindSecurity(id);
                row.DefaultCellStyle.BackColor = Color.Empty;
            }
        }

        private int[] GetChecked()
        {
            var result = new List<int>();
            foreach (DataGridViewRow r in dataGridViewSecurities.Rows)
            {
                if (true.Equals(r.Cells[SecSelect.Index].Value))
                {
                    result.Add((int)r.Cells[SecId.Index].Value);
                }
            }
            return result.ToArray();
        }

        private void buttonHistory_Click(object sender, EventArgs e)
        {
            var kind = (CandleKind)comboBoxKind.SelectedItem;
            foreach (var id in GetChecked())
            {
                transaq.GetHistoryData(id, kind.Id, 100, true);
            }
        }
    }
}
