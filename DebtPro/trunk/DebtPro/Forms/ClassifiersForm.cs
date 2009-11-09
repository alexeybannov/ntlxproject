using System;
using System.Collections;
using System.Collections.Generic;
using System.Windows.Forms;
using CI.Debt.Domain;
using CI.Debt.DAO;

namespace CI.Debt.Forms {

	/// <summary>
	/// “ип открыти€ формы классификатор.
	/// </summary>
	enum ClassifiersFormMode {

		/// <summary>
		/// –ежим просмотра.
		/// </summary>
		Browse,

		/// <summary>
		/// –ежим просмотра, с возможностью выбора классификатора.
		/// </summary>
		Select
	}

	/// <summary>
	/// ‘орма классификаторов.
	/// </summary>
	partial class ClassifiersForm : Form, IClassifiersView {

		private ClassifiersFormMode mode;

		private Hashtable cacheNodes;

		private Classifier selectedClassifier;

		/// <summary>
		/// —оздает экземпл€р типа ClassifiersForm с режимом Browse по умолчанию.
		/// </summary>
		public ClassifiersForm() : this(ClassifiersFormMode.Browse) { }

		/// <summary>
		/// —оздает экземпл€р типа ClassifiersForm.
		/// </summary>
		/// <param name="mode">–ежим формы</param>
		public ClassifiersForm(ClassifiersFormMode mode)
			: base() {

			InitializeComponent();
			this.mode = mode;
			buttonSelect.Visible = (this.mode == ClassifiersFormMode.Select);
			dataGridView.AutoGenerateColumns = false;
			cacheNodes = new Hashtable();
		}

		#region TreeView

		private void FillLevel(int level, TreeNodeCollection nodes, IList<Classifier> sortedClsfs) {
			if (level == 1) cacheNodes.Clear();
			nodes.Clear();
			if (sortedClsfs.Count == 0 || 5 < level) return;

			IList<Classifier> clsf = new List<Classifier>();

			Classifier currClsf = sortedClsfs[0];
			clsf.Add(currClsf);
			TreeNode node = AddNode(level, currClsf, nodes);

			for (int i = 1; i < sortedClsfs.Count; i++) {
				if (CompareClassifiers(level, currClsf, sortedClsfs[i])) {
					clsf.Add(sortedClsfs[i]);
				}
				else {
					FillLevel(level + 1, node.Nodes, clsf);

					currClsf = sortedClsfs[i];
					clsf.Clear();
					clsf.Add(currClsf);
					node = AddNode(level, currClsf, nodes);
				}
			}
			FillLevel(level + 1, node.Nodes, clsf);
		}

		private TreeNode AddNode(int level, Classifier clsf, TreeNodeCollection nodes) {
			TreeNode node = new TreeNode(GetLevelText(level, clsf, true)) {
				ImageIndex = (level != 5 ? 0 : 2),
				SelectedImageIndex = (level != 5 ? 0 : 2),
				Tag = (level != 5 ? null : clsf)
			};
			nodes.Add(node);
			if (level == 5) cacheNodes.Add(clsf.Id, node);
			return node;
		}

		private bool CompareClassifiers(int level, Classifier x, Classifier y) {
			return string.Compare(GetLevelText(level, x, false), GetLevelText(level, y, false)) == 0;
		}

