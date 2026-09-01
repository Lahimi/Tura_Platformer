using UnityEngine;

public class Shooter : MonoBehaviour
{
    public GameObject bulletPrefab;
    public Transform firePoint;
    private SpriteRenderer spriteRenderer;

    void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    public void Fire()
    {
        if (SoundManager.instance != null)
            SoundManager.instance.PlaySound(SoundManager.instance.playerShoot);

        GameObject newBullet = Instantiate(bulletPrefab, firePoint.position, Quaternion.identity);
        float playerDir = spriteRenderer.flipX ? -1f : 1f;
        newBullet.GetComponent<Bullet>().SetDirection(playerDir);
    }
}