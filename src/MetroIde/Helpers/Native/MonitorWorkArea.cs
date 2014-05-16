using System;
using System.Runtime.InteropServices;

namespace MetroIde.Helpers.Native
{
    internal static class MonitorWorkArea
    {
        [DllImport("user32")]
        internal static extern bool GetMonitorInfo(IntPtr hMonitor, MonitorInfo lpmi);

        [DllImport("User32")]
        internal static extern IntPtr MonitorFromWindow(IntPtr handle, int flags);

        [StructLayout(LayoutKind.Sequential)]
        public struct MinMaxInfo
        {
            public Point ptReserved;
            public Point ptMaxSize;
            public Point ptMaxPosition;
            public Point ptMinTrackSize;
            public Point ptMaxTrackSize;
        };

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
        public class MonitorInfo
        {
            public MonitorInfo()
            {
                cbSize = (uint) Marshal.SizeOf(typeof (MonitorInfo));
            }

            public uint cbSize;
            public Rect rcMonitor = new Rect();
            public Rect rcWork = new Rect();
            public uint dwFlags;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct Point
        {
            public int x;
            public int y;
        }

        [StructLayout(LayoutKind.Sequential, Pack = 0)]
        public struct Rect
        {
            public readonly int left;
            public readonly int top;
            public readonly int right;
            public readonly int bottom;
            public static readonly Rect Empty = new Rect();

            public int Width
            {
                get { return Math.Abs(right - left); } // Abs needed for BIDI OS
            }

            public int Height
            {
                get { return bottom - top; }
            }

            public Rect(int left, int top, int right, int bottom)
            {
                this.left = left;
                this.top = top;
                this.right = right;
                this.bottom = bottom;
            }

            public Rect(Rect rcSrc)
            {
                left = rcSrc.left;
                top = rcSrc.top;
                right = rcSrc.right;
                bottom = rcSrc.bottom;
            }

            public bool IsEmpty
            {
                get
                {
                    // TODO Bug on Bidi OS (hebrew arabic) left > right
                    return left >= right || top >= bottom;
                }
            }

            /// <summary> Return a user friendly representation of this struct </summary>
            public override string ToString()
            {
                if (this == Empty)
                {
                    return "Rect {Empty}";
                }
                return "Rect { left : " + left + " / top : " + top + " / right : " + right + " / bottom : " + bottom +
                       " }";
            }

            /// <summary> Determine if 2 Rect are equal (deep compare) </summary>
            public override bool Equals(object obj)
            {
                if (obj is Rect)
                {
                    return (this == (Rect) obj);
                }
                return false;

            }

            /// <summary>Return the HashCode for this struct (not garanteed to be unique)</summary>
            public override int GetHashCode()
            {
                return left.GetHashCode() + top.GetHashCode() + right.GetHashCode() + bottom.GetHashCode();
            }

            /// <summary> Determine if 2 Rect are equal (deep compare)</summary>
            public static bool operator ==(Rect rect1, Rect rect2)
            {
                return (rect1.left == rect2.left && rect1.top == rect2.top && rect1.right == rect2.right &&
                        rect1.bottom == rect2.bottom);
            }

            /// <summary> Determine if 2 Rect are different(deep compare)</summary>
            public static bool operator !=(Rect rect1, Rect rect2)
            {
                return !(rect1 == rect2);
            }
        }
    }
}