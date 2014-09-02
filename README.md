# Read This First

The git repositroy at https://github.com/craigsmitham/AcmeElevators contains the history and evolution of the code that was used to create the solution, but the final solution can be found at https://dotnetfiddle.net/UwHreh.

## Running the solution

Simply use .NET fiddle to run the final solution at https://dotnetfiddle.net/UwHreh. However, you can still download the repo from GitHub and run it using Visual Studio.

# Development Log

## Intial Design Observations

* The Elevator System interface conflates some concerns. 
* The status query and pickup commands seem like a natural public API, but the step and update should be internal
* We can clean up the interface and method calls to make them more intention reavealing
* The status query does not give the ability to specify an elevator ID, or it assumes we'll only be querying one elevator. This is confusing. 

## First Steps 

* Clean up interface to make it more idiomatic C#
* Split control system interface into public and internal API methods
* Add an elevator ID status to the status query

## Initial Implementation

I will implemenent a naive FirstComeFirstServeElevatorControlSystem to get something functional, testable, then improve from there.

## Halfway

After getting a little farther than halfway, the interface of the program was bothering me even more and constricting some design decisions. So, I decided to start a fresh with a new interface, having thought more about the problem at hand. I will attempt to design a smart implementation, instead of naive one. In some cases it was harder thinking about building the First-Come-First-Serve implementation.

https://github.com/craigsmitham/AcmeElevators/commit/5ebf1ef47287bd89dd15ca89f613f5e71222e571

# Final Solution Complete

* Complete with 10 minutes to spare. Final solution posted at https://dotnetfiddle.net/UwHreh

# Next Steps

* It would be cool to have some visualizations/reports to see what is going on for some feedback about possible strategies. In addition, some tests to prove correctness and better error handling would be expected for a more robust solution. Likely the needs of the reports/GUI and tests would drive a better interface design.