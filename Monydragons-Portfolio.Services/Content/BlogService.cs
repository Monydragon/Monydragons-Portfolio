﻿using System.Net.Http.Json;
using Monydragon_Portfolios.Models;
using Monydragons_Portfolio.Common;
using Monydragons_Portfolio.Services.Content.Interface;

namespace Monydragons_Portfolio.Services;

public class BlogService : IBlogService
{
    protected readonly HttpClient _httpClient;

    public BlogService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }
    
    public async Task<IEnumerable<BlogPost>> FetchPostsAsync(int pageNumber, int pageSize)
    {
        // First, fetch all posts (assuming they're not too large in number for this example)
        var allPosts = await GetBlogPostsAsync();

        // Calculate pagination
        var pagedPosts = allPosts
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize);

        return pagedPosts;
    }

    public async Task<List<BlogPost>> GetBlogPostsAsync()
    {
        try
        {
            var response = await _httpClient.GetFromJsonAsync<List<BlogPost>>(AppConstants.DevBlogManifestPath)  ?? new List<BlogPost>();
            return response;        
        }
        catch (Exception e)
        {
            Console.WriteLine("Error fetching blog posts: " + e.Message);
        }
        
        return new List<BlogPost>();
    }

    public async Task<string> GetContentFileAsync(string contentFile)
    {
        try
        {
            // Assuming contentFilePath is a relative URL like "blog/2024/01/2024-01-01 - New Year Coding Challenge/2024-01-01-ProgressPost.txt"
            // Adjust the path as necessary based on how your files are served in relation to the base address of the HttpClient
            var response = await _httpClient.GetAsync(contentFile);
            response.EnsureSuccessStatusCode();

            var content = await response.Content.ReadAsStringAsync();
            return content;
        }
        catch (HttpRequestException ex)
        {
            // Log the error or handle it as appropriate for your application
            Console.WriteLine($"An error occurred while fetching blog post content: {ex.Message}");
            return $"Error loading content: {ex.Message}";
        }
    }
}