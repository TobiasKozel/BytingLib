﻿namespace BytingLib
{
    public class Screenshotter : IDisposable
    {
        private Texture2D? screenshotTex;
        private readonly GraphicsDevice gDevice;
        private readonly DefaultPaths paths;

        public Screenshotter(GraphicsDevice gDevice, DefaultPaths paths)
        {
            this.gDevice = gDevice;
            this.paths = paths;
        }

        public void TakeScreenshot(bool randomScreenshot)
        {
            int w = gDevice.PresentationParameters.BackBufferWidth;
            int h = gDevice.PresentationParameters.BackBufferHeight;
            int[] backBuffer = new int[w * h];
            gDevice.GetBackBufferData(backBuffer);
            if (screenshotTex == null || screenshotTex.Width != w || screenshotTex.Height != h)
                screenshotTex = new Texture2D(gDevice, w, h, false, gDevice.PresentationParameters.BackBufferFormat);
            screenshotTex.SetData(backBuffer);
            string path = randomScreenshot ? paths.GetNewRandomScreenshotPng() : paths.GetNewScreenshotPng();
            screenshotTex.SaveAsPng(path);
        }

        public void Dispose()
        {
            screenshotTex?.Dispose();
        }
    }
}
