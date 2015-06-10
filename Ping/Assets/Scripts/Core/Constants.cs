using UnityEngine;
using System.Collections;

namespace SVBLM.Core {
	public static class Constants {
		public static string GAME_TITLE = "Lean";
		
		#region environment globals
		public static float GRAVITY = -0.4f;
		#endregion
		
		#region player locamotion
		public static float CHARACTER_HEIGHT = 1.8f;
		public static float CHARACTER_RADIUS = 0.5f;
		public static float CHARACTER_SKIN_WIDTH = 0.1f;
		public static float CHARACTER_STEP_HEIGHT = 0.1f;
		public static float CHARACTER_MAX_SLOPE = 45f;
		public static float CHARACTER_WALK_ACCELERATION = 3.0f;
		public static float CHARACTER_MOUSE_LOOK_ACCELERATION = 2.0f;
		public static float CHARACTER_JUMP_SPEED = 0.2f;
		public static float CHARACTER_TERMINAL_VELOCITY = 1.0f;
		public static float CHARACTER_LANDING_ROTATION_AMOUNT = 0.001f;
		public static float CHARACTER_LOOK_MAXIMUM_PITCH = 90f;
		public static float CHARACTER_MAX_HEAD_BOB_FREQUENCY = 0.2f;
		public static float CHARACTER_MAX_HEAD_BOB_AMOUNT = 0.5f;
		public static float CHARACTER_SPEED_FOR_MAX_HEAD_BOB = 10.0f;
		public static float CHARACTER_WALL_SLIDE_DETECT_DISTANCE = 0.5f;
		public static float CHARACTER_WALL_SLIDE_MAX_ANGLE_TOLERANCE = 30f;
		public static float CHARACTER_WALL_SLIDE_MIN_VELOCITY = 0.1f;
		public static float CHARACTER_WALL_SLIDE_MIN_ENTRY_VELOCITY = 0.8f;
		public static float CHARACTER_WALL_SLIDE_TIMEOUT = 0.2f;
		public static float CHARACTER_MOUSE_LOOK_ROLL_AMOUNT = 0.1f;
		#endregion
		
		#region lerp
		public static float LERP_FAST = 0.3f;
		public static float LERP_MEDIUM = 0.2f;
		public static float LERP_SLOW = 0.05f;
		public static float LERP_VERY_SLOW = 0.005f;
		#endregion
	}
}
