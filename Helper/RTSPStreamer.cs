using System;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using Emgu.CV;
using Emgu.CV.Structure;

public class RTSPStreamer
{
    private Process ffmpegProcess;
    private StreamWriter ffmpegInputWriter;
    private bool isStreaming = false;

    // RTSP URL to stream to
    private string rtspUrl = "rtsp://localhost:8554/stream"; // Change with your RTSP server address

    // Path to FFmpeg executable
    private string ffmpegPath = "ffmpeg.exe"; // Path to FFmpeg binary

    public void StartStreaming()
    {
        if (isStreaming) return;

        // Start FFmpeg process
        var startInfo = new ProcessStartInfo
        {
            FileName = ffmpegPath,
            Arguments = "-f rawvideo -pix_fmt yuv420p -s 640x480 -r 30 -i - -c:v libx264 -f rtsp " + rtspUrl,
            RedirectStandardInput = true,
            RedirectStandardOutput = true,  // Capture FFmpeg output
            RedirectStandardError = true,
            UseShellExecute = false,
            CreateNoWindow = true
        };

        ffmpegProcess = Process.Start(startInfo);
        ffmpegInputWriter = new StreamWriter(ffmpegProcess.StandardInput.BaseStream);
        isStreaming = true;

        // Log FFmpeg error output (async)
        Task.Run(async () =>
        {
            string stderr = await ffmpegProcess.StandardError.ReadToEndAsync();
            Console.WriteLine("FFmpeg error: " + stderr);
        });
    }

    public async Task StreamFrameAsync(Mat frame)
    {
        if (!isStreaming) return;

        // Convert the frame to YUV420p format
        byte[] yuvFrame = ConvertToYuv420p(frame);

        // Send the frame to the FFmpeg process via standard input
        await ffmpegInputWriter.BaseStream.WriteAsync(yuvFrame, 0, yuvFrame.Length);
        await ffmpegInputWriter.BaseStream.FlushAsync();
    }

    public void StopStreaming()
    {
        if (!isStreaming) return;

        // Close the FFmpeg process and input writer
        ffmpegInputWriter.Close();
        ffmpegProcess.WaitForExit();
        ffmpegProcess.Close();

        isStreaming = false;
    }

    // Convert frame to YUV420p format, compatible with FFmpeg
    private byte[] ConvertToYuv420p(Mat frame)
    {
        // You can use EmguCV to convert BGR frame to YUV420p
        var yuvImage = new Mat();
        CvInvoke.CvtColor(frame, yuvImage, Emgu.CV.CvEnum.ColorConversion.Bgr2Yuv);

        // Convert Mat to byte array (raw YUV data)
        var yuvBytes = new byte[yuvImage.Total * yuvImage.ElementSize];
        System.Runtime.InteropServices.Marshal.Copy(yuvImage.DataPointer, yuvBytes, 0, yuvBytes.Length);

        return yuvBytes;
    }
}
