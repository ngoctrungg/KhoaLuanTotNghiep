using KLTN_E.Data;

namespace KLTN_E.Models
{
    public class PagedList<T>
    {
        public List<T> Items { get; set; }
        public int TotalItems { get; set; }
        public int CurrentPage { get; set; }
        public int PageSize { get; set; }
        public int TotalPages => (int)Math.Ceiling((double)TotalItems / PageSize);
        public int? Loai { get; set; }

        public PagedList(List<T> items, int totalItems, int currentPage, int pageSize, int? loai)
        {
            Items = items;
            TotalItems = totalItems;
            CurrentPage = currentPage;
            PageSize = pageSize;
            Loai = loai;
        }
    }
}
