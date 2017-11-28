namespace ReverseShellServer
{
    class Program
    {
        static void Main(string[] args)
        {
            var manager = new ServerManager();
            manager.Start("127.0.0.1", 1234, 5000);
        }
    }
}
