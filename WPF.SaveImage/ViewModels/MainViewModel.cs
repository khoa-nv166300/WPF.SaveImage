using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using log4net;
using OpenCvSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace WPF.SaveImage
{
    public class MainViewModel : ObservableObject
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(MainViewModel));
        private readonly CameraModel _cameraModel;

        private BitmapSource _image;
        public BitmapSource Image
        {
            get => _image;
            set => SetProperty(ref _image, value);
        }

        public RelayCommand LiveCommand { get; }
        public RelayCommand TriggerCommand { get; }
        public RelayCommand SaveImageCommand { get; }

        public MainViewModel()
        {
            _cameraModel = new CameraModel();
            _cameraModel.ImageReceived += OnImageReceived;

            LiveCommand = new RelayCommand(OnLive);
            TriggerCommand = new RelayCommand(OnTrigger);
            SaveImageCommand = new RelayCommand(OnSaveImage);

            log.Info("Application started");
            _cameraModel.Initialize();
        }

        private void OnImageReceived(Mat mat)
        {
            Image = mat.ToBitmapSource();
            log.Info("Image received.");
        }

        private void OnLive()
        {
            _cameraModel.StartGrabbing();
            log.Info("Live mode started");
        }

        private void OnTrigger()
        {
            _cameraModel.Trigger();
            log.Info("Trigger pressed");
        }

        private void OnSaveImage()
        {
            if (Image != null)
            {
                var encoder = new PngBitmapEncoder();
                encoder.Frames.Add(BitmapFrame.Create(Image));
                using (var fs = System.IO.File.OpenWrite($"Images/{DateTime.Now:yyyyMMdd_HHmmss}.png"))
                {
                    encoder.Save(fs);
                }
                log.Info("Image saved");
            }
        }
    }
}