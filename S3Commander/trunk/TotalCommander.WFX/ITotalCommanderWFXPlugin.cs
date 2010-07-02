using System;

namespace TotalCommander.WFX
{
    public interface ITotalCommanderWFXPlugin
    {
        //void Init(Int32 PluginNr, IntPtr progressProc, IntPtr logProc, IntPtr requestProc)
        
        object FindFirst(string path, IntPtr findData);
        
        bool FindNext(object findFirstResult, FindData findData);

        void FindClose(object findFirstResult);


        void SetDefaultParams(DefaultParam defaultParam);

        string GetDefRootName();
    }
}
