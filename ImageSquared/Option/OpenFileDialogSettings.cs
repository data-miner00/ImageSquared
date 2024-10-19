namespace ImageSquared.Option;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public sealed class OpenFileDialogSettings
{
    public string Filter { get; set; }

    public string Title { get; set; }

    public bool Multiselect { get; set; }
}
