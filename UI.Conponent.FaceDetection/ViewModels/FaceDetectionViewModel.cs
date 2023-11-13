using Core.Tools;
using OpenCvSharp;
using OpenCvSharp.Extensions;
using OpenCvSharp.Face;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Regions;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Management;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace UI.Conponent.FaceDetection.ViewModels
{
    public class FaceDetectionViewModel: BindableBase, INavigationAware
    {
        private VideoCapture videoCapture;
        private Mat matImage = new Mat();
        private CascadeClassifier haarCascade;
        private System.Drawing.Rectangle rectangle;

        private LBPHFaceRecognizer recognizer;

        private WriteableBitmap _writeableBitmap;
        public WriteableBitmap writeableBitmap { get { return _writeableBitmap; } set { _writeableBitmap = value; base.RaisePropertyChanged();  } }

        private VideoWriter videoWriter;

        private Mat gray;
        private Mat result;
        private OpenCvSharp.Rect[] faces;

        private List<(string, int)> FaceNames = new List<(string, int)>() { ValueTuple.Create("liuyifei", 1), ValueTuple.Create("jingtian", 2) };

        private CancellationTokenSource PlayCameraCancelTokenSource;
        private CancellationTokenSource StartVedioCancelTokenSource;

        private List<string> _CameraArray;
        public List<string> CameraArray { get { return _CameraArray; } set { _CameraArray = value; base.RaisePropertyChanged(); } }

        private int _cameraIndex;
        public int CameraIndex { get { return _cameraIndex; } set { _cameraIndex = value; base.RaisePropertyChanged(); } }
        public bool IsSaveVideo { get; set; } = false;
        private bool _IsFace;
        public bool IsFace { get { return _IsFace; } set { _IsFace = value; base.RaisePropertyChanged();  } }
        //public bool IsFace { get; set; } = true;
        public bool IsFaceRecognizer { get; set; } = false;
        public ImageSource bitmapImage { get; set; }

        private bool _PlayButtonEnabled;
        private bool _StopButtonEnabled;
        public bool PlayButtonEnabled { get { return _PlayButtonEnabled; } set { _PlayButtonEnabled = value; base.RaisePropertyChanged(); } }
        public bool StopButtonEnabled { get { return _StopButtonEnabled; } set { _StopButtonEnabled = value; base.RaisePropertyChanged(); } }

        private bool _ImageSwitch;
        public bool ImageSwitch { get { return _ImageSwitch; } set { _ImageSwitch = value; base.RaisePropertyChanged(); } }

        private bool _CameraSwitch;
        public bool CameraSwitch { get { return _CameraSwitch; } set { _CameraSwitch = value; base.RaisePropertyChanged(); } }


        public ICommand SelectionChangedCommand { get; }
        public ICommand PlayCameraCommand { get; }
        public ICommand StopCameraCommand { get; }
        public ICommand IsFaceCheckedCommand { get; }
        public ICommand IsSaveVideoCheckedCommand { get; }
        public ICommand IsFaceRecognizerCommand {  get; }
        public ICommand PreviousCommand { get; }
        public ICommand NextCommand { get; }

        private string[] imageNames = Directory.GetFiles(@"detection","*.*", SearchOption.AllDirectories);
        private int ImagePtr = 0;

        public FaceDetectionViewModel()
        {
            SelectionChangedCommand = new DelegateCommand(SelectionChanged);
            IsFaceCheckedCommand = new DelegateCommand(IsFaceChecked);
            IsSaveVideoCheckedCommand = new DelegateCommand(IsSaveVideoChecked);
            PlayCameraCommand = new DelegateCommand(PlayCamera);
            StopCameraCommand = new DelegateCommand(StopCamera);
            IsFaceRecognizerCommand = new DelegateCommand(IsFaceRecognizerChecked);
            PreviousCommand = new DelegateCommand(PreviousImage);
            NextCommand = new DelegateCommand(NextImage);

            Application.Current.Exit += Current_Exit;
            //IsFace = true;

        }


        #region 事件

        private void NextImage()
        {
            ++this.ImagePtr;
            if (this.ImagePtr >= imageNames.Length)
            {
                this.ImagePtr = imageNames.Length - 1;
                return;
            }

            var imageName = imageNames[this.ImagePtr];
            matImage = Cv2.ImRead(imageName);
            SwitchImage();
            //matImage.Dispose();
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
            matImage = Cv2.ImRead(imageName);
            if (matImage.Empty()) { return; }

            SwitchImage();
            //matImage.Dispose();
        }

        private void SwitchImage()
        {
            if(!this.IsFace)
            {
                return;
            }

            var faceDe = FaceDetection(matImage);

            if (faceDe.Item2 != null)
            {
                foreach (var item in faceDe.Item2)
                {
                    Cv2.Rectangle(matImage, item, Scalar.Green, 2);
                }
            }

            if (IsFaceRecognizer)
            {
                List<(int, double)> ss = new List<(int, double)>();
                foreach (var item in faceDe.Item2)
                {
                    //    var rect1 = faces[0];

                    var rangeX = new OpenCvSharp.Range(item.Y, item.Y + item.Width);
                    var rangeY = new OpenCvSharp.Range(item.X, item.X + item.Height);

                    var gray = faceDe.Item1[rangeX, rangeY];

                    Predict(gray, out int label, out double confidence);
                    ss.Add((label, confidence));
                }

                string faceName = FaceNames.FirstOrDefault(x => x.Item2 == ss.OrderBy(y => y.Item2).FirstOrDefault().Item1).Item1 ?? string.Empty;
                Cv2.PutText(matImage, faceName, new OpenCvSharp.Point() { X = 50, Y = 50 }, HersheyFonts.HersheyDuplex, 1, Scalar.Red);

            }

            using (var img = BitmapConverter.ToBitmap(matImage))
            {
                writeableBitmap = WriteableBitmapHelper.BitmapToWriteableBitmap(img);
                img.Dispose();
            };
        }

        private void Current_Exit(object sender, ExitEventArgs e)
        {
            PlayCameraCancelTokenSource?.Cancel();
            StartVedioCancelTokenSource?.Cancel();
        }

        private void IsFaceRecognizerChecked()
        {
            if (this.IsFaceRecognizer)
            {
                if (!File.Exists("face_train.yml"))
                { 
                    this.IsFaceRecognizer = false;
                }

                //load train model
                LoadTrainModel();

                this.IsFace = true;
            }
            else
            {
                
            }
        }

        private async void IsSaveVideoChecked()
        {
            if (this.IsSaveVideo)
            {
                await Task.Delay(100);
                StartRecording();
            }
            else
            {
                StopRecording();
            }
        }

        private async void IsFaceChecked()
        {
            if (this.IsFace)
            {
                await Task.Delay(100);
                LoadFaceDetectionData();
            }
            else
            {
                FreeFaceDetectionData();
                IsFaceRecognizer = false;
            }
        }

        public void SelectionChanged()
        {
            if (CameraArray.Count - 1 < CameraIndex)
                return;

            if (CameraIndex == 0)
            {
                ImageSwitch = true;
                CameraSwitch = false;

                if (imageNames.Length > 0)
                {
                    var imageName = imageNames[0];
                    using (Bitmap bitmap = new Bitmap(imageName))
                    {
                        writeableBitmap = WriteableBitmapHelper.BitmapToWriteableBitmap(bitmap);
                    }
                }

                return;
            }

            ImageSwitch = false;
            CameraSwitch = true;

            if (videoCapture != null)
            {
                PlayCameraCancelTokenSource.Cancel();
            }

            CreateCamera();

            writeableBitmap = new WriteableBitmap(videoCapture.FrameWidth, videoCapture.FrameHeight, 0, 0, System.Windows.Media.PixelFormats.Bgra32, null);

            PlayButtonEnabled = false;
            StopButtonEnabled = true;

        }

        private void PlayCamera()
        {
            CreateCamera();

            if (IsFace)
            {
                LoadFaceDetectionData();
            }

            if(IsSaveVideo) 
            {
                StartRecording();
            }

            PlayButtonEnabled = false;
            StopButtonEnabled = true;

        }

        private void StopCamera()
        {
            PlayCameraCancelTokenSource.Cancel();

            PlayButtonEnabled = true;
            StopButtonEnabled = false;
        }

        #endregion

        private void InitializeCamera()
        {
            IsFace = true;
            PlayButtonEnabled = true;
            StopButtonEnabled = false;

            List<string> Cameras = new List<string>();
            Cameras.Add("Image");
            Cameras.AddRange(GetAllConnectedCameras().ToList());

            CameraArray = Cameras;//
        }

        private IEnumerable<string> GetAllConnectedCameras()
        {
            using (var searcher = new ManagementObjectSearcher("SELECT * FROM Win32_PnPEntity WHERE (PNPClass = 'Image' OR PNPClass = 'Camera')"))
            {
                foreach (var device in searcher.Get())
                {
                    yield return device["Caption"].ToString();
                }
            }
        }

        private void CreateCamera()
        {
            videoCapture = new VideoCapture(CameraIndex-1);
            videoCapture.Fps = 30;

            PlayCameraCancelTokenSource = new CancellationTokenSource();
            Task.Factory.StartNew(PlayCameraCallback, PlayCameraCancelTokenSource.Token).ContinueWith((t) => {
                t.Wait();

                StopDispose();
            });
        }

        private void PlayCameraCallback()
        {
            while (!PlayCameraCancelTokenSource.IsCancellationRequested && videoCapture != null && !videoCapture.IsDisposed)
            {
                videoCapture.Read(matImage);

                if (matImage.Empty()) break;
                Dispatcher.CurrentDispatcher.Invoke(new Action(() =>
                {
                    if (IsFace && haarCascade != null)
                    {
                        result = matImage.Clone();

                        var faceDe = FaceDetection(result);
                        if (faceDe.Item2 != null)
                        {
                            foreach (var item in faceDe.Item2)
                            {
                                Cv2.Rectangle(matImage, item, Scalar.Green, 2);
                            }
                        }

                        //gray = new Mat();
                        //Cv2.CvtColor(result, gray, ColorConversionCodes.BGR2GRAY);
                        //faces = haarCascade.DetectMultiScale(gray, 1.05, 3);
                        //if (faces.Length > 0)
                        //{
                        //    Cv2.Rectangle(matImage, faces[0], Scalar.Green, 2);
                        //}

                        if (IsFaceRecognizer)
                        {
                            Predict(faceDe.Item1, out int label, out double confidence);

                            string faceName = FaceNames.FirstOrDefault(x => x.Item2 == label).Item1 ?? string.Empty;
                            Cv2.PutText(matImage, faceName, new OpenCvSharp.Point() { X = 10, Y = matImage.Height - 10}, HersheyFonts.HersheySimplex, 14, Scalar.RoyalBlue);
                        }

                        result.Dispose();
                    }
                }));
                using (var img = BitmapConverter.ToBitmap(matImage))
                {
                    var now = DateTime.Now;
                    var g = Graphics.FromImage(img);
                    var brush = new SolidBrush(System.Drawing.Color.AliceBlue);
                    System.Globalization.CultureInfo cultureInfo = new CultureInfo("zh-CN");
                    var week = cultureInfo.DateTimeFormat.GetAbbreviatedDayName(now.DayOfWeek);
                    g.DrawString($"{week} {now.ToString("yyyyMMdd HH:mm:ss ")} ", new System.Drawing.Font(System.Drawing.SystemFonts.DefaultFont.Name, System.Drawing.SystemFonts.DefaultFont.Size), brush, new PointF(5, matImage.Rows - 30));
                    brush.Dispose();
                    g.Dispose();
                    rectangle = new System.Drawing.Rectangle(0, 0, img.Width, img.Height);

                    Application.Current.Dispatcher.Invoke(new Action(() =>
                    {
                        WriteableBitmapHelper.BitmapCopyToWriteableBitmap(img, writeableBitmap, rectangle, 0, 0, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
                    }));
                    img.Dispose();
                };

                Thread.Sleep(100);
            }
        }

        private void StopDispose()
        {
            if (videoCapture != null && videoCapture.IsOpened())
            {
                videoCapture.Dispose();
                videoCapture = null;
            }

            if (videoWriter != null && !videoWriter.IsDisposed)
            {
                videoWriter.Release();
                videoWriter.Dispose();
                videoWriter = null;
            }

            FreeFaceDetectionData();
            GC.Collect();
        }

        private void LoadFaceDetectionData()
        {
            var facePath = System.IO.Path.Combine(System.Environment.CurrentDirectory, "Data/haarcascade_frontalface_default.xml");
            if (!System.IO.File.Exists(facePath))
            {
                //WPFDevelopers.Minimal.Controls.MessageBox.Show("缺少人脸检测文件。", "错误", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            haarCascade = new CascadeClassifier(facePath);
        }

        private void FreeFaceDetectionData()
        {
            if (haarCascade != null)
            {
                haarCascade.Dispose();
                haarCascade = null;
                //gray.Dispose();
                //gray = null;
                result.Dispose();
                result = null;
                faces = null;
            }
        }

        private void StartRecording()
        {
            if (videoCapture == null)
            {
                //WPFDevelopers.Minimal.Controls.MessageBox.Show("未开启摄像机", "提示", MessageBoxButton.OKCancel, MessageBoxImage.Error);
                return;
            }

            var videoFile = System.IO.Path.Combine(System.Environment.CurrentDirectory, "Video");
            if (!System.IO.Directory.Exists(videoFile))
                System.IO.Directory.CreateDirectory(videoFile);
            var currentTime = System.IO.Path.Combine(videoFile, $"{DateTime.Now.ToString("yyyyMMddHHmmsshh")}.avi");
            videoWriter = new VideoWriter(currentTime, FourCC.XVID, videoCapture.Fps, new OpenCvSharp.Size(videoCapture.FrameWidth, videoCapture.FrameHeight));

            StartVedioCancelTokenSource = new CancellationTokenSource();
            Task.Factory.StartNew(AddCameraFrameToRecording, StartVedioCancelTokenSource.Token).ContinueWith((t) => { 
                t.Wait();

                if (videoWriter != null && !videoWriter.IsDisposed)
                {
                    videoWriter.Release();
                    videoWriter.Dispose();
                    videoWriter = null;
                }
            });

            //writerThread = new Thread(AddCameraFrameToRecording);
            //writerThread.Start();
        }

        private void StopRecording()
        {
            StartVedioCancelTokenSource?.Cancel();
        }

        private void AddCameraFrameToRecording()
        {
            var waitTimeBetweenFrames = 1_000 / videoCapture.Fps;
            var lastWrite = DateTime.Now;

            while (!videoWriter.IsDisposed && !StartVedioCancelTokenSource.IsCancellationRequested)
            {
                if (DateTime.Now.Subtract(lastWrite).TotalMilliseconds < waitTimeBetweenFrames)
                    continue;

                lastWrite = DateTime.Now;
                videoWriter.Write(matImage);
            }
        }

        #region 人脸检测

        private (Mat, OpenCvSharp.Rect[]) FaceDetection(Mat matImage)
        {
            Mat _gray = new Mat();
            Cv2.CvtColor(matImage, _gray, ColorConversionCodes.BGR2GRAY);
            var faces = haarCascade.DetectMultiScale(_gray, 1.35, 5);
            //if(faces.Count() > 0) 
            //{
            //    var rect1 = faces[0];

            //    var rangeX = new OpenCvSharp.Range(rect1.Y, rect1.Y + rect1.Width);
            //    var rangeY = new OpenCvSharp.Range(rect1.X, rect1.X + rect1.Height);

            //    return (_gray[rangeX,rangeY], faces);
            //}
            return (_gray, faces);
        }

        #endregion

        #region 人脸识别

        private void LoadTrainModel()
        {
            recognizer = OpenCvSharp.Face.LBPHFaceRecognizer.Create();
            recognizer.Read("face_train.yml");
        }

        private void Predict(Mat Image,out int label, out double confidence)
        {
            //LBPHFaceRecognizer recognizer = OpenCvSharp.Face.LBPHFaceRecognizer.Create();
            //recognizer.Read("face_train.yml");

            recognizer.Predict(Image, out label, out confidence);

        }

        private void Train()
        {
            //LBPHFaceRecognizer recognizer = OpenCvSharp.Face.LBPHFaceRecognizer.Create();
            //recognizer.Train(trainImages.Select(x => x.Image), trainImages.Select(x => x.ImageGroupId));

            List<TrainImage> trainImages = new List<TrainImage>();

            LoadFaceDetectionData();
            
            string[] filePaths = Directory.GetFiles(AppDomain.CurrentDomain.BaseDirectory + "\\Train\\liuyifei");
            foreach (var item in filePaths)
            {
                var img = OpenCvSharp.Cv2.ImRead(item);

                var face = FaceDetection(img);

                foreach (var inneritem in face.Item2)
                {
                    var rangeX = new OpenCvSharp.Range(inneritem.Y, inneritem.Y + inneritem.Width);
                    var rangeY = new OpenCvSharp.Range(inneritem.X, inneritem.X + inneritem.Height);

                    var gray = face.Item1[rangeX, rangeY];
                    trainImages.Add(new TrainImage() { Image = gray, ImageGroupId = FaceNames[0].Item2 });
                }
                //if (face.Item1 != null)
                //{
                //    //face.SaveImage(Guid.NewGuid() + ".jpeg");
                //    trainImages.Add(new TrainImage() { Image = face.Item1, ImageGroupId = FaceNames[0].Item2 });
                //}
            }

            string[] filePaths1 = Directory.GetFiles(AppDomain.CurrentDomain.BaseDirectory + "\\Train\\jingtian");
            foreach (var item in filePaths1)
            {
                var img = OpenCvSharp.Cv2.ImRead(item);

                var face = FaceDetection(img);

                foreach (var inneritem in face.Item2)
                {
                    var rangeX = new OpenCvSharp.Range(inneritem.Y, inneritem.Y + inneritem.Width);
                    var rangeY = new OpenCvSharp.Range(inneritem.X, inneritem.X + inneritem.Height);

                    var gray = face.Item1[rangeX, rangeY];
                    trainImages.Add(new TrainImage() { Image = gray, ImageGroupId = FaceNames[1].Item2 });
                }

                //if (face.Item1 != null)
                //{
                //    //face.SaveImage(Guid.NewGuid() + ".jpeg");
                //    trainImages.Add(new TrainImage() { Image = face.Item1, ImageGroupId = FaceNames[1].Item2 });
                //}
            }

            LBPHFaceRecognizer recognizer = OpenCvSharp.Face.LBPHFaceRecognizer.Create();
            recognizer.Train(trainImages.Select(x => x.Image), trainImages.Select(x => x.ImageGroupId));

            recognizer.Write("face_train.yml");

        }

        #endregion

        #region Navigate

        public async void OnNavigatedTo(NavigationContext navigationContext)
        {
            InitializeCamera();

            CameraIndex = CameraArray.Count > 0 ? 0 : -1;
            ImageSwitch = true;

            if (imageNames.Length > 0)
            {
                var imageName = imageNames[this.ImagePtr];
                if (IsFace)
                {
                    LoadFaceDetectionData();

                    matImage = Cv2.ImRead(imageName);
                    if (matImage.Empty()) { return; }

                    await Task.Delay(200);
                    SwitchImage();
                }
                else
                {
                    using (Bitmap bitmap = new Bitmap(imageName))
                    { 
                        writeableBitmap = WriteableBitmapHelper.CreateCompatibleWriteableBitmap(bitmap);
                    }
                }
                
            }

            //var img = OpenCvSharp.Cv2.ImRead("C:\\Users\\liwen\\Desktop\\103d83f3b67dd2e182a35fa3679d4229.jpeg");

            //var face = FaceDetection(img);

            Train();

        }

        public bool IsNavigationTarget(NavigationContext navigationContext)
        {
            // throw new NotImplementedException();
            return true;
        }

        public void OnNavigatedFrom(NavigationContext navigationContext)
        {
            PlayCameraCancelTokenSource?.Cancel();
            StartVedioCancelTokenSource?.Cancel();
        }

        #endregion
    }

    public class TrainImage
    {
        public Mat Image { get; set; }

        public int ImageGroupId { get; set; }
    }
}
