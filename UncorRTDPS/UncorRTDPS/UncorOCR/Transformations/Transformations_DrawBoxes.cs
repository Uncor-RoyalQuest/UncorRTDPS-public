using System.Collections.Generic;
using System.Drawing;
using UncorRTDPS.UncorOCR.DamageOCR_Data;

namespace UncorRTDPS.UncorOCR.Transformations
{
    public class Transformations_DrawBoxes
    {


        public static void DrawBoxAroundTargets(Bitmap bmp, List<ChatRowInfo> chatRowsInfo, int currentlyRowsCount)
        {
            Graphics g = Graphics.FromImage(bmp);
            Pen pen = new Pen(Color.Blue);
            ChatRowInfo row;
            for (int r = 0; r < currentlyRowsCount; r++)
            {
                row = chatRowsInfo[r];
                if (row.ignoreThisRow)
                    continue;

                g.DrawRectangle(pen, row.targetHorizStart, row.posStart, row.targetHorizEnd - row.targetHorizStart, row.posEnd - row.posStart);
            }
            g.Dispose();
        }


        public static void DrawVagueBoxesAroundFillerWords(Bitmap bmp, int averageWhiteSpace, int wordDamageLength, int wordTargetLength, List<ChatRowInfo> chatRowsInfo, int currentlyRowsCount)
        {
            Graphics g = Graphics.FromImage(bmp);
            Pen pen = new Pen(Color.Red);
            ChatRowInfo row;
            for (int r = 0; r < currentlyRowsCount; r++)
            {
                row = chatRowsInfo[r];
                if (row.ignoreThisRow)
                    continue;

                g.DrawRectangle(pen, row.damageHorizPosEnd + averageWhiteSpace / 2, row.posStart, wordDamageLength, row.posEnd - row.posStart);
                g.DrawRectangle(pen, row.damageHorizPosEnd + averageWhiteSpace + wordDamageLength, row.posStart, wordTargetLength, row.posEnd - row.posStart);
            }
            g.Dispose();
        }


        public static void DrawVagueBoxAroundFillers(Bitmap bmp, int fillerLength, int averageWhiteSpace, List<ChatRowInfo> chatRowsInfo, int currentlyRowsCount)
        {
            Graphics g = Graphics.FromImage(bmp);
            Pen pen = new Pen(Color.Red);
            ChatRowInfo row;
            for (int r = 0; r < currentlyRowsCount; r++)
            {
                row = chatRowsInfo[r];
                if (row.ignoreThisRow)
                    continue;

                g.DrawRectangle(pen, row.damageHorizPosEnd + averageWhiteSpace / 2, row.posStart, fillerLength, row.posEnd - row.posStart);
            }
            g.Dispose();
        }

        public static void DrawBoxesAroundDamage(Bitmap bmp, List<ChatRowInfo> chatRowsInfo, int currentlyRowsCount)
        {
            Graphics g = Graphics.FromImage(bmp);
            Pen pen = new Pen(Color.Green);
            ChatRowInfo row;
            for (int r = 0; r < currentlyRowsCount; r++)
            {
                row = chatRowsInfo[r];
                if (row.ignoreThisRow)
                    continue;

                g.DrawRectangle(pen, row.damageHorizPosStart, row.posStart, row.damageHorizPosEnd - row.damageHorizPosStart, row.posEnd - row.posStart);
            }
            g.Dispose();
        }

        public static void DrawBoxesAroundNames(Bitmap bmp, List<ChatRowInfo> chatRowsInfo, int currentlyRowsCount)
        {
            Graphics g = Graphics.FromImage(bmp);
            Pen pen = new Pen(Color.Green);
            ChatRowInfo row;
            for (int r = 0; r < currentlyRowsCount; r++)
            {
                row = chatRowsInfo[r];
                if (row.ignoreThisRow)
                    continue;

                g.DrawRectangle(pen, row.damageHorizPosEnd + 1, row.posStart, bmp.Width - (row.damageHorizPosEnd + 120), row.posEnd - row.posStart);
            }
            g.Dispose();
        }

    }
}
