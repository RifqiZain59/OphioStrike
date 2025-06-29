using UnityEngine;

public class Finish : MonoBehaviour
{
    [SerializeField] private GameObject panelMenang;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            panelMenang.SetActive(true); // munculkan panel menang
            Time.timeScale = 0f; // pause game jika ingin
        }
    }
}