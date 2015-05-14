using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using Shooter.Model;
using Shooter.View;

namespace Shooter.Controller
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class Game1 : Microsoft.Xna.Framework.Game
    {
        private GraphicsDeviceManager graphics;

        private SpriteBatch spriteBatch;
        // Represents the player 
        private Player player;

      
        // Keyboard states used to determine key presses
        private KeyboardState currentKeyboardState;
        private KeyboardState previousKeyboardState;

        // Gamepad states used to determine button presses
        private GamePadState currentGamePadState;
        private GamePadState previousGamePadState;

        // A movement speed for the player
        private float playerMoveSpeed;

        // Image used to display the static background
        private Texture2D mainBackground;

        // Parallaxing Layers
        private ParallaxingBackground bgLayer1;
        private ParallaxingBackground bgLayer2;
        // Enemies
        private Texture2D enemyTexture;
        private List<Enemy> enemies;

        // The rate at which the enemies appear
        private TimeSpan enemySpawnTime;
        private TimeSpan previousSpawnTime;

        // A random number generator
        private Random random;

        private Texture2D projectileTexture;
        private List<Projectile> projectiles;

        // The rate of fire of the player laser
        private TimeSpan fireTime;
        private TimeSpan previousFireTime;

        private Texture2D bigProjectileTexture;
        private List<BigProjectile> bigProjectiles;

        private List<UpProjectile> upProjectiles;
        private List<DownProjectile> downProjectiles;

        // The rate of fire of the player laser
        private TimeSpan largeFireTime;

        private Texture2D explosionTexture;
        private List<Animation> explosions;

        // The sound that is played when a laser is fired
        private SoundEffect laserSound;

        // The sound used when the player or an enemy dies
        private SoundEffect explosionSound;

        // The music played during gameplay
        private Song gameplayMusic;

        //Number that holds the player score
        private int score;
        // The font used to display UI elements
        private SpriteFont font;

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
        }

        public int getScore()
        {
            this.score = score;
            return score;
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            // TODO: Add your initialization logic here
            // Initialize the player class
            player = new Player();
            bgLayer1 = new ParallaxingBackground();
            bgLayer2 = new ParallaxingBackground();

            // Initialize the enemies list
            enemies = new List<Enemy>();

            // Set the time keepers to zero
            previousSpawnTime = TimeSpan.Zero;

            // Used to determine how fast enemy respawns
            enemySpawnTime = TimeSpan.FromSeconds(.6f);

            // Initialize our random number generator
            random = new Random();

            // Set a constant player move speed
            playerMoveSpeed = 8.0f;

            projectiles = new List<Projectile>();
            upProjectiles = new List<UpProjectile>();
            downProjectiles = new List<DownProjectile>();

            // Set the laser to fire every quarter second
            fireTime = TimeSpan.FromSeconds(.15f);

            bigProjectiles = new List<BigProjectile>();

            // Set the laser to fire A LOT
            largeFireTime = TimeSpan.FromSeconds(.01f);

            explosions = new List<Animation>();

            //Set player's score to zero
            score = 0;

            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);

            Vector2 playerPosition = new Vector2(GraphicsDevice.Viewport.TitleSafeArea.X, GraphicsDevice.Viewport.TitleSafeArea.Y
            + GraphicsDevice.Viewport.TitleSafeArea.Height / 2);
            
            
            Animation playerAnimation = new Animation();
            Texture2D playerTexture = Content.Load<Texture2D>("Images/shipAnimation");
            playerAnimation.Initialize(playerTexture, Vector2.Zero, 115, 69, 8, 30, Color.White, 1f, true);

            player.Initialize(playerAnimation, playerPosition);
            // Load the parallaxing background
            bgLayer1.Initialize(Content, "Images/bgLayer1", GraphicsDevice.Viewport.Width, -1);
            bgLayer2.Initialize(Content, "Images/bgLayer2", GraphicsDevice.Viewport.Width, -2);

            enemyTexture = Content.Load<Texture2D>("Images/mineAnimation");

            projectileTexture = Content.Load<Texture2D>("Images/laser");

            bigProjectileTexture = Content.Load<Texture2D>("Images/laser2");

            mainBackground = Content.Load<Texture2D>("Images/mainbackground");

            explosionTexture = Content.Load<Texture2D>("Images/explosion");

            // Load the music
            gameplayMusic = Content.Load<Song>("sound/gameMusic");

            // Load the laser and explosion sound effect
            laserSound = Content.Load<SoundEffect>("sound/laserFire");
            explosionSound = Content.Load<SoundEffect>("sound/explosion");

            // Load the score font
            font = Content.Load<SpriteFont>("Images/gameFont");

            // Start the music right away
            PlayMusic(gameplayMusic);
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }



        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            // Allows the game to exit
            if (Keyboard.GetState().IsKeyDown(Keys.NumPad0))
            {
                this.Exit();
            }

            // TODO: Add your update logic here

            // Save the previous state of the keyboard and game pad so we can determinesingle key/button presses
            previousGamePadState = currentGamePadState;
            previousKeyboardState = currentKeyboardState;

            // Read the current state of the keyboard and gamepad and store it
            currentKeyboardState = Keyboard.GetState();
            currentGamePadState = GamePad.GetState(PlayerIndex.One);


            //Update the player
            UpdatePlayer(gameTime);

            // Update the parallaxing background
            bgLayer1.Update();
            bgLayer2.Update();

            // Update the enemies
            UpdateEnemies(gameTime);

            // Update the collision
            UpdateCollision();

            // Update the projectiles
            UpdateProjectiles();
            UpdateLargeProjectiles();
            UpdateDownProjectiles();
            UpdateUpProjectiles();
            

            // Update the explosions
            UpdateExplosions(gameTime);

            base.Update(gameTime);
        }

        

        private void UpdatePlayer(GameTime gameTime)
        {
            player.Update(gameTime);

            // Get Thumbstick Controls
            player.Position.X += currentGamePadState.ThumbSticks.Left.X * playerMoveSpeed;
            player.Position.Y -= currentGamePadState.ThumbSticks.Left.Y * playerMoveSpeed;

            // Use the Keyboard / Dpad
            if (currentKeyboardState.IsKeyDown(Keys.Left) ||
            currentGamePadState.DPad.Left == ButtonState.Pressed)
            {
                player.Position.X -= playerMoveSpeed;
            }
            if (currentKeyboardState.IsKeyDown(Keys.Right) ||
            currentGamePadState.DPad.Right == ButtonState.Pressed)
            {
                player.Position.X += playerMoveSpeed;
            }
            if (currentKeyboardState.IsKeyDown(Keys.Up) ||
            currentGamePadState.DPad.Up == ButtonState.Pressed)
            {
                player.Position.Y -= playerMoveSpeed;
            }
            if (currentKeyboardState.IsKeyDown(Keys.Down) ||
            currentGamePadState.DPad.Down == ButtonState.Pressed)
            {
                player.Position.Y += playerMoveSpeed;
            }

            // Make sure that the player does not go out of bounds
            player.Position.X = MathHelper.Clamp(player.Position.X, 0, GraphicsDevice.Viewport.Width - player.Width);
            player.Position.Y = MathHelper.Clamp(player.Position.Y, 0, GraphicsDevice.Viewport.Height - player.Height);

            // Fire only every interval we set as the fireTime

            if (Keyboard.GetState().IsKeyDown(Keys.Q) ||
            currentGamePadState.Buttons.LeftShoulder == ButtonState.Pressed)
            {
                if (gameTime.TotalGameTime - previousFireTime > fireTime)
                {
                    // Reset our current time
                    previousFireTime = gameTime.TotalGameTime;

                    // Add the projectile, but add it to the front and center of the player
                    AddProjectile(player.Position + new Vector2(player.Width / 2, 0));

                    // Play the laser sound
                    laserSound.Play();
                }
            }
            if (Keyboard.GetState().IsKeyDown(Keys.W) ||
            currentGamePadState.Buttons.LeftStick == ButtonState.Pressed && currentGamePadState.Buttons.RightStick == ButtonState.Pressed)
            {
                if (gameTime.TotalGameTime - previousFireTime > largeFireTime)
                {
                    //Reset our current time
                    previousFireTime = gameTime.TotalGameTime;

                    AddBigProjectile(player.Position + new Vector2(player.Width / 2, 0));
                    // Play the laser sound
                    laserSound.Play();
                }

            }

            if (Keyboard.GetState().IsKeyDown(Keys.E) ||
            currentGamePadState.Buttons.RightShoulder == ButtonState.Pressed)
            {
                if (gameTime.TotalGameTime - previousFireTime > fireTime)
                {
                    //Reset our current time
                    previousFireTime = gameTime.TotalGameTime;

                    AddProjectile(player.Position + new Vector2(player.Width / 2, 0));
                    AddUpProjectile(player.Position + new Vector2(player.Width / 2, 0));
                    AddDownProjectile(player.Position + new Vector2(player.Width / 2, 0));
                    
                    // Play the laser sound
                    laserSound.Play();
                }

            }

            // reset score if player health goes to zero
            if (player.Health <= 0)
            {
                player.Health = 100;
                score = 0;
            }
        }

        private void PlayMusic(Song song)
        {
            // Due to the way the MediaPlayer plays music,
            // we have to catch the exception. Music will play when the game is not tethered
            try
            {
                // Play the music
                MediaPlayer.Play(song);

                // Loop the currently playing song
                MediaPlayer.IsRepeating = true;
            }
            catch { }
        }

        private void AddExplosion(Vector2 position)
        {
            Animation explosion = new Animation();
            explosion.Initialize(explosionTexture, position, 134, 134, 12, 45, Color.White, 1f, false);
            explosions.Add(explosion);
        }

        private void UpdateExplosions(GameTime gameTime)
        {
            for (int i = explosions.Count - 1; i >= 0; i--)
            {
                explosions[i].Update(gameTime);
                if (explosions[i].Active == false)
                {
                    explosions.RemoveAt(i);
                }
            }
        }

        // Adds normal projetile lazor
        private void AddProjectile(Vector2 position)
        {
                Projectile projectile = new Projectile();
                projectile.Initialize(GraphicsDevice.Viewport, projectileTexture, position);
                projectiles.Add(projectile);
        }

        // Adds the bigger, badder lazor
        private void AddBigProjectile(Vector2 position)
        { 
                BigProjectile bigProjectile = new BigProjectile();
                bigProjectile.Initialize(GraphicsDevice.Viewport, bigProjectileTexture, position);
                bigProjectiles.Add(bigProjectile);     
        }

        private void AddUpProjectile(Vector2 position)
        {
            UpProjectile upProjectile = new UpProjectile();
            upProjectile.Initialize(GraphicsDevice.Viewport, projectileTexture, position);
            upProjectiles.Add(upProjectile);
        }

        private void AddDownProjectile(Vector2 position)
        {
            DownProjectile downProjectile = new DownProjectile();
            downProjectile.Initialize(GraphicsDevice.Viewport, projectileTexture, position);
            downProjectiles.Add(downProjectile);
        }

        private void UpdateProjectiles()
        {
            
            // Update the Projectiles
            for (int i = projectiles.Count - 1; i >= 0; i--)
            {
                projectiles[i].Update();

                if (projectiles[i].Active == false)
                {
                    projectiles.RemoveAt(i);
                }
            }
        }

        private void UpdateUpProjectiles()
        {

            // Update the up Projectiles
            for (int i = upProjectiles.Count - 1; i >= 0; i--)
            {
                upProjectiles[i].Update();

                if (upProjectiles[i].Active == false)
                {
                    upProjectiles.RemoveAt(i);
                }
            }
        }

        private void UpdateDownProjectiles()
        {

            // Update the up Projectiles
            for (int i = downProjectiles.Count - 1; i >= 0; i--)
            {
                downProjectiles[i].Update();

                if (downProjectiles[i].Active == false)
                {
                    downProjectiles.RemoveAt(i);
                }
            }
        }

        private void UpdateLargeProjectiles()
        {
            
            for (int i = bigProjectiles.Count - 1; i >= 0; i--)
            {
                bigProjectiles[i].Update();

                if (bigProjectiles[i].Active == false)
                {
                    bigProjectiles.RemoveAt(i);
                }
            }
            
        }
        private void AddEnemy()
        {
            // Create the animation object
            Animation enemyAnimation = new Animation();

            // Initialize the animation with the correct animation information
            enemyAnimation.Initialize(enemyTexture, Vector2.Zero, 47, 61, 8, 30, Color.White, 1f, true);

            // Randomly generate the position of the enemy
            Vector2 position = new Vector2(GraphicsDevice.Viewport.Width + enemyTexture.Width / 2, random.Next(100, GraphicsDevice.Viewport.Height - 100));

            // Create an enemy
            Enemy enemy = new Enemy();

            // Initialize the enemy
            enemy.Initialize(enemyAnimation, position);

            // Add the enemy to the active enemies list
            enemies.Add(enemy);
        }

        private void UpdateEnemies(GameTime gameTime)
        {

            // Spawn a new enemy every 1.5 seconds
            if (gameTime.TotalGameTime - previousSpawnTime > enemySpawnTime)
            {
                previousSpawnTime = gameTime.TotalGameTime;

                // Add an Enemy
                AddEnemy();
            }

            // Update the Enemies
            for (int i = enemies.Count - 1; i >= 0; i--)
            {
                
                enemies[i].Update(gameTime);

                if (enemies[i].Active == false)
                {
                    // If not active and health <= 0
                    if (enemies[i].Health <= 0)
                    {
                        // Add an explosion
                        AddExplosion(enemies[i].Position);
                        // Play the explosion sound
                        explosionSound.Play();
                        //Add to the player's score
                        score += enemies[i].Value;
                    }
                    enemies.RemoveAt(i);
                }
            }
        }

        private void UpdateCollision()
        {
            // Use the Rectangle's built-in intersect function to 
            // determine if two objects are overlapping
            Rectangle rectangle1;
            Rectangle rectangle2;

            // Only create the rectangle once for the player
            rectangle1 = new Rectangle((int)player.Position.X,
            (int)player.Position.Y,
            player.Width,
            player.Height);

            // Do the collision between the player and the enemies
            for (int i = 0; i < enemies.Count; i++)
            {
                rectangle2 = new Rectangle((int)enemies[i].Position.X,
                (int)enemies[i].Position.Y,
                enemies[i].Width,
                enemies[i].Height);

                // Determine if the two objects collided with each
                // other
                if (rectangle1.Intersects(rectangle2))
                {
                    // Subtract the health from the player based on
                    // the enemy damage
                    player.Health -= enemies[i].Damage;

                    // Since the enemy collided with the player
                    // destroy it
                    enemies[i].Health = 0;

                    // If the player health is less than zero we died
                    if (player.Health <= 0)
                        player.Active = false;
                }

            }
            // Projectile vs Enemy Collision
            for (int i = 0; i < projectiles.Count; i++)
            {
                for (int j = 0; j < enemies.Count; j++)
                {
                    // Create the rectangles we need to determine if we collided with each other
                    rectangle1 = new Rectangle((int)projectiles[i].Position.X -
                    projectiles[i].Width / 2, (int)projectiles[i].Position.Y -
                    projectiles[i].Height / 2, projectiles[i].Width, projectiles[i].Height);

                    rectangle2 = new Rectangle((int)enemies[j].Position.X - enemies[j].Width / 2,
                    (int)enemies[j].Position.Y - enemies[j].Height / 2,
                    enemies[j].Width, enemies[j].Height);

                    // Determine if the two objects collided with each other
                    if (rectangle1.Intersects(rectangle2))
                    {
                        enemies[j].Health -= projectiles[i].Damage;
                        projectiles[i].Active = false;
                    }
                }
            }

            for (int i = 0; i < bigProjectiles.Count; i++)
            {
                for (int j = 0; j < enemies.Count; j++)
                {
                    // Create the rectangles we need to determine if we collided with each other
                    rectangle1 = new Rectangle((int)bigProjectiles[i].Position.X -
                    bigProjectiles[i].Width / 2, (int)bigProjectiles[i].Position.Y -
                    bigProjectiles[i].Height / 2, bigProjectiles[i].Width, bigProjectiles[i].Height);

                    rectangle2 = new Rectangle((int)enemies[j].Position.X - enemies[j].Width / 2,
                    (int)enemies[j].Position.Y - enemies[j].Height / 2,
                    enemies[j].Width, enemies[j].Height);

                    // Determine if the two objects collided with each other
                    if (rectangle1.Intersects(rectangle2))
                    {
                        enemies[j].Health -= bigProjectiles[i].Damage;
                        bigProjectiles[i].Active = false;
                    }
                }
            }

            for (int i = 0; i < upProjectiles.Count; i++)
            {
                for (int j = 0; j < enemies.Count; j++)
                {
                    // Create the rectangles we need to determine if we collided with each other
                    rectangle1 = new Rectangle((int)upProjectiles[i].Position.X -
                    upProjectiles[i].Width / 2, (int)upProjectiles[i].Position.Y -
                    upProjectiles[i].Height / 2, upProjectiles[i].Width, upProjectiles[i].Height);

                    rectangle2 = new Rectangle((int)enemies[j].Position.X - enemies[j].Width / 2,
                    (int)enemies[j].Position.Y - enemies[j].Height / 2,
                    enemies[j].Width, enemies[j].Height);

                    // Determine if the two objects collided with each other
                    if (rectangle1.Intersects(rectangle2))
                    {
                        enemies[j].Health -= upProjectiles[i].Damage;
                        upProjectiles[i].Active = false;
                    }
                }
            }

            for (int i = 0; i < downProjectiles.Count; i++)
            {
                for (int j = 0; j < enemies.Count; j++)
                {
                    // Create the rectangles we need to determine if we collided with each other
                    rectangle1 = new Rectangle((int)downProjectiles[i].Position.X -
                    downProjectiles[i].Width / 2, (int)downProjectiles[i].Position.Y -
                    downProjectiles[i].Height / 2, downProjectiles[i].Width, downProjectiles[i].Height);

                    rectangle2 = new Rectangle((int)enemies[j].Position.X - enemies[j].Width / 2,
                    (int)enemies[j].Position.Y - enemies[j].Height / 2,
                    enemies[j].Width, enemies[j].Height);

                    // Determine if the two objects collided with each other
                    if (rectangle1.Intersects(rectangle2))
                    {
                        enemies[j].Health -= downProjectiles[i].Damage;
                        downProjectiles[i].Active = false;
                    }
                }
            }
        }
        

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);

            // TODO: Add your drawing code here

            // Start drawing
            spriteBatch.Begin();

            spriteBatch.Draw(mainBackground, Vector2.Zero, Color.White);

            // Draw the moving background
            bgLayer1.Draw(spriteBatch);
            bgLayer2.Draw(spriteBatch);

            // Draw the Enemies
            for (int i = 0; i < enemies.Count; i++)
            {
                enemies[i].Draw(spriteBatch);
            }
            
            
            // Draw the Projectiles
            for (int i = 0; i < projectiles.Count; i++)
            {
                projectiles[i].Draw(spriteBatch);
            }

            // Draw the bigger Projectiles
            for (int i = 0; i < bigProjectiles.Count; i++)
            {
                bigProjectiles[i].Draw(spriteBatch);
            }

            for (int i = 0; i < upProjectiles.Count; i++)
            {
                upProjectiles[i].Draw(spriteBatch);
            }

            for (int i = 0; i < downProjectiles.Count; i++)
            {
                downProjectiles[i].Draw(spriteBatch);
            }
            

            // Draw the explosions
            for (int i = 0; i < explosions.Count; i++)
            {
                explosions[i].Draw(spriteBatch);
            }

            // Draw the score
            spriteBatch.DrawString(font, "score: " + score, new Vector2(GraphicsDevice.Viewport.TitleSafeArea.X, GraphicsDevice.Viewport.TitleSafeArea.Y), Color.White);
            // Draw the player health
            spriteBatch.DrawString(font, "health: " + player.Health, new Vector2(GraphicsDevice.Viewport.TitleSafeArea.X, GraphicsDevice.Viewport.TitleSafeArea.Y + 30), Color.White);

            // Draw the Player
            player.Draw(spriteBatch);

            // Stop drawing
            spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
