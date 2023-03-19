#nullable enable
#r "nuget: Docker.Registry.DotNet, 1.2.1"
using Docker.Registry.DotNet;
using Docker.Registry.DotNet.Authentication;
using System.Threading;
using Docker.Registry.DotNet.Models;

// This is a script for dotnet-script.
// After installing the .NET SDK, can execute the following.
// $ dotnet tool install -g dotnet-script
// $ dotnet script ./30-list-registry-images.csx

{
    var config = new RegistryClientConfiguration("localhost:5000");
    var authenticator = new AnonymousOAuthAuthenticationProvider();
    using var client = config.CreateClient(authenticator);
    var catalog = await client.Catalog.GetCatalogAsync();
    foreach (var repo in catalog.Repositories)
    {
        var tagsResult = await client.Tags.ListImageTagsAsync(repo);
        foreach (var tag in tagsResult.Tags)
        {
            try
            {
                var tagManifest = await client.Manifest.GetManifestAsync(repo, tag);
                Console.WriteLine($"Tag={tag}, MediaType={tagManifest.MediaType}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Tag={tag}, Error={ex.Message}");
            }
        }
    }
}