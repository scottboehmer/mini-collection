namespace MiniCollectionTool;

static class CollectionOperations
{
    public static void NewCollection(string file, string name, bool interactive)
    {
        var collection = new Data.Collection();
        collection.Name = name;

        if (interactive)
        {
            var list = GetInteractiveList();
            foreach (var entry in list)
            {
                var match = collection.Miniatures.Find((x) => String.Equals(x.Name, entry.Name));
                if (match == null)
                {
                    collection.Miniatures.Add(entry);
                }
                else
                {
                    match.CountInCollection += entry.CountInCollection;
                    match.PendingCount += entry.PendingCount;
                }
            }
        }

        SaveCollection(file, collection);
    }

    public static void PrintCollection(string file)
    {
        Data.Collection collection = LoadCollection(file);

        Console.WriteLine(collection.Name);
        foreach (var entry in collection.Miniatures)
        {
            Console.WriteLine($"- {entry.Name} ... {entry.CountInCollection + entry.PendingCount}");
        }
    }

    public static void CountCollection(string file, string forcesPath)
    {
        Data.Collection collection = LoadCollection(file);
        uint count = 0;
        uint pending = 0;
        uint painted = 0;
        uint allocated = 0;

        foreach (var entry in collection.Miniatures)
        {
            count += entry.CountInCollection;
            pending += entry.PendingCount;
        }

        var forceFiles = System.IO.Directory.EnumerateFiles(forcesPath, "*.json");
        foreach (var forceFile in forceFiles)
        {
            try
            {
                var force = ForceOperations.LoadForce(forceFile);
                
                foreach (var forceMini in force.Miniatures)
                {
                    if (forceMini.Painted)
                    {
                        painted++;
                    }
                    allocated++;
                }
            }
            catch (Exception)
            {
                Console.Error.WriteLine($"Unable to parse force in {System.IO.Path.GetFileName(file)}");
            }
        }

        Console.WriteLine(collection.Name);
        Console.WriteLine($"- In Collection: {count}");
        Console.WriteLine($"- Allocated to a Force: {allocated}");
        Console.WriteLine($"- Painted: {painted}");
        Console.WriteLine($"- Pending Purchases: {pending}");
    }

    public delegate bool ForceMiniatureFilter(Data.ForceMiniature mini);

    public static void PrintFilteredCollection(string file, string forcesPath, ForceMiniatureFilter filter)
    {
        Data.Collection collection = LoadCollection(file);

        var forceFiles = System.IO.Directory.EnumerateFiles(forcesPath, "*.json");
        foreach (var forceFile in forceFiles)
        {
            try
            {
                var force = ForceOperations.LoadForce(forceFile);
                
                foreach (var forceMini in force.Miniatures)
                {
                    if (!filter(forceMini))
                    {
                        var match = collection.Miniatures.Find((x) => String.Equals(x.Name, forceMini.Name));
                        if (match == null)
                        {
                            Console.Error.WriteLine($"No match for miniature name in collection: {forceMini.Name}");
                        }
                        else if (match.CountInCollection == 0)
                        {
                            Console.Error.WriteLine($"Not enough of miniature in collection: {forceMini.Name}");
                        }
                        else
                        {
                            match.CountInCollection--;
                        }
                    }
                }
            }
            catch (Exception)
            {
                Console.Error.WriteLine($"Unable to parse force in {System.IO.Path.GetFileName(file)}");
            }
        }

        Console.WriteLine(collection.Name);
        foreach (var entry in collection.Miniatures)
        {
            var count = entry.CountInCollection + entry.PendingCount;
            if (count > 0)
            {
                Console.WriteLine($"- {entry.Name} ... {entry.CountInCollection + entry.PendingCount}");
            }
        }
    }

