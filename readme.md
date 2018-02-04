# AkkaExchange
## Event sourced commodity exchange 
This is a learning project and as such certainly falls fowl of many good software principles. The primary aims are to learn about:
- Event Sourcing
- CQRS
- Akka.NET Persistence & Streaming
Everything else is secondary until the I can find the time to do it properly.

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
- [ ] Figure out what a `ICommandHandler` should really look like. Is it appropriate to return errors here? Should you provide the new State alongside the Event?
- [ ] Better improve the consistency charactistics of the system. There are currently many oppotunities to screw your state up.
- [ ] Develop client to be a bit more mature and fully featured, showing charts for price, volume, order history etc.
- [ ] Rewrite core domain code in pure F#. (See below.)
- [ ] Connect it to a database for persistent sessions. Maybe also add auth for web clients.
- [ ] Tidy up stream implementations; get rid of hacky internal Observable/Subject code and use the Akka.NET native `ISubscriber` and `IPublisher` interfaces. 
- [ ] Better error handling for failed order execution. In fact, better error handling everywhere.

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

## Test Driven Development
One regret of the project was that I didn't use TDD more effectively. Often when I ran into a problem I didn't understand (e.g. creating an Akka Stream from an `IActorRef` source), I ended up writing a bunch of tests anyway to help me narrow down the source of the issue.

The functional, state-machine nature of the `ICommandHandler`s and `IState.Update` functions in particular lend themselves very well to simple unit testing as you don't have to worry much about how you get into your initial "Arrange" state. This is one of the big advantages of immutable programming. 

When writing code in a mutable OO form, your state is hidden and encapsulated inside a class. The class then exposes a domain specific API, in the form of Methods, that control how the internal state can be modified.

The difficulty with this approach is that it can make it difficult to write isolated unit tests. In order to get your class from its default state to state Z, you often have to execute Methods X and Y beforehand. This introduces temporal dependencies into your code that makes your unit tests more coupled and harder to reason about. When you break method X, don't be surprised if the tests that check methods Y and Z also break.

Seperating your state from your functionality makes this a lot easier without affecting the strong consistency characteristics that you get from traditional encapsulation. Note: the `IState` classes aren't good examples of this as the encapsulate both state and functionality. The `ICommandHandler` methods are a better example of what I mean.

Also frankly a lot of the actor / messaging / streams stuff is difficult to reason about for a noob like me. Starting with tests allow you to ensure that you understand the most basic concepts before building it up bit by bit to end up with what you want. See the `AkkaStreamsTests` class for an example of what I mean.

# Conclussion
Although I'm new to all this stuff, I am starting to recognise the advantages to using this approach when the complexity of the problem you are trying to solve justifies the complexity of the means. Namely:
- Event Sourcing is "lossless". You persist the events that create your state rather than the state itself. This makes it easy to see a complete audit trail of everything that has happened in your system. It's no surprise that most critical systems like databases and financial ledgers use this approach. To paraphrase Greg Young:
> Do you think that your bank balance is a value or a calculation?
- Messaging and CQRS are resiliant by default. You write code in such a way that your components don't wait for responses from their collaborators. This is known as "Tell don't ask". It forces the programmer to think explicitly about what happens when components fail to communicate with each other. It makes it less likely that a poorly implemented method will bubble a `NullReferenceException` all the way up the stack and blow the process up. The trade off is that it is sometimes difficult to tell exactly what is going on in the system. The only way to offset this is to implement good logging and great testing. It forces you to be a better programmer.
- I'm not going to use this for every CRUD app I ever build. There are a lot of concepts to learn and plumbing to grind through. But this approach would be my go-to if I was ever tasked with building something complex or important enough that a large amount of money or lives depended it. I'd recommend that you don't employ me to do this anytime soon.

That's all folks!
@jazzyskeltor
