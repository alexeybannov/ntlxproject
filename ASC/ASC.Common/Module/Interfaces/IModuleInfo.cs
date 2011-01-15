namespace ASC.Common.Module
{
    public interface IModuleInfo : IBaseInfo
    {
        ModuleType Type { get; }
        IModulePartInfo[] ModulePartsInfo { get; }
    }
}