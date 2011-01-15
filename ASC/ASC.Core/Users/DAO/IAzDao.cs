using System;
using System.Collections.Generic;
using ASC.Common.Security.Authorizing;

namespace ASC.Core.Users.DAO
{
	public interface IAzDAO
	{
		List<AzRecord> GetAce(Guid? subjectID, Guid? actionID, string objectId);

		void SaveAce(AzRecord record);

		void RemoveAce(AzRecord record);


		List<AzObjectInfo> GetAzObjectInfos();

		AzObjectInfo GetAzObjectInfo(string objectId);

		void SaveAzObjectInfo(AzObjectInfo azObjectInfo);

		void RemoveAzObjectInfo(AzObjectInfo azObjectInfo);
	}
}