using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using FishORamaEngineLibrary;
using SharpDX.DirectWrite;

namespace FishORama
{
    /// CLASS: Piranha - this class is structured as a FishORama engine Token class
    /// It contains all the elements required to draw a token to screen and update it (for movement etc)
    class Piranha : IDraw
    {
        // CLASS VARIABLES
        // Variables hold the information for the class
        // NOTE - these variables must be present for the class to act as a TOKEN for the FishORama engine
        private string textureID;               // Holds a string to identify asset used for this token
        private float xPosition;                // Holds the X coordinate for token position on screen
        private float yPosition;                // Holds the Y coordinate for token position on screen
        private int xDirection;                 // Holds the direction the token is currently moving - X value should be either -1 (left) or 1 (right)
        private int yDirection;                 // Holds the direction the token is currently moving - Y value should be either -1 (down) or 1 (up)
        private Screen screen;                  // Holds a reference to the screen dimansions (width and height)
        private ITokenManager tokenManager;     // Holds a reference to the TokenManager - for access to ChickenLeg variable

        // *** ADD YOUR CLASS VARIABLES HERE *** 
        private float xSpeed;
        private float ySpeed;
        private int fishNumber;
        private int teamNumber;
        private float newPositionUp;
        private float newPositionDown;
        private Vector2 originalPosition;
        private float initXPosition;
        private float initYPosition;
        private bool clockWise;
        private float angle;
        private float radius;
        private float circleSpeed;
        public PiranhaState currentState = PiranhaState.StartPosition;
        public int Score = 0; // Variable to store score
       
      





        /// CONSTRUCTOR: Piranha Constructor
        /// The elements in the brackets are PARAMETERS, which will be covered later in the course
        public Piranha(string pTextureID, float pXpos, float pYpos, float pXspeed, float pYspeed, Screen pScreen, ITokenManager pTokenManager,  int pFishNumber, int pTeamNumber, int pxDirection, int pyDirection, bool pclockWise)
        {
            // State initialisation (setup) for the object
            textureID = pTextureID;
            xPosition = pXpos;
            yPosition = pYpos;
            xSpeed = pXspeed;
            ySpeed = pYspeed;
            xDirection = pxDirection;
            yDirection = pyDirection;
            screen = pScreen;
            tokenManager = pTokenManager;
            fishNumber = pFishNumber; // Store fish number
            teamNumber = pTeamNumber; // Store team number
            newPositionUp = yPosition + 50;
            newPositionDown = yPosition - 50;
            originalPosition = new Vector2(pXpos, pYpos);
            initXPosition = pXpos;
            initYPosition = pYpos;
            angle = 0f;
            radius = 40f;
            circleSpeed = 0.05f;
            clockWise = pclockWise; // used to change the direction of the circle i.e both team rotating towards the centre
           
            
        }
        

        public enum PiranhaState //ref enum
        {
            StartPosition,
            FeedingTime,
            ReturnPosition
        }

        private void startPosition() //ref circular movement
        {   
            if (teamNumber == 1) //sets the direction of the piranha
            {
                xDirection = 1;
            }
            else
            {
                xDirection = -1;
            }

            if (clockWise) // to set which way the piranha rotate
                angle -= circleSpeed;
            else
                angle += circleSpeed;

            xPosition = initXPosition + MathF.Cos(angle) * radius; //  circle movement equation
            yPosition = initYPosition + MathF.Sin(angle) * radius; //  circle movement equation
           
        }

        private void feedingTime() // ref movement 
        {
            // creates varaible for current position
            Vector2 currentPosition = new Vector2(xPosition, yPosition);

            // position of the chickenleg
            Vector2 chickenlegPosition = new Vector2(tokenManager.ChickenLeg.Position.X, tokenManager.ChickenLeg.Position.Y);

            // Calculate direction vector from current position to chickenleg
            Vector2 direction = Vector2.Normalize(chickenlegPosition - currentPosition);

            //  movement speed
            float Speed = 4.0f;


            // Update the position by adding the direction multiplied by the speed
            currentPosition += direction * Speed;

            // Update the object's position
            xPosition = currentPosition.X;
            yPosition = currentPosition.Y;

            //calculate distance to chicken from current position
            float distanceToChickenLeg = Vector2.Distance(new Vector2(xPosition, yPosition), tokenManager.ChickenLeg.Position);

            // Check if the piranha is within 10 of the chicken leg
            if (distanceToChickenLeg < 10.0f)
            {
                // Remove the chicken leg from the token manager
                tokenManager.RemoveChickenLeg();

                //incrementing score by 1
                Score++;
               


            }
            
        }

