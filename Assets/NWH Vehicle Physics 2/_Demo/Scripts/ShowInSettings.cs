namespace NWH.VehiclePhysics2.Demo
{
    [System.AttributeUsage(System.AttributeTargets.Field, AllowMultiple = false)]
    public class ShowInSettings : System.Attribute
    {
        public string name = null;
        public float min = 0f;
        public float max = 1f;
        public float step = 0.1f;

        public ShowInSettings(string name)
        {
            this.name = name;
        }

        public ShowInSettings(float min, float max, float step = 0.1f)
        {
            this.min = min;
            this.max = max;
            this.step = step;
        }

        public ShowInSettings(string name, float min, float max, float step = 0.1f)
        {
            this.name = name;
            this.min = min;
            this.max = max;
            this.step = step;
        }

        public ShowInSettings() {}
    }
}
