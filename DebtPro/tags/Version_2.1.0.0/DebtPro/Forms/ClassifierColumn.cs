using System;
using System.Windows.Forms;
using CI.Debt.DAO;
using CI.Debt.Domain;

// Подробнее о DataGridView см. http://www.rsdn.ru/article/dotnet/datagridfaq.xml?print
// http://www.rsdn.ru/article/dotnet/DataGridView20.xml?print
// http://www.rsdn.ru/article/dotnet/DataGridView20part2.xml?print

namespace CI.Debt.Forms {

	/// <summary>
	/// Тип определет столбец классификаторов в DataGridView.
	/// </summary>
	public class ClassifierColumn : DataGridViewColumn {

		/// <inheritdoc/>
		public ClassifierColumn() : base(new ClassifierCell()) { }
	}

	/// <summary>
	/// Тип определет ячеку столбца классификаторов.
	/// </summary>
	public class ClassifierCell : DataGridViewTextBoxCell {

		/// <inheritdoc/>
		public ClassifierCell() : base() { }

		/// <inheritdoc/>
		public override void InitializeEditingControl(int rowIndex, object initialFormattedValue, DataGridViewCellStyle dataGridViewCellStyle) {
			base.InitializeEditingControl(rowIndex, initialFormattedValue, dataGridViewCellStyle);
			var str = initialFormattedValue as string;
			if (!string.IsNullOrEmpty(str)) DataGridView.EditingControl.Text = str;
		}

		/// <inheritdoc/>
		public override Type EditType {
			get { return typeof(ClassifierEditingControl); }
		}

		/// <inheritdoc/>
		public override object ParseFormattedValue(object formattedValue, DataGridViewCellStyle cellStyle, System.ComponentModel.TypeConverter formattedValueTypeConverter, System.ComponentModel.TypeConverter valueTypeConverter) {
			try {
				var clsfCode = formattedValue as string;
				if (string.IsNullOrEmpty(clsfCode)) return null;

				var clsf = DebtDAO.FindClassifier(clsfCode);
				if (clsf == null) {
					ErrorText = string.Format("Классификатор с кодом {0} не обнаружен в справочнике.\r\nБудет создан новый классификатор", clsfCode);
					return new Classifier() { MaskedCode = clsfCode, GrpName12 = "<Новый классификатор>" };
				}
				ErrorText = string.Empty;
				return clsf;
			}
			catch (ClassifierFormatException e) {
				ErrorText = e.Message;
				return Classifier.Empty;
			}
		}
	}
}