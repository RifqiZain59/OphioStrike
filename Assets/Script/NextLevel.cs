using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class NextLevel : MonoBehaviour
{
    // Fungsi ini akan dipanggil saat tombol ditekan (misalnya tombol untuk ke Level Dua)
    public void LoadLevelDua()
    {
        Time.timeScale = 1f; // Ensure time is running normally
        SceneManager.LoadScene("Level_Dua"); // Memuat scene dengan nama "Level_Dua"
    }

    // Fungsi baru untuk memuat Main Menu
    public void LoadMainMenu()
    {
        Time.timeScale = 1f; // Also ensure time is normal when returning to Main Menu
        SceneManager.LoadScene("Main Menu"); // Memuat scene dengan nama "Main_Menu"
    }
}