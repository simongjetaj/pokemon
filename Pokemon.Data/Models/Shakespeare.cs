namespace Pokemon.Data.Models
{
    public class Translation
    {
        public Success Success { get; set; }
        public Contents Contents { get; set; }
    }

    public class Success
    {
        public int Total { get; set; }
    }

    public class Contents
    {
        public string Translated { get; set; }
        public string Text { get; set; }
        public string Translation { get; set; }
    }
}
