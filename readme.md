Gameplay Video: https://youtube.com/shorts/ltYdf3NoFa4

# Fruit Swap Task

## Overview

This project is my case study for building a match-3 style game in Unity. It focuses on creating a scalable, fast-to-prototype system using a few techniques. Below, I outline the architecture and technical choices I made.  

If you just want to jump in, the core flow is in `GameManager -> Update`. Alternatively if you want to see the more architectural decitions check out `Injection`, `ObjectPoolManager` and `GridManager`

---

## Architecture

The project makes heavy use of Dependency Injection and asynchronous tasks. Both have their trade-offs, but for this prototype they worked really well.  

Managers are created as needed, usually controlled by the GameManager or objects that rely on their behaviour. They persist across scenes, though they could easily be disposed of between scenes if desired.  

Animations and game flow run asynchronously. The core game loop, grid logic and board object logic are all run with a safe async flow either by logging issues or by a cancellation token. For the effects, it's more loose, as the re-use timing means effects cannot run tasks again during their lifespan.

Game Settings are stored in scriptable objects and loaded when required. This configuration data could be replaced by any source, as long as the settings manager produces a GameSettings object from it. In this case ScriptableObjects are just a simple container.

Other points worth noting:

- **Object Pooling:** All BoardObjects and Effects are pooled. The generic pooling system works well for prototypes, letting me pass in any factory method as long as it implements `IPooledObject`.  
- **Manager Lifecycle:** Managers have multiple initialisation stages to ensure dependencies are ready when needed, and they are properly disposed of when no longer required.  
- **BoardObjectFactory:** Separates some responsibilities out of `GridManager`. `GridManager` probably still does a bit too much, but it’s fine for a prototype.   
- **Scene Transitions:** I really like the automatic fade-in/out on scene loads—it’s simple but satisfying.
- **Input:** Input.MouseButtonDown does support single touches, I saw no need to implement touches directly as the game doesn't need that data. But easily adapted if required.
- **Save Data:** I decided not to implement a proper save data serializer for the sake of simplicity, and in the spirit of prototyping. PlayerPrefs is bad, but it's only one integer. I have an example for serializers I have written in the past on my github.

I hope this project demonstrates my ability with some more advanced Unity functionality, as well as project architecture. I also tried to make it at least a little bit pretty :) I produced this entire project from scratch in a weekend, aside from the visual elements provided, and am proud of the structure and the speed of delivery.
