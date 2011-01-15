#region usings

using System;

#endregion

namespace ASC.Common.Services.Factories
{
    public interface IServiceFactory
    {
        IService GetService(Type type);
    }
}