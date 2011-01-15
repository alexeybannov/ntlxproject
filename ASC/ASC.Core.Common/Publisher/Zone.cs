namespace ASC.Core.Common.Publisher
{
    public class Zone
    {
        public Zone()
        {
        }

        public Zone(string id)
        {
            ID = id;
        }

        public Zone(string id, int width)
        {
            ID = id;
            Width = width;
        }

        public Zone(string id, int? width, int? height)
        {
            ID = id;
            Width = width;
            Height = height;
        }

        public string ID;

        public int? Width;

        public int? Height;
    }
}