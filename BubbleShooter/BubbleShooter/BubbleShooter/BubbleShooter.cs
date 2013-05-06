using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using BubbleShooter.Classes;

namespace BubbleShooter
{
    #region Delegates/enums
    public delegate void EventManager(Object sender);
    public delegate void ButtonEventManager();

    public enum BubbleColor
    {
        Pink=0, Yellow, Blue, Green, Gray, Black
    }

    public enum ButtonType
    {
        Save = 0, Exit, Sound
    }

    public enum DisplayType
    {
        Score = 0, HighScore, Difficulty, LevelUp
    }
    #endregion

    #region BubbleShooter
    public class BubbleShooter : Microsoft.Xna.Framework.Game
    {
        #region GameObjects
        GraphicsDeviceManager graphics;
        UIState uiState;
        InputState inputState;
        SpriteBatch spriteBatch;
        Texture2D borderTexture, bubbleTexture, backgroundTexture, arrowTexture, buttonTexture, displayTexture;
        SpriteFont scoreFont, textFont;
        Dictionary<int,GridBubble> gridBubbles = new Dictionary<int,GridBubble>();
        List<DeadBubble> deadBubbles = new List<DeadBubble>();
        List<Group> disposeGroups = new List<Group>();
        Dictionary<int, BubbleColor> availableColors = new Dictionary<int, BubbleColor>();
        List<Button> buttons = new List<Button>();
        List<Display> displays = new List<Display>();
        MovingBubble activeBubble;
        SoundEffect bubblePop, bubbleLaunch;
        Arrow arrow;
        CollisionChecker collisionChecker;
        GroupChecker groupChecker;
        String levelName ="default";
        Random r = new Random();
        #endregion

        #region Integers
        int _BALLSFIRED = 0;
        int _LEVELS = 1;
        int _STARTINGLEVEL = 1;
        int _CURRENTLEVEL = 1;
        int _TEXTUREMODE = 0;
        int _DIFFICULTY = 0;
        int _HIGHSCORE = 0;
        int _SCORE = 0;
        #endregion

        Vector2 scorePlacement = new Vector2(342, 12);
        Vector2 highScorePlacement = new Vector2(342, 42);
        Vector2 notification = new Vector2(20, 180);
        Vector2 soundButton = new Vector2(310, 390);
        Vector2 saveButton = new Vector2(310, 430);
        Boolean soundOn = false;
        Boolean easyMode = true;
        Boolean levelUp = false;

        #region Constants

        const int _DEFAULTTEXTUREMODE = 0;
        const int _BUBBLESFIREDMAX = 12;
        const int _WIDTH = 400;
        const int _HEIGHT = 470;
        const int _OFFSETLEFT = 10;
        const int _OFFSETRIGHT = -110;
        const int _OFFSETTOP = 10;
        const int _OFFSETBOTTOM = -24;
        const int _NUMBEROFBUBBLES = 6; // Number from 1 to 6 - there's only 6 types of bubble
        const int _MAXNUMBEROFBUBBLES = 6;

        #endregion

        #region Event Managers
        public event EventManager Click;
        #endregion

        #region Constructor
        public BubbleShooter()
        {
            graphics = new GraphicsDeviceManager(this);
            graphics.IsFullScreen = false;
            graphics.PreferredBackBufferWidth = _WIDTH;
            graphics.PreferredBackBufferHeight = _HEIGHT;
            IsMouseVisible = true;
            IsFixedTimeStep = false;
            Content.RootDirectory = "Content";
        }
        #endregion

        #region Initialization
        protected override void Initialize()
        {
            ReadInitialData();

            spriteBatch = new SpriteBatch(GraphicsDevice);
            uiState = new UIState(spriteBatch,Content);
            inputState = new InputState();
            collisionChecker = new CollisionChecker(gridBubbles);
            groupChecker = new GroupChecker(gridBubbles, deadBubbles);

            InitializeSides();

            // Initialize non-drawable game components
            base.Initialize();
            //LoadContent() kinda

        }

        #endregion

