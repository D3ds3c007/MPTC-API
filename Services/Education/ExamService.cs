using System.ComponentModel.DataAnnotations;
using System.Text;
using System.Text.Json;
using Microsoft.AspNetCore.Identity;
using MPTC_API.Data;
using MPTC_API.Models.Education;
using MPTC_API.Models.DTO;
using MPTC_API.Services.Authentication;

namespace MPTC_API.Services.Attendance
{
    public class ExamService
    {
        public static List<ExamDTO> listExams(MptcContext context)
        {
            List<ExamDTO> examDTOs = new List<ExamDTO>();
            List<Exam> exams = context.Exams.ToList();
            
            foreach (Exam exam in exams)
            {
                ExamDTO examDTO = new ExamDTO();

                examDTO.IdExam = exam.IdExam;
                examDTO.Period = DataService.FormatDateRange(exam.Period.BeginDate, exam.Period.EndDate);
                examDTO.Session = ExamService.setSessionName(exam.Session);
                examDTO.Subject = exam.Subject.SubjectName;
                examDTO.Level = exam.Level.LevelName;
                examDTO.Uripath = exam.Uripath;
                examDTO.UripathAssetNote = exam.UripathAssetNote;
                examDTO.DateExam = exam.DateCreated;
                examDTO.StaffId = exam.StaffId;

                examDTOs.Add(examDTO);
            }
            return examDTOs;
        }

        public static string setSessionName(int sessionNumber){
            string[] sessionName = { "TERM1", "TERM2", "FINAL TERM" };
            string sessionString = "";
            for(int i=0; i<sessionName.Count(); i++){
                if(i+1 == sessionNumber){
                    sessionString = sessionName[i];
                }
            }
            return sessionString;
        }

        public static void createExam(Exam exam, MptcContext _context)
        {
            //create new exam
            _context.Exams.Add(exam);
            _context.SaveChangesAsync();
        }

        // public static List<ExamDTO> toExamDTO(List<Exam> exams)
        // {
        //     List<ExamDTO> examDTOs = new List<ExamDTO>();

        //     foreach (Exam exam in exams)
        //     {
        //         ExamDTO examDTO = new ExamDTO();

        //         examDTO.IdExam = exam.IdExam;
        //         examDTO.Period = DataService.FormatDateRange(exam.Period.BeginDate, exam.Period.EndDate);
        //         examDTO.Session = exam.Session;
        //         examDTO.Subject = exam.Subject.SubjectName;
        //         examDTO.Level = exam.Level.LevelName;
        //         examDTO.Uripath = exam.Uripath;
        //         examDTO.UripathAssetNote = exam.UripathAssetNote;
        //         examDTO.DateExam = exam.DateCreated;
        //         examDTO.StaffId = exam.StaffId;

        //         examDTOs.Add(examDTO);
        //     }

        //     return examDTOs;
        // }

        public static async Task<string> UploadPDFAsync(IFormFile PDFfile)
        {
            string destinationPath = "./Temp/";

            if (PDFfile == null || Path.GetExtension(PDFfile.FileName).ToLower() != ".pdf")
            {
                return "Invalid file. Only PDF files are allowed.";
            }

            // Ensure the destination directory exists
            if (!Directory.Exists(destinationPath))
            {
                Directory.CreateDirectory(destinationPath);
            }

            // Full path where the uploaded PDF file will be saved
            var pdfFilePath = Path.Combine(destinationPath, PDFfile.FileName);

            // Save the PDF file to the destination directory
            try
            {
                using (var fileStream = new FileStream(pdfFilePath, FileMode.Create))
                {
                    await PDFfile.CopyToAsync(fileStream);
                }
            }
            catch (Exception ex)
            {
                return $"Error while saving the file: {ex.Message}";
            }

            return pdfFilePath;
        }

        public static void updateExam(Exam exam, MptcContext _context){
       
            Exam existingExam = _context.Exams.Find(exam.IdExam);
            if (existingExam == null)
            {
                throw new InvalidOperationException("Exam does not exist");
            }

            // Update properties of the existing exam
            existingExam.PeriodId = exam.PeriodId;
            existingExam.Session = exam.Session;
            existingExam.SubjectId = exam.SubjectId;
            existingExam.LevelId = exam.LevelId;
            existingExam.SubjectId = exam.SubjectId;
            existingExam.UripathAssetNote = exam.UripathAssetNote;
            existingExam.Uripath = exam.Uripath;
            existingExam.DateCreated = exam.DateCreated;
            existingExam.StaffId = exam.StaffId;
            // Add other properties to update as needed

            // Save changes to the database
            _context.SaveChanges();

        }

        public static void deleteExam(int idExam, MptcContext _context){
            var exam = _context.Exams.Find(idExam);

            if (exam == null)
            {
                //trows exception
                throw new InvalidOperationException("Exam does not exist");
            }

            // Remove the exam from the database
            _context.Exams.Remove(exam);

            // Save changes to the database
            _context.SaveChanges();
        }

        // public static async Task<string> UploadZipFile(IFormFile zipFile){
            // string destinationPath = "./Temp/";

            // if (PDFfile == null || Path.GetExtension(PDFfile.FileName).ToLower() != ".pdf")
            // {
            //     return "Invalid file. Only PDF files are allowed.";
            // }

            // // Ensure the destination directory exists
            // if (!Directory.Exists(destinationPath))
            // {
            //     Directory.CreateDirectory(destinationPath);
            // }

            // // Full path where the uploaded PDF file will be saved
            // var pdfFilePath = Path.Combine(destinationPath, PDFfile.FileName);

            // // Save the PDF file to the destination directory
            // try
            // {
            //     using (var fileStream = new FileStream(pdfFilePath, FileMode.Create))
            //     {
            //         await PDFfile.CopyToAsync(fileStream);
            //     }
            // }
            // catch (Exception ex)
            // {
            //     return $"Error while saving the file: {ex.Message}";
            // }

            // return pdfFilePath;
        // }

    }

}
