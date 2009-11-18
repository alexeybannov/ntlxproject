using CI.Debt.Domain;
using NHibernate;

namespace CI.Debt.DAO {

	class DebtInterceptor : EmptyInterceptor {

		public override bool OnSave(object entity, object id, object[] state, string[] propertyNames, NHibernate.Type.IType[] types) {
			var unsavedable = entity as IUnsavedable;
			if (unsavedable != null) unsavedable.IsUnsaved = false;
			return base.OnSave(entity, id, state, propertyNames, types);
		}

		public override object IsUnsaved(object entity) {
			var unsavedable = entity as IUnsavedable;
			return unsavedable != null ? unsavedable.IsUnsaved : base.IsUnsaved(entity);
		}
	}
}
