using DntSite.Web.Common.BlazorSsr.Utils;
using DntSite.Web.Features.AppConfigs.Components;
using DntSite.Web.Features.Stats.Entities;
using DntSite.Web.Features.Stats.RoutingConstants;
using DntSite.Web.Features.Stats.Services.Contracts;

namespace DntSite.Web.Features.Stats.Components;

public partial class ShowAllLocalPageReferrers
{
    private string? _pageTitle;

    private SiteReferrer? _referrer;

    [Parameter] public string? Url { get; set; }

    [Parameter] public int? CurrentPage { set; get; }

    private int PageNumber => CurrentPage ?? 1;

    [CascadingParameter] internal ApplicationState ApplicationState { set; get; } = null!;

    [InjectComponentScoped] internal ISiteReferrersService SiteReferrersService { set; get; } = null!;

    protected override async Task OnInitializedAsync()
    {
        await SetTitleAsync();
        AddBreadCrumbs();
    }

    private async Task SetTitleAsync()
    {
        _referrer = await SiteReferrersService.FindLocalReferrerAsync(Url);

        if (_referrer is null)
        {
            ApplicationState.NavigateToNotFoundPage();

            return;
        }

        _pageTitle = string.Create(CultureInfo.InvariantCulture,
            $"صفحات مرتبط «{_referrer.DestinationTitle}»، صفحه: {CurrentPage ?? 1}");
    }

    private void AddBreadCrumbs()
        => ApplicationState.BreadCrumbs.AddRange([
            new BreadCrumb
            {
                Title = _referrer?.DestinationTitle ?? "مطالب",
                GlyphIcon = DntBootstrapIcons.BiNewspaper,
                Url = _referrer?.DestinationUrl ?? "/"
            },
            new BreadCrumb
            {
                Title = string.Create(CultureInfo.InvariantCulture, $"صفحات مرتبط، صفحه: {CurrentPage ?? 1}"),
                GlyphIcon = DntBootstrapIcons.BiSearch,
                Url = "/".CombineUrl(StatsRoutingConstants.MoreLocalPageReferrersBase)
                    .CombineUrl(Uri.EscapeDataString(Url ?? ""))
            }
        ]);
}
