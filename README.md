# VR Labeling Software
### A VR/AR tool for 3D anatomical model labeling on Meta Quest 3

Built for **Dr. Evan Goldman** at the Penn State College of Medicine, this Unity application allows medical instructors to place, annotate, search, and manage labelled pins on 3D anatomical models in both VR and AR environments.

---

## Current Feature Status

| Feature | Status |
|---|---|
| VR / AR mode switching | ✅ Working |
| Add label pin to model | ✅ Working |
| Remove label pin | ✅ Working |
| Highlight label pin | ✅ Working |
| Search algorithm (label term lookup) | ✅ Working |
| Note-adding (bottom section of label) | ✅ Working |
| Label always faces user camera | ✅ Working |
| Menu follows head movement | ✅ Working |
| Rotate 3D model | ✅ Working |
| Model import (.obj loading) | 🔧 In Progress |
| Save label data (persistence) | 🔧 In Progress |
| Load previously saved labels | 🔧 In Progress |

---

## Overview

The VR Labeling Software allows instructors and medical professionals to:

- Place **labelled pins** on the surface of 3D anatomical models in VR or AR
- Add a **term** (e.g. "Left Ventricle") to the upper section of each label
- Write **freeform notes** in the bottom section of each label
- **Highlight** specific pins to bring them into focus during instruction
- **Search** all labels by their term text in real time
- **Rotate** models freely for inspection from any angle
- Switch seamlessly between **VR** (immersive) and **AR** (pass-through) modes
- Have labels **always face the user's camera**, regardless of model orientation
- Navigate via a **menu that follows head movement**, keeping controls accessible at all times

---

## Architecture

The system follows a **layered MVC-inspired architecture** adapted for Unity-based VR applications:

```
┌─────────────────────────────────────────────────┐
│            Presentation / View Layer             │
│   Unity Scene · Label GameObjects · VR UI        │
└────────────────────┬────────────────────────────┘
                     │
┌────────────────────▼────────────────────────────┐
│         Controller / Interaction Layer           │
│  VRInputController · UIController · Raycast      │
└────────────────────┬────────────────────────────┘
                     │
┌────────────────────▼────────────────────────────┐
│              Domain / Model Layer                │
│  LabelManager · LabelPin · LabelSearch           │
│  ModeManager · ModelRotator · LabelBillboard     │
└────────────────────┬────────────────────────────┘
                     │
┌────────────────────▼────────────────────────────┐
│             Persistence Layer (WIP)              │
│      PersistenceManager · ExportService          │
└─────────────────────────────────────────────────┘
```

### Key Classes

| Class | Responsibility |
|---|---|
| `LabelManager` | Add, remove, highlight, and retrieve all label pins |
| `LabelPin` | Stores term, note, anchor position, and highlight state |
| `LabelSearch` | Real-time search across all pin terms (case-insensitive) |
| `LabelBillboard` | Rotates label to always face the user's camera |
| `HeadFollowMenu` | Lerps the main menu to stay in the user's field of view |
| `ModeManager` | Toggles between VR and AR (pass-through) modes |
| `ModelRotator` | Handles controller drag input to rotate the 3D model |
| `VRInputController` | Maps Meta Quest 3 controller events to system actions |
| `UIController` | Manages VR UI panels, search input, and feedback messages |
| `ModelManager` | *(In progress)* Loads .obj model files into the scene |
| `PersistenceManager` | *(In progress)* Saves and loads label data across sessions |

---

## Quick Start

### Prerequisites

