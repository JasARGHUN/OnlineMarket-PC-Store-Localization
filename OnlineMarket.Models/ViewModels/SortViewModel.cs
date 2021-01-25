namespace OnlineMarket.Models.ViewModels
{
    public enum SortState
    {
        NameAsc,
        NameDesc,
        PriceAsc,
        PriceDesc,
        CategoryAsc,
        CategoryDesc
    }

    public class SortViewModel
    {
        public SortState NameSort { get; private set; } // значение для сортировки по имени
        public SortState PriceSort { get; private set; }    // значение для сортировки по возрасту
        public SortState CategorySort { get; private set; }   // значение для сортировки по компании
        public SortState Current { get; private set; }     // текущее значение сортировки

        public SortViewModel(SortState sortOrder)
        {
            NameSort = sortOrder == SortState.NameAsc ? SortState.NameDesc : SortState.NameAsc;
            PriceSort = sortOrder == SortState.PriceAsc ? SortState.PriceDesc : SortState.PriceAsc;
            CategorySort = sortOrder == SortState.CategoryAsc ? SortState.CategoryDesc : SortState.CategoryAsc;
            Current = sortOrder;
        }
    }
}
