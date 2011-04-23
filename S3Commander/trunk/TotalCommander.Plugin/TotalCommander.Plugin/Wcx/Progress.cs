using System.Text;

namespace TotalCommander.Plugin.Wcx
{
    public class Progress
    {
        private readonly Callback callback;


        internal Progress(Callback callback)
        {
            this.callback = callback;
        }


        internal delegate int Callback(string filename, int size);
    }
}
