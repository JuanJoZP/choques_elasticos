using UnityEngine;

public class Game : MonoBehaviour
{
    public GameObject particlePrefab;
    private Vector3 mouseDownPosition;
    private bool hitObject = false;
    private bool paused = true;
    private RaycastHit2D hit;

    void Update()
    {
        // pausar con 'p'
        if (Input.GetKeyDown(KeyCode.P)) {
            paused = !paused;
        }

        // solamente se pueden manipular las particulas
        // con el juego pausado
        if (paused) {
            // spawnear particula clickeando y arrastrando
            if (Input.GetMouseButtonDown(0))
            {
                mouseDownPosition = Input.mousePosition;
                RaycastHit2D hit = Physics2D.Raycast(mouseDownPosition, Vector2.zero);
                if (hit.collider != null) {
                    hitObject = true;
                } else {
                    hitObject = false;
                }
            }

            if (Input.GetMouseButtonUp(0)) 
            {
                if (!hitObject)
                {
                    Vector2 spawnPosition = Camera.main.ScreenToWorldPoint(mouseDownPosition);
                    Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                    Vector3 mouseDelta = mousePosition - spawnPosition;

                    if (!hitObject)
                    {
                        GameObject clone = Instantiate(particlePrefab, spawnPosition, Quaternion.identity);
                        ParticleScript cloneScript = clone.GetComponent<ParticleScript>();
                        cloneScript.mass = 1;
                        cloneScript.velocity = mouseDelta;
                    }
                }
            }

            // cambiar masa con el scroll
            float scrollInput = Input.GetAxis("Mouse ScrollWheel");
            if (!Input.GetKey(KeyCode.R) && scrollInput != 0)
            {
                Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                RaycastHit2D hit = Physics2D.Raycast(mousePosition, Vector2.zero);

                if (hit.collider != null)
                {
                    ParticleScript objectScript = hit.collider.GetComponent<ParticleScript>();
                    if (objectScript != null)
                    {
                        if (scrollInput > 0) // Scroll up
                        {
                            objectScript.mass += 1f;
                        }
                        else if (scrollInput < 0) // Scroll down
                        {
                            objectScript.mass -= 1f;
                        }
                    }
                }
            }

            // cmabiar radio con scroll + r
            if (Input.GetKey(KeyCode.R) && scrollInput != 0)
            {
                Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                RaycastHit2D hit = Physics2D.Raycast(mousePosition, Vector2.zero);

                if (hit.collider != null)
                {
                    Vector3 currentScale = hit.collider.transform.localScale;
                        if (scrollInput > 0) // Scroll up
                        {
                            hit.collider.transform.localScale = currentScale * 1.1f;
                        }
                        else if (scrollInput < 0) // Scroll down
                        {
                            hit.collider.transform.localScale = currentScale * 0.9f;
                        }
                }
            }
        }
    }
}
