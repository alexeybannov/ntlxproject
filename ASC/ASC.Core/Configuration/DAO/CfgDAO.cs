using System;
using System.Collections.Generic;
using ASC.Common.Data;
using ASC.Common.Data.Sql;
using ASC.Common.Data.Sql.Expressions;
using ASC.Common.Security.Authentication;
using ASC.Core.Security.Authentication;
using ASC.Core.Users;
using ASC.Core.Users.DAO;

namespace ASC.Core.Configuration.DAO
{
    class CfgDAO : DAOBase, ICfgDAO
    {
        public CfgDAO(string dbId)
            : base(dbId)
        {

        }

        #region IConfigDao

        public void SaveSettings(string key, byte[] data)
        {
            if (data == null || data.Length == 0)
            {
                DbManager.ExecuteNonQuery(Delete("core_settings").Where("Id", key));
            }
            else
            {
                DbManager.ExecuteNonQuery(Insert("core_settings").InColumns("Id", "Value").Values(key, data));
            }
        }

        public byte[] GetSettings(string key)
        {
            return DbManager.ExecuteScalar<byte[]>(Query("core_settings").Select("Value").Where("Id", key));
        }

        public void SaveUserSecurity(UserSecurity s)
        {
            DbManager.ExecuteNonQuery(
                Insert("core_usersecurity").InColumns("UserID", "PwdHash", "PwdHashSHA512").Values(s.UserID, s.PasswordHash, s.PasswordHashSHA512)
            );
        }

        public UserSecurity GetUserSecurity(Guid userID)
        {
            var list = DbManager
                .ExecuteList(Query("core_usersecurity").Select("UserID", "PwdHash", "PwdHashSHA512").Where("UserID", userID))
                .ConvertAll<UserSecurity>(r =>
                {
                    return new UserSecurity(new Guid(Convert.ToString(r[0])))
                    {
                        PasswordHash = Convert.ToString(r[1]),
                        PasswordHashSHA512 = Convert.ToString(r[2]),
                    };
                }
                );
            return list.Count != 0 ? list[0] : null;
        }

        public IUserAccount GetAccount(Credential credential)
        {
            if (credential == null) throw new ArgumentNullException("credential");

            var rows = DbManager.ExecuteList(GetQueryFindAccount(credential).Where("Upper(u.Email)", credential.Login.ToUpperInvariant()));
            if (rows.Count == 0)
            {
                rows = DbManager.ExecuteList(GetQueryFindAccount(credential).Where("u.ID", credential.Login));
                if (rows.Count == 0) return null;
            }

            return ToAccount(rows[0], credential.Tenant);
        }

        public List<IUserAccount> GetAccounts(int tenant)
        {
            var query = new SqlQuery("core_user u")
                .Select("u.ID", "u.LastName", "u.FirstName", "u.Title", "u.Department", "u.Email")
                .Select(new SqlQuery("core_usergroup g").Select("1").Where(Exp.EqColumns("u.ID", "g.UserID") & Exp.Eq("g.GroupID", ASC.Core.Users.Constants.GroupAdmin.ID)))
                .Where("u.Tenant", tenant)
                .Where("u.Status", EmployeeStatus.Active);

            return DbManager
                .ExecuteList(query)
                .ConvertAll(row => ToAccount(row, tenant));
        }

        public IEnumerable<Guid> GetAccountRoles(Guid accountId)
        {
            return DbManager
                .ExecuteList(new SqlQuery("core_usergroup").Select("GroupId").Where("UserId", accountId))
                .ConvertAll(r => new Guid((string)r[0]));
        }


        private SqlQuery GetQueryFindAccount(Credential credential)
        {
            //select u.ID, u.LastName, u.FirstName, u.Title, u.Department, u.Email
            //from core_user u, core_usersecurity s where u.ID = s.UserID and u.ID = ? and s.PwdHash = ? and u.Tenant = ?
            var query = new SqlQuery()
                .From("core_user u")
                .From("core_usersecurity s")
                .Select("u.ID", "u.LastName", "u.FirstName", "u.Title", "u.Department", "u.Email")
                .Where(Exp.EqColumns("u.ID", "s.UserID") & Exp.Eq("s.PwdHash", credential.PasswordHash))
                .Where("u.Status", EmployeeStatus.Active);
            if (credential.Tenant != -1) query.Where("u.Tenant", credential.Tenant);

            return query;
        }

        private IUserAccount ToAccount(object[] r, int tenant)
        {
            var userInfo = new UserInfo()
            {
                ID = new Guid((string)r[0]),
                LastName = (string)r[1],
                FirstName = (string)r[2],
                Title = (string)r[3],
                Department = (string)r[4],
                Email = (string)r[5]
            };
            return new UserAccount(userInfo, tenant);
        }

        #endregion
    }
}
