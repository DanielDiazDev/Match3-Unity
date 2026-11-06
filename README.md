# Match3

A casual 2D Match-3 puzzle game made with **Unity 6**.

Swap adjacent pieces to match three or more, complete level objectives, use power-ups, and progress through a level map with quests, a shop, and rewarded ads.

<p align="center">
  <img src="https://img.shields.io/badge/Unity-6%2B-232323?style=flat-square&logo=unity" alt="Unity version" />
  <img src="https://img.shields.io/badge/URP-Universal%20Render%20Pipeline-5a5a5a?style=flat-square" alt="URP" />
  <img src="https://img.shields.io/badge/Platform-Android%20%7C%20Standalone-green?style=flat-square" />
</p>

## 🎮 Features

- **Match-3 gameplay** — swap tiles, match 3+ pieces, cascades, and chain reactions
- **Level-based progression** — level selector with unlockable stages and persistent save data
- **Quest system** — level-specific objectives (piece destruction, score targets, etc.)
- **Power-ups & boosters** — special pieces and activatable power-ups with VFX feedback
- **In-game shop** — purchasable items and currency
- **Ads integration** — Unity Ads for rewarded and interstitial ads
- **Audio system** — background music and SFX with mute control
- **Particle VFX** — match explosions, spawns, and UI effects
- **Smooth animations** — tweens via PrimeTween, reactive UI via UniRx/UniTask
- **Pixel-art UI font** — Thaleah Pixel Font for a retro casual look
- **Editor tools** — custom level editor windows for designing levels and quests
- **Mobile-ready** — Android build settings, portrait/landscape orientation support

## 📸 Screenshots

Add your screenshots here:

```markdown
| Main Menu | Level Selector | In-Game |
|-----------|----------------|---------|
| ![](docs/screenshots/main-menu.png) | ![](docs/screenshots/level-selector.png) | ![](docs/screenshots/gameplay.png) |
```

## 🛠️ Tech Stack & Dependencies

**Engine**
- Unity 6 LTS or newer

**Third-party packages**
- [UniTask](https://github.com/Cysharp/UniTask) — async/await for Unity
- [Reflex](https://github.com/gustavopsantos/reflex) — lightweight MVVM-style framework
- [PrimeTween](https://kyrylokuzyk.itch.io/primetween) — high-performance tweening
- [Easy Text Effects](https://github.com/LeiQiaoZhi/Easy-Text-Effect) — animated UI text
- [UniRx](https://github.com/neuecc/UniRx) — reactive extensions for Unity
- Unity Ads 4.16.4

**Built-in Unity packages used**
- Input System, TextMeshPro, 2D Sprite, Tilemap, Particle System, Timeline, UI Toolkit

## 🗂️ Project Structure

```
Assets/
├── Match3/                 # Game source code & assets
│   ├── Art/                # Sprites, backgrounds, UI art
│   ├── Audio/              # Music and SFX
│   ├── Data/               # ScriptableObjects (levels, quests, shop, score, tiles)
│   ├── Prefabs/            # Reusable UI and gameplay prefabs
│   ├── Resources/          # Resources-loaded assets
│   ├── Scenes/             # MainMenu, LevelSelector, Game
│   ├── Scripts/            # C# source code
│   │   ├── Level/          # Level flow (MVP: Model, View, Presenter, UseCase)
│   │   ├── Match/          # Match-3 board logic (swap, match, gravity, refill, spawner)
│   │   ├── PowerUps/       # Power-up system
│   │   ├── Quests/         # Quest objectives
│   │   ├── UI/             # UI controllers and views
│   │   ├── Utils/          # Event dispatcher, encryption, extensions
│   │   └── ...
│   └── VFX/                # Particle effects
├── Editor/                 # Editor windows and tools
├── Plugins/                # PrimeTween, UniRx, etc.
├── Samples/                # Package samples
├── Settings/
├── TextMesh Pro/
├── Thaleah_PixelFont/
└── UIGraph/
```

## 🚀 Getting Started

### Prerequisites
- [Unity Hub](https://unity.com/download) and Unity Editor **6 LTS or newer**
- The **Universal Render Pipeline** package (included via `Packages/manifest.json`)

### Clone and open

```bash
git clone https://github.com/USERNAME/Match3.git
cd Match3
```

1. Open **Unity Hub**.
2. Click **Add** and select the `Match3` folder.
3. Open the project. Unity will import packages automatically (may take a few minutes on first launch).

### Play
- Open `Assets/Match3/Scenes/MainMenu.unity` and press **Play**.
- Scenes available: `MainMenu`, `LevelSelector`, `Game`.

### Build for Android
1. File → Build Settings → select **Android** → Switch Platform.
2. Configure Player Settings (bundle identifier, icons, etc.).
3. Click **Build** (APK/AAB).

> The `.gitignore` excludes `Library/`, `Temp/`, builds, and IDE files. Do not commit generated binaries.

## 🧩 Contributing

Contributions are welcome! To contribute:

1. Fork the repository.
2. Create a feature branch: `git checkout -b feature/your-feature`
3. Commit your changes: `git commit -m "Add some feature"`
4. Push to the branch: `git push origin feature/your-feature`
5. Open a Pull Request.

Please keep code consistent with the existing style and folder structure.


## 🙏 Credits & Assets

- **Pixel font:** [Thaleah Pixel Font](https://elthen.itch.io/thaleah-pixel-font)
- **Art assets:** Flat Match 3 Game Assets + GUI, Violet Theme UI, Simasart, and forest background assets
- **Tweening:** PrimeTween
- **Async/Rx:** UniTask, UniRx
- **Framework:** Reflex

## 📫 Contact

Questions or suggestions? Open an issue or contact the repository owner.

---

Made with ❤️ and Unity.
