static class FileHelpers
{
    public static string GetCollectionFileName()
    {
        return "collection.json";
    }

    public static string GetForcesDirectory()
    {
        return "forces";
    }

    public static string GetForceFileName(string forceName)
    {
        var filename = forceName.ToLower().Replace(' ','-').Replace("'", "");
        return $"{GetForcesDirectory()}/{filename}.json";
    }
}