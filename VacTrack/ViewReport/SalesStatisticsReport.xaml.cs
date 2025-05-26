using DatabaseManager;
using LiveChartsCore;
using LiveChartsCore.Kernel.Sketches;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.Painting;
using LiveChartsCore.SkiaSharpView.SKCharts;
using MaterialDesignThemes.Wpf;
using Microsoft.EntityFrameworkCore;
using Microsoft.Win32;
using SkiaSharp;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;


namespace VacTrack.ViewReport
{
    /// <summary>
    /// Interaction logic for SalesStatisticsReport.xaml
    /// </summary>
    public partial class SalesStatisticsReport : Page
    {
        //private SalesStatisticsViewModel ThisViewModel => (SalesStatisticsViewModel)DataContext;
        public SalesStatisticsReport()
        {
            InitTheme();

            InitializeComponent();
        }

        private static void InitTheme()
        {
            if (new PaletteHelper().GetTheme().GetBaseTheme() == BaseTheme.Dark)
                LiveCharts.Configure(settings => settings
                    .AddDefaultMappers()
                    .AddSkiaSharp()
                    .AddDarkTheme());
            else
                LiveCharts.Configure(settings => settings
                    .AddDefaultMappers()
                    .AddSkiaSharp()
                    .AddLightTheme());
        }

