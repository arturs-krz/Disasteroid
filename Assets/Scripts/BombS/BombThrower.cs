using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;

public class BombThrower : MonoBehaviourPun
{
    private MoneyManager moneyManager;
    private Image buttonFill;

    private float bombCost = 2e8f;
    private float cooldown = 2f;

    bool onCooldown = false;
    float cooldownTimer = 0f;

    private float startTime;
    private bool showedInfo = false;
    void Start()
    {
        moneyManager = GameObject.FindWithTag("ResourceManager").GetComponent<MoneyManager>();
        buttonFill = GameObject.Find("BombButton").GetComponent<Image>();

        PhotonGameLobby.OnJoinGame += (isMaster) =>
        {
            if (!isMaster)
            {
                //UIMessage.ShowMessage("Launch bombs to deflect incoming asteroids!");
                startTime = Time.time;
            }
        };
    }

    void Update()
    {
        if (buttonFill == null)
        {
            buttonFill = GameObject.Find("BombButton").GetComponent<Image>();
        }

        if (!showedInfo && Time.time - startTime > 2f && !PhotonNetwork.IsMasterClient)
        {
            showedInfo = true;
            UIMessage.ShowMessage("Launch bombs to deflect incoming asteroids!");
        }

        if (onCooldown)
        {
            cooldownTimer += Time.deltaTime;
            if (cooldownTimer > cooldown)
            {
                onCooldown = false;
                cooldownTimer = 0f;
                buttonFill.fillAmount = 1f;
            }
            else
            {
                float fillAmount = cooldownTimer / cooldown;
                buttonFill.fillAmount = fillAmount;
            }
        }
    }

    public void BombThrowing()
    {
        //if ((ARController.Instance.earthMarker.transform.position - ARController.Instance.FirstPersonCamera.transform.position).magnitude < 1f)
        //{
        //    // too close to Earth, don't allow
            
        //}

        if (!onCooldown && moneyManager.currentMoney >= bombCost)
        {
            Vector3 spawnPoint = ARController.Instance.FirstPersonCamera.transform.position + (ARController.Instance.FirstPersonCamera.transform.forward * 0.15f);
            Vector3 localPosition = ARController.Instance.earthMarker.transform.InverseTransformPoint(spawnPoint);
            Vector3 localOrientation = ARController.Instance.earthMarker.transform.InverseTransformDirection(ARController.Instance.FirstPersonCamera.transform.forward);
            photonView.RPC("SpawnBomb", RpcTarget.MasterClient, localPosition, localOrientation);

            moneyManager.currentMoney -= bombCost;
            onCooldown = true;
        }
        else if (moneyManager.currentMoney < bombCost)
        {
            UIMessage.ShowMessage("Insufficient funds!");
        }
    }

    [PunRPC]
    void SpawnBomb(Vector3 position, Vector3 localOrientation)
    {
        //Debug.Log("Received bomb spawn request");
        // GameObject resourceManager = GameObject.FindGameObjectWithTag("ResourceManager");
        // moneyManager moneyManage = resourceManager.GetComponent<moneyManager>();

        //comment this when the other thingy is working
        // moneyManage.currentMoney -= bombCost;
        if (moneyManager.currentMoney >= bombCost)
        {
            moneyManager.currentMoney -= bombCost;
            PhotonNetwork.Instantiate("AR-bomb", position, Quaternion.LookRotation(localOrientation));
        }

        //if(moneyManage.currentMoney > costs)
        //{
        //    moneyManage.currentMoney -= costs;
        //    PhotonNetwork.Instantiate("AR-Bomb", localPosition, Quaternion.identity);
        //}
        //else
        //{
        //    // write: not enough moneyyyy
        //}
    }
}
