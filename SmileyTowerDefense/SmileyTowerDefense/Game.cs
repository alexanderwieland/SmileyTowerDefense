using System;
using System.Collections.Generic;
using System.Linq;
using System.Diagnostics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using System.Threading;
using System.IO;
using Microsoft.Xna.Framework.Net;

namespace SmileyTowerDefense
{
    public class Game : Microsoft.Xna.Framework.Game
    {
        #region Definitions 

        //VideoStuff
        //Video vid;
        //VideoPlayer vidPlayer;
        //Texture2D vidTexture;
        //Rectangle vidRect;

        //SPStuff
        Player spPlayer;

        //MenuStuff
        enum GameState
        {
            Video,
            MainMenu,
            HighScore,
            SinglePlayer,
            Multiplayer,
            Exiting
        }
        GameState CurrentGameState = GameState.MainMenu;
        int screenwidth = 640;
        int screenheight = 480;
        MenuButton buttonSP;
        MenuButton buttonMP;
        MenuButton buttonHS;
        MenuButton buttonEX;
        Texture2D menubackground;
        Rectangle menurec;
        Texture2D title;
        Rectangle titlerec;
        HighScore highscore;
        Texture2D hsbackground;

        //XNAGameStuff
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;        
        SpriteFont spritefont;
        SpriteFont spritefont2;

        //SplitScreenStuff
        Viewport mainViewport;
        Viewport leftViewport;
        Viewport rightViewport;
        Texture2D splitbar;

        //NetworkStuff
        NetworkSession networkSession;
        AvailableNetworkSessionCollection availableSessions;
        int selectedSessionIndex;
        PacketReader packetReader = new PacketReader();
        PacketWriter packetWriter = new PacketWriter();

        #endregion

        public Game()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;

            this.Window.Title = "Smiley Tower Defense";

            graphics.PreferredBackBufferWidth = screenwidth;
            graphics.PreferredBackBufferHeight = screenheight;
            graphics.ApplyChanges();

            // Add Gamer Services
            Components.Add(new GamerServicesComponent(this));
            // Respond to the SignedInGamer event
            SignedInGamer.SignedIn +=
                new EventHandler<SignedInEventArgs>(SignedInGamer_SignedIn);
        }

        void SignedInGamer_SignedIn(object sender, SignedInEventArgs e)
        {
            e.Gamer.Tag = new Player(Content, e.Gamer.Gamertag  + e.Gamer.ToString());
        }

