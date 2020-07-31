using System.Globalization;
using UnityEngine;
using UnityEngine.UI;

// Source: https://forum.unity.com/threads/fpstotext-free-fps-framerate-calculator-with-options.463667/
namespace Performance
{
    /// <summary>
    /// <para>Pushes the Framerate value to a Text component.</para>
    /// </summary>
    public class FpsToText : MonoBehaviour
    {
        [Header("// Sample Groups of Data ")]
        public bool GroupSampling = true;
        public int SampleSize = 20;

        [Header("// Config ")]
        public Text TargetText;
        public int UpdateTextEvery = 1;
        public int MaxTextLength = 5;
        public bool Smoothed = true;
        public bool ForceIntResult = true;

        [Header("// System FPS (updates once/sec)")]
        public bool UseSystemTick = false;

        [Header("// Color Config ")]
        public bool UseColors = true;
        public Color Good = Color.green;
        public Color Okay = Color.yellow;
        public Color Bad = Color.red;
        public int OkayBelow = 60;
        public int BadBelow = 30;

        public float Framerate { get { return _fps; } }

        protected float[] FpsSamples;
        protected int SampleIndex;
        protected int TextUpdateIndex;
        private float _fps;

        private int _sysLastSysTick;
        private int _sysLastFrameRate;
        private int _sysFrameRate;

        protected virtual void Reset()
        {
            SampleSize = 20;
            UpdateTextEvery = 1;
            MaxTextLength = 5;
            Smoothed = true;
            UseColors = true;
            Good = Color.green;
            Okay = Color.yellow;
            Bad = Color.red;
            OkayBelow = 60;
            BadBelow = 30;
            UseSystemTick = false;
            ForceIntResult = true;
        }

        protected virtual void Start()
        {
            FpsSamples = new float[SampleSize];
            for (int i = 0; i < FpsSamples.Length; i++) FpsSamples[i] = 0.001f;
            if (!TargetText) enabled = false;
        }

        protected virtual void Update()
        {
            if (GroupSampling) Group();
            else SingleFrame();

            string fps = _fps.ToString(CultureInfo.CurrentCulture);

            SampleIndex = SampleIndex < SampleSize - 1 ? SampleIndex + 1 : 0;
            TextUpdateIndex = TextUpdateIndex > UpdateTextEvery ? 0 : TextUpdateIndex + 1;
            if (TextUpdateIndex == UpdateTextEvery) TargetText.text = fps.Substring(0, fps.Length < 5 ? fps.Length : 5);

            if (!UseColors) return;
            if (_fps < BadBelow)
            {
                TargetText.color = Bad;
                return;
            }
            TargetText.color = _fps < OkayBelow ? Okay : Good;
        }

        protected virtual void SingleFrame()
        {
            _fps = UseSystemTick
                ? GetSystemFramerate()
                : Smoothed ? 1 / Time.smoothDeltaTime : 1 / Time.deltaTime;
            if (ForceIntResult) _fps = (int)_fps;
        }

        protected virtual void Group()
        {
            FpsSamples[SampleIndex] = UseSystemTick
                ? GetSystemFramerate()
                : Smoothed ? 1 / Time.smoothDeltaTime : 1 / Time.deltaTime;

            _fps = 0;
            bool loop = true;
            int i = 0;
            while (loop)
            {
                if (i == SampleSize - 1) loop = false;
                _fps += FpsSamples[i];
                i++;
            }
            _fps /= FpsSamples.Length;
            if (ForceIntResult) _fps = (int)_fps;
        }

        protected virtual int GetSystemFramerate()
        {
            if (System.Environment.TickCount - _sysLastSysTick >= 1000)
            {
                _sysLastFrameRate = _sysFrameRate;
                _sysFrameRate = 0;
                _sysLastSysTick = System.Environment.TickCount;
            }
            _sysFrameRate++;
            return _sysLastFrameRate;
        }
    }
}