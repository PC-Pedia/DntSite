﻿namespace DntSite.Web.Features.Exports.Models;

[DebuggerDisplay(value: "{Body}")]
public class ExportComment : TreeItem<int?>
{
    public required string Body { set; get; }

    public required string PersianDate { set; get; }

    public required string Author { set; get; }
}
