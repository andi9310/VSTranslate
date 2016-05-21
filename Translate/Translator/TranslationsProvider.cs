using glosbeClient;

namespace VSTranslate
{
    public static class TranslationsProvider
    {
        public static GlosbeClient Translator { get; } = new GlosbeClient();
    }
}
