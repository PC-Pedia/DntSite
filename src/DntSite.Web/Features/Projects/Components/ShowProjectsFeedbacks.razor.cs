﻿using DntSite.Web.Common.BlazorSsr.Utils;
using DntSite.Web.Features.AppConfigs.Components;
using DntSite.Web.Features.Common.Utils.Pagings.Models;
using DntSite.Web.Features.Projects.Entities;
using DntSite.Web.Features.Projects.RoutingConstants;
using DntSite.Web.Features.Projects.Services.Contracts;

namespace DntSite.Web.Features.Projects.Components;

public partial class ShowProjectsFeedbacks
{
    private const int PostItemsPerPage = 10;

    private string _pageTitle = "بازخوردهای پروژه‌‌ها";

    private PagedResultModel<ProjectIssue>? _posts;

    [MemberNotNullWhen(returnValue: true, nameof(UserFriendlyName))]
    private bool HasUserFriendlyName => !string.IsNullOrWhiteSpace(UserFriendlyName);

    [Parameter] public string? UserFriendlyName { set; get; }

    [InjectComponentScoped] internal IProjectIssuesService ProjectIssuesService { set; get; } = null!;

    [InjectComponentScoped] internal IProjectsService ProjectsService { set; get; } = null!;

    [Parameter] public int? CurrentPage { set; get; }

    [Parameter] public int? ProjectId { set; get; }

    [CascadingParameter] internal ApplicationState ApplicationState { set; get; } = null!;

    private string GetBasePath()
    {
        if (HasUserFriendlyName)
        {
            return $"{ProjectsRoutingConstants.ProjectsFeedbacks}/{Uri.EscapeDataString(UserFriendlyName)}";
        }

        return ProjectId.HasValue
            ? string.Create(CultureInfo.InvariantCulture,
                $"{ProjectsRoutingConstants.ProjectFeedbacksBase}/{ProjectId.Value}")
            : ProjectsRoutingConstants.ProjectsComments;
    }

    private async Task<string> GetMainTitleAsync()
    {
        if (HasUserFriendlyName)
        {
            return $"آرشیو بازخوردهای پروژه‌های {UserFriendlyName}";
        }

        if (ProjectId.HasValue)
        {
            var title = await GetProjectTitleAsync();

            return $"بازخوردهای پروژه {title}";
        }

        return "بازخوردهای پروژه‌ها";
    }

    private async Task<string?> GetProjectTitleAsync()
        => ProjectId.HasValue ? (await ProjectsService.FindProjectAsync(ProjectId.Value))?.Title : "";

    protected override async Task OnInitializedAsync()
    {
        _pageTitle = await GetMainTitleAsync();

        if (HasUserFriendlyName)
        {
            await ShowUserFeedbacksAsync();
        }
        else if (ProjectId.HasValue)
        {
            await ShowProjectFeedbacksListAsync(ProjectId.Value);
        }
        else
        {
            await ShowAllFeedbacksListAsync();
        }
    }

    private async Task ShowProjectFeedbacksListAsync(int projectId)
    {
        CurrentPage ??= 1;

        _posts = await ProjectIssuesService.GetLastPagedProjectIssuesAsNoTrackingAsync(projectId, CurrentPage.Value - 1,
            PostItemsPerPage);

        AddProjectBreadCrumbs(await GetProjectTitleAsync() ?? "");
    }

    private void AddProjectBreadCrumbs(string name)
        => ApplicationState.BreadCrumbs.AddRange([..ProjectsBreadCrumbs.DefaultProjectBreadCrumbs(name, ProjectId)]);

    private async Task ShowAllFeedbacksListAsync()
    {
        CurrentPage ??= 1;

        _posts = await ProjectIssuesService.GetLastPagedAllProjectsIssuesAsNoTrackingAsync(CurrentPage.Value - 1,
            PostItemsPerPage);

        AddCommentsListBreadCrumbs();
    }

    private void AddCommentsListBreadCrumbs()
        => ApplicationState.BreadCrumbs.AddRange([..ProjectsBreadCrumbs.DefaultBreadCrumbs]);

    private async Task ShowUserFeedbacksAsync()
    {
        CurrentPage ??= 1;

        _posts = await ProjectIssuesService.GetLastPagedProjectIssuesOfUserAsync(UserFriendlyName!,
            CurrentPage.Value - 1, PostItemsPerPage);

        AddUserCommentsBreadCrumbs();
    }

    private void AddUserCommentsBreadCrumbs()
        => ApplicationState.BreadCrumbs.AddRange([
            ..ProjectsBreadCrumbs.DefaultBreadCrumbs, new BreadCrumb
            {
                Title = _pageTitle,
                Url = GetBasePath(),
                GlyphIcon = DntBootstrapIcons.BiPerson
            }
        ]);

    private static string GetPostAbsoluteUrl(ProjectIssue projectIssue)
        => string.Create(CultureInfo.InvariantCulture,
            $"{ProjectsRoutingConstants.ProjectFeedbacksBase}/{projectIssue.ProjectId}/{projectIssue.Id}");
}
