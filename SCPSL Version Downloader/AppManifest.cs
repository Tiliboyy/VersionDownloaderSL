namespace SCPSL_Version_Downloader;

public class AppManifest
{
    public string VersionName { get; }
    
    public ulong ManifestId { get; }

    public AppManifest(string versionName, ulong manifestId)
    {
        VersionName = versionName;
        ManifestId = manifestId;
    }
}