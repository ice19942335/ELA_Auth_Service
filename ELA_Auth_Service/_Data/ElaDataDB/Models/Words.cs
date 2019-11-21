using System.Collections.Generic;

namespace ELA_Auth_Service._Data.ElaDataDB.Models
{
    public partial class Words
    {
        public Words()
        {
            Answers = new HashSet<Answers>();
        }

        public int Id { get; set; }
        public string Eng { get; set; }
        public string Rus { get; set; }
        public string Transcript { get; set; }
        public string Audio { get; set; }
        public string ImgUrl { get; set; }

        public virtual ICollection<Answers> Answers { get; set; }
    }
}
