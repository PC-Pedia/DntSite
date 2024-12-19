using DntSite.Web.Features.Exports.Models;
using DntSite.Web.Features.RssFeeds.Models;

namespace DntSite.Web.Features.Exports.Services.Contracts;

public interface IPdfExportService : IScopedService
{
    public Task InvalidateExportedFilesAsync(WhatsNewItemType itemType, params IList<int>? docIds);

    public IList<int>? GetAvailableExportedFilesIds(WhatsNewItemType itemType);

    public string GetExportsOutputFolder(WhatsNewItemType itemType);

    public Task<ExportFileLocation> GetExportFileLocationAsync(WhatsNewItemType itemType, int id);

    public Task<string?> CreateSinglePdfFileAsync(WhatsNewItemType itemType,
        int id,
        string title,
        params IList<ExportDocument> docs);
}