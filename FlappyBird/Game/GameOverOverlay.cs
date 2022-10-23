using FlappyBird.GameEngine;
using SkiaSharp;
using System;

namespace FlappyBird
{
	public class GameOverOverlay : Overlay
	{
		private const float FlashDuration = 1.0f;
		private static readonly InterperlatorDelegate FlashInterpolator = Animator.Interpolations.Decelerate;

		private const float InitialBounceOffset = -1.0f;
		private const float InitialBounceSpeed = -2.0f / 0.015f;
		private const float BounceAcceleration = 0.25f / 0.015f;

		private readonly Sprite gameOver;
		private readonly Sprite land;

		private readonly Animator fadeAnimator;

		private SKPoint position;
		private float offset;
		private float offsetSpeed;

		private readonly ButtonSprite playButton; // !

		public bool Finished { get; private set; } // !
		public bool Visible { get; private set; } //!

		public GameOverOverlay(Game game, SpriteSheet spriteSheet, bool hidden = true)
			: base(game, spriteSheet)//, hidden)
		{
			playButton = new ButtonSprite(SpriteSheet.Sprites[FlappyBirdSprites.button_play]);

			gameOver = SpriteSheet.Sprites[FlappyBirdSprites.text_game_over];
			land = SpriteSheet.Sprites[FlappyBirdSprites.land];

			fadeAnimator = new Animator();
		}

        
		public override void Show() // override
		{
			base.Show();

			fadeAnimator.Start(0f, 1f, FlashInterpolator, FlashDuration);

			offset = InitialBounceOffset;
			offsetSpeed = InitialBounceSpeed;

			Visible = true;
			Finished = false;
		}

		public override void Hide() // override
		{
			base.Hide();

			fadeAnimator.Start(1f, 0f, FlashInterpolator, FlashDuration);

			offset = 0;

			Finished = false;
		}
        

		public override void Resize(int width, int height)
		{
			base.Resize(width, height);

			position = new SKPoint
                ((width - gameOver.Size.Width) / 2f / 4.8f, 
                (height - land.Size.Height) / 3f / 4.8f
                );

			// the layout of this screen is:
			//  - the title at eye level at 1/3 the way down the screen
			//  - the ground resting on the bottom
			//  - the big buttons resting on the ground
			//  - the bird is centered in the space above the ground
			//  - the rate button is 2/3 in the space above the ground
			//  - the copyright is just below the grass

			var third = height / 3f;

			// title
            /*
			var titleTop = third - title.Size.Height / 2f;
			titlePos = new SKPoint(
			(
				width - title.Size.Width) / 2f / App.scale,
				titleTop / App.scale
			); // :)
            */

			

			// play buttons
			var buttonSpace = (width - playButton.Size.Width - 100) / 3f / App.scale;

			var bb = FlappyBirdGame.ButtonShadowBorder.Bottom;
			playButton.Location = new SKPoint
			(
				buttonSpace / App.scale - 200,  // - 200 added by me ;)
				200 / App.scale//(groundLevel - playButton.Size.Height + bb) / App.scale
			);

			buttonSpace += playButton.Size.Width + buttonSpace;
			
		}

		public override void Update(TimeSpan dt)
		{
			base.Update(dt);

			// the fade
			fadeAnimator.Update(dt);

			// the bounce
			if (offset < 0)
			{
				offset += (int)(offsetSpeed * dt.TotalSeconds);
				offsetSpeed += BounceAcceleration;
			}
			else
			{
				offset = 0;
			}

			// finished when both are done
			if (offset >= 0 && fadeAnimator.Finished)
			{
				Finished = true;
				if (fadeAnimator.Value == 0f)
					Visible = false;
			}
		}

		public override void Draw(SKCanvas canvas)
		{
			base.Draw(canvas);

			if (!Visible)
				return;

			var alpha = (byte)(fadeAnimator.Value * 255);
			gameOver.Draw(canvas, position.X, position.Y + offset, alpha);

			// play button 
			playButton.Draw(canvas);
		}

        /*
		public void Tap(SKPointI point) // override
		{
			//base.Tap(point);

			if (playButton.HitTest(point))
			{
				PlayTapped?.Invoke(this, EventArgs.Empty);
			}
			
		}
        */

		public event EventHandler PlayTapped;
	}
}
