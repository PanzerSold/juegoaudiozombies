using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class ZombieNavmesh : MonoBehaviour
{
    [Header("Navegación")]
    [SerializeField] private float repathInterval = 0.2f;
    [SerializeField] private float waypointReachDistance = 1f;
    //----- NUEVO CODIGO ----- Velocidad de giro hacia el jugador cuando está frenado en una barricada
    [SerializeField] private float rotationSpeed = 10f;
    //----- NUEVO CODIGO -----

    private NavMeshAgent agent;
    private Transform playerTransform;

    private Vector3 waypoint;
    private bool followingWaypoint = false;
    private bool isStunned = false;
    //----- NUEVO CODIGO ----- Frenado porque está rompiendo una barricada
    private bool isHolding = false;
    //----- NUEVO CODIGO -----
    private float repathTimer = 0f;

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();

        // El Rigidbody ya no mueve al zombie: lo mueve el agente
        Rigidbody rb = GetComponent<Rigidbody>();
        if (rb != null) rb.isKinematic = true;
    }

    private void Start()
    {
        playerTransform = GameObject.FindWithTag("Player").transform;
    }

    private void Update()
    {
        if (!agent.isOnNavMesh) return;

        //----- NUEVO CODIGO ----- Si está frenado en la barricada, solo mira al jugador
        if (isHolding)
        {
            LookAtPlayer();
            return;
        }
        //----- NUEVO CODIGO -----

        if (isStunned) return;

        repathTimer -= Time.deltaTime;
        if (repathTimer > 0f) return;
        repathTimer = repathInterval;

        agent.SetDestination(GetTarget());
    }

    private Vector3 GetTarget()
    {
        if (followingWaypoint)
        {
            Vector3 flatDiff = waypoint - transform.position;
            flatDiff.y = 0f;

            if (flatDiff.magnitude < waypointReachDistance) followingWaypoint = false;
            else return waypoint;
        }

        return playerTransform.position;
    }

    //----- NUEVO CODIGO ----- Giro manual, porque con el agente frenado no rota solo
    private void LookAtPlayer()
    {
        Vector3 direction = playerTransform.position - transform.position;
        direction.y = 0f;
        if (direction.sqrMagnitude < 0.01f) return;

        Quaternion rotation = Quaternion.LookRotation(direction);
        transform.rotation = Quaternion.Slerp(transform.rotation, rotation, rotationSpeed * Time.deltaTime);
    }

    // Frena o libera al agente según esté aturdido o frenado en una barricada
    private void ApplyStop()
    {
        if (!agent.isOnNavMesh) return;

        bool stop = isStunned || isHolding;
        agent.isStopped = stop;

        if (stop) agent.velocity = Vector3.zero;
        else repathTimer = 0f;
    }
    //----- NUEVO CODIGO -----

    // Reemplaza al scriptedMovement: primero va a este punto (la barricada) y después persigue al jugador
    public void SetWaypoint(Vector3 position)
    {
        waypoint = position;
        followingWaypoint = true;
    }

    public void SetSpeed(float speed)
    {
        agent.speed = speed;
    }

    public void SetStunned(bool stunned)
    {
        isStunned = stunned;
        //----- NUEVO CODIGO -----
        ApplyStop();
        //----- NUEVO CODIGO -----
    }

    //----- NUEVO CODIGO ----- ZombieBase lo llama cada frame; solo hace algo cuando el valor cambia
    public void SetHolding(bool holding)
    {
        if (isHolding == holding) return;
        isHolding = holding;
        ApplyStop();
    }
    //----- NUEVO CODIGO -----
}