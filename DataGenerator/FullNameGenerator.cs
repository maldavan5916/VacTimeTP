using Bogus;

namespace DataGenerator
{
    public class FullNameGenerator
    {
        private readonly Faker _faker;
        private readonly List<string> _malePatronymics = new List<string>
    {
        "Александрович", "Сергеевич", "Иванович", "Дмитриевич", "Николаевич",
        "Петрович", "Андреевич", "Владимирович", "Михайлович", "Юрьевич"
    };

        private readonly List<string> _femalePatronymics = new List<string>
    {
        "Александровна", "Сергеевна", "Ивановна", "Дмитриевна", "Николаевна",
        "Петровна", "Андреевна", "Владимировна", "Михайловна", "Юрьевна"
    };

        public FullNameGenerator()
        {
            _faker = new Faker("ru");
        }

        public string GenerateFullName()
        {
            var gender = _faker.PickRandom<Bogus.DataSets.Name.Gender>();
            var lastName = _faker.Name.LastName(gender);
            var firstName = _faker.Name.FirstName(gender);
            var patronymic = gender == Bogus.DataSets.Name.Gender.Male
                ? _faker.PickRandom(_malePatronymics)
                : _faker.PickRandom(_femalePatronymics);

            return $"{lastName} {firstName} {patronymic}";
        }
    }
}