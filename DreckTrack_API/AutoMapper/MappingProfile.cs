using AutoMapper;
using DreckTrack_API.AutoMapper.TypeConverter;
using DreckTrack_API.Models.Dto;
using DreckTrack_API.Models.Dto.Auth;
using DreckTrack_API.Models.Dto.Information;
using DreckTrack_API.Models.Dto.Items;
using DreckTrack_API.Models.Dto.Items.Book;
using DreckTrack_API.Models.Dto.Items.Game;
using DreckTrack_API.Models.Dto.Items.Movie;
using DreckTrack_API.Models.Dto.Items.Show;
using DreckTrack_API.Models.Entities;
using DreckTrack_API.Models.Entities.Collectibles;
using DreckTrack_API.Models.Entities.Collectibles.Information;
using DreckTrack_API.Models.Entities.Collectibles.Items;
using DreckTrack_API.Models.Entities.Collectibles.Items.Book;
using DreckTrack_API.Models.Entities.Collectibles.Items.Game;
using DreckTrack_API.Models.Entities.Collectibles.Items.Movie;
using DreckTrack_API.Models.Entities.Collectibles.Items.Show;

namespace DreckTrack_API.AutoMapper;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        // Base mapping
        CreateMap<CollectibleItemDto, CollectibleItem>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .Include<BookDto, Book>()
            .Include<MovieDto, Movie>()
            .Include<ShowDto, Show>()
            .Include<GameDto, Game>();
        CreateMap<CollectibleItem, CollectibleItemDto>()
            .Include<Book, BookDto>()
            .Include<Movie, MovieDto>()
            .Include<Show, ShowDto>()
            .Include<Game, GameDto>();

        // Derived mappings
        CreateMap<BookDto, Book>().ReverseMap();
        CreateMap<MovieDto, Movie>().ReverseMap();
        CreateMap<ShowDto, Show>().ReverseMap();
        CreateMap<GameDto, Game>().ReverseMap();
        CreateMap<SeasonDto, Season>().ReverseMap();
        CreateMap<EpisodeDto, Episode>().ReverseMap();

        // User mapping
        CreateMap<ApplicationUser, UserProfileDto>();

        // External IDs mapping
        CreateMap<ExternalId, ExternalIdDto>().ReverseMap();

        // User collectible items mapping
        CreateMap<UserCollectibleItem, UserCollectibleItemDto>()
            .ForMember(dest => dest.CollectibleItem, opt => opt.MapFrom(src => src.CollectibleItem));

        CreateMap<UserCollectibleItemDto, UserCollectibleItem>();

        // Add mapping from AddUserCollectibleItemDto to UserCollectibleItem
        CreateMap<AddUserCollectibleItemDto, UserCollectibleItem>()
            .ConvertUsing<AddUserCollectibleItemDtoToUserCollectibleItemConverter>();

        // Episode progress mapping
        CreateMap<UserShowEpisodeProgress, UserShowEpisodeProgressDto>().ReverseMap();
    }
}

