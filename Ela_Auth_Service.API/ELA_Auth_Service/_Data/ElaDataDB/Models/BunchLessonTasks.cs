namespace ELA_Auth_Service._Data.ElaDataDB.Models
{
    public partial class BunchLessonTasks
    {
        public int Id { get; set; }
        public int LessonsId { get; set; }
        public int TasksId { get; set; }

        public virtual Lessons Lessons { get; set; }
        public virtual Tasks Tasks { get; set; }
    }
}
