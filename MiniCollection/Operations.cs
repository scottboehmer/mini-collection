namespace Operations
{
    static class CollectionOperations
    {
        public static void NewCollection(string file, string name)
        {
            var collection = new Data.Collection();
            collection.Name = name;

            SaveCollection(file, collection);
        }

        public static void PrintCollection(string file)
        {
            Data.Collection collection = LoadCollection(file);

            Console.WriteLine(collection.Name);
            foreach (var entry in collection.Miniatures)
            {
                Console.WriteLine($"{entry.Name} ... {entry.CountInCollection + entry.PendingCount}");
            }
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

        private static Data.Collection LoadCollection(string file)
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
    }
}