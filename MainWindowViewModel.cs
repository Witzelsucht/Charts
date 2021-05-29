using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using LiveCharts;
using LiveCharts.Wpf;
using System.Linq;
using System.Windows.Media;

namespace Charts
{
    public class MainWindowViewModel : ViewModelBase
    {
        public List<Measurement> Measurements { get; set; }

        public RelayCommand Load { get; set; }

        private SeriesCollection _seriesViews;

        public SeriesCollection SeriesViews
        {
            get { return _seriesViews; }
            set { Set(ref _seriesViews, value); }
        }


        private bool _isLoaded;
        public bool IsLoaded
        {
            get { return _isLoaded; }
            set { Set(ref _isLoaded, value); }
        }

        private bool _isError;
        public bool IsError
        {
            get { return _isError; }
            set { Set(ref _isError, value); }
        }

        private string _errorMessage;
        public string ErrorMessage
        {
            get { return _errorMessage; }
            set { Set(ref _errorMessage, value); }
        }

        public MainWindowViewModel()
        {
            Measurements = new List<Measurement>();
            Load = new RelayCommand(_Load);
        }

        private void _Load()
        {
            Measurements = new List<Measurement>();
            var dialog = new OpenFileDialog()
            {
                Title = "Select a file",
                Filter = "Text Filest (*.txt)|*.txt"
            };
            if (dialog.ShowDialog() ?? false)
            {
                var path = dialog.FileName;
                var line = "";
                using StreamReader sr = new StreamReader(path);
                while ((line = sr.ReadLine()) != null)
                {
                    try
                    {
                        var arr = line.Replace(" ", string.Empty).Replace(".", ",").Split(';');
                        var measurement = new Measurement()
                        {
                            Voltage = float.Parse(arr[0]),
                            Current = float.Parse(arr[1]),
                            Temperature = float.Parse(arr[2]),
                            Time = int.Parse(arr[3])
                        };
                        Measurements.Add(measurement);
                    }
                    catch (Exception e)
                    {
                        IsError = true;
                        ErrorMessage = e.Message;
                    }
                }
                SeriesViews = new SeriesCollection
                    {
                       new LineSeries
                       {
                           Title = "Voltage",
                           Stroke = new SolidColorBrush(Color.FromRgb(0,0,255)),
                           Fill = new SolidColorBrush(Color.FromArgb(0,0,0,0)),
                           Values = new ChartValues<float>(Measurements.Select(m => m.Voltage).ToArray()),


                       },
                       new LineSeries
                       {
                           Title = "Current",
                           Stroke = new SolidColorBrush(Color.FromRgb(0,255,0)),
                           Fill = new SolidColorBrush(Color.FromArgb(0,0,0,0)),
                           Values = new ChartValues<float>(Measurements.Select(m => m.Current).ToArray())
                       },
                       new LineSeries
                       {
                           Title = "Temperature",
                           Stroke = new SolidColorBrush(Color.FromRgb(255,0,0)),
                           Fill = new SolidColorBrush(Color.FromArgb(0,0,0,0)),
                           Values = new ChartValues<float>(Measurements.Select(m => m.Temperature).ToArray()),
                       }
                    };
                IsLoaded = true;
            }
        }
    }

    public class Measurement
    {
        public float Voltage { get; set; }
        public float Current { get; set; }
        public float Temperature { get; set; }
        public int Time { get; set; }
    }
}
