using System.Collections.Generic;

namespace ELA_Auth_Service._Data.ElaDataDB.Models
{
    public partial class Lessons
    {
        public Lessons()
        {
            BunchLessonTasks = new HashSet<BunchLessonTasks>();
            UserLessonStats = new HashSet<UserLessonStats>();
        }

        public int Id { get; set; }
        public string Name { get; set; }
        public int ThemeId { get; set; }
        public int PointsToPay { get; set; }
        public string ImgUrl { get; set; }

        public virtual Theme Theme { get; set; }
        public virtual ICollection<BunchLessonTasks> BunchLessonTasks { get; set; }
        public virtual ICollection<UserLessonStats> UserLessonStats { get; set; }
    }
}
