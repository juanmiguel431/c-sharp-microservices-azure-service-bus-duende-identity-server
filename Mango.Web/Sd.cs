namespace Mango.Web
{
    public static class Sd
    {
        public static string ProductApiBase { get; set; }
        public static string ShoppingCartApiBase { get; set; }
        
        public const string Admin = "Admin";
        public const string Customer = "Customer";
        
        public enum ApiType
        {
            Get,
            Post,
            Put,
            Patch,
            Delete
        }
    }
}
