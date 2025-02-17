using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerController : MonoBehaviour {
    private static readonly int IsMovingHorizontal = Animator.StringToHash("MovingHorizontal");
    private static readonly int IsMovingVertical = Animator.StringToHash("MovingVertical");
    private static readonly int IsSneaking = Animator.StringToHash("Sneaking");
    private static readonly int IsSprinting = Animator.StringToHash("Sprinting");
    private static readonly int Vertical = Animator.StringToHash("Vertical");
    private static readonly int Horizontal = Animator.StringToHash("Horizontal");

    [Header("References")] [SerializeField]
    private PlayerHUD hud;

    [Header("Movement")] [SerializeField] private float speed = 5f;

    [SerializeField] private float sprintSpeedMultiplier = 1.5f;
    [SerializeField] private float sneakSpeedMultiplier = 0.75f;

    [Header("Stamina")] [SerializeField] private float maxStamina = 100f;
    [SerializeField] private float staminaRegenRate = 10f;
    [SerializeField] private float staminaDepletionRate = 10f;

    [SerializeField, Tooltip("The minimum stamina to initialize a sprint")]
    private float sprintStartStamina = 10f;

    [Header("Animation")] [SerializeField] private Animator animator;

    private Rigidbody2D _rb;
    private float _currentStamina;
    private Vector2 _movement;
    private bool _sprinting;
    private bool _sneaking;
    private bool _sprintRecharge;
    private float _speedModifier = 1f;

    private void Awake() {
        _rb = GetComponent<Rigidbody2D>();
        _currentStamina = maxStamina;
    }

    private void Update() {
        // Get input
        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");
        _movement = new Vector2(horizontal, vertical).normalized;

        _sneaking = Input.GetButton("Sneak");
        HandleSprinting();
        HandleStaminaDepletion();
    }
    private void FixedUpdate() {
        Move();
    }

    private void HandleStaminaDepletion() {
        if (_sprinting) {
            _currentStamina -= Time.deltaTime * staminaDepletionRate;
        } else {
            _currentStamina += Time.deltaTime * staminaRegenRate;
        }

        _currentStamina = Mathf.Clamp(_currentStamina, 0, maxStamina);
        hud.UpdateStamina(_currentStamina, maxStamina);
    }


    private void HandleSprinting() {
        bool sprintButtonPressed = Input.GetButton("Sprint");
        
        if (_currentStamina >= maxStamina) _sprintRecharge = false;
        if (Input.GetButtonDown("Sprint") && _currentStamina >= sprintStartStamina) _sprintRecharge = false;
        if (_currentStamina <= sprintStartStamina) _sprintRecharge = true;
        if (_currentStamina <= 0) _sprinting = false;
        
        bool canStartSprinting = !_sprintRecharge && _currentStamina >= sprintStartStamina;
        bool canContinueSprinting = _sprinting && _currentStamina > 0;

        _sprinting = (canStartSprinting || canContinueSprinting) && sprintButtonPressed && _movement != Vector2.zero;
    }

    private void Move() {
        float currentSpeed = speed * _speedModifier;
        if (_sneaking) {
            currentSpeed *= sneakSpeedMultiplier;
        }
        else if (_sprinting) {
            currentSpeed *= sprintSpeedMultiplier;
        }

        _rb.velocity = _movement * currentSpeed;

        // Update animator values
        animator.SetFloat(Horizontal, _movement.x);
        animator.SetFloat(Vertical, _movement.y);
        animator.SetBool(IsMovingVertical, _movement.y != 0);
        animator.SetBool(IsMovingHorizontal, _movement.x != 0);
        animator.SetBool(IsSneaking, _sneaking);
        animator.SetBool(IsSprinting, _sprinting && !_sneaking);
    }
    
    public void AdjustSpeed(float modifier) {
        _speedModifier = modifier;
    }

    public void ResetSpeed() {
        _speedModifier = 1f;
    }
}