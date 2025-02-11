using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class CharacterController : MonoBehaviour
{
    [Header("Movement")] [SerializeField] private float speed = 5f;

    [SerializeField] private float sprintSpeedMultiplier = 1.5f;
    [SerializeField] private float sneakSpeedMultiplier = 0.75f;

    [Header("Stamina")] [SerializeField] private float maxStamina = 100f;
    [SerializeField] private float staminaRegenRate = 10f;
    [SerializeField] private float staminaDepletionRate = 10f;
    [SerializeField, Tooltip("The minimum stamina to initialize a sprint")]
    private float sprintStartStamina = 10f;

    private Rigidbody2D _rb;
    private float _currentStamina;
    private Vector2 _movement;
    private bool _sprinting;
    private bool _sneaking;

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
        
        _sneaking = Input.GetKey(KeyCode.LeftControl);
        _sprinting = Input.GetKey(KeyCode.LeftShift) && _currentStamina > sprintStartStamina;
        
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
    }
}