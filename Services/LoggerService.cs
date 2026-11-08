namespace p48_lessons_asp.Services
{
    public class LoggerService
    {
        public List<string> Logs { get; } = new();

        public void Add(string message)
        {
            Logs.Add(message);
            Console.WriteLine($"[LOG]: {message}");
        }
    }
}