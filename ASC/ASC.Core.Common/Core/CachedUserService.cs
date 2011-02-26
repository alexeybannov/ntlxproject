using System;
using System.Collections.Generic;
using System.Linq;

namespace ASC.Core
{
    public class CachedUserService : IUserService
    {
        private IUserService service;
        private CacheHelper cache;


        public CachedUserService(IUserService service)
        {
            this.service = service;
            this.cache = new CacheHelper();
        }


        public IEnumerable<User> GetUsers(int tenant, DateTime from)
        {
            lock (cache)
            {
                return cache.Get<User>(
                    tenant,
                    from,
                    last => service.GetUsers(tenant, last).Select(u => new CacheItem<User>(u, u.Removed, u.ModifiedOn)));
            }
        }

        public User GetUser(int tenant, Guid id)
        {
            return GetUsers(tenant, default(DateTime)).SingleOrDefault(u => u.Id == id);
        }

        public User GetUser(int tenant, string login, string password)
        {
            return service.GetUser(tenant, login, password);
        }

        public User SaveUser(int tenant, User user)
        {
            user = service.SaveUser(tenant, user);
            lock (cache)
            {
                cache.Reset<User>(tenant);
                cache.Reset<UserGroupRef>(tenant);
            }
            return user;
        }

        public void RemoveUser(int tenant, Guid id)
        {
            service.RemoveUser(tenant, id);
            lock (cache)
            {
                cache.Reset<User>(tenant);
                cache.Reset<UserGroupRef>(tenant);
            }
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
            lock (cache)
            {
                return cache.Get<Group>(
                    tenant,
                    from,
                    last => service.GetGroups(tenant, last).Select(g => new CacheItem<Group>(g, g.Removed, g.ModifiedOn)));
            }
        }

        public Group GetGroup(int tenant, Guid id)
        {
            return GetGroups(tenant, default(DateTime)).FirstOrDefault(g => g.Id == id);
        }

        public Group SaveGroup(int tenant, Group group)
        {
            group = service.SaveGroup(tenant, group);
            lock (cache)
            {
                cache.Reset<Group>(tenant);
                cache.Reset<UserGroupRef>(tenant);
            }
            return group;
        }

        public void RemoveGroup(int tenant, Guid id)
        {
            service.RemoveGroup(tenant, id);
            lock (cache)
            {
                cache.Reset<Group>(tenant);
                cache.Reset<UserGroupRef>(tenant);
            }
        }


        public IEnumerable<UserGroupRef> GetUserGroupRefs(int tenant, DateTime from)
        {
            lock (cache)
            {
                return cache.Get<UserGroupRef>(
                    tenant,
                    from,
                    last => service.GetUserGroupRefs(tenant, last).Select(r => new CacheItem<UserGroupRef>(r, r.Removed, r.ModifiedOn)));
            }
        }

        public UserGroupRef SaveUserGroupRef(int tenant, UserGroupRef r)
        {
            r = service.SaveUserGroupRef(tenant, r);
            cache.Reset<UserGroupRef>(tenant);
            return r;
        }

        public void RemoveUserGroupRef(int tenant, Guid userId, Guid groupId, UserGroupRefType refType)
        {
            service.RemoveUserGroupRef(tenant, userId, groupId, refType);
            cache.Reset<UserGroupRef>(tenant);
        }
    }
}
