using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using FishORamaEngineLibrary;
using System.Collections.Generic;
using System.Drawing.Text;
using static FishORama.Piranha;

namespace FishORama
{
    /// CLASS: Simulation class - the main class users code in to set up a FishORama simulation
    /// All tokens to be displayed in the scene are added here
    public class Simulation : IUpdate, ILoadContent
    {
        // CLASS VARIABLES
        // Variables store the information for the class
        private IKernel kernel;                 // Holds a reference to the game engine kernel which calls the draw method for every token you add to it
        private Screen screen;                  // Holds a reference to the screeen dimensions (width, height)
        private ITokenManager tokenManager;     // Holds a reference to the TokenManager - for access to ChickenLeg variable
        
        /// PROPERTIES
        public ITokenManager TokenManager      // Property to access chickenLeg variable
        {
            set { tokenManager = value; }
        }

        // *** ADD YOUR CLASS VARIABLES HERE ***
        // Variables to hold fish will be declared here

        //  Piranha list
        List<Piranha> piranhaFishList = new List<Piranha>();

        // Random object
        Random rand;
        int scoreteam1;
        int scoreteam2;




        /// CONSTRUCTOR - for the Simulation class - run once only when an object of the Simulation class is INSTANTIATED (created)
        /// Use constructors to set up the state of a class
        public Simulation(IKernel pKernel)
        {
            kernel = pKernel;                   // Stores the game engine kernel which is passed to the constructor when this class is created
            screen = kernel.Screen;             // Sets the screen variable in Simulation so the screen dimensions are accessible

            // *** ADD OTHER INITIALISATION (class setup) CODE HERE ***
            rand = new Random();
           





        }

        /// METHOD: LoadContent - called once at start of program
        /// Create all token objects and 'insert' them into the FishORama engine
        public void LoadContent(IGetAsset pAssetManager)
        {
            // *** ADD YOUR NEW TOKEN CREATION CODE HERE ***
            // Code to create fish tokens and assign to thier variables goes here
            // Remember to insert each token into the kernel

            // Create 3 Piranha  for the left side 
            for (int i = 0; i < 3; i++)
            {
                // Calc randomised start positions for left side
                int XposL = -300;
                int YposL = 150 - (i * 150); // distance between each fish on y axis

                // Create fish for left side
                Piranha piranha = new Piranha("Piranha2", XposL, YposL, 1, 1, screen, tokenManager, i + 1, 1, 1,1, true); // Team 1
                piranhaFishList.Add(piranha);
                kernel.InsertToken(piranha);
            }

            // create 3 piranha for the right side
            for (int i = 0; i < 3; i++)
            {
                // Calc randomised start positions for right side
                int XposR = 300;
                int YposR = 150 - (i * 150); // distance between each fish on y axis

                // Create fish for right side
                Piranha piranha = new Piranha("Piranha2", XposR, YposR, 1, 1, screen, tokenManager, i + 1, 2, -1,1, false); // Team 2
                piranhaFishList.Add(piranha);
                kernel.InsertToken(piranha);

            }

        }

        private bool stillinStartPosition() // boolean to check if piranha in startposition
        {
            foreach (Piranha piranha in piranhaFishList)
            {
                if (piranha.currentState != PiranhaState.StartPosition)
                {
                    return false;
                }
            }
            return true;
        }
        private void PlaceLeg()
            {
           
                // Place the chicken leg at the center of the screen
                ChickenLeg newChickenLeg = new ChickenLeg("ChickenLeg", 0, 0);
                tokenManager.SetChickenLeg(newChickenLeg);
                kernel.InsertToken(newChickenLeg);
            
            }
        private void selectedFish() // used to pick a piranha from each team
        {
            int selectedFishNumber = rand.Next(0, 3); // picking number between 0 and 2
            // random number used to pick index from list
            piranhaFishList[selectedFishNumber].currentState = PiranhaState.FeedingTime;

            // whichever random number is generated + 3 so it selects the opposite fish in the list
            piranhaFishList[selectedFishNumber + 3].currentState = PiranhaState.FeedingTime; 

        }
 

        /// METHOD: Update - called 60 times a second by the FishORama engine when the program is running
        /// Add all tokens so Update is called on them regularly
        public void Update(GameTime gameTime)
        {

            // *** ADD YOUR UPDATE CODE HERE ***
            // Each fish object (sitting in a variable) must have Update() called on it here
           
            // Loop over list and call update on piranha
            foreach (Piranha fish in piranhaFishList)
            {
                fish.Update();

              
            }


            //place chickenleg randomly if not on screen and piranha in startposition
            if (stillinStartPosition() && tokenManager.ChickenLeg ==  null)
            {
                // used to create a number for placing the chickenleg
                int randomNumber = rand.Next(0, 200);
                if (randomNumber == 1)
                {
                    PlaceLeg();
                    selectedFish();

                    scoreteam1 = 0; // sets score for team 1 to 0
                    scoreteam2 = 0; // sets score for team 2 to 0

                    // loop to iterate over piranha list count
                    for (int i = 0; i < piranhaFishList.Count; i++)
                    {
                        if (i < 3)
                        {   // adding piranha score to the team score
                            scoreteam1 += piranhaFishList[i].Score;
                        }
                        else
                        {   // adding piranha score to the team score
                            scoreteam2 += piranhaFishList[i].Score;
                        }
                    }

                    Console.WriteLine(scoreteam1);
                    Console.WriteLine(scoreteam2);
                    Console.WriteLine();
                }
   
            }  


        }
       
    }
}
