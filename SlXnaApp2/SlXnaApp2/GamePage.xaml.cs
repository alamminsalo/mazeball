using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Microsoft.Phone.Controls;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Devices.Sensors;

namespace SlXnaApp2
{
    public partial class GamePage : PhoneApplicationPage
    {
        public ContentManager contentManager;
        GameTimer timer;
        SpriteBatch spriteBatch;
        SpriteFont font1;
        Ball ball;
        GameObject goal;
        Vector2 startPos;
        GameObject[] hole;
        int level;

        Microsoft.Xna.Framework.Rectangle baselevel;

        String test = "";

        GameObject[] wall;

        Vector2 motionvec, impactdir;
        Motion motion;

        public GamePage()
        {
            InitializeComponent();

            level = 0;

            if (Motion.IsSupported)
            {
                // Get the content manager from the application
                contentManager = (Application.Current as App).Content;

                // Create a timer for this page
                timer = new GameTimer();
                timer.UpdateInterval = TimeSpan.FromTicks(166666);
                timer.Update += OnUpdate;
                timer.Draw += OnDraw;
            }
            else
            {
                Console.Write("Motion not supported");
            }
            
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            // Set the sharing mode of the graphics device to turn on XNA rendering
            SharedGraphicsDeviceManager.Current.GraphicsDevice.SetSharingMode(true);
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(SharedGraphicsDeviceManager.Current.GraphicsDevice);

            level = int.Parse(NavigationContext.QueryString["level"]);

            setupLevel(level);

            font1 = contentManager.Load<SpriteFont>("font1");

            motionvec = new Vector2(0, 0);

            if (motion == null)
            {
                motion = new Motion();
                motion.TimeBetweenUpdates = timer.UpdateInterval;
                motion.CurrentValueChanged += new EventHandler<SensorReadingEventArgs<MotionReading>>(motion_CurrentValueChanged);
            }
            try
            {
                motion.Start();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Couldn't start motion sensor");
            }
           
            // TODO: use this.content to load your game content here

            // Start the timer
            timer.Start();

            base.OnNavigatedTo(e);
        }

        private void setupLevel(int i)
        {
            if (i == 0)
            {
                baselevel = new Microsoft.Xna.Framework.Rectangle(0,0, scaledX(480), scaledY(800));
                startPos = new Vector2(60, 60);
                
                createBall();

                wall = new GameObject[16];

                wall[0] = new GameObject(0, 200, 300, 16, "wall", this);
                wall[1] = new GameObject(180, 400, 300, 16, "wall", this);
                wall[2] = new GameObject(0, 600, 300, 16, "wall", this);
               //
                hole = new GameObject[8];
               //
                hole[0] = new GameObject(400, 200, 54, 54, "ball", this);
                hole[1] = new GameObject(60, 360, 54, 54, "ball", this);
                hole[2] = new GameObject(200, 500, 54, 54, "ball", this);
                hole[3] = new GameObject(300, 720,  54, 54, "ball", this);

                goal = new GameObject(0, 600, 200, 200, "wall", this);

                return;
            }

            if (i == 1)
            {
                baselevel = new Microsoft.Xna.Framework.Rectangle(0, 0, spriteBatch.GraphicsDevice.DisplayMode.Width, spriteBatch.GraphicsDevice.DisplayMode.Height);
                startPos = new Vector2(scaledX(400), scaledY(50));

                createBall();

                wall = new GameObject[16];

                wall[0] = new GameObject(140, 200, 340, 30, "wall", this);
                wall[1] = new GameObject(140, 200, 30, 440, "wall", this);
                wall[2] = new GameObject(140, 640, 200, 30, "wall", this);
                wall[3] = new GameObject(340, 340, 30, 330, "wall", this);
                
                
                //
                hole = new GameObject[8];

                hole[0] = new GameObject(60, 100, 54, 54, "ball", this);
                hole[1] = new GameObject(20, 600, 54, 54, "ball", this);
                hole[2] = new GameObject(260, 420, 54, 54, "ball", this);
                hole[3] = new GameObject(380, 720, 54, 54, "ball", this);
                hole[4] = new GameObject(180, 300, 54, 54, "ball", this);
                hole[5] = new GameObject(80, 440, 54, 54, "ball", this);
                hole[6] = new GameObject(200, 740, 54, 54, "ball", this);

                goal = new GameObject(160, 540, 200,100, "wall", this);

                return;
            }

            if (i == 2)
            {
                baselevel = new Microsoft.Xna.Framework.Rectangle(0, 0, spriteBatch.GraphicsDevice.DisplayMode.Width, spriteBatch.GraphicsDevice.DisplayMode.Height);
                startPos = new Vector2(scaledX(200), scaledY(700));

                createBall();

                wall = new GameObject[16];

                wall[0] = new GameObject(120, 200, 30, 650, "wall", this);
                wall[1] = new GameObject(260, 360, 300, 500, "wall", this);
                wall[2] = new GameObject(120, 200, 300, 30, "wall", this);
                wall[3] = new GameObject(140, 0, 30, 50, "wall", this);


                //
                hole = new GameObject[8];

                hole[0] = new GameObject(30, 60, 54, 54, "ball", this);
                hole[1] = new GameObject(50, 300, 54, 54, "ball", this);
                hole[2] = new GameObject(260, 260, 42, 42, "ball", this);
                hole[3] = new GameObject(200, 80, 50, 50, "ball", this);
                hole[4] = new GameObject(400, 60, 54, 54, "ball", this);


                goal = new GameObject(0, 700, 120, 140, "wall", this);

                return;
            }

        }

