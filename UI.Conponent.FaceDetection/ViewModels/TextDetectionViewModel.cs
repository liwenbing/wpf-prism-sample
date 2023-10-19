using OpenCvSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Xml.Linq;

namespace UI.Conponent.FaceDetection.ViewModels
{
    public class TextDetectionViewModel
    {

        private Mat _matImage;
        private Mat _grayImage;
        private Mat _kernelRect;
        private Mat _threshMat;
        private Mat _dilationMat;

        private double thresh1;

        public TextDetectionViewModel()
        {
            Detection();
        }


        private void Detection() 
        {
            _matImage = OpenCvSharp.Cv2.ImRead("C:\\Users\\Administrator\\Desktop\\1697593797802.png");

            _grayImage = new Mat();
            Gray();

            _kernelRect = new Mat();
            _threshMat = new Mat();
            Threshold();

            
            _dilationMat = new Mat();   
            Dilate();

            GetContours();
        }

        private void Gray()
        {
            Cv2.CvtColor(_matImage, _grayImage, ColorConversionCodes.BGR2GRAY);

            Cv2.ImWrite($"TextDetection_Gray_{Guid.NewGuid()}.jpg", _grayImage);
        }

        private void Threshold()
        {
            Mat dst = new Mat();
            thresh1 = Cv2.Threshold(_grayImage, _grayImage, 180, 255, ThresholdTypes.BinaryInv);

            //Mat mat = new Mat();
            //Cv2.BitwiseAnd(dst, dst, mat);
            thresh1 = Cv2.Threshold(_grayImage, _threshMat, 100, 255, ThresholdTypes.BinaryInv);


            //Cv2.Threshold(mat, _threshMat, 100, 255, ThresholdTypes.Otsu);

            Cv2.ImWrite($"TextDetection_Thresh_{Guid.NewGuid()}.jpg", _threshMat);
        }

        private void Dilate()
        {
            Mat element1 = new Mat();
            Mat element2 = new Mat();
            OpenCvSharp.Size size1 = new OpenCvSharp.Size(30, 9);
            OpenCvSharp.Size size2 = new OpenCvSharp.Size(24, 6);

            element1 = Cv2.GetStructuringElement(MorphShapes.Rect, size1);
            element2 = Cv2.GetStructuringElement(MorphShapes.Rect, size2);

            Mat dilation2 = new Mat();
            _kernelRect = Cv2.GetStructuringElement(MorphShapes.Rect, new OpenCvSharp.Size(18, 18));
            Cv2.Dilate(_threshMat, dilation2, _kernelRect, null ,1);

            Cv2.ImWrite($"TextDetection_Dilate_{Guid.NewGuid()}.jpg", dilation2);


            Mat erosion = new Mat();
            Cv2.Erode(dilation2, erosion, element1);

            //6. 再次膨胀，让轮廓明显一些
            //Mat dilation2 = new Mat();
            Cv2.Dilate(erosion, _dilationMat, element2, null, 2);
            Cv2.ImWrite($"TextDetection_Dilate2_{Guid.NewGuid()}.jpg", _dilationMat);
        }

        private void GetContours()
        {
            Scalar color = new Scalar(0, 0, 255);
            OpenCvSharp.Point[][] contours;
            HierarchyIndex[] hierarchy;
            Cv2.FindContours(_dilationMat, out contours, out hierarchy, RetrievalModes.External, ContourApproximationModes.ApproxNone, null);

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

    }
}
