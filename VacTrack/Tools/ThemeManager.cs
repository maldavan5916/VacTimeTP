using MaterialDesignColors.ColorManipulation;
using MaterialDesignThemes.Wpf;
using System.Windows.Media;

namespace VacTrack.Tools
{
    class ThemeManager
    {
        private static readonly PaletteHelper _paletteHelper = new();

        public static void SaveTheme(BaseTheme currentTheme = BaseTheme.Inherit)
        {
            // 1. Получаем текущую тему
            Theme theme = _paletteHelper.GetTheme();  // :contentReference[oaicite:0]{index=0}

            // 2. Сохраняем базовую тему
            Properties.Settings.Default.AppTheme = currentTheme switch
            {
                BaseTheme.Dark => "Dark",
                BaseTheme.Light => "Light",
                BaseTheme.Inherit => "Inherit",
                _ => "Inherit"
            };

            // 3. Сохраняем ключевые цвета (Mid.Primary → ARGB)
            Color primaryMid = theme.PrimaryMid.Color;
            Color secondaryMid = theme.SecondaryMid.Color;
            Properties.Settings.Default.PrimaryColor = primaryMid.ToString();
            Properties.Settings.Default.SecondaryColor = secondaryMid.ToString();

            // 4. Сохраняем ColorAdjustment
            if (theme.ColorAdjustment is not null)
            {
                Properties.Settings.Default.DesiredContrastRatio = theme.ColorAdjustment.DesiredContrastRatio;
                Properties.Settings.Default.Contrast = theme.ColorAdjustment.Contrast.ToString();
            }
            else
            {
                Properties.Settings.Default.DesiredContrastRatio = 0;
                Properties.Settings.Default.Contrast = "None";
            }

            // 5. Физически сохраняем всё в файл настроек
            Properties.Settings.Default.Save();             // :contentReference[oaicite:1]{index=1}
        }

        public static void ApplySavedTheme()
        {
            // 1. Получаем текущую тему
            Theme theme = _paletteHelper.GetTheme();

            // 2. Восстанавливаем BaseTheme
            if (Enum.TryParse<BaseTheme>(Properties.Settings.Default.AppTheme, out var savedBase))
                theme.SetBaseTheme(savedBase);

            // 3. Восстанавливаем ключевые цвета
            Color primary = (Color)ColorConverter.ConvertFromString(Properties.Settings.Default.PrimaryColor);
            Color secondary = (Color)ColorConverter.ConvertFromString(Properties.Settings.Default.SecondaryColor);

            theme.SetPrimaryColor(primary);
            theme.SetSecondaryColor(secondary);

            // 4. Восстанавливаем ColorAdjustment
            if (
                Properties.Settings.Default.DesiredContrastRatio != 0 &&
                Properties.Settings.Default.Contrast != "None"
                )
            {
                float desiredRatio = Properties.Settings.Default.DesiredContrastRatio;
                theme.ColorAdjustment ??= new ColorAdjustment();
                theme.ColorAdjustment.DesiredContrastRatio = desiredRatio;

                if (Enum.TryParse<Contrast>(Properties.Settings.Default.Contrast, out var savedContrast))
                    theme.ColorAdjustment.Contrast = savedContrast;
            }
            else
                theme.ColorAdjustment = null;

            // 5. Применяем собранную тему
            _paletteHelper.SetTheme(theme);
        }
    }
}
