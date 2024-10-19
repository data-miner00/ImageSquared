namespace ImageSquared.Option;

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public sealed class DefaultSettings
{
    [Range(0, 100)]
    public int SimilarityPercentageThreshold { get; set; }

    public string OpenFileDialogFilter { get; set; }

    public bool Debug { get; set; }

    public string StorageFolderPath { get; set; }
}
