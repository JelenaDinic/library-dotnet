using System.ComponentModel;
using System.Text.Json.Serialization;

namespace Library.Enums
{
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum UserRole
    {
        [Description("USER")]
        USER = 1,
        [Description("ADMIN")]
        ADMIN = 2,
        [Description("LIBRARIAN")]
        LIBRARIAN = 3
    }
}
