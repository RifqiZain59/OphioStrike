using UnityEngine;
using UnityEngine.UI; // Pastikan ini ada untuk Slider
using UnityEngine.SceneManagement;
using System.Collections;
using TMPro; // Tambahkan ini untuk menggunakan Text Mesh Pro

public class LoadingManager : MonoBehaviour
{
    // Ubah dari Image menjadi Slider
    public Slider loadingSlider; 
    // Tambahkan referensi untuk Text Mesh Pro
    public TMP_Text loadingPercentageText; 

    public string sceneToLoad = "GameScene"; 

    // Opsional: Waktu penundaan setelah loading 100% sebelum scene aktif
    public float activationDelay = 1.0f; // Contoh: 1 detik

    // Kecepatan interpolasi untuk membuat gerakan bilah lebih halus
    // Sesuaikan nilai ini di Inspector untuk efek yang diinginkan (misal: 5f, 10f)
    public float lerpSpeed = 5f; 

    void Start()
    {
        // Pastikan slider dan teks disembunyikan di awal jika belum diatur
        if (loadingSlider != null)
        {
            loadingSlider.gameObject.SetActive(false);
        }
        if (loadingPercentageText != null)
        {
            loadingPercentageText.gameObject.SetActive(false);
        }
        
        Debug.Log("LoadingManager: Starting load process."); // Debugging
        
        // Aktifkan slider dan teks, serta atur nilainya ke 0 saat memulai
        if (loadingSlider != null)
        {
            loadingSlider.gameObject.SetActive(true);
            loadingSlider.value = 0f; // PASTIKAN DIMULAI DARI 0
            Debug.Log("LoadingManager: loadingSlider activated and set to 0 value."); // Debugging
        }
        else
        {
            Debug.LogError("LoadingManager: loadingSlider is NOT assigned in the Inspector!"); // Debugging
        }

        if (loadingPercentageText != null)
        {
            loadingPercentageText.gameObject.SetActive(true);
            loadingPercentageText.text = "0%"; // Atur teks awal
        }
        else
        {
            Debug.LogError("LoadingManager: loadingPercentageText is NOT assigned in the Inspector!"); // Debugging
        }

        StartCoroutine(LoadSceneAsync());
    }

    IEnumerator LoadSceneAsync()
    {
        // Memulai operasi pemuatan scene secara asynchronous
        AsyncOperation operation = SceneManager.LoadSceneAsync(sceneToLoad);
        operation.allowSceneActivation = false; // Mencegah scene aktif secara otomatis setelah dimuat

        // Handle jika scene tidak ditemukan
        if (operation == null)
        {
            Debug.LogError("LoadingManager: Scene '" + sceneToLoad + "' could not be loaded. Check Build Settings and scene name."); // Debugging
            yield break; // Keluar dari coroutine jika scene tidak ditemukan
        }

        // Variabel untuk melacak apakah scene siap diaktifkan dan penundaan telah dimulai
        bool readyToActivate = false;

        // Loop selama scene belum sepenuhnya dimuat dan diaktifkan (operation.isDone akan menjadi true setelah allowSceneActivation = true)
        while (!operation.isDone)
        {
            // Menghitung target progres untuk slider
            // operation.progress berjalan dari 0 hingga 0.9 saat scene dimuat.
            // Kita membaginya dengan 0.9 untuk menskalakannya ke 0-1 untuk bilah loading.
            float targetProgress = Mathf.Clamp01(operation.progress / 0.9f);

            if (loadingSlider != null)
            {
                // Lakukan interpolasi linear pada nilai slider
                loadingSlider.value = Mathf.Lerp(loadingSlider.value, targetProgress, lerpSpeed * Time.deltaTime);
            }
            else
            {
                Debug.LogError("LoadingManager: loadingSlider became null during update!"); // Debugging
            }

            // Perbarui teks persentase
            if (loadingPercentageText != null)
            {
                // Hitung persentase dari nilai slider yang sudah diinterpolasi
                int percentage = Mathf.RoundToInt(loadingSlider.value * 100f);
                loadingPercentageText.text = percentage + "%";
            }

            // Ketika scene hampir dimuat (progres mencapai 0.9)
            if (operation.progress >= 0.9f)
            {
                // Pastikan nilai slider visual mencapai 100%
                if (loadingSlider != null)
                {
                    loadingSlider.value = Mathf.Lerp(loadingSlider.value, 1f, lerpSpeed * Time.deltaTime);
                }

                // Perbarui teks persentase menjadi 100% saat slider mencapai penuh
                if (loadingPercentageText != null)
                {
                    loadingPercentageText.text = Mathf.RoundToInt(loadingSlider.value * 100f) + "%"; // Pastikan 100% ditampilkan
                }

                // Jika nilai slider sudah mendekati 100% secara visual (misal >= 99%)
                // dan kita belum memulai penundaan aktivasi
                if (loadingSlider != null && loadingSlider.value >= 0.99f && !readyToActivate)
                {
                    readyToActivate = true; // Tandai bahwa kita siap untuk mengaktifkan scene
                    Debug.Log("LoadingManager: Scene almost loaded, waiting for activation delay..."); // Debugging
                    yield return new WaitForSeconds(activationDelay); // TUNGGU DISINI SEBELUM AKTIVASI

                    operation.allowSceneActivation = true; // Aktifkan scene baru
                    Debug.Log("LoadingManager: Scene Activated!"); // Debugging
                    // Coroutine akan secara alami berakhir ketika operation.isDone menjadi true
                    // setelah scene baru diizinkan untuk aktif.
                }
            }

            yield return null; // Tunggu satu frame sebelum melanjutkan loop
        }

        // Ini adalah fallback jika loop somehow berakhir sebelum scene diaktifkan sepenuhnya.
        // Ini memastikan slider 100% dan scene diaktifkan.
        if (!operation.allowSceneActivation)
        {
            if (loadingSlider != null)
            {
                loadingSlider.value = 1f;
            }
            if (loadingPercentageText != null)
            {
                loadingPercentageText.text = "100%"; // Pastikan teks juga 100%
            }
            operation.allowSceneActivation = true;
            Debug.Log("LoadingManager: Scene activated via final fallback."); // Debugging
        }
    }
}
