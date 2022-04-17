using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;



public class DieRelive : MonoBehaviour
{

    public void TakeDamage(int amount)
    {
        if (health = 0)
        {
            health = maxhealth;
            Respawn();
        }
    }

    private void Respawn()
    {
        yield return new WaitForSeconds(3);
        Player.transform.position = Player.RespawnPosition;
        
    }
}
