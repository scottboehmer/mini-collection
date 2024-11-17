using System.CommandLine;

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
        addCommand.SetHandler((interactive, mini) => {AddMiniature(interactive, mini);}, interactiveOption, miniOption);

        var addForceCommand = new Command("add-force", "Add a force to the collection")
        {
            forceOption,
            factionOption,
            interactiveOption
        };
        addForceCommand.SetHandler((interactive, force, faction) => {AddForce(interactive, force, faction);}, interactiveOption, forceOption, factionOption);

        var addMiniToForceCommand = new Command("add-mini-to-force", "Add a mini to a force")
        {
            forceOption,
            miniOption
        };
        addMiniToForceCommand.SetHandler((force,mini) => {AddMiniToForce(force, mini);}, forceOption, miniOption);

        var paintMiniCommand = new Command("paint-mini", "Mark a miniature as painted")
        {
            forceOption,
            miniOption
        };
        paintMiniCommand.SetHandler((force,mini) => {PaintMini(force, mini);}, forceOption, miniOption);

        var renderCommand = new Command("render", "Generate markdown files for the collection");
        renderCommand.SetHandler(() => {Render(); });

        var rootCommand = new RootCommand("Mini Collection Tool");
        rootCommand.AddCommand(renderCommand);
        rootCommand.AddCommand(addCommand);
        rootCommand.AddCommand(addForceCommand);
        rootCommand.AddCommand(addMiniToForceCommand);
        rootCommand.AddCommand(paintMiniCommand);
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

    static int AddForce(bool interactive, string? force, string? faction)
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
        ForceOperations.NewForce(fileName, force, faction, interactive);
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

    static int PaintMini(string? force, string? mini)
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
        ForceOperations.MarkUnitAsPainted(fileName, mini);
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