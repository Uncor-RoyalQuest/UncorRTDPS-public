using System.Windows.Media.Imaging;

namespace UncorRTDPS.Services.DamageHistory
{
    public class Entity
    {
        public int Id { get; set; }
        public BitmapImage Img { get; set; }
        public string Name { get; set; }
        public string Damage { get; set; }
        public string DPS { get; set; }
        public string BattleDuration { get; set; }
        public string Hits { get; set; }
        public string MaxHitDmg { get; set; }
        public string TimeStart { get; set; }
        public string TimeEnd { get; set; }

        public Entity()
        {
        }

        public Entity(int id, BitmapImage img, string name, string damage, string dPS, string battleDuration, string hits, string maxHitDmg, string timeStart, string timeEnd)
        {
            Id = id;
            Img = img;
            Name = name;
            Damage = damage;
            DPS = dPS;
            BattleDuration = battleDuration;
            Hits = hits;
            MaxHitDmg = maxHitDmg;
            TimeStart = timeStart;
            TimeEnd = timeEnd;
        }
    }
}
