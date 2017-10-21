using Prime.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GalaSoft.MvvmLight.Command;
using Prime.Utility;

namespace Prime.Ui.Wpf.ViewModel
{
    public class ServiceEditViewModel : VmBase, IDataErrorInfo
    {
        private readonly Debouncer _debouncer;
        private bool _initialCheck = true;

        public ServiceEditViewModel() { }

        public ServiceEditViewModel(INetworkProviderPrivate provider)
        {
            DeleteCommand = new RelayCommand(Delete);
            _debouncer = new Debouncer();
            Service = provider;
            Configuration = Service.GetApiConfiguration;

            if (Configuration == null)
                throw new ArgumentException("No API Configuration object for " + Service.GetType());

            UserKey = UserContext.Current.GetApiKey(Service);

            if (UserKey != null)
            {
                _apiKey = UserKey.Key;
                _apiSecret = UserKey.Secret;
                _apiExtra1 = UserKey.Extra;
                _apiName = UserKey.Name;
                DecideTest(1);
            }
            else
            {
                _apiName = Service.Title;
                _initialCheck = false;
            }

            this.PropertyChanged += ServiceEditViewModel_PropertyChanged;
        }

        private void ServiceEditViewModel_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            var prop = e.PropertyName;
            if (prop != nameof(ApiKey) && prop != nameof(ApiSecret) && prop != nameof(ApiExtra1) && prop != nameof(ApiName))
                return;

            DecideTest();
        }

        private void DecideTest(int debounceInterval = 700)
        {
            StatusText = "";
            StatusResult = false;

            var allin = this[nameof(ApiName)] == null && this[nameof(ApiKey)] == null && this[nameof(ApiSecret)] == null &&
                        this[nameof(ApiExtra1)] == null;
            if (!allin)
                return;

            _debouncer.Debounce(debounceInterval, o => CheckKeys());
        }

        private void CheckKeys()
        {
            StatusText = "Checking the keys now...";

            var apikey = new ApiKey(Service.Network, _apiName, _apiKey, _apiSecret, _apiExtra1);
            var t = ApiCoordinator.TestApiAsync(Service, new ApiTestContext(apikey));
            t.ContinueWith(task => ApiKeyCheckResult(task, apikey));
        }


        private void Delete()
        {
            var keys = UserContext.Current.ApiKeys;
            if (UserKey == null)
                return;

            if (!UserKey.Delete(UserContext.Current).IsSuccess)
            {
                StatusText = "Unable to remove this key.";
                StatusResult = false;
                return;
            }

            keys.Remove(UserKey);

            UserKey = null;
            ApiKey = null;
            ApiSecret = null;
            ApiExtra1 = null;
            ApiName = null;

            StatusText = "Key removed.";
            StatusResult = true;
        }

        private void ApiKeyCheckResult(Task<ApiResponse<bool>> x, ApiKey key)
        {
            var ok = x.Result.Response;
            StatusResult = ok;

            if (ok && _initialCheck)
            {
                StatusText = ok ? "Successfully connected, these keys are working." : "Unable to connect, check you've entered the information correctly.";
                _initialCheck = false;
                return;
            }

            StatusText = ok ? "Successfully connected, the keys have been saved." : "Unable to connect, check you've entered the information correctly.";
            
            if (!ok)
                return;
            
            var keys = UserContext.Current.ApiKeys;

            if (UserKey != null)
                keys.Remove(UserKey);

            UserKey = key;

            keys.Add(key);
            keys.Save();
        }

        private string _apiName;
        private string _apiKey;
        private string _apiSecret;
        private string _apiExtra1;
        private string _statusText;
        private bool _statusResult;

        public INetworkProviderPrivate Service { get; private set; }

        public ApiKey UserKey { get; private set; }

        public ApiConfiguration Configuration { get; private set; }

        public RelayCommand DeleteCommand { get;  }

        public bool IsDeleteVisible => UserKey != null;

        private ProviderData _providerData;
        public ProviderData ProviderData => _providerData ?? (_providerData = UserContext.Current.Data(Service));

        public string ApiName
        {
            get => this._apiName;
            set => Set(ref _apiName, value, x=> x.Trim());
        }

        public string ApiKey
        {
            get => this._apiKey;
            set => Set(ref _apiKey, value, x => x.Trim());
        }

        public string ApiSecret
        {
            get => this._apiSecret;
            set => Set(ref _apiSecret, value, x => x.Trim());
        }

        public string ApiExtra1
        {
            get => this._apiExtra1;
            set => Set(ref _apiExtra1, value, x => x.Trim());
        }

        public string StatusText
        {
            get => _statusText;
            private set => Set(ref _statusText, value);
        }

        public bool StatusResult
        {
            get => _statusResult;
            private set => Set(ref _statusResult, value);
        }

        public string Error => null;

        public string this[string propertyName]
        {
            get
            {
                switch (propertyName)
                {
                    case nameof(ApiName):
                        return IsNameSane(_apiName);
                    case nameof(ApiKey):
                        return !IsKeySane(_apiKey) ? "Required" : null;
                    case nameof(ApiSecret):
                        return Configuration.HasSecret && !IsKeySane(_apiSecret) ? "Required" : null;
                    case nameof(ApiExtra1):
                        return Configuration.HasExtra && !IsKeySane(_apiExtra1) ? "Required" : null;
                    default:
                        return null;
                }
            }
        }

        private string IsNameSane(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                return "Required";

            name = name.Trim();
            var exists = UserContext.Current.ApiKeys.Any(x => x.Name.Equals(name, StringComparison.OrdinalIgnoreCase) && UserKey?.Id != x.Id);
            return exists ? "Name already exists" : null;
        }

        private bool IsKeySane(string key)
        {
            if (string.IsNullOrWhiteSpace(key))
                return false;
            key = key.Trim();
            if (key.Contains(" "))
                return false;
            return true;
        }
    }
}
