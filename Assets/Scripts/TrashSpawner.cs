using UnityEngine;

public class TrashSpawner : MonoBehaviour
{
    [Header("Pengaturan Sampah")]
    public GameObject[] trashPrefabs; 
    public int totalTrashToSpawn = 10; 

    [Header("Area Spawn")]
    public Vector2 spawnAreaSize = new Vector2(10f, 10f); 
    
    [Tooltip("Ketinggian awal 'laser' ditembakkan (misal setinggi langit-langit kelas)")]
    public float raycastHeight = 3f; 
    
    [Tooltip("Jarak ke atas dari titik jatuh agar sampah tidak tembus meja/lantai")]
    public float spawnOffset = 0.1f;

    [Header("Deteksi Permukaan")]
    [Tooltip("Pilih layer apa saja yang boleh dijatuhi sampah (misal: Default, Furniture, Floor)")]
    public LayerMask validSurfaces;

    void Start()
    {
        SpawnTrash();
    }

    void SpawnTrash()
    {
        int spawnedCount = 0;
        int maxAttempts = totalTrashToSpawn * 5; // Jaga-jaga biar gak infinite loop kalau area kepenuhan
        int attempts = 0;

        while (spawnedCount < totalTrashToSpawn && attempts < maxAttempts)
        {
            attempts++;

            // 1. Tentukan titik X dan Z acak di dalam area
            float randomX = Random.Range(-spawnAreaSize.x / 2, spawnAreaSize.x / 2);
            float randomZ = Random.Range(-spawnAreaSize.y / 2, spawnAreaSize.y / 2);
            
            // 2. Tentukan titik awal tembakan laser dari atas (Y di-set ke raycastHeight)
            Vector3 rayStartPos = new Vector3(
                transform.position.x + randomX, 
                transform.position.y + raycastHeight, 
                transform.position.z + randomZ
            );

            // 3. Tembakkan Raycast ke bawah (Vector3.down)
            RaycastHit hit;
            if (Physics.Raycast(rayStartPos, Vector3.down, out hit, raycastHeight * 2, validSurfaces))
            {
                // Kalau kena sesuatu yang masuk di validSurfaces (Meja atau Lantai)
                
                int randomIndex = Random.Range(0, trashPrefabs.Length);
                GameObject selectedTrash = trashPrefabs[randomIndex];
                Quaternion randomRotation = Quaternion.Euler(Random.Range(0, 360), Random.Range(0, 360), Random.Range(0, 360));

                // Spawn di titik tabrakan (hit.point) ditambah sedikit offset ke atas
                Vector3 finalSpawnPosition = hit.point + new Vector3(0, spawnOffset, 0);

                Instantiate(selectedTrash, finalSpawnPosition, randomRotation);
                spawnedCount++;
            }
        }
    }

    private void OnDrawGizmosSelected()
    {
        // Menggambar area spawn agar mudah dilihat dari atas
        Gizmos.color = new Color(1, 0, 0, 0.3f); 
        Gizmos.DrawCube(transform.position + new Vector3(0, raycastHeight/2, 0), new Vector3(spawnAreaSize.x, raycastHeight, spawnAreaSize.y));
    }
}