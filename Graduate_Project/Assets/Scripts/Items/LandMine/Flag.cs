using System.Collections;
using Unity.Mathematics;
using UnityEngine;

namespace Items.LandMine
{
    public class Flag : LandMine
    {
        private void Update()
        {
            Invoke(nameof(MineSpawn),3f);
        }



        private void MineSpawn()
        {
            var selfPos = gameObject.transform.position;
            Instantiate(mine, new Vector3(selfPos.x, selfPos.y,selfPos.z) , quaternion.identity); 
            Destroy(gameObject);
        }
        

        /*private IEnumerator MineSpawn()
        {
            var selfPos = gameObject.transform.position;
            Instantiate(mine, new Vector3(selfPos.x, selfPos.y,selfPos.z) , quaternion.identity); 
            Destroy(gameObject);
            yield return new WaitForSeconds(3);
        }*/
    }
}