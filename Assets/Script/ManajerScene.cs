using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Script Manajer Scene dengan fungsi terpisah untuk setiap tujuan scene.
/// Disesuaikan untuk alur: Menu Utama -> Pilih Level -> Level Permainan.
/// </summary>
public class ManajerScene : MonoBehaviour
{
    // --- Fungsi Navigasi Utama ---

    /// <summary>
    /// Fungsi untuk kembali ke Menu Utama.
    /// Hubungkan ini ke tombol "Back to Menu" atau "Home".
    /// </summary>
    public void PindahKeMainMenu()
    {
        Debug.Log("Perintah untuk pindah ke scene: Main Menu");
        SceneManager.LoadScene("Main Menu");
    }

    /// <summary>
    /// Fungsi untuk pindah ke halaman Pilih Level.
    /// Hubungkan ini ke tombol "Play" atau "Start" dari Menu Utama.
    /// </summary>
    public void PindahKePilihLevel()
    {
        Debug.Log("Perintah untuk pindah ke scene: Pilih_Level");
        SceneManager.LoadScene("Pilih_Level");
    }


    // --- Fungsi untuk Memulai Level ---

    /// <summary>
    /// Fungsi untuk memulai Level 1.
    /// Hubungkan ini ke tombol "Level 1" di dalam scene "PilihLevel".
    /// </summary>
    public void PindahKeLevelSatu()
    {
        Debug.Log("Perintah untuk pindah ke scene: Level_Satu");
        SceneManager.LoadScene("Level_Satu");
    }

    /// <summary>
    /// Fungsi untuk memulai Level 2.
    /// Hubungkan ini ke tombol "Level 2" di dalam scene "PilihLevel".
    /// </summary>
    public void PindahKeLevelDua()
    {
        Debug.Log("Perintah untuk pindah ke scene: Level_Dua");
        SceneManager.LoadScene("Level_Dua");
    }

    // --- Fungsi Utilitas ---

    /// <summary>
    /// Fungsi untuk keluar dari aplikasi.
    /// </summary>
    public void KeluarAplikasi()
    {
        Debug.Log("Perintah untuk keluar dari aplikasi...");
        Application.Quit();
    }
}