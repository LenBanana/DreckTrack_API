using AutoMapper;
using DreckTrack_API.AutoMapper.TypeConverter;
using DreckTrack_API.Models.Dto;
using DreckTrack_API.Models.Entities;
using DreckTrack_API.Models.Entities.Collectibles;
using DreckTrack_API.Models.Entities.Collectibles.Information;

namespace DreckTrack_API.AutoMapper;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        // Base mapping
        CreateMap<CollectibleItemDto, CollectibleItem>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.ExternalIds, opt => opt.Ignore())
            .Include<BookDto, Book>()
            .Include<MovieDto, Movie>()
            .Include<ShowDto, Show>();
        CreateMap<CollectibleItem, CollectibleItemDto>()
            .Include<Book, BookDto>()
            .Include<Movie, MovieDto>()
            .Include<Show, ShowDto>();

        // Derived mappings
        CreateMap<BookDto, Book>().ReverseMap();
        CreateMap<MovieDto, Movie>().ReverseMap();
        CreateMap<ShowDto, Show>().ReverseMap();
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

