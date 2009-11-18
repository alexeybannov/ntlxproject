using System.Windows.Forms;

namespace CI.Debt.Forms {

	public class DataGridViewEx : DataGridView {

		public DataGridViewEx()
			: base() {
			SetStyle(ControlStyles.OptimizedDoubleBuffer, true);
		}
	}
}
