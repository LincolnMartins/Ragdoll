using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class GameController : MonoBehaviour
{
    //Singleton
    public static GameController gameController;

    //NPCs & Player
    public Dictionary<GameObject, GameObject> Enemies = new Dictionary<GameObject, GameObject>();
    [SerializeField] private GameObject Enemy;
    [SerializeField] private GameObject Player;

    //Level & Money
    [HideInInspector] public int playerMoney = 0;
    [HideInInspector] public int playerLevel = 1;
    [SerializeField] private TextMeshProUGUI moneyText;
    [SerializeField] private TextMeshProUGUI levelText;

    private void Awake()
    {
        gameController = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        Player = GameObject.FindGameObjectWithTag("Player");

        //Instancia os NPCs nos pontos de respawn
        foreach (var spawnPoint in GameObject.FindGameObjectsWithTag("Respawn"))
            Enemies.Add(spawnPoint, Instantiate(Enemy, spawnPoint.transform.position, Quaternion.identity));
    }

    // Update is called once per frame
    void Update()
    {
        //calculo de ganho de nivel
        if (playerLevel < 3)
        {
            if (playerMoney >= playerLevel * 70)
            {
                playerMoney -= playerLevel * 70;
                playerLevel++;

                var player = Player.GetComponent<PlayerController>();
                player.playerCharacter.GetComponent<Renderer>().material = player.materials[playerLevel - 2];
            }
        }

        //Mantém o texto do level e money do jogador atualizado
        if (playerMoney > int.Parse(moneyText.text))
            moneyText.text = (int.Parse(moneyText.text) + 1).ToString();
        else if (playerMoney < int.Parse(moneyText.text))
            moneyText.text = (int.Parse(moneyText.text) - 1).ToString();
        
        if(playerLevel != int.Parse(levelText.text)) levelText.text = playerLevel.ToString();
    }


    //Ativa/Desativa Ragdolls
    public void RagdollMode(bool mode, GameObject rig, Animator anim, Collider col)
    {
        Collider[] ragdollColliders;
        Rigidbody[] limbsRigidbodies;

        ragdollColliders = rig.GetComponentsInChildren<Collider>();
        limbsRigidbodies = rig.GetComponentsInChildren<Rigidbody>();

        if (mode)
        {
            anim.enabled = false;
            col.enabled = false;

            foreach (var collider in ragdollColliders)
                collider.enabled = true;

            foreach (var rigidbody in limbsRigidbodies)
                rigidbody.isKinematic = false;
        }
        else
        {
            foreach (var collider in ragdollColliders)
                if(!collider.isTrigger)
                    collider.enabled = false;

            foreach (var rigidbody in limbsRigidbodies)
                rigidbody.isKinematic = true;

            col.enabled = true;
            anim.enabled = true;
        }
    }
}
