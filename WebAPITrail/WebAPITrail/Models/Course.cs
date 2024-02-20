using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebAPITrail
{
    public  class Course
    {
        public string courseId { get; set; }

        public string courseName { get; set; }

        public string departments { get; set; }

        public List<Learns> learn { get; set;}

        public ICollection<Student> Students { get; private set; }




    }
    public class Student
    {

        public int StudentId { get; set; }
        public string StudentName { get; set; }
        public int level { get; set; }

        public byte[]  photo { get; set; }
        public List<Learns> learn { get; set; }
       
        public ICollection<Course> Courses { get; private set; }


    }



    public class Learns { 
        public Course course { get; set; }

        public Student student { get; set; } 

        public int StudentId { get; set; }

        public string courseId { get; set; }


        public int year { get; set; }

        public string semester { get; set; }

        public float? grade { get; set; }

        
       

       
    }
}
