namespace ImageSquared.Integrations.CsvMappers;

using CsvHelper.Configuration;
using ImageSquared.Core.Models;

internal sealed class LoadHistoryRecordMapper : ClassMap<LoadHistoryRecord>
{
    public LoadHistoryRecordMapper()
    {
        this.Map(m => m.Id).Name("Id");
        this.Map(m => m.FilePath).Name("FilePath");
        this.Map(m => m.FileName).Name("FileName");
        this.Map(m => m.FileExtension).Name("FileExtension");
        this.Map(m => m.CreatedAt).Name("CreatedAt");
        this.Map(m => m.FileSize).Name("FileSize");
    }
}
