using DatabaseManager;
using Bogus;

namespace DataGenerator
{
    public class Program
    {
        public static void Main()
        {
            var context = new DatabaseContext();
            context.Database.EnsureCreated();

            var units = DatabaseSeeder.SeedUnits();
            var divisions = DatabaseSeeder.SeedDivisions();
            var posts = DatabaseSeeder.SeedPosts();
            var employees = DatabaseSeeder.SeedEmployees(divisions, posts);
            var locations = DatabaseSeeder.SeedLocations(employees, 5);
            var products = DatabaseSeeder.SeedProducts(units, locations);
            var materials = DatabaseSeeder.SeedMaterials(units, locations, 30);
            var prod_mater = DatabaseSeeder.SeedProduct_Materials(products, materials);
            var counterparties = DatabaseSeeder.SeedCounterparties(20);
            var receipts = DatabaseSeeder.SeedReceipts(materials, counterparties, 20);
            var contracts = DatabaseSeeder.SeedContracts(products, counterparties);
            var sales = DatabaseSeeder.SeedSales(contracts);

            context.Units.AddRange(units);
            context.Divisions.AddRange(divisions);
            context.Posts.AddRange(posts);
            context.Employees.AddRange(employees);
            context.Locations.AddRange(locations);
            context.Products.AddRange(products);
            context.Materials.AddRange(materials);
            context.Product_Materials.AddRange(prod_mater);
            context.Counterparties.AddRange(counterparties);
            context.Receipts.AddRange(receipts);
            context.Contracts.AddRange(contracts);
            context.Sales.AddRange(sales);
            
            context.SaveChanges();
        }
    }

    public static class DatabaseSeeder
    {
        public static List<Unit> SeedUnits()
        {
            return
            [
                new() { Name = "л",    Description = "Литр" },
                new() { Name = "пач",  Description = "Пачка" },
                new() { Name = "кг",   Description = "Килограмм" },
                new() { Name = "см",   Description = "Сантиметр" },
                new() { Name = "шт",   Description = "Штука" },
                new() { Name = "мкв",  Description = "Метр квадратный" },
                new() { Name = "бут",  Description = "Бутылка" },
                new() { Name = "г",    Description = "Грамм" },
                new() { Name = "м",    Description = "Метр" },
                new() { Name = "мм",   Description = "Миллиметр" },
                new() { Name = "упак", Description = "Упаковка" },
                new() { Name = "кор",  Description = "Коробка" },
                new() { Name = "рул",  Description = "Рулон" },
                new() { Name = "бан",  Description = "Банка" },
                new() { Name = "ящ",   Description = "Ящик" },
                new() { Name = "ед",   Description = "Единица" }
            ];
        }

        public static List<Division> SeedDivisions()
        {
            return
            [
                new() { Name = "Бухгалтерия" },
                new() { Name = "Руководство" },
                new() { Name = "Разработки" },
                new() { Name = "Сборки" },
                new() { Name = "Отладки" },
                new() { Name = "Склад" }
            ];
        }

        public static List<Post> SeedPosts()
        {
            return
            [
                new() { Name = "Директор" },               // Только 1
                new() { Name = "Заместитель директора" },  // Только 1
                new() { Name = "Главный бухгалтер" },      // Только 1
                new() { Name = "Бухгалтер" },
                new() { Name = "Инженер-конструктор" },
                new() { Name = "Инженер-программист" },
                new() { Name = "Механик" },
                new() { Name = "Сварщик" },
                new() { Name = "Монтажник" },
                new() { Name = "Наладчик" },
                new() { Name = "Кладовщик" }
            ];
        }

