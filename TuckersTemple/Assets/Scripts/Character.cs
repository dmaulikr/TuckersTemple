/*
 * Character.cs
 * 
 * Is attached to the Actor prefab
 */

// namespaces:
using UnityEngine;
using System.Collections;

// Character class - base class for characters
public class Character : MonoBehaviour {

	// fields
	public int direction;
    private GameMaster gm;
    private Vector2 goalPos;
    private bool isWalking;
    public float speed = 0.05f;
    private Vector2[] v2Dirs = { Vector2.up, Vector2.right, Vector2.down, Vector2.left };

	// Actor Initialization
	void Start() {
        //find and save the GameMaster
        gm = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameMaster>();

		// extract initial direction from lvl file
		direction = 0;

        goalPos = transform.position;
        isWalking = false;
	}
	
	// Actor Update
	void Update() {
        // check for characters turn to walk
        if (isWalking)
        {
            if(goalPos.x == transform.position.x && goalPos.y == transform.position.y)
            {
                print(goalPos);
                //set new parent based on goalPos
                transform.parent = gm.getTile(transform.position).transform;
                isWalking = false;
                gm.doneWalking();
            }
            else
            {
                transform.position = Vector2.MoveTowards(transform.position, goalPos, speed);
            }
        }

		// check enemy and trap collisions:
		// if (collision with enemy || trap) {
		// 	 	// play death sprite/animation
	    //		Destroy(GameObject)
	    //}

		// Check goal collision
		//if (goalCollision) {
		//	win
		//}
	}

	// walk to new tile
	public void walk(){
        int directionToWalk = findNextMove(direction);

        //decide where to move and call WalkTo based on directionToWalk
        float walkDistance = gm.tileSize;
        switch (directionToWalk)
        {
            //no moves
            case -1:
                gm.doneWalking();
                break;
            case 0:
                direction = directionToWalk;
                WalkTo(new Vector2(0, walkDistance));
                break;
            case 1:
                direction = directionToWalk;
                WalkTo(new Vector2(walkDistance, 0));
                break;
            case 2:
                direction = directionToWalk;
                WalkTo(new Vector2(0, -walkDistance));
                break;
            case 3:
                direction = directionToWalk;
                WalkTo(new Vector2(-walkDistance, 0));
                break;
        }

	}

    //Tells the character the offset to walk to
    public void WalkTo(Vector2 pos)
    {
        goalPos = new Vector2(pos.x + transform.position.x, pos.y + transform.position.y);
        isWalking = true;
    }

    //take in direction for actor to move
    //returns 0,1,2,3 for which direction they should move
    //returns -1 if no valid move found
    private int findNextMove(int dir)
    {

        //order to try in is straight->right->left->back

        //this is the modifies to the directions something can face
        int[] dirMods = { 0, 1, -1, 2 };
        //directions are 0,1,2,3, with 0 being up and going clockwise.
        for (int i = 0; i < 4; i++)
        {
            //make a current direction by adding the direction modifier to the direction
            int currDir = dir + dirMods[i];

            //Normalize currDir within 0 to 3
            if (currDir > 3)
            {
                currDir -= 4;
            }
            else if (currDir < 0)
            {
                currDir += 4;
            }
            //RAYCAST LASER BEAMS ♫♫♫♫♫
            //Actual comment: Raycasts are complicated.  Here it is then I'll explain
            RaycastHit2D raycast = Physics2D.Raycast(transform.position, v2Dirs[currDir], gm.tileSize, LayerMask.GetMask("Wall"));
            /*
             * RaycastHit2D is a return type of raycasts, that holds something
             * If it holds nothing, then there was nothing! woooo
             * Arg1 - position, where we start the laser beam
             * Arg2 - direction, from Vector2.up, down etc
             * Arg3 - distance, we don't want further than tile size
             * Arg4 - The layer mask, currently walls so thats all we shoot at
             */
            
            //if raycast doesn't return anything, then there isn't anything there!
            if (raycast.collider == null)
            {
              //return the first valid move found
                return currDir;
            }
            else
            {
                print("Raycast hit something in " + currDir);
            }
        }
        //if no moves were found
        return -1;
    }
}
