# Fruit Swap Task

## Overview

This project is my case study for building a match-3 style game in Unity. It focuses on creating a scalable, fast-to-prototype system using a few techniques. Below, I outline the architecture and technical choices I made.  

If you just want to jump in, the core flow is in `GameManager -> Update`.

---

## Architecture

The project makes heavy use of Dependency Injection and asynchronous tasks. Both have their trade-offs, but for this prototype they worked really well.  

Managers are created as needed, usually controlled by the GameManager or objects that rely on their behaviour. They persist across scenes, though they could easily be disposed of between scenes if desired.  

Animations and game flow run asynchronously. Sometimes tasks handle exceptions or cancellations, sometimes they don’t. I mainly used cancellations for pooled board objects to prevent tasks from interfering with reused objects.  

Other points worth noting:

- **Object Pooling:** All BoardObjects and Effects are pooled. The generic pooling system works well for prototypes, letting me pass in any factory method as long as it implements `IPooledObject`.  
- **Manager Lifecycle:** Managers have multiple initialisation stages to ensure dependencies are ready when needed, and they are properly disposed of when no longer required.  
- **BoardObjectFactory:** Separates some responsibilities out of `GridManager`. `GridManager` probably still does a bit too much, but it’s fine for a prototype.  
- **Async Handling in GameManager:** Not all async calls are fully protected. I didn’t see any issues during testing, so I didn’t wrap everything in try-catch preemptively.  
- **Scene Transitions:** I really like the automatic fade-in/out on scene loads—it’s simple but satisfying.
- **Input:** Input.MouseButtonDown does support single touches, I saw no need to implement touches directly as the game doesn't need that data. But easily adapted if required.

I hope this project demonstrates my ability with basic and some more advanced Unity functionality, as well as project architecture. I also tried to make it at least a little bit pretty :) 
