using UncorRTDPS.Util;

namespace UncorRTDPS.Screencap
{
    public class SelectedArea
    {
        public bool IsTopLeftSet { get; private set; } = false;
        public int X_topLeft { get; set; }
        public int Y_topLeft { get; set; }

        public bool IsBotRightSet { get; private set; } = false;
        public int X_botRight { get; set; }
        public int Y_botRight { get; set; }

        public int Width { get; set; }
        public int Height { get; set; }

        public enum Corners
        {
            TopLeft, TopRight, BotLeft, BotRight, None
        }

        public SelectedArea() { }

        public SelectedArea(SelectedArea sa)
        {
            int X_tl = sa.X_topLeft;
            int Y_tl = sa.Y_topLeft;

            int X_br = sa.X_botRight;
            int Y_br = sa.Y_botRight;

            if (X_tl + X_br == 0 && Y_tl + Y_br == 0)
                return;

            SetTopLeft(X_tl, Y_tl);
            SetBotRight(X_br, Y_br);
        }

        public SelectedArea(int X_tl, int Y_tl, int X_br, int Y_br)
        {
            if (X_tl + X_br == 0 && Y_tl + Y_br == 0)
                return;

            SetTopLeft(X_tl, Y_tl);
            SetBotRight(X_br, Y_br);
        }

        public SelectedArea(string X_tl, string Y_tl, string X_br, string Y_br)
        {
            int?
                iX_tl = SInt.FromString(X_tl),
                iY_tl = SInt.FromString(Y_tl),
                iX_br = SInt.FromString(X_br),
                iY_br = SInt.FromString(Y_br);

            if (iX_tl == null || iY_tl == null || iX_br == null || iY_br == null)
            {
                return;
            }

            if (iX_tl.Value + iX_br.Value == 0 && iY_tl.Value + iY_br.Value == 0)
                return;

            SetTopLeft(iX_tl.Value, iY_tl.Value);
            SetBotRight(iX_br.Value, iY_br.Value);
        }

        public void SetTopLeft(int X, int Y)
        {
            X_topLeft = X;
            Y_topLeft = Y;
            IsTopLeftSet = true;
            if (IsTopLeftSet && IsBotRightSet)
            {
                //if (!areCornersCorrectlyCorresponding(X_topLeft, Y_topLeft, X_botRight, Y_botRight))
                //    correctPoints();
                UpdateWidthHeight();
            }
        }

        public void SetBotRight(int X, int Y)
        {
            X_botRight = X;
            Y_botRight = Y;
            IsBotRightSet = true;
            if (IsTopLeftSet && IsBotRightSet)
            {
                //if (!areCornersCorrectlyCorresponding(X_topLeft, Y_topLeft, X_botRight, Y_botRight))
                //    correctPoints();
                UpdateWidthHeight();
            }
        }

        public void UpdateWidthHeight()
        {
            Width = X_botRight - X_topLeft;
            Height = Y_botRight - Y_topLeft;
        }

        public void ShiftLeft(int val)
        {
            SetTopLeft(X_topLeft - val, Y_topLeft);
            SetBotRight(X_botRight - val, Y_botRight);
        }

        public void ShiftRight(int val)
        {
            SetTopLeft(X_topLeft + val, Y_topLeft);
            SetBotRight(X_botRight + val, Y_botRight);
        }

        public void ShiftTop(int val)
        {
            SetTopLeft(X_topLeft, Y_topLeft - val);
            SetBotRight(X_botRight, Y_botRight - val);
        }

        public void ShiftBot(int val)
        {
            SetTopLeft(X_topLeft, Y_topLeft + val);
            SetBotRight(X_botRight, Y_botRight + val);
        }

        public void ShiftCornerLeft(Corners corner, int val)
        {
            if (corner == Corners.TopLeft || corner == Corners.BotLeft)
            {
                SetTopLeft(X_topLeft - val, Y_topLeft);
            }
            else if (corner == Corners.TopRight || corner == Corners.BotRight)
            {
                SetBotRight(X_botRight - val, Y_botRight);
            }
        }

        public void ShiftCornerRight(Corners corner, int val)
        {
            if (corner == Corners.TopLeft || corner == Corners.BotLeft)
            {
                SetTopLeft(X_topLeft + val, Y_topLeft);
            }
            else if (corner == Corners.TopRight || corner == Corners.BotRight)
            {
                SetBotRight(X_botRight + val, Y_botRight);
            }
        }

        public void ShiftCornerTop(Corners corner, int val)
        {
            if (corner == Corners.TopLeft || corner == Corners.TopRight)
            {
                SetTopLeft(X_topLeft, Y_topLeft - val);
            }
            else if (corner == Corners.BotRight || corner == Corners.BotLeft)
            {
                SetBotRight(X_botRight, Y_botRight - val);
            }
        }

        public void ShiftCornerBot(Corners corner, int val)
        {
            if (corner == Corners.TopLeft || corner == Corners.TopRight)
            {
                SetTopLeft(X_topLeft, Y_topLeft + val);
            }
            else if (corner == Corners.BotRight || corner == Corners.BotLeft)
            {
                SetBotRight(X_botRight, Y_botRight + val);
            }
        }

        public bool IsThisPointsCoorectlyCorresponding()
        {
            if (X_topLeft <= X_botRight && Y_topLeft <= Y_botRight)
                return true;
            return false;
        }

        //is Top Left actually top left, or bot right
        public static bool AreCornersCorrectlyCorresponding(int X_tl, int Y_tl, int X_br, int Y_br)
        {
            if (X_tl <= X_br && Y_tl <= Y_br)
                return true;
            return false;
        }

        public static bool IsEquals(SelectedArea sa1, SelectedArea sa2)
        {
            if (sa1.X_topLeft == sa2.X_topLeft &&
                sa1.Y_topLeft == sa2.Y_topLeft &&
                sa1.X_botRight == sa2.X_botRight &&
                sa1.Y_botRight == sa2.Y_botRight)
                return true;
            return false;
        }

    }
}
