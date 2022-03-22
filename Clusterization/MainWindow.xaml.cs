using Clusterization._2DArray;
using Clusterization.ReportModel;
using Microsoft.Win32;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Shapes;

namespace Clusterization
{
    public partial class MainWindow : Window
    {
        int _inputNodes = 3;
        int _length = 100;
        int _defaultTestCount = 500;
        int _testCount;

        bool _draw = true;
        bool _isProcessing = false;

        Progress<ParallelReport> progress;

        public ObservableCollection<ObservableCollection<Rectangle>> ColorPalette { get; set; }
        object _lock = new object();

        SOMNetwork _network;
        Operations _operations;
        Stopwatch _stopwatch;


        public MainWindow()
        {
            InitializeComponent();

            progress = new Progress<ParallelReport>();
            progress.ProgressChanged += ReportProgress;

            _stopwatch = new Stopwatch();

            _network = new SOMNetwork(_inputNodes, _length);
            _operations = new Operations();

            ColorPalette = new ObservableCollection<ObservableCollection<Rectangle>>();

            _testCount = _defaultTestCount;
            double size = 900 / (double)_length;
            for (int i = 0; i < _length; i++)
            {
                ColorPalette.Add(new ObservableCollection<Rectangle>());
                for (int j = 0; j < _length; j++)
                {
                    Rectangle rectangle = new Rectangle
                    {
                        Height = size,
                        Width = size,
                        Fill = Brushes.Transparent
                    };

                    Canvas.SetTop(rectangle, i * size);
                    Canvas.SetLeft(rectangle, j * size);

                    ColorPalette[i].Add(rectangle);
                    Image.Children.Add(rectangle);
                }
            }
            Update();
        }

        private void ReportProgress(object sender, ParallelReport e)
        {
            _network.Iteration = e.TestCount;
            Update(e.Weights);
        }

        private async void Learn_Click(object sender, RoutedEventArgs e)
        {
            _isProcessing = !_isProcessing;

            if (!_isProcessing)
            {
                return;
            }

            if (!(bool)IsParallel.IsChecked)
            {
                _stopwatch.Start();

                while (_isProcessing && _defaultTestCount - _network.Iteration > 0)
                {
                    await Task.Run(() => _network.TrainingAsync());
                    Update();
                }

                _stopwatch.Stop();
            }
            else
            {
                _stopwatch.Start();
                await Task.Run(() => _network.TrainingParallel(_defaultTestCount, progress));
                _stopwatch.Stop();

                //_stopwatch.Start();
                //Parallel.For(0, _testCount, i =>
                //{
                //   _network.TrainingAsync();
                //});
                //_stopwatch.Stop();

                //_stopwatch.Start();
                //List<Task> tasks = new List<Task>();
                //for (int i = 0; i < _testCount; i++)
                //{
                //    tasks.Add(Task.Run(() => _network.TrainingAsync()));
                //}
                //await Task.WhenAll(tasks);
                //_stopwatch.Stop();

                Update();
            }
        }

        private void Draw_Click(object sender, RoutedEventArgs e)
        {
            if (!_draw)
            {
                DrawNetwork();
            }
            _draw = !_draw;
        }

        private void Reset_Click(object sender, RoutedEventArgs e)
        {
            _stopwatch.Reset();
            _isProcessing = false;
            _testCount = _defaultTestCount;
            _network = new SOMNetwork(_inputNodes, _length);
            Update();
        }

        private void Load_Click(object sender, RoutedEventArgs e)
        {
            _isProcessing = false;
            OpenFileDialog load = new OpenFileDialog();
            load.Filter = "Open file(*.json)|*.json|Все файлы(*.*)|*.*";

            if (load.ShowDialog() == true)
            {
                _network = JsonConvert.DeserializeObject<SOMNetwork>(File.ReadAllText(load.FileName));
                FileName.Text = load.FileName;
                _testCount = _defaultTestCount - _network.Iteration;
                Update();
            }
        }
        private void Save_Click(object sender, RoutedEventArgs e)
        {
            _isProcessing = false;
            string ser = JsonConvert.SerializeObject(_network);
            SaveFileDialog save = new SaveFileDialog();
            save.Filter = "Save file(*.json)|*.json|Все файлы(*.*)|*.*";

            if (save.ShowDialog() == true)
            {
                File.WriteAllText(save.FileName, ser);
                FileName.Text = save.FileName;
            }
        }
        private void Test_Click(object sender, RoutedEventArgs e)
        {
            _isProcessing = false;
            Update();
        }

        private void Exit_Click(object sender, RoutedEventArgs e)
        {
            Reset_Click(new object(), new RoutedEventArgs());
            this.Close();
        }

        public void Update()
        {
            Update(_network.Weights);
        }

        public void Update(_2DArray.Matrix weights)
        {
            _testCount = _defaultTestCount - _network.Iteration;
            TestText.Text = $"{_testCount}";

            var time = _stopwatch.Elapsed;
            TimeSpent.Text = string.Format("{0:00}:{1:00}:{2:000}", time.Minutes, time.Seconds, time.Milliseconds);

            if (_draw && weights != null)
            {
                DrawNetwork(weights);
            }
        }

        public void DrawNetwork()
        {
            DrawNetwork(_network.Weights);
        }

        public void DrawNetwork(_2DArray.Matrix weights)
        {
            var image = _operations.GetRGBFromMatrix(weights);

            int k = 0;
            for (int i = 0; i < _length; i++)
            {
                for (int j = 0; j < _length; j++, k++)
                {
                    ColorPalette[i][j].Fill = image[k];
                }
            }
        }
    }
}
