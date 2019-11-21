namespace ELA_Auth_Service._Data.ElaDataDB.Models
{
    public partial class Answers
    {
        public int Id { get; set; }
        public int AnswersTypeId { get; set; }
        public int WordsId { get; set; }
        public int TasksId { get; set; }

        public virtual AnswersType AnswersType { get; set; }
        public virtual Tasks Tasks { get; set; }
        public virtual Words Words { get; set; }
    }
}
