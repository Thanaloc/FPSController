# FPS Controller

A state machine-based first person controller for Unity 6+. Supports walk, sprint, crouch, head bob, smooth camera transitions, FOV shift, acceleration/deceleration, coyote time, jump buffer, air control, slope handling, and landing camera impact.

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

| Asset | MoveSpeed | ColliderHeight | CameraHeight | BobFrequency | BobAmplitude | Acceleration | Deceleration |
|---|---|---|---|---|---|---|---|
| Idle | 0 | 2 | 1.6 | 0 | 0 | 50 | 40 |
| Walk | 5 | 2 | 1.6 | 10 | 0.01 | 50 | 35 |
| Sprint | 10 | 2 | 1.6 | 16 | 0.02 | 40 | 25 |
| Crouch | 3 | 1.2 | 0.8 | 6 | 0.005 | 60 | 50 |

Sprint has a lower deceleration for a subtle slide that sells the speed. Crouch has high acceleration for a grounded, snappy feel.

Settings:

| Field | Default | Description |
|---|---|---|
| SprintFOVMultiplier | 0.17 | FOV increase ratio while sprinting |
| JumpForce | 7 | Vertical impulse on jump |
| CoyoteTime | 0.12 | Grace period (seconds) after leaving ground where jump is still allowed |
| JumpBufferTime | 0.1 | If jump is pressed this many seconds before landing, it fires on contact |
| AirControlMultiplier | 0.4 | Horizontal control while airborne (0 = none, 1 = full ground control) |

### Scene hierarchy

```
Player (Layer: Player)
├── CameraHolder (position: 0, 1.6, 0)
│   └── Main Camera (position: 0, 0, 0)
└── PlayerMesh (optional, position: 0, 0, 0)
```

The mesh (capsule, character model, etc.) goes as a direct child of Player, not under CameraHolder — otherwise it rotates when looking up/down. Unity's default capsule (2m tall, pivot at center) aligns with the CharacterController out of the box. For custom meshes, make sure the feet sit at Y=0.

### Components on the Player GameObject

- **CharacterController** (height: 2, center: 0/1/0)
- **PlayerInputHandler** — assign your 5 InputActionReferences (Move, Look, Sprint, Crouch, Jump)
- **PlayerMotor** — assign CharacterController, Camera, CameraHolder, CameraHeadBob, Settings, PlayerInputHandler. Optionally assign CameraLandingImpact.
- **PlayerLook** — assign PlayerInputHandler, CameraHolder
- **PlayerStateMachine** — assign PlayerMotor, PlayerInputHandler, and the 4 State Data assets
- **CameraHeadBob** — assign CharacterController, PlayerStateMachine
- **CameraLandingImpact** *(optional)* — assign PlayerMotor. Adds a camera dip on landing proportional to fall speed.

### Input

The package uses `InputActionReference` fields, so it works with any InputAction asset. Create your own or use Unity's default template, then drag the actions into PlayerInputHandler:
- Move (Vector2)
- Look (Vector2)
- Sprint (Button)
- Crouch (Button)
- Jump (Button)

## Features

### Movement smoothing

Movement uses `Vector3.MoveTowards` with per-state acceleration and deceleration rates instead of instant direction changes. This gives a progressive start and a short slide on stop — the slide is especially noticeable during sprint. Tune `Acceleration` and `Deceleration` in each State Data asset.

### Coyote time & jump buffer

Coyote time allows the player to jump for a short window after walking off a ledge. Jump buffer queues a jump press that happened slightly before landing and fires it on contact. Both are configured in the Settings asset and combine to make jumping feel responsive and forgiving.

### Air control

While airborne, horizontal input is scaled by `AirControlMultiplier`. At 0.4 (default), the player can slightly adjust their trajectory mid-air without being able to fully reverse direction.

### Slope handling

On slopes steeper than 20°, movement speed is gradually reduced. At the CharacterController's `slopeLimit` angle, speed drops to 30% of normal. This prevents full-speed climbing on steep surfaces.

### Sprint direction

Sprint only activates when the player is moving forward (stick Y > 0.5). Backpedaling or pure strafing stays at walk speed.

### Landing impact *(optional)*

`CameraLandingImpact` produces a short downward camera dip when landing, proportional to fall speed. Wire it into `PlayerMotor`'s Inspector slot to enable it. Leave the slot empty to disable.

| Field | Default | Description |
|---|---|---|
| MaxDip | 0.15 | Maximum downward offset in units |
| MaxFallSpeed | 15 | Fall speed at which max dip is reached |
| RecoverySpeed | 8 | How fast the offset returns to zero |

### Look smoothing *(optional, off by default)*

`PlayerLook` has an optional `SmoothDamp`-based smoothing that removes micro-jitter without adding perceptible input lag. Enable `_EnableSmoothing` in the Inspector and adjust `_SmoothTime` (0.02–0.05 recommended). Leave disabled for raw 1:1 input.

## Hooking into state changes

The state machine exposes a C# Action you can subscribe to from your game code. The event is guaranteed to fire once during `Start()` with the initial state, so subscribers registered in `OnEnable()` receive it without manual initialization.

```csharp
[SerializeField] private FPSController.PlayerStateMachine _StateMachine;

void OnEnable() => _StateMachine.OnStateChanged += OnPlayerStateChanged;
void OnDisable() => _StateMachine.OnStateChanged -= OnPlayerStateChanged;

void OnPlayerStateChanged(FPSController.PlayerStateBase p_state)
{
    float speed = p_state.Data.MoveSpeed;
}
```

You can also subscribe to landing events on the motor:

```csharp
[SerializeField] private FPSController.PlayerMotor _Motor;

void OnEnable() => _Motor.OnLanded += OnPlayerLanded;
void OnDisable() => _Motor.OnLanded -= OnPlayerLanded;

void OnPlayerLanded(float p_verticalVelocity)
{
    // p_verticalVelocity is negative. Heavier landing = more negative.
    float intensity = Mathf.Abs(p_verticalVelocity);
}
```

## Controlling the motor from game code

```csharp
// Disable movement (cutscene, dialogue, etc.)
_Motor.SetMovementEnabled(false);

// Disable jump
_Motor.SetJumpEnabled(false);

// Take full control of vertical movement (ladders, swimming, etc.)
_Motor.SetGravityOverride(true);
_Motor.SetVerticalVelocity(climbSpeed);

// Release control back to the motor
_Motor.SetGravityOverride(false);

// Read ground state
bool grounded = _Motor.IsGrounded;
```

## Adding game-specific data

`PlayerStateDataSO` only contains universal fields (movement, camera, smoothing). For game-specific data (noise radius, stamina cost, etc.), create your own SO and map it to each state in your project.

## Architecture overview

```
PlayerInputHandler      reads inputs, exposes properties
PlayerStateMachine      owns 4 states, fires OnStateChanged
PlayerStateBase         abstract class, holds state data SO
  ├─ PlayerIdleState
  ├─ PlayerWalkState
  ├─ PlayerSprintState
  └─ PlayerCrouchState
PlayerMotor             CharacterController wrapper, gravity, coyote/buffer,
                        accel/decel, slope, air control, camera/FOV lerp
PlayerLook              mouse rotation, vertical clamp, optional smoothing
CameraHeadBob           sin-based vertical offset, smooth fade on stop
CameraLandingImpact     (optional) camera dip on landing, proportional to fall speed
```