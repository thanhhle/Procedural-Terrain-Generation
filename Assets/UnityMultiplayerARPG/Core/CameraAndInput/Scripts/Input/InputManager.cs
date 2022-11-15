﻿using System.Collections.Generic;
using UnityEngine;
#if USE_REWIRED
using Rewired;
#endif

public static class InputManager
{
    private static Dictionary<string, SimulateButton> simulateInputs = new Dictionary<string, SimulateButton>();
    private static Dictionary<string, SimulateAxis> simulateAxis = new Dictionary<string, SimulateAxis>();
    public static bool useMobileInputOnNonMobile = false;

    public static bool HasInputSetting(string keyName)
    {
        return InputSettingManager.Singleton != null && InputSettingManager.Singleton.Settings.ContainsKey(keyName);
    }

#if USE_REWIRED
    public static float GetAxis(string name, bool raw, int playerId = 0)
#else
    public static float GetAxis(string name, bool raw)
#endif
    {
        // Try get input by rewired system
#if USE_REWIRED
        try
        {
            Player player = ReInput.players.GetPlayer(playerId);
            return raw ? player.GetAxisRaw(name) : player.GetAxis(name);
        }
        catch { }
#endif

        if (useMobileInputOnNonMobile || Application.isMobilePlatform)
        {
            SimulateAxis foundSimulateAxis;
            float axis = 0f;
            if (simulateAxis.TryGetValue(name, out foundSimulateAxis))
                axis = foundSimulateAxis.Value;
            if (raw)
            {
                if (axis > 0f)
                    axis = 1f;
                if (axis < 0f)
                    axis = -1f;
            }
            return axis;
        }

        try
        {
            return raw ? Input.GetAxisRaw(name) : Input.GetAxis(name);
        }
        catch { }
        return 0f;
    }

#if USE_REWIRED
    public static bool GetButton(string name, int playerId = 0)
#else
    public static bool GetButton(string name)
#endif
    {
        // Try get input by rewired system
#if USE_REWIRED
        try
        {
            Player player = ReInput.players.GetPlayer(playerId);
            return player.GetButton(name);
        }
        catch { }
#endif

        if (useMobileInputOnNonMobile || Application.isMobilePlatform)
        {
            SimulateButton foundSimulateButton;
            return simulateInputs.TryGetValue(name, out foundSimulateButton) && foundSimulateButton.Pressed;
        }

        if (HasInputSetting(name))
        {
            List<KeyCode> keyCodes = InputSettingManager.Singleton.Settings[name];
            foreach (KeyCode keyCode in keyCodes)
            {
                if (Input.GetKey(keyCode))
                    return true;
            }
        }

        try
        {
            return Input.GetButton(name);
        }
        catch { }
        return false;
    }

#if USE_REWIRED
    public static bool GetButtonDown(string name, int playerId = 0)
#else
    public static bool GetButtonDown(string name)
#endif
    {
        // Try get input by rewired system
#if USE_REWIRED
        try
        {
            Player player = ReInput.players.GetPlayer(playerId);
            return player.GetButtonDown(name);
        }
        catch { }
#endif

        if (useMobileInputOnNonMobile || Application.isMobilePlatform)
        {
            SimulateButton foundSimulateButton;
            return simulateInputs.TryGetValue(name, out foundSimulateButton) && foundSimulateButton.ButtonDown;
        }

        if (HasInputSetting(name))
        {
            List<KeyCode> keyCodes = InputSettingManager.Singleton.Settings[name];
            foreach (KeyCode keyCode in keyCodes)
            {
                if (Input.GetKeyDown(keyCode))
                    return true;
            }
        }

        try
        {
            return Input.GetButtonDown(name);
        }
        catch { }
        return false;
    }

#if USE_REWIRED
    public static bool GetButtonUp(string name, int playerId = 0)
#else
    public static bool GetButtonUp(string name)
#endif
    {
        // Try get input by rewired system
#if USE_REWIRED
        try
        {
            Player player = ReInput.players.GetPlayer(playerId);
            return player.GetButtonUp(name);
        }
        catch { }
#endif

        if (useMobileInputOnNonMobile || Application.isMobilePlatform)
        {
            SimulateButton foundSimulateButton;
            return simulateInputs.TryGetValue(name, out foundSimulateButton) && foundSimulateButton.ButtonUp;
        }

        if (HasInputSetting(name))
        {
            List<KeyCode> keyCodes = InputSettingManager.Singleton.Settings[name];
            foreach (KeyCode keyCode in keyCodes)
            {
                if (Input.GetKeyUp(keyCode))
                    return true;
            }
        }

        try
        {
            return Input.GetButtonUp(name);
        }
        catch { }
        return false;
    }

    public static void SetButtonDown(string name)
    {
        if (!simulateInputs.ContainsKey(name))
        {
            simulateInputs.Add(name, new SimulateButton());
        }
        simulateInputs[name].Press();
    }

    public static void SetButtonUp(string name)
    {
        if (!simulateInputs.ContainsKey(name))
        {
            simulateInputs.Add(name, new SimulateButton());
        }
        simulateInputs[name].Release();
    }

    public static void SetAxisPositive(string name)
    {
        if (!simulateAxis.ContainsKey(name))
        {
            simulateAxis.Add(name, new SimulateAxis());
        }
        simulateAxis[name].Update(1f);
    }

    public static void SetAxisNegative(string name)
    {
        if (!simulateAxis.ContainsKey(name))
        {
            simulateAxis.Add(name, new SimulateAxis());
        }
        simulateAxis[name].Update(-1f);
    }

    public static void SetAxisZero(string name)
    {
        if (!simulateAxis.ContainsKey(name))
        {
            simulateAxis.Add(name, new SimulateAxis());
        }
        simulateAxis[name].Update(0);
    }

    public static void SetAxis(string name, float value)
    {
        if (!simulateAxis.ContainsKey(name))
        {
            simulateAxis.Add(name, new SimulateAxis());
        }
        simulateAxis[name].Update(value);
    }

    public static Vector3 MousePosition()
    {
        return Input.mousePosition;
    }

    public class SimulateButton
    {
        private int lastPressedFrame = -5;
        private int releasedFrame = -5;

        public bool Pressed { get; private set; }

        public void Press()
        {
            if (Pressed)
                return;
            Pressed = true;
            lastPressedFrame = Time.frameCount;
        }

        public void Release()
        {
            Pressed = false;
            releasedFrame = Time.frameCount;
        }

        public bool ButtonDown
        {
            get { return lastPressedFrame - Time.frameCount == -1; }
        }

        public bool ButtonUp
        {
            get { return releasedFrame == Time.frameCount - 1; }
        }
    }

    public class SimulateAxis
    {
        public float Value { get; private set; }
        public void Update(float value)
        {
            Value = value;
        }
    }
}
