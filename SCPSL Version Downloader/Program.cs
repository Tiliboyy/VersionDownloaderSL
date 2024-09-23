
using System.Diagnostics;
using System.IO.Compression;
using System.Net;
#pragma warning disable SYSLIB0014

namespace SCPSL_Version_Downloader;

public static class Program
{
    private const string SteamCmdUrl = "https://steamcdn-a.akamaihd.net/client/installer/steamcmd.zip";
    private const string SteamCmdZip = "steamcmd.zip";
    private const string SteamCmdDirectory = "SteamCmd";
    private const string SteamCmdExecutable = "steamcmd.exe";
    private const string GamePath = "SCP Secret Laboratory";
    private const int AppId = 700330 ;
    private const int DepotId = 700331;

    #region manifests

    

    private static readonly List<AppManifest> AppManifests =
    [
        new AppManifest("3.3.3", 7924322849000851029),
        new AppManifest("4.0.0?", 4963207822109185377),
        new AppManifest("5.0.0?", 5722470823901679166),
        new AppManifest("5.0.1", 4089705714363909038),
        new AppManifest("5.1.0", 7734289575429951173),
        new AppManifest("5.1.1?", 3800401292358127990),
        new AppManifest("6.0.0", 7089478779300548807),
        new AppManifest("6.0.1", 793573531890397746),
        new AppManifest("6.2.0?", 1226841551812954575),
        new AppManifest("7.0.0", 8489169785801766595),
        new AppManifest("7.1.0", 8807654374367541235),
        new AppManifest("7.2.0", 2377293815234063777),
        new AppManifest("7.3.0?", 776792956030232683),
        new AppManifest("7.3.1", 3783932444127680093),
        new AppManifest("7.4.0", 6376127292146458708),
        new AppManifest("8.0.0", 232646715872639227),
        new AppManifest("Last Megapatch 1 Version", 8083032121875433812),
        new AppManifest("9.0.0", 2366631187512234141),
        new AppManifest("9.0.1", 2996374676097618839),
        new AppManifest("9.0.2", 6917946207489532109),
        new AppManifest("9.0.3", 2763191184725075339),
        new AppManifest("9.1.0", 2770466623095488309),
        new AppManifest("9.1.1?", 2872011996962451918),
        new AppManifest("9.1.2?", 8313805103622281799),
        new AppManifest("9.1.3", 8921359437193746517),
        new AppManifest("10.0.0", 6433805092299598103),
        new AppManifest("10.0.1 (Used in Glitched Death%)", 5342902436675276477),
        new AppManifest("10.0.2", 8326472859548146131),
        new AppManifest("10.0.3", 3224655206188158628),
        new AppManifest("10.0.4", 4051947639075134857),
        new AppManifest("Halloween 2020", 2943695047864811766),
        new AppManifest("10.1", 1488128717569901429),
        new AppManifest("10.1.1", 2120264334119202861),
        new AppManifest("10.1.2", 1758573161302072344),
        new AppManifest("Christmas 2020", 7343233868355134570),
        new AppManifest("10.2", 631493978816937676),
        new AppManifest("10.2.1", 4901196860528136185),
        new AppManifest("10.2.2", 6880895020545579636),
        new AppManifest("Halloween 2021", 4531559817783273895),
        new AppManifest("11.0.3", 278873398872973782),
        new AppManifest("11.1", 1048801106569093509),
        new AppManifest("Christmas 2021", 7493629236436561204),
        new AppManifest("11.1.2", 8787302709176048575),
        new AppManifest("11.1.3", 6258986561530864513),
        new AppManifest("11.1.4", 5745289456538530560),
        new AppManifest("11.1.5", 3274841123143224184),
        new AppManifest("11.2", 2456092932804224326),
        new AppManifest("11.2.1", 193310193313059048),
        new AppManifest("12.0 (Christmas 2022)", 4574081108245455218),
        new AppManifest("12.0.1 (Holiday Patch)", 2857661223516013318),
        new AppManifest("13.0.0", 8846682790161457503),
        new AppManifest("13.1", 6126205187152802267)
    ];
    #endregion