        public static List<Employee> SeedEmployees(List<Division> divisions, List<Post> posts)
        {
            var employees = new List<Employee>();
            var faker = new Faker("ru");

            // Специальные должности
            var director = posts.First(p => p.Name == "Директор");
            var deputy = posts.First(p => p.Name == "Заместитель директора");
            var chiefAccountant = posts.First(p => p.Name == "Главный бухгалтер");

            // Руководство
            var leadership = divisions.FirstOrDefault(d => d.Name == "Руководство")!;
            employees.Add(GenerateEmployee(faker, leadership, director));
            employees.Add(GenerateEmployee(faker, leadership, deputy));

            // Бухгалтерия
            var accounting = divisions.FirstOrDefault(d => d.Name == "Бухгалтерия")!;
            employees.Add(GenerateEmployee(faker, accounting, chiefAccountant));
            employees.Add(GenerateEmployee(faker, accounting, posts.First(p => p.Name == "Бухгалтер")));

            // Гарантируем создание Кладовщика
            var warehouse = divisions.FirstOrDefault(d => d.Name == "Склад")!;
            var warehouseClerk = posts.First(p => p.Name == "Кладовщик");
            employees.Add(GenerateEmployee(faker, warehouse, warehouseClerk));

            // Оставшиеся сотрудники
            var generalPosts = posts
                .Where(p => p.Name != "Директор" && p.Name != "Заместитель директора" && p.Name != "Главный бухгалтер")
                .ToList();

            var rnd = new Random();
            int additionalCount = rnd.Next(22, 28); // всего будет от 25 до 30

            for (int i = 0; i < additionalCount; i++)
            {
                var division = faker.PickRandom(divisions);
                var availablePosts = generalPosts.Where(p =>
                    (division.Name == "Разработки" && (p.Name.Contains("Инженер") || p.Name == "Механик")) ||
                    (division.Name == "Сборки" && (p.Name == "Сварщик" || p.Name == "Монтажник")) ||
                    (division.Name == "Отладки" && (p.Name == "Наладчик" || p.Name.Contains("Инженер"))) ||
                    (division.Name == "Бухгалтерия" && p.Name == "Бухгалтер") ||
                    (division.Name == "Руководство" && p.Name == "Бухгалтер") || // резерв
                    (division.Name == "Склад" && p.Name == "Кладовщик") ||
                    true // fallback
                ).ToList();

                var post = faker.PickRandom(availablePosts);
                employees.Add(GenerateEmployee(faker, division, post));
            }

            return employees;
        }

        private static Employee GenerateEmployee(Faker faker, Division division, Post post)
        {
            var hireDate = faker.Date.Past(10);
            var isDismissed = faker.Random.Bool(0.1f); // 10% уволены

            return new Employee
            {
                Fio = new FullNameGenerator().GenerateFullName(),
                Division = division,
                DivisionId = division.Id,
                Post = post,
                PostId = post.Id,
                DateOfBirth = faker.Date.Past(40, DateTime.Today.AddYears(-20)),
                DateHire = hireDate,
                DateDismissal = isDismissed ? hireDate.AddYears(faker.Random.Int(1, 5)) : null,
                PhoneNumber = faker.Phone.PhoneNumber("375########"),
                Address = faker.Address.FullAddress(),
                PassportData = faker.Random.Replace("КН##########"),
                Salary = Math.Round(faker.Random.Double(35000, 90000), 2),
                Bonuses = faker.Random.Bool(0.3f) ? Math.Round(faker.Random.Double(0, 10), 2) : null,
                BankDetails = faker.Finance.Iban()
            };
        }

        public static List<Location> SeedLocations(List<Employee> employees, int count)
        {
            var baseNames = new List<string>
            {
                "Склад",
                "Мастерская",
                "Цех",
                //"Архив",
                //"Хранилище запасов"
            };

            var rand = new Random();

            // Фильтруем только кладовщиков
            var warehouseWorkers = employees.Where(e => e.Post?.Name == "Кладовщик").ToList();
            if (warehouseWorkers.Count == 0)
                throw new InvalidOperationException("Нет сотрудников с должностью 'Кладовщик'.");

            var locations = new List<Location>();

            for (int i = 0; i < count; i++)
            {
                var baseName = baseNames[i % baseNames.Count];
                var numberedName = $"{baseName} №{(i / baseNames.Count) + 1}";

                var employee = warehouseWorkers[rand.Next(warehouseWorkers.Count)];

                locations.Add(new Location
                {
                    Name = numberedName,
                    EmployeeId = employee.Id,
                    Employee = employee
                });
            }

            return locations;
        }

