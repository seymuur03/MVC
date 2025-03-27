namespace PartialView.pustok.Helpers
{
    public class PaginatedList<T>:List<T>
    {
        public int CurentPage { get; set; }
        public int PageCount { get; set; }
        public int First { get; set; }
        public int Last { get; set; }
        public bool HasNext { get; set; }
        public bool HasPrevious { get; set; }
        public PaginatedList(List<T>items,int currentPage,int pageCount)
        {
            this.AddRange(items);
            CurentPage = currentPage;    
            PageCount = pageCount;
            HasNext = currentPage < pageCount;
            HasPrevious = currentPage > 1;  
            var start = currentPage - 2;
            var end = currentPage + 2;

            if (start < 1)
            {
                start = 1;
                end = Math.Min(pageCount, 5); // genre sayi az olanda menfi ededlerde yaranirdi paginationda bunu yazdim qarsini almaq ucun
            }

            if (end > pageCount)
            {
                end = pageCount;
                start = Math.Max(1, end - 4);
            }
            First = start;
           Last = end;
        }

        public static PaginatedList<T> PaginationMethod(IQueryable<T> query,int take,int currentPage)
        {
            var count = query.Count();
            var datas = query.Skip((currentPage - 1) * take).Take(take).ToList();
            var pageCount = (int)Math.Ceiling((decimal)count / take);
            return new PaginatedList<T>(datas,currentPage,pageCount);
        }
    }
}
