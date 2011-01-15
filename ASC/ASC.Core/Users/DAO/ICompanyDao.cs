using System;
using System.Collections.Generic;

namespace ASC.Core.Users.DAO
{
	public interface ICompanyDAO
	{
		List<CompanyProperty> GetCompanyProperties();

		void SaveCompanyProperty(CompanyProperty property);

		List<DepartmentManagerRef> GetDepartmentManagerRefs();

		void SetDepartmentManager(Guid deparmentID, Guid userID);

		bool RemoveDepartmentManager(Guid deparmentID, Guid userID);
	}
}