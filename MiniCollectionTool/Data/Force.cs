namespace MiniCollectionTool.Data;

class Force
{
    public Force()
    {
        Name = "";
        Faction = "";
        Miniatures = new List<ForceMiniature>();
    }

    public string Name { get; set; }
    public string Faction { get; set; }
    public List<ForceMiniature> Miniatures { get; set; }
}

class ForceMiniature
{
    public ForceMiniature()
    {
        Name = "";
    }

    public string Name { get; set; }
    public bool Painted { get; set; }
}
