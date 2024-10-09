using System;
using System.Drawing;
using System.Threading.Tasks;
using Compunet.YoloV8;
using DlibDotNet;
using DlibDotNet.Dnn;
using DlibDotNet.Extensions;
using Emgu.CV;
using Emgu.CV.Structure;
using MongoDB.Driver;
using MPTC_API.Models.Attendance;
using static System.Text.Json.JsonElement;

namespace MPTC_API.Services
{
    public class RecognitionService
    {
        private readonly IMongoCollection<EmployeeImage> _employeeImages;
        private static ShapePredictor _shapePredictor;
        private static LossMetric _net;


        public RecognitionService(IMongoClient mongoClient )
        {
            var database = mongoClient.GetDatabase("admin"); // Replace with your database name
            Console.WriteLine(database);
            _employeeImages = database.GetCollection<EmployeeImage>("EmployeeImage");


            var ShapePredictorPath = Path.Combine(Directory.GetCurrentDirectory(), "Utils/Recognition/shape_predictor_68_face_landmarks.dat");
            var faceRecognitionModelPath = Path.Combine(Directory.GetCurrentDirectory(), "Utils/Recognition/dlib_face_recognition_resnet_model_v1.dat");
            _net = DlibDotNet.Dnn.LossMetric.Deserialize(faceRecognitionModelPath);
            _shapePredictor = ShapePredictor.Deserialize(ShapePredictorPath);
            // Constructor logic here
        }
        public async Task<Compunet.YoloV8.Data.BoundingBox> CheckFaceInUploadedImage(Image<Bgr, Byte> pictureToAnalyze)
        {
            try
            {
                using (var predictorFace = YoloV8Predictor.Create("Utils/Recognition/yolov8n-face.onnx"))
                {
                    var image = SixLabors.ImageSharp.Image.Load(ConvertToImageSharp(pictureToAnalyze));
                    // Await the async call to avoid deadlocks
                    var faces = await predictorFace.DetectAsync(image);

                    // Check if any faces were detected
                    if (faces.Boxes.Length == 0)
                    {
                        return null;
                    }
                    return faces.Boxes[0];
                }
            }
            catch (Exception ex)
            {
                // Log or handle the exception
                Console.WriteLine($"Error: {ex.Message}");
                // You could throw the exception again or handle it here
                return null;
            }
        }

        public List<Dictionary<string, Object>>  ExtractFaceDescriptorPerImage(ArrayEnumerator pictureArray)
        {
             List<Dictionary<string, Object>> faceDescriptors =  new List<Dictionary<string, Object>>();
            foreach(var picture in pictureArray)
            {
                string base64String = picture.GetProperty("preview").GetString();
                Image<Bgr, Byte> pictureToAnalyze = ConvertBase64ToImage(base64String);
                Compunet.YoloV8.Data.BoundingBox faceBoundingBox = CheckFaceInUploadedImage(pictureToAnalyze).Result;

                if(faceBoundingBox != null)
                {
                    DlibDotNet.Matrix<RgbPixel> dlibImage = ConvertImageToDlibMatrix(pictureToAnalyze);
                    DlibDotNet.Rectangle dlibRect = ConvertBoundingBoxToDlibRect(faceBoundingBox);

                    var shape = _shapePredictor.Detect(dlibImage, dlibRect);
                    var faceChipDetails  = Dlib.GetFaceChipDetails(shape, 150);
                    var faceChip = Dlib.ExtractImageChip<RgbPixel>(dlibImage, faceChipDetails);

                    using(var result = faceChip.ToBitmap())
                    {
                        DlibDotNet.Matrix<RgbPixel> matrix = result.ToMatrix<RgbPixel>();
                        var faceDescriptor = _net.Operator(matrix);
                        //Create the dictionnary, add base64string and the appropriate facedescriptor
                        Dictionary<string, Object> metadata = new Dictionary<string, Object>
                        {
                            { "picture", base64String },
                            { "descriptor", faceDescriptor.ToArray().SelectMany(x => x).ToArray() }
                        };
                        faceDescriptors.Add(metadata);
                    }
                }else{
                    return null;
                }

            }

            return faceDescriptors;

        }
                


        public ReadOnlySpan<byte> ConvertToImageSharp(Image<Bgr, byte> mat)
        {
            using var ms = new MemoryStream();
            mat.ToBitmap().Save(ms, System.Drawing.Imaging.ImageFormat.Png);
            byte[] byteArray = ms.ToArray();
            ReadOnlySpan<byte> span = new ReadOnlySpan<byte>(byteArray);

            return span;
        }

       public Image<Bgr, Byte> ConvertBase64ToImage(string base64String)
        {
            // Step 1: Decode Base64 string to byte array
            byte[] imageBytes = Convert.FromBase64String(base64String.Split(",")[1]);

            // Step 2: Load the byte array into a MemoryStream
            using (MemoryStream ms = new MemoryStream(imageBytes))
            {
                // Step 3: Create an Image<Bgr, Byte> from the MemoryStream
                Bitmap bitmap = new Bitmap(ms);
                Mat mat = bitmap.ToMat();
                Image<Bgr, Byte> image = mat.ToImage<Bgr, Byte>();

                return image;
            }
        }

        public DlibDotNet.Matrix<RgbPixel> ConvertImageToDlibMatrix(Image<Bgr, Byte> image)
        {
            DlibDotNet.Matrix<RgbPixel> result;   
            using(var grayImage = image.Convert<Gray, Byte>())
            {
                // Convert the image to a byte array
                using(var dlibImage = new DlibDotNet.Matrix<RgbPixel>(grayImage.Height, grayImage.Width))
                {
                    var bytes = image.Bytes;

                    // Copy the image data to the Dlib matrix
                    for(int y=0; y<grayImage.Height; y++)
                    {
                        for(int x=0; x<grayImage.Width; x++)
                        {
                             // Calculate the pixel index in the byte array (BGR format)
                            int index = (y * image.Width + x) * image.NumberOfChannels;
                            byte blue = bytes[index];
                            byte green = bytes[index + 1];
                            byte red = bytes[index + 2];
                            dlibImage[y, x] = new RgbPixel(red, green, blue);
                        }
                    }

                    //clone dlib image to result
                    result = dlibImage.Clone();
                }

            }
            return result;
            
        }

        public DlibDotNet.Rectangle ConvertBoundingBoxToDlibRect(Compunet.YoloV8.Data.BoundingBox box)
        {
            //yolo bounding box properties
            int x = box.Bounds.X;
            int y = box.Bounds.Y;
            int width = box.Bounds.Width;
            int height = box.Bounds.Height;

            //Dlib rectangle properties
            int left = x;
            int top = y;
            int right = x + width;
            int bottom = y + height;

            return new DlibDotNet.Rectangle(left, top, right, bottom);
        }

        public async Task InsertEmployeeImages(List<EmployeeImage> employeeImage)
        {
            await _employeeImages.InsertManyAsync(employeeImage);

        }
    }
}