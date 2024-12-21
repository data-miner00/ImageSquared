namespace ImageSquared.Core.Models;

using System;

public class LoadHistoryRecord
{
    public int Id { get; set; }

    public string FilePath { get; set; }

    public string FileName { get; set; }

    public string FileExtension { get; set; }

    public DateTime CreatedAt { get; set; }

    public long FileSize { get; set; }

    public string FullPath => $"{this.FilePath}/{this.FileName}.{this.FileExtension}";
}
