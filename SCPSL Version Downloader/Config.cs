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
            Instance = new Config();
            File.WriteAllText("Config.yml",new Serializer().Serialize(Instance));
            return;
        }
        var text = File.ReadAllText("Config.yml");
        Instance = new Deserializer().Deserialize<Config>(text);
    }
}