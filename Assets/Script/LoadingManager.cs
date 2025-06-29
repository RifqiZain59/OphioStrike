using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;
using TMPro;

/// <summary>
/// Mengelola proses loading scene secara asynchronous dengan menampilkan progress
/// pada sebuah Slider dan teks persentase menggunakan TextMeshPro.
/// </summary>
public class LoadingManager : MonoBehaviour
{
    [Header("UI Elements")]
    [Tooltip("Slider untuk menampilkan progress loading.")]
    public Slider loadingSlider;

    [Tooltip("Teks untuk menampilkan persentase loading (TMP_Text).")]
    public TMP_Text loadingPercentageText;

    [Header("Scene Settings")]
    [Tooltip("Nama scene yang akan dimuat. Pastikan nama ini sesuai dan scene telah ditambahkan ke Build Settings.")]
    public string sceneToLoad;

    [Header("Loading Behavior")]
    [Tooltip("Kecepatan interpolasi untuk memperhalus gerakan progress bar.")]
    [Range(1f, 20f)]
    public float lerpSpeed = 10f;

    [Tooltip("Waktu tunda (detik) setelah loading mencapai 100% sebelum scene aktif.")]
    public float activationDelay = 0.5f;

    void Start()
    {
        // Validasi awal untuk memastikan semua referensi telah diatur di Inspector.
        // Ini adalah langkah PENTING untuk menghindari NullReferenceException.
        if (loadingSlider == null)
        {
            Debug.LogError("ERROR: Loading Slider belum di-assign di Inspector!");
            this.enabled = false; // Nonaktifkan script ini jika setup tidak valid.
            return;
        }

        if (loadingPercentageText == null)
        {
            Debug.LogError("ERROR: Loading Percentage Text belum di-assign di Inspector!");
            this.enabled = false; // Nonaktifkan script ini jika setup tidak valid.
            return;
        }

        if (string.IsNullOrEmpty(sceneToLoad))
        {
            Debug.LogError("ERROR: Nama Scene To Load belum diisi di Inspector!");
            this.enabled = false; // Nonaktifkan script ini jika setup tidak valid.
            return;
        }

        // Memulai proses loading
        StartCoroutine(LoadSceneAsync());
    }

    /// <summary>
    /// Coroutine untuk memuat scene secara asynchronous dan memperbarui UI.
    /// </summary>
    private IEnumerator LoadSceneAsync()
    {
        // Inisialisasi UI
        loadingSlider.value = 0f;
        loadingPercentageText.text = "0%";

        // Memulai operasi pemuatan scene
        AsyncOperation operation = SceneManager.LoadSceneAsync(sceneToLoad);

        // Menangani jika scene tidak dapat ditemukan di Build Settings
        if (operation == null)
        {
            Debug.LogError($"ERROR: Scene '{sceneToLoad}' tidak dapat dimuat. Periksa nama scene dan pastikan sudah ditambahkan ke Build Settings (File -> Build Settings).");
            yield break; // Hentikan coroutine
        }

        // Mencegah scene aktif secara otomatis setelah selesai dimuat
        operation.allowSceneActivation = false;

        float currentProgress = 0f;

        // Loop berjalan selama proses loading (operation.progress berhenti di 0.9)
        while (operation.progress < 0.9f)
        {
            // Menghitung progress target (0 sampai 1)
            float targetProgress = Mathf.Clamp01(operation.progress / 0.9f);

            // Interpolasi nilai progress untuk gerakan yang lebih halus
            currentProgress = Mathf.Lerp(currentProgress, targetProgress, lerpSpeed * Time.deltaTime);
            
            loadingSlider.value = currentProgress;
            loadingPercentageText.text = Mathf.RoundToInt(currentProgress * 100f) + "%";

            yield return null; // Tunggu frame berikutnya
        }

        // ---- Proses Loading Selesai (Mencapai 90%) ----

        // Animasikan sisa progress ke 100% secara visual
        float finalProgressTarget = 1f;
        while (currentProgress < finalProgressTarget - 0.01f) // Loop sampai sangat dekat dengan 100%
        {
            currentProgress = Mathf.Lerp(currentProgress, finalProgressTarget, lerpSpeed * Time.deltaTime);
            loadingSlider.value = currentProgress;
            loadingPercentageText.text = Mathf.RoundToInt(currentProgress * 100f) + "%";
            yield return null;
        }

        // Pastikan UI menunjukkan 100%
        loadingSlider.value = 1f;
        loadingPercentageText.text = "100%";
        
        Debug.Log("Loading Selesai. Menunggu aktivasi scene...");

        // Tunggu sejenak sesuai delay yang ditentukan
        yield return new WaitForSeconds(activationDelay);

        // Aktifkan scene yang sudah dimuat
        Debug.Log("Mengaktifkan Scene!");
        operation.allowSceneActivation = true;
    }
}
