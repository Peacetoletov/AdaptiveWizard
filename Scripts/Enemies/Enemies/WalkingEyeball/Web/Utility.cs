using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Assertions;


namespace AdaptiveWizard.Assets.Scripts.Enemies.Enemies.WalkingEyeball.Web
{
    public static class Utility
    {
        public static int MoveAndCheckCollision(Web web, Vector2 direction, float speed, BoxCollider2D collider) {
            float distance = speed * Time.deltaTime;
            RaycastHit2D hit = Physics2D.BoxCast(web.transform.position, collider.size, 0, direction, distance, LayerMask.GetMask("Wall", "Player"));
            
            Vector2 movement = direction.normalized * distance;
            Move(web, movement);

            // Return 0 if nothing was hit, return 1 if player was hit, return 2 if wall was hit.
            if (hit.collider == null) {    
                return 0;
            }
            if (1 << hit.transform.gameObject.layer == LayerMask.GetMask("Player")) {
                return 1;
            }
            return 2;
        }

        private static void Move(Web web, Vector3 movement) {
            web.transform.position += movement;
        }
    }
}