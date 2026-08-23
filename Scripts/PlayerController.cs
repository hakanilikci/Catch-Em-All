using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;

public class PlayerController : MonoBehaviour
{
    private Rigidbody rb;
    private int count;
    private float movementX;
    private float movementY;

    [Header("Player Settings")]
    public float speed = 0;
    
    [Header("UI References")]
    public TextMeshProUGUI countText;
    public GameObject winTextObject;
    
    [Header("Audio")]
    public AudioClip collectSound;
    public AudioClip collisionSound;
    public AudioClip loseSound;
    public AudioClip winSound;
    public AudioClip hitSound;
    public AudioSource bgMusic; 
    private AudioSource audioSource;
    
    private float startTime;
    private bool isGameActive;

    void Start()
    {
        // Get Rigidbody and AudioSource components
        rb = GetComponent<Rigidbody>();
        audioSource = GetComponent<AudioSource>(); 
        
        // Initialize score and game state
        count = 0;
        SetCountText();
        winTextObject.SetActive(false);
        
        startTime = Time.time;
        isGameActive = true;
    }

    void OnMove(InputValue movementValue)
    {
        // Input system callback for movement
        if (!isGameActive) return;
        Vector2 movementVector = movementValue.Get<Vector2>();
        movementX = movementVector.x;
        movementY = movementVector.y;
    }

    void SetCountText()
    {
        countText.text = "Pokemons  " + count.ToString();
    }

    private bool canTakeDamage = true;
    private void OnCollisionEnter(Collision collision)
    {
        // Play collision sound if hitting something hard enough
        if (collision.relativeVelocity.magnitude > 2.0f && audioSource != null && collisionSound != null)
        {
            audioSource.PlayOneShot(collisionSound);
        }

        // Check for collision with enemies
        if (collision.gameObject.CompareTag("Enemy"))
        {
            EnemyHit();
        }
    }

    public void EnemyHit()
    {
        // Handle player getting hit by an enemy
        if (!canTakeDamage || !isGameActive) return;
        
        // Decrease score
        count = count - 1;
        SetCountText();
        
        // Check for Game Over condition
        if (count <= 0)
        {
            GameOver("Snorlax Got You!\nTry Again!", hitSound);
        }
        else
        {
            // Play hit sound (pain sound)
            if (audioSource != null && hitSound != null)
            {
                audioSource.PlayOneShot(hitSound);
            }
            StartCoroutine(DamageCooldown());
        }
    }

    IEnumerator DamageCooldown()
    {
        canTakeDamage = false;
        yield return new WaitForSeconds(2.0f);
        canTakeDamage = true;
    }

    public void GameOver(string message, AudioClip specificSound = null)
    {
        if (this == null || !isGameActive) return;
        
        isGameActive = false;
        winTextObject.gameObject.SetActive(true);
        winTextObject.GetComponent<TextMeshProUGUI>().text = message;

        if (bgMusic != null) bgMusic.Stop();  //stops the music when UI is visible after game is over.

        AudioClip clipToPlay = (specificSound != null) ? specificSound : loseSound;

        if (audioSource != null && clipToPlay != null)
        {
            audioSource.PlayOneShot(clipToPlay);
        }

        GetComponent<MeshRenderer>().enabled = false;  /* makes the character invisible*/
        GetComponent<Collider>().enabled = false;
        rb.isKinematic = true;
    }

    private void FixedUpdate()
    {
        if (!isGameActive) return;

        // Apply forces to brake slowly
        if (Keyboard.current != null && Keyboard.current.spaceKey.isPressed)
        {
            // Brake/Stop if space is pressed
            rb.linearVelocity = Vector3.Lerp(rb.linearVelocity, Vector3.zero, 0.1f);
        }
        else
        //create a movement in x and y axis
        {
           
            Vector3 movement = new Vector3(movementX, 0.0f, movementY);
            rb.AddForce(movement * speed);
        }
    }

    private void Update()
    {
        // Check if player fell off the map
        if (transform.position.y < -10.0f)
        {
            GameOver("You Lost!\nTry Again!");
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        // Collect Pokemon
        if (other.gameObject.CompareTag("Pokemon"))
        {
            other.gameObject.SetActive(false);
            count = count + 1;
            SetCountText();
            
            if (audioSource != null && collectSound != null)
            {
                audioSource.PlayOneShot(collectSound);
            }
        }
        // Fall into water
        else if (other.gameObject.CompareTag("Water"))
        {
            GameOver("You Lost!\nTry Again!");
        }
        // Check win condition at the Arch
        else if (other.gameObject.CompareTag("Arch"))
        {
            if (count >= 3)
            {
                // Calculate and display finish time
                float finishTime = Time.time - startTime;
                string timeStr = finishTime.ToString("F2");
                
                winTextObject.SetActive(true);
                winTextObject.GetComponent<TextMeshProUGUI>().text = "You Catch'em ALL\nTime: " + timeStr + "s";
                
                if (bgMusic != null) bgMusic.Stop(); // Stop background music

                if (audioSource != null && winSound != null)
                {
                    audioSource.PlayOneShot(winSound);
                }

                // Remove enemy and stop game
                Destroy(GameObject.FindGameObjectWithTag("Enemy"));
                isGameActive = false;
                
                Time.timeScale = 0;
                
                GetComponent<MeshRenderer>().enabled = false;
                GetComponent<Collider>().enabled = false;
            }
        }
    }
}