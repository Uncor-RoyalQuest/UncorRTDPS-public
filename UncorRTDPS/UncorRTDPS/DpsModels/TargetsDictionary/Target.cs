namespace UncorRTDPS.DpsModels.TargetsDictionary
{
    public enum TargetType
    { 
        Boss, Elite, Common 
    }

    public class Target
    {
        public int idInsideTargetType;
        public int idUnique;
        public TargetType targetType;
        public string originalName;
        public string searchFriendlyOriginalName;
        public long hp;

        public Target(int idInsideTargetType, TargetType targetType, string originalName, long hp)
        {
            this.idInsideTargetType = idInsideTargetType;
            this.targetType = targetType;
            this.originalName = originalName;
            this.searchFriendlyOriginalName = TargetNameTransformations.MakeTargetNameSearchFriendly_Regex(originalName);
            this.hp = hp;
        }
    }
}
