using System.Text.Json;
using System.Text.Json.Serialization;
using DreckTrack_API.Models.Dto;

namespace DreckTrack_API.AutoMapper.TypeConverter;

public class CollectibleItemDtoConverter : JsonConverter<CollectibleItemDto>
{
    public override CollectibleItemDto Read(ref Utf8JsonReader reader, Type typeToConvert,
        JsonSerializerOptions options)
    {
        using var jsonDoc = JsonDocument.ParseValue(ref reader);
        var root = jsonDoc.RootElement;

        if (!root.TryGetProperty("itemType", out JsonElement itemTypeElement))
        {
            throw new JsonException("Missing ItemType property.");
        }

        // Get the itemType value
        var itemType = itemTypeElement.GetString();

        if (string.IsNullOrWhiteSpace(itemType))
        {
            throw new JsonException("ItemType property is null or empty.");
        }

        var targetType = itemType switch
        {
            "Book" => typeof(BookDto),
            "Movie" => typeof(MovieDto),
            "Show" => typeof(ShowDto),
            "Game" => typeof(GameDto),
            _ => throw new JsonException($"Unknown ItemType: {itemType}")
        };

        var jsonString = root.GetRawText();
        return (CollectibleItemDto?)JsonSerializer.Deserialize(jsonString, targetType, options)
               ?? throw new InvalidOperationException($"Deserialization failed for ItemType: {itemType}");
    }

    public override void Write(Utf8JsonWriter writer, CollectibleItemDto value, JsonSerializerOptions options)
    {
        writer.WriteStartObject();

        // Write the discriminator property
        writer.WriteString("itemType", value.ItemType);

        // Write common properties
        writer.WritePropertyName("id");
        writer.WriteStringValue(value.Id?.ToString());
        writer.WriteString("title", value.Title);
        writer.WriteString("description", value.Description);
        if (value.ReleaseDate.HasValue)
            writer.WriteString("releaseDate", value.ReleaseDate.Value);
        writer.WriteString("language", value.Language);
        writer.WriteString("coverImageUrl", value.CoverImageUrl);
        if (value.AverageRating.HasValue)
            writer.WriteNumber("averageRating", value.AverageRating.Value);
        if (value.RatingsCount.HasValue)
            writer.WriteNumber("ratingsCount", value.RatingsCount.Value);
        writer.WriteStartArray("genres");
        foreach (var genre in value.Genres ?? Enumerable.Empty<string>())
        {
            writer.WriteStringValue(genre);
        }
        writer.WriteEndArray();
        writer.WriteStartArray("tags");
        foreach (var tag in value.Tags ?? Enumerable.Empty<string>())
        {
            writer.WriteStringValue(tag);
        }
        writer.WriteEndArray();
        writer.WriteString("updatedAt", value.UpdatedAt);

        // Serialize ExternalIds
        writer.WriteStartArray("externalIds");
        foreach (var externalId in value.ExternalIds)
        {
            externalId.CollectibleItemId = value.Id ?? Guid.Empty;
            JsonSerializer.Serialize(writer, externalId, options);
        }
        writer.WriteEndArray();

        // Serialize properties specific to the derived type
        switch (value)
        {
            case BookDto book:
                writer.WriteStartArray("authors");
                foreach (var author in book.Authors)
                {
                    writer.WriteStringValue(author);
                }
                writer.WriteEndArray();
                
                writer.WriteString("publisher", book.Publisher);
                if (book.PageCount.HasValue)
                    writer.WriteNumber("pageCount", book.PageCount.Value);
                if (book.CurrentPage.HasValue)
                    writer.WriteNumber("currentPage", book.CurrentPage.Value);
                if (book.Format.HasValue)
                    writer.WriteString("format", book.Format.Value.ToString());
                break;

            case MovieDto movie:
                // writer.WriteString("director", movie.Director);
                writer.WriteNumber("duration", movie.Duration ?? 0);
                break;
            
            case GameDto game:
                writer.WriteString("platform", game.Platform);
                break;

            case ShowDto show:
                writer.WriteStartArray("seasons");
                foreach (var season in show.Seasons)
                {
                    foreach (var episode in season.Episodes)
                    {
                        episode.SeasonId = season.Id;
                    }
                    JsonSerializer.Serialize(writer, season, options);
                }
                writer.WriteEndArray();
                
                writer.WriteStartArray("episodes");
                foreach (var episode in show.Seasons.SelectMany(season => season.Episodes))
                {
                    JsonSerializer.Serialize(writer, episode, options);
                }
                writer.WriteEndArray();
                break;

            case not null:
                break;

            default:
                throw new NotSupportedException($"Type {value.GetType()} is not supported by this converter.");
        }

        writer.WriteEndObject();
    }
}