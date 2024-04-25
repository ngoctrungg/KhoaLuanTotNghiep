namespace KLTN_E.Data
{
    public class Comment
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Content { get; set; }
        public DateTime CreatedDate { get; set; }
        public int ProductId { get; set; }
        public HangHoa? Product { get; set; }
        public string UserId { get; set; }
        public KhachHang? User { get; set; }
    }
}
