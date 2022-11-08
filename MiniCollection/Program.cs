﻿if (args.Length == 0)
{
    Console.WriteLine("Collection Manager");
    Console.WriteLine("Try the 'list' command");
}
else
{
    if (!System.IO.Directory.Exists(FileHelpers.GetForcesDirectory()))
    {
        System.IO.Directory.CreateDirectory(FileHelpers.GetForcesDirectory());
    }
    switch (args[0])
    {
        case "new":
            bool interactive = false;
            if (args.Length == 2 && String.Equals(args[1], "interactive"))
            {
                interactive = true;
            }
            Operations.CollectionOperations.NewCollection(FileHelpers.GetCollectionFileName(), "My Miniatures", interactive);
            break;
        case "list":
            if (args.Length == 1)
            {
                Operations.CollectionOperations.PrintCollection(FileHelpers.GetCollectionFileName());
            }
            else if (args.Length == 2 && String.Equals(args[1], "unpainted"))
            {
                Operations.CollectionOperations.PrintFilteredCollection(FileHelpers.GetCollectionFileName(), FileHelpers.GetForcesDirectory(), (x) => !x.Painted);
            }
            else if (args.Length == 2 && String.Equals(args[1], "unallocated"))
            {
                Operations.CollectionOperations.PrintFilteredCollection(FileHelpers.GetCollectionFileName(), FileHelpers.GetForcesDirectory(), (x) => false);
            }
            /*else if (args.Length == 2 && String.Equals(args[1], "painted"))
            {
                Operations.CollectionOperations.PrintFilteredCollection(FileHelpers.GetCollectionFileName(), FileHelpers.GetForcesDirectory(), (x) => x.Painted);
            }
            else if (args.Length == 2 && String.Equals(args[1], "allocated"))
            {
                Operations.CollectionOperations.PrintFilteredCollection(FileHelpers.GetCollectionFileName(), FileHelpers.GetForcesDirectory(), (x) => true);
            }*/
            break;
        case "add":
            if (args.Length < 2)
            {
                Console.Error.WriteLine("Miniature name required");
                break;
            }
            if (String.Equals(args[1], "interactive"))
            {
                Operations.CollectionOperations.AddMiniatures(FileHelpers.GetCollectionFileName());
            }
            else
            {
                Operations.CollectionOperations.AddMiniature(FileHelpers.GetCollectionFileName(), args[1]);
            }
            break;
        case "newforce":
            if (args.Length != 3)
            {
                Console.Error.WriteLine("Force name and faction required");
                break;
            }
            Operations.ForceOperations.NewForce(FileHelpers.GetForceFileName(args[1]), args[1], args[2], true);
            break;
        case "listforce":
            if (args.Length != 2)
            {
                Console.Error.WriteLine("Force name required");
                break;
            }
            Operations.ForceOperations.PrintForce(FileHelpers.GetForceFileName(args[1]));
            break;
        case "forces":
            Operations.ForceOperations.ListForces(FileHelpers.GetForcesDirectory());
            break;
        case "count":
            Operations.CollectionOperations.CountCollection(FileHelpers.GetCollectionFileName(), FileHelpers.GetForcesDirectory());
            break;
        default:
            Console.Error.WriteLine($"Unrecognized command: {args[0]}");
            break;
    }
}