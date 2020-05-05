// temp classs f
namespace BlockSlider
{
    public class GlobalState
    {
        private static GlobalState _globalState;
        public static GlobalState Instance
        {
            get
            {
                if(_globalState == null)
                    _globalState = new GlobalState();

                return _globalState;
            }

        }

        public Board Board;
    }


}