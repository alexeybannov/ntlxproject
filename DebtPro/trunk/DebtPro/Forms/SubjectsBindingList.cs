using System;
using System.Collections.Generic;
using System.ComponentModel;
using CI.Debt.Domain;

namespace CI.Debt.Forms {

	class SubjectsBindingList : BindingList<Subject> {

		private bool sorted = false;

		public SubjectsBindingList(IList<Subject> subjects) : base(subjects) { }

		protected override bool SupportsSortingCore {
			get { return true; }
		}

		protected override bool IsSortedCore {
			get { return sorted; }
		}

		protected override void ApplySortCore(PropertyDescriptor prop, ListSortDirection direction) {
			List<Subject> list = this.Items as List<Subject>;
			if (list != null) {
				list.Sort(new SubjectsComparer(prop, direction));
				sorted = true;
			}
			else {
				sorted = false;
			}
		}

		private class SubjectsComparer : IComparer<Subject> {

			private PropertyDescriptor prop;
			private ListSortDirection direction;

			public SubjectsComparer(PropertyDescriptor prop, ListSortDirection direction) {
				this.prop = prop;
				this.direction = direction;
			}

			#region IComparer Members

			public int Compare(Subject x, Subject y) {
				Subject s1 = (direction == ListSortDirection.Ascending ? x : y);
				Subject s2 = (direction == ListSortDirection.Ascending ? y : x);
				switch (prop.Name) {
					case "Name": return string.Compare(s1.Name, s2.Name);
					case "Code": return string.Compare(s1.Code, s2.Code);
					case "BudgetName": return string.Compare(s1.BudgetName, s2.BudgetName);
				}
				throw new ArgumentOutOfRangeException("PropertyDescriptor");
			}

			#endregion
		}
	}	
}