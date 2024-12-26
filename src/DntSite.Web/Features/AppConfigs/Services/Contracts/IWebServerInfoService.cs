using DntSite.Web.Features.AppConfigs.Models;

namespace DntSite.Web.Features.AppConfigs.Services.Contracts;

public interface IWebServerInfoService : IScopedService
{
    public Task<WebServerInfoModel> GetWebServerInfoAsync();
}
