﻿namespace BytingLib
{
    public class InputRecordingTriggerer<T> : IUpdate where T : struct
    {
        private readonly KeyInput keys;
        private readonly InputRecordingManager<T> inputRecordingManager;
        private readonly string inputRecordingDir;
        private readonly Action<Action> onStartPlaying;

        public InputRecordingTriggerer(KeyInput devKeys, InputRecordingManager<T> inputRecordingManager, string inputRecordingDir, Action<Action> onStartPlaying, bool startRecordingInstantly)
        {
            keys = devKeys;
            this.inputRecordingManager = inputRecordingManager;
            this.inputRecordingDir = inputRecordingDir;
            this.onStartPlaying = onStartPlaying;
            if (inputRecordingDir != null && startRecordingInstantly)
                StartRecording();
        }

        public void Update()
        {
            if (keys.F5.Pressed)
            {
                if (keys.Shift.Down)
                    inputRecordingManager.ToggleRecording(GetNewRecordingFile());
                else
                {
                    string? file = GetLastRecordingFile();
                    if (file != null)
                    {
                        onStartPlaying(() =>
                        {
                            inputRecordingManager.TogglePlaying(file);
                        });
                    }
                }
            }
        }

        private string GetNewRecordingFile()
        {
            return Path.Combine(inputRecordingDir, DateTime.Now.ToString("yyyy.MM.dd_HH.mm.ss_fff") + ".inr");
        }

        private string? GetLastRecordingFile()
        {
            string[] files = Directory.GetFiles(inputRecordingDir, "*.inr");
            if (files.Length == 0)
                return null;

            Array.Sort(files);
            return files[0];
        }

        public void StartRecording()
        {
            inputRecordingManager.StartRecording(GetNewRecordingFile());
        }
    }

}
