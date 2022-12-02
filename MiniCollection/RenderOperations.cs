using System.Linq;

namespace Operations
{
    static class RenderOperations
    {
        public static void RenderForce(string forceFile, string outputFile)
        {
            var force = ForceOperations.LoadForce(forceFile);

            using (var writer = new StreamWriter(outputFile))
            {
                writer.WriteLine($"# {force.Name}");
                writer.WriteLine($"**Faction:** {force.Faction}");
                if (force.Miniatures.FirstOrDefault((x) => x.Painted) != null)
                {
                    writer.WriteLine("## Current Force");
                    foreach (var mini in force.Miniatures)
                    {
                        if (mini.Painted)
                        {
                            writer.WriteLine($"- {mini.Name}");
                        }
                    }
                }
                if (force.Miniatures.FirstOrDefault((x) => !x.Painted) != null)
                {
                    writer.WriteLine("## Planned Expansion");
                    foreach (var mini in force.Miniatures)
                    {
                        if (!mini.Painted)
                        {
                            writer.WriteLine($"- {mini.Name}");
                        }
                    }
                }
            }
        }

        public static void RenderForces(string forcePath, string outputPath)
        {
            var files = System.IO.Directory.EnumerateFiles(forcePath, "*.json");
            foreach (var file in files)
            {
                try
                {
                    var outputFile = Path.Join(outputPath, $"{System.IO.Path.GetFileNameWithoutExtension(file)}.md");
                    RenderForce(file, outputFile);
                }
                catch (Exception)
                {
                    Console.Error.WriteLine($"Unable to parse force in {System.IO.Path.GetFileName(file)}");
                }
            }
        }

        public static void RenderCollection(string collectionFile, string forcePath, string outputPath)
        {
            var paintedCounts = new Dictionary<string, uint>();
            var allocatedCounts = new Dictionary<string, uint>();

            var collection = CollectionOperations.LoadCollection(collectionFile);
            var outputFile = Path.Join(outputPath, "collection.md");

            var files = System.IO.Directory.EnumerateFiles(forcePath, "*.json");
            foreach (var file in files)
            {
                try
                {
                    var force = ForceOperations.LoadForce(file);
                    foreach (var mini in force.Miniatures)
                    {
                        if (allocatedCounts.ContainsKey(mini.Name))
                        {
                            allocatedCounts[mini.Name]++;
                        }
                        else
                        {
                            allocatedCounts[mini.Name] = 1;
                        }
                        if (mini.Painted)
                        {
                            if (paintedCounts.ContainsKey(mini.Name))
                            {
                                paintedCounts[mini.Name]++;
                            }
                            else
                            {
                                paintedCounts[mini.Name] = 1;
                            }
                        }
                    }
                }
                catch (Exception)
                {
                    Console.Error.WriteLine($"Unable to parse force in {System.IO.Path.GetFileName(file)}");
                }
            }

            using (var writer = new StreamWriter(outputFile))
            {
                uint totalInCollection = 0;
                uint totalPending = 0;
                uint totalWishlist = 0;
                uint totalAllocated = 0;
                uint totalPainted = 0;

                writer.WriteLine($"# {collection.Name}");
                writer.WriteLine();
                writer.WriteLine("| Miniature | In Collection | Pending Order | Wishlist | Allocated | Painted |");
                writer.WriteLine("| :--- | ---: | ---: | ---: | ---: | ---: |");
    
                foreach (var mini in collection.Miniatures)
                {
                    var painted = paintedCounts.ContainsKey(mini.Name) ? paintedCounts[mini.Name] : 0;
                    var allocated = allocatedCounts.ContainsKey(mini.Name) ? allocatedCounts[mini.Name] : 0;

                    totalInCollection += mini.CountInCollection;
                    totalPending += mini.PendingCount;
                    totalWishlist += mini.WishlistCount;
                    totalAllocated += allocated;
                    totalPainted += painted;
                    
                    writer.WriteLine($"| {mini.Name} | {mini.CountInCollection} | {mini.PendingCount} | {mini.WishlistCount} | {allocated} | { painted} |");
                }

                writer.WriteLine($"| TOTAL | {totalInCollection} | {totalPending} | {totalWishlist} | {totalAllocated} | {totalPainted} |");

                writer.WriteLine();
            }
        }

        public static void RenderReadyToPaint(string collectionFile, string forcePath, string outputPath)
        {
            var paintedCounts = new Dictionary<string, uint>();
            var allocatedCounts = new Dictionary<string, uint>();
            var unpaintedSchemes = new Dictionary<string, List<string> >();


            var collection = CollectionOperations.LoadCollection(collectionFile);
            var outputFile = Path.Join(outputPath, "ready-to-paint.md");

            var files = System.IO.Directory.EnumerateFiles(forcePath, "*.json");
            foreach (var file in files)
            {
                try
                {
                    var force = ForceOperations.LoadForce(file);
                    foreach (var mini in force.Miniatures)
                    {
                        if (allocatedCounts.ContainsKey(mini.Name))
                        {
                            allocatedCounts[mini.Name]++;
                        }
                        else
                        {
                            allocatedCounts[mini.Name] = 1;
                        }
                        if (mini.Painted)
                        {
                            if (paintedCounts.ContainsKey(mini.Name))
                            {
                                paintedCounts[mini.Name]++;
                            }
                            else
                            {
                                paintedCounts[mini.Name] = 1;
                            }
                        }
                        else
                        {
                            if (!unpaintedSchemes.ContainsKey(mini.Name))
                            {
                                unpaintedSchemes[mini.Name] = new List<string>();
                            }

                            unpaintedSchemes[mini.Name].Add(force.Name);
                        }
                    }
                }
                catch (Exception)
                {
                    Console.Error.WriteLine($"Unable to parse force in {System.IO.Path.GetFileName(file)}");
                }
            }

            using (var writer = new StreamWriter(outputFile))
            {
                uint totalReady = 0;

                writer.WriteLine($"# {collection.Name}");
                writer.WriteLine();
                writer.WriteLine("| Miniature | Ready to Paint | Forces |");
                writer.WriteLine("| :--- | ---: | :--- |");
    
                foreach (var mini in collection.Miniatures)
                {
                    var painted = paintedCounts.ContainsKey(mini.Name) ? paintedCounts[mini.Name] : 0;
                    var allocated = allocatedCounts.ContainsKey(mini.Name) ? allocatedCounts[mini.Name] : 0;

                    uint unpaintedInCollection = 0;
                    uint unpaintedInForces = 0;

                    if (mini.CountInCollection > painted)
                    {
                        unpaintedInCollection = mini.CountInCollection - painted;
                    }

                    if (allocated > painted)
                    {
                        unpaintedInForces = allocated - painted;
                    }

                    uint readyToPaint = unpaintedInCollection > unpaintedInForces ? unpaintedInForces : unpaintedInCollection;

                    totalReady += readyToPaint;

                    if (readyToPaint > 0)
                    {
                        writer.WriteLine($"| {mini.Name} | {readyToPaint} | {String.Join(", ", unpaintedSchemes[mini.Name])} |");
                    }
                }

                writer.WriteLine($"| TOTAL | {totalReady} | |");

                writer.WriteLine();
            }
        }
    }
}