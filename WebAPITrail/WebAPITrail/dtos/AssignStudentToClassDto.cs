namespace WebAPITrail.dtos
{
    public class AssignStudentToClassDto
    {
        public int StudentId { get; set; }
        public string courseName { get; set; }

        public string courseId { get; set; }

        public int year { get; set; }

        public string semester { get; set; }

        public float? grade { get; set; }

    }
}
