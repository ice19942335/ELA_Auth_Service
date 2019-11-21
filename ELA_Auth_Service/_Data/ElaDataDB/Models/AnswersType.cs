using System.Collections.Generic;

namespace ELA_Auth_Service._Data.ElaDataDB.Models
{
    public partial class AnswersType
    {
        public AnswersType()
        {
            Answers = new HashSet<Answers>();
        }

        public int Id { get; set; }
        public string Type { get; set; }

        public virtual ICollection<Answers> Answers { get; set; }
    }
}
