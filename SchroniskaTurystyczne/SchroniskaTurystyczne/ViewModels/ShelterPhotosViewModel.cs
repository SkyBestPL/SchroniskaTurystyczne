namespace SchroniskaTurystyczne.ViewModels
{
    public class ShelterPhotosViewModel
    {
        public int ShelterId { get; set; }
        public string ShelterName { get; set; }
        public List<PhotoViewModel> Photos { get; set; }
    }

    public class PhotoViewModel
    {
        public int Id { get; set; }
        public byte[] PhotoData { get; set; }
        public byte[] ThumbnailData { get; set; }
    }
}
