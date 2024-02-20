using Microsoft.EntityFrameworkCore.Metadata.Conventions;
using System;
using System.Globalization;
using System.Linq.Expressions;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;
using System.Text.RegularExpressions;
using System.Threading.Channels;
using tryME;

public class Program
{
    static void Main()
    {
        var context = new ApplicationDBcontext();
        bool t = true;


        while (t)
        {
            Console.WriteLine("hello user please choose what you want to do ");
            Console.WriteLine("1-add new student");
            Console.WriteLine("2-remove student");
            Console.WriteLine("3- add course");
            Console.WriteLine("4-assign student to course");
            Console.WriteLine("5-grade student on course");
            Console.WriteLine("6-display courses for the student");
            Console.WriteLine("7-Exist the program");

            string c = Console.ReadLine();
            int choice = Int32.Parse(c);


            switch (choice)
            {
                case 1:
                    try
                    {
                        AddStudent(context);
                    }
                    catch
                    {
                        Console.WriteLine("invalid info");
                    }
                    break;
                case 2:
                    RemoveStudent(context);
                    break;
                case 3:
                    AddCourse(context);
                    break;
                case 4:
                    AssignStudentToCourse(context);
                    break;
                case 5:
                    GradeStudent(context);
                    break;
                case 6:
                    DisplayCourses(context);
                    break;
                default:
                    t = false;
                    break;
                    

            }
            try
            {
                context.SaveChanges();
            }
            catch   (Exception ex) {
                 Console.WriteLine(ex.ToString());
            }
            Console.WriteLine("\n");

        }
        static void AddStudent(ApplicationDBcontext context)
        {
            Console.WriteLine("please enter the student ID");
            int id = Int32.Parse(Console.ReadLine());

            bool exists = context.Students.Any(s => s.StudentId == id);

            while (exists)
            {
                Console.WriteLine("this student already exists please enter another id");
                id = Int32.Parse(Console.ReadLine());
                exists = context.Students.Any(s => s.StudentId == id);
            }

            Console.WriteLine("please enter the student name");
            string name = Console.ReadLine();
           
            Console.WriteLine("please enter the student level ");
            int level = Int32.Parse(Console.ReadLine());

            while(level > 4 || level < 0 ) {
                Console.WriteLine(" invalid level please enter a valid student level ");
                 level = Int32.Parse(Console.ReadLine());
            }


            var Student = new Student() { StudentId = id, StudentName = name, level = level };
          
            context.Students.Add(Student);


        }

        static void RemoveStudent(ApplicationDBcontext context)
        {
            Console.WriteLine("please enter the student ID");
            int id = Int32.Parse(Console.ReadLine());

            bool exists = context.Students.Any(s => s.StudentId == id);

            while (!exists)
            {
                
                Console.WriteLine("this student doesn't exist please enter another id");
                id = Int32.Parse(Console.ReadLine());
                exists = context.Students.Any(s => s.StudentId == id);
            }



            var student = context.Students.Find(id);

            context.Students.Remove(student);

            Console.WriteLine($"student  {student.StudentName} has been deleted");

        }

        static void AddCourse(ApplicationDBcontext context)
        {
            Console.WriteLine("please enter the course id");
            string id = Console.ReadLine();
           

            bool exists = context.courses.Any(s => s.courseId == id);

            while (exists)
            {
                Console.WriteLine("this course doesn't exists please enter a correct course id");
                 id = Console.ReadLine();
                 exists = context.courses.Any(s => s.courseId == id);
            }
            Console.WriteLine("please enter the name of the course name");
            string name = Console.ReadLine();

            Console.WriteLine("please enter the course department");
            string department = Console.ReadLine();

            var course = new Course() { courseId = id, courseName = name, departments = department };

            context.courses.Add(course);



        }


        static void AssignStudentToCourse(ApplicationDBcontext context)
        {
            Console.WriteLine("please enter the student id");
            int SID = Int32.Parse(Console.ReadLine());

            bool exists1 = context.Students.Any(s => s.StudentId == SID);

            while (!exists1)
            {
                Console.WriteLine("this student doesn't exists please enter a valid id");
                SID = Int32.Parse(Console.ReadLine());
                exists1 = context.Students.Any(s => s.StudentId == SID);
            }

            Console.WriteLine("please enter the name of the course id");
            string CID = Console.ReadLine();

          

            bool exists = context.courses.Any(s => s.courseId == CID);

            if (!exists)
            {
                Console.WriteLine("this course doesn't exists please enter a valid course id");
                CID = Console.ReadLine();
                exists = context.courses.Any(s => s.courseId == CID);

            }

            Console.WriteLine("please enter the year of the course");
            int year = Int32.Parse(Console.ReadLine());

            while(year < 0 || year > 2023 ) {
                Console.WriteLine("invalid year please enter a valid year");
                 year = Int32.Parse(Console.ReadLine());
            }


            Console.WriteLine("please enter the semester from Either fall or spring");
            string semester = Console.ReadLine();

            while(!(semester.Equals("fall")||semester.Equals("spring")))
            {
                Console.WriteLine("invalid semester please enter fall or spring");
                 semester = Console.ReadLine();
            }



            var student = context.Students.Find(SID);

            var course = context.courses.Find(CID);

            var learning = new Learns() { courseId = course.courseId, StudentId = student.StudentId, year = year, semester = semester };

            context.Learns.Add(learning);




        }

        static void GradeStudent(ApplicationDBcontext context)
        {

            Console.WriteLine("please enter the student id");
            int SID = Int32.Parse(Console.ReadLine());

            bool exists1 = context.Students.Any(s => s.StudentId == SID);

            while (!exists1)
            {
                Console.WriteLine("this student doesn't exists please enter a valid id");
                SID = Int32.Parse(Console.ReadLine());
                exists1 = context.Students.Any(s => s.StudentId == SID);
            }

            Console.WriteLine("please enter the name of the course id");
            string CID = Console.ReadLine();



            bool exists = context.courses.Any(s => s.courseId == CID);

            if (!exists)
            {
                Console.WriteLine("this course doesn't exists please enter a valid course id");
                CID = Console.ReadLine();
                exists = context.courses.Any(s => s.courseId == CID);

            }



            Console.WriteLine("please enter the year of the course");
            int year = Int32.Parse(Console.ReadLine());

            while (year < 0 || year > 2023)
            {
                Console.WriteLine("invalid year please enter a valid year");
                year = Int32.Parse(Console.ReadLine());
            }


            Console.WriteLine("please enter the semester from Either fall or spring");
            string semester = Console.ReadLine();

            while (!(semester.Equals("fall") || semester.Equals("spring")))
            {
                Console.WriteLine("invalid semester please enter fall or spring");
                semester = Console.ReadLine();
            }

            var learning = context.Learns.Find(SID, CID,semester , year);

            Console.WriteLine("please enter the student grade");

            float grade = float.Parse(Console.ReadLine());

            while (grade > 4.00 || grade < 0.00)
            {
                Console.WriteLine("invalid grade please enter the grade between 0 and 4");

            }

                learning.grade = grade;




        }

        static void DisplayCourses(ApplicationDBcontext context)
        {

            Console.WriteLine("please enter the student id");
            int SID = Int32.Parse(Console.ReadLine());
            bool exists = context.Students.Any(s => s.StudentId == SID);

            while (!exists)
            {

                Console.WriteLine("this student doesn't exist please enter another id");
                SID = Int32.Parse(Console.ReadLine());
                exists = context.Students.Any(s => s.StudentId == SID);
            }

            var courses = context.Learns.Where(c => c.StudentId == SID).ToList();

            if(!courses.Any() )
            {
                Console.WriteLine("this student does not have any courses");

            }

            foreach (var course in courses)
            {
                Console.WriteLine($"{course.courseId},{course.year},{course.semester}");
            }


        }
    }
}



