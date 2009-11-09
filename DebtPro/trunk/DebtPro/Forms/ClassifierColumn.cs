using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Forms;
using CI.Debt.Domain;
using CI.Debt.DAO;

namespace CI.Debt.Forms {

	/// <summary>
	/// Тип определет столбец классификаторов в DataGridView.
	/// Подробнее о DataGridView см. http://www.rsdn.ru/article/dotnet/datagridfaq.xml?print
	/// http://www.rsdn.ru/article/dotnet/DataGridView20.xml?print
	/// http://www.rsdn.ru/article/dotnet/DataGridView20part2.xml?print
	/// </summary>
	public class ClassifierColumn : DataGridViewColumn {

		/// <inheritdoc/>
		public ClassifierColumn() : base(new ClassifierCell()) { }

		/// <inheritdoc/>
		public override DataGridViewCell CellTemplate {
			get { return base.CellTemplate; }
			set {
				if (value != null && !value.GetType().IsAssignableFrom(typeof(ClassifierCell))) throw new InvalidCastException("Must be a ClassifierCell");
				base.CellTemplate = value;
			}
		}
	}

	/// <summary>
	/// Тип определет ячеку столбца классификаторов.
	/// Подробнее о DataGridView см. http://www.rsdn.ru/article/dotnet/datagridfaq.xml?print
	/// http://www.rsdn.ru/article/dotnet/DataGridView20.xml?print
	/// http://www.rsdn.ru/article/dotnet/DataGridView20part2.xml?print
	/// </summary>
	public class ClassifierCell : DataGridViewTextBoxCell {

		/// <inheritdoc/>
		public ClassifierCell() : base() { }

		/// <inheritdoc/>
		public override void InitializeEditingControl(int rowIndex, object initialFormattedValue, DataGridViewCellStyle dataGridViewCellStyle) {
			base.InitializeEditingControl(rowIndex, initialFormattedValue, dataGridViewCellStyle);
			ClassifierEditingControl ctl = DataGridView.EditingControl as ClassifierEditingControl;
			Classifier clsf = Value as Classifier;
			if (clsf != null) ctl.Text = clsf.MaskedCode;
		}

		/// <inheritdoc/>
		public override Type EditType {
			get { return typeof(ClassifierEditingControl); }
		}

		/// <inheritdoc/>
		public override Type FormattedValueType {
			get { return typeof(String); }
		}

		/// <inheritdoc/>
		public override Type ValueType {
			get { return typeof(Classifier); }
		}

		/// <inheritdoc/>
		public override object ParseFormattedValue(object formattedValue, DataGridViewCellStyle cellStyle, System.ComponentModel.TypeConverter formattedValueTypeConverter, System.ComponentModel.TypeConverter valueTypeConverter) {
			try {
				if (formattedValue == null) return null;
				Classifier clsf = DebtDAO.FindClassifier((string)formattedValue);
				ErrorText = string.Empty;
				return clsf;
			}
			catch (ClassifierNotFoundException notFoundExc) {
				try {
					ErrorText = string.Format("{0}\r\n{1}", notFoundExc.Message, "Будет создан новый классификатор.");
					return new Classifier() { MaskedCode = (string)formattedValue, GrpName12 = "<Новый классификатор>" };
				}
				catch (ClassifierFormatException formatEx) {
					ErrorText = formatEx.Message;
					return DebtDAO.EmptyClassifier;
				}
			}
		}
	}
}