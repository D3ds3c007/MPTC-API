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

        public static void createExam(MptcContext context)
        {
            // List<ExamDTO> examDTOs = new List<ExamDTO>();

            // List<Exam> exams = context.Exams.Add(exam);
            // foreach (Exam exam in exams)
            // {
            //     ExamDTO examDTO = new ExamDTO();

            //     examDTO.IdExam = exam.IdExam;
            //     examDTO.Period = exam.Period;
            //     examDTO.Session = exam.Session;
            //     examDTO.Subject = exam.Subject.SubjectName;
            //     examDTO.Level = exam.Level.LevelName;
            //     examDTO.Uripath = exam.Uripath;
            //     examDTO.UripathAssetNote = exam.UripathAssetNote;
            //     examDTO.DateExam = exam.DateCreated;
            //     examDTO.StaffId = exam.StaffId;

            //     examDTOs.Add(examDTO);
            // }
        }
    }

}
