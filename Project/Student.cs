using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project
{
    internal class Student
    {
        private int id;
        public int Id { get { return id; } set { return; } }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Major { get; set; }
 

        public List<Course> Courses { get; set; }

        public Student(int id,string firstName, string lastName,string major)
        {
            this.id = id;
            FirstName = firstName;
            LastName = lastName;
            Major = major;
            Courses = new List<Course>();    
        }
    }
}