		private string GetLevelText(int level, Classifier clsf, bool withClsfName) {
			switch (level) {
				case 1: return string.Format("{0}     {1}", clsf.GrpCode01, (withClsfName ? clsf.GrpName01 : null)).Trim();
				case 2: return string.Format("{0} {1}     {2}", clsf.GrpCode02, clsf.GrpCode03, (withClsfName ? string.Format("{0} {1}", clsf.GrpName02, clsf.GrpName03).Trim() : null)).Trim();
				case 3: return string.Format("{0} {1} {2}     {3}", clsf.GrpCode04, clsf.GrpCode05, clsf.GrpCode06, (withClsfName ? string.Format("{0} {1} {2}", clsf.GrpName04, clsf.GrpName05, clsf.GrpName06).Trim() : null)).Trim();
				case 4: return string.Format("{0} {1}     {2}", clsf.GrpCode07, clsf.GrpCode08, (withClsfName ? string.Format("{0} {1}", clsf.GrpName07, clsf.GrpName08).Trim() : null)).Trim();
				case 5: return string.Format("{0}     {1}", clsf.MaskedCode, (withClsfName ? string.Format("{0} {1}", clsf.GrpName11, clsf.GrpName12).Trim() : null)).Trim();
			}
			return string.Empty;
		}

		private void treeView1_BeforeExpand(object sender, TreeViewCancelEventArgs e) {
			if (e.Node != null) e.Node.SelectedImageIndex = e.Node.ImageIndex = 1;
		}

		private void treeView1_BeforeSelect(object sender, TreeViewCancelEventArgs e) {
			if (e.Node != null && e.Node.Level != 4) {
				e.Node.SelectedImageIndex = e.Node.ImageIndex = (e.Node.IsExpanded ? 1 : 0);
			}
		}

		private void treeView1_BeforeCollapse(object sender, TreeViewCancelEventArgs e) {
			if (e.Node != null) e.Node.SelectedImageIndex = e.Node.ImageIndex = 0;
		}

		private void treeView1_NodeMouseDoubleClick(object sender, TreeNodeMouseClickEventArgs e) {
			TreeNode node = treeView.SelectedNode;
			if (node != null && node.Level == 4 && mode == ClassifiersFormMode.Select) {
				DialogResult = DialogResult.OK;
				Close();
			}
		}

		#endregion

		#region IClassifiersView Members

		public void ShowClassifiers(IList<Classifier> classifiers) {
			classifierBindingSource.DataSource = classifiers;

			treeView.BeginUpdate();
			FillLevel(1, treeView.Nodes, classifiers);
			treeView.EndUpdate();

			if (this.ShowDialog() == DialogResult.OK) {
				if (mode == ClassifiersFormMode.Select && ClassifierSelected != null) {
					ClassifierSelected(this, EventArgs.Empty);
				}
			}
		}

		public Classifier SelectedClassifier {
			get {
				Classifier clsf = null;
				if (radioButtonTree.Checked) {
					if (treeView.SelectedNode != null && treeView.SelectedNode.Level == 4) clsf = treeView.SelectedNode.Tag as Classifier;
				}
				else {
					clsf = classifierBindingSource.Current as Classifier;
				}
				return clsf;
			}
			set {
				selectedClassifier = value;
			}
		}

		public event EventHandler ClassifierSelected;

		#endregion

		private void radioButton_CheckedChanged(object sender, EventArgs e) {
			treeView.Visible = radioButtonTree.Checked;
			dataGridView.Visible = radioButtonList.Checked;
		}

		private void dataGridView_CellMouseDoubleClick(object sender, DataGridViewCellMouseEventArgs e) {
			if (0 <= e.RowIndex && mode == ClassifiersFormMode.Select) {
				DialogResult = DialogResult.OK;
				Close();
			}
		}

		private void ClassifiersForm_Shown(object sender, EventArgs e) {
			if (radioButtonList.Checked) {
				if (classifierBindingSource.DataSource != null &&
					selectedClassifier != null &&
					0 <= classifierBindingSource.IndexOf(selectedClassifier)) {

					classifierBindingSource.Position = classifierBindingSource.IndexOf(selectedClassifier);
				}
			}
			else {
				if (selectedClassifier != null && cacheNodes.ContainsKey(selectedClassifier.Id)) {
					TreeNode node = (TreeNode)cacheNodes[selectedClassifier.Id];
					node.EnsureVisible();
					treeView.SelectedNode = node;
				}
			}
		}
	}
}