// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

#nullable disable

using System;
using System.Linq;
using SDL;

namespace osu.Framework.Platform.SDL2
{
    /// <summary>
    /// Maintain a copy of the SDL-provided bindings for the given controller.
    /// Used to determine whether a given event's joystick button or axis is unmapped.
    /// </summary>
    internal class SDL2ControllerBindings
    {
        public readonly IntPtr JoystickHandle;
        public readonly IntPtr ControllerHandle;

        /// <summary>
        /// Bindings returned from <see cref="SDL_GameControllerGetBindForButton"/>, indexed by <see cref="SDL_GamepadButton"/>.
        /// Empty if the joystick does not have a corresponding ControllerHandle.
        /// </summary>
        public SDL_GameControllerButtonBind[] ButtonBindings;

        /// <summary>
        /// Bindings returned from <see cref="SDL_GameControllerGetBindForAxis"/>, indexed by <see cref="SDL_GamepadAxis"/>.
        /// Empty if the joystick does not have a corresponding ControllerHandle.
        /// </summary>
        public SDL_GameControllerButtonBind[] AxisBindings;

        public SDL2ControllerBindings(IntPtr joystickHandle, IntPtr controllerHandle)
        {
            JoystickHandle = joystickHandle;
            ControllerHandle = controllerHandle;

            PopulateBindings();
        }

        public void PopulateBindings()
        {
            if (ControllerHandle == IntPtr.Zero)
            {
                ButtonBindings = Array.Empty<SDL_GameControllerButtonBind>();
                AxisBindings = Array.Empty<SDL_GameControllerButtonBind>();
                return;
            }

            ButtonBindings = Enumerable.Range(0, (int)SDL_GamepadButton.SDL_GAMEPAD_BUTTON_MAX)
                                       .Select(i => SDL_GameControllerGetBindForButton(ControllerHandle, (SDL_GamepadButton)i)).ToArray();

            AxisBindings = Enumerable.Range(0, (int)SDL_GamepadAxis.SDL_GAMEPAD_AXIS_MAX)
                                     .Select(i => SDL_GameControllerGetBindForAxis(ControllerHandle, (SDL_GamepadAxis)i)).ToArray();
        }

        public bool IsJoystickButtonBound(byte buttonIndex)
        {
            for (int i = 0; i < ButtonBindings.Length; i++)
            {
                if (ButtonBindings[i].bindType != SDL_GamepadBindingType.SDL_GAMEPAD_BINDTYPE_NONE && ButtonBindings[i].value.button == buttonIndex)
                    return true;
            }

            return false;
        }

        public bool IsJoystickAxisBound(byte axisIndex)
        {
            for (int i = 0; i < AxisBindings.Length; i++)
            {
                if (AxisBindings[i].bindType != SDL_GamepadBindingType.SDL_GAMEPAD_BINDTYPE_NONE && AxisBindings[i].value.axis == axisIndex)
                    return true;
            }

            return false;
        }
    }
}
