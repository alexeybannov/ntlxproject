using System.Text;

namespace TotalCommander.Plugin.Wfx
{
    public class Password
    {
        private Password.Callback callback;
        private int pluginNumber;
        private int cryptoNumber;

        /// <summary>
        /// It is set when the user has defined a master password.
        /// </summary>
        public bool MasterPasswordDefined
        {
            get;
            private set;
        }


        internal Password(Password.Callback callback, int pluginNumber, int cryptoNumber, int flags)
        {
            this.callback = callback;
            this.pluginNumber = pluginNumber;
            this.cryptoNumber = cryptoNumber;
            this.MasterPasswordDefined = flags == 1;
        }


        public FileOperationResult Save(string connection, string password)
        {
            return Crypt(CryptMode.SavePassword, connection, ref password);
        }

        public FileOperationResult Load(string connection, ref string password)
        {
            return Load(connection, false, ref password);
        }

        public FileOperationResult Load(string connection, bool noUI, ref string password)
        {
            return Crypt(noUI ? CryptMode.LoadPasswordNoUI : CryptMode.LoadPassword, connection, ref password);
        }

        public FileOperationResult Copy(string sourceConnection, string targetConnection)
        {
            return Crypt(CryptMode.CopyPassword, sourceConnection, ref targetConnection);
        }

        public FileOperationResult Move(string sourceConnection, string targetConnection)
        {
            return Crypt(CryptMode.MovePassword, sourceConnection, ref targetConnection);
        }

        public FileOperationResult Delete(string connection)
        {
            var password = string.Empty;
            return Crypt(CryptMode.DeletePassword, connection, ref password);
        }


        private FileOperationResult Crypt(CryptMode mode, string connectionName, ref string password)
        {
            var passwordBuilder = new StringBuilder(password ?? string.Empty);
            passwordBuilder.EnsureCapacity(Win32.MAX_PATH);

            var result = callback(
                pluginNumber,
                cryptoNumber,
                (int)mode,
                !string.IsNullOrEmpty(connectionName) ? connectionName : null,
                passwordBuilder,
                Win32.MAX_PATH);

            password = passwordBuilder.ToString();

            return (FileOperationResult)result;
        }


        private enum CryptMode
        {
            SavePassword = 1,
            LoadPassword,
            LoadPasswordNoUI,
            CopyPassword,
            MovePassword,
            DeletePassword
        }

        internal delegate int Callback(int pluginNumber, int cryptoNumber, int mode, string connectionName, StringBuilder password, int maxLen);
    }
}
