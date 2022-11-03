namespace Data
{
    class Collection
    {
        public Collection()
        {
            Name = "";
            Miniatures = new List<CollectionMiniature>();
        }

        public string Name { get; set; }
        public List<CollectionMiniature> Miniatures { get; set; }
    }

    class CollectionMiniature
    {
        public CollectionMiniature()
        {
            Name = "";
        }

        public string Name { get; set; }
        public uint CountInCollection { get; set; }
        public uint PendingCount { get; set; }
    }
}