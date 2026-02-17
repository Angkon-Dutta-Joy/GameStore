using System;
using GameStore.Api.Dtos;

namespace GameStore.Api.EndPoints;

public static class GamesEndPoints
{
    
    const string GetGameEndpointName = "GetGame";
    private static readonly List<GameDto> games =                         // Initialize an in-memory list of games with some sample data
    [
        new(1, "Street Fighter II", "Fighting", 19.99M, new DateOnly(1992, 7, 15)),
        new(2, "Final Fantasy VII Rebirth", "RPG", 69.99M, new DateOnly(2024, 2, 29)),
        new(3, "Astro Bot", "Platformer", 59.99M, new DateOnly(2024, 9, 6))
    ];

    public static void MapGamesEndPoints(this WebApplication app)
    {

        var group = app.MapGroup("/games");
        // GET /games
        group.MapGet("/", () => games);           //no need to write /games here because we are already in the /games group, this endpoint will return the list of all games in the in-memory list when a GET request is made to /games

        // GET /games/l
        group.MapGet("/{id}", (int id) => 
        { 
            var game = games.Find(game => game.Id == id);

            return game is null ? Results.NotFound() : Results.Ok(game);  // Return the game if found, otherwise return a 404 Not Found response
        })
            .WithName(GetGameEndpointName);  // MapGet endpoint to retrieve a game by its id, using the Find method to search the games list for a game with the matching id. The endpoint is named using WithName for use in the CreatedAtRoute response of the POST endpoint.

        // POST /games
        group.MapPost("/", (CreateGameDto newGame) => {
                GameDto game = new(
                    games.Count + 1,
                    newGame.Name,
                    newGame.Genre,
                    newGame.Price,
                    newGame.ReleaseDate
                );

            games.Add(game);  // Add the new game to the list

            return Results.CreatedAtRoute(GetGameEndpointName, new { id = game.Id }, game); // Return a 201 Created response with the location of the new game and the game data in the response body
        });


        // Put /games/1
        group.MapPut("/{id}", (int id, UpdateGameDto updatedGame) =>     // MapPut endpoint to update an existing game with the specified id using the data from the UpdateGameDto
        {
            var index = games.FindIndex(game => game.Id == id);   // Find the index of the game to update
            if (index == -1) 
            {
                return Results.NotFound();           // If the game is not found, return a 404 Not Found response
            }  

            games[index] = new GameDto(  // Create a new GameDto with the updated information
                id,
                updatedGame.Name,
                updatedGame.Genre,
                updatedGame.Price,
                updatedGame.ReleaseDate
            );
            
            return Results.NoContent();  // Return a 204 No Content response to indicate the update was successful
        }); 


        // DELETE /games/1
        group.MapDelete("/{id}", (int id) =>  // MapDelete endpoint to delete a game with the specified id
        {
            games.RemoveAll(game => game.Id ==id);
            return Results.NoContent();  // Return a 204 No Content response to indicate the deletion was successful
        });
    }
}