        private void ExportBtn_Click(object sender, RoutedEventArgs e)
        {
            LiveCharts.Configure(settings => settings
                   .AddDefaultMappers()
                   .AddSkiaSharp()
                   .AddLightTheme());
            try
            {

                // 1. Настройка диалога сохранения
                var dlg = new SaveFileDialog
                {
                    Title = "Сохранить график как...",
                    Filter =
                    //"BMP Image (*.bmp)|*.bmp|" +
                    //"GIF Image (*.gif)|*.gif|" +
                    //"ICO Image (*.ico)|*.ico|" +
                    "JPEG Image (*.jpg;*.jpeg)|*.jpg;*.jpeg|" +
                    "PNG Image (*.png)|*.png|" +
                    //"WBMP Image (*.wbmp)|*.wbmp|" +
                    "WEBP Image (*.webp)|*.webp|" +
                    //"PKM Image (*.pkm)|*.pkm|" +
                    //"KTX Image (*.ktx)|*.ktx|" +
                    //"ASTC Image (*.astc)|*.astc|" +
                    //"DNG Image (*.dng)|*.dng|" +
                    //"HEIF Image (*.heif;*.heic)|*.heif;*.heic|" +
                    //"AVIF Image (*.avif)|*.avif|" +
                    "All Files (*.*)|*.*",
                    DefaultExt = "png",
                    AddExtension = true,
                    FilterIndex = 2  // по умолчанию PNG
                };

                // 2. Показать диалог
                if (dlg.ShowDialog() != true) return;

                var path = dlg.FileName;
                var ext = Path.GetExtension(path).ToLowerInvariant();

                // 3. Сопоставление расширения с форматом
                SKEncodedImageFormat format = ext switch
                {
                    //".bmp" => SKEncodedImageFormat.Bmp,
                    //".gif" => SKEncodedImageFormat.Gif,
                    //".ico" => SKEncodedImageFormat.Ico,
                    ".jpg" or ".jpeg" => SKEncodedImageFormat.Jpeg,
                    ".png" => SKEncodedImageFormat.Png,
                    //".wbmp" => SKEncodedImageFormat.Wbmp,
                    ".webp" => SKEncodedImageFormat.Webp,
                    //".pkm" => SKEncodedImageFormat.Pkm,
                    //".ktx" => SKEncodedImageFormat.Ktx,
                    //".astc" => SKEncodedImageFormat.Astc,
                    //".dng" => SKEncodedImageFormat.Dng,
                    //".heif" or ".heic" => SKEncodedImageFormat.Heif,
                    //".avif" => SKEncodedImageFormat.Avif,
                    _ => SKEncodedImageFormat.Png
                };

                // 4. Рендер и сохранение
                var skChart = new SKCartesianChart(chartControl)
                {
                    Width = (int)chartControl.ActualWidth,
                    Height = (int)chartControl.ActualHeight
                };

                skChart.SaveImage(path, format);

                MessageBox.Show(
                    $"График сохранён в формате {format.ToString().ToLower()} по адресу:\n{path}",
                    "Экспорт завершён",
                    MessageBoxButton.OK,
                    MessageBoxImage.Information
                );
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                  ex.Message,
                   "Ошибка",
                   MessageBoxButton.OK,
                   MessageBoxImage.Error
               );
            }
            finally
            {
                InitTheme();
            }
        }
    }


    public class SalesStatisticsViewModel : INotifyPropertyChanged
    {
        #region interface implemented 
        public event PropertyChangedEventHandler? PropertyChanged;
        protected void SetProperty<Ts>(ref Ts field, Ts value, [CallerMemberName] string? propertyName = null)
        {
            if (!EqualityComparer<Ts>.Default.Equals(field, value))
            {
                field = value;
                OnPropertyChanged(propertyName);
            }
        }
        protected void OnPropertyChanged([CallerMemberName] string? name = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        #endregion

        private readonly DatabaseContext Db = new();
        private DbSet<Sale> DbSet;
        private ObservableCollection<Sale> Sales;

        private bool _isGroupedByYear = false;
        public bool IsGroupedByYear
        {
            get => _isGroupedByYear;
            set
            {
                if (value != _isGroupedByYear)
                {
                    _isGroupedByYear = value;
                    CreateGraphics();
                    OnPropertyChanged(nameof(IsGroupedByYear));
                }
            }
        }

        private bool _isCountGraph = true;
        public bool IsCountGraph
        {
            get => _isCountGraph;
            set
            {
                if (value != _isCountGraph)
                {
                    _isCountGraph = value;
                    CreateGraphics();
                    OnPropertyChanged(nameof(IsCountGraph));
                }
            }
        }

        private record YearMonth(int Year, int Month);

        private List<IGrouping<YearMonth, Sale>> GroupSales => [.. Sales
            .GroupBy(e => new YearMonth(e.Date.Year, e.Date.Month))
            .OrderBy(g => g.Key.Year).ThenBy(g => g.Key.Month)];

        private List<IGrouping<int, Sale>> GroupSalesByYear => [.. Sales
            .GroupBy(e => e.Date.Year)
            .OrderBy(g => g.Key)
            ];

        private ISeries[] _series = [];
        public ISeries[] Series
        {
            get => _series;
            set => SetProperty(ref _series, value);
        }

        private ICartesianAxis[] _xAxes = [];
        public ICartesianAxis[] XAxes
        {
            get => _xAxes;
            set => SetProperty(ref _xAxes, value);
        }
        public ICartesianAxis[] YAxes { get; set; } = [
            new Axis
            {
                CrosshairLabelsBackground = SKColors.DarkOrange.AsLvcColor(),
                CrosshairLabelsPaint = new SolidColorPaint(SKColors.DarkRed),
                CrosshairPaint = new SolidColorPaint(SKColors.DarkOrange, 1),
                CrosshairSnapEnabled = true // snapping is also supported
            }
        ];

        public ICommand RefreshCommand { get; set; }

        public SalesStatisticsViewModel()
        {
            RefreshCommand = new RelayCommand(Refresh);
            LoadData();

            if (Sales == null || DbSet == null) throw new ArgumentNullException(nameof(Sales));

            CreateGraphics();
        }

        private void Refresh(object obj)
        {
            LoadData();
            CreateGraphics();
        }

        private void LoadData()
        {
            DbSet = Db.Set<Sale>();
            DbSet
                .Include(e => e.Contract)
                    .ThenInclude(c => c != null ? c.Product : null)
                    .ThenInclude(p => p != null ? p.Unit : null)
                .Include(e => e.Contract)
                    .ThenInclude(c => c != null ? c.Counterpartie : null)
                .Load();

            Sales = DbSet.Local.ToObservableCollection();
        }


        private ISeries[] CreateSalesSeries<TKey>(IEnumerable<IGrouping<TKey, Sale>> groupedSales)
        {
            List<ISeries> series = [];
            List<int> totalSaleCount = [];
            List<double> totalSaleSumm = [];

            foreach (var group in groupedSales)
            {
                int count = 0;
                double summ = 0;
                foreach (var s in group)
                {
                    count += s.Count;
                    summ += s.Summ;
                }

                totalSaleCount.Add(count);
                totalSaleSumm.Add(summ);
            }

            if (IsCountGraph)
                series.Add(new LineSeries<int>
                {
                    Name = "Всего продаж",
                    Values = totalSaleCount
                });
            else
                series.Add(new LineSeries<double>
                {
                    Name = "Всего продаж " + Properties.Settings.Default.Currency,
                    Values = totalSaleSumm
                });

            return [.. series];
        }

        private void CreateGraphics()
        {
            if (!IsGroupedByYear)
            {
                Series = CreateSalesSeries(GroupSales);
                XAxes = [ new Axis {
                    Labeler = value => value.ToString("N2"),
                    Labels = [.. GroupSales
                        .Select(g => new DateTime(g.Key.Year, g.Key.Month, 1)
                            .ToString("MMMM\r\nyyyy", new CultureInfo("ru-RU")))]
                }];
            }
            else
            {
                Series = CreateSalesSeries(GroupSalesByYear);
                XAxes = [ new Axis {
                    Labeler = value => value.ToString("N2"),
                    Labels = [.. GroupSalesByYear.Select(g => g.Key.ToString())]
                }];
            }

            foreach (var axis in XAxes)
            {
                axis.MinLimit = null;
                axis.MaxLimit = null;
            }

            foreach (var axis in YAxes)
            {
                axis.MinLimit = null;
                axis.MaxLimit = null;
            }

        }
    }
}