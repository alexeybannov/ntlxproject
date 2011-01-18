using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using ASC.Common.Data.Sql;
using ASC.Common.Data.Sql.Expressions;
using ASC.Core.Data;

namespace ASC.Core
{
    public class DbUserService : DbBaseService, IUserService
    {
        private readonly string tenantColumn = "tenant";


        public DbUserService(ConnectionStringSettings connectionString)
            : base(connectionString)
        {

        }


        public IEnumerable<User> GetUsers(int tenant, DateTime from)
        {
            return ExecList(GetUserQuery(tenant, from))
                .ConvertAll(r => ToUser(r));
        }

        public User SaveUser(int tenant, User user)
        {
            if (user == null) throw new ArgumentNullException("user");
            if (user.Id == default(Guid)) user.Id = Guid.NewGuid();

            var i = new SqlInsert("core_user", true)
                .InColumnValue("id", user.Id.ToString())
                .InColumnValue("username", user.UserName)
                .InColumnValue("firstname", user.FirstName)
                .InColumnValue("lastname", user.LastName)
                .InColumnValue("sex", user.Sex)
                .InColumnValue("bithdate", user.BirthDate)
                .InColumnValue("status", user.Status)
                .InColumnValue("title", user.Title)
                .InColumnValue("department", user.Department)
                .InColumnValue("workfromdate", user.WorkFromDate)
                .InColumnValue("terminateddate", user.WorkToDate)
                .InColumnValue("contacts", user.Contacts)
                .InColumnValue("email", user.Email)
                .InColumnValue("location", user.Location)
                .InColumnValue("notes", user.Notes)
                .InColumnValue("removed", user.Removed)
                .InColumnValue("last_modified", DateTime.UtcNow)
                .InColumnValue(tenantColumn, tenant);

            ExecNonQuery(i);

            return user;
        }

        public void RemoveUser(int tenant, Guid id)
        {
            var sid = id.ToString();
            var batch = new List<ISqlInstruction>();

            batch.Add(new SqlDelete("core_acl").Where("subject", sid).Where(tenantColumn, tenant));
            batch.Add(new SqlDelete("core_subscription").Where("recipient", sid).Where(tenantColumn, tenant));
            batch.Add(new SqlDelete("core_subscriptionmethod").Where("recipient", sid).Where(tenantColumn, tenant));
            batch.Add(new SqlDelete("core_groupmanager").Where("userid", sid).Where(tenantColumn, tenant));
            batch.Add(new SqlDelete("core_usergroup").Where("userid", sid).Where(tenantColumn, tenant));
            batch.Add(new SqlDelete("core_userphoto").Where("userid", sid).Where(tenantColumn, tenant));
            batch.Add(new SqlDelete("core_usersecurity").Where("userid", sid).Where(tenantColumn, tenant));
            batch.Add(new SqlDelete("core_user").Where("id", sid).Where(tenantColumn, tenant));

            ExecBatch(batch);
        }

        public void SetUserPhoto(int tenant, Guid id, byte[] photo)
        {
            var sql = photo != null && photo.Length != 0 ?
                (ISqlInstruction)new SqlInsert("core_userphoto").InColumns("userid", "photo", tenantColumn).Values(id.ToString(), photo, tenant) :
                (ISqlInstruction)new SqlDelete("core_userphoto").Where("userid", id.ToString()).Where(tenantColumn, tenant);

            ExecNonQuery(sql);
        }

        public byte[] GetUserPhoto(int tenant, Guid id)
        {
            var photo = ExecScalar<byte[]>(new SqlQuery("core_userphoto").Select("photo").Where("userid", id.ToString()).Where(tenantColumn, tenant));
            return photo ?? new byte[0];
        }


        public IEnumerable<Group> GetGroups(int tenant, DateTime from)
        {
            return ExecList(GetGroupQuery(tenant, from))
                .ConvertAll(r => ToGroup(r));
        }

        public Group SaveGroup(int tenant, Group group)
        {
            if (group == null) throw new ArgumentNullException("user");
            if (group.Id == default(Guid)) group.Id = Guid.NewGuid();

            var batch = new List<ISqlInstruction>();
            var i = new SqlInsert("core_user", true)
                .InColumnValue("id", group.Id.ToString())
                .InColumnValue("name", group.Name)
                .InColumnValue("parentid", group.ParentId.ToString())
                .InColumnValue("categoryid", group.CategoryId.ToString())
                .InColumnValue("removed", group.Removed)
                .InColumnValue("last_modified", DateTime.UtcNow)
                .InColumnValue(tenantColumn, tenant);

            batch.Add(i);
            if (group.Removed)
            {
                var ids = CollectGroupChilds(tenant, group.Id.ToString());
                batch.Add(new SqlUpdate("core_group").Set("removed", true).Set("last_modified", DateTime.UtcNow).Where(Exp.In("id", ids)).Where(tenantColumn, tenant));
                batch.Add(SetDepartmentNullUpdate(tenant, ids));
            }

            ExecBatch(batch);

            return group;
        }

        public void RemoveGroup(int tenant, Guid id)
        {
            var batch = new List<ISqlInstruction>();
            var ids = CollectGroupChilds(tenant, id.ToString());
            ids.Add(id.ToString());

            batch.Add(new SqlDelete("core_acl").Where(Exp.In("subject", ids)).Where(tenantColumn, tenant));
            batch.Add(new SqlDelete("core_subscription").Where(Exp.In("recipient", ids)).Where(tenantColumn, tenant));
            batch.Add(new SqlDelete("core_subscriptionmethod").Where(Exp.In("recipient", ids)).Where(tenantColumn, tenant));
            batch.Add(new SqlDelete("core_groupmanager").Where(Exp.In("groupid", ids)).Where(tenantColumn, tenant));
            batch.Add(SetDepartmentNullUpdate(tenant, ids));
            batch.Add(new SqlDelete("core_usergroup").Where(Exp.In("groupid", ids)).Where(tenantColumn, tenant));
            batch.Add(new SqlDelete("core_group").Where(Exp.In("id", ids)).Where(tenantColumn, tenant));

            ExecBatch(batch);
        }