        #region Content PipeLine
        protected override void LoadContent()
        {
            scoreFont = Content.Load<SpriteFont>("Fonts\\Verdana");
            textFont = Content.Load<SpriteFont>("Fonts\\VerdanaText");
            try
            {
                buttonTexture = Content.Load<Texture2D>(String.Format("Textures\\Button_{0}", _TEXTUREMODE));
                arrowTexture = Content.Load<Texture2D>(String.Format("Textures\\Arrow_{0}", _TEXTUREMODE));
                backgroundTexture = Content.Load<Texture2D>(String.Format("Textures\\Background_{0}", _TEXTUREMODE));
                borderTexture = Content.Load<Texture2D>(String.Format("Textures\\Border_{0}", _TEXTUREMODE));
                bubbleTexture = Content.Load<Texture2D>(String.Format("Textures\\Bubblegum_{0}", _TEXTUREMODE));
                displayTexture = Content.Load<Texture2D>(String.Format("Textures\\Display_{0}", _TEXTUREMODE));
                bubblePop = Content.Load<SoundEffect>(String.Format("Sounds\\Pop_{0}", _TEXTUREMODE));
                bubbleLaunch = Content.Load<SoundEffect>(String.Format("Sounds\\Launch_{0}", _TEXTUREMODE));
            }
            catch (Exception)
            {
                buttonTexture = Content.Load<Texture2D>(String.Format("Textures\\Button_{0}", _DEFAULTTEXTUREMODE));
                arrowTexture = Content.Load<Texture2D>(String.Format("Textures\\Arrow_{0}", _DEFAULTTEXTUREMODE));
                backgroundTexture = Content.Load<Texture2D>(String.Format("Textures\\Background_{0}", _DEFAULTTEXTUREMODE));
                borderTexture = Content.Load<Texture2D>(String.Format("Textures\\Border_{0}", _DEFAULTTEXTUREMODE));
                bubbleTexture = Content.Load<Texture2D>(String.Format("Textures\\Bubblegum_{0}", _DEFAULTTEXTUREMODE));
                displayTexture = Content.Load<Texture2D>(String.Format("Textures\\Display_{0}", _DEFAULTTEXTUREMODE));
                bubblePop = Content.Load<SoundEffect>(String.Format("Sounds\\Pop_{0}", _DEFAULTTEXTUREMODE));
                bubbleLaunch = Content.Load<SoundEffect>(String.Format("Sounds\\Launch_{0}", _DEFAULTTEXTUREMODE));
            }



            Display d = new Display(uiState, displayTexture, textFont, notification, DisplayType.LevelUp, 260, 50, levelName, false);
            displays.Add(d);
            Button temp = new Button(inputState, uiState, buttonTexture, textFont, soundButton, ButtonType.Sound);
            temp.OnClick += ToggleSound;
            buttons.Add(temp);

            temp = new Button(inputState, uiState, buttonTexture, textFont, saveButton, ButtonType.Save);
            temp.OnClick += SaveHighScore;
            buttons.Add(temp);

            arrow = new Arrow(inputState, uiState, new Vector2((_WIDTH + _OFFSETLEFT + _OFFSETRIGHT) / 2, _HEIGHT + _OFFSETBOTTOM), arrowTexture);

            NewGame();
        }
        protected override void UnloadContent()
        {
        }
        #endregion

