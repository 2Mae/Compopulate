namespace Compopulate
{

    [System.AttributeUsage(System.AttributeTargets.Field, AllowMultiple = true)]
    public class Att : System.Attribute
    {
        //using string flags since only primitives can be used as attribute parameters.
        public string[] flags;
        public Att()
        {
            flags = new string[0];
        }
        public Att(params string[] flags)
        {
            this.flags = flags;
        }
    }

    public static class Flags
    {
        public const string allowNull = "allowNull";
    }
}