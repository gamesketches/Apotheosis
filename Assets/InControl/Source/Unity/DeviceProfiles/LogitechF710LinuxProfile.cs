using System;


namespace InControl
{
	// @cond nodoc
	[AutoDiscover]
	public class LogitechF710LinuxProfile : UnityInputDeviceProfile
	{
		public LogitechF710LinuxProfile()
		{
			Name = "Logitech F710 Controller";
			Meta = "Logitech F710 Controller on Linux";

			IncludePlatforms = new[] {
				"Linux"
			};

			JoystickNames = new[] {
				"Logitech Gamepad F710"
			};

			ButtonMappings = new[] {
				new InputControlMapping {
					Handle = "A",
					Target = InputControlType.Action1,
					Source = Button0
				},
				new InputControlMapping {
					Handle = "B",
					Target = InputControlType.Action2,
					Source = Button1
				},
				new InputControlMapping {
					Handle = "X",
					Target = InputControlType.Action3,
					Source = Button2
				},
				new InputControlMapping {
					Handle = "Y",
					Target = InputControlType.Action4,
					Source = Button3
				},
				new InputControlMapping {
					Handle = "Left Bumper",
					Target = InputControlType.LeftBumper,
					Source = Button4
				},
				new InputControlMapping {
					Handle = "Right Bumper",
					Target = InputControlType.RightBumper,
					Source = Button5
				},
				new InputControlMapping {
					Handle = "Left Stick Button",
					Target = InputControlType.LeftStickButton,
					Source = Button9
				},
				new InputControlMapping {
					Handle = "Right Stick Button",
					Target = InputControlType.RightStickButton,
					Source = Button10
				},
				new InputControlMapping {
					Handle = "Start",
					Target = InputControlType.Start,
					Source = Button7
				},
				new InputControlMapping {
					Handle = "Back",
					Target = InputControlType.Select,
					Source = Button6
				}
			};

			AnalogMappings = new[] {
				LeftStickLeftMapping( Analog0 ),
				LeftStickRightMapping( Analog0 ),
				LeftStickUpMapping( Analog1 ),
				LeftStickDownMapping( Analog1 ),

				RightStickLeftMapping( Analog3 ),
				RightStickRightMapping( Analog3 ),
				RightStickUpMapping( Analog4 ),
				RightStickDownMapping( Analog4 ),

				new InputControlMapping {
					Handle = "Left Trigger",
					Target = InputControlType.LeftTrigger,
					Source = Analog2,
				},
				new InputControlMapping {
					Handle = "Right Trigger",
					Target = InputControlType.RightTrigger,
					Source = Analog5,
				},

				DPadLeftMapping( Analog6 ),
				DPadRightMapping( Analog6 ),
				DPadUpMapping( Analog7 ),
				DPadDownMapping( Analog7 ),
			};
		}
	}
	// @endcond
}

