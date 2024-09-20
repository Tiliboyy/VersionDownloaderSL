using YamlDotNet.Serialization;

namespace SCPSL_Version_Downloader;

public class Config
{
    public static Config Instance;
    public string Username { get; set; } = "Your Username";
    public string Password { get; set; } = "Your Password";
    public static void Load()
    {
        if (!File.Exists("Config.yml"))
        {
            File.WriteAllText("Config.yml",new Serializer().Serialize(new Config()));
        }
        var text = File.ReadAllText("Config.yml");
        Instance = new Deserializer().Deserialize<Config>(text);
    }
}