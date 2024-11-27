using UnityEngine;

public class ParticleScript : MonoBehaviour
{
    private const float MAXMASS = 10;
    private bool handlingCollision = false;
    private bool paused = true;
    private Gradient gradient;
    private LineRenderer lineRenderer;

    public bool customColor = false;
    public Vector3 velocity;
    public float mass;

    void Start()
    {
        gradient = new Gradient();
        var colors = new GradientColorKey[3];
        colors[0] = new GradientColorKey(Color.green, 0.0f);
        colors[1] = new GradientColorKey(Color.yellow, 0.5f);
        colors[2] = new GradientColorKey(Color.red, 1.0f);
        var alphas = new GradientAlphaKey[2];
        alphas[0] = new GradientAlphaKey(1.0f, 0.0f);
        alphas[1] = new GradientAlphaKey(1.0f, 1.0f);
        gradient.SetKeys(colors, alphas);

        lineRenderer = gameObject.AddComponent<LineRenderer>();

        lineRenderer.startWidth = 0.1f;
        lineRenderer.endWidth = 0.1f;
        lineRenderer.material = new Material(Shader.Find("Sprites/Default"));
        lineRenderer.startColor = Color.white;
        lineRenderer.endColor = Color.white;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.P)) {
            paused = !paused;
            lineRenderer.enabled = !lineRenderer.enabled;
        }

        lineRenderer.SetPosition(0, transform.position);
        lineRenderer.SetPosition(1, transform.position + velocity);

        if (!customColor) {
            UpdateColor();
        }

        if (!paused) {
            transform.position += velocity * Time.deltaTime;
        }
    }
    
    void OnTriggerEnter2D(Collider2D collision)
    {
        if (!handlingCollision) {
            ParticleScript B = collision.gameObject.GetComponent<ParticleScript>(); 
            B.handlingCollision = true;
            
            // siguiendo proceso de: https://www.imada.sdu.dk/u/rolf/Edu/DM815/E10/2dcollisions.pdf
            float massA = mass;
            float massB = B.mass;
    
            // vectores perpendiculares (normal) y tangenciales al choque
            Vector3 normal = new Vector3(B.transform.position.x - transform.position.x, B.transform.position.y - transform.position.y, 0);
            Vector3 unitNormal = normal/Mathf.Sqrt(Mathf.Pow(normal.x, 2)+Mathf.Pow(normal.y, 2));
            Vector3 unitTangent = new Vector3(-unitNormal.y, unitNormal.x, 0);
    
            float velocityNormalA = Vector3.Dot(unitNormal, velocity);
            float velocityTangentA = Vector3.Dot(unitTangent, velocity);
            float velocityNormalB = Vector3.Dot(unitNormal, B.velocity);
            float velocityTangentB = Vector3.Dot(unitTangent, B.velocity);
    
            // la velocidad tangencial no cambia
            float newVelocityTangentA = velocityTangentA;
            float newVelocityTangentB = velocityTangentB;
    
            // la velocidad normal cambia segun las ecuaciones para choque
            // elastico en una dimension
            float newVelocityNormalA = (velocityNormalA*(massA-massB) + 2*massB*velocityNormalB) / (massA+massB);
            float newVelocityNormalB = (velocityNormalB*(massB-massA) + 2*massA*velocityNormalA) / (massA+massB);
    
            // juntar las componentes normal y tangencial para obtener la nueva velocidad
            Vector3 newVelocityA = newVelocityNormalA*unitNormal + newVelocityTangentA*unitTangent;
            Vector3 newVelocityB = newVelocityNormalB*unitNormal + newVelocityTangentB*unitTangent;

            velocity = newVelocityA;
            B.velocity = newVelocityB;
        }
    }

    void OnTriggerExit2D(Collider2D collision)
    {
       handlingCollision = false;
    }

    private void UpdateColor()
    {
        float redIntensity = mass/MAXMASS;
        GetComponent<Renderer>().material.color = gradient.Evaluate(redIntensity);
    }
}