        public int scaledX(int x)
        {
            return x * (int)ActualWidth / 480;
        }

        public int scaledY(int y)
        {
            return y * (int)ActualHeight / 800;
        }

        public float getAspectRatio()
        {
            return (float)(ActualWidth / ActualHeight);
        }

        private void createBall()
        {
            ball = new Ball(-99,-99, 48, 48, "ball", this);
            ball.setPos((int)startPos.X,(int)startPos.Y);
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            // Stop the timer
            timer.Stop();

            // Set the sharing mode of the graphics device to turn off XNA rendering
            SharedGraphicsDeviceManager.Current.GraphicsDevice.SetSharingMode(false);

            base.OnNavigatedFrom(e);
        }

        void motion_CurrentValueChanged(object sender, SensorReadingEventArgs<MotionReading> e)
        {
            // This event arrives on a background thread. Use BeginInvoke to call
            // CurrentValueChanged on the UI thread.
           // ball.resetTime();
            //Dispatcher.BeginInvoke(() => CurrentValueChanged(e.SensorReading));
        }
        /// <summary>
        /// Allows the page to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        private void OnUpdate(object sender, GameTimerEventArgs e)
        {
            ballLogic();
        }

        private void ballLogic()
        {
            float pitch = motion.CurrentValue.Attitude.Pitch;
            float roll = motion.CurrentValue.Attitude.Roll;

            impactdir = new Vector2(0, 0);

            Microsoft.Xna.Framework.Rectangle left = new Microsoft.Xna.Framework.Rectangle(ball.getBounds().Left, ball.getBounds().Center.Y - 7, 20, 15);
            Microsoft.Xna.Framework.Rectangle right = new Microsoft.Xna.Framework.Rectangle(ball.getBounds().Right - 20, ball.getBounds().Center.Y - 7, 20, 15);
            Microsoft.Xna.Framework.Rectangle top = new Microsoft.Xna.Framework.Rectangle(ball.getBounds().Center.X - 7, ball.getBounds().Top, 11, 20);
            Microsoft.Xna.Framework.Rectangle bottom = new Microsoft.Xna.Framework.Rectangle(ball.getBounds().Center.X - 7, ball.getBounds().Bottom - 20, 15, 20);

            if (goal.getBounds().Contains(ball.getBounds()))
            {
                MessageBox.Show("Great!");
                level += 1;
                setupLevel(level);
            }

            for (int i = 0; hole[i] != null; i++)
            {
                if (hole[i].getBounds().Contains(ball.getBounds().Center))
                {
                    createBall();
                }
            }

            //test = "NO HOLE";

           for (int i = 0; wall[i] != null; i++)
           {
               if (ball.getBounds().Intersects(wall[i].getBounds()))
               {
                   if (wall[i].getBounds().Intersects(left))
                       impactdir.X = -1;
                   else if (wall[i].getBounds().Intersects(right))
                       impactdir.X = 1;
                   if (wall[i].getBounds().Intersects(top))
                       impactdir.Y = -1;
                   else if (wall[i].getBounds().Intersects(bottom))
                       impactdir.Y = 1;
                   //test = "Touching!";
               }
           }

           if (!(baselevel.Contains(left)))
               impactdir.X = -1;
           else if (!(baselevel.Contains(right)))
               impactdir.X = 1;
           if (!(baselevel.Contains(top)))
               impactdir.Y = -1;
           else if (!(baselevel.Contains(bottom)))
               impactdir.Y = 1;

           ball.Move(roll, pitch, impactdir);
        }

