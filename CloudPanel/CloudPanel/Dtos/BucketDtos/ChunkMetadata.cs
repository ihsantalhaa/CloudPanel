namespace CloudPanel.WebApi.Dtos.BucketDtos
{
    public class ChunkMetadata
    {
        public string FileId { get; set; }        // Benzersiz dosya kimliği
        public string FileName { get; set; }      // Orijinal dosya adı
        public long ChunkIndex { get; set; }      // Mevcut parça indeksi
        public long TotalChunks { get; set; }     // Toplam parça sayısı
        public long FileSize { get; set; }        // Toplam dosya boyutu
    }
}
