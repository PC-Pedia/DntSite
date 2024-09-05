﻿using DntSite.Web.Features.AppConfigs.Components;
using DntSite.Web.Features.AppConfigs.Services.Contracts;
using DntSite.Web.Features.Stats.Middlewares.Contracts;
using DntSite.Web.Features.UserProfiles.Models;
using DntSite.Web.Features.UserProfiles.RoutingConstants;
using DntSite.Web.Features.UserProfiles.Services.Contracts;

namespace DntSite.Web.Features.UserProfiles.Components;

[DoNotLogReferrer]
public partial class ForgottenPassword
{
    [CascadingParameter] internal DntAlert Alert { set; get; } = null!;

    [InjectComponentScoped] internal IUsersManagerEmailsService UsersManagerEmailsService { set; get; } = null!;

    [InjectComponentScoped] internal IUserProfilesManagerService UsersService { set; get; } = null!;

    [Inject] internal IAppFoldersService AppFoldersService { set; get; } = null!;

    [SupplyParameterFromForm] public ForgottenPasswordModel Model { get; set; } = new();

    [CascadingParameter] internal ApplicationState ApplicationState { set; get; } = null!;

    protected override void OnInitialized()
    {
        base.OnInitialized();

        AddBreadCrumbs();
    }

    private void AddBreadCrumbs() => ApplicationState.BreadCrumbs.AddRange([UserProfilesBreadCrumbs.Login]);

    private async Task PerformAsync()
    {
        var operationResult = await UsersService.ProcessForgottenPasswordAsync(Model);

        switch (operationResult.Stat)
        {
            case OperationStat.Failed:
                Alert.ShowAlert(AlertType.Danger, title: "خطا!", operationResult.Message);

                break;
            case OperationStat.Succeeded:
                Alert.ShowAlert(AlertType.Success, title: "با تشکر!", operationResult.Message);
                ResetForm();

                break;
        }
    }

    private void ResetForm() => Model = new ForgottenPasswordModel();
}
