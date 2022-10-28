namespace SmartWeightApp.Models
{
    internal class AlertException : Exception
    {
        public string Title { get; set; }

        public AlertException(string title, string message) : base(message)
        {
            Title = title;
        }
    }
}
