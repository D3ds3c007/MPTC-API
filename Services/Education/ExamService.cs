using System.ComponentModel.DataAnnotations;
using System.Text;
using System.Text.Json;
using Microsoft.AspNetCore.Identity;
using MPTC_API.Data;
using MPTC_API.Models.Education;
using MPTC_API.Models.DTO;

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
                examDTO.Period = exam.Period;
                examDTO.Session = exam.Session;
                examDTO.Subject = exam.Subject;
                examDTO.Level = exam.Level;
                examDTO.Uripath = exam.Uripath;
                examDTO.UripathAssetNote = exam.UripathAssetNote;
                examDTO.DateExam = exam.DateCreated;
                examDTO.StaffId = exam.StaffId;

                examDTOs.Add(examDTO);
            }
            return examDTOs;
        }

        public static void createExam(Exam exam, MptcContext _context)
        {
            //create new exam
            _context.Exams.Add(exam);
            _context.SaveChangesAsync();
        }

        public static List<ExamDTO> toExamDTO(List<Exam> exams)
        {
            List<ExamDTO> examDTOs = new List<ExamDTO>();

            foreach (Exam exam in exams)
            {
                ExamDTO examDTO = new ExamDTO();

                examDTO.IdExam = exam.IdExam;
                examDTO.Period = exam.Period;
                examDTO.Session = exam.Session;
                examDTO.Subject = exam.Subject;
                examDTO.Level = exam.Level;
                examDTO.Uripath = exam.Uripath;
                examDTO.UripathAssetNote = exam.UripathAssetNote;
                examDTO.DateExam = exam.DateCreated;
                examDTO.StaffId = exam.StaffId;

                examDTOs.Add(examDTO);
            }

            return examDTOs;
        }

        public static async Task<string> UploadPDFAsync(IFormFile PDFfile)
        {
            string destinationPath = "Temp/";

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

    }

}
