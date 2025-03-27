namespace PartialView.pustok.Models
{
    public class Slider : BaseEntity
    {
        public string Name { get; set; }
        public string Desc { get; set; }
        public string ImgName { get; set; }
        public string ButtonLink { get; set; }
        public string ButtonText { get; set; }
        public int Order { get; set; }
    }
}
