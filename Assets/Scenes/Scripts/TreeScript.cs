using UnityEngine;

public class TreeScript : MonoBehaviour
{
    [Header("Налаштування Дерева")]
    public float health = 3f;
    public GameObject logPrefab; 
    public int dropAmount = 3;

    public void TakeDamage(float damage)
    {
        health -= damage;
        
        Debug.Log($"Дерево: отримано {damage} урону. Залишилось {health}");

        if (health <= 0)
        {
            ChopDown();
        }
    }

    void ChopDown()
    {
        for (int i = 0; i < dropAmount; i++)
        {
            Vector3 randomPos = transform.position + new Vector3(Random.Range(-0.5f, 0.5f), Random.Range(-0.5f, 0.5f), 0);
            Instantiate(logPrefab, randomPos, Quaternion.identity);
        }
        Destroy(gameObject);
    }
}