﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace BytingLib
{
    public static class BaseGameFactory
    {
        public static IStuffDisposable CreateDefaultGame(Game game, GraphicsDeviceManager graphics, string? inputRecordingDir, out KeyInput keys, out MouseInput mouse, out GamePadInput gamePad, bool mouseWithActivationClick = true)
        {
            IStuffDisposable stuff = new StuffDisposable(typeof(IUpdate), typeof(IDraw));

            KeyInput keysDev;
            StructSource<FullInput> inputSource;
            InputRecordingManager<FullInput> inputRecordingManager;

            Func<MouseState> getMouseState;

            if (mouseWithActivationClick)
                getMouseState = Mouse.GetState;
            else
            {
                MouseWithoutActivationClick mouseFiltered = new MouseWithoutActivationClick(Mouse.GetState, f => game.Activated += f);
                getMouseState = mouseFiltered.GetState;
            }

            stuff.Add(inputSource = new StructSource<FullInput>(() => new FullInput(getMouseState(), Keyboard.GetState(), GamePad.GetState(0))));
            KeyInput _keys = new KeyInput(() => inputSource.Current.KeyState);
            stuff.Add(keys = _keys);
#if DEBUG
            stuff.Add(keysDev = new KeyInput(Keyboard.GetState));
#else
            stuff.Add(keysDev = new KeyInput(() => default));
#endif
            stuff.Add(mouse = new MouseInput(() => inputSource.Current.MouseState));
            stuff.Add(gamePad = new GamePadInput(() => inputSource.Current.GamePadState));
            stuff.Add(inputRecordingManager = new InputRecordingManager<FullInput>(stuff, inputSource, CreateInputRecorder, PlayInput));

            stuff.Add(new UpdateKeyPressed(keys, Keys.Escape, game.Exit));
            if (inputRecordingDir != null)
                stuff.Add(new InputRecordingTriggerer<FullInput>(keysDev, inputRecordingManager, inputRecordingDir));

            WindowManager windowManager = new WindowManager(true, game.Window, graphics);
            game.Window.AllowUserResizing = true;
            stuff.Add(new UpdateKeyPressed(keys, Keys.F11, windowManager.ToggleFullscreen));
            stuff.Add(new UpdateKeyPressed(keysDev, Keys.Right, windowManager.SwapScreen));
            stuff.Add(new UpdateKeyPressed(keysDev, Keys.Left, windowManager.SwapScreen));

            return stuff;
        }

        private static IDisposable CreateInputRecorder(StructSource<FullInput> inputSource, string path)
        {
            string dir = Path.GetDirectoryName(path)!;
            if (!Directory.Exists(dir))
                Directory.CreateDirectory(dir);

            FileStream fs = File.Create(path);
            StructStreamWriterCompressed<FullInput> recorder = new(fs);
            inputSource.OnUpdate += AddState;

            return new OnDispose(() =>
            {
                inputSource.OnUpdate -= AddState;
                recorder.Dispose();
                fs.Dispose();
            });

            void AddState(FullInput state) => recorder.AddState(state);
        }

        private static void PlayInput(StructSource<FullInput> inputSource, string path, Action onFinish)
        {
            FileStream fs = File.OpenRead(path);
            StructStreamReaderCompressed<FullInput> playback = new(fs);
            inputSource.SetSource(playback, _ =>
            {
                playback.Dispose();
                fs.Dispose();

                onFinish?.Invoke();
            });
        }
    }
}
