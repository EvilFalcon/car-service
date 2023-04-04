using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;

namespace Car_Service
{
    internal class Program
    {
        public static void Main(string[] args)
        {
            MainBuilder mainBuilder = new MainBuilder();
            Service service = mainBuilder.BuildService();
            service.Open();

            PartsConfig config = new PartsConfig();
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

    public class Container
    {
        private readonly int _maxCountDetail;
        private Queue<Detail> _details = new Queue<Detail>();

        public Container(int config)
        {
            _maxCountDetail = config;
        }

        public Container(Container container)
        {
            Name = container.Name;
            _details = container._details;
        }

        public string Name { get; }
        public int CountDitail => _details.Count;
        public int Price { get; private set; } = 0;

        public Detail GiveDetail()
        {
            Detail detail = _details.Dequeue();
            return detail;
        }

        public void TakeDetail(Detail details)
        {
            Price += details.Price;
            _details.Enqueue(details);
        }

        public void TakeDetail(List<Detail> details)
        {
            _details = (Queue<Detail>)_details.Concat(details);
        }
    }

    public class Storage
    {
        private Dictionary<string, Container> _contaners;

        public Storage(Dictionary<string, Container> details)
        {
            _contaners = details;
        }

        public Detail IsAvailable(string nameDetails)
        {
            if (_contaners.ContainsKey(nameDetails) == false)
                return null;

            if (_contaners[nameDetails].CountDitail < 0)
                return null;

            return _contaners[nameDetails].GiveDetail();
        }

        public void ShowInfo()
        {
            foreach (var detail in _contaners)
            {
                Console.WriteLine($"{detail.Key}  {detail.Value.CountDitail}");
            }
        }
    }

    public class ShopContainerDetail
    {
        private List<Container> _conteiners;

        public ShopContainerDetail(List<Container> conteiners)
        {
            _conteiners = conteiners;
        }

        //public Container SellDetails(string nameDetail)
        //{
        //    foreach (var conteiner in _conteiners)
        //    {
        //        if (conteiner.Name == nameDetail)
        //        {
        //            Container container = new Container(conteiner);
        //        }
        //    }
        //}
    }

    public class Service
    {
        private PersonListBuilder _builder;
        private Storage _storage;
        private List<Person> _persons;
        private int _money;
        private ShopContainerDetail _shop;
        private ServiceConfig _config;

        public Service(Storage storage, List<Person> persons, PersonListBuilder builder, ShopContainerDetail shop,
            ServiceConfig config,
            int money)
        {
            _money = money;
            _persons = persons;
            _storage = storage;
            _builder = builder;
            _shop = shop;
            _config = config;
        }

        public void Open()
        {
            Work();
        }

        public void AddPersonList(Person[] persons)
        {
            _persons.AddRange(persons.ToList());
        }

        private void Work()
        {
            while (_persons.Count > 0 && _money >= 0)
            {
                _storage.ShowInfo();
            }
        }

        private void FixCarDetail(Detail detail)
        {
            detail.Fix();
        }

        private bool TryPau(int price)
        {
            if (_money < price)
                return false;

            return true;
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
        private readonly PartsConfig _config;

        public CarFactory(DetailFactory detailFactory, PartsConfig config)
        {
            _detailFactory = detailFactory;
            _config = config;
        }

        public Car Create()
        {
            Part[] parts = _config.GetParts();

            List<Detail> details = new List<Detail>();

            foreach (var itemConfig in parts)
            {
                details.Add(_detailFactory.Create(itemConfig.Name, itemConfig.Price, Utils.GetBoolRandom()));
            }

            return new Car(details);
        }
    }

    public class MainBuilder
    {
        private PartsConfig _partsConfig = new PartsConfig();
        private ShopConfig _shopConfig = new ShopConfig();
        private DetailFactory _detailFactory = new DetailFactory();
        private ContainerConfig _containerConfig = new ContainerConfig();
        private ServiceConfig _serviceConfig = new ServiceConfig();
        private CarFactory _carFactory;

        private PersonListBuilder _personListBuilder;
        private ContainerBuilder _containerBuilder;
        private ServiceBuilder _serviceBuilder;

        public MainBuilder()
        {
            _carFactory = new CarFactory(_detailFactory, _partsConfig);
            _personListBuilder = new PersonListBuilder(_carFactory);
            _containerBuilder = new ContainerBuilder(_detailFactory, _partsConfig, _containerConfig);
            _serviceBuilder = new ServiceBuilder(_containerBuilder, _personListBuilder, _shopConfig, _serviceConfig);
        }

        public Service BuildService()
        {
            return _serviceBuilder.Build();
        }
    }

    public class ServiceBuilder
    {
        private readonly ContainerBuilder _containerBuilder;
        private readonly PersonListBuilder _personListBuilder;
        private readonly ShopConfig _shopConfig;
        private readonly ServiceConfig _serviceConfig;

        public ServiceBuilder(
            ContainerBuilder containerBuilder,
            PersonListBuilder personListBuilder,
            ShopConfig shopConfig,
            ServiceConfig serviceConfig
        )
        {
            _containerBuilder = containerBuilder;
            _personListBuilder = personListBuilder;
            _shopConfig = shopConfig;
            _serviceConfig = serviceConfig;
        }

        public Service Build()
        {
            Storage storage = new Storage(_containerBuilder.CreateNewCollections());
            ShopContainerDetail shop =
                new ShopContainerDetail(_containerBuilder.CreateNewCollections().Values.ToList());
            List<Person> persons = _personListBuilder.Build(_serviceConfig.MinPersonList, _serviceConfig.MaxPersonList);
            return new Service(storage, persons,_personListBuilder, shop, _serviceConfig, _serviceConfig.Money);
        }
    }

    public class ContainerBuilder
    {
        private readonly DetailFactory _detailFactory;
        private readonly ContainerConfig _containerConfig;
        private readonly List<Part> _partsConfig = new List<Part>();

        public ContainerBuilder(
            DetailFactory detailFactory,
            PartsConfig partsConfig,
            ContainerConfig containerConfig
        )
        {
            _detailFactory = detailFactory;
            _containerConfig = containerConfig;
            _partsConfig.AddRange(partsConfig.GetParts());
        }

        public Dictionary<string, Container> CreateNewCollections()
        {
            Dictionary<string, Container> containers = new Dictionary<string, Container>();

            foreach (var part in _partsConfig)
            {
                containers.Add(part.Name, Create(part));
            }

            return containers;
        }

        private Container Create(Part part)
        {
            Container container = new Container(_containerConfig.MaxCountDetail);

            for (int i = 0; i < _containerConfig.MaxCountDetail; i++)
            {
                container.TakeDetail(_detailFactory.Create(part.Name, part.Price));
            }

            return container;
        }
    }

    public class PersonListBuilder
    {
        private readonly CarFactory _carFactory;

        public PersonListBuilder(CarFactory carFactory)
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

    public class ServiceConfig
    {
        public int MaxPersonList { get; } = 50;
        public int MinPersonList { get; } = 5;

        public int Money { get; } = 55555;
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

    public class ContainerConfig
    {
        public int MaxCountDetail = 15;
    }

    public class ShopConfig
    {
        public int SellCountDetail { get; } = 10;
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