using ImbaBetWeb.DataAccess.Interfaces;
using ImbaBetWeb.Model;
using ImbaBetWeb.Models.Consts;
using System.ComponentModel;
using System.Reflection;

namespace ImbaBetWeb.Logic
{
    public class SettingsManager
    {
        private readonly ISettingStore settingStore;
        private Dictionary<string, Setting> _cachedSettings;

        public SettingsManager(ISettingStore store)
        {
            settingStore = store;

            var task = settingStore.EnsureInitializedAsync();
            task.Wait();

            var task2 = store.GetAllAsync();
            task2.Wait();

            _cachedSettings = task2.Result.ToDictionary(k => k.Key, v => v);
        }


        public async Task<IEnumerable<Setting>> GetAllSettingsAsync()
        {
            var settings = await settingStore.GetAllAsync();
            _cachedSettings = settings.ToDictionary(k => k.Key, v => v);

            return settings;
        }

        public async Task<T> GetCachedSettingAsync<T>(string key) where T : IConvertible
        {
            if(_cachedSettings.TryGetValue(key, out var setting))
            {
                return (T)Convert.ChangeType(setting.Value, typeof(T));
            }
            
            return await GetSettingAsync<T>(key);
        }

        public async Task<T> GetSettingAsync<T>(string key) where T : IConvertible
        {
            var setting = await GetSettingInternal(key);

            // update cache
            _cachedSettings[setting.Key] = setting;

            return (T)Convert.ChangeType(setting.Value, typeof(T));
        }

        public async Task SetSettingAsync<T>(string key, T value) where T : IConvertible
        {
            var setting = await GetSettingInternal(key);
            setting.Value = (string)Convert.ChangeType(value, typeof(string));
            
            await settingStore.UpdateAsync(setting);

            // update cache
            _cachedSettings[setting.Key] = setting;
        }

        public async Task ResetSettingAsync(string key)
        {
            var setting = await GetSettingInternal(key);
            setting.Value = setting.Default;

            await settingStore.UpdateAsync(setting);

            // update cache
            _cachedSettings[setting.Key] = setting;
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
					Key = (string)value,
					Default = (string)(defaultValueAttribute?.Value ?? ""),
					Value = (string)(defaultValueAttribute?.Value ?? ""),
					Description = descriptionAttribute?.Description ?? "",
				};

				return setting;
			}).ToList();

            // only add settings which are not null and don't exist already in db
            var availableSettings = await settingStore.GetAllAsync();
            var settingsToBeAdded = settings.Where(x => x != null).Select(x => x!).Where(s => !availableSettings.Any(x => x.Key == s.Key));

            foreach( var setting in settingsToBeAdded )
            {
                await settingStore.CreateAsync(setting);
            }
		}

	}
}
