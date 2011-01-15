namespace ASC.Common.Security.Authorizing
{
    public interface IAuthCategoriesProvider
    {
        AuthCategory[] GetAuthCategories();
    }
}