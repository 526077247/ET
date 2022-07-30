using UnityEngine;

namespace ET
{
    public static class InputHelper
    {
        public static bool GetKeyDown(int code)
        {
            return Input.GetKeyDown((KeyCode) code);
        }
        public static bool GetKey(int code)
        {
            return Input.GetKey((KeyCode) code);
        }
        public static bool GetKeyUp(int code)
        {
            return Input.GetKeyUp((KeyCode) code);
        }

        public static bool GetMouseButtonDown(int code)
        {
            return Input.GetMouseButtonDown(code);
        }
    }
}