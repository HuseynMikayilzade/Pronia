namespace FrontToBack.Areas.Manage.ViewModels
{
    public class PaginationVm<T>where T : class,new()
    {
        public double TotalPage { get; set; }
        public int PageCount { get; set; }
        public List<T> Items { get; set; }
    }
}
