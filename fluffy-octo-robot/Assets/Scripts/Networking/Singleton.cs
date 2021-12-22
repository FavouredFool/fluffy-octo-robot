using UnityEngine;
using Unity.Netcode;

namespace FluffyRobot.Core.Singeltons {

    public class Singelton<T> : NetworkBehaviour where T: Component {
        private static T _instance;

        public static T Instance {
            get {
                if (_instance == null) {
                    var objs = FindObjectsOfType(typeof(T)) as T[];

                    if (objs.Length > 0) {
                        _instance = objs[0];
                    }

                    if (objs.Length > 1) {
                        Debug.Log("There is more than one " + typeof(T).Name + " in this scene");
                    }

                    if (_instance == null) {
                        GameObject gameObject = new GameObject();
                        gameObject.name = string.Format("_{0}", typeof(T).Name);
                        _instance = gameObject.AddComponent<T>();
                    }
                }

                return _instance;
            }

        }
    } 
}

