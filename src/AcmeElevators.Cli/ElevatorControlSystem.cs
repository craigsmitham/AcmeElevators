using System;
using System.Collections.Generic;
using System.Linq;


public class Program
{
    public static void Main(string[] args)
    {
        const int numberOfFloors = 10;
        const int numberOfElevators = 1;
        const int numberOfRequests = 10 * 1000;
        var pickupCount = 0;
        var stepCount = 0;
        var random = new Random();
        IElevatorControlSystem system = new ControlSystem(numberOfElevators);


        while (pickupCount < numberOfRequests)
        {
            var originatingFloor = random.Next(1, numberOfFloors + 1);
            var destinationFloor = random.Next(1, numberOfFloors + 1);
            if (originatingFloor != destinationFloor)
            {
                system.Pickup(originatingFloor, destinationFloor);
                pickupCount++;
            }
        }

        while (system.AnyOutstandingPickups())
        {
            system.Step();
            stepCount++;
        }

        Console.WriteLine("Transported {0} elevator riders to their requested destinations in {1} steps.", pickupCount, stepCount);
        Console.ReadLine();
    }
}


public interface IElevatorControlSystem
{
    Elevator GetStatus(int elevatorId);
    void Update(int elevatorId, int floorNumber, int goalFloorNumber);
    void Pickup(int pickupFloor, int destinationFloor);
    void Step();
    bool AnyOutstandingPickups();
}

public class ControlSystem : IElevatorControlSystem
{
    public List<Elevator> Elevators { get; set; }
    public List<Rider> WaitingRiders { get; set; }

    public ControlSystem(int numberOfElevators)
    {
        Elevators = Enumerable.Range(0, numberOfElevators).Select(eid => new Elevator(eid)).ToList();
        WaitingRiders = new List<Rider>();
    }


    public Elevator GetStatus(int elevatorId)
    {
        return Elevators.First(e => e.Id == elevatorId);
    }

    public void Update(int elevatorId, int floorNumber, int goalFloorNumber)
    {
        UpdateElevator(elevatorId, e =>
        {
            e.CurrentFloor = floorNumber;
            e.DestinationFloor = goalFloorNumber;
        });
    }

    public void Pickup(int pickupFloor, int destinationFloor)
    {
        WaitingRiders.Add(new Rider(pickupFloor, destinationFloor));
    }

    private void UpdateElevator(int elevatorId, Action<Elevator> update)
    {
        Elevators = Elevators.Select(e =>
        {
            if (e.Id == elevatorId) update(e);
            return e;
        }).ToList();
    }

    public void Step()
    {
        var busyElevatorIds = new List<int>();
        // unload elevators
        Elevators = Elevators.Select(e =>
        {
            var disembarkingRiders = e.Riders.Where(r => r.DestinationFloor == e.CurrentFloor).ToList();
            if (disembarkingRiders.Any())
            {
                busyElevatorIds.Add(e.Id);
                e.Riders = e.Riders.Where(r => r.DestinationFloor != e.CurrentFloor).ToList();
            }
            //Update destination floor?
            return e;
        }).ToList();

        // Embark passengers to available elevators
        WaitingRiders.GroupBy(r => new { r.OriginatingFloor, r.Direction }).ToList().ForEach(waitingFloor =>
        {
            var availableElevator =
                Elevators.FirstOrDefault(
                    e =>
                        e.CurrentFloor == waitingFloor.Key.OriginatingFloor &&
                        (e.Direction == waitingFloor.Key.Direction || !e.Riders.Any()));
            if (availableElevator != null)
            {
                busyElevatorIds.Add(availableElevator.Id);
                var embarkingPassengers = waitingFloor.ToList();
                UpdateElevator(availableElevator.Id, e => e.Riders.AddRange(embarkingPassengers));
                WaitingRiders = WaitingRiders.Where(r => embarkingPassengers.All(er => er.Id != r.Id)).ToList();
            }
        });


        Elevators.ForEach(e =>
        {
            var isBusy = busyElevatorIds.Contains(e.Id);
            int destinationFloor;
            if (e.Riders.Any())
            {
                var closestDestinationFloor =
                    e.Riders.OrderBy(r => Math.Abs(r.DestinationFloor - e.CurrentFloor))
                        .First()
                        .DestinationFloor;
                destinationFloor = closestDestinationFloor;
            }
            else if (e.DestinationFloor == e.CurrentFloor && WaitingRiders.Any())
            {
                // Lots of optimization could be done here, perhaps?
                destinationFloor = WaitingRiders.GroupBy(r => new { r.OriginatingFloor }).OrderBy(g => g.Count()).First().Key.OriginatingFloor;
            }
            else
            {
                destinationFloor = e.DestinationFloor;
            }

            var floorNumber = isBusy
                ? e.CurrentFloor
                : e.CurrentFloor + (destinationFloor > e.CurrentFloor ? 1 : -1);

            Update(e.Id, floorNumber, destinationFloor);
        });
    }

    public bool AnyOutstandingPickups()
    {
        return WaitingRiders.Any();
    }
}

public class Elevator
{
    public Elevator(int id)
    {
        Id = id;
        Riders = new List<Rider>();
    }

    public int Id { get; private set; }
    public int CurrentFloor { get; set; }
    public int DestinationFloor { get; set; }

    public Direction Direction
    {
        get
        {
            return CurrentFloor == 1
                ? Direction.Up
                : DestinationFloor > CurrentFloor ? Direction.Up : Direction.Down;
        }
    }

    public List<Rider> Riders { get; set; }
}

public class Rider
{
    public Rider(int originatingFloor, int destinationFloor)
    {
        OriginatingFloor = originatingFloor;
        DestinationFloor = destinationFloor;
        Id = Guid.NewGuid();
    }

    public Guid Id { get; private set; }
    public int OriginatingFloor { get; private set; }
    public int DestinationFloor { get; private set; }

    public Direction Direction
    {
        get { return OriginatingFloor < DestinationFloor ? Direction.Up : Direction.Down; }
    }
}

public enum Direction
{
    Up = 1,
    Down = -1
}
