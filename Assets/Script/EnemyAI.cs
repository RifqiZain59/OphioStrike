using UnityEngine;

public class EnemyAI : MonoBehaviour
{
    [Header("Pengaturan Pergerakan")]
    public float enemySpeed = 2.0f; // Kecepatan pergerakan musuh
    public float detectionRange = 10.0f; // Jarak deteksi pemain
    public float randomMoveInterval = 3.0f; // Interval waktu untuk memilih arah random baru
    public float returnSpeed = 1.5f; // Kecepatan musuh kembali ke posisi awal

    [Header("Pengaturan Kesehatan")]
    public int maxHealth = 50; // Kesehatan maksimum musuh
    private int currentHealth; // Kesehatan musuh saat ini

    private GameObject player; // Referensi ke objek pemain
    private Vector3 initialPosition; // Posisi awal musuh
    private Vector3 randomTargetPosition; // Posisi tujuan random untuk musuh
    private float timeSinceLastRandomMove = 0.0f; // Timer untuk pergerakan random
    private bool isMovingRandomly = false; // Status apakah musuh sedang bergerak random
    private bool isReturning = false; // Status apakah musuh sedang kembali ke posisi awal
    private Rigidbody2D rb; // Untuk fisika 2D (jika game Anda 2D)
    private Collider2D enemyCollider; // Collider musuh

    void Start()
    {
        // Simpan posisi awal musuh saat game dimulai
        initialPosition = transform.position;

        player = GameObject.FindGameObjectWithTag("Player");
        if (player == null)
        {
            Debug.LogError("Objek 'Player' tidak ditemukan! Pastikan pemain Anda memiliki Tag 'Player'.");
        }

        rb = GetComponent<Rigidbody2D>();
        if (rb == null)
        {
            Debug.LogError("Objek musuh tidak memiliki komponen Rigidbody2D!");
            rb = gameObject.AddComponent<Rigidbody2D>();
            rb.gravityScale = 1f;
            rb.constraints = RigidbodyConstraints2D.FreezeRotation;
        }

        // Dapatkan collider musuh
        enemyCollider = GetComponent<Collider2D>();
        if (enemyCollider == null)
        {
            Debug.LogError("Objek musuh tidak memiliki komponen Collider2D!");
        }

        currentHealth = maxHealth; // Set kesehatan awal musuh
        ChooseNewRandomTarget(); // Inisialisasi target random awal
    }

    void Update()
    {
        if (player == null) return;

        float distanceToPlayer = Vector2.Distance(transform.position, player.transform.position);

        if (distanceToPlayer <= detectionRange)
        {
            // Musuh dekat dengan pemain: mengejar
            StopRandomMovement();
            isReturning = false; // Pastikan status kembali dinonaktifkan

            Vector2 directionToPlayer = (player.transform.position - transform.position).normalized;
            rb.velocity = new Vector2(directionToPlayer.x * enemySpeed, rb.velocity.y);

            FlipSprite(directionToPlayer.x);
        }
        else
        {
            // Musuh jauh dari pemain
            // Jika musuh belum berada di posisi awal, kembali ke sana
            if (Vector2.Distance(transform.position, initialPosition) > 0.1f) // Ambang batas kecil untuk dianggap 'sampai'
            {
                isReturning = true; // Set status kembali
                StopRandomMovement(); // Hentikan pergerakan random saat kembali

                Vector2 directionToInitial = (initialPosition - transform.position).normalized; // Arah ke posisi awal
                rb.velocity = new Vector2(directionToInitial.x * returnSpeed, rb.velocity.y); // Bergerak kembali

                FlipSprite(directionToInitial.x); // Balik sprite saat kembali
            }
            else
            {
                // Musuh sudah di posisi awal atau sangat dekat, lanjutkan dengan pergerakan random
                isReturning = false; // Reset status kembali
                HandleRandomMovement();
            }
        }
    }

    void HandleRandomMovement()
    {
        // Hanya bergerak random jika tidak sedang dalam proses kembali ke posisi awal
        if (isReturning) return;

        timeSinceLastRandomMove += Time.deltaTime;

        if (timeSinceLastRandomMove >= randomMoveInterval || !isMovingRandomly)
        {
            ChooseNewRandomTarget();
            timeSinceLastRandomMove = 0.0f;
            isMovingRandomly = true;
        }

        Vector2 currentPos = transform.position;
        Vector2 targetPosHorizontal = new Vector2(randomTargetPosition.x, currentPos.y);

        Vector2 moveDirection = (targetPosHorizontal - currentPos).normalized;
        rb.velocity = new Vector2(moveDirection.x * enemySpeed, rb.velocity.y);

        FlipSprite(moveDirection.x);
    }

    void ChooseNewRandomTarget()
    {
        float randomXOffset = Random.Range(-5f, 5f);
        // Pastikan target random juga relatif terhadap initialPosition, atau tetap relatif terhadap posisi saat ini
        // Jika ingin tetap relatif terhadap initialPosition, gunakan:
        // randomTargetPosition = new Vector3(initialPosition.x + randomXOffset, initialPosition.y, initialPosition.z);
        // Namun, jika ingin random movement di area sekitar musuh saat ini (setelah kembali), tetap seperti sebelumnya:
        randomTargetPosition = new Vector3(transform.position.x + randomXOffset, transform.position.y, transform.position.z);
    }

    void StopRandomMovement()
    {
        isMovingRandomly = false;
        if (rb != null)
        {
            rb.velocity = new Vector2(0f, rb.velocity.y);
        }
    }

    void FlipSprite(float horizontalDirection)
    {
        if (horizontalDirection > 0)
        {
            transform.localScale = new Vector3(Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
        }
        else if (horizontalDirection < 0)
        {
            transform.localScale = new Vector3(-Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
        }
    }

    // Fungsi ini dipanggil dari PlayerMovement saat pemain menginjak musuh
    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        Debug.Log(gameObject.name + " menerima " + damage + " damage.  HP: " + currentHealth);

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        Debug.Log(gameObject.name + " mati!");
        // Tambahkan efek kematian di sini (animasi, suara, partikel, dll.)

        // Nonaktifkan collider sebelum menghancurkan objek
        if (enemyCollider != null)
        {
            enemyCollider.enabled = false;
        }

        Destroy(gameObject); // Hancurkan musuh
    }
}