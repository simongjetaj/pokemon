namespace Pokemon.Data.Models
{
    public class TranslationType
    {
        private TranslationType(string value) { Value = value; }

        public string Value { get; private set; }

        public static TranslationType Shakespeare { get { return new TranslationType("shakespeare.json"); } }
        public static TranslationType Yoda { get { return new TranslationType("yoda.json"); } }
    }
}
