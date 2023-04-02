using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace Car_Service
{
    internal class Program
    {
        public static void Main(string[] args)
        {
            DetailFactory detailFactory = new DetailFactory();
            Service service = new Service(detailFactory);
            service.Open();
            Console.ReadKey();
        }
    }

    public class Person
    {
        public Car _car;
        private int _money = 1000000;

        public Person(Car car)
        {
            _car = car;
        }

        public Car GiveAwayCar()
        {
            return _car;
        }
    }

    public class Car
    {
        private readonly List<Detail> _details;

        public Car(List<Detail> details) =>
            _details = details;

        public void AddDetails(List<Detail> details) =>
            _details.AddRange(details);

        public Detail[] RemoveDetail()
        {
            Detail[] filmedDetails = new Detail[_details.Count];

            for (int i = 0; i < _details.Count; i++)
            {
                filmedDetails[i] = _details[i];
            }

            _details.Clear();
            return filmedDetails;
        }

        public void ShowInfo()
        {
            Console.WriteLine("информация о состоянии машины");

            Console.WriteLine("деталь   статус");

            foreach (var detail in _details)
            {
                detail.ShowInfoStats();
            }
        }
    }

    public class Storage
    {
        private Dictionary<string, int> _details;

        public Storage(Dictionary<string, int> details)
        {
            _details = details;
        }

        public bool IsAvailable(string nameDetails)
        {
            if (_details.ContainsKey(nameDetails) == false)
                return false;
            
            if (_details[nameDetails] < 0)
                return false;

            return true;
        }

        public void ShowInfo()
        {
            foreach (var detail in _details)
            {
                Console.WriteLine($"{detail.Key}  {detail.Value}");
            }
        }
    }

    public class Service
    {
        private readonly PersonListBuilder _builder;
        private readonly DetailFactory factory;
        private readonly PartsConfig _config;
        private Storage _storage;
        private List<Person> _persons;
        private int _money = 50000;

        public Service(DetailFactory factory, PartsConfig config, Storage storage)
        {
            this.factory = factory;
            _config = config;
            _storage = storage;
        }

        public void Open()
        {
            FillStorage();
            Work();
        }

        private void FillStorage()
        {
            Dictionary<string, int> details = new Dictionary<string, int>();
            Part[] parts = _config.GetParts();

            for (int i = 0; i < parts.Length; i++)
            {
                string name = parts[i].Name;
                int count = Utils.GetRandomNumber(ServiceConfig.MinPersonQueue, ServiceConfig.MaxPersonQueue);
                details.Add(name, count);
            }

            _storage = new Storage(details);
        }

        private void Work()
        {
            _storage.ShowInfo();
        }

        private void FixCarDitail()
        {
            if (_storage.IsAvailable()==false)
        }
    }

    public static class Utils
    {
        private static Random _random = new Random();

        public static int GetRandomNumber(int value)
        {
            int number = _random.Next(value + 1);
            return number;
        }
        
        public static int GetRandomNumber(int minValue, int maxValue)
        {
            return _random.Next(minValue, maxValue + 1);
        }
        
        public static bool GetBoolRandom()
        {
            return _random.Next(1) == 0;
        }
    }

    public class Detail
    {
        public Detail(string name, int price, bool isWhole)
        {
            Name = name;
            Price = price;
            IsWhole = isWhole;
        }

        public int Price { get; }
        public string Name { get; }
        public bool IsWhole { get; private set; }

        public void ShowInfoStats()
        {
            string stats = GetStats();

            Console.WriteLine($"{Name}|{stats}");
        }

        public void Fix()
        {
            IsWhole = true;
        }
        
        private string GetStats()
        {
            if (IsWhole == false)
                return "Сломана";

            return "Целая";
        }
    }

    public class DetailFactory
    {
        public Detail Create(string name, int price, bool isWhole = true)
        {
            return new Detail(name, price, isWhole);
        }
    }

    public class CarFactory
    {
        private readonly DetailFactory _detailFactory;

        public CarFactory(DetailFactory detailFactory)
        {
            _detailFactory = detailFactory;
        }

        public Car Create()
        {
            PartsConfig config = new PartsConfig();
            Part[] parts = config.GetParts();

            List<Detail> details = new List<Detail>();

            foreach (var itemConfig in parts)
            {
                details.Add(_detailFactory.Create(itemConfig.Name, itemConfig.Price, Utils.GetBoolRandom()));
            }

            return new Car(details);
        }
    }

    public class PersonListBuilder
    {
        private readonly CarFactory _carFactory;

        public PersonListBuilder(CarFactory carFactory, DetailFactory detailFactory)
        {
            _carFactory = carFactory;
        }

        public List<Person> Build(int minCountQueue, int maxCountQueue) =>
            AddPersons(minCountQueue, maxCountQueue);

        private Person CreatePerson()
        {
            Car car = _carFactory.Create();

            return new Person(car);
        }

        private List<Person> AddPersons(int minValue, int maxValue)
        {
            List<Person> persons = new List<Person>();
            Person person;

            for (int i = 0; i < Utils.GetRandomNumber(minValue, maxValue); i++)
            {
                person = CreatePerson();
                persons.Add(person);
            }

            return persons;
        }
    }

    public static class ServiceConfig
    {
        public static int MaxPersonQueue { get; } = 50;
        public static int MinPersonQueue { get; } = 5;
    }

    public class PartsConfig
    {
        private List<Part> _parts = new List<Part>()
        {
            new Part("Глушитель", 10000),
            new Part("Воздушный фильтер", 2500),
            new Part("Тормозные колодки", 5000),
            new Part("Двигатель", 100000),
        };

        public Part[] GetParts()
        {
            return _parts.ToArray();
        }
    }

    public class Part
    {
        public Part(string name, int price)
        {
            Price = price;
            Name = name;
        }

        public int Price { get; }
        public string Name { get; }
    }
}