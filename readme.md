# AkkaExchange
## Event sourced commodity exchange 
The goal of this project is to better get to grips with
- Event Sourcing
- CQRS
- Akka.NET Persistence & Streaming

This is a learning project and as such certainly falls fowl of many good software principles. The primary aims are the learning objectives listed above.

# Domain Concepts
The core domain concepts are:

## Clients
A Client can:
- Connect and Disconnect from the the exchange.
- Place Bid (buy) or Ask (sell) orders with parameters for Price and Quantity.
- Subscribe to State, Error and Event streams.

## Order book
The OrderBook is a singleton responsible for:
- Receiving NewOrder, AmendOrder and RemoveOrder commands from clients.
- Periodically matching pairs of orders.
- Forwarding matched orders to the Execution Engine.
- Completing or Amending orders that have been processed by the Execution Engine.
Matching is done by a simple Price/Time priority partial fill algorithm.

## Order Executor
The OrderExecutorManager (singleton) and OrderExecutors are responsible for:
- Receiving matched Orders from the Order Book.
- Executing matched Orders.
- Forwarding executed Orders back to the OrderBook.

# Abstractions
The core abstractions are:
- `ICommand` which represents a client's request to change part of the system state.
- `ICommandHandler` which is a function of form `(State, Command) -> HandlerResult`. This validates a command against the current state of an entity and either returns `Success: Event` or `Error: List of Strings`.
- `IState` which is the result of folding over an event stream to get the current state of an entity.
- `IState.Update` which is a function of form `(Event) -> State`. This updates the state after a successful event.
- `IEvent` which represents the result of a successful `ICommand` that has been persisted into the event log.
- Queries, represented as `IObservable<T>` instances. This is how a subscriber can receive state updates from the system.

# Implementation
The project is implemented using .NET Core, C# and Akka.NET (Actors, Persistence & Streams). Although there is a Test project, I haven't been rigourously using TDD.
Furthermore, there is a `AkkaExchange.Web` project, which is intended to be a basic web cli that allows you to connect as a client and send orders to the exchange.

# Todo
As far as the core objective of this project goes, it is more or less complete. The main thing left to do is to get feedback from subject matter experts to review how well I've implemented the core concepts of Event Sourcing and CQRS.
That aside, there are a few nice-to-dos that I would like to find time to tie up. Many of these are tied to the "Lessons Learnt" section.
[ ] Figure out what a `ICommandHandler` should really look like. Is it appropriate to return errors here? Should you provide the new State alongside the Event?
[ ] Better improve the consistency charactistics of the system. There are currently many oppotunities to screw your state up.
[ ] Develop client to be a bit more mature and fully featured, showing charts for price, volume, order history etc.
[ ] Rewrite core domain code in pure F#. (See below.)
[ ] Connect it to a database for persistent sessions. Maybe also add auth for web clients.
[ ] Tidy up stream implementations; get rid of hacky internal Observable/Subject code and use the Akka.NET native `ISubscriber` and `IPublisher` interfaces. 
[ ] Better error handling for failed order execution. In fact, better error handling everywhere.

# Lessons Learnt
## Functional Programming
The style of coding that you end up getting pushed towards would look quite strange to someone who has only ever done Object-Orientated Programming. One set of examples are the classes that implement `IState`. Here's an example from the `ClientState` class:
````
public class ClientState : Message, IState<ClientState>
{
    public Guid ClientId { get; }
    public ClientStatus Status { get; }
    public DateTime? StartedAt { get; }
    // ...

    public ClientState(
        Guid clientId, 
        ClientStatus status, 
        DateTime? startedAt, 
        DateTime? endedAt, 
        IImmutableList<ICommand> orderCommandHistory, 
        decimal balance,
        decimal amount)
    {
        // ...
    }

    public ClientState Update(IEvent evnt)
    {
        if (evnt is StartConnectionEvent startConnectionEvent &&
            Status == ClientStatus.Pending)
        {
            // Do something and return new instance
        }

        if (evnt is EndConnectionEvent endConnectionEvent &&
            Status == ClientStatus.Connected)
        {
            // Do else something and return new instance
        }
        
        if (evnt is ExecuteOrderEvent executeOrderEvent)
        {
            // Do else something and return new instance
        }

        if (evnt is CompleteOrderEvent completeOrderEvent &&
            Status == ClientStatus.Connected)
        {
            // Do else something and return new instance
        }

        return this;
    }
}
````
The multiple `if` statements and the type testing of the `Update` function are quite unusual. As is the insistence on not only Immutable Properties but even Immutable Collections.
But this is a core consequence of what it means to be event sourced. One of the biggest "aha" moments for me was when I realised that *`Command` and `Event` types are to Event Sourcing what `interface` types are to imperative OO.*

If you have worked on a large .NET or Java enterprise app, you will be well used to using Interfaces as the glue between your components. These Interfaces are often found right at the root of your dependency tree and allow you to depend on abstractions instead of implementations across your project (The `D` in `SOLID`).

This role in Event Sourcing is fulfilled by your `Command` and `Event` types. 

Instead of you components and services depending on a library of `interface`s, they depend on libraries of `Command`s and `Event`s. 

Instead of starting your design by thinking about what the core Interfaces should look like, you start your design by thinking about what you core Commands and Events look like (see: Event Storming).

Instead of the `Client` class implementing an interface that might look like this:
````
interface IClient
{
    void StartConnection();
    void EndConnection();
    void ExecuteOrder(Order order);
    void CompleteOrder(Order order);
}
````
You have to write a single, ugly `Update` function that responds to these:
````
class StartConnectionEvent { ... }
class EndConnectionEvent { ... }
class ExecuteOrderEvent { ... }
class CompleteOrderEvent { ... }
````
The technique we use to do this is called Pattern Matching. C# doesn't have great support for this (yet), so we end up having to abuse `if` statements. F# however is well build for this sort of thing and interops pretty seamlessly with C#. You may not understand F#, but compare the `Update` function in the `ClientState` class to this:
````
let update state event = 
    match event with
    | StartConnectionEvent -> // Update and return new "state" instance
    | EndConnectionEvent -> // Update and return new "state" instance
    | ExecuteOrderEvent -> // Update and return new "state" instance
    | CompleteOrderEvent -> // Update and return new "state" instance
````
This style of coding is built into the core of the language. 

F# would also give us safety advantages. For example, imagine that someone added a new event to this collection, lets call it `ConfigureConnectionEvent`. In the C# implementation, the compiler will give us no help whatsoever to make sure that the new case is properly handled by all of our code. At least if we were using imperative style Interfaces, the code would no longer compile until we added the new `ConfigureConnection()` method to all of our implementing classes.

If we were using F# we could write the Event types in such a way that the compiler would tell us that we were missing a particular case from our pattern matching statements (see: Union Types).

Anyway, as much as a F# fanboy that I may be, you could carry on fine with your domain code in C#. You would just need to get used to this strange new style and be more careful what adding new Commands and Events to your domain. I think that in a future implementation I would still use C# for all of the Akka plumbing code as the F# APIs are second class citizens and not as mature.

## TDD
