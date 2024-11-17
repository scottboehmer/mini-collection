using System.CommandLine;
using System.Net;

namespace MiniCollectionTool;

class Program
{
    static int Main(string[] args)
    {
        var miniOption = new Option<string?>(
            name: "--mini",
            description: "The miniature name"
        );

        var forceOption = new Option<string?>(
            name: "--force",
            description: "The force name"
        );

        var factionOption = new Option<string?>(
            name: "--faction",
            description: "The faction name"
        );
        
        var interactiveOption = new Option<bool>(
            name: "--interactive",
            description: "Use interactive mode",
            getDefaultValue: () => false
        );

        var addCommand = new Command("add-mini", "Add a miniature to the collection")
        {
            miniOption,
            interactiveOption
        };
        addCommand.SetHandler((i, m) => {AddMiniature(i, m);}, interactiveOption, miniOption);

        var addForceCommand = new Command("add-force", "Add a force to the collection")
        {
            forceOption,
            factionOption
        };
        addForceCommand.SetHandler((force, faction) => {AddForce(force, faction);}, forceOption, factionOption);

        var addMiniToForceCommand = new Command("add-mini-to-force", "Add a mini to a force")
        {
            forceOption,
            miniOption
        };
        addMiniToForceCommand.SetHandler((force,mini) => {AddMiniToForce(force, mini);}, forceOption, miniOption);

        var renderCommand = new Command("render", "Generate markdown files for the collection");
        renderCommand.SetHandler(() => {Render(); });

        var rootCommand = new RootCommand("Mini Collection Tool");
        rootCommand.AddCommand(renderCommand);
        rootCommand.AddCommand(addCommand);
        rootCommand.AddCommand(addForceCommand);
        rootCommand.AddCommand(addMiniToForceCommand);
        return rootCommand.Invoke(args);
    }

    static int AddMiniature(bool interactive, string? miniature)
    {
        EnsureCollection();

        if (interactive)
        {
            CollectionOperations.AddMiniatures(FileHelpers.GetCollectionFileName());
        }
        else
        {
            if (String.IsNullOrEmpty(miniature))
            {
                Console.Error.WriteLine("Miniature name required");
                return 1;
            }
            CollectionOperations.AddMiniature(FileHelpers.GetCollectionFileName(), miniature);
        }
        return 0;
    }

    static int AddForce(string? force, string? faction)
    {
        if (String.IsNullOrEmpty(force))
        {
            Console.Error.WriteLine("Force name required");
            return 1;
        }
        if (String.IsNullOrEmpty(faction))
        {
            Console.Error.WriteLine("Faction name required");
            return 1;
        }
        var fileName = FileHelpers.GetForceFileName(force);
        ForceOperations.NewForce(fileName, force, faction, true);
        return 0;
    }

    static int AddMiniToForce(string? force, string? mini)
    {
        if (String.IsNullOrEmpty(force))
        {
            Console.Error.WriteLine("Force name required");
            return 1;
        }
        if (String.IsNullOrEmpty(mini))
        {
            Console.Error.WriteLine("Miniature name required");
            return 1;
        }
        var fileName = FileHelpers.GetForceFileName(force);
        ForceOperations.AddToForce(fileName, mini);
        return 0;
    }

    static int Render()
    {
        EnsureCollection();

        RenderOperations.RenderCollection(FileHelpers.GetCollectionFileName(), FileHelpers.GetForcesDirectory(), FileHelpers.GetRenderDirectory());
        RenderOperations.RenderReadyToPaint(FileHelpers.GetCollectionFileName(), FileHelpers.GetForcesDirectory(), FileHelpers.GetRenderDirectory());
        RenderOperations.RenderUnallocated(FileHelpers.GetCollectionFileName(), FileHelpers.GetForcesDirectory(), FileHelpers.GetRenderDirectory());
        RenderOperations.RenderForces(FileHelpers.GetForcesDirectory(), FileHelpers.GetRenderDirectory());
        return 0;
    }

    static void EnsureCollection()
    {
        var file = FileHelpers.GetCollectionFileName();
        if (!File.Exists(file))
        {
            CollectionOperations.NewCollection(file, "My Collection", false);
        }
    }
}