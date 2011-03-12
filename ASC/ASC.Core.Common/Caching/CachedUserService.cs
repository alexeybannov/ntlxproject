using System;
using System.Collections.Generic;
using System.Linq;
using ASC.Core.Caching;
using ASC.Core.Tenants;

namespace ASC.Core.Caching
{
    public class CachedUserService : IUserService
    {
        private const string USERS = "users/";
        private const string GROUPS = "groups/";
        private const string REFS = "refs/";

        private readonly IUserService service;
        private readonly ICache cache;
        private DateTime lastDbAccess;
        private volatile bool cacheValid;


        public TimeSpan CacheExpiration
        {
            get;
            set;
        }

        public TimeSpan DbExpiration
        {
            get;
            set;
        }


        public CachedUserService(IUserService service)
        {
            if (service == null) throw new ArgumentNullException("service");

            this.service = service;
            this.cache = new AspCache();

            CacheExpiration = TimeSpan.FromHours(1);
            DbExpiration = TimeSpan.FromSeconds(15);
            lastDbAccess = DateTime.UtcNow;
            cacheValid = true;
        }


        public IEnumerable<User> GetUsers(int tenant, DateTime from)
        {
            return GetUsers(tenant).Values;
        }

        public User GetUser(int tenant, Guid id)
        {
            User u;
            GetUsers(tenant).TryGetValue(id, out u);
            return u;
        }

        public User GetUser(int tenant, string login, string password)
        {
            return service.GetUser(tenant, login, password);
        }

        public User SaveUser(int tenant, User user)
        {
            user = service.SaveUser(tenant, user);
            InvalidateCache();
            return user;
        }

        public void RemoveUser(int tenant, Guid id)
        {
            service.RemoveUser(tenant, id);
            InvalidateCache();
        }

        public byte[] GetUserPhoto(int tenant, Guid id)
        {
            return service.GetUserPhoto(tenant, id);
        }

        public void SetUserPhoto(int tenant, Guid id, byte[] photo)
        {
            service.SetUserPhoto(tenant, id, photo);
        }

        public string GetUserPassword(int tenant, Guid id)
        {
            return service.GetUserPassword(tenant, id);
        }

        public void SetUserPassword(int tenant, Guid id, string password)
        {
            service.SetUserPassword(tenant, id, password);
        }


        public IEnumerable<Group> GetGroups(int tenant, DateTime from)
        {
            return GetGroups(tenant).Values;
        }

        public Group GetGroup(int tenant, Guid id)
        {
            Group g;
            GetGroups(tenant).TryGetValue(id, out g);
            return g;
        }

        public Group SaveGroup(int tenant, Group group)
        {
            group = service.SaveGroup(tenant, group);
            InvalidateCache();
            return group;
        }

        public void RemoveGroup(int tenant, Guid id)
        {
            service.RemoveGroup(tenant, id);
            InvalidateCache();
        }


        public IEnumerable<UserGroupRef> GetUserGroupRefs(int tenant, DateTime from)
        {
            GetChangesFromDb();

            var key = REFS + tenant.ToString();
            var refs = cache.Get(key) as IEnumerable<UserGroupRef>;
            if (refs == null)
            {
                refs = service.GetUserGroupRefs(tenant, default(DateTime)).ToList();
                cache.Insert(key, refs, CacheExpiration);
            }
            return refs;
        }

        public UserGroupRef SaveUserGroupRef(int tenant, UserGroupRef r)
        {
            r = service.SaveUserGroupRef(tenant, r);
            InvalidateCache();
            return r;
        }

        public void RemoveUserGroupRef(int tenant, Guid userId, Guid groupId, UserGroupRefType refType)
        {
            service.RemoveUserGroupRef(tenant, userId, groupId, refType);
            InvalidateCache();
        }


        private IDictionary<Guid, User> GetUsers(int tenant)
        {
            GetChangesFromDb();

            var key = USERS + tenant.ToString();
            var users = cache.Get(key) as IDictionary<Guid, User>;
            if (users == null)
            {
                users = service.GetUsers(tenant, default(DateTime)).ToDictionary(u => u.Id);
                cache.Insert(key, users, CacheExpiration);
            }
            return users;
        }

        private IDictionary<Guid, Group> GetGroups(int tenant)
        {
            GetChangesFromDb();

            var key = GROUPS + tenant.ToString();
            var groups = cache.Get(key) as IDictionary<Guid, Group>;
            if (groups == null)
            {
                groups = service.GetGroups(tenant, default(DateTime)).ToDictionary(g => g.Id);
                cache.Insert(key, groups, CacheExpiration);
            }
            return groups;
        }

        private void GetChangesFromDb()
        {
            if (cacheValid && (lastDbAccess - DateTime.UtcNow).Duration() <= DbExpiration) return;

            lock (cache)
            {
                if (cacheValid && (lastDbAccess - DateTime.UtcNow).Duration() <= DbExpiration) return;

                foreach (var tenantGroup in service.GetUsers(Tenant.DEFAULT_TENANT, lastDbAccess).GroupBy(u => u.Tenant))
                {
                    var users = cache.Get(USERS + tenantGroup.Key) as IDictionary<Guid, User>;
                    if (users != null)
                    {
                        foreach (var u in tenantGroup)
                        {
                            if (u.Removed) users.Remove(u.Id);
                            else users[u.Id] = u;
                        }
                    }
                }

                foreach (var tenantGroup in service.GetGroups(Tenant.DEFAULT_TENANT, lastDbAccess).GroupBy(g => g.Tenant))
                {
                    var groups = cache.Get(GROUPS + tenantGroup.Key) as IDictionary<Guid, Group>;
                    if (groups != null)
                    {
                        foreach (var g in tenantGroup)
                        {
                            if (g.Removed) groups.Remove(g.Id);
                            else groups[g.Id] = g;
                        }
                    }
                }

                foreach (var tenantGroup in service.GetUserGroupRefs(Tenant.DEFAULT_TENANT, lastDbAccess).GroupBy(r => r.Tenant))
                {
                    var refs = cache.Get(REFS + tenantGroup.Key) as List<UserGroupRef>;
                    if (refs != null)
                    {
                        foreach (var r in tenantGroup)
                        {
                            refs.Remove(r);
                            if (!r.Removed) refs.Add(r);
                        }
                    }
                }

                cacheValid = true;
                lastDbAccess = DateTime.UtcNow;
            }
        }

        private void InvalidateCache()
        {
            cacheValid = false;
        }
    }
}