        protected override void Initialize()
        {
            //vidPlayer = new VideoPlayer();
            base.Initialize();
        }

        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);

            IsMouseVisible = true;

            spritefont = Content.Load<SpriteFont>("Arial");
            spritefont2 = Content.Load<SpriteFont>("Arial2");

            buttonSP = new MenuButton(Content.Load<Texture2D>("MainMenu/SPButton"), graphics.GraphicsDevice);
            buttonMP = new MenuButton(Content.Load<Texture2D>("MainMenu/MPButton"), graphics.GraphicsDevice);
            buttonHS = new MenuButton(Content.Load<Texture2D>("MainMenu/HSButton"), graphics.GraphicsDevice);
            buttonEX = new MenuButton(Content.Load<Texture2D>("MainMenu/EXButton"), graphics.GraphicsDevice);

            buttonSP.SetPosition(new Vector2(60, 120));
            buttonMP.SetPosition(new Vector2(60, 180));
            buttonHS.SetPosition(new Vector2(60, 240));
            buttonEX.SetPosition(new Vector2(60, 300));

            menubackground = Content.Load<Texture2D>("MainMenu/MenuBG");
            menurec = new Rectangle(0, 0, screenwidth, screenheight);

            title = Content.Load<Texture2D>("MainMenu/Ueberschrift");
            titlerec = new Rectangle(60, 20, title.Width, title.Height);

            hsbackground = Content.Load<Texture2D>("MainMenu/HSBG");
            highscore = new HighScore(Content.Load<Texture2D>("MainMenu/Back"), graphics.GraphicsDevice);

            //spPlayer = new Player(Content, SignedInGamer.SignedInGamers[0].DisplayName);
            //spPlayer = new Player(Content, "Philo");

            //vid = Content.Load<Video>("Intro");
            //vidRect = new Rectangle(0, 0, screenwith, screenheight);
            //if(CurrentGameState == GameState.Video)
            //vidPlayer.Play(vid);
            
        }

        protected override void UnloadContent()
        {

        }

        private void HandleGameplayInput(Player player, GameTime gameTime)
        {
                UpdateInput(player, gameTime);
                networkSession.Update();
        }

        protected override void Update(GameTime gameTime)
        {
            MouseState mouse = Mouse.GetState();
            KeyboardState kstatee = Keyboard.GetState();

            switch (CurrentGameState)
            {
                case GameState.Video:
                    //if (vidPlayer.State == MediaState.Stopped)
                    CurrentGameState = GameState.MainMenu;
                    break;
                case GameState.MainMenu:
                    
                    if (buttonSP.isClicked == true)
                    {
                        spPlayer = new Player(Content, "Player1" );
                        CurrentGameState = GameState.SinglePlayer;
                        screenwidth = spPlayer.level.Width * 32;
                        screenheight = (spPlayer.level.Height + 1) * 32;
                        graphics.PreferredBackBufferWidth = screenwidth;
                        graphics.PreferredBackBufferHeight = screenheight;
                        graphics.ApplyChanges();
                    }
                    if (buttonMP.isClicked == true)
                    {
                        CurrentGameState = GameState.Multiplayer;
                        splitbar = Content.Load<Texture2D>("Game/SplitBalken");
                        spPlayer = new Player(Content, "Player1"); //spPlayer nur zwecks Abmessungen
                        screenwidth = spPlayer.level.Width * 32 * 2 +2 ;
                        screenheight = (spPlayer.level.Height + 1) * 32;
                        graphics.PreferredBackBufferWidth = screenwidth;
                        graphics.PreferredBackBufferHeight = screenheight;
                        graphics.ApplyChanges();

                        GraphicsDevice.Viewport = new Viewport(0, 0, screenwidth, screenheight);
                        graphics.ApplyChanges();
                        mainViewport = GraphicsDevice.Viewport;


                        leftViewport = mainViewport;
                        rightViewport = mainViewport;
                        leftViewport.Width = (leftViewport.Width / 2 ) -1;
                        rightViewport.Width = (rightViewport.Width / 2)-1 ;
                        rightViewport.X = leftViewport.Width  +2;

                        graphics.ApplyChanges();
                    }
                    if (buttonHS.isClicked == true)
                    {
                        highscore = new HighScore(Content.Load<Texture2D>("MainMenu/Back"), graphics.GraphicsDevice);
                        screenwidth = 300;
                        screenheight = 300;
                        graphics.PreferredBackBufferWidth = screenwidth;
                        graphics.PreferredBackBufferHeight = screenheight;
                        graphics.ApplyChanges();
                        CurrentGameState = GameState.HighScore;
                    }
                    if (buttonEX.isClicked == true) CurrentGameState = GameState.Exiting;

                    buttonSP.Update(mouse);
                    buttonMP.Update(mouse);
                    buttonHS.Update(mouse);
                    buttonEX.Update(mouse);

                    break;
                case GameState.SinglePlayer:
                    if (SignedInGamer.SignedInGamers[0].Gamertag != null && SignedInGamer.SignedInGamers[0].Gamertag != spPlayer.Name)
                        spPlayer.Name = SignedInGamer.SignedInGamers[0].Gamertag;
                    if (spPlayer.Lives <= 0 && spPlayer.Lives != -1)
                    {
                        spPlayer.Lost();
                        spPlayer.Lives = -1;
                    }
                    if(spPlayer.Lives > 0)
                        spPlayer.Update(gameTime, true);
                    
                    break;
                case GameState.Multiplayer:
                    if (!Guide.IsVisible)
                    {
                        foreach (SignedInGamer signedInGamer in
                            SignedInGamer.SignedInGamers)
                        {
                            Player player = signedInGamer.Tag as Player;

                            if (networkSession != null)
                            {
                                if (networkSession.SessionState ==
                                            NetworkSessionState.Lobby)
                                {
                                    HandleLobbyInput();

                                }
                                else
                                {
                                    if(player.Lives>0)
                                    HandleGameplayInput(player, gameTime);

                                }
                            }
                            else if (availableSessions != null)
                            {
                                HandleAvailableSessionsInput();
                            }
                            else
                            {
                                HandleTitleScreenInput();
                            }
                        }
                    }

                    break;
                case GameState.HighScore:
                    if (highscore.highscorebutton.isClicked == true)
                    {
                        SetMainMenuOptions();
                    }
                    highscore.Update(mouse);
                    break;
                case GameState.Exiting:
                    this.Exit();
                    break;
            }

            if (kstatee.IsKeyDown(Keys.Escape))
            {
                if (CurrentGameState == GameState.MainMenu)
                    CurrentGameState = GameState.Exiting;
                else
                {
                    SetMainMenuOptions();
                }
            }
            
            base.Update(gameTime);
            

        }

        protected override void OnExiting(object sender, EventArgs args)
        {
            if(CurrentGameState != GameState.MainMenu)
            CurrentGameState = GameState.MainMenu;

            
        }

        private void UpdateInput(Player player, GameTime gameTime)
        {
            foreach (LocalNetworkGamer gamer in networkSession.LocalGamers)
            {
                ReceiveNetworkData(gamer, gameTime);

                packetWriter.Write(player.Money);
                packetWriter.Write(player.Lives);

                packetWriter.Write(player.Towers.Count);

                if (player.Towers.Count >= 1)
                {
                    packetWriter.Write(player.Towers[player.Towers.Count-1].Position);      
                }

                packetWriter.Write(player.waveManager.CurrentWave.Enemies.Count);
                foreach (Enemy e in player.waveManager.CurrentWave.Enemies)
                {
                    packetWriter.Write(e.Position);
                }

                gamer.SendData(packetWriter, SendDataOptions.None);

                player.Update(gameTime, true);
            }

        }

        void ReceiveNetworkData(LocalNetworkGamer gamer, GameTime gameTime)
        {
            while (gamer.IsDataAvailable)
            {
                NetworkGamer sender;
                gamer.ReceiveData(packetReader, out sender);               

                if (!sender.IsLocal)
                {
                    Player player = sender.Tag as Player;

                    int money = packetReader.ReadInt32();
                    int lives = packetReader.ReadInt32();

                    player.Money = money;
                    player.Lives = lives;

                    int towers = packetReader.ReadInt32();
                    if (player.Towers.Count != towers)
                    {
                        Vector2 position = packetReader.ReadVector2();
                        player.NewTowerType = "ArrowTower";
                        player.AddMultiplayerTower(position);
                    }

                    int enemies = packetReader.ReadInt32();
                    if (player.waveManager.CurrentWave.Enemies.Count == enemies)
                    {
                        foreach (Enemy e in player.waveManager.CurrentWave.Enemies)
                        {
                            Vector2 position = packetReader.ReadVector2();
                            e.Position = position;
                        }
                    }

                    player.Update(gameTime, false);  
                }
            }           
        }

        private void DrawAvailableSessions()
        {
            GraphicsDevice.Clear(Color.White);
            spriteBatch.Begin();
            float y = 100;

            spriteBatch.DrawString(spritefont,
                "Available sessions (A=join, B=back)",
                new Vector2(101, y + 1), Color.Gray);
            spriteBatch.DrawString(spritefont,
                "Available sessions (A=join, B=back)",
                new Vector2(100, y), Color.Black);

            y += spritefont.LineSpacing * 2;

            int selectedSessionIndex = 0;

            for (
                int sessionIndex = 0;
                sessionIndex < availableSessions.Count;
                sessionIndex++)
            {
                Color color = Color.Black;

                if (sessionIndex == selectedSessionIndex)
                    color = Color.Black;

                spriteBatch.DrawString(spritefont,
                    availableSessions[sessionIndex].HostGamertag,
                    new Vector2(100, y), color);

                y += spritefont.LineSpacing;
            }
            spriteBatch.End();
        }

        protected void HandleAvailableSessionsInput()
        {
            KeyboardState keystate = Keyboard.GetState();

            if (keystate.IsKeyDown(Keys.A))
            {
                // Join the selected session.
                if (availableSessions.Count > 0)
                {
                    networkSession = NetworkSession.Join(
                        availableSessions[selectedSessionIndex]);
                    HookSessionEvents();

                    availableSessions.Dispose();
                    availableSessions = null;
                }
            }
            else if (keystate.IsKeyDown(Keys.Up))
            {
                // Select the previous session from the list.
                if (selectedSessionIndex > 0)
                    selectedSessionIndex--;
            }
            else if (keystate.IsKeyDown(Keys.Down))
            {
                // Select the next session from the list.
                if (selectedSessionIndex < availableSessions.Count - 1)
                    selectedSessionIndex++;
            }
            else if (keystate.IsKeyDown(Keys.B))
            {
                // Go back to the title screen.
                availableSessions.Dispose();
                availableSessions = null;

                SetMainMenuOptions();
            }

        }

        private void DrawTitleScreen()
        {
            GraphicsDevice.Clear(Color.White);
            string message = "";

            if (SignedInGamer.SignedInGamers.Count == 0)
            {
                message = "No profile signed in!  \n" +
                    "Press the Home key on the keyboard or \n" +
                    "the Xbox Guide Button on the controller to sign in.";
            }
            else
            {
                message += "Press A to create a new session\n" +
                    "X to search for sessions\nB to quit\n\n";
            }
            spriteBatch.Begin();
            spriteBatch.DrawString(spritefont, message,
                new Vector2(101, 101), Color.Gray);
            spriteBatch.DrawString(spritefont, message,
                new Vector2(100, 100), Color.Black);
            spriteBatch.End();
        }

        void DrawPlayer(Player player, Viewport viewport)
        {
            //GraphicsDevice.Clear(Color.CornflowerBlue);

            graphics.GraphicsDevice.Viewport = viewport;

            spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend);

            player.Draw(spriteBatch);            

            spriteBatch.End();
        }

        private void DrawGameplay(GameTime gameTime)
        {
            GraphicsDevice.Viewport = mainViewport;

            spriteBatch.Begin();
            spriteBatch.Draw(splitbar, new Rectangle(leftViewport.Width, 0, 2, screenheight), Color.Black);
            spriteBatch.End();

            Player player;
            if (networkSession != null)
            {                
                foreach (NetworkGamer networkGamer in networkSession.AllGamers)
                {
                    player = networkGamer.Tag as Player;
                    if (networkGamer.IsLocal)
                    {
                        DrawPlayer(player, leftViewport);
                        
                    }
                    else
                    {
                        DrawPlayer(player, rightViewport);
                    }                    
                }
                spriteBatch.Begin();
                foreach (NetworkGamer networkGamer in networkSession.AllGamers)
                {
                    player = networkGamer.Tag as Player;
                    if (networkGamer.IsLocal)
                    {
                        
                        if (player.Lives <= 0)
                            spriteBatch.DrawString(spritefont2, "Game Over", new Vector2(leftViewport.Width / 2 - 50, screenheight / 2 - 10), Color.White);
                    }
                    else
                    {
                        if (player.Lives <= 0)
                            spriteBatch.DrawString(spritefont2, "Game Over", new Vector2(rightViewport.Width / 2 - 50, screenheight / 2 - 10), Color.White);
                    }
                }
                spriteBatch.End();

            }

            //spriteBatch.Begin();
            //spriteBatch.Draw(splitbar, new Rectangle(leftViewport.Width, 0, 2, screenheight), Color.Transparent);
            //spriteBatch.End();
        }

        private void DrawLobby()
        {
            GraphicsDevice.Clear(Color.White);
            spriteBatch.Begin();
            float y = 100;

            spriteBatch.DrawString(spritefont2, "Lobby (A=ready, B=leave)",
                new Vector2(101, y + 1), Color.Gray);
            spriteBatch.DrawString(spritefont2, "Lobby (A=ready, B=leave)",
                new Vector2(101, y), Color.Black);

            y += spritefont2.LineSpacing * 2;

            foreach (NetworkGamer gamer in networkSession.AllGamers)
            {
                string text = gamer.Gamertag;

                Player player = gamer.Tag as Player;               

                if (gamer.IsReady)
                    text += " - ready!";
                
                spriteBatch.DrawString(spritefont2, text, new Vector2(170, y),
                    Color.Black);

                y += spritefont.LineSpacing + 64;
            }
            spriteBatch.End();
        }

        protected void HandleLobbyInput()
        {
            KeyboardState keystate = Keyboard.GetState();

            if (keystate.IsKeyDown(Keys.A))
            {
                foreach (LocalNetworkGamer gamer in networkSession.LocalGamers)
                    gamer.IsReady = true;
            }

            if (keystate.IsKeyDown(Keys.B))
            {
                networkSession.Dispose();
                networkSession = null;
                availableSessions = null;
                SetMainMenuOptions();
            }

            // The host checks if everyone is ready, and moves 
            // to game play if true.
            if (networkSession != null)
            {
                if (networkSession.IsHost)
                {
                    if (networkSession.IsEveryoneReady)
                        networkSession.StartGame();
                }
            }

            // Pump the underlying session object.
            //if (networkSession != null)
            networkSession.Update();
        }

        protected override void Draw(GameTime gameTime)
        {
            switch (CurrentGameState)
            {
                case GameState.Video:
                    //vidTexture = vidPlayer.GetTexture();
                    //spriteBatch.Begin();

                    //spriteBatch.Draw(vidTexture, vidRect, Color.White);

                    //spriteBatch.End();
                    break;
                case GameState.MainMenu:
                    spriteBatch.Begin();

                    spriteBatch.Draw(menubackground, menurec, Color.White);
                    spriteBatch.Draw(title, titlerec, Color.White);

                    buttonSP.Draw(spriteBatch);
                    buttonMP.Draw(spriteBatch);
                    buttonHS.Draw(spriteBatch);
                    buttonEX.Draw(spriteBatch);

                    spriteBatch.End();
                    break;
                case GameState.SinglePlayer:
                    spriteBatch.Begin();
                   
                    spPlayer.Draw(spriteBatch);

                    if (spPlayer.Lives <= 0)
                        spriteBatch.DrawString(spritefont2, "Game Over", new Vector2(screenwidth / 2 - 50, screenheight / 2 - 10), Color.White);

                    spriteBatch.End();
                    break;
                case GameState.Multiplayer:
                    if (networkSession != null)
                    {
                        if (networkSession.SessionState == NetworkSessionState.Lobby)
                            DrawLobby();
                        else
                            DrawGameplay(gameTime);
                    }
                    else if (availableSessions != null)
                    {
                        DrawAvailableSessions();
                    }
                    else
                    {
                        DrawTitleScreen();
                    }
            //        spriteBatch.Begin();
            //spriteBatch.Draw(splitbar, new Rectangle(leftViewport.Width, 0, 2, screenheight), Color.Transparent);
            //spriteBatch.End();
                    break;
                case GameState.HighScore:
                    spriteBatch.Begin();
                    spriteBatch.Draw(hsbackground, new Rectangle(0,0,300,300), Color.White);
                    highscore.Draw(spriteBatch, spritefont2);                
                    spriteBatch.End();
                    break;
                case GameState.Exiting:
                    
                    break;
            }

            base.Draw(gameTime);
        }

        private void SetMainMenuOptions()
        {
            CurrentGameState = GameState.MainMenu;
            buttonHS.isClicked = false;
            buttonMP.isClicked = false;
            buttonSP.isClicked = false;
            buttonEX.isClicked = false;
            highscore.highscorebutton.isClicked = false;
            screenwidth = 640;
            screenheight = 480;
            graphics.PreferredBackBufferWidth = screenwidth;
            graphics.PreferredBackBufferHeight = screenheight;
            graphics.ApplyChanges();
        }

        protected void HandleTitleScreenInput()
        {
            KeyboardState keystate = Keyboard.GetState();

            if (keystate.IsKeyDown(Keys.A))
            {
                CreateSession();
            }
            else if (keystate.IsKeyDown(Keys.X))
            {
                availableSessions = NetworkSession.Find(
                    NetworkSessionType.SystemLink, 2, null);

                selectedSessionIndex = 0;
            }
            else if (keystate.IsKeyDown(Keys.B))
            {
                SetMainMenuOptions();
            }
        }

        void CreateSession()
        {
            
            networkSession = NetworkSession.Create(
                NetworkSessionType.SystemLink,
                2, 8, 2,
                null);

            networkSession.AllowHostMigration = true;
            networkSession.AllowJoinInProgress = true;

            HookSessionEvents();
        }

        private void HookSessionEvents()
        {
            networkSession.GamerJoined +=
                new EventHandler<GamerJoinedEventArgs>(
                    networkSession_GamerJoined);
        }

        void networkSession_GamerJoined(object sender, GamerJoinedEventArgs e)
        {
            if (!e.Gamer.IsLocal)
            {
                e.Gamer.Tag = new Player(Content, e.Gamer.Gamertag + e.Gamer.ToString());
            }
            else
            {
                e.Gamer.Tag = GetPlayer(e.Gamer.Gamertag, e.Gamer.Gamertag + e.Gamer.ToString());
            }
        }

        Player GetPlayer(String gamertag, string name)
        {
            foreach (SignedInGamer signedInGamer in
                SignedInGamer.SignedInGamers)
            {
                if (signedInGamer.Gamertag == gamertag)
                {
                    return signedInGamer.Tag as Player;
                }
            }

            return new Player(Content, name);
        }

        
    }
}

