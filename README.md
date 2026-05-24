# 🏙️ Smart City – Resiliens
### Projet de Réalité Augmentée | AR Project
 
---
 
## 📖 About the Project
 
**Smart City – Resiliens** is an augmented reality application built with Unity and Vuforia that simulates an interactive smart city. The player acts as an urban operator whose mission is to detect and resolve critical incidents threatening the city before time runs out.
 
The city comes to life anchored to a visual marker detected by the device's camera. Once the AR scene is loaded, incidents begin to appear randomly across the city — fires, power outages, water leaks, and road accidents — each requiring the player to complete a dedicated mini-game within a time limit. Fail to respond in time and the city deteriorates. Resolve all incidents and the Smart City survives.
 
The project was developed as part of a university AR course, applying concepts of 3D scene management, gamification, UI/UX design, and real-time interaction using Unity 6 and Vuforia Engine 11.
 
---
 
## ✨ Features
 
- 📍 **AR Marker Tracking** — City anchored to a physical image target via Vuforia
- 🔥 **Fire Incident** — Fire extinguisher mini-game: drag to aim, spray smoke clouds to extinguish fire points
- ⚡ **Power Outage** — Cable rewiring mini-game
- 💧 **Water Leak** — Virtual valve control mini-game
- 🚗 **Road Accident** — One-tap incident resolution
- 🔊 **Audio & Visual Effects** — Distinct alarms and particle systems per incident type
- ⏱️ **Global Timer** — Survive long enough and the Smart City wins
---
 
## 🛠️ Tech Stack
 
| Tool | Version |
|------|---------|
| Unity | 6000.0.46f1 |
| Vuforia Engine | 11.4.4 |
| Language | C# |
| Target Platform | Android |
| 3D Assets | Cartoon City Free Low Poly Pack (it happy) |
 
---
 
## 🚀 Getting Started
 
### 1. Clone the Repository
 
```bash
git clone https://github.com/ahmed6xx/AR-project-2026.git
```
 
### 2. Open in Unity Hub
 
- Open **Unity Hub**
- Click **Open > Add project from disk**
- Select the cloned folder (the one containing the `Assets/` directory)
- Make sure you have **Unity 6000.0.46f1** installed. If not, install it via Unity Hub under **Installs > Install Editor**
### 3. Fix the Vuforia Package (Expected Step)
 
> ⚠️ **This step is required.** Vuforia is not committed to the repository due to its file size, so the project will show errors on first open. This is expected.
 
**To fix it:**
 
1. Download the Vuforia package file:
   ```
   com.ptc.vuforia.engine-11.4.4.tgz
   ```
   📥 **Download link:** `[[https://drive.google.com/drive/folders/1VLyRoTOxqJOaajtLyBU9GkefJTfcBcM5?usp=drive_link]]`
2. Place the downloaded `.tgz` file into the following folder inside the cloned project:
   ```
   YOUR_PROJECT_FOLDER/Packages/
   ```
   So the final path should look like:
   ```
   YOUR_PROJECT_FOLDER/Packages/com.ptc.vuforia.engine-11.4.4.tgz
   ```
 
3. Open (or reopen) the project in Unity. Unity will detect the local package automatically and resolve the Vuforia dependencies.
4. Once Unity finishes importing, the errors should clear and the project should compile successfully.
---
 
## 📁 Project Structure
 
```
Assets/
├── Resources/          # Sprites, icons, audio clips
├── Scenes/             # Unity scenes
├── Scripts/            # All C# game scripts
│   ├── GameManager.cs
│   ├── FireExtinguisherMinigame.cs
│   ├── FirePoint.cs
│   └── ...
├── ithappy/            # Cartoon City 3D asset pack
Packages/
├── manifest.json
└── com.ptc.vuforia.engine-11.4.4.tgz   ← place it here
```
 
---
 
 
## 📄 License
 
This project was developed for academic purposes as part of a university Augmented Reality course.
