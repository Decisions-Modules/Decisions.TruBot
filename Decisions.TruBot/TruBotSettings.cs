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

namespace Decisions.TruBot
{
    [ORMEntity("trubot_settings")]
    [DataContract]
    [Writable]
    public class TruBotSettings : AbstractModuleSettings, IInitializable, INotifyPropertyChanged, IValidationSource
    {
        public TruBotSettings()
        {
            this.EntityName = "TruBot Settings";
        }

        public override string GetEntityShortName()
        {
            return EntityName;
        }

        [ORMField] private string baseUrl;

        [ORMField] private string username;

        [ORMField] private string password;

        [ORMField]
        [PropertyHidden]
        private string token;

        [ORMField]
        [PropertyHidden]
        private string sid;

        [PropertyClassification(0, " ", "TruBot Settings")]
        [ReadonlyEditor]
        [ExcludeInDescription]
        public string ApiKeyMessage
        {
            get
            {
                return
                    "A license for TruBot is needed. Learn more at: https://www.datamatics.com/lp/intelligent-automation/idp-trucap/partner/decisions?utm_source=decisions.com&utm_medium=module_link_click&utm_content=ad1";
            }
            set { }
        }

        [PropertyClassification(1, "Base URL", "TruBot Settings")]
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

        [PropertyClassification(2, "Username", "TruBot Settings")]
        [DataMember]
        [WritableValue]
        public string Username
        {
            get => username;
            set
            {
                username = value;
                OnPropertyChanged(nameof(Username));
            }
        }

        [PasswordText]
        [PropertyClassification(3, "Password", "TruBot Settings")]
        [DataMember]
        [WritableValue]
        public string Password
        {
            get => password;
            set
            {
                password = value;
                OnPropertyChanged(nameof(Password));
            }
        }

        [PropertyHidden]
        [DataMember]
        [WritableValue]
        public string Token
        {
            get => token;
            set { token = value; }
        }

        [PropertyHidden]
        [DataMember]
        [WritableValue]
        public string Sid
        {
            get => sid;
            set { sid = value; }
        }

        public string GetApiUrl()
        {
            return $"{BaseUrl}/CockpitPublicWebApi/api";
        }

        public string GetAccountUrl()
        {
            return $"{GetApiUrl()}/Account";
        }

        public string GetBotUrl()
        {
            return $"{GetApiUrl()}/Bot";
        }

        public string GetBotLogUrl()
        {
            return $"{GetApiUrl()}/BotLog";
        }

        public string GetBotVariableValuesUrl()
        {
            return $"{GetApiUrl()}/parameter/BotVariableValues";
        }

        public string GetProcessInformationUrl()
        {
            return $"{GetApiUrl()}/bot/GetProcessInformation";
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

            bool canAdministrate =
                FolderPermission.CanAdministrate == (FolderPermission.CanAdministrate & permission) ||
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
}