- [Unity 2022.3 LTS](https://unity.com/releases/lts)
- [XR Interaction Toolkit](https://docs.unity3d.com/Packages/com.unity.xr.interaction.toolkit@2.5/manual/index.html) `2.5.x`
- [Meta XR SDK](https://developer.oculus.com/downloads/package/meta-xr-core-sdk/) `60.x`
- Meta Quest 3 headset (or XR Device Simulator for editor testing)

### Setup

1. **Clone the repository**
   ```bash
   git clone https://github.com/pruhnav/capstoneproject2026.git
   cd capstoneproject2026
   ```

2. **Open in Unity**  
   Open Unity Hub → Add project from disk → select the cloned folder  
   Unity version: **2022.3 LTS**

3. **Install packages** (if not auto-resolved)  
   Window → Package Manager → install:
   - XR Interaction Toolkit `2.5.x`
   - XR Plugin Management
   - Meta XR Core SDK

4. **Configure build target**  
   File → Build Settings → Android → Switch Platform  
   Player Settings → XR Plug-in Management → enable **OpenXR** (or Meta XR)

5. **Build and deploy to Quest 3**
   ```
   File → Build Settings → Build and Run
   ```
   Or use **Link / Air Link** for testing directly from the editor.

---

## Running Tests

Tests use Unity's built-in **NUnit Test Framework**.  
Open: `Window → General → Test Runner`

### EditMode Tests (no headset required)
```
Test Runner → EditMode → Run All
```
Covers: LabelManager, LabelPin, LabelSearch, ModeManager, ModelRotator, LabelBillboard

### PlayMode Tests (simulated XR)
```
Test Runner → PlayMode → Run All
```
Covers: VR/AR mode switching, label billboarding, menu follow, controller interaction  
Uses the **XR Device Simulator** — no physical headset needed.

### Code Coverage
```
Window → Analysis → Code Coverage → Start Recording → Run All Tests → Generate Report
```
Current coverage across implemented modules: **~90.6%**

---

## Label Data Format

Each label pin stores the following fields (planned JSON schema for persistence):

```json
{
  "labelID": "a1b2c3d4",
  "term": "Left Ventricle",
  "note": "Largest pumping chamber; contracts during systole.",
  "anchorPosition": { "x": 0.12, "y": -0.05, "z": 0.34 },
  "isHighlighted": false
}
```

---

## Known Limitations

- **Model import** is not yet implemented — the current build uses a pre-loaded model in the scene. `.obj` file loading via `ModelManager` is in progress.
- **Save / Load** is not yet implemented — label pins placed in a session are lost on restart. `PersistenceManager` is scaffolded and in development.
- No CSV export yet (planned alongside persistence).

---

## Repository Structure

```
capstoneproject2026/
├── Assets/
│   ├── Scripts/
│   │   ├── Labels/          # LabelManager, LabelPin, LabelBillboard
│   │   ├── Search/          # LabelSearch
│   │   ├── Interaction/     # VRInputController, RaycastService
│   │   ├── UI/              # UIController, HeadFollowMenu
│   │   ├── Mode/            # ModeManager (VR/AR toggle)
│   │   ├── Model/           # ModelRotator, ModelManager (WIP)
│   │   └── Persistence/     # PersistenceManager (WIP)
│   ├── Tests/
│   │   ├── EditMode/        # Unit + integration tests (no hardware)
│   │   └── PlayMode/        # VR/AR and interaction tests (XR Simulator)
│   ├── Scenes/
│   │   └── Main.unity       # Primary VR/AR labeling scene
│   └── Prefabs/
│       └── LabelPin.prefab  # Label pin prefab with billboard component
├── Packages/
│   └── manifest.json
└── README.md
```

---

## Technology Stack

| Category | Technology |
|---|---|
| Engine | Unity 2022.3 LTS |
| Language | C# |
| VR/AR Framework | XR Interaction Toolkit 2.5 |
| Headset | Meta Quest 3 (OpenXR) |
| Testing | Unity Test Framework (NUnit) |
| Coverage | Unity Code Coverage Package |
| Documentation | C# XML DocFX |
| Version Control | Git / GitHub |

---

## Team — CMPSC 488, Fall 2025

- Khush Mistry
- Lasya Madhuri Gundapaneni
- Pranav Balachander
- Yamini Satish

**Faculty:** Dr. Sayed Mohsin Reza · Dr. Evan Goldman  
**Institution:** The Pennsylvania State University

---

## License

This project is licensed under the [MIT License](LICENSE).

---

## Acknowledgments

- Built with [Unity](https://unity.com/) and the [XR Interaction Toolkit](https://docs.unity3d.com/Packages/com.unity.xr.interaction.toolkit@2.5/manual/index.html)
- VR/AR support via [Meta XR SDK](https://developer.oculus.com/downloads/package/meta-xr-core-sdk/)
- Developed for anatomy education at the Penn State College of Medicine