    public static void AddPendingMiniature(string file, string miniature)
    {
        bool added = false;
        Data.Collection collection = LoadCollection(file);
        foreach (var entry in collection.Miniatures)
        {
            if (String.Equals(entry.Name, miniature))
            {
                if (entry.WishlistCount > 0)
                {
                    entry.WishlistCount--;
                }
                entry.PendingCount++;
                added = true;
            }
        }
        if (!added)
        {
            var mini = new Data.CollectionMiniature();
            mini.Name = miniature;
            mini.PendingCount = 1;
            collection.Miniatures.Add(mini);
            added = true;
        }
        SaveCollection(file, collection);
    }

    public static void AddWishlistMiniature(string file, string miniature)
    {
        bool added = false;
        Data.Collection collection = LoadCollection(file);
        foreach (var entry in collection.Miniatures)
        {
            if (String.Equals(entry.Name, miniature))
            {
                entry.WishlistCount++;
                added = true;
            }
        }
        if (!added)
        {
            var mini = new Data.CollectionMiniature();
            mini.Name = miniature;
            mini.WishlistCount = 1;
            collection.Miniatures.Add(mini);
            added = true;
        }
        SaveCollection(file, collection);
    }

    public static void AddMiniature(string file, string miniature, bool removeFromPending = false)
    {
        bool added = false;
        Data.Collection collection = LoadCollection(file);
        foreach (var entry in collection.Miniatures)
        {
            if (String.Equals(entry.Name, miniature))
            {
                if (removeFromPending)
                {
                    if (entry.PendingCount == 0)
                    {
                        Console.Error.WriteLine("Unable to remove miniature from pending");
                        return;
                    }
                    entry.PendingCount--;
                }
                entry.CountInCollection++;
                added = true;
            }
        }
        if (!added)
        {
            if (removeFromPending)
            {
                Console.Error.WriteLine("Unable to remove miniature from pending");
                return;
            }
            var mini = new Data.CollectionMiniature();
            mini.Name = miniature;
            mini.CountInCollection = 1;
            collection.Miniatures.Add(mini);
            added = true;
        }
        SaveCollection(file, collection);
    }

    public static void AddMiniatures(string file)
    {
        var collection = LoadCollection(file);
        var list = GetInteractiveList();
        foreach (var entry in list)
        {
            var match = collection.Miniatures.FirstOrDefault(x => String.Equals(entry.Name, x.Name));
            if (match != null)
            {
                match.CountInCollection += entry.CountInCollection;
            }
            else
            {
                collection.Miniatures.Add(entry);
            }
        }
        SaveCollection(file, collection);
    }

    public static Data.Collection LoadCollection(string file)
    {
        using (var stream = new FileStream(file, FileMode.Open, FileAccess.Read))
        {
            var collection = System.Text.Json.JsonSerializer.Deserialize<Data.Collection>(stream);
            if (collection == null)
            {
                throw new FormatException($"Unable to read collection file: {file}");
            }
            return collection;
        }
    }

    private static void SaveCollection(string file, Data.Collection collection)
    {
        collection.Miniatures.Sort((a,b) => String.Compare(a.Name, b.Name));
        using (var stream = new FileStream(file, FileMode.Create, FileAccess.Write))
        {
            var options = new System.Text.Json.JsonSerializerOptions();
            options.WriteIndented = true;
            System.Text.Json.JsonSerializer.Serialize<Data.Collection>(stream, collection, options);
        }
    }

    private static List<Data.CollectionMiniature> GetInteractiveList()
    {
        Console.WriteLine("Enter a name of 'done' in order to exit.");
        var list = new List<Data.CollectionMiniature>();
        while(true)
        {
            Console.Write("Miniature Name: ");
            string? miniName = Console.ReadLine()?.Trim();
            if (miniName == null || String.Equals(miniName, "done"))
            {
                break;
            }
            Console.Write("Count: ");
            string? count = Console.ReadLine()?.Trim();
            if (count == null)
            {
                break;
            }
            if (UInt32.TryParse(count, out uint parsedCount))
            {
                var miniature = new Data.CollectionMiniature();
                miniature.Name = miniName;
                miniature.CountInCollection = parsedCount;
                list.Add(miniature);
            }
            else
            {
                Console.Error.WriteLine("Unable to parse count!");
            }
        }
        return list;
    }
}