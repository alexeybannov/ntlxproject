using System;
using System.Text;
using TotalCommander.Plugin.Wfx.Internal;

namespace TotalCommander.Plugin.Wfx
{
    /// <summary>
    /// <see cref="Request"/> is a class, which the plugin can use to request input from the user.
    /// This class is received through the <see cref="ITotalCommanderWfxPlugin.Init"/> function when the plugin is loaded.
    /// </summary>
	public class Request
	{
		private int pluginNumber;

		private RequestCallback request;

		internal Request(int pluginNumber, RequestCallback request)
		{
			if (request == null) throw new ArgumentNullException("request");

			this.pluginNumber = pluginNumber;
			this.request = request;
        }

        #region UserName

        /// <summary>
        /// Ask for the user name, e.g. for a connection.        /// </summary>
        /// <param name="username">
        /// This string contains the default text presented to the user, and will receive the (modified) text which the user enters.
        /// Set <paramref name="username"/> = null to have no default text.
        /// </param>
        /// <returns>Returns <strong>true</strong> if the user clicked OK or Yes, <strong>false</strong> otherwise.</returns>
        public bool GetUserName(ref string username)
        {
            return GetRequest(RequestType.UserName, null, null, ref username);
        }

        /// <summary>
        /// Ask for the user name, e.g. for a connection.
        /// </summary>
        /// <param name="username">
        /// This string contains the default text presented to the user, and will receive the (modified) text which the user enters.
        /// Set <paramref name="username"/> = null to have no default text.
        /// </param>
        /// <param name="text">Override the default text.</param>
        /// <returns>Returns <strong>true</strong> if the user clicked OK or Yes, <strong>false</strong> otherwise.</returns>
        public bool GetUserName(ref string username, string text)
        {
            return GetRequest(RequestType.UserName, text, null, ref username);
        }

        /// <summary>
        /// Ask for the user name, e.g. for a connection.
        /// </summary>
        /// <param name="username">
        /// This string contains the default text presented to the user, and will receive the (modified) text which the user enters.
        /// Set <paramref name="username"/> = null to have no default text.
        /// </param>
        /// <param name="text">Override the default text.</param>
        /// <param name="title">Custom title for the dialog box.</param>
        /// <returns>Returns <strong>true</strong> if the user clicked OK or Yes, <strong>false</strong> otherwise.</returns>
        public bool GetUserName(ref string username, string text, string title)
        {
            return GetRequest(RequestType.UserName, text, title, ref username);
        }

        #endregion UserName


        #region Password

        /// <summary>
        /// Ask for a password, e.g. for a connection (shows ***).        /// </summary>
        /// <param name="password">
        /// This string contains the default text presented to the user, and will receive the (modified) text which the user enters.
        /// Set <paramref name="password"/> = null to have no default text.
        /// </param>
        /// <returns>Returns <strong>true</strong> if the user clicked OK or Yes, <strong>false</strong> otherwise.</returns>
        public bool GetPassword(ref string password)
        {
            return GetRequest(RequestType.UserName, null, null, ref password);
        }

        /// <summary>
        /// Ask for a password, e.g. for a connection (shows ***).
        /// </summary>
        /// <param name="password">
        /// This string contains the default text presented to the user, and will receive the (modified) text which the user enters.
        /// Set <paramref name="password"/> = null to have no default text.
        /// </param>
        /// <param name="text">Override the default text.</param>
        /// <returns>Returns <strong>true</strong> if the user clicked OK or Yes, <strong>false</strong> otherwise.</returns>
        public bool GetPassword(ref string password, string text)
        {
            return GetRequest(RequestType.UserName, text, null, ref password);
        }

        /// <summary>
        /// Ask for a password, e.g. for a connection (shows ***).
        /// </summary>
        /// <param name="password">
        /// This string contains the default text presented to the user, and will receive the (modified) text which the user enters.
        /// Set <paramref name="password"/> = null to have no default text.
        /// </param>
        /// <param name="text">Override the default text.</param>
        /// <param name="title">Custom title for the dialog box.</param>
        /// <returns>Returns <strong>true</strong> if the user clicked OK or Yes, <strong>false</strong> otherwise.</returns>
        public bool GetPassword(ref string password, string text, string title)
        {
            return GetRequest(RequestType.UserName, text, title, ref password);
        }

        #endregion Password


        private bool GetRequest(RequestType requestType, string text, string title, ref string result)
        {
            var resultBuilder = new StringBuilder(result ?? string.Empty);
            resultBuilder.EnsureCapacity(Win32.MAX_PATH);

            var ok = request(
                pluginNumber,
                (int)requestType,
                !string.IsNullOrEmpty(title) ? title : null,
                !string.IsNullOrEmpty(text) ? text : null,
                resultBuilder,
                Win32.MAX_PATH);

            result = resultBuilder.ToString();

            return ok;
        }
    }
}
