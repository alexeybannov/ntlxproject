using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using ASC.Common.Data.Sql;
using ASC.Common.Data.Sql.Expressions;
using ASC.Core.Data;
using ASC.Core.Tenants;
using ASC.Core.Users;
using ASC.Security.Cryptography;

namespace ASC.Core.Data
{
    public class DbUserService : DbBaseService, IUserService
    {
        public DbUserService(ConnectionStringSettings connectionString)
            : base(connectionString, "tenant")
        {

        }


        public IEnumerable<User> GetUsers(int tenant, DateTime from)
        {
            var q = GetUserQuery(tenant, from);
            return ExecList(q).Select(r => ToUser(r));
        }

        public User GetUser(int tenant, Guid id)
        {
            var q = GetUserQuery(tenant, default(DateTime)).Where("id", id);
            return ExecList(q).Select(r => ToUser(r)).SingleOrDefault();
        }

        public User GetUser(int tenant, string login, string password)
        {
            if (string.IsNullOrEmpty(login)) throw new ArgumentNullException("login");

            var q = GetUserQuery()
                .InnerJoin("core_usersecurity s", Exp.EqColumns("u.id", "s.userid"))
                .Where("u.status", (int)EmployeeStatus.Active)
                .Where(Exp.Eq("upper(u.email)", login.ToUpperInvariant()) | Exp.Eq("u.id", login))
                .Where("s.pwdhash", password);
            if (tenant != Tenant.DEFAULT_TENANT)
            {
                q.Where("u.tenant", tenant);
            }

            return ExecList(q).ConvertAll(r => ToUser(r)).SingleOrDefault();
        }

        public User SaveUser(int tenant, User user)
        {
            if (user == null) throw new ArgumentNullException("user");
            
            if (user.Id == default(Guid)) user.Id = Guid.NewGuid();
            user.LastModified = DateTime.UtcNow;
            user.Tenant = tenant;

            var i = Insert("core_user", tenant)
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
                .InColumnValue("last_modified", user.LastModified);
            ExecNonQuery(i);
            return user;
        }

        public void RemoveUser(int tenant, Guid id)
        {
            RemoveUser(tenant, id, false);
        }

        public void RemoveUser(int tenant, Guid id, bool immediate)
        {
            var sid = id.ToString();
            var batch = new List<ISqlInstruction>();

            batch.Add(Delete("core_acl", tenant).Where("subject", sid));
            batch.Add(Delete("core_subscription", tenant).Where("recipient", sid));
            batch.Add(Delete("core_subscriptionmethod", tenant).Where("recipient", sid));
            batch.Add(Delete("core_userphoto", tenant).Where("userid", sid));
            if (immediate)
            {
                batch.Add(Delete("core_usergroup", tenant).Where("userid", sid));
                batch.Add(Delete("core_user", tenant).Where("id", sid));
            }
            else
            {
                batch.Add(Update("core_usergroup", tenant).Set("removed", true).Set("last_modified", DateTime.UtcNow).Where("userid", sid));
                batch.Add(Update("core_user", tenant).Set("removed", true).Set("last_modified", DateTime.UtcNow).Where("id", sid));
            }
            ExecBatch(batch);
        }

        public void SetUserPhoto(int tenant, Guid id, byte[] photo)
        {
            var sql = photo != null && photo.Length != 0 ?
                (ISqlInstruction)Insert("core_userphoto", tenant).InColumns("userid", "photo").Values(id.ToString(), photo) :
                (ISqlInstruction)Delete("core_userphoto", tenant).Where("userid", id.ToString());

            ExecNonQuery(sql);
        }

        public byte[] GetUserPhoto(int tenant, Guid id)
        {
            var photo = ExecScalar<byte[]>(Query("core_userphoto", tenant).Select("photo").Where("userid", id.ToString()));
            return photo ?? new byte[0];
        }

        public string GetUserPassword(int tenant, Guid id)
        {
            var q = Query("core_usersecurity", tenant).Select("pwdhashsha512").Where("userid", id.ToString());
            var h2 = ExecScalar<string>(q);
            return !string.IsNullOrEmpty(h2) ? Crypto.GetV(h2, 1, false) : null;
        }

        public void SetUserPassword(int tenant, Guid id, string password)
        {
            var h1 = !string.IsNullOrEmpty(password) ? Hasher.Base64Hash(password, HashAlg.SHA256) : null;
            var h2 = !string.IsNullOrEmpty(password) ? Crypto.GetV(password, 1, true) : null;
            var i = Insert("core_usersecurity", tenant)
                .InColumnValue("userid", id.ToString())
                .InColumnValue("pwdhash", h1)
                .InColumnValue("pwdhashsha512", h1);
            ExecNonQuery(i);
        }


        public IEnumerable<Group> GetGroups(int tenant, DateTime from)
        {
            var q = GetGroupQuery(tenant, from);
            return ExecList(q).ConvertAll(r => ToGroup(r));
        }

        public Group GetGroup(int tenant, Guid id)
        {
            var q = GetGroupQuery(tenant, default(DateTime)).Where("id", id);
            return ExecList(q).ConvertAll(r => ToGroup(r)).SingleOrDefault();
        }

        public Group SaveGroup(int tenant, Group group)
        {
            if (group == null) throw new ArgumentNullException("user");
         
            if (group.Id == default(Guid)) group.Id = Guid.NewGuid();
            group.LastModified = DateTime.UtcNow;
            group.Tenant = tenant;

            var i = Insert("core_group", tenant)
                .InColumnValue("id", group.Id.ToString())
                .InColumnValue("name", group.Name)
                .InColumnValue("parentid", group.ParentId.ToString())
                .InColumnValue("categoryid", group.CategoryId.ToString())
                .InColumnValue("removed", group.Removed)
                .InColumnValue("last_modified", group.LastModified);
            ExecNonQuery(i);
            return group;
        }

