using ImbaBetWeb.Data;
using ImbaBetWeb.Models;
using ImbaBetWeb.Models.Consts;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json.Linq;
using System.ComponentModel;
using System.Reflection;

namespace ImbaBetWeb.Logic
{
    public class SettingsManager
    {
        private readonly ApplicationContext _context;

        private readonly Dictionary<string, Setting> _cachedSettings;

        public SettingsManager(ApplicationContext context)
        {
            _context = context;
            _cachedSettings = context.Settings.ToDictionary(k => k.Key, v => v);
        }


        public async Task<List<Setting>> GetAllSettingsAsync()
        {
            return await _context.Settings.ToListAsync();
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

        public async Task<bool> SetSettingAsync<T>(string key, T value) where T : IConvertible
        {
            var setting = await GetSettingInternal(key);
            setting.Value = (string)Convert.ChangeType(value, typeof(string));
            var changes  = await _context.SaveChangesAsync();

            // update cache
            _cachedSettings[setting.Key] = setting;

            return changes == 1;
        }

        public async Task<bool> ResetSettingAsync(string key)
        {
            var setting = await GetSettingInternal(key);
            setting.Value = setting.Default;
            var changes = await _context.SaveChangesAsync();

            // update cache
            _cachedSettings[setting.Key] = setting;

            return changes == 1;
        }

        private async Task<Setting> GetSettingInternal(string key)
        {
            // dirty workaround: in case all tables got deleted
            if(!_context.Settings.Any())
            {
                // try to seed
                await SeedSettingsAsync();
            }

            return await _context.Settings.SingleOrDefaultAsync(x => x.Key == key) ?? throw new Exception($"Setting not found: {key}");
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
            var settingsToBeAdded = settings.Where(x => x != null).Select(x => x!).Where(s => !_context.Settings.Any(x => x.Key == s.Key));

			await _context.Settings.AddRangeAsync(settingsToBeAdded);
			await _context.SaveChangesAsync();

		}

	}
}
