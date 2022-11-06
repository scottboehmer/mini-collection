using System.Linq;

namespace Operations
{
    static class ForceOperations
    {
        public static void NewForce(string file, string name, string faction, bool interactive)
        {
            var force = new Data.Force();
            force.Name = name;
            force.Faction = faction;

            if (interactive)
            {
                var list = GetInteractiveList();
                foreach (var entry in list)
                {
                    force.Miniatures.Add(entry);
                }
            }

            SaveForce(file, force);
        }

        public static void PrintForce(string file)
        {
            Data.Force force = LoadForce(file);

            Console.WriteLine($"{force.Faction} / {force.Name}");
            foreach (var entry in force.Miniatures)
            {
                if (entry.Painted)
                {
                    Console.WriteLine($"- {entry.Name}");
                }
                else
                {
                    Console.WriteLine($"- {entry.Name} (unpainted)");
                }
            }
        }

        public static void ListForces(string directory)
        {
            var forces = new List<string>();
            var files = System.IO.Directory.EnumerateFiles(directory, "*.json");
            foreach (var file in files)
            {
                try
                {
                    var force = LoadForce(file);
                    forces.Add($"{force.Faction} / {force.Name}: {force.Miniatures.Count()}");
                }
                catch (Exception)
                {
                    Console.Error.WriteLine($"Unable to parse force in {System.IO.Path.GetFileName(file)}");
                }
            }
            forces.Sort();
            foreach (var name in forces)
            {
                Console.WriteLine($"- {name}");
            }
        }

        public static Data.Force LoadForce(string file)
        {
            using (var stream = new FileStream(file, FileMode.Open, FileAccess.Read))
            {
                var force = System.Text.Json.JsonSerializer.Deserialize<Data.Force>(stream);
                if (force == null)
                {
                    throw new FormatException($"Unable to read force file: {file}");
                }
                return force;
            }
        }

        private static void SaveForce(string file, Data.Force force)
        {
            force.Miniatures.Sort((a,b) => String.Compare(a.Name, b.Name));
            using (var stream = new FileStream(file, FileMode.Create, FileAccess.Write))
            {
                var options = new System.Text.Json.JsonSerializerOptions();
                options.WriteIndented = true;
                System.Text.Json.JsonSerializer.Serialize<Data.Force>(stream, force, options);
            }
        }

        private static List<Data.ForceMiniature> GetInteractiveList()
        {
            var list = new List<Data.ForceMiniature>();
            while(true)
            {
                Console.Write("Miniature Name: ");
                string? miniName = Console.ReadLine()?.Trim();
                if (miniName == null || String.Equals(miniName, "done"))
                {
                    break;
                }
                Console.Write("Painted: ");
                string? paintedString = Console.ReadLine()?.Trim();
                if (paintedString == null)
                {
                    break;
                }
                if (String.Equals(paintedString, "Y", StringComparison.OrdinalIgnoreCase) ||
                    String.Equals(paintedString, "N", StringComparison.OrdinalIgnoreCase))
                {
                    var miniature = new Data.ForceMiniature();
                    miniature.Name = miniName;
                    miniature.Painted = String.Equals(paintedString, "Y", StringComparison.OrdinalIgnoreCase);
                    list.Add(miniature);
                }
                else
                {
                    Console.Error.WriteLine("Unable to parse painted value!");
                }
            }
            return list;
        }
    }
}