namespace DatabaseManager
{
    public class Employee
    {
        public int Id { get; set; }
        public required string Fio { get; set; }
        
        public int DivisionId { get; set; }
        public Division? Division { get; set; } // Внешний ключ и навигационное свойство к Division
        
        public int PostId { get; set; }
        public Post? Post { get; set; } // Внешний ключ и навигационное свойство к Post
        
        public DateTime DateHire { get; set; }
        public DateTime? DateDismissal { get; set; } = null;
    }
}
