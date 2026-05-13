using System.Text.Json.Serialization;


namespace MainService.Enums
{

    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum SortOrder
    {
        Ascending,
        Descending
    }
}
