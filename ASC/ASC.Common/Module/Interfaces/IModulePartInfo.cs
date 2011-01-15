namespace ASC.Common.Module
{
    public interface IModulePartInfo : IBaseInfo
    {
        IModuleInfo ModuleInfo { get; }
        ModulePartType ModulePartType { get; }
    }
}