        public static List<Product> SeedProducts(List<Unit> units, List<Location> locations)
        {
            var rand = new Random();

            // Предпочтительно использовать "Штука"
            var defaultUnit = units.FirstOrDefault(u => u.Name.Contains("шт", StringComparison.OrdinalIgnoreCase)) ?? units.First();

            var productNames = new List<(string namePrefix, string codePrefix)>
            {
                ("Установка синтеза графена", "УСГ"),
                ("Электро-дуговой испаритель", "ЭДИ"),
                ("Вакуумная установка", "ВУ"),
                ("Печь отжига кремния", "ПОК"),
                ("Ионно-плазменная установка", "ИПУ"),
                ("Вакуумная камера", "ВК"),
                ("Система напыления", "СН"),
                ("Источник испарения", "ИИ"),
                ("Установка ионной очистки", "УИО"),
                ("Магнетронная система", "МС"),
                ("Герметизатор корпуса", "ГК"),
                ("Контроллер давления", "КД"),
                ("Система откачки", "СО"),
                ("Ионный насос", "ИН"),
                ("Газораспределительная система", "ГРС")
            };

            var products = new List<Product>();

            for (int i = 0; i < productNames.Count; i++)
            {
                var (namePrefix, codePrefix) = productNames[i];
                var serialNumber = $"{codePrefix}-{rand.Next(100, 999)}";
                var location = locations[rand.Next(locations.Count)];

                products.Add(new Product
                {
                    Name = namePrefix,
                    SerialNo = serialNumber,
                    UnitId = defaultUnit.Id,
                    Unit = defaultUnit,
                    LocationId = location.Id,
                    Location = location,
                    Count = rand.Next(1, 10),
                    Price = Math.Round(rand.NextDouble() * 20000 + 500, 2), // от 500 до ~20500
                    Nds = 20
                });
            }

            return products;
        }

        public static List<Material> SeedMaterials(List<Unit> units, List<Location> locations, int count)
        {
            var materials = new List<Material>();
            var faker = new Faker("ru");

            var boltSizes = new List<int> { 3, 4, 5, 6, 8, 10 };
            var nutSizes = new List<int> { 3, 4, 5, 6, 8, 10 };

            var materialInfo = new List<(string Name, List<string> AllowedUnits)>
            {
                ("Болт",             new List<string> { "шт" }),
                ("Гайка",            new List<string> { "шт" }),
                ("ДСП",              new List<string> { "мкв" }),
                ("Лист алюминиевый", new List<string> { "мкв" }),
                ("ВД40",             new List<string> { "шт" }),
                ("ПР200",            new List<string> { "шт" }),
                ("ПЛК220",           new List<string> { "шт" }),
                ("Шайба",            new List<string> { "шт" }),
                ("Прокладка",        new List<string> { "шт" }),
                ("Крепёжный винт",   new List<string> { "шт" }),
                ("Кабель",           new List<string> { "м" }),
                ("Фильтр",           new List<string> { "шт" }),
                ("Резистор",         new List<string> { "шт" }),
                ("Конденсатор",      new List<string> { "шт" }),
                ("Транзистор",       new List<string> { "шт" }),
                ("Диод",             new List<string> { "шт" }),
                ("Микросхема",       new List<string> { "шт" }),
                ("Термопара",        new List<string> { "шт" }),
                ("Ремень",           new List<string> { "м" }),
                ("Шланг",            new List<string> { "м" }),
                ("Ролик",            new List<string> { "шт" }),
                ("Подшипник",        new List<string> { "шт" }),
                ("Паллет",           new List<string> { "шт" }),
                ("Провод",           new List<string> { "м" }),
                ("Изоляция",         new List<string> { "м" }),
                ("Светодиод",        new List<string> { "шт" }),
                ("Смазка",           new List<string> { "л" }),
                ("Шестерня",         new List<string> { "шт" }),
                ("Редуктор",         new List<string> { "шт" }),
                ("Трос",             new List<string> { "м" }),
                ("Кран",             new List<string> { "шт" }),
                ("Грузоподъёмник",   new List<string> { "шт" })
            };

            var rnd = new Random();

            while (materials.Count < count)
            {
                var (baseName, allowedUnits) = faker.PickRandom(materialInfo);

                // Получаем допустимую единицу измерения
                var unit = units.FirstOrDefault(u => allowedUnits.Contains(u.Name));
                if (unit == null) continue;

                // Получаем место хранения
                var location = faker.PickRandom(locations);
                if (location == null) continue;

                // Генерация имени с размерами
                string name = baseName;
                if (baseName == "Болт")
                {
                    int size = faker.PickRandom(boltSizes);
                    int length = faker.Random.Int(10, 100);
                    name = $"Болт М{size}x{length}";
                }
                else if (baseName == "Гайка")
                {
                    int size = faker.PickRandom(nutSizes);
                    name = $"Гайка М{size}";
                }

                // Избегаем дубликатов по названию
                if (materials.Any(m => m.Name == name))
                    continue;

                materials.Add(new Material
                {
                    Name = name,
                    //UnitId = unit.Id,
                    Unit = unit,
                    //LocationId = location.Id,
                    Location = location,
                    Count = faker.Random.Int(1, 100),
                    Price = Math.Round(faker.Random.Double(1, 5000), 2)
                });
            }

            return materials;
        }

