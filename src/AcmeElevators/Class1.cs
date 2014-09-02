using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AcmeElevators
{
    public interface IElevatorControlSystem
    {
        /// <summary>
        /// Returns the elevator status
        /// </summary>
        /// <returns>A tuple containing the Elevator ID, Floor Number, and Goal Floor Number</returns>
        Tuple<int, int, int> Status();

        /// <summary>
        /// Update the elevator state
        /// </summary>
        /// <param name="elevatorId">Desired elevator ID</param>
        /// <param name="floorNumber">Change the floor number</param>
        /// <param name="goalFloorNumber">Change the goal floor number</param>
        void Update(int elevatorId, int floorNumber, int goalFloorNumber);
        void Pickup(int floorNumber, int direction);
        void Step();
    }
}
