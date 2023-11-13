using Core.Tools;
using OpenCvSharp;
using OpenCvSharp.Dnn;
using OpenCvSharp.Extensions;
using OpenCvSharp.Text;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Regions;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Size = OpenCvSharp.Size;

namespace UI.Conponent.FaceDetection.ViewModels
{
    
    public class TextDetectionViewModel: BindableBase, INavigationAware
    {
       
        private Mat _matImage;
        private Mat _grayImage;
        private Mat _kernelRect;
        private Mat _threshMat;
        private Mat _dilation1Mat;
        private Mat _ErodeMat;
        private Mat _dilation2Mat;
        private Mat _sobel;

        private string _ImageDir;
        public string ImageDir { get { return _ImageDir; } set { _ImageDir = value; RaisePropertyChanged(); } }

        public BitmapImage _orgBitmapImage;
        public BitmapImage OrgBitmapImage { get { return _orgBitmapImage; } set { _orgBitmapImage = value; RaisePropertyChanged(); } }

        public BitmapImage _deteBitmapImage;
        public BitmapImage DeteBitmapImage { get { return _deteBitmapImage; } set { _deteBitmapImage = value; RaisePropertyChanged(); } }

        public BitmapImage _grayBitmapImage;
        public BitmapImage grayBitmapImage { get { return _grayBitmapImage; } set { _grayBitmapImage = value; RaisePropertyChanged(); } }

        public BitmapImage _SobelBitmapImage;
        public BitmapImage SobelBitmapImage { get { return _SobelBitmapImage; } set { _SobelBitmapImage = value; RaisePropertyChanged(); } }

        public BitmapImage _ThreshBitmapImage;
        public BitmapImage ThreshBitmapImage { get { return _ThreshBitmapImage; } set { _ThreshBitmapImage = value; RaisePropertyChanged(); } }

        public BitmapImage _Dilate1BitmapImage;
        public BitmapImage Dilate1BitmapImage { get { return _Dilate1BitmapImage; } set { _Dilate1BitmapImage = value; RaisePropertyChanged(); } }

        public BitmapImage _ErodeBitmapImage;
        public BitmapImage ErodeBitmapImage { get { return _ErodeBitmapImage; } set { _ErodeBitmapImage = value; RaisePropertyChanged(); } }

        public BitmapImage _Dilate2BitmapImage;
        public BitmapImage Dilate2BitmapImage { get { return _Dilate2BitmapImage; } set { _Dilate2BitmapImage = value; RaisePropertyChanged(); } }

        private string[] imageNames;
        private int ImagePtr = 0;

        public ICommand PreviousCommand { get; }
        public ICommand NextCommand { get; }

        public TextDetectionViewModel()
        {
            //TextDetectionModel_DB("");

            ImageDir = "C:\\Users\\Administrator\\Desktop";
            imageNames = Directory.GetFiles(ImageDir, "*.*", SearchOption.AllDirectories).Where(x => x.EndsWith(".jpg") || x.EndsWith(".jpeg") || x.EndsWith(".png")).ToArray();

            PreviousCommand = new DelegateCommand(PreviousImage);
            NextCommand = new DelegateCommand(NextImage);
            //ocr.Run
            
        }

        private void NextImage()
        {
            ++this.ImagePtr;
            if (this.ImagePtr >= imageNames.Length)
            {
                this.ImagePtr = imageNames.Length - 1;
                return;
            }

            var imageName = imageNames[this.ImagePtr];
            _matImage = Cv2.ImRead(imageName);

            Detection();
        }

        private void PreviousImage()
        {
            --this.ImagePtr;
            if (this.ImagePtr < 0)
            {
                this.ImagePtr = 0;
                return;
            }
            var imageName = imageNames[this.ImagePtr];
            _matImage = Cv2.ImRead(imageName);
            if (_matImage.Empty()) { return; }

            Detection();
        }

        private void Detection() 
        {
            OrgBitmapImage = GetBitmapImage(_matImage); ;

            Gray();
            grayBitmapImage = GetBitmapImage(_grayImage);

            Sobel();
            SobelBitmapImage = GetBitmapImage(_sobel);

            Threshold();
            ThreshBitmapImage = GetBitmapImage(_threshMat);

            Dilate();
            Dilate1BitmapImage = GetBitmapImage(_dilation1Mat);
            ErodeBitmapImage = GetBitmapImage(_ErodeMat);
            Dilate2BitmapImage = GetBitmapImage(_dilation2Mat);

            //Detection();
            GetContours();
            DeteBitmapImage = GetBitmapImage(_matImage);
        }

