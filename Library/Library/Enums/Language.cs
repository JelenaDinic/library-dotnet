using System.Text.Json.Serialization;

namespace Library.Enums
{
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum Language
    {
        SERBIAN,
        ENGLISH,
        GERMAN,
        RUSSIAN
    }
}
