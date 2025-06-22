# 🌌 Eldora Framework – RPG & Simulation System (Unity + Fusion)

Welcome to the **Eldora Framework**, a modular RPG and quantum-inspired simulation toolkit built in Unity using **Photon Fusion**. It blends classic RPG mechanics with modern multiplayer networking, immersive UI, dynamic visuals, and deep narrative systems. This project includes over **55 modular C# scripts** and is tailored for top-down 2D and hybrid environments with fantasy/quantum themes.

---

## 📦 Features Overview

* 🔁 Multiplayer with Photon Fusion (Shared mode)
* 🧠 Advanced Enemy AI (Group tactics, separation, orbit/charge logic)
* 🎮 Modular Player Controllers (Top-down / 2D platformer)
* 🗺️ Quest system (Trigger, GUI, reward popup, persistence)
* 🪐 Atom simulation-inspired systems
* 🖥️ GUI-based inventory, equipment & HUD
* 🎬 Dynamic intro & camera traveling
* 💬 Dialogue system (chat bubble, trigger-based, stylized UI)
* 🌙 Day/Night cycle & environmental FX
* 🧩 Modular interaction system with triggers

---

## 📁 Project Structure

### 🎮 Player & Input

* `PlayerController.cs` – Main movement, dash, attack, regeneration, Fusion-ready.
* `PlayerController2D.cs` – Alternative 2D platformer variant.
* `PlayerSpawner.cs`, `LocalPlayerSpawner.cs`, `PCSpawner.cs` – Network-aware spawning logic.
* `SwordAttack.cs` – Collider-based melee attack with cooldown.

### ⚔️ Enemies & AI

* `Enemy.cs` – Main AI behavior: orbit/charge, attack patterns, health, audio.
* `EnemyController2D.cs` – Physics-based 2D controller variant.
* `EnemyManager.cs` – Spawner/manager for grouped AI behaviors.

### 📡 Networking (Photon Fusion)

* Fully configured `NetworkObject`, `NetworkTransform`, and authority delegation in all controllers.
* Shared mode logic integrated via `FusionBootstrap`, `GameInitializer`, etc.

### 📜 Quest System

* `QuestManager.cs`, `QuestData.cs`, `QuestSystem.cs` – Quest core logic and persistence.
* `QuestTrigger.cs`, `QuestRewardPopup.cs`, `QuestGUI.cs` – Triggers, UI, and reward system.

### 🧙 Dialogue System

* `DialogueSystem.cs`, `DialogueTrigger.cs`, `DialogueBubbleChat.cs`, `DialogueUI.cs` – Localized dialogue, chat bubbles, stylized boxes.
* `DialTrigger.cs` – Trigger and auto-start dialogue via collider.

### 🎨 UI & HUD

* `InventoryGUI.cs`, `ItemDatabase.cs`, `ItemData.cs` – Visual item/equipment system.
* `ActionBarSlot.cs`, `EquipmentSlot.cs` – Equipable slot logic.
* `AOEHUD.cs`, `UIManager.cs` – Display skill/aoe info and UI management.

### 🌎 World & Environment

* `TeleportZone.cs`, `TPWorldTrigger.cs`, `TPWorldTriggerTwo.cs` – Portals & teleportation triggers.
* `WaterParallaxLayers.cs`, `ParallaxEffectWithLight.cs` – Environmental visual effects.
* `SpriteSortingByY.cs`, `PlayerDepthScaler.cs` – Dynamic Z-sorting & depth scaling.
* `FixedCameraManager.cs`, `CameraFollow2D.cs`, `CameraTraveling.cs`, `TopDownCameraTravel.cs` – Dynamic and fixed camera setups.
* `DayNightCycle.cs` – HDRP-compatible time cycle.
* `DangerZoneTrigger.cs`, `SharedMapManager.cs`, `SpawnZoneManager.cs` – Environmental triggers & multiplayer-safe zones.

### 🎮 Systems & Persistence

* `GameManager.cs` – Central hub for game logic.
* `SaveSystem.cs` – Player save/load logic (JSON/PlayerPrefs ready).
* `IntroManager.cs`, `MainMenuManager.cs`, `InGamePauseMenu.cs` – Scene & UI transitions.
* `ArtifactPickup.cs`, `InteractableObject.cs` – Object interaction logic.

---

## ✅ Setup Instructions

1. **Unity Version**: Recommended 2022.3 LTS with **Photon Fusion 2+**
2. **Install Fusion SDK** from the Photon Dashboard.
3. Drag and drop all relevant prefab components into your scene: `PlayerSpawner`, `GameManager`, `UIManager`, `FusionBootstrap`, etc.
4. Set your NetworkRunner to Shared Mode and ensure `Runner.ProvideInput` is called correctly.
5. Use `PlayerController.cs` for movement + combat or swap in `PlayerController2D.cs`.
6. To spawn players, make sure you use the correct prefab references via the `PlayerSpawner`.
7. Attach colliders and `InteractableObject.cs` to anything you want clickable or lootable.

---

## 🧪 Sample Use

### Player Setup

```csharp
public class MySpawner : MonoBehaviour {
    public GameObject playerPrefab;
    public void SpawnPlayer() {
        var runner = FindObjectOfType<NetworkRunner>();
        runner.Spawn(playerPrefab, Vector3.zero, Quaternion.identity);
    }
}
```

### Creating a Quest

```csharp
QuestData quest = new QuestData {
    questTitle = "Find the Artifact",
    questDescription = "Retrieve the lost artifact in the Mystic Forest.",
    isCompleted = false
};
QuestManager.Instance.AddQuest(quest);
```

---

## 💡 Tips & Best Practices

* Use layers/tags: "Player", "Enemy", "Interactable", "Portal".
* Keep `QuestManager` and `GameManager` in **DontDestroyOnLoad** if your game spans multiple scenes.
* Use the `DialogueTrigger` on colliders with `isTrigger = true`.
* For pixel art or 2D use: enable `SpriteSortingByY` for natural layering.
* Health, mana, and endurance values are synchronized using `[Networked]` properties.

---

## 🔧 Planned Additions

* 🎵 Sound manager
* 🧠 Skill tree / spell manager
* 🧬 Atom-based evolution system
* 🔥 Dynamic VFX (flash, dissolve, particle auras)
* 🌐 WebGL-friendly save/load layer

---

## 📚 Credits

This system is developed by **Maëlik**, a solo game developer passionate about immersive simulations and RPG storytelling.

---

## 📄 License

MIT License — Free to use and adapt for personal or commercial projects with attribution.

---

## 🚀 Social

Follow development & updates:

* 🎮 GitHub: https://github.com/FalconSnowed
* 🧠 Twitter/X: https://x.com/FalconSnowed
