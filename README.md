# 🧩 Narrow-Phase Collision Detection (SAT & GJK)

This project explores two widely used narrow-phase discrete collision detection algorithms in computer graphics and game engines.

#### ⚙️ Algorithms
- **SAT / OBB (2D)** – Collision detection and penetration resolution for oriented convex 2D shapes.  
  *(Separating Axis Theorem)*
- **GJK (3D)** – Collision detection between arbitrary convex 3D shapes.  
  *(Gilbert–Johnson–Keerthi algorithm)*

#### ✨ Design Notes
- SAT operates on oriented bounding boxes and convex polygons.
- SAT does not scale well to 3D, making it impractical for complex scenes.
- GJK is preferred in 3D due to better scalability for convex collision detection.

#### 🎨 Implementation & Visualization
- All algorithms are implemented from scratch in Unity for learning purposes.
- SAT visualizes the Minimum Translation Vector (MTV) to show penetration depth and resolution direction.
- GJK visualizes only collision states and world-space vertices of convex shapes  
  *(MTV extraction via EPA is not implemented.)*

---

## 📸 Project Screenshots from Unity

#### 🔷 SAT / OBB (2D)

<div align="left">
  <img src="Assets/Resources/obb-sat.png" width="900">  
</div>

- Cyan arrow represents MTV
- Red outlines indicate detected collisions between convex shapes


#### 🧊 GJK (3D)

<div align="left">
  <img src="Assets/Resources/gjk.png" width="900">  
</div>

- Red points show world-space vertices of convex meshes
- Scene highlights collision state detection (no EPA or MTV extraction)

---

## 🎥 Demo Video
A short walkthrough demonstrating the SAT (2D) and GJK (3D) collision detection in action is available [here](https://youtu.be/NrKk_oyRyNc).

