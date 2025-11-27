using UnityEngine;
public class Repair : MonoBehaviour
{
    [Header("Movement and sound")]
    [SerializeField] private float speed = 1f;
    [SerializeField] private AudioClip sound;
    [SerializeField] private float volume = 1.0f;

    private void Start()
    {
        var rb = GetComponent<Rigidbody2D>();
        rb.velocity = Vector3.down * speed;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Player"))
        {
            GameManager.Instance.HealPlayer();
            if (sound != null)
            {
                GameObject sfxPlayer = GameObject.Find("SFXPlayer");
                AudioSource aud = sfxPlayer.GetComponent<AudioSource>();
                aud.PlayOneShot(sound, volume);
            }
            Destroy(this.gameObject);
        }

        if (other.gameObject.layer == LayerMask.NameToLayer("Boundry") && 
            other.gameObject.name == "BoundaryDown")
        {
            Destroy(this.gameObject);
        }
    }
}