        private void returnposition()
        {
           if (teamNumber == 1)  // used to change direction when returning to original position
            {
                xDirection = -1;
            }
           else
            {
                xDirection = 1;
            }

            // Current position of the piranha
            Vector2 currentPosition = new Vector2(xPosition, yPosition);

            // Original position of the piranha
            Vector2 originalPosition = new Vector2(initXPosition, initYPosition);

            // Calculate direction vector from current position to original position
            Vector2 direction = Vector2.Normalize(originalPosition - currentPosition);

            // Define the  speed
            float Speed = 4.0f; // Adjust as needed

            float distanceToOriginal = Vector2.Distance(currentPosition, originalPosition);


            // Update the position by adding the direction multiplied by the speed
            currentPosition += direction * Speed;

            if (distanceToOriginal < 2)
            {
                // If piranha is close to the original position, reset its position 
                xPosition = originalPosition.X;
                yPosition = originalPosition.Y;

                currentState = PiranhaState.StartPosition;
            }
            else
            {
                // Update the piranha's position variables
                xPosition = currentPosition.X;
                yPosition = currentPosition.Y;
            }

        }

        private void StartPositionBehavior()
        {
            //  start position behavior 
            startPosition();

            
        }

        private void FeedingTimeBehavior()
        {
          
            //  feeding time behavior, checks to see if chickenleg is in the scene
            if ( tokenManager.ChickenLeg != null)
            {
                feedingTime();
            }
            else
            {
                currentState = PiranhaState.ReturnPosition;
            }
        }

        private void ReturnPositionBehavior()
        {
            //  return position behavior 
            returnposition();
  
        }



        //switch statement used to move between currentState 
        public void Update()
        {
            switch (currentState)
            {
                case PiranhaState.StartPosition:
                    StartPositionBehavior();
                    break;
                case PiranhaState.FeedingTime:
                    FeedingTimeBehavior();
                    break;
                case PiranhaState.ReturnPosition:
                    ReturnPositionBehavior();
                    break;
                default:
                   
                    break;
            }

            
        }
    



        /// METHOD: Draw - Called repeatedly by FishORama engine to draw token on screen
        /// DO NOT ALTER, and ensure this Draw method is in each token (fish) class
        /// Comments explain the code - read and try and understand each lines purpose
        public void Draw(IGetAsset pAssetManager, SpriteBatch pSpriteBatch)
        {
            Asset currentAsset = pAssetManager.GetAssetByID(textureID); // Get this token's asset from the AssetManager

            SpriteEffects horizontalDirection; // Stores whether the texture should be flipped horizontally

            if (xDirection < 0)
            {
                // If the token's horizontal direction is negative, draw it reversed
                horizontalDirection = SpriteEffects.FlipHorizontally;
            }
            else
            {
                // If the token's horizontal direction is positive, draw it regularly
                horizontalDirection = SpriteEffects.None;
            }

            // Draw an image centered at the token's position, using the associated texture / position
            pSpriteBatch.Draw(currentAsset.Texture,                                             // Texture
                              new Vector2(xPosition, yPosition * -1),                                // Position
                              null,                                                             // Source rectangle (null)
                              Color.White,                                                      // Background colour
                              0f,                                                               // Rotation (radians)
                              new Vector2(currentAsset.Size.X / 2, currentAsset.Size.Y / 2),    // Origin (places token position at centre of sprite)
                              new Vector2(1, 1),                                                // scale (resizes sprite)
                              horizontalDirection,                                              // Sprite effect (used to reverse image - see above)
                              1);                                                               // Layer depth
        }
    }
}
