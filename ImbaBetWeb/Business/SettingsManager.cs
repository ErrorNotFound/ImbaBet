using ImbaBetWeb.DataAccess.Interfaces;
using ImbaBetWeb.Model;
using ImbaBetWeb.Model.Consts;
using System.ComponentModel;
using System.Reflection;

namespace ImbaBetWeb.Business
{
    public class SettingsManager
    {
        private readonly ISettingStore settingStore;
        private Dictionary<string, Setting> _cachedSettings;

        public SettingsManager(ISettingStore store)
        {
            settingStore = store;

            var task = settingStore.GetAllAsync();
            task.Wait();

            _cachedSettings = task.Result.ToDictionary(k => k.Id, v => v);
        }


        public async Task<IEnumerable<Setting>> GetAllSettingsAsync()
        {
            var settings = await settingStore.GetAllAsync();
            _cachedSettings = settings.ToDictionary(k => k.Id, v => v);

            return settings;
        }

        public async Task<T> GetCachedSettingValueAsync<T>(string key) where T : IConvertible
        {
            if(_cachedSettings.TryGetValue(key, out var setting))
            {
                return (T)Convert.ChangeType(setting.Value, typeof(T));
            }
            
            return await GetSettingValueAsync<T>(key);
        }

        public async Task<T> GetSettingValueAsync<T>(string key) where T : IConvertible
        {
            var setting = await GetSettingInternal(key);

            // update cache
            _cachedSettings[setting.Id] = setting;

            return (T)Convert.ChangeType(setting.Value, typeof(T));
        }

        public async Task SetSettingValueAsync<T>(string key, T value) where T : IConvertible
        {
            var setting = await GetSettingInternal(key);
            setting.Value = (string)Convert.ChangeType(value, typeof(string));
            
            await settingStore.UpdateAsync(setting);

            // update cache
            _cachedSettings[setting.Id] = setting;
        }

        public async Task ResetSettingAsync(string key)
        {
            var setting = await GetSettingInternal(key);
            setting.Value = setting.Default;

            await settingStore.UpdateAsync(setting);

            // update cache
            _cachedSettings[setting.Id] = setting;
        }

        private async Task<Setting> GetSettingInternal(string key)
        {
            return await settingStore.GetAsync(key) ?? throw new Exception($"Setting not found: {key}");
        }

		public async Task SeedSettingsAsync()
		{
			var fieldInfos = typeof(SettingNames)
				.GetFields(BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy)
				.Where(fi => fi.IsLiteral && !fi.IsInitOnly).ToList();

			var settings = fieldInfos.Select(fi =>
			{
				var value = fi.GetRawConstantValue();
				if (value == null)
					return null;
				var defaultValueAttribute = fi.GetCustomAttribute<DefaultValueAttribute>();
				var descriptionAttribute = fi.GetCustomAttribute<DescriptionAttribute>();

				var setting = new Setting()
				{
					Id = (string)value,
					Default = (string)(defaultValueAttribute?.Value ?? ""),
					Value = (string)(defaultValueAttribute?.Value ?? ""),
					Description = descriptionAttribute?.Description ?? "",
				};

				return setting;
			}).ToList();

            // only add settings which are not null and don't exist already in db
            var availableSettings = await settingStore.GetAllAsync();
            var settingsToBeAdded = settings.Where(x => x != null).Select(x => x!).Where(s => !availableSettings.Any(x => x.Id == s.Id));

            foreach( var setting in settingsToBeAdded )
            {
                await settingStore.CreateAsync(setting);
            }
		}

	}
}
