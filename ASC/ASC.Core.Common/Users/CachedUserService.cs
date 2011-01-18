using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Caching;

namespace ASC.Core
{
    public class CachedUserService
    {
        private DbUserService userService;
        private Cache cache;

        private TimeSpan period = TimeSpan.FromSeconds(5);
        private DateTime lastUsersUpdate;


        public CachedUserService()
        {
            this.userService = new DbUserService(null);
            cache = HttpRuntime.Cache;
        }


        public IEnumerable<User> GetUsers(int tenant)
        {
            lock (cache)
            {
                var key = "u" + tenant.ToString();
                var users = cache.Get(key) as List<User>;

                if (users == null || period < (lastUsersUpdate - DateTime.UtcNow).Duration())
                {
                    if (users == null) users = new List<User>();
                    foreach (var u in userService.GetUsers(tenant, lastUsersUpdate))
                    {
                        users.Remove(u);
                        if (!u.Removed) users.Add(u);
                    }
                    lastUsersUpdate = DateTime.UtcNow;
                    cache.Insert(key, users, null, Cache.NoAbsoluteExpiration, TimeSpan.FromMinutes(10), CacheItemPriority.Default, null);
                }

                return users;
            }
        }

        public User SaveUser(int tenant, User user)
        {
            throw new NotImplementedException();
        }

        public void RemoveUser(int tenant, Guid id)
        {
            throw new NotImplementedException();
        }

        public byte[] GetUserPhoto(int tenant, Guid id)
        {
            throw new NotImplementedException();
        }

        public void SetUserPhoto(int tenant, Guid id, byte[] photo)
        {
            throw new NotImplementedException();
        }


        public IEnumerable<Group> GetGroups(int tenant, DateTime from)
        {
            throw new NotImplementedException();
        }

        public Group SaveGroup(int tenant, Group group)
        {
            throw new NotImplementedException();
        }

        public void RemoveGroup(int tenant, Guid id)
        {
            throw new NotImplementedException();
        }


        public IEnumerable<UserGroupRef> GetUserGroupRefs(int tenant, DateTime from)
        {
            throw new NotImplementedException();
        }

        public UserGroupRef SaveUserGroupRef(int tenant, UserGroupRef r)
        {
            throw new NotImplementedException();
        }

        public void RemoveUserGroupRef(int tenant, Guid userId, Guid groupId, UserGroupRefType refType)
        {
            throw new NotImplementedException();
        }
    }
}
