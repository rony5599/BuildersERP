namespace BuilderERP.Web.Storage;

public interface IUploadsPathProvider
{
    //string PhysicalRoot { get; }
    public string PhysicalRoot
    {
        get
        {
            var relativePath = "uploads"; // or your relative path
            var absolutePath = Path.Combine(AppContext.BaseDirectory, relativePath);
            return Path.GetFullPath(absolutePath);
        }
    }

    string GetPath(string subfolder);
}

/// <summary>
/// Resolves where uploaded files are stored on disk. Defaults to a folder outside the
/// app's content root (sibling of it) in non-Development environments so that a CI/CD
/// deploy which mirrors the publish output into the content root can never delete
/// previously uploaded files. Override via "Storage:UploadsRoot" (or the
/// STORAGE__UPLOADSROOT environment variable) to point at a dedicated data drive/share.
/// </summary>
public class UploadsPathProvider : IUploadsPathProvider
{
    public string PhysicalRoot { get; }

    public UploadsPathProvider(IConfiguration configuration, IWebHostEnvironment environment)
    {
        var configuredRoot = configuration["Storage:UploadsRoot"];

        if (!string.IsNullOrWhiteSpace(configuredRoot))
        {
            PhysicalRoot = configuredRoot;
        }
        else if (environment.IsDevelopment())
        {
            PhysicalRoot = Path.Combine(environment.WebRootPath, "uploads");
        }
        else
        {
            var contentRootParent = Directory.GetParent(environment.ContentRootPath)?.FullName
                ?? environment.ContentRootPath;
            PhysicalRoot = Path.Combine(contentRootParent, "BuilderERP-Data", "uploads");
        }
    }

    public string GetPath(string subfolder) => Path.Combine(PhysicalRoot, subfolder);
}
