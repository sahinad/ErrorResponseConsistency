using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
namespace Blog.API.Endpoints;

public static class BlogEndpoints
{
    public static void MapBlogEndpoints(this IEndpointRouteBuilder routes)
    {
        var group = routes.MapGroup("/api/Blog").WithTags(nameof(Blog));

        group.MapGet("/", async (AppDbContext db) => await db.Blogs.ToListAsync())
        .WithName("GetAllBlogs")
        .WithOpenApi()
        .RequireAuthorization("admin");

        group.MapGet("/{id}", async Task<Results<Ok<Entities.Blog>, NotFound>>(int id, AppDbContext db) =>
            {
                return await db.Blogs.AsNoTracking()
                        .FirstOrDefaultAsync(model => model.Id == id)
                    is { } model
                    ? TypedResults.Ok(model)
                    : TypedResults.NotFound();
            })
            .WithName("GetBlogById")
            .WithOpenApi();

        group.MapPut("/{id}", async Task<Results<Ok, NotFound>> (int id, Entities.Blog blog, AppDbContext db) =>
        {
            var affected = await db.Blogs
                .Where(model => model.Id == id)
                .ExecuteUpdateAsync(setters => setters
                  .SetProperty(m => m.Id, blog.Id)
                  .SetProperty(m => m.Name, blog.Name)
                  .SetProperty(m => m.SiteUrl, blog.SiteUrl)
                );

            return affected == 1 ? TypedResults.Ok() : TypedResults.NotFound();
        })
        .WithName("UpdateBlog")
        .WithOpenApi();

        group.MapPost("/", async (Entities.Blog blog, AppDbContext db) =>
        {
            db.Blogs.Add(blog);
            await db.SaveChangesAsync();
            return TypedResults.Created($"/api/Blog/{blog.Id}", blog);
        })
        .WithName("CreateBlog")
        .WithOpenApi();

        group.MapDelete("/{id}", async Task<Results<Ok, NotFound>> (int id, AppDbContext db) =>
        {
            var affected = await db.Blogs
                .Where(model => model.Id == id)
                .ExecuteDeleteAsync();

            return affected == 1 ? TypedResults.Ok() : TypedResults.NotFound();
        })
        .WithName("DeleteBlog")
        .WithOpenApi();
    }
}