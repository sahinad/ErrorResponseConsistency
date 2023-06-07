using System.ComponentModel.DataAnnotations;

namespace Blog.API.Entities;

public class Blog
{
    public int Id { get; set; }
    
    [MaxLength(200)]
    public required string Name { get; set; }

    [MaxLength(200)]
    public required string SiteUrl { get; set; }
}