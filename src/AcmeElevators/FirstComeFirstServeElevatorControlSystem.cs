using System;
using System.Collections.Generic;
using System.Linq;

namespace AcmeElevators
{
    public class FirstComeFirstServeElevatorControlSystem : IElevatorControlSystem
    {
        private readonly int _numberOfFloors;
        private List<Rider> _requests = new List<Rider>();
        private readonly Dictionary<int, Queue<Rider>> _elevatorDestinations = new Dictionary<int, Queue<Rider>>();
        private List<Elevator> _elevators;

        public FirstComeFirstServeElevatorControlSystem(int numberOfElevators, int numberOfFloors)
        {
            _elevators = Enumerable.Range(1, numberOfElevators)
                .Select(elevatorId => new Elevator
                {
                    Direction = Direction.Up,
                    ElevatorId = elevatorId,
                    FloorNumber = 1,
                    GoalFloorNumber = 1
                }).ToList();

            _elevators.ForEach(e => _elevatorDestinations.Add(e.ElevatorId, new Queue<Rider>()));

            _numberOfFloors = numberOfFloors;

        }

        public IElevatorStatus GetStatus(int elevatorId)
        {
            return _elevators.First(e => e.ElevatorId == elevatorId);
        }

        public void Pickup(int currentFloorNumber, Direction direction)
        {
            _requests.Add(new Rider { RequestedFloorNumber = currentFloorNumber, Direction = direction });
        }

        public void Update(int elevatorId, int floorNumber, int goalFloorNumber)
        {
            throw new System.NotImplementedException();
        }

        public void Step()
        {
            var busyElevatorIds = new List<int>();

            // Unload passengers
            _elevators
                .Where(e => e.GoalFloorNumber == e.FloorNumber && _elevatorDestinations[e.ElevatorId].Any())
                .ToList().ForEach(e =>
                {
                    busyElevatorIds.Add(e.ElevatorId);
                    _elevatorDestinations[e.ElevatorId].Dequeue();
                });

            // Service any requests with available elevators
            _requests
                .GroupBy(r => new { FloorNumber = r.RequestedFloorNumber, r.Direction })
                .ToList()
                .ForEach(floorRequests =>
                {
                    var availableElevator =
                        _elevators.FirstOrDefault(
                            e => e.Direction == floorRequests.Key.Direction &&
                                 e.FloorNumber == floorRequests.Key.FloorNumber &&
                                 !_elevatorDestinations[e.ElevatorId].Any());

                    if (availableElevator != null)
                    {
                        // Enqueue requests
                        floorRequests.ToList().ForEach(request => _elevatorDestinations[availableElevator.ElevatorId].Enqueue(request));
                        // If elevators are in drop-off or pick-up state, they should not progress to next floor
                        busyElevatorIds.Add(availableElevator.ElevatorId);

                        // Update outstanding requests
                        var pickedUpRiderIds = floorRequests.Select(r => r.Id).ToList();
                        _requests = _requests.Where(r => !pickedUpRiderIds.Contains(r.Id)).ToList();
                    }
                });


            _elevators.Where(e => !busyElevatorIds.Contains(e.ElevatorId)).ToList()
                .ForEach(e =>
                {
                    var busy = false;
                    var hasRiders = _elevatorDestinations[e.ElevatorId].Any();
                    // Try to unload any that are requesting the current floor
                    if (hasRiders)
                    {
                        // Try to unload any current riders
                        var nextRider = _elevatorDestinations[e.ElevatorId].Peek();
                        while (nextRider.RequestedFloorNumber == e.FloorNumber)
                        {
                            _elevatorDestinations[e.ElevatorId].Dequeue();
                            nextRider = _elevatorDestinations[e.ElevatorId].Peek();
                            busy = true;
                        }
                    }

                    // load any riders on the current floor heading the current direction, or the most of one direction 

                    if (!busy)
                    {
                        var nextRider = _elevatorDestinations[e.ElevatorId].Peek();
                        var direction = e.FloorNumber < nextRider.RequestedFloorNumber ? Direction.Up : Direction.Down;
                        var nextFloorNumber = direction == Direction.Up ? e.FloorNumber + 1 : e.FloorNumber - 1;
                        Update(e.ElevatorId, e.FloorNumber + 1, nextRider.RequestedFloorNumber);
                        // Update
                    }

                });



        }


        private class Elevator : IElevatorStatus
        {
            public int ElevatorId { get; set; }
            public int FloorNumber { get; set; }
            public int GoalFloorNumber { get; set; }
            public Direction Direction { get; set; }
        }

        public class Rider
        {
            public Rider()
            {
                Id = new Guid();
            }

            public Guid Id { get; private set; }
            public int RequestedFloorNumber { get; set; }
            public Direction Direction { get; set; }
        }
    }
}