        #region Update
        protected override void Update(GameTime gameTime)
        {
            #region InputUpdate

            if (this.IsActive)
            {
                inputState.Update();
            }

            if (inputState.Clicking())
            {
                if (Click != null && MouseIsInWindow())
                {
                    Click(this);
                }
            }

            foreach (Button b in buttons)
            {
                b.Update();
            }

            #region Animation updates

            arrow.Update();

            if (deadBubbles.Count != 0)
            {

                foreach (DeadBubble b in deadBubbles)
                {
                    if (b.Exploded)
                    {
                        if (b.DeadTimer > DeadBubble._MAXDEADTIMEREXPL && !disposeGroups.Contains(b.Group))
                        {
                            disposeGroups.Add(b.Group);
                        }
                    }
                    else
                        if (b.DeadTimer > DeadBubble._MAXDEADTIMERNOTEXPL && !disposeGroups.Contains(b.Group))
                        {
                            disposeGroups.Add(b.Group);
                        }
                    b.Update(gameTime);
                }
            }

            if (disposeGroups.Count != 0)
            {
                foreach (Group g in disposeGroups)
                {
                    groupChecker.DestroyMembers(g);
                }
            }

            #endregion

            if (!levelUp)
            {
                IsMouseVisible = easyMode ? true : !MouseIsInWindow();
                if (inputState.KeyboardState.IsKeyDown(Keys.Space))
                    this.Exit();

            #endregion           // UPDATE MOUSE & KEYBOARD

                activeBubble.Update(gameTime);

                if (activeBubble.IsLaunched && !activeBubble.IsMoving)
                {
                    Vector2 temp = activeBubble.SetIntoGrid(this);
                    int tempInt = (int)temp.X + Bubble._BUBBLESEVENLAYER * (int)temp.Y - (Bubble._BUBBLESEVENLAYER - Bubble._BUBBLESUNEVENLAYER) * (int)temp.Y / 2;
                    // Change 2d position to position in a semi - continious chain
                    GridBubble tempBubble = new GridBubble(activeBubble);
                    try
                    {
                        gridBubbles.Add(tempInt, tempBubble);
                    }
                    catch (Exception e)
                    {
                        System.Diagnostics.Debug.WriteLine(e);
                    }
                    // Add the active bubbles to the stopped bubbles list - try catch for adding bubbles at existing spots, certain bugs just aren't filtered out yet.

                    groupChecker.ClearStoppedGroups();

                    // Clear any groups to avoid confusion
                    tempBubble.Group = new Group(tempInt, tempBubble);
                    groupChecker.CheckColor(tempInt, tempBubble);
                    // Create a new group for the freshly stopped Bubble and make it add connected same color bubbles

                    AdjustScore(-10);
                    _BALLSFIRED++;

                    if (tempBubble.Group.NumberOfGroupMembers > 2)
                    {
                        if (soundOn)
                        {
                            bubblePop.Play();
                        }

                        groupChecker.KillMembers(tempBubble.Group, this, true);
                        groupChecker.CheckAll();
                        groupChecker.KillFlying(this);

                        _BALLSFIRED--;
                        if (gridBubbles.Count == 0)
                        {
                            _BALLSFIRED = 0;
                            if (_CURRENTLEVEL != _LEVELS)
                            {
                                levelUp = true;
                                IsMouseVisible = true;
                                foreach (Display d in displays)
                                {
                                    if (d.DisplayType == DisplayType.LevelUp)
                                    {
                                        d.Visible = true;
                                        d.StringMessage = levelName;
                                     }
                                }
                                Click += LevelUp;
                            }
                            else
                            {
                                NewGame();
                            }
                        }
                        else
                            DetermineBubbleColors(); 
                    }
                    else
                    {
                        if (temp.Y >= 17)
                            NewGame();                        
                    }

                    if (_DIFFICULTY >= 0)
                    {
                        if (_BALLSFIRED > _BUBBLESFIREDMAX / (_DIFFICULTY + 1))
                        {
                            if (soundOn)
                            {
                                bubbleLaunch.Play();
                            }
                            _BALLSFIRED = 0;
                            AddLine();

                        }
                    }

                    activeBubble = MakeNewBubble();
                    if (!levelUp)
                    {
                        Click += activeBubble.LaunchBubble;
                        Click += LaunchBubble;
                    }
                }
            }

        }
        #endregion

        #region Draw
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            spriteBatch.Begin();
            spriteBatch.Draw(backgroundTexture, Vector2.Zero, Color.White);

            if (deadBubbles.Count != 0)
            {
                foreach (DeadBubble b in deadBubbles)
                {
                    b.Draw();
                }
            }

            if (gridBubbles.Count != 0)
            {
                foreach (KeyValuePair<int,GridBubble> kvp in gridBubbles)
                {
                    kvp.Value.Draw();
                }
            }



            spriteBatch.Draw(borderTexture, Vector2.Zero, Color.White);
            if (activeBubble != null)
            {
                activeBubble.Draw();
            }
            arrow.Draw();

            foreach (Button b in buttons)
            {
                b.Draw();
            }

            spriteBatch.DrawString(scoreFont, _SCORE.ToString(), scorePlacement, Color.White);
            spriteBatch.DrawString(scoreFont, _HIGHSCORE.ToString(), highScorePlacement, Color.White);

            foreach (Display d in displays)
            {
                d.Draw();
            }
            spriteBatch.End();
        }

        #endregion

        #region Bubble methods

