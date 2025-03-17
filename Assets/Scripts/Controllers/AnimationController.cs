using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationController : MonoBehaviour
{
    PlayerController playerController;
    public PlayerController GetPlayerController {  get { if (playerController == null) playerController = GetComponentInParent<PlayerController>(); return playerController; } }

    EnemyController enemyController;
    public EnemyController GetEnemyController {  get { if (enemyController == null) enemyController = GetComponentInParent<EnemyController>(); return enemyController; } }

    public void StartLeftPunch()
    {
        GetPlayerController.GetLeftHand.Attack(GetPlayerController.GetPlayer.playerStruct.attackDamage);
        GetPlayerController.GetLeftHand.Trail(true);
    }
    public void StopLeftPunch()
    {
        GetPlayerController.GetLeftHand.StopAttack();
        GetPlayerController.GetLeftHand.Trail(false);
    }

    public void StartRightPunch()
    {
        GetPlayerController.GetRightHand.Attack(GetPlayerController.GetPlayer.playerStruct.attackDamage);
        GetPlayerController.GetRightHand.Trail(true);
    }

    public void StopRightPunch()
    {
        GetPlayerController.GetRightHand.StopAttack();
        GetPlayerController.GetRightHand.Trail(false);
    }

    public void StartScratch()
    {
        GetEnemyController.GetRightHand.Attack(enemyController.enemyStruct.attack);
        GetEnemyController.GetRightHand.Trail(true);
    }

    public void StopScratch()
    {
        GetEnemyController.GetRightHand.StopAttack();
        GetEnemyController.GetRightHand.Trail(false);
    }


    public void StartAxeSlash()
    {
        GetPlayerController.equipItem.GetComponent<Weapon>().StartAttack();
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