        /// <summary>
        /// Allows the page to draw itself.
        /// </summary>
        private void OnDraw(object sender, GameTimerEventArgs e)
        {
            SharedGraphicsDeviceManager.Current.GraphicsDevice.Clear(Color.Wheat);

            spriteBatch.Begin();
            //spriteBatch.DrawString(font1, "X: " + ball.getPos().X, new Vector2(200, 400), Color.White);
            //spriteBatch.DrawString(font1, "Y: " + ball.getPos().Y, new Vector2(200, 500), Color.White);
            ////
            //spriteBatch.DrawString(font1, "X: " + scaledX(100), new Vector2(200, 200), Color.White);
            //spriteBatch.DrawString(font1, "Y: " + ball.getPos().Y, new Vector2(200, 300), Color.White);
            //spriteBatch.DrawString(font1,test, new Vector2(100, 100), Color.Red);

            spriteBatch.Draw(goal.getTex(), goal.getBounds(), Color.Tan);

            for (int i = 0; hole[i] != null; i++)
            {
                spriteBatch.Draw(hole[i].getTex(), hole[i].getBounds(), hole[i].getTex().Bounds, Color.Black);
            }

            for (int i=0; wall[i]!=null; i++)
            {
                spriteBatch.Draw(wall[i].getTex(), wall[i].getBounds(), Color.DarkGray);
            }


            //new Microsoft.Xna.Framework.Rectangle((int)wall.getStart().X, (int)wall.getStart().Y, wall.getLength(), 8)
            spriteBatch.Draw(ball.getTex(), ball.getBounds(),Color.White);

            spriteBatch.End();
            // TODO: Add your drawing code here
        }
    }

    public class GameObject
    {
        public Texture2D tex;
        public Microsoft.Xna.Framework.Rectangle bounds;
        public float x, y;

        public GameObject(int x, int y, int width, int height, string texstr, GamePage game)
        {
            this.tex = game.contentManager.Load<Texture2D>(texstr);
            this.x = game.scaledX(x); 
            this.y = game.scaledY(y);
            this.bounds = new Microsoft.Xna.Framework.Rectangle(game.scaledX(x), game.scaledY(y), game.scaledX(width), game.scaledY(height));
        }
        public Texture2D getTex()
        {
            return tex;
        }
        public Microsoft.Xna.Framework.Rectangle getBounds()
        {
            return bounds;
        }
        virtual public Vector2 getPos()
        {
            return new Vector2(x, y);
        }
        virtual public void setPos(int x, int y)
        {
            this.x = x;
            this.y = y;
        }
        public Vector2 getOrigin()
        {
            return new Vector2(bounds.Width/2, bounds.Height/2);
        }
    }


    public class Ball : GameObject
    {
        Vector2 pos;
        Vector2 velocity;

