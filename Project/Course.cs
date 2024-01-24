using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project
{
    internal class Course
    {
        public string CourseCode { get; set; }
        public string CourseName { get; set; }
        public string CourseInstructor { get; set; }
        public string CourseMajor { get; set; } 
        public int CourseCredits { get; set; }
        public string CourseSchedule { get; set; }  
        public Course(string courseCode, string courseName , string courseInstructor,string courseMajor,int courseCredits,string courseSchedule)
        {
            CourseCode = courseCode;
            CourseName = courseName;
            CourseInstructor = courseInstructor;
            CourseMajor = courseMajor;
            CourseCredits = courseCredits;
            CourseSchedule = courseSchedule;
        }


        public override string ToString()
        {
            return $"{CourseCode}: {CourseName} : Course Instructor: {CourseInstructor}";
        }
    }
}
