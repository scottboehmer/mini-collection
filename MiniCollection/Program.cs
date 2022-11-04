if (args.Length == 0)
{
    Console.WriteLine("Collection Manager");
    Console.WriteLine("Try the 'list' command");
}
else
{
    switch (args[0])
    {
        case "new":
            bool interactive = false;
            if (args.Length == 2 && String.Equals(args[1], "interactive"))
            {
                interactive = true;
            }
            Operations.CollectionOperations.NewCollection("collection.json", "My Miniatures", interactive);
            break;
        case "list":
            Operations.CollectionOperations.PrintCollection("collection.json");
            break;
        case "add":
            if (args.Length < 2)
            {
                Console.Error.WriteLine("Miniature name required");
                break;
            }
            if (String.Equals(args[1], "interactive"))
            {
                Operations.CollectionOperations.AddMiniatures("collection.json");
            }
            else
            {
                Operations.CollectionOperations.AddMiniature("collection.json", args[1]);
            }
            break;
        default:
            Console.Error.WriteLine($"Unrecognized command: {args[0]}");
            break;
    }
}