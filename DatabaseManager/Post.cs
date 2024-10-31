namespace DatabaseManager
{
    public class Post
    {
        public int Id { get; set; }
        public required string Name { get; set; }

        // Навигационное свойство: одна должность может быть у нескольких сотрудников
        public List<Employee> Employees { get; set; } = new List<Employee>();
    }
}
