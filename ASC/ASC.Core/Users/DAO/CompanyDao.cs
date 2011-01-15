using System;
using System.Collections.Generic;

namespace ASC.Core.Users.DAO
{
	class CompanyDAO : DAOBase, ICompanyDAO
	{
		public CompanyDAO(string dbId, int tenant)
			: base(dbId, tenant)
		{

		}

		#region ICompanyDAO Members

		/// <inheritdoc/>
		public List<CompanyProperty> GetCompanyProperties()
		{
			return DbManager
				.ExecuteList(Query("core_property").Select("Name", "Value"))
				.ConvertAll<CompanyProperty>(r => new CompanyProperty((string)r[0], (string)r[1]));
		}

		/// <inheritdoc/>
		public void SaveCompanyProperty(CompanyProperty p)
		{
			if (string.IsNullOrEmpty(p.Value))
			{
				DbManager.ExecuteNonQuery(Delete("core_property").Where("Name", p.Name));
			}
			else
			{
				DbManager.ExecuteNonQuery(Insert("core_property").InColumnValue("Name", p.Name).InColumnValue("Value", p.Value));
			}
		}

		/// <inheritdoc/>
		public List<DepartmentManagerRef> GetDepartmentManagerRefs()
		{
			return DbManager
				.ExecuteList(Query("core_groupmanager").Select("UserID", "GroupID"))
				.ConvertAll<DepartmentManagerRef>(
					r => new DepartmentManagerRef(new Guid(Convert.ToString(r[1])), new Guid(Convert.ToString(r[0])))
				);
		}

		/// <inheritdoc/>
		public void SetDepartmentManager(Guid deparmentID, Guid userID)
		{
			DbManager.ExecuteNonQuery(
				Insert("core_groupmanager").InColumns("UserID", "GroupID").Values(userID.ToString(), deparmentID.ToString())
			);
		}

		/// <inheritdoc/>
		public bool RemoveDepartmentManager(Guid deparmentID, Guid userID)
		{
			return 0 < DbManager.ExecuteNonQuery(
				Delete("core_groupmanager").Where("UserID", userID.ToString()).Where("GroupID", deparmentID.ToString())
			);
		}

		#endregion
	}
}