using System.Windows.Forms;

namespace CI.Debt.Forms {

	public partial class WaitForm : Form {

		public bool CanClose {
			get;
			set;
		}

		public WaitForm() {
			InitializeComponent();
			CanClose = false;
		}

		private void WaitForm_FormClosing(object sender, FormClosingEventArgs e) {
			e.Cancel = !CanClose;
		}
	}
}
