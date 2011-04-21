using System;
using System.Collections.Generic;
using System.IO;

namespace TotalCommander.Plugin.Wcx
{
    public abstract class TotalCommanderWcxPlugin : ITotalCommanderWcxPlugin
    {
        private readonly IDictionary<IntPtr, IArchiveUnpacker> unpackers = new Dictionary<IntPtr, IArchiveUnpacker>();


        /// <summary>
        /// Plugin interface version.
        /// </summary>
        public Version PluginInterfaceVersion
        {
            get;
            private set;
        }

        /// <summary>
        /// Suggested location+name of the ini file where the plugin could store its data.
        /// </summary>
        public string PluginIniFile
        {
            get;
            private set;
        }

        public virtual BackgroundFlags BackgroundSupport
        {
            get { return BackgroundFlags.NotSupported; }
        }

        public virtual PackerCapabilities PackerCapabilities
        {
            get { return PackerCapabilities.None; }
        }


        public virtual bool CanYouHandleThisFile(string fileName)
        {
            return false;
        }


        public abstract IArchiveUnpacker Unpack(string archiveName, OpenArchiveMode mode);


        void ITotalCommanderWcxPlugin.SetDefaultParams(DefaultParam dp)
        {
            PluginInterfaceVersion = dp.PluginInterfaceVersion;
            PluginIniFile = dp.DefaultIniFileName;
        }

        BackgroundFlags ITotalCommanderWcxPlugin.GetBackgroundFlags()
        {
            return BackgroundSupport;
        }

        PackerCapabilities ITotalCommanderWcxPlugin.GetPackerCapabilities()
        {
            return PackerCapabilities;
        }

        ArchiveResult ITotalCommanderWcxPlugin.OpenArchive(string archiveName, OpenArchiveMode mode, out IntPtr archive)
        {
            archive = IntPtr.Zero;
            try
            {
                var unpacker = Unpack(archiveName, mode);
                if (unpacker == null) return ArchiveResult.Default;

                lock (unpackers)
                {
                    unpackers[archive = (IntPtr)(unpackers.Count + 1)] = unpacker;
                    unpacker.Reset();
                }
                return ArchiveResult.Success;
            }
            catch (WcxException error)
            {
                return error.ArchiveResult;
            }
        }

        ArchiveResult ITotalCommanderWcxPlugin.ReadHeader(IntPtr archive, out ArchiveHeader header)
        {
            header = null;
            try
            {
                lock (unpackers)
                {
                    IArchiveUnpacker unpacker;
                    if (unpackers.TryGetValue(archive, out unpacker))
                    {
                        if (unpacker.MoveNext())
                        {
                            header = unpacker.Current;
                            return ArchiveResult.Success;
                        }
                        else
                        {
                            return ArchiveResult.EndArchive;
                        }
                    }
                }
                return ArchiveResult.Default;
            }
            catch (WcxException error)
            {
                return error.ArchiveResult;
            }
        }

        ArchiveResult ITotalCommanderWcxPlugin.ProcessFile(IntPtr archive, ArchiveProcess operation, string filepath, string filename)
        {
            lock (unpackers)
            {
                IArchiveUnpacker unpacker;
                if (unpackers.TryGetValue(archive, out unpacker))
                {
                    var path = !string.IsNullOrEmpty(filename) ? Path.Combine(filepath, filename) : filename;
                    unpacker.UnpackFile(unpacker.Current, path, operation);
                    return ArchiveResult.Success;
                }
            }
            return ArchiveResult.Default;
        }

        ArchiveResult ITotalCommanderWcxPlugin.CloseArchive(IntPtr archive)
        {
            lock (unpackers)
            {
                IArchiveUnpacker unpacker;
                if (unpackers.TryGetValue(archive, out unpacker))
                {
                    unpackers.Remove(archive);
                    unpacker.Dispose();
                    return ArchiveResult.Success;
                }
            }
            return ArchiveResult.Default;
        }
    }
}
