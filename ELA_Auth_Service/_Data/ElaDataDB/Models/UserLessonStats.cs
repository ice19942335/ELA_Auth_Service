namespace ELA_Auth_Service._Data.ElaDataDB.Models
{
    public partial class UserLessonStats
    {
        public int Id { get; set; }
        public string UsersId { get; set; }
        public int LessonsId { get; set; }
        public int ComplTasksCounter { get; set; }

        public virtual Lessons Lessons { get; set; }
        public virtual Users Users { get; set; }
    }
}
