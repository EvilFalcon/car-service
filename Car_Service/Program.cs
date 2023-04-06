using System;
using System.Collections.Generic;
using System.Linq;

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

        public Car(List<Detail> details)
        {
            _details = details;
        }

        public Detail[] RemoveDetails()
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

        public bool TryIsWork()
        {
            bool isWork = true;

            foreach (var detail in _details)
            {
                if (detail.IsWhole == false)
                {
                    isWork = false;
                }
            }

            return isWork;
        }
    }

    public class Container
    {
        private Queue<Detail> _details = new Queue<Detail>();
        private int _price;

        public Container()
        {      
        }
      

        public Container(Container container)
        {
            Name = container.Name;
            _details = container._details;
        }

        public string Name { get; private set; }
        public int CountDitail => _details.Count;

        public Detail GiveDetail()
        {
            Detail detail = _details.Dequeue();
            return detail;
        }

        public void TakeDetail(Detail details)
        {
            _price += details.Price;
            Name = details.Name;
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
            Console.WriteLine("Детали на складе\n");

            foreach (var detail in _contaners)
            {
                Console.WriteLine($"{detail.Key}  {detail.Value.CountDitail}");
            }
        }
    }

    public class Service
    {
        private Queue<Person> _persons;
        private List<Detail> _brokenParts = new List<Detail>();
        private List<Detail> _detailsCar;
        private Car _clientCar;
        private Storage _storage;
        private int _money;
        private int _priceRepair;
        private readonly float _repairPercentage = 1.2f;

        public Service(Storage storage, Queue<Person> persons, int money)
        {
            _money = money;
            _persons = persons;
            _storage = storage;
        }

        public void Open()
        {
            Work();
        }

        private void Work()
        {
            const ConsoleKey CommandDiagnoseDetail = ConsoleKey.NumPad1;
            const ConsoleKey CommandFixCar = ConsoleKey.NumPad2;
            const ConsoleKey CommandNextClient = ConsoleKey.NumPad3;

            Person person = GiveNextClient();

            while (_persons.Count > 0 && _money >= 0)
            {
                Console.Clear();
                _storage.ShowInfo();
                Console.WriteLine($"ваши деньги {_money}\n\n\n\n");
                Console.WriteLine($"Управление " +
                                  $"\n{CommandDiagnoseDetail}<---Сделать диагностику" +
                                  $"\n{CommandFixCar}<---Поченить машину " +
                                  $"\n{CommandNextClient}<---Вернуть машину и взять следуюшего клиента");

                _clientCar = person.GiveAwayCar();

                ConsoleKey key = Console.ReadKey(true).Key;

                switch (key)
                {
                    case CommandDiagnoseDetail:
                        DiagnoseDetails();
                        break;

                    case CommandFixCar:
                        FixCar();
                        break;

                    case CommandNextClient:
                        person = GiveNextClient(person);
                        break;
                }

                Console.WriteLine("Нажмите любую клавишу для продолжения");
                Console.ReadKey();
            }
        }

        private Person GiveNextClient(Person oldClient = null)
        {
            if (oldClient == null)
            {
                return _persons.Dequeue();
            }

            CheckRepairQuality();
            return _persons.Dequeue();
        }

        private void CheckRepairQuality()
        {
            if (_detailsCar==null||_detailsCar.Count==0)
            {
                DiagnoseDetails();
            }
            
            _clientCar.ReturnParts(_detailsCar.ToArray());

            if (_clientCar.TryIsWork() == false)
            {
                _money -= _priceRepair;
                Console.WriteLine($"Вы получили штраф в размере : {_priceRepair} ");
            }
            else
            {
                _money += _priceRepair;
                Console.WriteLine($"Вы заработали : {_priceRepair}");
            }

            _detailsCar.Clear();
            _brokenParts.Clear();
            _priceRepair = 0;
        }

        private void DiagnoseDetails()
        {
            if (_brokenParts.Count() != 0)
            {
                return;
            }

            Console.Write("У машины :");

            _detailsCar = new List<Detail>(_clientCar.RemoveDetails());

            foreach (var detail in _detailsCar)
            {
                if (detail.IsWhole == false)
                {
                    Console.Write($" Сломан/а :{detail.Name}\n ");
                    _brokenParts.Add(detail);
                }
            }

            if (_brokenParts.Count == 0)
            {
                Console.WriteLine("Ничего не сломанно");
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

                if (detail == null)
                {
                    Console.WriteLine($"У вас нет данной дитали {part.Name}.");
                }
                else
                {
                    Repair(detail);
                }
            }

            Console.WriteLine("Вы собрали автомобиль как смогли ");
        }

        private void Repair(Detail detail)
        {

            for (int i = 0; i < _detailsCar.Count; i++)
            {
                if (_detailsCar[i].Name == detail.Name)
                {
                    _detailsCar[i] = detail;
                }
            }
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
            return _random.Next(2) == 0;
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
            Queue<Person> persons =
                _personListBuilder.Build(_serviceConfig.MinPersonList, _serviceConfig.MaxPersonList);
            return new Service(storage, persons, _serviceConfig.Money);
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
            Container container = new Container();

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

    public class ServiceConfig
    {
        public int MaxPersonList { get; } = 15;
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