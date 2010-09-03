using System;

namespace TotalCommander.Plugin.Wfx
{
    /// <summary>
    /// Defines constants that are used by the <see cref="ITotalCommanderWfxPlugin.GetCustomIcon"/>
    /// </summary>
    /// <seealso cref="ITotalCommanderWfxPlugin.GetCustomIcon"/>
    [Flags]
    public enum CustomIconFlag
	{
        /// <summary>
        /// Requests the large 48x48 icon.
        /// </summary>
        Large = 0,

        /// <summary>
        /// Requests the small 16x16 icon.
        /// </summary>
        Small = 1,

        /// <summary>
        /// The function is called from the background thread.
        /// </summary>
        Background = 2
	}
}
