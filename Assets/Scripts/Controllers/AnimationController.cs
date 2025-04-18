using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationController : MonoBehaviour
{
    PlayerController playerController;
    public PlayerController GetPlayerController {  get { if (playerController == null) playerController = GetComponentInParent<PlayerController>(); return playerController; } }

    EnemyController enemyController;
    public EnemyController GetEnemyController {  get { if (enemyController == null) enemyController = GetComponentInParent<EnemyController>(); return enemyController; } }

    NPCController npcController;
    public NPCController GetNPCController { get { if (npcController == null) npcController = GetComponentInParent<NPCController>(); return npcController; } }

    public void StartLeftPunch()
    {
        if (GetPlayerController != null)
        {
            if (GetPlayerController.DamagedTimer == 0)
            {
                GetPlayerController.GetLeftHand.Attack(GetPlayerController.GetPlayer.playerStruct.attackDamage);
                GetPlayerController.GetLeftHand.Trail(true);
            }
        }

        if(GetNPCController != null)
        {
            if (GetNPCController.DamagedTimer == 0)
            {
                GetNPCController.GetLeftHand.Attack(50);
                GetNPCController.GetLeftHand.Trail(true);
            }
        }
    }
    public void StopLeftPunch()
    {
        if (GetPlayerController != null)
        {
            GetPlayerController.GetLeftHand.StopAttack();
            GetPlayerController.GetLeftHand.Trail(false);
        }
        if (GetNPCController != null)
        {
            GetNPCController.GetLeftHand.StopAttack();
            GetNPCController.GetLeftHand.Trail(false);
        }
    }

    public void StartRightPunch()
    {
        if (GetPlayerController != null)
        {
            if (GetPlayerController.DamagedTimer == 0)
            {
                GetPlayerController.GetRightHand.Attack(GetPlayerController.GetPlayer.playerStruct.attackDamage);
                GetPlayerController.GetRightHand.Trail(true);
            }
        }
        if (GetNPCController != null)
        {
            if (GetNPCController.DamagedTimer == 0)
            { 
                GetNPCController.GetRightHand.Attack(50);
                GetNPCController.GetRightHand.Trail(true);
            }
        }
    }

    public void StopRightPunch()
    {
        if (GetPlayerController != null)
        {
            GetPlayerController.GetRightHand.StopAttack();
            GetPlayerController.GetRightHand.Trail(false);
        }
        if (GetNPCController != null)
        {
            GetNPCController.GetRightHand.StopAttack();
            GetNPCController.GetRightHand.Trail(false);
        }
    }

    public void StartScratch()
    {
        if (GetNPCController.DamagedTimer == 0)
        {
            GetNPCController.GetRightHand.Attack(GetNPCController.npcStruct.attack);
            GetNPCController.GetRightHand.Trail(true);
        }
    }

    public void StopScratch()
    {
        GetNPCController.GetRightHand.StopAttack();
        GetNPCController.GetRightHand.Trail(false);
    }


    public void StartAxeSlash()
    {
        if (GetPlayerController.DamagedTimer == 0)
        {
            GetPlayerController.equipItem.GetComponent<Weapon>().StartAttack();
        }
     //   GetPlayerController.equipWeapon.GetComponent<WeaponTrail>().Trail(true);
      /*  GameObject slash = Instantiate(GetPlayerController.testEffect);
        slash.transform.position = GetPlayerController.Transforms.position;
        Vector3 currentRotation = GetPlayerController.Transforms.rotation.eulerAngles;
        currentRotation.y += 180;
        slash.transform.rotation = Quaternion.Euler(currentRotation);
        slash.GetComponent<ParticleSystem>().Play();    */

    }

    public void StopAxeSlash()
    {
      //  GetPlayerController.equipWeapon.GetComponent<WeaponTrail>().Trail(false);
       GetPlayerController.equipItem.GetComponent<Weapon>().StopAttack();

    }
}
