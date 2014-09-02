namespace AcmeElevators
{
    public interface IElevatorControlSystem : IPublicElevatorControls, IInternalElevatorControls { }

    public interface IPublicElevatorControls
    {
        /// <summary>
        /// Get an elevator's status 
        /// </summary>
        /// <param name="elevatorId">The ID for the desired elevator</param>
        /// <returns>The status for the specified elevator</returns>
        IElevatorStatus GetStatus(int elevatorId);

        /// <summary>
        /// Request an elevator pickup
        /// </summary>
        /// <param name="currentFloorNumber">The floor number</param>
        /// <param name="direction">The desired elevator direction</param>
        void Pickup(int currentFloorNumber, Direction direction);
    }

    public interface IInternalElevatorControls
    {
        /// <summary>
        /// Update the elevator state
        /// </summary>
        /// <param name="elevatorId">Desired elevator ID</param>
        /// <param name="floorNumber">Change the floor number</param>
        /// <param name="goalFloorNumber">Change the goal floor number</param>
        void Update(int elevatorId, int floorNumber, int goalFloorNumber);

        /// <summary>
        /// Time step forward
        /// </summary>
        void Step();
    }

    public interface IElevatorStatus
    {
        int ElevatorId { get; }
        int FloorNumber { get; }
        int GoalFloorNumber { get; }
        Direction Direction { get; }
    }

    public enum Direction
    {
        Up = 1,
        Down = -1
    }
}
