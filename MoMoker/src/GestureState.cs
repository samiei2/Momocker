using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoMoker.src
{
    public class GestureState
    {
        long timeStamp = 0;

        public GestureState(long v)
        {
            this.TimeStamp = v;
        }

        public long TimeStamp
        {
            get
            {
                return timeStamp;
            }

            set
            {
                timeStamp = value;
            }
        }
    }

    public class MouseGestureState : GestureState
    {
        internal System.Drawing.Point oldCursorPosition;
        internal SpacePoint oldHandPosition;

        public MouseGestureState(long v):base(v)
        {
            
        }
    }
}