        private void DetermineBubbleColors()
        {
            int k = 0;
            availableColors.Clear();
            foreach (KeyValuePair<int, GridBubble> kvp in gridBubbles)
            {
                if (!availableColors.ContainsValue(kvp.Value.BubbleColor))
                {
                    availableColors.Add(k, kvp.Value.BubbleColor);
                    k++;
                }


                if (availableColors.Count == 6)
                    break;

            }
        }

        private MovingBubble MakeNewBubble()
        {
            int temp = r.Next(0, availableColors.Count);
            return new MovingBubble(uiState, inputState,collisionChecker, new Vector2((_WIDTH +_OFFSETLEFT +_OFFSETRIGHT)/2, _HEIGHT+_OFFSETBOTTOM), new Vector2(0, 0), bubbleTexture, availableColors[temp]);
        }

        private GridBubble MakeGridBubble(int i, BubbleColor bubbleColor)
        {
            if ((int)bubbleColor >= 0 && (int)bubbleColor < _MAXNUMBEROFBUBBLES)
            {
                return new GridBubble(uiState, inputState, Bubble.GetGrid(i, this), bubbleTexture, bubbleColor);
            }
            else return null;
        }

        private GridBubble MakeGridBubble(int i)
        {
            int temp = r.Next(0, availableColors.Count);
            return MakeGridBubble(i, availableColors[temp]);
        }

        #endregion

        #region DataHandling

        private void LoadLevel(int i)
        {
            int[] tempArray = ReadData(i);
            if (tempArray != null)
            {
                for (int k = 0; k < tempArray.Length; k++)
                {
                    GridBubble b = MakeGridBubble(k, (BubbleColor)tempArray[k]);
                        if (b != null)
                        {
                            gridBubbles.Add(k, b);
                        }
                }
            }
            DetermineBubbleColors();
            activeBubble = MakeNewBubble();
            Click += activeBubble.LaunchBubble;
            Click += LaunchBubble;
        }

        private int[] ReadData(int i)
        {
            int[] tempArray;
            gridBubbles.Clear();

            try
            {
                using (StreamReader reader = new StreamReader(String.Format("Levels\\Level_{0}.txt", i)))
                {
                    levelName = reader.ReadLine();
                    String temp = reader.ReadLine();

                    String[] bubblebits = temp.Split(' ');
                    int k = 0;
                    tempArray = new int[bubblebits.Length];

                    foreach (String str in bubblebits)
                    {
                        tempArray[k] = int.Parse(str);
                        k++;
                    }
                }

                return tempArray;
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine(e.ToString());
            }

            if (levelName == null)
                levelName = "_FailedToLoadError";
            return null;
        }

        private void SaveHighScore()
        {
            if (_HIGHSCORE < _SCORE)
            {
                _HIGHSCORE = _SCORE;;
                try
                {
                    using (StreamWriter writer = new StreamWriter("Highscore.txt"))
                    {

                        writer.WriteLine("[highscore]");
                        writer.WriteLine(_HIGHSCORE);
                    }
                }
                catch (Exception e)
                {
                    System.Diagnostics.Debug.WriteLine(e.ToString());
                }
            }
        }

        private void ReadInitialData()
        {
            try
            {
                using (StreamReader reader = new StreamReader("Levels\\Levels.txt"))
                {
                    string temp;
                    do
                    {
                        temp = reader.ReadLine();
                        System.Diagnostics.Debug.WriteLine(temp);
                        if (temp == "[levels_#]")
                            _LEVELS = int.Parse(reader.ReadLine());
                        if (temp == "[startingLevel_#]")
                            _STARTINGLEVEL = int.Parse(reader.ReadLine());
                    }
                    while (temp != null);
                    _CURRENTLEVEL = _STARTINGLEVEL;
                }
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine(e.ToString());
            }

            try
            {
                using (StreamReader reader = new StreamReader("Settings.txt"))
                {
                    string temp;
                    do
                    {
                        temp = reader.ReadLine();
                        System.Diagnostics.Debug.WriteLine(temp);
                        if (temp == "[textureMode]")
                            _TEXTUREMODE = int.Parse(reader.ReadLine());
                        if (temp == "[difficulty]")
                            _DIFFICULTY = int.Parse(reader.ReadLine());
                        if (temp == "[easyModeOn]")
                            easyMode = bool.Parse(reader.ReadLine());
                    }
                    while (temp != null);
                }
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine(e.ToString());
            }

            try
            {
                using (StreamReader reader = new StreamReader("Highscore.txt"))
                {
                    string temp;
                    do
                    {
                        temp = reader.ReadLine();
                        System.Diagnostics.Debug.WriteLine(temp);
                        if (temp == "[highscore]")
                            _HIGHSCORE = int.Parse(reader.ReadLine());
                    }
                    while (temp != null);
                }
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine(e.ToString());
            }
        }
        #endregion

