using AutoMapper;
using DreckTrack_API.Models.Dto;
using DreckTrack_API.Models.Dto.Items;
using DreckTrack_API.Models.Entities.Collectibles;
using DreckTrack_API.Models.Entities.Collectibles.Items;
using DreckTrack_API.Models.Entities.Collectibles.Items.Book;
using DreckTrack_API.Models.Entities.Collectibles.Items.Game;
using DreckTrack_API.Models.Entities.Collectibles.Items.Movie;
using DreckTrack_API.Models.Entities.Collectibles.Items.Show;

namespace DreckTrack_API.AutoMapper.TypeConverter;

// Cannot be abstract because AutoMapper will not be able to create an instance of it
public class AddUserCollectibleItemDtoToUserCollectibleItemConverter : ITypeConverter<AddUserCollectibleItemDto, UserCollectibleItem>
{
    public UserCollectibleItem Convert(AddUserCollectibleItemDto source, UserCollectibleItem destination, ResolutionContext context)
    {
        destination = new UserCollectibleItem
        {
            Status = source.Status,
            UserRating = source.UserRating,
            Notes = source.Notes,
            DateStarted = source.DateStarted,
            DateCompleted = source.DateCompleted,
            CollectibleItem = MapCollectibleItem(source.CollectibleItem, context),
            // If CollectibleItemDto.Id is invalid or null, generate a new CollectibleItemId
            CollectibleItemId = source.CollectibleItem?.Id == Guid.Empty ? Guid.NewGuid() : source.CollectibleItem?.Id ?? Guid.NewGuid()
        };

        return destination;
    }

    private static CollectibleItem MapCollectibleItem(CollectibleItemDto dto, ResolutionContext context)
    {
        if (dto == null || string.IsNullOrEmpty(dto.ItemType))
        {
            throw new InvalidOperationException("Unknown or null CollectibleItemDto type");
        }

        return dto.ItemType.ToLower() switch
        {
            "book" => context.Mapper.Map<Book>(dto),
            "movie" => context.Mapper.Map<Movie>(dto),
            "show" => context.Mapper.Map<Show>(dto),
            "game" => context.Mapper.Map<Game>(dto),
            _ => throw new InvalidOperationException($"Unknown CollectibleItemDto type: {dto.ItemType}")
        };
    }
}

