Fruit Swap Task

## Overview

This project is my Case Study for creating a match 3 style game in Unity. The project makes heavy use of a few techniques to create a scalable, quick to produce prototype. Below is an explanation of the architecture and technical decisions.

If you'd prefer to dive straight into the code I suggest starting with GameManager -> Update for the core flow.

---

## Architecture

The Project makes heavy use of Dependency Injection and Asynchronous Tasks. Both of these have risks and downsides, but for this project they worked extremely well. 

All managers are created when they are needed, primarily being controlled by the game manager and other in-game objects that require the behaviour. They are persisted through scenes, but could be disposed of between scenes just as easily. 

All animations and game flow are managed Asynchronously, sometimes accounting for exceptions and cancellations, and sometimes not. The decision to use cancellations was based on the danger in a task running over, during development the only place I needed to guarantee cancellation was in the Object Pooled board objects that could be re-used very frequently, though the delayed deactivation would prevent early re-use.

Other noteworthy elements:

- All BoardObjects and Effects are Object Pooled, with a generic object pooling method that I like for prototypes due to being able to pass any factory method I want in, as long as I inherit from IPooledObject

- Managers are properly disposed of and have multiple stages for initialisation for times where I needed to guarantee all other managers were initialised prior to steps in other managers

- BoardObjectFactory is primarily to separate concerns out of the GridManager. The GridManager is probably still doing a bit too much, but for a prototype it's fine.

- The GameManager does handle Async unsafely. I did not find any likely case where this caused issues, but wrapping everything in a try-catch seemed unnecessary prior to any issues.

- I like the automatic fade loading/unloading on scene load its very satisfying :) 

---