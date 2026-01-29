using UnityEngine;
using System.Collections;

public class Spawner : MonoBehaviour
{
    [SerializeField] private GameObject[] obstacles;

    void Start()
    {
        StartCoroutine(SpawnObstacle());
    }

    private IEnumerator SpawnObstacle()
    {
        // 1. Verificación de seguridad inicial
        if (obstacles == null || obstacles.Length == 0)
        {
            Debug.LogError("No hay obstáculos asignados al Spawner");
            yield break;
        }

        yield return new WaitUntil(() => GameManager.Instance != null && GameManager.Instance.IsGameStarted);

        while (true)
        {
            // 2. Esperar un tiempo aleatorio ANTES de spawnear
            float randomTime = Random.Range(1f, 2.5f);
            yield return new WaitForSeconds(randomTime);

            int randomIndex = Random.Range(0, obstacles.Length);
            GameObject newObstacle = Instantiate(obstacles[randomIndex], transform.position, Quaternion.identity);

            // 3. Ajustar collider con verificación de seguridad (Null Check)
            Collider2D col = newObstacle.GetComponent<Collider2D>();

            // Verificamos que col exista Y que la instancia del menú esté lista
            if (col != null && MenuSystem.Instance != null)
            {
                // Verificamos que los paneles no sean nulos antes de leer su .activeSelf
                bool menuActivo = (MenuSystem.Instance.PanelMainMenu != null && MenuSystem.Instance.PanelMainMenu.activeSelf) ||
                                 (MenuSystem.Instance.PanelDeadMenu != null && MenuSystem.Instance.PanelDeadMenu.activeSelf) ||
                                 (MenuSystem.Instance.PanelOptions != null && MenuSystem.Instance.PanelOptions.activeSelf);

                col.isTrigger = menuActivo;
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Obstacle") || collision.CompareTag("valla"))
        {
            Destroy(collision.gameObject);
        }
    }   

}