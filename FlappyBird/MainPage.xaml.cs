﻿using System;
using SkiaSharp;
using SkiaSharp.Views.Forms;
using Xamarin.Forms;

namespace FlappyBird
{
	public partial class MainPage : ContentPage
	{
		private readonly FlappyBirdGame game;
		private readonly SKSizeI baseSize;

		private float scale = 1;
		private SKPoint offset = SKPoint.Empty;

		public MainPage()
		{
			InitializeComponent();

			game = new FlappyBirdGame();
			baseSize = new SKSizeI(288, 512);

			SizeChanged += OnSizeChanged;

			_ = game.LoadContentAsync();
		}

		protected override void OnAppearing()
		{
			base.OnAppearing();

			game.Resize(baseSize.Width, baseSize.Height);

			gameSurface.InvalidateSurface();

			game.Start();
		}

		private void OnSizeChanged(object sender, EventArgs e)
		{
			game.Resize(baseSize.Width, baseSize.Height);
		}

		private void OnPaintSurface(object sender, SKPaintGLSurfaceEventArgs e)
		{
			game.Update();

			var canvas = e.Surface.Canvas;
			canvas.Clear(SKColors.Black);

			using var save = new SKAutoCanvasRestore(canvas, true);

			//RnD
			scale = Math.Min
			(
					(float)e.RenderTarget.Width /*e.BackendRenderTarget.Width*/ 
					/ baseSize.Width,
					(float)e.RenderTarget.Height/*e.BackendRenderTarget.Height*/ 
					/ baseSize.Height
			);

			SKRect screenRect = e.RenderTarget.Rect;/*e.BackendRenderTarget.Rect*/
			SKRect centeredRect = screenRect.AspectFit(baseSize);

			offset = centeredRect.Location;

			canvas.Translate(offset);
			canvas.Scale(scale);

			canvas.ClipRect(SKRect.Create(baseSize));

			game.Draw(canvas);
		}

		private void OnTouch(object sender, SKTouchEventArgs e)
		{
			var pos = e.Location;
			var x = (pos.X - offset.X) / scale;
			var y = (pos.Y - offset.Y) / scale;

			if (e.ActionType == SKTouchAction.Pressed)
			{
				game.TouchDown(new SKPointI((int)x, (int)y));
			}
			else if (e.ActionType == SKTouchAction.Released)
			{
				game.TouchUp(new SKPointI((int)x, (int)y));
				game.Tap(new SKPointI((int)x, (int)y));
			}

			e.Handled = true;
		}
	}
}
