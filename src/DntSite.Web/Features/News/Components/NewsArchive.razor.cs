﻿using DntSite.Web.Features.AppConfigs.Components;
using DntSite.Web.Features.Common.Utils.Pagings;
using DntSite.Web.Features.Common.Utils.Pagings.Models;
using DntSite.Web.Features.News.Entities;
using DntSite.Web.Features.News.RoutingConstants;
using DntSite.Web.Features.News.Services.Contracts;
using DntSite.Web.Features.Searches.Services.Contracts;

namespace DntSite.Web.Features.News.Components;

public partial class NewsArchive
{
    private const int ItemsPerPage = 10;

    private string? _basePath;
    private PagedResultModel<DailyNewsItem>? _posts;

    [InjectComponentScoped] internal IDailyNewsItemsService DailyNewsItemsService { set; get; } = null!;

    [CascadingParameter] internal ApplicationState ApplicationState { set; get; } = null!;

    [Parameter] public int? CurrentPage { set; get; }

    [Parameter] public string? Filter { set; get; }

    [InjectComponentScoped] internal ISearchItemsService SearchItemsService { set; get; } = null!;

    protected override async Task OnInitializedAsync()
    {
        await ShowDailyNewsItemsAsync(Filter);
        AddBreadCrumbs();
    }

    private void AddBreadCrumbs() => ApplicationState.BreadCrumbs.AddRange([..NewsBreadCrumbs.DefaultBreadCrumbs]);

    private async Task DoSearchAsync(string gridifyFilter)
    {
        await SearchItemsService.SaveSearchItemAsync(gridifyFilter);

        ApplicationState.NavigateTo(
            $"{NewsRoutingConstants.NewsFilterBase}/{Uri.EscapeDataString(gridifyFilter ?? "*")}/page/1");
    }

    private async Task ShowDailyNewsItemsAsync(string? gridifyFilter)
    {
        CurrentPage ??= 1;

        _basePath = $"{NewsRoutingConstants.NewsFilterBase}/{Uri.EscapeDataString(gridifyFilter ?? "*")}";

        _posts = await DailyNewsItemsService.GetLastPagedDailyNewsItemsIncludeUserAndTagsAsync(new DntQueryBuilderModel
        {
            GridifyFilter = gridifyFilter.NormalizeGridifyFilter(),
            IsAscending = false,
            Page = CurrentPage.Value,
            PageSize = ItemsPerPage,
            SortBy = nameof(DailyNewsItem.Id)
        });
    }
}
