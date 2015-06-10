using UnityEngine;
using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Reflection;

namespace SVBLM.Core {
	public class FSMDelegate : FSM {
		private Action doneCallback;
		public FSM delegator;

		public void OnDone(Action callback) {
			doneCallback = callback;
		}

		void OnDestroy() {
			doneCallback ();
		}
	}
}
