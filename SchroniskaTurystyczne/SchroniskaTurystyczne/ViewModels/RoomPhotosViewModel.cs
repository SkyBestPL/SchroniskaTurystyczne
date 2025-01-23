namespace SchroniskaTurystyczne.ViewModels
{
    public class RoomPhotosViewModel
    {
        public int RoomId { get; set; }
        public string RoomName { get; set; }
        public List<RoomPhotoViewModel> Photos { get; set; }
    }

    public class RoomPhotoViewModel
    {
        public int Id { get; set; }
        public byte[] ThumbnailData { get; set; }
    }
}
