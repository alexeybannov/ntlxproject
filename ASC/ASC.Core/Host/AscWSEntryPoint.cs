using System;

namespace ASC.Core.Host
{
    public class AscWSEntryPoint : MarshalByRefObject
    {
        public User[] GetUsers()
        {
            return Array.ConvertAll
                (
                    CoreContext.UserManager.GetUsers(),
                    ui => new User(ui)
                );
        }
    }
}
