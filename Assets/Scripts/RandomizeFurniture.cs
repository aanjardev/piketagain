using UnityEngine;

public class RandomizeFurniture : MonoBehaviour
{
    [Header("Pengaturan Berantakan")]
    public float maxPositionOffset = 0.5f; 
    public float maxRotationAngle = 20f; 
    
    [Tooltip("Pilih layer objek yang gak boleh ditabrak (misal: layer Furniture)")]
    public LayerMask obstacleLayer; 

    private Vector3 originalPosition;
    private Quaternion originalRotation;
    private Collider myCollider; // Ubah dari BoxCollider jadi Collider umum

    void Start()
    {
        originalPosition = transform.position;
        originalRotation = transform.rotation;
        
        // Mengambil MeshCollider atau jenis collider apa pun yang menempel
        myCollider = GetComponent<Collider>(); 

        MakeItMessy();
    }

    void MakeItMessy()
    {
        int maxAttempts = 10; 
        bool positionFound = false;

        for (int i = 0; i < maxAttempts; i++)
        {
            float randomX = Random.Range(-maxPositionOffset, maxPositionOffset);
            float randomZ = Random.Range(-maxPositionOffset, maxPositionOffset);
            Vector3 testPosition = originalPosition + new Vector3(randomX, 0, randomZ);

            float randomRotY = Random.Range(-maxRotationAngle, maxRotationAngle);
            Quaternion testRotation = originalRotation * Quaternion.Euler(0, randomRotY, 0);

            if (myCollider != null)
            {
                myCollider.enabled = false;

                // Hitung offset titik tengah agar sesuai dengan posisi test yang baru
                // (Penting karena pivot 3D model kadang ada di bawah, bukan di tengah)
                Vector3 centerOffset = myCollider.bounds.center - transform.position;
                Vector3 checkCenter = testPosition + centerOffset;

                // myCollider.bounds.extents adalah setengah dari dimensi ukuran mesh
                bool isColliding = Physics.CheckBox(
                    checkCenter,
                    myCollider.bounds.extents, 
                    testRotation,
                    obstacleLayer
                );

                myCollider.enabled = true;

                if (!isColliding) 
                {
                    transform.position = testPosition;
                    transform.rotation = testRotation;
                    positionFound = true;
                    break; 
                }
            }
            else
            {
                transform.position = testPosition;
                transform.rotation = testRotation;
                positionFound = true;
                break;
            }
        }

        if (!positionFound)
        {
            float randomRotY = Random.Range(-maxRotationAngle, maxRotationAngle);
            transform.rotation = originalRotation * Quaternion.Euler(0, randomRotY, 0);
        }
    }

    public void Luruskan() 
    {
        transform.position = originalPosition;
        transform.rotation = originalRotation;
    }
}