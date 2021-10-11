namespace Tel.Egram.Model.Messenger.Catalog.Entries
{
    public class HomeEntryModel : EntryModel
    {
        static HomeEntryModel()
        {
            Instance = new HomeEntryModel
            {
                Id = -1,
                Order = -1,
                Title = "Home"
            };
        }

        private HomeEntryModel()
        {
        }

        public static HomeEntryModel Instance { get; }
    }
}