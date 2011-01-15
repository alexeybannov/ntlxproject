#region usings

using ASC.Common.Security.Authorizing;

#endregion

namespace ASC.Common.Module
{
    public interface IModule
    {
        IModuleInfo Info { get; }
        IModulePart[] Parts { get; }
        AuthCategory[] AuthorizingCategories { get; }
    }
}