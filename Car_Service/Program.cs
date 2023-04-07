using System;
using System.Collections.Generic;

namespace Car_Service
{
    internal class Program
    {
        public static void Main(string[] args)
        {
            MainBuilder mainBuilder = new MainBuilder();
            Service service = mainBuilder.BuildService();
            service.Work();
        }
    }

    public class Order
    {
        private Car _car;

        public Order(Car car)
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

        public Detail[] GetBrokenDetails()
        {
            List<Detail> filmedDetails = new List<Detail>();

            for (int i = 0; i < _details.Count; i++)
            {
                if (_details[i].IsNotBroken == false)
                {
                    filmedDetails.Add(_details[i]);
                }
            }

            foreach (var filmedDetail in filmedDetails)
            {
                _details.Remove(filmedDetail);
            }

            return filmedDetails.ToArray();
        }

        public void InstallDetails(Detail[] details) =>
            _details.AddRange(details);

        public bool CheckBrokenDetail()
        {
            bool isWork = true;

            foreach (var detail in _details)
            {
                if (detail.IsNotBroken == false)
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

        public Container(Container container)
        {
            _details = new Queue<Detail>(container._details);
            Name = container.Name;
        }

        public Container(string name)
        {
            Name = name;
        }

        public string Name { get; private set; }
        public bool HesDetails => _details.Count > 0;
        public int DitailCount => _details.Count;

        public Detail GiveDetail() =>
            _details.Dequeue();

        public void TakeDetail(Detail details) =>
            _details.Enqueue(details);
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

            Container container = _contaners[nameDetails];

            if (_contaners[nameDetails].HesDetails == false)
                return null;

            return container.GiveDetail();
        }

        public void ShowInfo()
        {
            Console.WriteLine("Детали на складе\n");

            foreach (var contaner in _contaners)
            {
                string contanerName = contaner.Key;
                int details = contaner.Value.DitailCount;

                Console.WriteLine($"{contanerName}  {details}");
            }
        }
    }

    public class Service
    {
        private readonly float _repairPercentage = 1.2f;
        private readonly int _fine = 10000;

        private Queue<Order> _orders;
        private List<Detail> _brokenDetails = new List<Detail>();
        private Car _car;
        private Storage _storage;
        private int _money;
        private int _priceRepair;

        public Service(Storage storage, Queue<Order> orders, int money)
        {
            _money = money;
            _orders = orders;
            _storage = storage;
        }

        public void Work()
        {
            const ConsoleKey CommandFixCar = ConsoleKey.NumPad1;
            const ConsoleKey CommandNextClient = ConsoleKey.NumPad2;

            GiveNextOrderOnRepairCar();

            while (_orders.Count > 0 && _money >= 0)
            {
                _storage.ShowInfo();
                Console.WriteLine($"ваши деньги {_money}\n\n\n\n");
                Console.WriteLine("Управление");
                Console.WriteLine($"\n{CommandFixCar}<---Поченить машину ");
                Console.WriteLine($"\n{CommandNextClient}<---Вернуть машину и взять следуюшего клиента");

                ConsoleKey key = Console.ReadKey().Key;
                Console.Clear();

                switch (key)
                {
                    case CommandFixCar:
                        StartWorkingOnCar();
                        break;

                    case CommandNextClient:
                        GiveNextOrderOnRepairCar();
                        break;
                }

                Console.WriteLine("Нажмите любую клавишу для продолжения");
                Console.ReadKey();
            }
        }

        private void GiveNextOrderOnRepairCar()
        {
            if (_orders == null)
                return;

            if (_car != null)
                CheckRepairQuality();

            Order order = _orders.Dequeue();
            _car = order.GiveAwayCar();
        }

        private void CheckRepairQuality()
        {
            if (_car.CheckBrokenDetail() == false)
            {
                _money -= _fine;
                Console.WriteLine($"Вы получили штраф в размере : {_fine}. Так как не починили автомобиль! ");
            }
            else
            {
                _money += _priceRepair;
                Console.WriteLine($"Вы заработали : {_priceRepair}");
            }

            _brokenDetails.Clear();
            _priceRepair = 0;
        }

        private void StartWorkingOnCar()
        {
            DiagnoseDetails();
            Repair(FixCar());
        }

        private void DiagnoseDetails()
        {
            _brokenDetails = new List<Detail>(_car.GetBrokenDetails());

            Console.Write("У машины ");

            if (_brokenDetails.Count > 0)
            {
                Console.Write(": Сломан -");

                foreach (var detail in _brokenDetails)
                {
                    Console.WriteLine($"{detail.Name}");
                }
            }
            else
            {
                Console.WriteLine(": Ничего не сломанно");
                return;
            }

            CalculateRepairCost();
        }

        private void CalculateRepairCost()
        {
            foreach (var part in _brokenDetails)
            {
                _priceRepair += Convert.ToInt32(part.Price * _repairPercentage);
            }
        }

