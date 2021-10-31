# How does this thing work?

RTK provides a couple layers of functionality, which will be described in more detail below.

First, it uses a dynamic assembly generation library `Il2CppToolkit` which is based on Prefare's `Il2CppDumper` but provides support to do all of this work at runtime on the client machine, rather than requiring a developer to release a new build every time there is an update to the game assembly.

Next, the service application runs in the background, and using this library it constantly reads and updates relevant game data- persisting it locally so you always have it available even when the game isn't running.

Lastly, the service application provides two modes to communicate with it via websockets. The first is for local native applications where they have access to the process via the loopback interface (`wss://localhost:9090`), and the second is for web applications which cannot access local resources, which can communicate using our proxy service at `wss://raid-toolkit.azurewebsites.net/source`.

## Dynamic assembly generation

`Il2CppToolkit` provides a library which will process the game static_metadata + GameAssembly.dll to reverse-engineer the classes and memory addresses of objects defined in the game; from that information, it replicates the same object/API structures with APIs to access and read data from the game process using this generated code. The actual code being generated is very small (a couple IL instructions at most), and is mostly just "shape" information like classes, members, etc.

The final assembly is emitted using `Lokad.ILPack` to save it to disk, and it is kept for the next time the application is run until a new game version is detected, where it will do this process all over again.

## Background processing

By default, every 10 seconds RTK will scan active processes for `Raid` processes (it supports multi-client!), and in turn will fetch the latest data from each otf those processes and the accounts they are logged in with. All of the data is persisted using a durable hash of the users account ID, so <u>these IDs are not persisted in plain-form outside of the game</u>.

## WebSockets

The websocket implementation exposed on the local loopback is based on [@remote-ioc/runtime](https://github.com/dnchattan/remote-ioc) messages. The APIs exposed are defined in the `SDK\DataModel\APIs` directory.

For web clients, the client first connects to the proxy service, which will return an `ack` message with a `channelId`. This channel ID and the clients Origin should be passed to RTK on the local machine by opening `rtk://open?channel=${msg.channelId}&origin=${document.location.origin}`.

When the client receives this over the protocol handler, it will ask the user for permission for the website to access their data, and once accepted it will connect to the proxy service itself and deliver an access response targeting the `sessionId` it was provided.

The proxy service will send an authorization response to the web client, indicating it has been connected (or refused), at which point it can start communicating with the RTK process over that channel. The `send` message contents will be forwarded as the `@remote-ioc` message subprotocol.
