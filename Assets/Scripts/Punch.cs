using UnityEngine;

public class Punch : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Enemy")
        {
            if (GetComponentInParent<PlayerController>().anim.GetCurrentAnimatorStateInfo(1).IsName("Punch"))
            {
                GameController.gameController.RagdollMode(true, other.gameObject.GetComponent<NPCController>().rig, other.gameObject.GetComponent<Animator>(), other.gameObject.GetComponent<Collider>());
                other.gameObject.GetComponent<NPCController>().isFainted = true;
            }
        }
    }
}