        public void RemoveGroup(int tenant, Guid id)
        {
            RemoveGroup(tenant, id, false);
        }

        public void RemoveGroup(int tenant, Guid id, bool immediate)
        {
            var batch = new List<ISqlInstruction>();

            var ids = CollectGroupChilds(tenant, id.ToString());

            batch.Add(Delete("core_acl", tenant).Where(Exp.In("subject", ids)));
            batch.Add(Delete("core_subscription", tenant).Where(Exp.In("recipient", ids)));
            batch.Add(Delete("core_subscriptionmethod", tenant).Where(Exp.In("recipient", ids)));
            batch.Add(UserDepartmentToNull(tenant, ids));
            if (immediate)
            {
                batch.Add(Delete("core_usergroup", tenant).Where(Exp.In("groupid", ids)));
                batch.Add(Delete("core_group", tenant).Where(Exp.In("id", ids)));
            }
            else
            {
                batch.Add(Update("core_usergroup", tenant).Set("removed", true).Set("last_modified", DateTime.UtcNow).Where(Exp.In("groupid", ids)));
                batch.Add(Update("core_group", tenant).Set("removed", true).Set("last_modified", DateTime.UtcNow).Where(Exp.In("id", ids)));
            }

            ExecBatch(batch);
        }


        public IEnumerable<UserGroupRef> GetUserGroupRefs(int tenant, DateTime from)
        {
            var q = GetUserGroupRefQuery(tenant, default(DateTime));
            return ExecList(q).ConvertAll(r => ToUserGroupRef(r));
        }

        public UserGroupRef SaveUserGroupRef(int tenant, UserGroupRef r)
        {
            if (r == null) throw new ArgumentNullException("userGroupRef");
            
            r.LastModified = DateTime.UtcNow;
            r.Tenant = tenant;

            var i = Insert("core_usergroup", tenant)
                .InColumnValue("userid", r.UserId.ToString())
                .InColumnValue("groupid", r.GroupId.ToString())
                .InColumnValue("ref_type", (int)r.RefType)
                .InColumnValue("removed", r.Removed)
                .InColumnValue("last_modified", r.LastModified);

            ExecNonQuery(i);

            return r;
        }

        public void RemoveUserGroupRef(int tenant, Guid userId, Guid groupId, UserGroupRefType refType)
        {
            RemoveUserGroupRef(tenant, userId, groupId, refType, false);
        }

        public void RemoveUserGroupRef(int tenant, Guid userId, Guid groupId, UserGroupRefType refType, bool immediate)
        {
            var where = Exp.Eq("userid", userId.ToString()) & Exp.Eq("groupid", groupId.ToString()) & Exp.Eq("ref_type", (int)refType);
            var i = immediate ?
                (ISqlInstruction)Delete("core_usergroup", tenant).Where(where) :
                (ISqlInstruction)Update("core_usergroup", tenant).Where(where).Set("removed", true).Set("last_modified", DateTime.UtcNow);
            ExecNonQuery(i);
        }


        private SqlQuery GetUserQuery()
        {
            return new SqlQuery("core_user u")
                .Select("id", "username", "firstname", "lastname", "sex", "bithdate")
                .Select("status", "title", "department", "workfromdate", "terminateddate")
                .Select("contacts", "email", "location", "notes", "removed", "last_modified", "tenant");
        }

        private SqlQuery GetUserQuery(int tenant, DateTime from)
        {
            var q = GetUserQuery();
            if (tenant != Tenant.DEFAULT_TENANT) q.Where("tenant", tenant);
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
                LastModified = Convert.ToDateTime(r[16]),
                Tenant = Convert.ToInt32(r[17]),
            };
        }


        private SqlQuery GetGroupQuery(int tenant, DateTime from)
        {
            var q = new SqlQuery("core_group").Select("id", "name", "parentid", "categoryid", "removed", "last_modified", "tenant");
            if (tenant != Tenant.DEFAULT_TENANT) q.Where("tenant", tenant);
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
                LastModified = Convert.ToDateTime(r[5]),
                Tenant = Convert.ToInt32(r[6]),
            };
        }

        private List<string> CollectGroupChilds(int tenant, string id)
        {
            var result = new List<string>();
            var childs = ExecList(Query("core_group", tenant).Select("id").Where("parentid", id)).Select(r => (string)r[0]);
            foreach (var child in childs)
            {
                result.Add(child);
                result.AddRange(CollectGroupChilds(tenant, child));
            }
            result.Add(id);
            return result.Distinct().ToList();
        }

        private SqlUpdate UserDepartmentToNull(int tenant, List<string> groups)
        {
            return Update("core_user u", tenant)
                .Set("department", null)
                .Set("last_modified", DateTime.UtcNow)
                .Where(Exp.In("id", Query("core_usergroup r", tenant).Select("userid").Where(Exp.In("groupid", groups))));
        }


        private SqlQuery GetUserGroupRefQuery(int tenant, DateTime from)
        {
            var q = new SqlQuery("core_usergroup").Select("userid", "groupid", "ref_type", "removed", "last_modified");
            if (tenant != Tenant.DEFAULT_TENANT) q.Where("tenant", tenant);
            if (from != default(DateTime)) q.Where(Exp.Ge("last_modified", from));
            return q;
        }

        private UserGroupRef ToUserGroupRef(object[] r)
        {
            return new UserGroupRef(new Guid((string)r[0]), new Guid((string)r[1]), (UserGroupRefType)Convert.ToInt32(r[2]))
            {
                Removed = Convert.ToBoolean(r[3]),
                LastModified = Convert.ToDateTime(r[4]),
                Tenant = Convert.ToInt32(r[5]),
            };
        }
    }
}