        #region Parameters
        private Boolean MouseIsInWindow()
        {
            return inputState.X > _OFFSETLEFT && inputState.X < _WIDTH + _OFFSETRIGHT && inputState.Y > _OFFSETTOP && inputState.Y < _HEIGHT+_OFFSETBOTTOM;
        }

        public void AdjustScore(int i)
        {
            _SCORE += i;
        }

        public int Width
        {
            get { return _WIDTH; }
        }

        public int Height
        {
            get { return _HEIGHT; }
        }

        public int OffsetLeft
        {
            get { return _OFFSETLEFT; }
        }

        public int OffsetTop
        {
            get { return _OFFSETTOP; }
        }
        #endregion Parameters

        public void ToggleSound()
        {
            soundOn = soundOn ? false : true;
        }

        private void InitializeSides()
        {
            Side left = new Side(_OFFSETLEFT, new Vector2(1, 0));
            collisionChecker.AddSide(left);
            SpeedSide top = new SpeedSide(_OFFSETTOP, new Vector2(0, 1));
            collisionChecker.AddSide(top);
            Side right = new Side(_WIDTH + _OFFSETRIGHT, new Vector2(-1, 0));
            collisionChecker.AddSide(right);
        }

        private void LevelUp(Object o)
        {
            levelUp = false;
            foreach (Display d in displays)
            {
                if (d.DisplayType == DisplayType.LevelUp)
                    d.Visible = false;
            }
            _CURRENTLEVEL++;
            LoadLevel(_CURRENTLEVEL);
            Click -= LevelUp;
        }
        private void NewGame()
        {
            SaveHighScore();
            _BALLSFIRED = 0;
            _SCORE = 0;
            _CURRENTLEVEL = _STARTINGLEVEL;
            LoadLevel(_CURRENTLEVEL);               
        }

        public void LaunchBubble(Object o)
        {
            if (soundOn)
            {
                bubbleLaunch.Play();
            }
            Click -= LaunchBubble;
        }

        public void AddLine()
        {
            Dictionary<int, GridBubble> temp = new Dictionary<int, GridBubble>(gridBubbles);
            Boolean over = false;
            gridBubbles.Clear();
            try
            {
                for (int i = 0; i < Bubble._BUBBLESUNEVENLAYER; i++)
                {
                    gridBubbles.Add(i, MakeGridBubble(i));
                }
                foreach (KeyValuePair<int, GridBubble> kvp in temp.OrderBy(k => k.Key))
                {
                    if (kvp.Key % Bubble._BUBBLESBOTHLAYERS < Bubble._BUBBLESUNEVENLAYER && gridBubbles.ContainsKey(kvp.Key))
                    {
                        gridBubbles.Add(kvp.Key + Bubble._BUBBLESEVENLAYER, MakeGridBubble(kvp.Key + Bubble._BUBBLESEVENLAYER, kvp.Value.BubbleColor));
                        over = over ? true : kvp.Key + Bubble._BUBBLESEVENLAYER + Bubble._BUBBLESUNEVENLAYER > Bubble._BUBBLESBOTHLAYERS * 9;
                    }
                    else
                    {
                        if (kvp.Key % Bubble._BUBBLESBOTHLAYERS > Bubble._BUBBLESUNEVENLAYER && gridBubbles.ContainsKey(kvp.Key))
                        {
                            gridBubbles.Add(kvp.Key + Bubble._BUBBLESUNEVENLAYER, MakeGridBubble(kvp.Key + Bubble._BUBBLESUNEVENLAYER, kvp.Value.BubbleColor));
                            over = over ? true : kvp.Key + Bubble._BUBBLESUNEVENLAYER * 2 > Bubble._BUBBLESBOTHLAYERS * 9;
                        }
                        else
                        {
                            gridBubbles.Add(kvp.Key, MakeGridBubble(kvp.Key, kvp.Value.BubbleColor));
                        }
                    }
                }
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine(e);
            }
            if (over) NewGame();

        }

    }
#endregion
}
