using UnityEngine;
using System.Collections;

namespace SVBLM.Core {
	/// <summary>
	/// This class is used as a base for configuring complex game objects. (Something we will do exclusively through code). 	
	/// </summary>
	public abstract class Constructor : MonoBehaviour {
		public T AddOrGetComponent<T>(Transform child = null) where T : Component {
			T component;
			if(child == null) {
				component = GetComponent<T>();
			} else {
				component = child.gameObject.GetComponent<T>();
			}
			
			if(component == null) {
				if(child == null) {
					component = gameObject.AddComponent<T>();
				} else {
					component = child.gameObject.AddComponent<T>();
				}
			} 
			
			return component;
		}
		
		public Transform CreateChildTransform(string name = "New Child Component") {
			GameObject go = (GameObject) Instantiate(new GameObject());
			go.name = name;
			go.transform.parent = transform;
			return go.transform;
		}
		
		public Transform CloneSelfToChild(string name = "New Clone") {
			GameObject go = (GameObject)  Instantiate(gameObject);
			Destroy(go.GetComponent<Constructor>());
			go.name = "New Child Component";
			go.transform.parent = transform;
			return go.transform;
		}
	}
}
