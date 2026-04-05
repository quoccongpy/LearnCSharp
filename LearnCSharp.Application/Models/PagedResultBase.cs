namespace LearnCSharp.Application.Models
{
    public class PagedResultBase
    {
        public int CurrentPage { get; set; }

        public int PageCount => (int)Math.Ceiling((double)RowCount / PageSize);

        public int PageSize { get; set; }
        public int RowCount { get; set; }

        public int FirstRowOnPage => (CurrentPage - 1) * PageSize + 1;
        public int LastRowOnPage => Math.Min(CurrentPage * PageSize, RowCount);
    }
}