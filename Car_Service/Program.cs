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
        }
    }

    public class Person
    {
        private Car _car;
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

        public void ReturnParts(Detail[] details) =>
            _details.AddRange(details);
    }

    public class Container
    {
        private readonly int _maxCountDetail;
        private Queue<Detail> _details = new Queue<Detail>();
        private int _price;

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

        public Detail GiveDetail()
        {
            Detail detail = _details.Dequeue();
            return detail;
            
        }

        public void TakeDetail(Detail details)
        {
            _price += details.Price;
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

        public Detail GiveDetail(string nameDetails)
        {
            if (_contaners.ContainsKey(nameDetails) == false)
                return null;

            if (_contaners[nameDetails].CountDitail <= 0)
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
        private Queue<Person> _persons;
        private List<Detail> _brokenParts = new List<Detail>();
        private List<Detail> _detailsCar;
        private Car _clientCar;

        private PersonListBuilder _builder;
        private Storage _storage;
        private ShopContainerDetail _shop;
        private ServiceConfig _config;

        private int _money;
        private int _priceRepair;
        private float _repairPercentage = 1.2f;

        public Service(Storage storage, Queue<Person> persons, PersonListBuilder builder, ShopContainerDetail shop,
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

        public void AddPersonList(Queue<Person> persons)
        {
            foreach (var person in persons)
            {
                _persons.Enqueue(person);
            }
        }

        private void Work()
        {
            const ConsoleKey CommandDiagnoseDetail = ConsoleKey.NumPad1;
            const ConsoleKey CommandFixCar = ConsoleKey.NumPad2;
            const ConsoleKey CommandNextClient = ConsoleKey.NumPad3;
            const ConsoleKey CommandBuySpareParts = ConsoleKey.NumPad4;
            const ConsoleKey CommandExitProgram = ConsoleKey.Escape;
               
            Person person = _persons.Dequeue();

            while (_persons.Count > 0 && _money >= 0)
            {
                _storage.ShowInfo();

                ConsoleKey key = Console.ReadKey(true).Key;

                switch (key)
                {
                    case CommandDiagnoseDetail:
                        DiagnoseDetail(_clientCar = person.GiveAwayCar());
                        break;

                    case CommandFixCar:
                        FixCar();
                        break;

                    case CommandNextClient:
                        
                        break;

                    case CommandBuySpareParts:

                        break;

                    case CommandExitProgram:

                        break;
                }
            }
        }


        private Person GiveNextClient()
        {
            
        }
        
        private void DiagnoseDetail(Car car)
        {
            Console.Write("у машины :");
            _detailsCar = new List<Detail>(car.RemoveDetail());

            foreach (var detail in _detailsCar)
            {
                if (detail.IsWhole == false)
                {
                    Console.Write($"\n Сломан : {detail.Name}");
                    _brokenParts.Add(detail);
                }
            }

            CalculateRepairCost();
        }

        private void CalculateRepairCost()
        {
            if (_brokenParts.Count == 0)
                return;

            foreach (var part in _brokenParts)
            {
                _priceRepair += Convert.ToInt32(part.Price * _repairPercentage);
            }
        }

        private void FixCar()
        {
            if (_detailsCar.Count == 0)
                return;

            Detail detail;

            foreach (var part in _brokenParts)
            {
                detail = _storage.GiveDetail(part.Name);
                
                if(detail == null)
                    Console.WriteLine($"У вас нет данной дитали {part.Name}. \nЗакажите ее в магазине!");
                else
                    Repair(detail);
                
            }
            
            _clientCar.ReturnParts(_detailsCar.ToArray());//вынести в другой метод
            _detailsCar.Clear();                          //вынести в другой метод
            _brokenParts.Clear();                         //вынести в другой метод
            _money += _priceRepair;                       //вынести в другой метод
            _priceRepair = 0;                             //вынести в другой метод
        }

        private void Repair(Detail detail)
        {
            if (detail == null)
                return;

            for (int i = 0; i < _detailsCar.Count; i++)
            {
                if (_detailsCar[i].Name == detail.Name)
                {
                    _detailsCar[i] = detail;
                }
            }
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
            return _random.Next(1 + 1) == 0;
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

        public Detail(Detail detail)
        {
            Name = detail.Name;
            Price = detail.Price;
            IsWhole = detail.IsWhole;
        }

        public int Price { get; }
        public string Name { get; }
        public bool IsWhole { get; private set; }

        public void ShowInfoStats()
        {
            string stats = GetStats();

            Console.WriteLine($"{Name}|{stats}");
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
            _serviceBuilder = new ServiceBuilder(_containerBuilder, _personListBuilder, _serviceConfig);
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
        private readonly ServiceConfig _serviceConfig;

        public ServiceBuilder(
            ContainerBuilder containerBuilder,
            PersonListBuilder personListBuilder,
            ServiceConfig serviceConfig
        )
        {
            _containerBuilder = containerBuilder;
            _personListBuilder = personListBuilder;
            _serviceConfig = serviceConfig;
        }

        public Service Build()
        {
            Storage storage = new Storage(_containerBuilder.CreateNewCollections());
            ShopContainerDetail shop =
                new ShopContainerDetail(_containerBuilder.CreateNewCollections().Values.ToList());
            Queue<Person> persons =
                _personListBuilder.Build(_serviceConfig.MinPersonList, _serviceConfig.MaxPersonList);
            return new Service(storage, persons, _personListBuilder, shop, _serviceConfig, _serviceConfig.Money);
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

        public Queue<Person> Build(int minCountQueue, int maxCountQueue)
        {
            return AddPersons(minCountQueue, maxCountQueue);
        }

        private Person CreatePerson()
        {
            Car car = _carFactory.Create();

            return new Person(car);
        }

        private Queue<Person> AddPersons(int minValue, int maxValue)
        {
            Queue<Person> persons = new Queue<Person>();

            for (int i = 0; i < Utils.GetRandomNumber(minValue, maxValue); i++)
            {
                Person person = CreatePerson();
                persons.Enqueue(person);
            }

            return persons;
        }
    }

    public   class ServiceConfig
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