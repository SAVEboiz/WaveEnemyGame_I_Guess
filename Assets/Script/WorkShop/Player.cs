using System.Collections.Generic;
using UnityEngine;

public class Player : Character
{
    [Header("Hand setting")]
    public Transform RightHand;
    public Transform LeftHand;
    public List<Item> inventory = new List<Item>();

    Vector3 _inputDirection;
    bool _isAttacking = false;
    bool _isInteract = false;
    public bool _isMove = false;
    private bool _isDead = false;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        animator = GetComponent<Animator>();
        health = maxHealth;
    }

    public void FixedUpdate()
    {
        if (_isDead) return;

        Move(_inputDirection);
        Turn(_inputDirection);
        Attack(_isAttacking);
        Interact(_isInteract);
    }
    public void Update()
    {
        if (_isDead) return;

        HandleInput();
    }
    public void AddItem(Item item) {
        inventory.Add(item);
    }
    
    private void HandleInput()
    {
        float x = Input.GetAxis("Horizontal");
        float y = Input.GetAxis("Vertical");

        _inputDirection = new Vector3(x, 0, y);
        if (Input.GetMouseButtonDown(0)) {
            _isAttacking = true;
        }
        if (Input.GetKeyDown(KeyCode.E))
        {
            _isInteract = true;
        }

    }
    public void Attack(bool isAttacking) {
        if (isAttacking) {
            animator.SetTrigger("Attack");
            var e = InFront as Idestoryable;
            if (e != null)
            {
                e.TakeDamage(Damage);
                Debug.Log($"{gameObject.name} attacks for {Damage} damage.");
            }
            _isAttacking = false;
        }
    }
    private void Interact(bool interactable)
    {
        if (interactable)
        {
            IInteractable e = InFront as IInteractable;
            if (e != null) {
                e.Interact(this);
            }
            _isInteract = false;

        }
    }
    public override void TakeDamage(int amount)
    {
        if (_isDead) return;

        base.TakeDamage(amount);
        GameManager.instance.UpdateHealthBar(health, maxHealth);

        if (health <= 0)
        {
            Die();
        }
    }
    public override void Heal(int amount)
    {
        if (_isDead) return;

        base.Heal(amount);
        GameManager.instance.UpdateHealthBar(health, maxHealth);
    }

    public void Die()
    {
        _isDead = true;
        Debug.Log($"{gameObject.name} has died!");
        GameManager.instance.ShowGameOver();
    }

}
