namespace Cedar.HttpCommandHandling
{
    public class HttpProblemDetailsDto
    {
        public int Status { get; set; }

        public string Type { get; set; }

        public string Detail { get; set; }

        public string Title { get; set; }

        public string Instance { get; set; }
    }
}