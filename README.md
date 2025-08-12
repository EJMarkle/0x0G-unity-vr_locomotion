# LightBikes

A VR-enabled light cycle racing game inspired by Tron, built with Unity 6. Race against AI opponents while leaving deadly light walls in your wake and avoid crashing into them to survive in the arena.

---

## Table of Contents

- [Overview](#overview)
- [Features](#features)
- [Installation](#installation)
- [How to Play](#how-to-play)
- [Project Structure](#project-structure)
- [Technical Implementation](#technical-implementation)
- [Building the Game](#building-the-game)

---

## Overview

LightBikes is a multiplayer arena-based racing game where players control futuristic light cycles that leave glowing trails behind them. The objective is to be the last racer standing by forcing opponents to crash into these light walls while avoiding them yourself. The game features full VR support with intuitive hand-tracked controls and immersive 3D spatial audio.

---

## Features

### Core Gameplay

- **Arena-based Racing:** Fast-paced light cycle combat in a confined 3D arena
- **Light Wall Mechanics:** Dynamic trail system that creates deadly barriers
- **AI Opponents:** Intelligent AI racers with multiple difficulty levels
- **Elimination System:** Last-racer-standing gameplay with lives system
- **Multiple Game Modes:** Solo play against AI and potential multiplayer support

### VR Features

- **Full VR Support:** Compatible with OpenXR standard VR headsets
- **Hand Tracking:** Natural hand interactions using XR Hands system
- **Intuitive Controls:** Physical bike controls with throttle and steering knobs
- **Immersive Audio:** 3D spatial audio with engine sounds and collision effects

### Visual & Audio

- **Neon Aesthetic:** Tron-inspired visual design with glowing elements
- **Dynamic Lighting:** Real-time lighting effects for walls and bikes
- **Space Environment:** Multiple skybox options with space themes
- **Visual Effects:** Particle systems and trail renderers
- **Adaptive Audio:** Dynamic audio mixing based on gameplay events

---

## Installation

### Open in Unity

1. Launch Unity Hub
2. Click "Open" and select the project folder
3. Ensure Unity 6000.0+ is installed

### Configure XR Settings

1. Go to `Edit > Project Settings > XR Plug-in Management`
2. Enable OpenXR for your target platform
3. Configure interaction profiles for your VR headset

### Import Required Packages

The project includes all necessary packages:

- XR Interaction Toolkit (3.1.1)
- Universal Render Pipeline (17.0.4)
- Input System (1.13.1)

---

## How to Play

### VR Controls

- **Throttle:** Use the right-hand knob to control speed
- **Steering:** Use the left-hand handlebar to steer your light cycle
- **Braking:** Pull back on the throttle or use head lean for braking
- **Menu Navigation:** Point and click with controllers or use hand tracking

### Gameplay Objective

- **Survive:** Avoid crashing into light walls (yours or opponents')
- **Eliminate:** Force AI opponents to crash into walls
- **Strategy:** Use speed and positioning to trap enemies
- **Victory:** Be the last racer alive to win the round

### Game Mechanics

- **Light Walls:** Created automatically as you move with throttle engaged
- **Collision:** Any contact with walls results in elimination
- **Lives System:** Each racer starts with multiple lives
- **Speed Management:** Balance speed with maneuverability

---

## Project Structure

```text
Assets/
├── Animations/              # Animation clips and controllers
├── Audio/                   # Sound effects and music
├── Materials/               # Visual materials and shaders
├── Physics Materials/       # Physics materials for collision
├── Prefabs/                 # Reusable game objects
│   ├── Bike.prefab         # Main player bike prefab
│   ├── LightWall.prefab    # Wall segment prefab
│   └── AIWall[1-3].prefab  # AI-specific wall variants
├── Scenes/                  # Game scenes
│   ├── Menu.unity          # Main menu scene
│   └── Arena.unity         # Main gameplay arena
├── Scripts/                 # C# game logic
│   ├── GameManager.cs      # Core game state management
│   ├── BikeControls.cs     # VR input handling
│   ├── BikeMovement.cs     # Physics-based movement
│   ├── LightWallTrail.cs   # Trail generation system
│   ├── AIControls.cs       # AI behavior implementation
│   └── Racer.cs           # Base racer component
├── Settings/               # Project configuration
├── VRTemplateAssets/       # Unity VR template assets
└── XR/                     # XR configuration files
```

---

## Technical Implementation

### Core Systems

#### Movement System

- Physics-based bike movement using Rigidbody
- Hover mechanics with multiple hover points
- Visual tilt system for immersive feedback
- Speed-based audio and visual effects

#### Input System

- `IBikeInput` interface for modular input handling
- VR controller and hand tracking support
- AI input compatibility
- Deadzone and sensitivity controls

#### Trail System

- Dynamic wall segment spawning
- Distance-based segment creation
- Automatic cleanup and memory management
- Collision detection with multiple layers

#### AI System

- State machine-based AI behavior
- Pathfinding and obstacle avoidance
- Dynamic difficulty adjustment
- Multiple AI personality types

### Technical Specifications

- **Render Pipeline:** Universal Render Pipeline (URP)
- **Physics:** Unity Physics with custom collision layers
- **Audio:** 3D spatial audio with AudioSource management
- **Performance:** Optimized for VR 90fps target

---

### Debugging

- **Console Logs:** Comprehensive logging for all major systems
- **Visual Debugging:** Gizmos for collision detection and AI pathfinding
- **VR Debugging:** On-headset debug UI for real-time parameter tuning

---

## Building the Game

### Build Profiles

The project includes build profiles for:

- **PC VR:** Windows standalone with OpenXR
- **Android:** Quest/Android-based headsets
- **Development:** Debug builds with additional logging

### Build Steps

1. Open `File > Build Profiles`
2. Select your target platform
3. Configure XR settings for the platform
4. Build and deploy to your VR headset

### Optimization

- Enable GPU Instancing for wall segments
- Configure URP settings for VR performance
- Optimize physics calculations for stable framerates
- Use LOD groups for distant objects

### Third-Party Assets

- **Unity Technologies:** XR Interaction Toolkit, Universal Render Pipeline
- **Deep Space Skybox Pack:** Space environment visuals
- **Thaleah Pixel Font:** UI typography
