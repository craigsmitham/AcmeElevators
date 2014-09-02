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

