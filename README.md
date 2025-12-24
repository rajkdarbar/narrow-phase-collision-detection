# 🧩 Narrow-Phase Collision Detection (SAT & GJK)

This project explores two widely used **narrow-phase collision detection** algorithms in computer graphics and game engines.

#### ⚙️ Algorithms
- **SAT / OBB (2D)** – Collision detection and penetration resolution for oriented 2D shapes using the **Separating Axis Theorem**.
- **GJK (3D)** – Collision detection between arbitrary **convex 3D shapes** using the **Gilbert–Johnson–Keerthi algorithm**.

#### 🧠 Design Notes
- **SAT (2D)** operates on oriented bounding boxes and convex polygons.
- **SAT does not scale well to 3D**, making it impractical for complex scenes.
- **GJK is preferred in 3D** due to better scalability for convex collision detection.

#### 🎨 Implementation & Visualization
- All algorithms are implemented **from scratch in Unity** for learning purposes.
- **SAT** visualizes the **Minimum Translation Vector (MTV)** to show penetration depth and resolution direction.
- **GJK** visualizes only **collision states and world-space vertices** of convex shapes  
  *(MTV extraction via EPA is not implemented.)*

---

## Project Screenshots from Unity

#### SAT part
<div align="left">
  <img src="Assets/Resources/obb-sat.png" width="900">  
</div>

#### GJK part
<div align="left">
  <img src="Assets/Resources/gjk.png" width="900">  
</div>
