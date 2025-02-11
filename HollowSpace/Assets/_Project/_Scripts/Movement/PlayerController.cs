using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerController : MonoBehaviour
{
    private static readonly int IsMoving = Animator.StringToHash("Moving");
    private static readonly int IsSneaking = Animator.StringToHash("Sneaking");
    private static readonly int IsSprinting = Animator.StringToHash("Sprinting");
    private static readonly int Vertical = Animator.StringToHash("Vertical");
    private static readonly int Horizontal = Animator.StringToHash("Horizontal");

    private enum FacingDirection
    {
        Up,
        Down,
        Left,
        Right
    }
    
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
    private FacingDirection _facingDirection;

    private void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
        _currentStamina = maxStamina;
    }

    private void Update()
    {
        HandleStamina();
    }

    private void HandleStamina()
    {
        if (_sprinting)
        {
            _currentStamina -= Time.deltaTime * staminaDepletionRate;
        }
        else
        {
            _currentStamina += Time.deltaTime * staminaRegenRate;
        }
        _currentStamina = Mathf.Clamp(_currentStamina, 0, maxStamina);
    }

    private void FixedUpdate()
    {
        // Get input
        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");
        _movement = new Vector2(horizontal, vertical).normalized;

        _sneaking = Input.GetButton("Sneak");
        _sprinting = Input.GetButton("Sprint") && _currentStamina > sprintStartStamina;
        
        // Move
        Move();
    }

    private void Move()
    {
        float currentSpeed = speed;
        if (_sneaking)
        {
            currentSpeed *= sneakSpeedMultiplier;
        }
        else if (_sprinting)
        {
            currentSpeed *= sprintSpeedMultiplier;
        }
        
        _rb.velocity = _movement * currentSpeed;
        
        // Update animator values
        if (_movement != Vector2.zero)
        {
            animator.SetFloat(Horizontal, _movement.x);
            animator.SetFloat(Vertical, _movement.y);
        }
        animator.SetBool(IsMoving, currentSpeed > 0.1);
        animator.SetBool(IsSneaking, _sneaking);
        animator.SetBool(IsSprinting, _sprinting && !_sneaking);
    }
}