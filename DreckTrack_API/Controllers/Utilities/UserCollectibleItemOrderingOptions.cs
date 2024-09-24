using DreckTrack_API.Models.Dto;
using DreckTrack_API.Models.Entities.Collectibles.Items;
using DreckTrack_API.Models.Entities.Collectibles.Items.Game;
using DreckTrack_API.Models.Entities.Collectibles.Items.Show;

namespace DreckTrack_API.Controllers.Utilities;

public static class UserCollectibleItemOrderingOptions
{
    public static List<OrderingOption<UserCollectibleItem>> GetOptions(string? itemType)
    {
        var baseOptions = new List<OrderingOption<UserCollectibleItem>>
        {
            new()
            {
                Key = "Title",
                DisplayName = "Title",
                OrderExpression = uci => uci.CollectibleItem.Title
            },
            new()
            {
                Key = "DateAdded",
                DisplayName = "Date Added",
                OrderExpression = uci => uci.DateAdded
            },
            new()
            {
                Key = "ReleaseDate",
                DisplayName = "Release Date",
                OrderExpression = uci => uci.CollectibleItem.ReleaseDate ?? DateTime.MinValue
            }
        };

        switch (itemType)
        {
            // Extend options based on itemType
            case "Game":
                baseOptions.Add(new OrderingOption<UserCollectibleItem>
                {
                    Key = "TimePlayed",
                    DisplayName = "Time Played",
                    OrderExpression = uci => 
                        uci.CollectibleItem.ItemType == "Game" 
                            ? ((Game)uci.CollectibleItem).TimePlayed 
                            : 0
                });
                break;
            case "Show":
                baseOptions.AddRange(new List<OrderingOption<UserCollectibleItem>>
                {
                    new()
                    {
                        Key = "Seasons",
                        DisplayName = "Seasons",
                        OrderExpression = uci => 
                            uci.CollectibleItem.ItemType == "Show" 
                                ? ((Show)uci.CollectibleItem).Seasons.Count 
                                : 0
                    },
                    new()
                    {
                        Key = "Episodes",
                        DisplayName = "Episodes",
                        OrderExpression = uci => 
                            uci.CollectibleItem.ItemType == "Show" 
                                ? ((Show)uci.CollectibleItem).Seasons.Sum(season => season.Episodes.Count) 
                                : 0
                    }
                });
                break;
        }

        return baseOptions;
    }
}