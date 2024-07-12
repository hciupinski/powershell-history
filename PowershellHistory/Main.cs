using ManagedCommon;
using Microsoft.PowerToys.Settings.UI.Library;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Wox.Plugin;
using Wox.Plugin.Logger;

namespace Community.PowerToys.Run.Plugin.PowershellHistory
{
    public class Main : IPlugin, IPluginI18n, IContextMenu, ISettingProvider, IReloadable, IDisposable, IDelayedExecutionPlugin
    {
        private const string Setting = nameof(Setting);

        private const string PowershellScript = "Get-Content (Get-PSReadlineOption).HistorySavePath";

        // current value of the setting
        private bool _setting;

        private PluginInitContext _context;

        private string _iconPath;

        private bool _disposed;

        public string Name => Properties.Resources.plugin_name;

        public string Description => Properties.Resources.plugin_description;

        public static string PluginID => "40f47c8d20524d779baac89669570d92";

        // TODO: add additional options (optional)
        public IEnumerable<PluginAdditionalOption> AdditionalOptions => new List<PluginAdditionalOption>()
        {
            new PluginAdditionalOption()
            {
                Key = Setting,
                DisplayLabel = Properties.Resources.plugin_setting,
                Value = false,
            },
        };

        public void UpdateSettings(PowerLauncherPluginSettings settings)
        {
            _setting = settings?.AdditionalOptions?.FirstOrDefault(x => x.Key == Setting)?.Value ?? false;
        }

        // TODO: return context menus for each Result (optional)
        public List<ContextMenuResult> LoadContextMenus(Result selectedResult)
        {
            // return new List<ContextMenuResult>(0);
            Log.Info("LoadContextMenus", GetType());

            if (selectedResult?.ContextData is string path)
            {
                return
                [
                    new ContextMenuResult
                    {
                        PluginName = Name,
                        Title = "Copy (Enter)",
                        FontFamily = "Segoe Fluent Icons,Segoe MDL2 Assets",
                        Glyph = "\xE8C8", // Copy
                        AcceleratorKey = Key.Enter,
                        Action = _ => CopyToClipboard(path),
                    },
                ];
            }

            return [];
        }

        // TODO: return query results
        public List<Result> Query(Query query, bool delayedExecution)
        {
            ArgumentNullException.ThrowIfNull(query);

            var scriptResult = PowerShellExtensions.RunPowerShellScript(PowershellScript);

            var history = scriptResult.Split(new[] { "\r\n", "\r", "\n" }, StringSplitOptions.None)
                    .ToList()
                    .Distinct();

            var results = new List<Result>();

            // empty query
            if (!string.IsNullOrEmpty(query.Search))
            {
                var historyResult = history
                    .Where(x => x.Contains(query.Search))
                    .Select(command => new Result()
                    {
                        Title = command,
                        SubTitle = Description,
                        QueryTextDisplay = query.Search,
                        IcoPath = _iconPath,
                        Action = action =>
                        {
                            return true;
                        },
                        ContextData = command
                    })
                    .ToList();

                if (!historyResult.Any())
                {
                    results.Add(new Result
                    {
                        Title = "No results.",
                        SubTitle = Description,
                        QueryTextDisplay = string.Empty,
                        IcoPath = _iconPath,
                        Action = action =>
                        {
                            return true;
                        },
                    });

                    return results;
                }

                return historyResult;
            }
            else
            {
                var lastHistoryResults = history
                    .Take(50)
                    .Select(command => new Result()
                    {
                        Title = command,
                        SubTitle = Description,
                        QueryTextDisplay = query.Search,
                        IcoPath = _iconPath,
                        Action = action =>
                        {
                            return true;
                        },
                        ContextData = command
                    })
                    .ToList();

                return lastHistoryResults;
            }

            return results;
        }

        // TODO: return delayed query results (optional)
        public List<Result> Query(Query query)
        {
            ArgumentNullException.ThrowIfNull(query);

            var results = new List<Result>();

            results.Add(new Result
            {
                Title = "Searching..",
                SubTitle = Description,
                QueryTextDisplay = string.Empty,
                IcoPath = _iconPath,
                Action = action =>
                {
                    return true;
                },
            });

            return results;
        }

        public void Init(PluginInitContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _context.API.ThemeChanged += OnThemeChanged;
            UpdateIconPath(_context.API.GetCurrentTheme());
        }

        public string GetTranslatedPluginTitle()
        {
            return Properties.Resources.plugin_name;
        }

        public string GetTranslatedPluginDescription()
        {
            return Properties.Resources.plugin_description;
        }

        private void OnThemeChanged(Theme oldtheme, Theme newTheme)
        {
            UpdateIconPath(newTheme);
        }

        private void UpdateIconPath(Theme theme)
        {
            if (theme == Theme.Light || theme == Theme.HighContrastWhite)
            {
                _iconPath = "Images/PowershellHistory.light.png";
            }
            else
            {
                _iconPath = "Images/PowershellHistory.dark.png";
            }
        }

        public Control CreateSettingPanel()
        {
            throw new NotImplementedException();
        }

        public void ReloadData()
        {
            if (_context is null)
            {
                return;
            }

            UpdateIconPath(_context.API.GetCurrentTheme());
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        private static bool CopyToClipboard(string? value)
        {
            if (value != null)
            {
                Clipboard.SetText(value);
            }

            return true;
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed && disposing)
            {
                if (_context != null && _context.API != null)
                {
                    _context.API.ThemeChanged -= OnThemeChanged;
                }

                _disposed = true;
            }
        }
    }
}
