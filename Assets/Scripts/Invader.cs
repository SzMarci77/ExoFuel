using System;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(BoxCollider2D))]

public class Invader : MonoBehaviour
{

    [Header("Animation")]
    public Sprite[] animationSprites = new Sprite[0];
    public float animationTime = 1f;

    [Header("InvaderStats")]
    public int score = 10;
    public int health = 1;

    private SpriteRenderer spriteRenderer;
    private int animationFrame;

    [Header("RotateAndShoot")]
    private GameObject player;
    public bool autoRotate;
    public bool autoShoot;
    public bool autoAim;
    public bool shootingEnabled = true;

    [Header("Guns")]
    public List<Gun> guns = new List<Gun>();
    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }
    private void Start()
    {
        if (animationSprites.Length > 0)
        {
            spriteRenderer.sprite = animationSprites[0];
            InvokeRepeating(nameof(AnimateSprite), animationTime, animationTime);
        }
        foreach (Gun gun in guns)
        {
            gun.layerIndex = LayerMask.NameToLayer("InvaderMissile");
        }
        player = GameObject.FindGameObjectWithTag("Player");
    }
    private void Update()
    {
        if (player == null)
        {
            return;
        }

        if (autoRotate) RotateTo(player.transform.position);
        if (autoShoot) ShootTo(player.transform);
    }

    public void RotateTo(Vector3 target)
    {
        float offset = 90f;
        Vector2 direction = target - transform.position;
        direction.Normalize();
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(Vector3.forward * (angle + offset));
    }

    private void ShootTo(Transform target)
    {

        if (!shootingEnabled) 
        {
            return; 
        }

        foreach (Gun gun in guns)
        {
            if (autoAim)
                gun.Shoot(target);
            else
                gun.Shoot();
        }
    }
    private void AnimateSprite()
    {
        animationFrame++;

        if (animationFrame >= animationSprites.Length)
        {
            animationFrame = 0;
        }

        spriteRenderer.sprite = animationSprites[animationFrame];
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("PlayerMissile"))
        {
            GameManager.Instance.OnInvaderKilled(this);
        }
    }
}
