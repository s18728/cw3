namespace cw3.Services
{
    public interface IDbService
    {
        public Enrollment enrollStudent(Student studentToEnroll, string studiesName);

        public Enrollment promoteStudents(int studiesId, int semester);

        public string getStudentsString();

        public bool modifyStudent(Student studentNewValue);

        public bool removeStudent(string eska);

    }
}
