# Unity 3D Driving Prototype

[![Demo Video](https://img.youtube.com/vi/CwJNa5eDHWI/maxresdefault.jpg)](https://youtu.be/CwJNa5eDHWI)

**[🎥 Click here to watch the gameplay demo on YouTube!](https://youtu.be/CwJNa5eDHWI)**

## 🚀 About the Project
This is a 3D driving game prototype built in **Unity** using **C#**. Instead of relying on basic Unity presets, this project was developed from the ground up to demonstrate a strong understanding of clean code architecture, physics-based movement, and modular game systems. It serves as a showcase of my ability to solve common game development problems such as physics tunneling, state management, and optimized game loops.

## 🛠️ Key Technical Features

### 1. Physics-Driven Vehicle Controller
Migrated from frame-dependent `Transform.Translate` to a fully physics-driven `Rigidbody.MovePosition` and `MoveRotation` system inside `FixedUpdate`. This ensures smooth, jitter-free movement and completely eliminates "tunneling" glitches at high speeds using Continuous Dynamic collision detection.

### 2. Algorithmic Road Boundaries
Engineered a custom mathematical ray-casting algorithm to keep the vehicle strictly within the confines of a custom-shaped road polygon. If the car drifts out of bounds, the math safely and cleanly snaps it back without relying on bulky invisible physical walls.

### 3. Modular Traffic System
Built a dynamic traffic light state machine that:
*   Manually manipulates material emissions (`_EmissionColor`) through code to cycle lights.
*   Links to invisible trigger barriers (`OnTriggerStay`) that halt the car strictly on Red or Yellow lights.
*   Uses software-level failsafes to cancel forward input while touching a barrier, preventing physics glitching.

### 4. Optimized Game Loop & State Reset
Developed a centralized `GameManager` (Singleton) to handle scoring, UI updates, and win/loss conditions. Rather than using brute-force scene reloading, the game utilizes a highly optimized "soft-restart" system. It records the initial transformations and states of the player and all collectibles on `Start()`, allowing for instantaneous resets and respawns without dropping frames or losing references.

### 5. Event-Driven Audio
Implemented positional 3D audio (`AudioSource.PlayClipAtPoint`) that dynamically tracks the main camera to ensure collision and collection sounds are always audible to the player, regardless of where the event occurs in the world.

## 💻 Tech Stack
*   **Engine:** Unity 6
*   **Language:** C#
*   **Version Control:** Git / GitHub

## 🤝 Connect with Me
*   **LinkedIn:** [Yitzchak Kupinsky](https://www.linkedin.com/in/yitzchak-kupinsky/)
