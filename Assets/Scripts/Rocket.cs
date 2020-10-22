using UnityEngine;
using UnityEngine.SceneManagement;

public class Rocket : MonoBehaviour
{
    [SerializeField] private float rcsThrust = 100f;
    [SerializeField] private float mainThrust = 100f;
    [SerializeField] private AudioClip mainEngineSound;
    [SerializeField] private AudioClip deathSound;
    [SerializeField] private AudioClip successSound;

    //cache reference
    private Rigidbody rigidBody;
    private AudioSource audioSource;

    private enum State
    { Alive, Dying, Transcending }

    private State state = State.Alive;

    private void Start()
    {
        rigidBody = GetComponent<Rigidbody>();
        audioSource = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    private void Update()
    {
        // todo somewhere stop sound on death
        if (state == State.Alive)
        {
            RespondToThrustInput();
            RespondToRotateInput();
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (state != State.Alive) { return; } // ignore collisons
        switch (collision.gameObject.tag)
        {
            case "Friendly":
                print("ok");
                break;

            case "Finish":
                StartSuccessSequence();
                break;

            default:
                StartDeathSequence();
                break;
        }
    }

    private void StartSuccessSequence()
    {
        state = State.Transcending;
        audioSource.Stop();
        audioSource.PlayOneShot(successSound);
        Invoke("LoadNextScene", 1f); // parameterize time
    }

    private void StartDeathSequence()
    {
        print("Hit something deadly");
        state = State.Dying;
        audioSource.Stop();
        audioSource.PlayOneShot(deathSound);
        Invoke("LoadFirstLevel", 1f);
        // kill the player
    }

    private void RespondToThrustInput()
    {
        if (Input.GetKey(KeyCode.Space)) // can thrust while rotating
        {
            ApplyThrust();
        }
        else
        {
            audioSource.Stop();
        }
    }

    private void ApplyThrust()
    {
        rigidBody.AddRelativeForce(Vector3.up * mainThrust * Time.deltaTime);
        if (!audioSource.isPlaying) // so it doesn't layer
        {
            audioSource.PlayOneShot(mainEngineSound);
        }
    }

    private void RespondToRotateInput()
    {
        float rotationThisFrame = rcsThrust * Time.deltaTime;
        rigidBody.freezeRotation = true;// take manual control of rotation
        if (Input.GetKey(KeyCode.A))
        {
            transform.Rotate(Vector3.forward * rotationThisFrame);
        }
        else if (Input.GetKey(KeyCode.D))
        {
            transform.Rotate(-Vector3.forward * rotationThisFrame);
        }
        rigidBody.freezeRotation = false; // resume physics rotation
    }

    private void LoadFirstLevel()
    {
        SceneManager.LoadScene(0);
    }

    private void LoadNextScene()
    {
        SceneManager.LoadScene(1); // todo allow for more than 2 levels
    }
}