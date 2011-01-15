namespace ASC.Common.Security.Authorizing
{
    public static class AzObjectIdHelper
    {
        private static readonly string separator = "|";

        public static string GetFullObjectId(ISecurityObjectId objectId)
        {
            if (objectId == null) return null;
            return string.Format("{0}{1}{2}", objectId.ObjectType.FullName, separator, objectId.SecurityId);
        }
    }
}