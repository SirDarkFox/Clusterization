using Clusterization._2DArray;
using Clusterization.Text;
using Microsoft.Win32;
using Newtonsoft.Json;
using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace Clusterization
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        int _inputNodes = 3;
        int _length = 4;
        int _defaultTestCount = 300000;
        int _testCount;
        public ObservableCollection<ObservableCollection<TextBox>> ColorPalette { get; set; }

        Random _random;

        SOMNetwork _network;
        Operations _operations;

        BookCollection _bookCollection;

        bool _draw = false;
        bool _isProcessing = false;

        Task _task;
        CancellationTokenSource _tokenSource;
        CancellationToken _cancellationToken;

        public MainWindow()
        {
            InitializeComponent();
            _random = new Random();

            _bookCollection = new BookCollection();

            _tokenSource = new CancellationTokenSource();
            _cancellationToken = _tokenSource.Token;

            _network = new SOMNetwork(_inputNodes, _length);
            _operations = new Operations();

            ColorPalette = new ObservableCollection<ObservableCollection<TextBox>>();

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

            //for (int i = 0; i < _length; i++)
            //{
            //    ColorPalette.Add(new ObservableCollection<TextBox>());
            //    for (int j = 0; j < _length; j++)
            //    {
            //        TextBox textBlock = new TextBox
            //        {
            //            Height = size,
            //            Width = size,
            //            Foreground = Brushes.White,
            //            Background = Brushes.Transparent,
            //            BorderBrush = Brushes.White
            //        };

            //        Canvas.SetTop(textBlock, i * size);
            //        Canvas.SetLeft(textBlock, j * size);

            //        ColorPalette[i].Add(textBlock);
            //        Image.Children.Add(textBlock);
            //    }
            //}

            Update();
            DrawNetwork();
        }

        private async void Learn_Click(object sender, RoutedEventArgs e)
        {
            _isProcessing = !_isProcessing;

            if (!_isProcessing)
            {
                _tokenSource.Cancel();
                return;
            }


            _tokenSource = new CancellationTokenSource();
            _cancellationToken = _tokenSource.Token;

            _task = Task.Run(() =>
            {
                while (_defaultTestCount - _network.Iteration > 0)
                {
                    int index = _random.Next(_bookCollection.Books.Count);

                    _network.TrainingAsync(_bookCollection.Books[index].Vector);

                    if (_cancellationToken.IsCancellationRequested)
                    {
                        break;
                    }
                }
            }, _cancellationToken);



            int temp = _defaultTestCount - _network.Iteration;

            while (_isProcessing)
            {
                temp = _defaultTestCount - _network.Iteration;
                if (temp < _testCount)
                {
                    Update();

                    if (_draw)
                    {
                        DrawNetwork();
                    }
                }

                _testCount = temp;
                await Task.Delay(1);
            }
        }

        private void Draw_Click(object sender, RoutedEventArgs e)
        {
            _draw = !_draw;
        }

        private void Reset_Click(object sender, RoutedEventArgs e)
        {
            _isProcessing = false;
            _tokenSource.Cancel();
            _testCount = _defaultTestCount;
            _network = new SOMNetwork(_inputNodes, _length);
            DrawNetwork();
            Update();
        }

        private void Load_Click(object sender, RoutedEventArgs e)
        {
            _isProcessing = false;
            _tokenSource.Cancel();
            OpenFileDialog load = new OpenFileDialog();
            load.Filter = "Open file(*.json)|*.json|Все файлы(*.*)|*.*";

            if (load.ShowDialog() == true)
            {
                var saveClass = JsonConvert.DeserializeObject<Save>(File.ReadAllText(load.FileName));
                _network = saveClass.SOMNetwork;
                _bookCollection = saveClass.BookCollection;
                _inputNodes = _bookCollection.Vocabulary.Count;
                FileName.Text = load.FileName;
                _testCount = _defaultTestCount - _network.Iteration;
                Update();
            }
        }
        private void Save_Click(object sender, RoutedEventArgs e)
        {
            _isProcessing = false;
            _tokenSource.Cancel();
            var saveClass = new Save { SOMNetwork = _network, BookCollection = _bookCollection };
            string ser = JsonConvert.SerializeObject(saveClass);
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
            _tokenSource.Cancel();
            DrawNetwork();
        }

        public void Update()
        {
            TestText.Text = $"{_testCount}";
        }

        void DrawNetwork()
        {
            var image = _operations.GetRGBFromMatrix(_network.Weights);

            int k = 0;
            for (int i = 0; i < _length; i++)
            {
                for (int j = 0; j < _length; j++, k++)
                {
                    ColorPalette[i][j].Text = "";
                }
            }

            foreach (var book in _bookCollection.Books)
            {
                int index = _operations.FindMinDistanceIndex(_network.Weights, book.Vector);

                ColorPalette[index % _length][index / _length].Text += book.Title + "\n";
            }
        }
    }
}
