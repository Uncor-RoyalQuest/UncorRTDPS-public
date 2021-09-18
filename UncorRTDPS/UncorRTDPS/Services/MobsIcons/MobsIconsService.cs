using System.Collections.Generic;
using System.Windows.Media.Imaging;
using UncorRTDPS.DpsModels.TargetsDictionary;

namespace UncorRTDPS.Services.MobsIcons
{
    class MobsIconsService : Service
    {
        private Dictionary<string, BitmapImage>
            bosses = null,
            elites = null;

        public BitmapImage GetMobImage(Target target)
        {
            if (bosses == null || elites == null || target == null)
                return null;

            switch (target.targetType)
            {
                case TargetType.Boss:
                    if (bosses.ContainsKey(target.originalName))
                        return bosses[target.originalName];
                    return null;
                case TargetType.Elite:
                    if (elites.ContainsKey(target.originalName))
                        return elites[target.originalName];
                    return null;
            }
            return null;
        }

        public ServiceResponseStatus CloseService()
        {
            bosses.Clear();
            elites.Clear();
            return ServiceResponseStatus.OK;
        }

        /// <summary>
        /// param[0] = bosses linker file
        /// param[1] = bosses icons
        /// param[2] = elites linker file
        /// param[3] = elites icons
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public ServiceResponseStatus InitService(string[] param)
        {
            if (param.Length < 4)
                return ServiceResponseStatus.FAILED;
            ImgMobNameDictionaryLoader imgMobNameDictionaryLoader = new ImgMobNameDictionaryLoader();
            string bossesLinkerFile = param[0];
            string bossesIconsFolder = param[1];
            bosses = imgMobNameDictionaryLoader.LoadDictionary(bossesLinkerFile, bossesIconsFolder);
            string elitesLinkerFile = param[2];
            string elitesIconsFolder = param[3];
            elites = imgMobNameDictionaryLoader.LoadDictionary(elitesLinkerFile, elitesIconsFolder);
            return ServiceResponseStatus.OK;
        }
    }
}
