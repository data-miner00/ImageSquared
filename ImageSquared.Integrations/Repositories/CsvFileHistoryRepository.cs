namespace ImageSquared.Integrations.Repositories;

using CsvHelper;
using ImageSquared.Core;
using ImageSquared.Core.Models;
using ImageSquared.Core.Repositories;
using ImageSquared.Integrations.CsvMappers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using static System.Globalization.CultureInfo;

public sealed class CsvFileHistoryRepository : IHistoryRepository<LoadHistoryRecord>
{
    private readonly string csvFilePath;
    private readonly List<LoadHistoryRecord> records = [];

    public CsvFileHistoryRepository(string csvFilePath)
    {
        this.csvFilePath = Guard.ThrowIfNullOrWhitespace(csvFilePath);
    }

    public Task AddAsync(string filePath, CancellationToken cancellationToken = default)
    {
        var uri = new FileInfo(filePath);

        var record = new LoadHistoryRecord
        {
            Id = this.records.Count,
            FilePath = uri.DirectoryName ?? string.Empty,
            FileName = uri.Name,
            FileExtension = uri.Extension,
            CreatedAt = DateTime.Now,
        };

        this.records.Add(record);
        Save(this.csvFilePath, this.records);

        return Task.CompletedTask;
    }

    public Task<IEnumerable<LoadHistoryRecord>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        this.records.AddRange(Load(this.csvFilePath));
        return Task.FromResult<IEnumerable<LoadHistoryRecord>>(this.records);
    }

    private static List<LoadHistoryRecord> Load(string path)
    {
        List<LoadHistoryRecord> items;

        if (!File.Exists(path))
        {
            items = [];
            Console.WriteLine($"File to {path} not found");
        }
        else
        {
            using var streamReader = new StreamReader(path);
            using var csvReader = new CsvReader(streamReader, InvariantCulture);

            csvReader.Context.RegisterClassMap<LoadHistoryRecordMapper>();
            items = csvReader.GetRecords<LoadHistoryRecord>().ToList();
        }

        return items;
    }

    private static int Save(string path, List<LoadHistoryRecord> items)
    {
        try
        {
            using var streamWriter = new StreamWriter(path);
            using var csvWriter = new CsvWriter(streamWriter, InvariantCulture);

            csvWriter.WriteRecords(items);

            return 0;
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error: {0}", ex.Message);
            return -1;
        }
    }
}
