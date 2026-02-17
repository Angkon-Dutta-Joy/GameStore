using Microsoft.EntityFrameworkCore;

namespace GameStore.Api.Data;

public static class DataExtensions
{
    public static void MigrateDb(this WebApplication app)
    {
        using var scope = app.Services.CreateScope();
        var dbcontext = scope.ServiceProvider.GetRequiredService<GameStoreContext>();
        dbcontext.Database.Migrate();
    }

    public static void AddGameStoreDb(this WebApplicationBuilder builder)
    {
        var connString = "Data Source=GameStore.db";

        builder.Services.AddSqlite<GameStoreContext>(
            connString,
            options => options.UseSeeding((context, _) =>
                    {
                        if (!context.Set<Genre>().Any())
                        {
                            context
                                .Set<Genre>()
                                .AddRange(
                                    new Genre { Name = "Fighting" },
                                    new Genre { Name = "RPG" },
                                    new Genre { Name = "Platformer" },
                                    new Genre { Name = "Racing" },
                                    new Genre { Name = "Sports" }
                                );

                            context.SaveChanges();
                        }
                    }
                )
        );
    }
}
