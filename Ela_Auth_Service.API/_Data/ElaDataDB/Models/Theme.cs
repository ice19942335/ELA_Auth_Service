using System.Collections.Generic;

namespace ELA_Auth_Service._Data.ElaDataDB.Models
{
    public partial class Theme
    {
        public Theme()
        {
            Lessons = new HashSet<Lessons>();
        }

        public int Id { get; set; }
        public string Name { get; set; }

        public virtual ICollection<Lessons> Lessons { get; set; }
    }
}
