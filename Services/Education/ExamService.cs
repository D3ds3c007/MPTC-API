using System.ComponentModel.DataAnnotations;
using System.Text;
using System.Text.Json;
using Microsoft.AspNetCore.Identity;
using MPTC_API.Data;
using MPTC_API.Models.Education;
using MPTC_API.Models.DTO;
using MPTC_API.Services.Authentication;
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Canvas.Parser;
using System.Text.RegularExpressions;

namespace MPTC_API.Services.Education
{
    public class ExamService
    {
        public static List<ExamDTO> listExams(MptcContext context)
        {
            List<ExamDTO> examDTOs = new List<ExamDTO>();
            List<Exam> exams = context.Exams.ToList();

            foreach (Exam exam in exams)
            {
                ExamDTO examDTO = ExamService.toExamDTO(exam);
                examDTOs.Add(examDTO);
            }
            return examDTOs;
        }

        public static List<ExamDTO> listExamsPerProf(MptcContext context, int StaffId)
        {
            List<ExamDTO> examDTOs = new List<ExamDTO>();
            List<Exam> exams = context.Exams.Where(e => e.StaffId == StaffId).ToList();

            foreach (Exam exam in exams)
            {
                ExamDTO examDTO = ExamService.toExamDTO(exam);
                examDTOs.Add(examDTO);
            }
            return examDTOs;
        }

        public static string setSessionName(int sessionNumber)
        {
            string[] sessionName = { "TERM1", "TERM2", "FINAL TERM" };
            string sessionString = "";
            for (int i = 0; i < sessionName.Count(); i++)
            {
                if (i + 1 == sessionNumber)
                {
                    sessionString = sessionName[i];
                }
            }
            return sessionString;
        }

        public static async Task<Exam> createExam(Exam exam, MptcContext _context)
        {
            _context.Exams.Add(exam);
            await _context.SaveChangesAsync(); // Save and update exam with its new Id
            return exam; // exam.Id and other fields will now be set
        }   


        public static ExamUpdateDTO getExamInfo(int examId, MptcContext _context)
        {
            Exam exam = _context.Exams.Find(examId);
            ExamUpdateDTO examUpdateDTO = ExamService.toExamUpdateDTO(exam);
            return examUpdateDTO;
        }

        public static ExamDTO toExamDTO(Exam exam)
        {
            ExamDTO examDTO = new ExamDTO();

            examDTO.IdExam = exam.IdExam;
            examDTO.Name = exam.Name;
            examDTO.Period = DataService.FormatDateRange(exam.Period.BeginDate, exam.Period.EndDate);
            examDTO.Session = ExamService.setSessionName(exam.Session);
            examDTO.Subject = exam.Subject.SubjectName;
            examDTO.Level = exam.Level.LevelName;
            examDTO.Uripath = exam.Uripath;
            examDTO.UripathAssetNote = exam.UripathAssetNote;
            examDTO.DateExam = exam.DateCreated.ToString("dd/MM/yyyy");
            examDTO.DateLastUpdate = exam.DateLastModified.ToString("dd/MM/yyyy");
            examDTO.StaffId = exam.StaffId;

            return examDTO;
        }

        public static ExamUpdateDTO toExamUpdateDTO(Exam exam)
        {
            ExamUpdateDTO examUpdateDTO = new ExamUpdateDTO();

            examUpdateDTO.IdExam = exam.IdExam;
            examUpdateDTO.PeriodId = exam.PeriodId;
            examUpdateDTO.Session = exam.Session;
            examUpdateDTO.SubjectId = exam.SubjectId;
            examUpdateDTO.LevelId = exam.LevelId;
            examUpdateDTO.Uripath = exam.Uripath;
            examUpdateDTO.UripathAssetNote = exam.UripathAssetNote;
            examUpdateDTO.DateCreated = exam.DateCreated;
            examUpdateDTO.StaffId = exam.StaffId;

            return examUpdateDTO;
        }

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

        public static void updateExam(Exam exam, MptcContext _context)
        {

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
            existingExam.DateCreated = exam.DateCreated;

            // Save changes to the database
            _context.SaveChanges();

        }

        public static void deleteExam(int idExam, MptcContext _context)
        {
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

        public static void validateExam(MptcContext context, Exam exam)
        {
            // Check duplicate name
            if (context.Exams.Any(e => e.Name == exam.Name))
            {
                throw new Exception("An exam with the same name already exists.");
            }

            // Check duplicate session in the same period, subject, and level
            if (context.Exams.Any(e =>
                e.PeriodId == exam.PeriodId &&
                e.Session == exam.Session &&
                e.SubjectId == exam.SubjectId &&
                e.LevelId == exam.LevelId))
            {
                throw new Exception("An exam with the same session, subject, level, and period already exists.");
            }

            // Check if date is within period range
            var selectedPeriod = context.Periods.FirstOrDefault(p => p.IdPeriod == exam.PeriodId);
            if (selectedPeriod == null)
            {
                throw new Exception("Selected period not found.");
            }

            //Check if the dateExam is in the range of the selected period
            if (exam.DateCreated < selectedPeriod.BeginDate || exam.DateCreated > selectedPeriod.EndDate)
            {
                throw new Exception("The exam date is outside the selected period's date range.");
            }

            Console.WriteLine("Exam Validated");
        }

        

    }

}