        private void Gray()
        {
            _grayImage = new Mat();
            Cv2.CvtColor(_matImage, _grayImage, ColorConversionCodes.BGR2GRAY);


        }

        private void Sobel()
        {
            //1.Sobel算子，x方向求梯度
            _sobel = new Mat();
            Cv2.Sobel(_grayImage, _sobel, MatType.CV_8U, 1, 0, 3);

        }

        private void Threshold()
        {
            _threshMat = new Mat();
            Cv2.Threshold(_sobel, _threshMat, 0, 255, ThresholdTypes.Otsu | ThresholdTypes.Binary);
        }

        private void Dilate()
        {
            OpenCvSharp.Size size1 = new OpenCvSharp.Size(30, 9);
            OpenCvSharp.Size size2 = new OpenCvSharp.Size(24, 6);

            Mat element1 = Cv2.GetStructuringElement(MorphShapes.Rect, size1);
            Mat element2 = Cv2.GetStructuringElement(MorphShapes.Rect, size2);

            _dilation1Mat = new Mat();
            //_kernelRect = Cv2.GetStructuringElement(MorphShapes.Rect, new OpenCvSharp.Size(3, 3));
            Cv2.Dilate(_threshMat, _dilation1Mat, element2);

            _ErodeMat = new Mat();
            //腐蚀
            Cv2.Erode(_dilation1Mat, _ErodeMat, element1);

            //6. 再次膨胀，让轮廓明显一些
            _dilation2Mat = new Mat();
            Cv2.Dilate(_ErodeMat, _dilation2Mat, element2, null, 1);
        }

        private void GetContours()
        {
            Scalar color = new Scalar(0, 0, 255);
            OpenCvSharp.Point[][] contours;
            HierarchyIndex[] hierarchy;
            Cv2.FindContours(_dilation2Mat, out contours, out hierarchy, RetrievalModes.External, ContourApproximationModes.ApproxNone, null);

            if (contours.Length > 0)
            {
                var boxes = contours.Select(Cv2.BoundingRect);
                List<OpenCvSharp.Rect> rects = boxes.ToList();

                foreach (OpenCvSharp.Rect rect in rects)
                {
                    Cv2.Rectangle(_matImage, new OpenCvSharp.Point(rect.X, rect.Y), new OpenCvSharp.Point(rect.X + rect.Width, rect.Y + rect.Height), Scalar.Red, 2);
                }
            }

            Cv2.ImWrite($"TextDetection_End_{Guid.NewGuid()}.jpg", _matImage);
        }

        private BitmapImage GetBitmapImage(Mat image) 
        {
            var bitmap = image.ToBitmap();
            MemoryStream ms = new MemoryStream();
            bitmap.Save(ms, System.Drawing.Imaging.ImageFormat.Bmp);
            byte[] bytes = ms.ToArray();
            ms.Close();

            // BitmapImage 固定格式
            BitmapImage bitmapImage = new BitmapImage();
            bitmapImage.BeginInit();
            bitmapImage.StreamSource = new MemoryStream(bytes);
            bitmapImage.EndInit();

            return bitmapImage;
        }

        public void OnNavigatedTo(NavigationContext navigationContext)
        {
            var imageName = imageNames[this.ImagePtr];
            _matImage = Cv2.ImRead(imageName);

            //Rect[] rects;
            //float[] asdasda;
            //TextDetectorCNN textDetector = TextDetectorCNN.Create("onnx/crnn_cs_CN.onnx", "onnx/DB_IC15_resnet50.onnx");
            //textDetector.Detect(_matImage, out rects, out asdasda);

            Net net = CvDnn.ReadNetFromOnnx("onnx/crnn_cs_CN.onnx");
            
            var blob = CvDnn.BlobFromImage(_matImage);
            net.SetInput(blob);

            var detection = net.Forward();
            var detectionMat = new Mat(detection.Size(2), detection.Size(3),MatType.CV_32F, detection.Ptr(0));

            Detection();
        }

        public bool IsNavigationTarget(NavigationContext navigationContext)
        {
            return true;
            //throw new NotImplementedException();
        }

        public void OnNavigatedFrom(NavigationContext navigationContext)
        {
            //throw new NotImplementedException();
        }
    }
}
