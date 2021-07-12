using csPixelGameEngineCore.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace csPixelGameEngineCore
{
    public class KeyboardState
    {
        private readonly Dictionary<Key, bool> keyboardState = new Dictionary<Key, bool>();

        public bool this[Key key] { get => keyboardState[key]; set => keyboardState[key] = value; }

        public bool IsAnyKeyDown { get => keyboardState.Count > 0; }

        public bool IsKeyDown(Key key)
        {
            bool value = false;
            keyboardState.TryGetValue(key, out value);

            return value;
        }

        public bool IsKeyUp(Key key) => !IsKeyUp(key);
    }
}
