using UnityEngine;
using TMPro; // Tambahkan ini jika Anda menggunakan TextMeshPro

public class GameManager : MonoBehaviour
{
    // Variabel untuk menyimpan referensi ke objek Text UI Anda
    public TextMeshProUGUI scoreText; // Ubah ke public Text scoreText; jika Anda menggunakan Text biasa

    // Variabel untuk menyimpan nilai angka yang akan ditampilkan
    private int currentScore = 0; // Contoh: skor awal

    // Metode ini dipanggil saat skrip pertama kali diaktifkan
    void Start()
    {
        // Inisialisasi teks awal
        UpdateScoreDisplay();
    }

    // Metode ini akan dipanggil untuk memperbarui tampilan skor
    public void AddScore(int amount)
    {
        currentScore += amount;
        UpdateScoreDisplay();
    }

    // Metode ini akan dipanggil untuk mengatur tampilan skor secara langsung
    public void SetScore(int newScore)
    {
        currentScore = newScore;
        UpdateScoreDisplay();
    }

    // Metode internal untuk memperbarui teks di UI
    void UpdateScoreDisplay()
    {
        if (scoreText != null)
        {
            scoreText.text = "Score: " + currentScore.ToString(); // Sesuaikan format teks sesuai keinginan
        }
    }

    // Contoh bagaimana Anda bisa memanggil AddScore atau SetScore
    // Ini bisa dipanggil dari skrip lain, misalnya saat pemain mengumpulkan koin
    // void Update()
    // {
    //     // Contoh: Tekan spasi untuk menambah skor (Hanya untuk debugging)
    //     if (Input.GetKeyDown(KeyCode.Space))
    //     {
    //         AddScore(10);
    //     }
    // }
}