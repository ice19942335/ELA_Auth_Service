using System.Collections.Generic;

namespace ELA_Auth_Service._Data.ElaDataDB.Models
{
    public partial class Tasks
    {
        public Tasks()
        {
            Answers = new HashSet<Answers>();
            BunchLessonTasks = new HashSet<BunchLessonTasks>();
        }

        public int Id { get; set; }
        public int TaskTypesId { get; set; }

        public virtual TaskTypes TaskTypes { get; set; }
        public virtual ICollection<Answers> Answers { get; set; }
        public virtual ICollection<BunchLessonTasks> BunchLessonTasks { get; set; }
    }
}
