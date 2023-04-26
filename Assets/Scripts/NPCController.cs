using UnityEngine;

public class NPCController : MonoBehaviour
{
    public GameObject rig;
    public GameObject gripPoint;

    [HideInInspector] public bool isFainted;

    // Start is called before the first frame update
    void Start()
    {
        GameController.gameController.RagdollMode(false, rig, GetComponent<Animator>(), GetComponent<BoxCollider>());
    }

    // Update is called once per frame
    void Update()
    {
        //Respawn dos NPCs
        if(isFainted && rig.transform.position.y < -10)
        {
            foreach (var enemySpawn in GameController.gameController.Enemies)
            {
                if (enemySpawn.Value == gameObject)
                {
                    GameController.gameController.playerMoney += 10;
                    GetComponentInParent<NPCController>().isFainted = false;
                    GameController.gameController.RagdollMode(false, rig, GetComponent<Animator>(), GetComponent<BoxCollider>());
                    transform.rotation = Quaternion.identity;
                    transform.position = enemySpawn.Key.transform.position;
                }
            }
        }
    }
}
