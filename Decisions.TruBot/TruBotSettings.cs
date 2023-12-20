using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.Serialization;
using DecisionsFramework;
using DecisionsFramework.Data.ORMapper;
using DecisionsFramework.Design.ConfigurationStorage.Attributes;
using DecisionsFramework.Design.Properties;
using DecisionsFramework.Design.Properties.Attributes;
using DecisionsFramework.ServiceLayer;
using DecisionsFramework.ServiceLayer.Actions;
using DecisionsFramework.ServiceLayer.Actions.Common;
using DecisionsFramework.ServiceLayer.Services.Accounts;
using DecisionsFramework.ServiceLayer.Services.Administration;
using DecisionsFramework.ServiceLayer.Services.Folder;
using DecisionsFramework.ServiceLayer.Utilities;

namespace Decisions.TruBot;

[ORMEntity("trubot_settings")]
[DataContract]
[Writable]
public class TruBotSettings : AbstractModuleSettings, IInitializable, INotifyPropertyChanged, IValidationSource
{
    internal const string TRUBOT_IMAGES_PATH = "../wwwroot/Content/CustomModuleImages/Decisions.TruBot/|trubot.svg";
    
    public TruBotSettings()
    {
        this.EntityName = "TruBot Settings";
    }
    
    [ORMField]
     private string baseUrl = "http://localhost:56498/CockpitPublicWebApi/api";

     [PropertyClassification(0, "Base URL", "TruBot Settings")]
     [DataMember]
     [WritableValue]
     public string BaseUrl
     {
         get => baseUrl;
         set
         {
             baseUrl = value;
             OnPropertyChanged(nameof(BaseUrl));
         }
     }
     
     [PropertyClassification(1, " ", "TruBot Settings")]
     [ReadonlyEditor]
     [ExcludeInDescription]
     public string ApiKeyMessage
     {
         get
         {
             return "";
         }
         set { }
     }

     public string GetBaseUrl(string? overrideBaseUrl) {
        string url = baseUrl;
        if (!string.IsNullOrEmpty(overrideBaseUrl)) 
            url = overrideBaseUrl;

        return url;
    }

    public string GetBaseAccountUrl(string? overrideBaseUrl) 
    {
        return $"{GetBaseUrl(overrideBaseUrl)}/Account";
    }
    
    public string GetBaseBotUrl(string? overrideBaseUrl) 
    {
        return $"{GetBaseUrl(overrideBaseUrl)}/Bot";
    }

    public string GetBaseBotLogUrl(string? overrideBaseUrl)
    {
        return $"{GetBaseUrl(overrideBaseUrl)}/BotLog";
    }
    
    public string GetBotVariableValuesUrl(string? overrideBaseUrl)
    {
        return $"{GetBaseUrl(overrideBaseUrl)}/parameter/BotVariableValues";
    }
    
    public string GetProcessInformationUrl(string? overrideBaseUrl)
    {
        return $"{GetBaseUrl(overrideBaseUrl)}/bot/GetProcessInformation";
    }

    public event PropertyChangedEventHandler PropertyChanged;

    public void Initialize()
    {
        ModuleSettingsAccessor<TruBotSettings>.GetSettings();
    }

    private void OnPropertyChanged(string propertyName)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
    
    public ValidationIssue[] GetValidationIssues()
    {
        List<ValidationIssue> issues = new List<ValidationIssue>();

        return issues.ToArray();
    }
    
    public override BaseActionType[] GetActions(AbstractUserContext userContext, EntityActionType[] types)
    {
        List<BaseActionType> actions = new List<BaseActionType>();

        Account userAccount = userContext.GetAccount();

        FolderPermission permission = FolderService.GetAccountEffectivePermissionInternal(
            new SystemUserContext(), this.EntityFolderID, userAccount.AccountID);

        bool canAdministrate = FolderPermission.CanAdministrate == (FolderPermission.CanAdministrate & permission) ||
                                userAccount.GetUserRights<PortalAdministratorModuleRight>() != null ||
                                userAccount.IsAdministrator();

        if (canAdministrate)
        {
            actions.Add(new EditEntityAction(typeof(TruBotSettings), "Edit", "Edits TruBot Module Settings") 
            {
                IsDefaultGridAction = true,
                OkActionName = "SAVE",
                CancelActionName = null
            });
        }

        return actions.ToArray();
    }
    
    private void SaveSettings(AbstractUserContext userContext, object obj)
    {
        TruBotSettings settings = obj as TruBotSettings;
        ORM<TruBotSettings> orm = new ORM<TruBotSettings>();
        orm.Store(settings);
    }
}