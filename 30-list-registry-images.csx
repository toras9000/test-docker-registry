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

try
{
    // Registry host
    var registryHost = "localhost:5000";

    // Generate anonymous access client for target registry
    Console.WriteLine($"Registry: {registryHost}");
    var config = new RegistryClientConfiguration(registryHost);
    var authenticator = new AnonymousOAuthAuthenticationProvider();
    using var client = config.CreateClient(authenticator);

    // Retrieve Image Catalog
    var catalog = await client.Catalog.GetCatalogAsync();

    // Process each image in the catalog
    Console.WriteLine("Images:");
    foreach (var repo in catalog.Repositories)
    {
        // Get image tag list
        var tagsResult = await client.Tags.ListImageTagsAsync(repo);

        // Process each tag
        foreach (var tag in tagsResult.Tags)
        {
            // Image/Tag name
            Console.WriteLine($"  {repo}:{tag}");

            // Get tag manifest
            var tagManifest = default(GetImageManifestResult);
            try
            {
                tagManifest = await client.Manifest.GetManifestAsync(repo, tag);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"    Error={ex.Message}");
                continue;
            }

            // Processing by Manifest Type
            switch (tagManifest.Manifest)
            {
                case ManifestList list:
                    foreach (var item in list.Manifests)
                    {
                        var itemManifest = default(GetImageManifestResult);
                        try
                        {
                            itemManifest = await client.Manifest.GetManifestAsync(repo, item.Digest);
                            var archName = string.Join("/", new[] { item.Platform.Os, item.Platform.Architecture, item.Platform.Variant, }.Where(p => !string.IsNullOrWhiteSpace(p)));
                            var digest = itemManifest.Manifest is ImageManifest2_2 v2 ? v2.Config.Digest.Split(':')[1][..12] : default;
                            Console.WriteLine($"    Arch={archName}, Digest={digest}, MediaType={itemManifest.MediaType}");
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine($"    Error={ex.Message}");
                            continue;
                        }
                    }
                    break;

                default:
                    Console.WriteLine($"    Arch=(unknown), MediaType={tagManifest.MediaType}");
                    break;
            }
        }

    }
}
catch (Exception ex)
{
    Console.WriteLine($"  Error: {ex}");
}
