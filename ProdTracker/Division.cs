namespace ProdTracker
{
    public class Division
    {
        public int Id { get; set; }
        public required string Name { get; set; }

        // Навигационное свойство: один отдел может иметь множество сотрудников
        public List<Employee> Employees { get; set; } = new List<Employee>();
    }
}
