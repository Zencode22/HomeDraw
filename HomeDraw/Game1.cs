using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using HomeDraw.Entities;
using HomeDraw.UI;
using HomeDraw.World;

namespace HomeDraw
{
    /// <summary>
    /// Main game class. Wires together the world (house), entities (pets +
    /// collisions), and UI (hud). All gameplay logic lives in dedicated classes.
    /// </summary>
    public class Game1 : Game
    {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;

        // Game subsystems
        private House _house;
        private readonly List<Pet> _pets = new List<Pet>();
        private GhostManager _ghostManager;
        private Hud _hud;

        // Screen size
        private const int ScreenWidth = 800;
        private const int ScreenHeight = 600;

        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;

            // Set window size
            _graphics.PreferredBackBufferWidth = ScreenWidth;
            _graphics.PreferredBackBufferHeight = ScreenHeight;
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            // Load textures
            Texture2D houseTexture = Content.Load<Texture2D>("house");
            Texture2D roofTexture = Content.Load<Texture2D>("roof");
            Texture2D doorTexture = Content.Load<Texture2D>("door");
            Texture2D grassTexture = Content.Load<Texture2D>("grass");
            Texture2D petTexture = Content.Load<Texture2D>("pet");

            // Load font
            SpriteFont font = Content.Load<SpriteFont>("spritefont");

            // Build the world
            _house = new House(houseTexture, roofTexture, doorTexture, grassTexture);

            // Pet 1: A/D move, Space jump — starts on the ground near the house
            _pets.Add(new Pet(petTexture, new Vector2(500, House.GroundY), Color.White,
                "Pet 1 (Brown)", Keys.A, Keys.D, Keys.Space, House.GroundY, ScreenWidth));

            // Pet 2: Left/Right move, Up jump — starts on the ground on the other side
            _pets.Add(new Pet(petTexture, new Vector2(600, House.GroundY), Color.Orange,
                "Pet 2 (Orange)", Keys.Left, Keys.Right, Keys.Up, House.GroundY, ScreenWidth));

            // Ghosts roam the open yard above the HUD strip; the manager spawns
            // 4 of them in random, non-overlapping locations.
            Rectangle ghostBounds = new Rectangle(0, 0, ScreenWidth, (int)House.GroundY - Ghost.Height);
            _ghostManager = new GhostManager(petTexture, ghostBounds);

            // Build the HUD
            _hud = new Hud(font, ScreenWidth);
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed ||
                Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            KeyboardState keyboard = Keyboard.GetState();

            // Once a pet has grabbed 10 ghosts, freeze play until Enter restarts.
            if (Pet.IsGameOver)
            {
                if (keyboard.IsKeyDown(Keys.Enter))
                    RestartRound();

                base.Update(gameTime);
                return;
            }

            // Update all pets with their own controls
            foreach (Pet pet in _pets)
            {
                pet.Update(gameTime, keyboard);
            }

            // Keep pets from overlapping each other
            CollisionManager.ResolvePetCollisions(_pets);

            // Move ghosts, bounce them off each other, and let pets grab them
            _ghostManager.Update(gameTime, _pets);

            base.Update(gameTime);
        }

        // Reset scores and re-spawn all ghosts for a fresh round.
        private void RestartRound()
        {
            Pet.ResetGame();
            foreach (Pet pet in _pets)
            {
                pet.ResetScore();
            }

            // Remove every ghost and spawn a new set of 4.
            foreach (Ghost ghost in new List<Ghost>(_ghostManager.Ghosts))
            {
                _ghostManager.RemoveGhost(ghost);
            }
            for (int i = 0; i < 4; i++)
            {
                _ghostManager.AddGhost();
            }
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            _spriteBatch.Begin();

            // Background layer: grass + house parts
            _house.Draw(_spriteBatch);

            // Entities: ghosts (wander the yard), then pets on top
            _ghostManager.Draw(_spriteBatch);

            foreach (Pet pet in _pets)
            {
                pet.Draw(_spriteBatch);
            }

            // UI: control instructions and title text
            _hud.Draw(_spriteBatch, _pets);

            _spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
