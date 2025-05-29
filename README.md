

h-opc-pro
==============

An Opc Library and a command line to perform OPC operations with ease and transparency among different protocols. Currently supports synchronous operation over UA and DA protocols.
This fork is a netstandard2.0 port with minor improvements, will see if furthur maintenance will be carried out.

## Table of Contents
* [Use](#use)
* [Documentation](#documentation)
  * [Exploring the nodes](#exploring-the-nodes)
  * [Read a node](#read-a-node)
  * [Writing to a node](#writing-to-a-node)
  * [Monitoring a tag](#monitoring-a-tag)
  * [Go Asynchronous!](#go-asynchronous)
* [Command line](#command-line)
* [Disclaimer](#disclaimer)
* [Roadmap](#roadmap)


## Use

To install [`h-Opc-pro`](https://www.nuget.org/packages/h-opc-pro/), run the following command in the Package Manager Console:

    PM> Install-Package h-opc-pro


To install the command line interface, head to the [`nuget gallery`](https://www.nuget.org/packages/h-opc-pro/).

## Documentation

to use the UA Client simply...

````cs
using (var client = new UaClient(new Uri("opc.tcp://host-url")))
{
  client.Connect();
  // Use `client` here
}
````

or with options...

````cs
var options = new UaClientOptions {
  UserIdentity = new Opc.Ua.UserIdentity("<your-username>", "<your-password>")
};
using (var client = new UaClient(new Uri("opc.tcp://host-url")), options)
{
  client.Connect();
  // Use `client` here
}
````


and to use the DA Client instead:

````cs
using (var client = new DaClient(new Uri("opcda://host-url")))
{
  client.Connect();
  // Use `client` here
}
````

#### Exploring the nodes

You can get a reference to a node with...

````cs
var node = client.FindNode("path.to.my.node");
````

This will get you a reference to the node `node` in the folder `path.to.my`.

You can use the node reference to explore the hieriarchy of nodes with the properties `Parent` and `SubNodes`. For example...

````cs
Node parentNode = node.Parent;
IEnumerable<Node> children = client.ExploreFolder(node.Tag);
IENumerable<Node> grandChildren = children.SelectMany(m => client.ExploreFolder(m.Tag));
````

#### Read a node

Reading a variable? As simple as...

````cs
var myString = client.Read<string>("path.to.string").Value;
var myInt = client.Read<int>("path.to.num").Value;
````

The example above will read a string from the tags `string` and `num` in the folder `path.to`

### Read many nodes

```cs
List<ReadEvent<string>> stringItems = client.ReadMultiple<string>("path.to.string");
List<ReadEvent<int>> intItems = client.ReadMultiple<int>("path.to.num");
```

#### Writing to a node

To write a value just...

````cs
client.Write("path.to.string", "My new value");
client.Write("path.to.num", 42);
````

#### Monitoring a tag

Dead-simple monitoring:

````cs
client.Monitor<string>("path.to.string", (readEvent, unsubscribe) =>
{
  DoSomethingWithYourValue(readEvent.Value);
  if(ThatsEnough == true)
    unsubscribe();
});

````

The second parameter is an `Action<T, Action>` that has two parameter:

- `readEvent` contains all the information relevant to the event such as timestamps, quality and the value
- `unsubscribe` is a function that unsubscribes the current monitored item. It's very handy when you want to terminate your callback

it's **important** that you either enclose the client into a `using` statement or call `Dispose()` when you are finished, to unsubscribe all the monitored items and terminate the connection!

### Go Asynchronous!

Each method as an asynchornous counterpart that can be used with the async/await syntax. The asynchronous syntax is **recommended** over the synchronous one (maybe the synchronous one will be deprecated one day).

## Command line

You can also use the command line interface project to quickly test your an OPC. Build the `h-opc-cli` project or download it from the `release` page of this repository, then run:

````
h-opc-cli.exe [OpcType] [server-url]
````

Where `OpcType` is the type of opc to use (e.g: "UA", "DA"). Once the project is running, you can use the internal command to manipulate the variable. To have more information aboute the internal commands, type `help` or `?`


## Roadmap

- [ ] ...
