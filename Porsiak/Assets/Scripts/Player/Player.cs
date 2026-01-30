using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField] private float upForce = 800f;
    [SerializeField] private Transform groundcheck;
    [SerializeField] private LayerMask ground;
    [SerializeField] private float radius;

    private Rigidbody2D Handrb;
    private Animator HandAnimator;

    void Start()
    {
        Handrb = GetComponent<Rigidbody2D>();
        HandAnimator = GetComponent<Animator>();
    }

    void Update()
    {
        // 1. Verificación de seguridad: si el tiempo es 0, no hacemos nada
        if (Time.timeScale == 0f) return;

        // 2. Usamos el Singleton de MenuSystem para evitar buscar paneles locales que den error
        if (MenuSystem.Instance != null)
        {
            // Verificamos si CUALQUIERA de los paneles está activo
            bool isMenuOpen = (MenuSystem.Instance.PanelMainMenu != null && MenuSystem.Instance.PanelMainMenu.activeSelf) ||
                              (MenuSystem.Instance.PanelOptions != null && MenuSystem.Instance.PanelOptions.activeSelf) ||
                              (MenuSystem.Instance.PanelDeadMenu != null && MenuSystem.Instance.PanelDeadMenu.activeSelf);

            if (isMenuOpen)
            {
                HandAnimator.SetBool("isIdle", true);
                return; // Detiene el salto y movimiento si hay menú
            }
        }

        HandAnimator.SetBool("isIdle", false);

        // Lógica de Suelo y Salto
        bool isGrounded = Physics2D.OverlapCircle(groundcheck.position, radius, ground);
        HandAnimator.SetBool("isGrounded", isGrounded);

        if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
        {
            Handrb.linearVelocity = new Vector2(Handrb.linearVelocity.x, 0f);
            Handrb.AddForce(Vector2.up * upForce, ForceMode2D.Impulse);

            if (GameManager.Instance != null)
            {
                GameManager.Instance.PlayJumpSound();
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        // 1. Probabilidad general (ej: 40% de que suene algo)
        if (Random.Range(0, 101) <= 40)
        {
            // 2. Si es una VACA
            if (other.gameObject.name.Contains("SensorVaca") || other.gameObject.name.Contains("SensorSalto"))
            {
                if (GameManager.Instance != null)
                {
                    GameManager.Instance.PlayObstacleSound("vaca");
                }
            }
            // 3. Si es un PÁJARO
            else if (other.gameObject.name.Contains("SensorPajaro"))
            {
                if (GameManager.Instance != null)
                {
                    GameManager.Instance.PlayObstacleSound("pajaro");
                }
            }
        }

        // Desactivamos el sensor para no repetir en el mismo salto
        if (other.gameObject.name.Contains("Sensor"))
        {
            other.gameObject.SetActive(false);
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        // 1. EL OBSTÁCULO (Vaca/Enemigo) -> MATA
        if (collision.gameObject.CompareTag("Obstacle"))
        {
            // Si hay menú, lo volvemos trigger para que no empuje
            if (MenuSystem.Instance != null &&
               (MenuSystem.Instance.PanelMainMenu.activeSelf || MenuSystem.Instance.PanelOptions.activeSelf))
            {
                collision.collider.isTrigger = true;
                return;
            }

            // Si no hay menú, muerte normal
            if (GameManager.Instance != null)
            {
                GameManager.Instance.ShowGameOverPanel();
                Time.timeScale = 0;
            }
        }

        // 2. LA VALLA -> PLATAFORMA (NO MATA)
        else if (collision.gameObject.CompareTag("valla"))
        {
            // Verificamos si el jugador está por debajo de la valla
            // Si el centro del jugador es más bajo que el de la valla, la valla se vuelve trigger
            // para que la atravieses sin chocar de frente.
            if (transform.position.y < collision.transform.position.y)
            {
                collision.collider.isTrigger = true;
            }
        }
    }

    // Para que la valla vuelva a ser sólida después de atravesarla
    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("valla"))
        {
            other.isTrigger = false;
        }
    }

    private void OnDrawGizmos()
    {
        if (groundcheck != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(groundcheck.position, radius);
        }
    }
}