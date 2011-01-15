#region usings

using System;
using ASC.Common.Module;
using ASC.Common.Utils;
using ASC.Net;
using ASC.Reflection;

#endregion

namespace ASC.Common.Services
{
    [Serializable]
    public class ServiceInfoBase : IServiceInfo
    {
        public static readonly Guid DefaultInstanceID = Guid.Empty;

        #region fields

        private readonly string _Description;
        private readonly Guid _ID;
        private readonly ServiceInstancingType _InstancingType;
        private readonly string _Name;
        private readonly string _SysName;
        private readonly TransportType[] _TransportTypes;
        private readonly string _TypeName;
        private readonly string _Uri;
        private readonly Version _Version;
        private IModulePartInfo _ModulePartInfo;
        [NonSerialized] private Type _Type;

        #endregion

        #region constructor

        [Obsolete("This constructor is obsoleted for this type, please use another constructor instead.")]
        public ServiceInfoBase(Type type, Guid id, string name, string description, string sysName, Version version,
                               TransportType[] transportTypes, string uri)
        {
            if (type == null)
                throw new ArgumentNullException("type");
            if (!TypeHelper.ImplementInterface(type, typeof (IService)))
                throw new ServiceDefinitionException(name);
            var sAttr = TypeHelper.GetFirstAttribute<ServiceAttribute>(type);
            if (sAttr == null)
                throw new ServiceDefinitionException(name);
            _Type = type;
            _InstancingType = sAttr.InstancingType;
            _ID = sAttr.ServiceID;

            if (id == Guid.Empty)
                throw new ArgumentOutOfRangeException("id");
            _Name = name;
            _Description = description;
            if (String.IsNullOrEmpty(sysName))
                throw new ArgumentException(CommonDescriptionResource.SysName_ValueInvalid, "sysName");

            _SysName = sysName;
            _ModulePartInfo = null;
            if (version != null)
                _Version = (Version) version.Clone();
            else
                _Version = new Version();
            if (transportTypes == null)
                throw new ArgumentNullException("transportTypes");
            if (transportTypes.Length == 0)
                throw new ArgumentException(CommonDescriptionResource.ServiceInfoBase_TransTypes_ctor_Invalid,
                                            "transportTypes");

            _TransportTypes = (TransportType[]) transportTypes.Clone();
            _Uri = uri;
        }

        public ServiceInfoBase(Type type, string name, string description, string sysName, Version version,
                               TransportType[] transportTypes, string uri)
        {
            if (type == null)
                throw new ArgumentNullException("type");
            if (!TypeHelper.ImplementInterface(type, typeof (IService)))
                throw new ServiceDefinitionException(name);
            var sAttr = TypeHelper.GetFirstAttribute<ServiceAttribute>(type);
            if (sAttr == null)
                throw new ServiceDefinitionException(name);
            if (
                (sAttr.InstancingType == ServiceInstancingType.MultipleInstances ||
                 sAttr.InstancingType == ServiceInstancingType.MultipleFixedInstance)
                &&
                !TypeHelper.ImplementInterface(type, typeof (IInstancedService)))
                throw new InstancingTypeMismatchException(name,
                                                          "service is instanced but not implement IInstancedService");
            _Type = type;
            _TypeName = _Type.AssemblyQualifiedName;
            _InstancingType = sAttr.InstancingType;
            if (sAttr.ServiceID == Guid.Empty)
                throw new ArgumentOutOfRangeException("id");
            _ID = sAttr.ServiceID;

            _Name = name;
            _Description = description;
            SysNameChecker.Check(sysName);
            _SysName = sysName;
            _ModulePartInfo = null;
            if (version != null)
                _Version = (Version) version.Clone();
            else
                _Version = new Version();
            if (transportTypes == null)
                throw new ArgumentNullException("transportTypes");
            if (transportTypes.Length == 0)
                throw new ArgumentException(CommonDescriptionResource.ServiceInfoBase_TransTypes_ctor_Invalid,
                                            "transportTypes");
            _TransportTypes = (TransportType[]) transportTypes.Clone();
            _Uri = uri;
        }

        public ServiceInfoBase(Guid id, ServiceInstancingType instancingType, string typeName, string name,
                               string description, string sysName, Version version, TransportType[] transportTypes,
                               string uri)
        {
            if (typeName == null)
                throw new ArgumentNullException("typeName");

            _ID = id;
            _InstancingType = instancingType;
            _TypeName = typeName;

            _Name = name;
            _Description = description;
            SysNameChecker.Check(sysName);
            _SysName = sysName;
            _ModulePartInfo = null;
            if (version != null)
                _Version = (Version) version.Clone();
            else
                _Version = new Version();
            if (transportTypes == null)
                throw new ArgumentNullException("transportTypes");
            if (transportTypes.Length == 0)
                throw new ArgumentException(CommonDescriptionResource.ServiceInfoBase_TransTypes_ctor_Invalid,
                                            "transportTypes");
            _TransportTypes = (TransportType[]) transportTypes.Clone();
            _Uri = uri;
        }

        #endregion

        #region IServiceInfo Members

        public Guid ID
        {
            get { return _ID; }
        }

        public string Name
        {
            get { return _Name; }
        }

        public string Description
        {
            get { return _Description; }
        }

        public string SysName
        {
            get { return _SysName; }
        }

        public Version Version
        {
            get { return _Version; }
        }

        public string Uri
        {
            get
            {
                if (_ModulePartInfo != null && _ModulePartInfo.ModuleInfo != null)
                    return
                        UriUtil.BuildUri(
                            "",
                            _ModulePartInfo.ModuleInfo.SysName,
                            _ModulePartInfo.ModuleInfo.Version.ToString(2),
                            _ModulePartInfo.SysName,
                            _ModulePartInfo.Version.ToString(2),
                            _Uri
                            );
                else
                    return _Uri;
            }
        }

        public TransportType[] TransportTypes
        {
            get { return _TransportTypes; }
        }

        public string ServiceTypeName
        {
            get { return _TypeName; }
        }

        public Type ServiceType
        {
            get
            {
                if (_Type == null)
                    _Type = Type.GetType(_TypeName, false);
                return _Type;
            }
        }

        public IModulePartInfo ModulePartInfo
        {
            get { return _ModulePartInfo; }
            internal set { _ModulePartInfo = value; }
        }

        public ServiceInstancingType InstancingType
        {
            get { return _InstancingType; }
        }

        #endregion

        #region

        #endregion

        #region

        public override bool Equals(object obj)
        {
            var mpi = obj as IServiceInfo;
            if (mpi == null) return false;
            return Equals(ID, mpi.ID);
        }

        public override int GetHashCode()
        {
            return ID.GetHashCode();
        }

        public override string ToString()
        {
            return String.Format("[{0}]:[{1}]", _ModulePartInfo, Name);
        }

        #endregion
    }
}