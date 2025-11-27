using UnityEngine;

public class Gun : MonoBehaviour
{
    [Header("Gun properties")]
    public Projectile projectilePrefab;
    public float timeBetweenShoots = 0.5f;
    public float missileSpeed = 10f;
    public int layerIndex;
    public AudioClip pewPewSFX;
    public float pewpewVolume = 1.0f;

    private float gunHeat = 0f;

    private AudioSource audioSource;
    private Projectile projectile;

    public void Shoot(Transform target = null)
    {

        if (gunHeat <= 0)
        {
            gunHeat += timeBetweenShoots;
            projectile = Instantiate(projectilePrefab, transform.position, transform.rotation);

            if (pewPewSFX != null)
            {
                audioSource.PlayOneShot(pewPewSFX, pewpewVolume);
            }

            projectile.setSpeed(missileSpeed);
            projectile.SetDirection(target);
            projectile.gameObject.layer = layerIndex;
        }
    }

    private void Update()
    {
        if (gunHeat > 0)
        {
            gunHeat -= Time.deltaTime;
        }
    }
    private void Start()
    {
        InitializeAudioSource();
    }

    private void InitializeAudioSource()
    {
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }
    }
}
