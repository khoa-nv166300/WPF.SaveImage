using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using OpenCvSharp;
using OpenCvSharp.Extensions;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace WpfSaveImage
{

    public class MainViewModel : ObservableObject
    {
        private readonly CameraModel _cameraModel;
        private Mat mat;
        private Stopwatch sw;
        private bool bResult = false;
        public string AlignTitle { get; set; }

        private string _TextSaveImage;

        public string TextSaveImage
        {
            get { return _TextSaveImage; }
            set { _TextSaveImage = value; OnPropertyChanged(); }
        }

        private string _TextTactime;

        public string TextTactime
        {
            get { return _TextTactime; }
            set { _TextTactime = value; OnPropertyChanged(); }
        }

        private ImageSource _image;
        public ImageSource Image
        {
            get => _image;
            set => SetProperty(ref _image, value);
        }
        private Brush stateColor = BrushExtension.FromHex(HexStatusColor.Disabled);

        public Brush StateColor
        {
            get { return stateColor; }
            set
            {
                stateColor = value;
                OnPropertyChanged();
            }
        }
        public RelayCommand TriggerCommand { get; }
        public RelayCommand SaveImageCommand { get; }

        public MainViewModel()
        {
            AlignTitle = "Camera 01";
            sw = new Stopwatch();
            _cameraModel = new CameraModel();

            TriggerCommand = new RelayCommand(OnTrigger);
            SaveImageCommand = new RelayCommand(OnSaveImage);
            CDefines.SequenceChanged += CDefines_SequenceChanged;
        }

        private void CDefines_SequenceChanged(object sender, ESequence eSequence)
        {
            App.Current.Dispatcher.Invoke(new Action(() =>
            {
                switch (eSequence)
                {
                    case ESequence.Pending:
                        StateColor = BrushExtension.FromHex(HexStatusColor.Pending);
                        break;
                    case ESequence.Error:
                        StateColor = BrushExtension.FromHex(HexStatusColor.Error);
                        break;
                    case ESequence.Complete:
                        StateColor = BrushExtension.FromHex(HexStatusColor.Complete);
                        break;
                    case ESequence.Accepted:
                        StateColor = BrushExtension.FromHex(HexStatusColor.Accepted);
                        break;
                    case ESequence.Denied:
                        StateColor = BrushExtension.FromHex(HexStatusColor.Denied);
                        break;
                    case ESequence.Disabled:
                        StateColor = BrushExtension.FromHex(HexStatusColor.Disabled);
                        break;
                    default:
                        StateColor = BrushExtension.FromHex(HexStatusColor.Disabled);
                        break;
                }
            }));
        }



        private void OnTrigger()
        {
            if (_cameraModel != null)
            {
                sw.Restart();
                this.mat = this._cameraModel.Grab();
                if (mat != null)
                {
                    Image = mat.ToWriteableBitmap();
                    TextTactime = $"Trigger: {sw.ElapsedMilliseconds}ms";
                    CDefines.OnSequenceChanged(ESequence.Accepted);
                    Task.Factory.StartNew(() => { Thread.Sleep(100); CDefines.OnSequenceChanged(ESequence.Disabled); });
                
                }
                else
                {
                    TextTactime = $"Trigger: Fail";
                    CDefines.OnSequenceChanged(ESequence.Error);
                    Task.Factory.StartNew(() => { Thread.Sleep(100); CDefines.OnSequenceChanged(ESequence.Disabled); });
                }
            }
            else
            {
                TextTactime = $"Trigger: Fail";
                CDefines.OnSequenceChanged(ESequence.Error);
                Task.Factory.StartNew(() => { Thread.Sleep(100); CDefines.OnSequenceChanged(ESequence.Disabled); });
            }
        }

        private void OnSaveImage()
        {
            if (mat != null)
            {
                SaveImage(mat);
                TextSaveImage = $"Success!";
                CDefines.OnSequenceChanged(ESequence.Complete);
                Task.Factory.StartNew(() => { Thread.Sleep(100); CDefines.OnSequenceChanged(ESequence.Disabled); });
            }
            else
            {
                TextSaveImage = $"Fail!";
                CDefines.OnSequenceChanged(ESequence.Error);
                Task.Factory.StartNew(() => { Thread.Sleep(100); CDefines.OnSequenceChanged(ESequence.Disabled); });
            }
        }
        public void SaveImage(BitmapSource image)
        {
            if (image != null)
            {
                string filename = $"D:/Images_Khoa/{DateTime.Now:yyyyMMdd_HHmmss}.bmp";
                var encoder = new PngBitmapEncoder();
                encoder.Frames.Add(BitmapFrame.Create(image));

                Directory.CreateDirectory("D:/Images_Khoa");
                using (var fs = File.OpenWrite(filename))
                {
                    encoder.Save(fs);
                }
            }
        }

        public void SaveImage(Mat image)
        {
            if (mat != null)
            {
                Directory.CreateDirectory("D:/Images_Khoa");
                string filename = $"D:/Images_Khoa/{DateTime.Now:yyyyMMdd_HHmmss}.bmp";
                image.SaveImage(filename);
            }
            Console.WriteLine("SaveImage");
        }
    }
}