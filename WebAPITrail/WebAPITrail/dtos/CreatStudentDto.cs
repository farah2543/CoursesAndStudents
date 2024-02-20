namespace WebAPITrail.dtos
{
    public class CreatStudentDto
    {
        public int StudentId { get; set; }
        public string StudentName { get; set; }
        public int level { get; set; }

        public IFormFile photo { get; set; }
    }
}
