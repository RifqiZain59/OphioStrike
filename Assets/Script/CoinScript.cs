using UnityEngine; // Namespace dasar Unity

public class CoinScript : MonoBehaviour
{
    public int coinValue = 1; // Nilai koin yang akan ditambahkan ke pemain

    [Header("Rotation Settings")] // Pengaturan untuk animasi putaran koin
    // Kita hanya perlu satu kecepatan rotasi untuk putaran terus menerus
    public float rotationSpeed = 200f; // Kecepatan putaran koin (derajat per detik) - Akan digunakan untuk sumbu Y

    [Header("Effects Settings")] // Pengaturan untuk efek saat koin diambil
    public GameObject pickupEffectPrefab; // Prefab efek partikel saat koin diambil
    public AudioClip coinPickupSound; // Audio Clip efek suara saat koin diambil

    // Tidak perlu lagi initialZRotation dan initialPosition jika hanya putaran Y
    // private float initialZRotation; 
    // private Vector3 initialPosition; 

    // Fungsi Start tidak perlu lagi jika tidak ada initial position/rotation
    // private void Start()
    // {
    //     initialZRotation = transform.eulerAngles.z;
    //     initialPosition = transform.position;
    // }

    // Fungsi Update dipanggil setiap frame
    private void Update()
    {
        // === Putaran Terus Menerus (Sumbu Y) ===
        // Memutar objek secara terus menerus di sumbu Y (vertikal)
        // Kita menggunakan rotationSpeed yang baru di atas
        transform.Rotate(0f, rotationSpeed * Time.deltaTime, 0f, Space.Self);

        // === Kode Rotasi Depan-Belakang (Sumbu Z) DIHAPUS ===
        // float newZRotation = initialZRotation + Mathf.Sin(Time.time * zRotationSpeed * Mathf.Deg2Rad) * zRotationRange;
        // transform.rotation = Quaternion.Euler(transform.eulerAngles.x, transform.eulerAngles.y, newZRotation);

        // === Kode Gerak Melayang Kanan Kiri (Sumbu X) DIHAPUS ===
        // float newXPosition = initialPosition.x + Mathf.Sin(Time.time * hoverSpeed) * hoverDistance;
        // transform.position = new Vector3(newXPosition, initialPosition.y, initialPosition.z);
    }

    // Fungsi ini dipanggil ketika collider lain masuk ke dalam trigger koin
    private void OnTriggerEnter2D(Collider2D other)
    {
        // Memeriksa apakah objek yang menyentuh memiliki tag "Player"
        if (other.CompareTag("Player"))
        {
            // Mencoba mendapatkan komponen PlayerMovement dari objek yang menyentuh
            PlayerMovement playerMovement = other.GetComponent<PlayerMovement>();

            // Jika script PlayerMovement ditemukan pada objek pemain
            if (playerMovement != null)
            {
                // Memanggil fungsi AddCoins pada script pemain untuk menambahkan koin
                playerMovement.AddCoins(coinValue);
            }

            // === Efek Saat Koin Diambil (Partikel dan Suara) ===

            // Memainkan efek partikel jika prefabnya sudah diatur
            if (pickupEffectPrefab != null)
            {
                Instantiate(pickupEffectPrefab, transform.position, Quaternion.identity);
            }

            // Memainkan efek suara jika audio clipnya sudah diatur
            if (coinPickupSound != null)
            {
                AudioSource.PlayClipAtPoint(coinPickupSound, transform.position);
            }

            // Menghancurkan objek koin setelah diambil
            Destroy(gameObject);
        }
    }
}