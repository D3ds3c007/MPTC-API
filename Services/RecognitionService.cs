using System;
using System.Drawing;
using System.Net.WebSockets;
using System.Text;
using System.Threading.Tasks;
using Compunet.YoloV8;
using DlibDotNet;
using DlibDotNet.Dnn;
using DlibDotNet.Extensions;
using Emgu.CV;
using Emgu.CV.CvEnum;
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

        private static readonly Dictionary<string, float[]> _knownFaceEmbeddings = new Dictionary<string, float[]>();
        private Dictionary<DlibDotNet.Rectangle, string> _recognizedNames = new Dictionary<DlibDotNet.Rectangle, string>();

        private double scale = 0.0;





        public RecognitionService(IMongoClient mongoClient )
        {
            var database = mongoClient.GetDatabase("admin"); // Replace with your database name
            Console.WriteLine(database);
            _employeeImages = database.GetCollection<EmployeeImage>("EmployeeImage");


            var ShapePredictorPath = Path.Combine(Directory.GetCurrentDirectory(), "Utils/Recognition/shape_predictor_68_face_landmarks.dat");
            var faceRecognitionModelPath = Path.Combine(Directory.GetCurrentDirectory(), "Utils/Recognition/dlib_face_recognition_resnet_model_v1.dat");
            _net = DlibDotNet.Dnn.LossMetric.Deserialize(faceRecognitionModelPath);
            _shapePredictor = ShapePredictor.Deserialize(ShapePredictorPath);
            LoadKnownFaces();
            // Constructor logic here
        }

        private void LoadKnownFaces()
        {
            var directory = Path.Combine(Directory.GetCurrentDirectory(), "Data", "KnownFaces");
            string[] knownNames = { "Joe", "Kamala", "Obama1", "Obama2", "Donald Trump" };
            string[] knownImagePaths = Directory.GetFiles(directory, "*.jpg");

            for (int i = 0; i < knownImagePaths.Length; i++)
            {
                // Convert known image to Dlib image
                using (var image = Dlib.LoadImage<RgbPixel>(knownImagePaths[i]))
                {
                    var faceDetections = Dlib.GetFrontalFaceDetector().Operator(image);
                    if (faceDetections.Length > 0)
                    {
                        //Detect facial landmarks
                        var shape = _shapePredictor.Detect(image, faceDetections[0]);
                        var result = image.ToBitmap();

                        var faceChipDetails = Dlib.GetFaceChipDetails(shape, 150);
                        var faceChip = Dlib.ExtractImageChip<RgbPixel>(image, faceChipDetails);
                        result = faceChip.ToBitmap();


                        DlibDotNet.Matrix<RgbPixel> matrix = result.ToMatrix<RgbPixel>();
                        var faceDescriptor = _net.Operator(matrix);
                        _knownFaceEmbeddings.Add(knownNames[i], faceDescriptor.ToArray().SelectMany(x => x.ToArray()).ToArray());

                    }
                }
            }
        }

        public async Task ProcessFrames(VideoCapture capture, CancellationToken token, bool isIn)
        {
            var outFrame = new Image<Bgr, byte>(640, 480);
            var faceDetector = Dlib.GetFrontalFaceDetector();

            while (!token.IsCancellationRequested)
            {
                using (var predictor = YoloV8Predictor.Create("Utils/Recognition/yolov8n.onnx"))
                {
                        using (var predictorFace = YoloV8Predictor.Create("Utils/Recognition/yolov8n-face.onnx"))
                        {
                            while (true)
                            {
                                //wait for new weebsocket connection
                                if(isIn)
                                {
                                    if (GlobalService.wsIn == null || GlobalService.wsIn.State != WebSocketState.Open)
                                    {
                                        Console.WriteLine("Waiting for websocket In connection");
                                        await Task.Delay(1000);
                                        continue;
                                    }

                                }
                                else{
                                    if (GlobalService.wsOut == null || GlobalService.wsOut.State != WebSocketState.Open)
                                    {
                                        Console.WriteLine("Waiting for websocket Out connection");
                                        await Task.Delay(1000);
                                        continue;
                                    }
                                }

                                try
                                {
                                    using (var frame = capture.QueryFrame().ToImage<Bgr, byte>())
                                    {

                                        if (frame == null)
                                        {
                                            Console.WriteLine("Failed to capture frame.");
                                            await Task.Delay(2000);
                                            break;
                                        }
                                        double scaleX = 640.0 / frame.Width;
                                        double scaleY = 480.0 / frame.Height;
                                        scale = Math.Min(scaleX, scaleY);

                                        await Task.Run(() => ProcessFrame(frame, out outFrame, faceDetector, predictor, predictorFace));

                                        // Display the result
                                        //CvInvoke.Imshow("Real-Time Face Detection", frame);

                                        using var ms = new MemoryStream();
                                        outFrame.ToBitmap().Save(ms, System.Drawing.Imaging.ImageFormat.Png);
                                        string base64Image = Convert.ToBase64String(ms.ToArray());

                                        var message = new { type = "frame", data = base64Image };
                                        var jsonMessage = System.Text.Json.JsonSerializer.Serialize(message);

                                        //Console.WriteLine($"Output frame size is {outFrame.Size} ");
                                        switch (isIn)
                                        {
                                            case true:
                                                if (GlobalService.wsIn.State == WebSocketState.Open)
                                                {
                                                    await GlobalService.wsIn.SendAsync(
                                                        new ArraySegment<byte>(Encoding.UTF8.GetBytes(jsonMessage)), 
                                                        WebSocketMessageType.Text, 
                                                        true, 
                                                        CancellationToken.None
                                                    );
                                                    Console.WriteLine("Sent frame In to client");
                                                }
                                                else
                                                {
                                                    Console.WriteLine("WebSocket is closed");
                                                }
                                                break;  // Terminate the case after execution

                                            case false:

                                            if (GlobalService.wsOut.State == WebSocketState.Open)
                                                {
                                                    await GlobalService.wsOut.SendAsync(
                                                        new ArraySegment<byte>(Encoding.UTF8.GetBytes(jsonMessage)), 
                                                        WebSocketMessageType.Text, 
                                                        true, 
                                                        CancellationToken.None
                                                    );
                                                    Console.WriteLine("Sent frame Out to client");
                                                }
                                                else
                                                {
                                                    Console.WriteLine("WebSocket is closed");
                                                }
                                                break;  // Terminate the case after execution
                                        }


                                        


                                    }
                                }
                                catch (Exception e)
                                {
                                    Console.WriteLine($"Error processing frame: {e.Message}");
                                    await Task.Delay(1000);
                                    break;
                                }
                            }
                        }

                    }
            }
        }

        private void ProcessFrame(Image<Bgr, Byte> frame, out Image<Bgr, Byte> OutputFrame, FrontalFaceDetector faceDetector, YoloV8Predictor predictor, YoloV8Predictor predictorFace)
        {
            //remove duplicate names
            _recognizedNames.Clear();

            //scale the frame to 640x480

            int width = (int)(frame.Width * scale);
            int height = (int)(frame.Height * scale);
            CvInvoke.Resize(frame, frame, new Size(width, height), 0, 0, Inter.Linear);
            using (var resizedFrame = new Mat())
            {
                CvInvoke.Resize(frame, resizedFrame, new Size(width, height), 0, 0, Inter.Linear);

                frame = resizedFrame.ToImage<Bgr, byte>();
                //Console.WriteLine($"Processed frame size is {frame.Size} target size is {width}x{height}");
            }
            // Convert the frame to grayscale
            using (var grayFrame = frame.Convert<Gray, byte>())
            {

                // Create a Dlib image
                using (var dlibImage = new DlibDotNet.Matrix<RgbPixel>(grayFrame.Height, grayFrame.Width))
                {

                    // Get the byte array from the frame
                    var bytes = frame.Bytes;

                    // Fill the Dlib image
                    for (int y = 0; y < grayFrame.Height; y++)
                    {
                        for (int x = 0; x < grayFrame.Width; x++)
                        {
                            // Calculate the pixel index in the byte array (BGR format)
                            int index = (y * frame.Width + x) * frame.NumberOfChannels;
                            byte blue = bytes[index];
                            byte green = bytes[index + 1];
                            byte red = bytes[index + 2];
                            dlibImage[y, x] = new RgbPixel(red, green, blue);
                        }
                    }

                    // Detect faces
                    //var faces = faceDetector.Operator(dlibImage);
                    var image = SixLabors.ImageSharp.Image.Load(ConvertToImageSharp(frame));
                    var faces = Task.Run(() => predictorFace.DetectAsync(image)).Result;



                    if (faces.Boxes.Length > 0)
                    {
                        Task.Run(() =>
                        {
                            RecognizeAndDisplay(faces.Boxes, dlibImage.Clone());
                        });


                        Console.WriteLine($"Recognized names: {_recognizedNames.Count} and faces: {faces.Boxes.Length}");
                        if (_recognizedNames.Count > 2)
                        {
                            Console.WriteLine($"First Name: {_recognizedNames.First().Value} and Second Name: {_recognizedNames.ElementAt(1).Value}");
                        }

                        foreach (var recognizedFace in _recognizedNames)
                        {
                            var rect = recognizedFace.Key;
                            CvInvoke.Rectangle(frame, new System.Drawing.Rectangle(rect.Left, rect.Top, (int)rect.Width, (int)rect.Height), new MCvScalar(Color.Red.B, Color.Red.G, Color.Red.R), 2);
                            CvInvoke.PutText(frame, recognizedFace.Value, new System.Drawing.Point(rect.Left, rect.Top - 10), FontFace.HersheySimplex, 0.5, new MCvScalar(Color.Yellow.B, Color.Yellow.G, Color.Yellow.R), 1);
                        }


                    }


                    var detectionResult = Task.Run(() => predictor.DetectAsync(image)).Result;
                    detectionResult.Boxes.ToList().RemoveAll(box => box.Confidence < 0.5);
                    detectionResult.Boxes.ToList().ForEach(box =>
                    {
                        var rect = new System.Drawing.Rectangle(box.Bounds.X, box.Bounds.Y, box.Bounds.Width, box.Bounds.Height);
                        CvInvoke.Rectangle(frame, rect, new MCvScalar(0, 255, 0), 2);
                        CvInvoke.PutText(frame, $"{box.Class.Name}({box.Confidence * 100}%)", new System.Drawing.Point(box.Bounds.X, box.Bounds.Y - 10), FontFace.HersheySimplex, 0.5, new MCvScalar(0, 0, 255), 1);
                    });
                    OutputFrame = frame;
                }
            }
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

        private void RecognizeAndDisplay(Compunet.YoloV8.Data.BoundingBox[] faces, DlibDotNet.Matrix<RgbPixel> dlibImage)
        {


            List<float[]> faceDescriptors = new List<float[]>();
            //Console.WriteLine("Recognizing...");

            try
            {

                foreach (var face in faces)
                {
                    DlibDotNet.Rectangle faceRect = ConvertBoundingBoxToDlibRect(face);
                    var shape = _shapePredictor.Detect(dlibImage, faceRect);
                    var faceChipDetails = Dlib.GetFaceChipDetails(shape, 150);
                    var faceChip = Dlib.ExtractImageChip<RgbPixel>(dlibImage, faceChipDetails);


                    using (var result = faceChip.ToBitmap())
                    {
                        DlibDotNet.Matrix<RgbPixel> matrix = result.ToMatrix<RgbPixel>();
                        var faceDescriptor = _net.Operator(matrix);
                        faceDescriptors.Add(faceDescriptor.ToArray().SelectMany(x => x.ToArray()).ToArray());
                    }
                }
                Task.Run(() =>
                {
                    RecognizeFace(faceDescriptors, faces);
                });



            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
            finally
            {
                dlibImage.Dispose();
            }



        }

        private void RecognizeFace(List<float[]> faceDescriptors, Compunet.YoloV8.Data.BoundingBox[] faces)
        {
            double threshold = 0.5;
            double distance = double.MaxValue;
            string result = "Unknown";
            double distanceUknown = 0.0;
            int i = 0;
            // Compare the face descriptor with known embeddings
            foreach (var faceDescriptor in faceDescriptors)
            {
                // Compare the face descriptor with known embeddings
                foreach (var kvp in _knownFaceEmbeddings)
                {
                    var name = kvp.Key;
                    var knownEmbedding = kvp.Value;

                    // Compute the distance (e.g., Euclidean distance) between the embeddings
                    var distCalculated = CalculateEuclideanDistance(faceDescriptor, knownEmbedding);
                    distanceUknown = distCalculated;
                    // Set a threshold for recognition
                    if (distCalculated <= threshold && distCalculated <= distance) // Adjust threshold based on testing
                    {
                        //Beep each time a face is recognized
                        distance = distCalculated;
                        result = name;
                    }

                }
                _recognizedNames[ConvertBoundingBoxToDlibRect(faces[i])] = result;
                i++;

            }
            //Console.WriteLine($"Distance Unknown: {distanceUknown}");
            if (result != "Unknown")
            {
                Console.Beep();
                Console.WriteLine($"Recognized: {result} ({distance})");

            }
        }

        private double CalculateEuclideanDistance(float[] a, float[] b)
        {
            double sum = 0.0;
            for (int i = 0; i < a.Length; i++)
            {
                sum += Math.Pow(a[i] - b[i], 2);
            }
            return Math.Sqrt(sum);
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

        public async Task<List<EmployeeImage>> GetEmployeeImages()
        {
            return await _employeeImages.Find(image => true).ToListAsync();
        }

        public async Task<List<EmployeeImage>> GetEmployeeImage(int id)
        {
            return await _employeeImages.Find<EmployeeImage>(image => image.IdStaff == id).ToListAsync(); 
        }
    }
}