    private static Config Config => Config.Instance;
    public static async Task Main()
    {
        try
        {
            Config.Load();
            await Start();
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            Console.ReadKey();
        }
    }
    private static async Task Start()
    {
        var selectedVersion = SelectVersion();
        while (selectedVersion == null)
        {
            selectedVersion = SelectVersion();
        }
        await DownloadSteamCmd();
        await Download(selectedVersion);
        CopyGame(selectedVersion);
        Console.WriteLine("Done!");
        Console.ReadKey();
    }
    private static void CopyGame(AppManifest selectedVersion)
    {
        Console.Clear();
        if (!Directory.Exists(GamePath))
            Directory.CreateDirectory(GamePath);
        var appDir = Path.Combine(SteamCmdDirectory, $"steamapps/content/app_{AppId}");
        CopyFilesRecursively(appDir, GamePath + $"/{selectedVersion.VersionName}");
        Directory.Delete(appDir, true);
    }
    private static async Task Download(AppManifest selectedVersion)
    {
        var download = DownloadGame(selectedVersion);
        
        
        var dots = 1;
        while (!download.IsCompleted)
        {
            var text = "Downloading";
            dots++;
            if (dots > 3)
                dots = 1;
            for (int i = 0; i < dots; i++)
            {
                text += ".";
            }
            Console.Clear();
            Console.WriteLine("This might take a while");
            Console.WriteLine(text);
            await Task.Delay(TimeSpan.FromSeconds(1f));

        }
        Console.WriteLine("Task Done!");
    }
    private static AppManifest? SelectVersion()
    {
        for (int i = 0; i < AppManifests.Count; i++)
        {
            Console.WriteLine($"{i + 1}. {AppManifests[i].VersionName}");
        }

        Console.Write("Select a version: ");
        if (!int.TryParse(Console.ReadLine(), out var selectedIndex))
        {
            Console.WriteLine("Invalid selection.");
            return null;
        }
        selectedIndex -=1;

        if (selectedIndex < 0 || selectedIndex >= AppManifests.Count)
        {
            Console.WriteLine("Invalid selection.");
            return null;
        }

        var selectedVersion = AppManifests[selectedIndex];
        Console.WriteLine($"Selected Version {selectedVersion.VersionName} Manifest: {selectedVersion.ManifestId}");
        return selectedVersion;
    }
    private static void CopyFilesRecursively(string sourcePath, string targetPath)
    {
        foreach (string dirPath in Directory.GetDirectories(sourcePath, "*", SearchOption.AllDirectories))
        {
            Directory.CreateDirectory(dirPath.Replace(sourcePath, targetPath));
        }

        foreach (string newPath in Directory.GetFiles(sourcePath, "*.*",SearchOption.AllDirectories))
        {
            File.Copy(newPath, newPath.Replace(sourcePath, targetPath), true);
        }
        
    }
    private static async Task DownloadSteamCmd()
    {
        if (File.Exists(Path.Combine(SteamCmdDirectory, SteamCmdExecutable)))
        {
            return;
        }
        if (!Directory.Exists(SteamCmdDirectory))
        {
            Directory.CreateDirectory(SteamCmdDirectory);
        }

        using (WebClient client = new WebClient())
        {
            var tcs = new TaskCompletionSource<bool>();

            client.DownloadProgressChanged += (sender, args) =>
            {
                Console.Clear();
                Console.WriteLine("Downloading steamcmd...");
                Console.WriteLine($"Fortschritt: {args.ProgressPercentage}%");
            };

            client.DownloadFileCompleted += (sender, args) =>
            {
                if (args.Error != null)
                {
                    tcs.SetException(args.Error);
                }
                else
                {
                    tcs.SetResult(true);
                    Console.WriteLine("Download complete.");
                }
            };

            Console.WriteLine("Downloading steamcmd...");
            client.DownloadFileAsync(new Uri(SteamCmdUrl), Path.Combine(SteamCmdDirectory, SteamCmdZip));
            await tcs.Task;
            
        }
        Console.WriteLine("Extracting steamcmd...");
        ZipFile.ExtractToDirectory(Path.Combine(SteamCmdDirectory, SteamCmdZip), SteamCmdDirectory);
        File.Delete(Path.Combine(SteamCmdDirectory, SteamCmdZip));
    }
    private static async Task DownloadGame(AppManifest manifest)
    {
        Console.Clear();
        var arguments = $"+login {Config.Username} {Config.Password} +download_depot {AppId} {DepotId} {manifest.ManifestId} +quit";

        bool wroteEmptyError = false;
        ProcessStartInfo startInfo = new ProcessStartInfo
        {
            FileName = Path.Combine(Directory.GetCurrentDirectory(),SteamCmdDirectory, SteamCmdExecutable),
            Arguments = arguments,
            RedirectStandardOutput = true,  // Ausgabe in der Konsole anzeigen
            RedirectStandardError = true,
            UseShellExecute = false,
            CreateNoWindow = true
        };

        Console.WriteLine("Starting Download... (This might take a while)");
        using var process = Process.Start(startInfo);
        if (process == null)
        {

            Console.WriteLine("Error while starting SteamCmd: Process not found");
            return;

        }
        process.OutputDataReceived += (sender, args) =>
        {
            var current = "";
            if (File.Exists("logs.txt"))
                current = File.ReadAllText("logs.txt");
            File.WriteAllText("logs.txt",current+"\n"+args.Data);
        };
        process.ErrorDataReceived += (sender, args) =>
        {
            if (!wroteEmptyError)
            {
                return;
            }
            Console.WriteLine("ERROR: " + args.Data);
            var current = "";
            if (File.Exists("logs.txt"))
                current = File.ReadAllText("logs.txt");
            File.WriteAllText("logs.txt",current+"\n"+args.Data);

        };

        process.BeginOutputReadLine();
        process.BeginErrorReadLine();

        await process.WaitForExitAsync();
    }
}