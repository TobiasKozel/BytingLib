﻿using BytingLib.Markup;

namespace BytingLib
{
    public class TextToTexture : IDisposable
    {
        private readonly SpriteBatch spriteBatch;
        private readonly FontArray fontArray;
        private readonly IShaderColor textEffect; // TODO: convert to shader?
        private readonly Creator markupCreator;
        private readonly float verticalSpaceBetweenLines;
        private readonly Dictionary<TextToTextureKey, AssetHolder<Texture2D>> textures = new();
        private readonly DisposableContainer disposables = new();
        public float MinimumPixelsPerUnit { get; }

        public TextToTexture(SpriteBatch spriteBatch, FontArray fontArray, IShaderColor textEffect, Creator markupCreator, float verticalSpaceBetweenLines, float minimumPixelsPerUnit)
        {
            this.spriteBatch = spriteBatch;
            this.fontArray = fontArray;
            this.textEffect = textEffect;
            this.markupCreator = markupCreator;
            this.verticalSpaceBetweenLines = verticalSpaceBetweenLines;
            MinimumPixelsPerUnit = minimumPixelsPerUnit;
        }

        public Ref<Texture2D> UseTexture(string text, Vector3 right, Color color, float? verticalSpaceBetweenLines = null)
        {
            var font = fontArray.GetFont(1f, out float actualFontSize);
            var markupSettings = new MarkupSettings(spriteBatch, font, Anchor.TopLeft(0, 0), Color.White);
            markupSettings.VerticalSpaceBetweenLines = verticalSpaceBetweenLines ?? this.verticalSpaceBetweenLines;
            var drawElement = new MarkupRoot(markupCreator, text);
            Vector2 textSize = drawElement.GetSize(markupSettings);
            textSize /= actualFontSize;

            int fontSize = GetRightFontSize(right.Length() * 2f /* because right only measures half the length */, 
                (int)MathF.Ceiling(textSize.X), MinimumPixelsPerUnit);

            return CreateTextTexture(text, fontArray.GetFont(fontSize), color, fontSize, verticalSpaceBetweenLines);
        }

        private static int GetRightFontSize(float spaceInMeters, int textureWidthOfScale1, float targetPixelsPerCM)
        {
            float pixelsPerMeter = textureWidthOfScale1 / spaceInMeters;

            float requiredFontScale = targetPixelsPerCM / pixelsPerMeter;

            int fontSize = (int)MathF.Ceiling(requiredFontScale);
            fontSize = Math.Min(8, Math.Max(1, fontSize));
            return fontSize;
        }

        public Ref<Texture2D> CreateTextTexture(string text, Ref<SpriteFont> font, Color color, float textureScale = 1, float? verticalSpaceBetweenLines = null)
        {
            if (textures.ContainsKey((text, font.Value, color, textureScale)))
                return textures[(text, font.Value, color, textureScale)].Use();

            var markupSettings = new MarkupSettings(spriteBatch, font, Anchor.TopLeft(0, 0), color /* color doesn't matter, it's overridden by the "Color" Parameter of the effect */);
            markupSettings.TextureScale = Vector2.One * textureScale;
            markupSettings.VerticalSpaceBetweenLines = verticalSpaceBetweenLines ?? this.verticalSpaceBetweenLines;
            var drawElement = new MarkupRoot(markupCreator, text);

            Vector2 textSize = drawElement.GetSize(markupSettings);

            var gDevice = spriteBatch.GraphicsDevice;
            RenderTarget2D tex = new RenderTarget2D(gDevice, (int)Math.Ceiling(textSize.X), (int)Math.Ceiling(textSize.Y), false, SurfaceFormat.Color, DepthFormat.None);

            var targets = gDevice.GetRenderTargets();
            gDevice.SetRenderTarget(tex);
            gDevice.Clear(Color.Transparent);

            using (textEffect.Color.Use(color.ToVector4()))
            {
                spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.NonPremultiplied, SamplerState.PointClamp, null, null, textEffect.Effect);

                drawElement.Draw(markupSettings);

                spriteBatch.End();
            }

            gDevice.SetRenderTargets(targets);

            disposables.Use(tex);
            AssetHolder<Texture2D> assetHolder = new AssetHolder<Texture2D>(tex, tex.Name, _ =>
            {
                if (!textures.Remove((text, font.Value, color, textureScale)))
                    throw new BytingException("couldn't remove a texture from TextToTexture.textures");
                tex.Dispose();
            });

            textures.Add((text, font.Value, color, textureScale), assetHolder);

            return assetHolder.Use();
        }

        public void Dispose()
        {
            disposables.Dispose();
        }
    }
}