        private List<Detail> FixCar()
        {
            List<Detail> newDetails = new List<Detail>();

            Detail detail;

            foreach (var part in _brokenDetails)
            {
                detail = _storage.GiveDetail(part.Name);

                if (detail == null)
                {
                    Console.WriteLine($"У вас нет данной дитали {part.Name}.");
                }
                else
                {
                    newDetails.Add(detail);
                }
            }

            return newDetails;
        }

        private void Repair(List<Detail> newDetail)
        {
            if (newDetail.Count > 0)
                _car.InstallDetails(newDetail.ToArray());

            Console.WriteLine("Вы собрали автомобиль как смогли ");
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

        public static bool GetRandomBool()
        {
            return _random.Next(2) == 0;
        }
    }

    public class Detail
    {
        public Detail(string name, int price)
        {
            Name = name;
            Price = price;
        }

        public int Price { get; }
        public string Name { get; }
        public bool IsNotBroken { get; private set; } = true;

        public void Brake()
        {
            IsNotBroken = false;
        }
    }

    public class CarFactory
    {
        private readonly PartsConfig _config;

        public CarFactory(PartsConfig config)
        {
            _config = config;
        }

        public Car Create()
        {
            Part[] parts = _config.GetParts();

            List<Detail> details = new List<Detail>();

            foreach (var part in parts)
            {
                Detail detail = new Detail(part.Name, part.Price);

                if (Utils.GetRandomBool())
                    detail.Brake();

                details.Add(detail);
            }

            return new Car(details);
        }
    }

    public class MainBuilder
    {
        private readonly ServiceBuilder _serviceBuilder;

        public MainBuilder()
        {
            PartsConfig partsConfig = new PartsConfig();

            ContainerConfig containerConfig = new ContainerConfig();
            QueueOrdersConfig queueOrdersConfig = new QueueOrdersConfig();

            CarFactory carFactory = new CarFactory(partsConfig);
            OrdersQueueBuilder ordersQueueBuilder = new OrdersQueueBuilder(carFactory);
            ContainerBuilder containerBuilder = new ContainerBuilder(partsConfig, containerConfig);
            _serviceBuilder = new ServiceBuilder(containerBuilder, ordersQueueBuilder, queueOrdersConfig);
        }

        public Service BuildService()
        {
            return _serviceBuilder.Build();
        }
    }

    public class ServiceBuilder
    {
        private readonly ContainerBuilder _containerBuilder;
        private readonly OrdersQueueBuilder _ordersQueueBuilder;
        private readonly QueueOrdersConfig _queueOrdersConfig;

        public ServiceBuilder(
            ContainerBuilder containerBuilder,
            OrdersQueueBuilder ordersQueueBuilder,
            QueueOrdersConfig queueOrdersConfig
        )
        {
            _containerBuilder = containerBuilder;
            _ordersQueueBuilder = ordersQueueBuilder;
            _queueOrdersConfig = queueOrdersConfig;
        }

        public Service Build()
        {
            Storage storage = new Storage(_containerBuilder.CreateCollection());
            Queue<Order> persons =
                _ordersQueueBuilder.Build(_queueOrdersConfig.MinOrdersCount, _queueOrdersConfig.MaxOrdersCount);
            return new Service(storage, persons, _queueOrdersConfig.Money);
        }
    }

    public class ContainerBuilder
    {
        private readonly ContainerConfig _containerConfig;
        private readonly PartsConfig _partsConfig;

        public ContainerBuilder(
            PartsConfig partsConfig,
            ContainerConfig containerConfig
        )
        {
            _containerConfig = containerConfig;
            _partsConfig = partsConfig;
        }

        public Dictionary<string, Container> CreateCollection()
        {
            Dictionary<string, Container> containers = new Dictionary<string, Container>();

            foreach (var part in _partsConfig.GetParts())
            {
                containers.Add(part.Name, Create(part));
            }

            return containers;
        }

        private Container Create(Part part)
        {
            Container container = new Container(part.Name);

            for (int i = 0; i < _containerConfig.MaxCountDetails; i++)
            {
                container.TakeDetail(new Detail(part.Name, part.Price));
            }

            return container;
        }
    }

    public class OrdersQueueBuilder
    {
        private readonly CarFactory _carFactory;

        public OrdersQueueBuilder(CarFactory carFactory)
        {
            _carFactory = carFactory;
        }

        public Queue<Order> Build(int minValue, int maxValue)
        {
            int count = Utils.GetRandomNumber(minValue, maxValue);

            return AddPersons(count);
        }

        private Order CreatePerson()
        {
            Car car = _carFactory.Create();

            return new Order(car);
        }

        private Queue<Order> AddPersons(int value)
        {
            Queue<Order> persons = new Queue<Order>();

            for (int i = 0; i < value; i++)
            {
                Order order = CreatePerson();
                persons.Enqueue(order);
            }

            return persons;
        }
    }

    public class QueueOrdersConfig
    {
        public int MaxOrdersCount { get; } = 15;
        public int MinOrdersCount { get; } = 5;

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
        public readonly int MaxCountDetails = 15;
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