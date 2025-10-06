using Basler.Pylon;
using OpenCvSharp;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace WpfSaveImage
{
    public class CameraModel : IDisposable
    {
        Camera cam;
        Mat ImgGrab ;
        bool isOK, isNG;
        Stopwatch sw = new Stopwatch();
        object mlock = new object();
        public CameraModel()
        {
            cam = new Camera();
            cam.ConnectionLost += Cam_ConnectionLost;
            cam.StreamGrabber.ImageGrabbed += StreamGrabber_ImageGrabbed;
            Open();
        }


        private void StreamGrabber_ImageGrabbed(object sender, ImageGrabbedEventArgs e)
        {
            lock (mlock)
            {
                try
                {
                    var result = e.GrabResult;
                    if ((result.IsValid))
                    {
                        ImgGrab = new Mat(result.Height, result.Width, MatType.CV_8UC1, result.PixelData as byte[]);
                        isOK = true;
                    }
                    Console.WriteLine($"Tactime GetImage : {sw.ElapsedMilliseconds}ms");
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                    isNG = true;
                }
            }
        }
        public Mat Grab()
        {
            if (cam.IsConnected)
            {
                if (cam.StreamGrabber.IsGrabbing) cam.StreamGrabber.Stop();
                sw.Restart();
                isOK = isNG = false;
                cam.Parameters[PLCamera.AcquisitionMode].SetValue(PLCamera.AcquisitionMode.SingleFrame);
                cam.StreamGrabber.Start(1, GrabStrategy.LatestImages, GrabLoop.ProvidedByStreamGrabber);
                while (true)
                {
                    if (isOK) return ImgGrab;
                    if (isNG || sw.ElapsedMilliseconds > 3000) break;
                }
            }
            return null;
        }
        public Mat Live()
        {
            if (cam.IsConnected)
            {
                if (cam.StreamGrabber.IsGrabbing) cam.StreamGrabber.Stop();
                sw.Restart();
                isOK = isNG = false;
                cam.Parameters[PLCamera.AcquisitionMode].SetValue(PLCamera.AcquisitionMode.Continuous);
                cam.StreamGrabber.Start(1, GrabStrategy.LatestImages, GrabLoop.ProvidedByStreamGrabber);
                while (true)
                {
                    if (isOK) return ImgGrab;
                    if (isNG || sw.ElapsedMilliseconds > 3000) break;
                }
            }
            return null;
        }
        private void Cam_ConnectionLost(object sender, EventArgs e)
        {
            cam.Close();
            cam.Dispose();
        }
        void Open()
        {
            if ((cam.IsConnected == false))
            {
                try
                {
                    cam.Open();

                }
                catch (Exception ex)
                {

                    MessageBox.Show($"Camera Error: {ex}");
                }
            }
        }
        public void Dispose()
        {
            if (cam.IsConnected)
            {

                cam.Close();
                cam.Dispose();
            }
        }
    }
}