        public static List<Product_Material> SeedProduct_Materials(List<Product> products, List<Material> materials)
        {
            var faker = new Faker("ru");
            var rnd = new Random();
            var productMaterials = new List<Product_Material>();

            foreach (var product in products)
            {
                // Случайное количество материалов на изделие: от 10 до 15
                int materialCount = rnd.Next(10, 16);

                // Выбираем случайные материалы без повторений
                var selectedMaterials = faker.PickRandom(materials, materialCount).Distinct().ToList();

                foreach (var material in selectedMaterials)
                {
                    var link = new Product_Material
                    {
                        Product = product,
                        ProductId = product.Id,
                        Material = material,
                        MaterialId = material.Id,
                        Quantity = faker.Random.Int(1, 10)
                    };

                    productMaterials.Add(link);
                }
            }

            return productMaterials;
        }

        public static List<Counterpartie> SeedCounterparties(int count)
        {
            var faker = new Faker("ru");
            var counterparties = new List<Counterpartie>();

            for (int i = 0; i < count; i++)
            {
                var isLegalEntity = faker.Random.Bool(); // случайно: физ или юр лицо
                var type = isLegalEntity ? CounterpartieType.Ur : CounterpartieType.Fiz;

                var name = isLegalEntity
                    ? $"{faker.Company.CompanySuffix()} \"{faker.Company.CompanyName()}\""
                    : faker.Name.FullName();

                var legalAddress = faker.Address.ZipCode() + ", " + faker.Address.City() + ", " + faker.Address.StreetAddress();
                var phone = faker.Phone.PhoneNumber("+375#########");
                var postalAddress = faker.Address.ZipCode() + ", " + faker.Address.City() + ", " + faker.Address.StreetAddress();
                var unp = faker.Random.Replace("#########");
                var bankAccount = "BY" + faker.Random.ReplaceNumbers("##") + faker.Random.AlphaNumeric(26).ToUpper();

                var okulp = isLegalEntity ? faker.Random.Replace("##########") : null;
                var okpo = isLegalEntity ? faker.Random.Replace("##########") : null;
                var oked = isLegalEntity ? faker.Random.Replace("##.##") : faker.Random.Replace("##.##");

                var counterpartie = new Counterpartie
                {
                    Name = name,
                    LegalAddress = legalAddress,
                    PhoneNomber = phone,
                    PostalAddress = postalAddress,
                    Unp = unp,
                    BankAccount = bankAccount,
                    Type = type,
                    Okulp = okulp,
                    Okpo = okpo,
                    Oked = oked
                };

                counterparties.Add(counterpartie);
            }

            return counterparties;
        }

