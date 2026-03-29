# FPS Controller

A state machine-based first person controller for Unity 6+. Supports walk, sprint, crouch, head bob, smooth camera transitions, and FOV shift.

No singletons, no Find calls, no statics. Everything is wired through the Inspector.

## Requirements

- Unity 6+
- Input System package

## Installation

Package Manager > + > Add package from git URL > paste the repo URL.

## Setup

### Create the data assets

Right click in Project > Create > FPSController:
- **State Data** x4 (one per state: Idle, Walk, Sprint, Crouch)
- **Settings** x1

Recommended values:

| Asset | MoveSpeed | ColliderHeight | CameraHeight | BobFrequency | BobAmplitude |
|---|---|---|---|---|---|
| Idle | 0 | 2 | 1.6 | 0 | 0 |
| Walk | 5 | 2 | 1.6 | 10 | 0.01 |
| Sprint | 10 | 2 | 1.6 | 16 | 0.02 |
| Crouch | 3 | 1.2 | 0.8 | 6 | 0.005 |

Settings: SprintFOVMultiplier = 0.17

### Scene hierarchy

```
Player (Layer: Player)
├── CameraHolder (position: 0, 1.6, 0)
│   └── Main Camera (position: 0, 0, 0)
└── PlayerMesh (optional, position: 0, 0, 0)
```

The mesh (capsule, character model, etc.) goes as a direct child of Player, not under CameraHolder — otherwise it rotates when looking up/down. Unity's default capsule (2m tall, pivot at center) aligns with the CharacterController out of the box. For custom meshes, make sure the feet sit at Y=0. In first person you won't see it, but it's useful for shadows and debug.

### Components on the Player GameObject

- **CharacterController** (height: 2, center: 0/1/0)
- **PlayerInputHandler** — assign your 4 InputActionReferences (Move, Look, Sprint, Crouch)
- **PlayerMotor** — assign CharacterController, Camera, CameraHolder, CameraHeadBob, Settings
- **PlayerLook** — assign PlayerInputHandler, CameraHolder
- **PlayerStateMachine** — assign PlayerMotor, PlayerInputHandler, and the 4 State Data assets
- **CameraHeadBob** — assign CharacterController, PlayerStateMachine

### Input

The package uses `InputActionReference` fields, so it works with any InputAction asset. Create your own or use Unity's default template, then drag the actions into PlayerInputHandler:
- Move (Vector2)
- Look (Vector2)
- Sprint (Button)
- Crouch (Button)

## Hooking into state changes

The state machine exposes a C# Action you can subscribe to from your game code:

```csharp
[SerializeField] private FPSController.PlayerStateMachine _StateMachine;

void OnEnable() => _StateMachine.OnStateChanged += OnPlayerStateChanged;
void OnDisable() => _StateMachine.OnStateChanged -= OnPlayerStateChanged;

void OnPlayerStateChanged(FPSController.PlayerStateBase p_state)
{
    // read state data, trigger game-specific logic, etc.
    float speed = p_state.Data.MoveSpeed;
}
```

## Adding game-specific data

`PlayerStateDataSO` only contains universal fields (movement, camera). For game-specific data (noise radius, stamina cost, etc.), create your own SO and map it to each state in your project.

## Architecture overview

```
PlayerInputHandler    reads inputs, exposes properties
PlayerStateMachine    owns 4 states, fires OnStateChanged
PlayerStateBase       abstract class, holds state data SO
  ├─ PlayerIdleState
  ├─ PlayerWalkState
  ├─ PlayerSprintState
  └─ PlayerCrouchState
PlayerMotor           CharacterController wrapper, gravity, camera/FOV lerp
PlayerLook            mouse rotation, vertical clamp
CameraHeadBob         sin-based vertical offset, smooth fade on stop
```