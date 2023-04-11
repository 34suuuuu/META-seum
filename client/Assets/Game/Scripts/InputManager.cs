using System.Collections;
using System.Collections.Generic;
using Game.Scripts;
using UnityEngine;

public class InputManager : MonoBehaviour {
  public static InputManager Instance;
  private PlayerControls playerControls;
  private NetworkManager networkManager;

  private void Awake() {
    playerControls = new PlayerControls();
    networkManager = new NetworkManager();
    if (Instance) {
      Destroy(gameObject);
      return;
    }
    DontDestroyOnLoad(gameObject);
    Instance = this;
  }

  private void OnEnable() {
    playerControls.Enable();
  }

  private void OnDisable() {
    playerControls.Disable();
  }

  public Vector2 GetPlayerMovement() {
    return playerControls.Ground.Movement.ReadValue<Vector2>();
  }

  public Vector2 GetMouseDelta() {
    return playerControls.Ground.Look.ReadValue<Vector2>();
  }

  public bool playerJumpedThisFrame() {
    return playerControls.Ground.Jump.triggered;
  }
}