        public Ball(int x, int y, int width, int height, string texstr, GamePage game) : base(x,y,width,height,texstr,game)
        {
            pos = new Vector2(game.scaledX(x), game.scaledY(y));
            velocity = new Vector2(0);
            //bounds = new Microsoft.Xna.Framework.Rectangle(game.scaledX(x) - (int)getOrigin().X, game.scaledY(y) - (int)getOrigin().Y, game.scaledX(width), game.scaledY(height));
            //tex.Bounds.Inflate(getBounds().Width - tex.Bounds.Width, getBounds().Height - tex.Bounds.Height);
        }
        public void Move(float roll, float pitch, Vector2 impactdir)
        {
            if (impactdir.X == 0)
            {
               velocity.X += (float)Math.Sin(roll) * 9.81f/2;
             }
            else
            {
                velocity.X *= -0.3f;
                if (Math.Abs(velocity.X) < 1.5f)
                    velocity.X = 0;
                if (impactdir.X < 0)
                    pos.X += 1;
                else pos.X -= 1;
            }
           
            if (impactdir.Y == 0)
            {
              velocity.Y += (float)Math.Sin(pitch) * 9.81f/2;
            }
            else
            {
                velocity.Y *= -0.3f;
                if (Math.Abs(velocity.Y) < 1.5f)
                    velocity.Y = 0;
                if (impactdir.Y < 0)
                    pos.Y += 1;
                else pos.Y -= 1;
            }
            pos.X += velocity.X * 0.16f;
            pos.Y += velocity.Y * 0.16f;
            moveHitbox();
        }
        public override void setPos(int x, int y)
        {
            pos.X = x;
            pos.Y = y;
        }
        public override Vector2 getPos()
        {
            return pos;
        }
        public void setVel(Vector2 vel)
        {
            velocity = vel;
        }
        public Vector2 getVel()
        {
            return velocity;
        }
        private void moveHitbox()
        {
            this.bounds.X = (int)(pos.X - getOrigin().X);
            this.bounds.Y = (int)(pos.Y - getOrigin().Y);
        }
    }

   // public class Wall : GameObject
   // {
   //     //Texture2D tex;
   //     //Microsoft.Xna.Framework.Rectangle bounds;
   //     float angle;
   //
   //     public Wall(int x, int y, int width, int height, ref ContentManager contentManager, string texstr) : base(x,y,width,height,ref contentManager, texstr)
   //     {
   //         //tex = contentManager.Load<Texture2D>("wall");
   //         //bounds = new Microsoft.Xna.Framework.Rectangle(x,y,width,height);
   //         this.angle = angle;
   //     }
   //     public float getAngle()
   //     {
   //         return angle;
   //     }
   //     //public Texture2D getTex()
   //     //{
   //     //    return tex;
   //     //}
   //     //public Microsoft.Xna.Framework.Rectangle getBounds()
   //     //{
   //     //    return bounds;
   //     //}
   // }

    //public class Hole: GameObject
    //{
    //    //Texture2D tex;
    //    //Microsoft.Xna.Framework.Rectangle bounds;
    //    //int x, y;
    //    public Hole(int x, int y, int width, int height, string texstr, GamePage game) : base(x,y,width,height,texstr, game)
    //    {
    //        //this.x = x;
    //        //this.y = y;
    //        //tex = contentManager.Load<Texture2D>("hole");
    //        bounds = new Microsoft.Xna.Framework.Rectangle(game.scaledX(x), game.scaledY(y), width, height);
    //    }
    //    //public Vector2 getPos()
    //    //{
    //    //    return new Vector2(this.x, this.y);
    //    //}
    //    //public Texture2D getTex()
    //    //{
    //    //    return this.tex;
    //    //}
    //    //public Microsoft.Xna.Framework.Rectangle getBounds(){
    //    //    return this.bounds;
    //    //}
    //}

    //public class Goal : GameObject
    //{
    //
    //}
}