using System;

namespace ASC.Core.Users.DAO
{
	static class UserGroupMapper
	{
		public static string[] UserColumns
		{
			get;
			private set;
		}

		public static string[] CategoryColumns
		{
			get;
			private set;
		}

		public static string[] UserGroupColumns
		{
			get;
			private set;
		}

		public static string[] GroupColumns
		{
			get;
			private set;
		}

		static UserGroupMapper()
		{
			UserColumns = new[]
			{
				"ID",
				"FirstName", "LastName", "Sex", "BithDate", 
				"UserName", "Status",
				"Title", "Department", "WorkFromDate", "TerminatedDate",
				"Contacts", "Email", 
				"Location",
				"Notes",
				"Timestamp",
			};

			CategoryColumns = new[]
			{
				"ID", "ModuleID", "Name", "Description", "GroupType"
			};
			
			UserGroupColumns = new[]
			{
				"UserID", "GroupID"
			};

			GroupColumns = new[]
			{
				"ID", "Name", "Description", "CategoryID", "ParentID"
			};
		}


		public static UserInfo ToUser(object[] r)
		{
			return new UserInfo()
			{
				ID = ToGuid(r[0]),

				FirstName = (string)r[1],
				LastName = (string)r[2],
				Sex = r[3] !=null ? (bool?)Convert.ToBoolean(r[3]) : null,
				BirthDate = ToNullableDateTime(r[4]),

				UserName = (string)r[5],
				Status = (EmployeeStatus)Convert.ToInt64(r[6]),

				Title = (string)r[7],
				Department = (string)r[8],
				WorkFromDate = ToNullableDateTime(r[9]),
				TerminatedDate = ToNullableDateTime(r[10]),

				Email = (string)r[12],

                Location = (string)r[13],
				Notes = (string)r[14],
			}
			.ContactsFromString((string)r[11]);
		}

		public static GroupCategory ToCategory(object[] r)
		{
			return new GroupCategory()
			{
				ID = ToGuid(r[0]),
				ModuleID = ToGuid(r[1]),
				Name = (string)r[2],
				Description = (string)r[3],
				GroupType = (GroupType)Convert.ToInt64(r[4])
			};
		}

		public static UserGroupReference ToUserGroupRef(object[] r)
		{
			return new UserGroupReference(ToGuid(r[0]), ToGuid(r[1]));
		}

		public static GroupInfo ToGroup(object[] r)
		{
			return new GroupInfo()
			{
				ID = ToGuid(r[0]),
				Name = (string)r[1],
				Description = (string)r[2],
				CategoryID = ToGuid(r[3]),
				ParentID = ToGuid(r[4])
			};
		}

		public static Guid ToGuid(object value)
		{
			if (value == null) return default(Guid);
			if (value is Guid) return (Guid)value;
			if (value is string) return new Guid((string)value);
			throw new InvalidCastException("Can not convert " + value.ToString() + " to Guid.");
		}

		private static DateTime? ToNullableDateTime(object value)
		{
			return value != null ? (DateTime?)Convert.ToDateTime(value) : null;
		}
	}
}