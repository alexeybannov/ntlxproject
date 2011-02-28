using System;
using System.IO;
using System.Reflection;
using System.Runtime.Remoting;
using ASC.Common.Module;
using log4net.Config;

namespace ASC.Core.Host
{
    class ServiceInitializer : MarshalByRefObject
    {
        public ObjRef InitializeService(Guid partId, string servicePath, out Exception error)
        {
            error = null;
            try
            {
                LoadAssemblies(servicePath);

                AppDomain.CurrentDomain.SetData("DataDirectory", servicePath);
                XmlConfigurator.Configure();

                foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
                {
                    var servicePart = FindModuleServicesPart(assembly, partId);
                    if (servicePart != null)
                    {
                        return RemotingServices.Marshal((MarshalByRefObject)servicePart.ServiceHost);
                    }
                }
                error = new Exception(string.Format("Service of the module with Id = {0} not found.", partId));
            }
            catch (Exception ex)
            {
                error = ex;
            }
            return null;
        }


        private void LoadAssemblies(string srvDir)
        {
            foreach (var file in Directory.GetFiles(srvDir))
            {
                try
                {
                    var name = AssemblyName.GetAssemblyName(file);
                    if (!Array.Exists(AppDomain.CurrentDomain.GetAssemblies(), a => string.Compare(a.GetName().Name, name.Name, true) == 0))
                    {
                        Assembly.Load(name);
                    }
                }
                catch (BadImageFormatException) { }
                catch (FileLoadException) { }
            }
        }

        private static IModuleServicesPart FindModuleServicesPart(Assembly assembly, Guid servicePartId)
        {
            var attribute = Attribute.GetCustomAttribute(assembly, typeof(ModuleInfoAttribute), false) as ModuleInfoAttribute;
            if (attribute != null)
            {
                var module = (IModule)assembly.CreateInstance(attribute.ModuleType.FullName);
                foreach (var part in module.Parts)
                {
                    if (part.Info.ID == servicePartId) return part as IModuleServicesPart;
                }
            }
            return null;
        }
    }
}
