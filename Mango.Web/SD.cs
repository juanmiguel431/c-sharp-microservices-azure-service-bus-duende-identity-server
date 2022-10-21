namespace Mango.Web
{
    public static class SD
    {
        public static string ProductApiBase { get; set; }
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