        public IEnumerable<UserGroupRef> GetUserGroupRefs(int tenant, DateTime from)
        {
            return ExecList(GetUserGroupRefQuery(tenant, default(DateTime)))
                .ConvertAll(r => ToUserGroupRef(r));
        }

        public UserGroupRef SaveUserGroupRef(int tenant, UserGroupRef r)
        {
            if (r == null) throw new ArgumentNullException("userGroupRef");

            var table = r.RefType == UserGroupRefType.Contains ? "core_usergroup" : "core_groupmanager";
            var i = new SqlInsert(table, true)
                .InColumnValue("userid", r.UserId.ToString())
                .InColumnValue("groupid", r.GroupId.ToString())
                .InColumnValue("removed", r.Removed)
                .InColumnValue("last_modified", DateTime.UtcNow)
                .InColumnValue(tenantColumn, tenant);

            ExecNonQuery(i);

            return r;
        }

        public void RemoveUserGroupRef(int tenant, Guid userId, Guid groupId, UserGroupRefType refType)
        {
            var table = refType == UserGroupRefType.Contains ? "core_usergroup" : "core_groupmanager";
            var d = new SqlDelete(table)
                .Where(tenantColumn, tenant)
                .Where("userid", userId.ToString())
                .Where("groupid", groupId.ToString());

            ExecNonQuery(d);
        }


        private SqlQuery GetUserQuery(int tenant, DateTime from)
        {
            var q = new SqlQuery("core_user")
                .Select("id", "username", "firstname", "lastname", "sex", "bithdate")
                .Select("status", "title", "department", "workfromdate", "terminateddate")
                .Select("contacts", "email", "location", "notes", "removed")
                .Where(tenantColumn, tenant);
            if (from != default(DateTime)) q.Where(Exp.Ge("last_modified", from));

            return q;
        }

        private User ToUser(object[] r)
        {
            return new User
            {
                Id = new Guid((string)r[0]),
                UserName = (string)r[1],
                FirstName = (string)r[2],
                LastName = (string)r[3],
                Sex = r[4] != null ? Convert.ToBoolean(r[4]) : (bool?)null,
                BirthDate = (DateTime?)r[5],
                Status = Convert.ToInt32(r[6]),
                Title = (string)r[7],
                Department = (string)r[8],
                WorkFromDate = (DateTime?)r[9],
                WorkToDate = (DateTime?)r[10],
                Contacts = (string)r[11],
                Email = (string)r[12],
                Location = (string)r[13],
                Notes = (string)r[14],
                Removed = Convert.ToBoolean(r[15]),
            };
        }


        private SqlQuery GetGroupQuery(int tenant, DateTime from)
        {
            var q = new SqlQuery("core_group")
                .Select("id", "name", "parentid", "categoryid", "removed")
                .Where(tenantColumn, tenant);
            if (from != default(DateTime)) q.Where(Exp.Ge("last_modified", from));

            return q;
        }

        private Group ToGroup(object[] r)
        {
            return new Group
            {
                Id = new Guid((string)r[0]),
                Name = (string)r[1],
                ParentId = r[2] != null ? new Guid((string)r[2]) : Guid.Empty,
                CategoryId = r[3] != null ? new Guid((string)r[3]) : Guid.Empty,
                Removed = Convert.ToBoolean(r[4]),
            };
        }

        private List<string> CollectGroupChilds(int tenant, string id)
        {
            var result = new List<string>();
            var childs = ExecList(new SqlQuery("core_group").Select("id").Where("parentid", id).Where(tenantColumn, tenant))
                .ConvertAll(r => (string)r[0]);
            foreach (var child in childs)
            {
                result.Add(child);
                result.AddRange(CollectGroupChilds(tenant, child));
            }
            return result.Distinct().ToList();
        }

        private SqlUpdate SetDepartmentNullUpdate(int tenant, List<string> groups)
        {
            var users = new SqlQuery("core_usergroup").Select("userid").Where(Exp.In("groupid", groups));
            return new SqlUpdate("core_user")
                .Set("department", null)
                .Set("last_modified", DateTime.UtcNow)
                .Where(tenantColumn, tenant)
                .Where(Exp.In("id", users));
        }


        private SqlQuery GetUserGroupRefQuery(int tenant, DateTime from)
        {
            var q1 = new SqlQuery("core_usergroup")
                .Select("userid", "groupid", "1", "removed")
                .Where(tenantColumn, tenant);
            if (from != default(DateTime)) q1.Where(Exp.Ge("last_modified", from));

            var q2 = new SqlQuery("core_groupmanager")
                .Select("userid", "groupid", "0", "removed")
                .Where(tenantColumn, tenant);
            if (from != default(DateTime)) q2.Where(Exp.Ge("last_modified", from));

            return q1.Union(q2);
        }

        private UserGroupRef ToUserGroupRef(object[] r)
        {
            return new UserGroupRef
            {
                UserId = new Guid((string)r[0]),
                GroupId = new Guid((string)r[1]),
                RefType = (UserGroupRefType)Convert.ToInt32(r[2]),
                Removed = Convert.ToBoolean(r[3]),
            };
        }
    }
}