        public static List<Receipt> SeedReceipts(List<Material> materials, List<Counterpartie> counterparties, int groupCount)
        {
            var faker = new Faker("ru");
            var receipts = new List<Receipt>();
            var rnd = new Random();

            for (int i = 0; i < groupCount; i++)
            {
                // Одна дата и один контрагент на группу
                var date = faker.Date.Past(1); // случайная дата за последний год
                var counterparty = faker.PickRandom(counterparties);

                int groupSize = rnd.Next(3, 8); ; // Кол-во записей в каждой группе

                for (int j = 0; j < groupSize; j++)
                {
                    var material = faker.PickRandom(materials);
                    var count = faker.Random.Int(1, 50);
                    var summ = count * material.Price;

                    receipts.Add(new Receipt
                    {
                        MaterialId = material.Id,
                        Material = material,
                        Count = count,
                        Summ = summ,
                        Date = date,
                        CounterpartyId = counterparty.Id,
                        Counterpartie = counterparty
                    });
                }
            }

            return receipts;
        }

        public static List<Contract> SeedContracts(List<Product> products, List<Counterpartie> counterparties, int count = 40)
        {
            var faker = new Faker("ru");
            var contracts = new List<Contract>();

            for (int i = 0; i < count; i++)
            {
                var product = faker.PickRandom(products);
                var counterparty = faker.PickRandom(counterparties);
                var contractDate = faker.Date.Between(DateTime.Today.AddYears(-3), DateTime.Today);
                var quantity = faker.Random.Int(1, 4);
                var sum = product.Price * quantity;

                contracts.Add(new Contract
                {
                    Name = $"ДП-{i + 1:000}",
                    CounterpartyId = counterparty.Id,
                    Counterpartie = counterparty,
                    Date = contractDate,
                    Term = faker.Random.Int(6, 36), // срок в месяцах
                    Summ = sum,
                    ProductId = product.Id,
                    Product = product,
                    Count = quantity,
                    Status = faker.PickRandom(
                        ContractStatus.completed,
                        ContractStatus.created,
                        ContractStatus.running
                        )
                });
            }

            return contracts;
        }

        public static List<Sale> SeedSales(List<Contract> contracts, int totalSales = 30, int numberOfYears = 5)
        {
            var faker = new Faker("ru");
            var sales = new List<Sale>();
            var baseDate = DateTime.Today;
            int[] distribution = CalculateDistribution(totalSales, numberOfYears);
            int contractIndex = 0;

            for (int year = 0; year < numberOfYears; year++)
            {
                for (int i = 0; i < distribution[year]; i++)
                {
                    var contract = contracts[contractIndex++ % contracts.Count];
                    var productPrice = contract.Product?.Price ?? 0;
                    var count = faker.Random.Int(1, Math.Max(contract.Count / 2, 1));
                    var date = faker.Date.Between(baseDate.AddYears(-numberOfYears + year), baseDate.AddYears(-numberOfYears + year + 1));
                    var sum = productPrice * count;

                    sales.Add(new Sale
                    {
                        ContractId = contract.Id,
                        Contract = contract,
                        Date = date,
                        Count = count,
                        Summ = sum
                    });
                }
            }

            return sales;
        }

        private static int[] CalculateDistribution(int totalSales, int numberOfYears)
        {
            if (numberOfYears <= 0)
            {
                throw new ArgumentException("Number of years must be at least 1.");
            }

            if (numberOfYears == 1)
            {
                return [totalSales];
            }

            int W = numberOfYears * (numberOfYears + 1) / 2;
            int[] baseSales = new int[numberOfYears];
            int totalBase = 0;

            for (int i = 0; i < numberOfYears; i++)
            {
                double proportion = (double)(i + 1) / W;
                baseSales[i] = (int)Math.Floor(totalSales * proportion);
                totalBase += baseSales[i];
            }

            int remainder = totalSales - totalBase;
            for (int i = 0; i < remainder; i++)
            {
                baseSales[numberOfYears - 1 - i] += 1;
            }

            return baseSales;
        }
    }
}