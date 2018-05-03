# SpatialOS - Worker authority demo project
---

**SpatialOS**: [find out more here](https://improbable.io/games)

**Please note:** this project only works on Windows

---

## About the project

### Introduction

Worker authority is a central concept for working with SpatialOS. In your game you may have multiple types of workers operating on different aspects of your world, and for each worker type you may have many worker instances. Every component of every entity can individually be written to or updated by one single worker at a time: the authoritative worker. There are however multiple reasons why exactly authority over a component may be moved between workers at runtime.

David Rodriguez, one of our engineers in the Production team, put together a small "tank warfare" demo, with some simple behaviour and tools for visualising worker authority over particular components. We hope that it will help you to better understand how to reason about component authority and it is a great starting point to dig even deeper into this topic by yourself.

### Setup

To run the project simply download the repository and run `spatial worker build` from within the project root directory. You will need Unity Engine installed so if you haven't already run through [the SpatialOS setup for Windows](https://docs.improbable.io/reference/latest/shared/get-started/setup/win) yet you should do so now.

When you start the demo (`spatial local launch`) the `UnityWorker` instances and C# worker instances will be started automatically. You can then open the UnityClient worker in your Unity Engine editor and connect to the simulation by hitting play.

The UI within the demo should guide you the rest of the way, but for some more information read on!

### Explanation

The SpatialOS load-balancer may decide to re-allocate a component to a different worker, _i.e._ change authority, for a few different reasons:

- The original worker crashed
- Movement of the tank unit to which the component belongs
- The "centre of gravity" of the workers moved (changing the location of the implicit boundary or overlap between the AI workers)

In each of these situations the load-balancer may decide that the best ongoing authority strategy is one which involves an immediate change. Both the former and the receiving worker are notified of the authority change with an `AuthorityChange` operation.

Working with SpatialOS, and debugging your logic, requires an understanding of when these authority changes may be occurring in your game. You should then put some thought into how you can ensure that the worker that receives authority over the component is given all information necessary to seamlessly continue the simulation.

This demo project offers an interactive way to visualize authority changes within a toy example. The code is not an example of "best practice" but it should nonetheless be of interest to anyone interested in what load-balancing configurations for multiple worker types can look like as well as for anyone interested in seeing a C# worker and Unity SDK worker operating together in the same project.

The scenario is a tank warfare game in which tanks drive in a circle around the centre of the world. There are two 'aspects of the world' to be simulated, each with an associated SpatialOS component:

- The position of each tank unit (`Position`); the tank will remain stationary unless the authoritative worker updates the position property
- The rotation angle of the gun turret (`TurretInfo`); the turret will remain at a fixed orientation unless the authoritative worker updates the rotation property

Each aspect of the world has a unique worker type responsible for it. Authority for the tank `Position` component is only ever given to a worker of the type that is built using the Unity SDK. Authority for the `TurretInfo` component is only ever given to a worker of the type built using the C# SDK. The demo is configured to have four `UnityWorker` instances and two `CSharpWorker` instances.

You can find more information about authority, and how to handle changes of authority in the [docs](https://docs.improbable.io/reference/latest/shared/design/understanding-access#understanding-read-and-write-access-authority).

### Feedback

Did you find this demo helpful? Do you have more questions? Bring your comments and ideas to the [SpatialOS forums](https://forums.improbable.io/)!

