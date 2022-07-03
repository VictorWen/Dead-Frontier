using UnityEngine;

public class Projectile : MonoBehaviour
{
    public struct Data {
        public bool canDamagePlayer;
        public bool canDamageMobs;
    }

    private Rigidbody2D body;
    private float speed;
    private Vector2 direction;
    private Data data;
    private float lifetime = 5;

    public void Initialize(float speed, Vector2 direction, Data data) {
        this.speed = speed;
        this.direction = direction;
        this.data = data;
    }

    // ===============
    //  UNITY METHODS
    // ===============

    private void Start() {
        body = GetComponent<Rigidbody2D>();
        
        float angle = Mathf.Atan2(direction.y, direction.x);
        angle = Mathf.Rad2Deg * angle;
        transform.eulerAngles = new Vector3(0, 0, angle);
    }

    private void FixedUpdate() {
        lifetime -= Time.fixedDeltaTime;
        if (lifetime <= 0)
            Destroy(gameObject);
        else {
            Vector2 displacement = direction * speed * Time.fixedDeltaTime;
            body.MovePosition(body.position + displacement);
        }
    }
    
    private void OnTriggerEnter2D(Collider2D other) {
        bool foundContact = false;
        if (other.name == "Player" && data.canDamagePlayer) {
            foundContact = true;
            // Do something
        }
        else if (other.CompareTag("Projectile Wall")) {
            foundContact = true;
        }
        // else if (other.name != "Player" && data.canDamageMobs) {
        //     foundContact = true;
        //     // Do something
        // }

    if (foundContact)
            Destroy(gameObject);
    }
}
