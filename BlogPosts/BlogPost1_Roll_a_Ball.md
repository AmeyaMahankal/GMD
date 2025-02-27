# Improving the Roll-a-Ball Unity Game Tutorial
We tried to expand on the base game by adding some new mechanics that we though would be fun: **a freeze obstacle**, **moving collectibles**, **jumping**, and **better dying system**. These mechanics make the game at the very least more dynamic. Here’s how we did it!

## Freeze Obstacle

We introduced an obstacle that slows down the player for 3 seconds upon collision. This adds a new layer of challenge by forcing the player to maneuver carefully. The player would change colour indicating the freeze effect is active.

## Moving Collectibles
To make the environment feel more dynamic, we added moving obstacles and collectibles. We got the idea from the Enemy object, so we used Unity’s NavMesh for pathfinding and ensured that collectible items are zooming through the playable space in random directions.

## Jumping
Rolling is fun and all, but jump



ing adds more control and fun to the game, most importantly it adds more chaos We modified the player’s movement script to allow jumping using the space bar.

To implement this, we started by introducint two variables. `isGrounded` which would tell us if the player is on the ground and `jumpForce` to control how powerful the jump is

`private bool isGrounded`

`public float jumpForce = 2.0f`

`isGrounded` is updated in the `FixedUpdate` together with the movement to ensure consistency. We used `Physics.Raycast` to check if the player is on the ground before allowing them to jump. This prevents mid-air jumps. A Raycast is like an invisible laser beam that extends from a point in a specific direction to detect objects in its path.

```isGrounded = Physics.Raycast(transform.position, Vector3.down, 0.5f);```

If the ray detects a surface below the player within 0.5f units, `isGrounded` is set to true. Otherwise, it's false, preventing jumping while in the air.
Then we added a new `OnJump` action to our player controller bound to the Spacebar. Every time the player presses the Spacebar the `OnJump` event is fired.

```
void OnJump()
 {
     if (isGrounded) // Only allow jump if the player is grounded
     {
         Debug.Log("Jump");
         rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
     }
 }
```
We used addForce on the rigid body of the player to apply sudden upward force instantly and create a jump. ...And Voila, we have jumping. 
![Untitled design](https://github.com/user-attachments/assets/05622eba-a173-4268-993e-8eba2cb534b7)



## Fast and Faster
