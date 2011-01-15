using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Common;
using System.Globalization;
using System.IO;
using System.Linq;
using ASC.Common.Data;
using ASC.Core.Configuration.DAO;
using ASC.Core.Factories;
using ASC.Core.Users;

namespace ASC.Core.Tenants
{
    /// <summary>
    /// 
    /// </summary>
    public class TenantRegistrator
    {
        private ITenantDAO dao;


        /// <summary>
        /// 
        /// </summary>
        /// <param name="connectionString"></param>
        public TenantRegistrator(ConnectionStringSettings connectionString)
        {
            if (!DbRegistry.IsDatabaseRegistered(DAOFactory.DAO_KEY))
            {
                DbRegistry.RegisterDatabase(DAOFactory.DAO_KEY, connectionString);
            }
            dao = new TenantDAO(DAOFactory.DAO_KEY);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="dbProviderFactory"></param>
        /// <param name="connectionString"></param>
        public TenantRegistrator(DbProviderFactory dbProviderFactory, string connectionString)
        {
            if (!DbRegistry.IsDatabaseRegistered(DAOFactory.DAO_KEY))
            {
                DbRegistry.RegisterDatabase(DAOFactory.DAO_KEY, dbProviderFactory, connectionString);
            }
            dao = new TenantDAO(DAOFactory.DAO_KEY);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="daoFactory"></param>
        public TenantRegistrator(IDAOFactory daoFactory)
        {
            dao = daoFactory.GetTenantDAO();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="address"></param>
        public void CheckTenantAddress(string address)
        {
            dao.CheckTenantAddress(address);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="registrationInfo"></param>
        /// <returns></returns>
        public string RegisterTenantInterim(TenantRegistrationInfo registrationInfo)
        {
            ValidateTenantRegistrationInfo(registrationInfo);
            return dao.SaveTenantInterim(registrationInfo);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="tenantInterimKey"></param>
        public string RegisterTenant(string tenantInterimKey)
        {
            return RegisterTenant(tenantInterimKey, null);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="tenantInterimKey"></param>
        /// <param name="templatesPath"></param>
        public string RegisterTenant(string tenantInterimKey, string templatesPath)
        {
            var registrationInfo = dao.GetTenantInterim(tenantInterimKey);
            if (registrationInfo == null) throw new Exception("Registration out of date.");

            return RegisterTenant(registrationInfo, templatesPath);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="registrationInfo"></param>
        /// <param name="templatesPath"></param>
        public string RegisterTenant(TenantRegistrationInfo registrationInfo, string templatesPath)
        {
            ValidateTenantRegistrationInfo(registrationInfo);

            bool doLogout = true;
            try
            {
                SecurityContext.AuthenticateMe(Configuration.Constants.CoreSystem);

                var owner = new TenantOwner(registrationInfo.Email)
                {
                    FirstName = registrationInfo.FirstName,
                    LastName = registrationInfo.LastName,
                };
                CoreContext.Authentication.SetUserPassword(owner.Id, registrationInfo.Password);
                CoreContext.TenantManager.SaveTenantOwner(owner);

                var tenant = new Tenant(registrationInfo.Address.ToLowerInvariant())
                {
                    Name = registrationInfo.Name,
                    Language = registrationInfo.Culture.Name,
                    TimeZone = registrationInfo.TimeZoneInfo,
                    OwnerId = owner.Id,
                };
                tenant.TrustedDomains.AddRange(registrationInfo.TrustedDomains);
                tenant = CoreContext.TenantManager.SaveTenant(tenant);
                CoreContext.TenantManager.SetCurrentTenant(tenant);

                var user = new UserInfo()
                {
                    ID = owner.Id,
                    LastName = registrationInfo.LastName,
                    FirstName = registrationInfo.FirstName,
                    Email = registrationInfo.Email,
                    UserName = registrationInfo.Email.Substring(0, registrationInfo.Email.IndexOf('@'))
                };
                user = CoreContext.UserManager.SaveUserInfo(user);
                CoreContext.Authentication.SetUserPassword(user.ID, registrationInfo.Password);
                CoreContext.UserManager.AddUserIntoGroup(user.ID, Constants.GroupAdmin.ID);

                dao.InitializeTemplateData(tenant.TenantId, user.ID, ReadSqlTemplates(templatesPath, tenant.GetCulture()));

                SecurityContext.Logout();
                doLogout = false;
                return SecurityContext.AuthenticateMe(registrationInfo.Email, registrationInfo.Password);
            }
            finally
            {
                if (doLogout) SecurityContext.Logout();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="registrationInfo"></param>
        public void ValidateTenantRegistrationInfo(TenantRegistrationInfo registrationInfo)
        {
            if (registrationInfo == null) throw new ArgumentNullException("registrationInfo");
            if (string.IsNullOrEmpty(registrationInfo.Name)) throw new Exception("Community name can not be empty");
            if (string.IsNullOrEmpty(registrationInfo.Address)) throw new Exception("Community address can not be empty");

            if (string.IsNullOrEmpty(registrationInfo.Email)) throw new Exception("Account email can not be empty");
            if (string.IsNullOrEmpty(registrationInfo.FirstName)) throw new Exception("Account firstname can not be empty");
            if (string.IsNullOrEmpty(registrationInfo.LastName)) throw new Exception("Account lastname can not be empty");
            if (registrationInfo.Password == null) registrationInfo.Password = GeneratePassword(6);
        }


        private string[] ReadSqlTemplates(string templatesPath, CultureInfo culture)
        {
            if (string.IsNullOrEmpty(templatesPath)) return null;

            var path = Path.Combine(templatesPath, culture.Name);
            if (!Directory.Exists(path)) path = Path.Combine(templatesPath, culture.TwoLetterISOLanguageName);
            if (!Directory.Exists(path)) return null;

            var sql = new List<string>();
            foreach (var file in Directory.GetFiles(path, "*.sql", SearchOption.TopDirectoryOnly))
            {
                sql.AddRange(File.ReadAllLines(file).Where(s => !string.IsNullOrEmpty(s.Trim())));
            }
            return sql.ToArray();
        }

        private string GeneratePassword(int length)
        {
            var noise = "1234567890mnbasdflkjqwerpoiqweyuvcxnzhdkqpsdk";
            var random = new Random();
            var pwd = string.Empty;
            while (0 < length--) pwd += noise[random.Next(noise.Length)];
            return pwd;
        }
    }
}
