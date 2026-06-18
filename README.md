# Rollaball

A 3D arcade game built in Unity (URP) featuring responsive physics-based mechanics, customized shaders, and optimized layouts for web-based distribution.

## Live Demo
The latest stable WebGL distribution is hosted on itch.io:
**https://kaho412.itch.io/not-yet-named**

---

## Architecture & Implementation Details

* **Physics-Driven Controllers:** Movement is calculated using native 3D physics (`Rigidbody`), utilizing torque and force vectors for smooth, responsive player acceleration.
* **Dynamic Canvas Scaling:** The user interface uses TextMeshPro assets wrapped in an adaptive `Canvas Scaler` configured to a baseline resolution of `1920x1080` (0.5 match blend). This guarantees pixel-perfect layout preservation when embedded inside scalable browser viewports.
* **Modular Environments:** Built with stylized low-poly environment geometry, customized mist particle subsystems, and dynamic animated hazards utilizing isolated runtime scripts.

---

## Technical Highlights: WebGL Optimization

* **Dynamic Text Rendering:** Vector-based TextMeshPro configurations prevent text stretching or boundary clipping across fluid browser frames.
* **Repository Architecture:** Configured with an optimized internal tracking mask (`.gitignore`) to isolate structural code (`Assets/`, `Packages/`, `ProjectSettings/`) from heavy, auto-generated local databases (`Library/`, `Temp/`, `Obj/`).

---

## Local Development & Environment Setup

### Prerequisites
* **Unity 2022.3 LTS** (or compatible version)
* **WebGL Build Support** module
