using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public GameObject playerCharacter;
    public Material[] materials;

    [SerializeField] private float movementSpeed;
    [SerializeField] private float rotationSpeed;
    [SerializeField] private GameObject playerRig;
    [SerializeField] private GameObject gripPoint;    

    private Vector3 movementDirection;
    private Rigidbody rb;
    [HideInInspector] public Animator anim;

    private GameObject lastCorpse;
    private List<GameObject> grabbedCorpses;
    private Vector3 lastPosition;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        anim = GetComponent<Animator>();

        GameController.gameController.RagdollMode(false, playerRig, anim, GetComponent<BoxCollider>());

        grabbedCorpses = new List<GameObject>();
        lastCorpse = gameObject;
    }

    // Update is called once per frame
    void Update()
    {
        //movimentação
        movementDirection = new Vector3(Input.GetAxis("Horizontal") * movementSpeed, 0.0f, Input.GetAxis("Vertical") * movementSpeed);

        //carrega corpos
        if (Input.GetButtonDown("Fire2"))
        {
            int corpseLimit;
            switch(GameController.gameController.playerLevel)
            {
                case 2: { corpseLimit = 5; break; }
                case 3: { corpseLimit = 7; break; }
                default: { corpseLimit = 3; break; }
            }

            if (grabbedCorpses.Count < corpseLimit)
            {
                foreach (var corpse in GameObject.FindGameObjectsWithTag("Enemy"))
                {
                    if (corpse.GetComponent<NPCController>().isFainted)
                    {
                        if (Vector3.Distance(corpse.transform.position, transform.position) <= 3)
                        {
                            float angle = Vector3.Angle(transform.forward, corpse.transform.position - transform.position);
                            if (Mathf.Abs(angle) <= 90)
                            {
                                if (!grabbedCorpses.Contains(corpse))
                                {
                                    grabbedCorpses.Add(corpse);
                                    if (!anim.GetBool("isCarrying")) anim.SetBool("isCarrying", true);
                                    corpse.gameObject.GetComponent<NPCController>().rig.GetComponent<Rigidbody>().isKinematic = true;
                                    if (lastCorpse != gameObject) corpse.transform.parent = lastCorpse.GetComponent<NPCController>().gripPoint.transform;
                                    else corpse.transform.parent = gripPoint.transform;
                                    corpse.GetComponent<NPCController>().rig.transform.localPosition = new Vector3(0, 0, 0);
                                    corpse.GetComponent<NPCController>().rig.transform.localRotation = Quaternion.Euler(new Vector3(0, 0, 0));
                                    corpse.GetComponent<NPCController>().transform.localPosition = new Vector3(0, 0, 0);
                                    corpse.GetComponent<NPCController>().transform.localRotation = Quaternion.Euler(new Vector3(0, 0, 0));
                                    lastCorpse = corpse;
                                    break;
                                }
                            }
                        }
                    }
                }
            }
        }

        //soco
        if (Input.GetButtonDown("Fire1"))
            if (!anim.GetBool("isCarrying"))
                anim.SetTrigger("Punch");

        Vector3 corpse_velocity = (lastPosition - transform.position) * Time.deltaTime * 5;
        lastPosition = transform.position;

        //solta corpos
        if (Input.GetButtonDown("Fire3"))
        {
            if (anim.GetBool("isCarrying"))
            {
                var transforms = gripPoint.GetComponentsInChildren<Transform>();
                foreach (var transform in transforms)
                {
                    var obj = transform.gameObject;
                    if (obj.tag == "Enemy")
                    {
                        transform.parent = null;
                        var corpse = obj.GetComponent<NPCController>().rig.GetComponent<Rigidbody>();
                        corpse.isKinematic = false;
                        corpse.velocity = corpse_velocity;
                    }
                }
                grabbedCorpses.Clear();
                lastCorpse = gameObject;
                anim.SetBool("isCarrying", false);
            }
        }


        //impede movimentação para além dos limites do mapa
        if (transform.position.x > 24) transform.position = new Vector3(24, transform.position.y, transform.position.z);
        else if (transform.position.x < -24) transform.position = new Vector3(-24, transform.position.y, transform.position.z);

        if (transform.position.z > 24) transform.position = new Vector3(transform.position.x, transform.position.y, 24);
        else if (transform.position.z < -24) transform.position = new Vector3(transform.position.x, transform.position.y, -24);
    }

    private void FixedUpdate()
    {
        if (movementDirection != Vector3.zero)
        {
            transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(movementDirection), rotationSpeed * Time.fixedDeltaTime);
            if (!anim.GetBool("isRunning")) anim.SetBool("isRunning", true);
        }
        else if (anim.GetBool("isRunning")) anim.SetBool("isRunning", false);
    }

    private void LateUpdate()
    {
        rb.velocity = movementDirection;
    }
}
